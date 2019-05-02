//tabs=4
// --------------------------------------------------------------------------------
// ASCOM ObservingConditions driver for mySQMPRO
//
// Implements:	ASCOM ObservingConditions interface version: 1.0.4
// Author:		Robert Brown brown_rb@yahoo.com
//
// 1.0.4  29042019
// Bug fixes

// 1.0.3 02042018
// Fix to SQM value

// 1.0.2 18012018
// Changes to Windows locate - eng-us
// Changes to serial comms

// 1.0.1 27102017
// Enable ASCOM trace logging
//
// 6.0.0 20/10/2017 
// Initial release
// trace disabled due to my system not having permissions on ascom folder in documents

/*
 * Send	Return Code		
:01#	A		returns magnitude value (as float) - can be up to 10s delay
:02#  	B		returns frequency
:03#	C		returns Irradiance value uW/cm2
:04#	D		returns firmware version number
:05#	E		returns firmware filename
:06#	F		returns lightvalue from LDR
:07#	G		returns period gate time
:19#	O		Get IR Sensor object temperature
:20#	P		Get IR Sensor ambienttemperature
:21#	U		Get LUX value
:23#	R		get Raining (boolean)
:24#	S		get raining analogue value (int)
:25xx#			set setpoint1 - sent on connect (client must limit to 99 to -99)
:26xx#			set setpoint2 - sent on connect (client must limit to 99 to -99)
:27#	V		get cloudstate, 1=clear, 2=partly cloudy, 3=cloudy
:28#	W		get setpoint1
:29#	X		get setpoint2
:31#	Z		get NELM
:32#	a		get BME820 humidity (app can calculate dew point from temp and humidity)
:33#	e/b		get BME820 pressure
:34#	c		get BME820 temperature
:35#	d		get BME280 dewpoint
 */

#define ObservingConditions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;

namespace ASCOM.mySQMPRO
{
    //
    // Your driver's DeviceID is ASCOM.mySQMPRO.ObservingConditions
    //
    // The Guid attribute sets the CLSID for ASCOM.mySQMPRO.ObservingConditions
    // The ClassInterface/None addribute prevents an empty interface called
    // _mySQMGPSPRO from being created and used as the [default] interface

    /// <summary>
    /// ASCOM ObservingConditions Driver for mySQMPRO.
    /// </summary>
    [Guid("d42d4afc-26ea-455f-a082-be9336b6ba21")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ObservingConditions : IObservingConditions
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        public const string driverID = "ASCOM.mySQMPRO.ObservingConditions";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        public const string driverDescription = "mySQMPROASCOM";
        public const string driverInfo = "Arduino mySQMPROASCOM driver by RB Brown 2017.";
        public const string driverVersion = "1.0.4";

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Private variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        private TraceLogger tl;

        /// <summary>
        /// Private variable to hold serial port
        /// </summary>
        private Serial objSerial = new Serial();

        // defines for defining the time of the next call for each method/property request
        // this is used to decide whether to return a cached value or not
        public static Mutex Sermutex = new Mutex();
        public static DateTime DewPointNextCall;
        public static DateTime HumidityNextCall;
        public static DateTime PressureNextCall;
        public static DateTime SkyBrightnessNextCall;
        public static DateTime SkyQualityNextCall;
        public static DateTime SkyTemperatureNextCall;
        public static DateTime TemperatureNextCall;
        public static TimeSpan Delay_Between_Calls = TimeSpan.FromMilliseconds(900);

        // defines for windows locale
        public CultureInfo thisCulture;
        public CultureInfo thisICCulture;
        public static CultureInfo newCulture;
        public static CultureInfo newICCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="mySQMPRO"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public ObservingConditions()
        {
            tl = new TraceLogger("", "mySQMPRO");
            tl.Enabled = Properties.Settings.Default.TraceEnabled;
            tl.LogMessage("ObservingConditions", "Starting initialisation");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object

            // save threads culture
            newCulture = thisCulture = CultureInfo.CurrentCulture;
            newICCulture = thisICCulture = CultureInfo.CurrentUICulture;
            newCulture = CultureInfo.CreateSpecificCulture("en-US");
            newICCulture = CultureInfo.CreateSpecificCulture("en-US");

            tl.LogMessage("ObservingConditions", "Completed initialisation");
        }


        //
        // PUBLIC COM INTERFACE IObservingConditions IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected
            if (IsConnected)
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.Save();
                }
                else
                {
                    //revert to old values - cancel
                    Properties.Settings.Default.Reload();
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                tl.LogMessage("SupportedActions Get", "Returning list...");
                ArrayList supportedActions = new ArrayList();

                supportedActions.Add("DewPoint");  
                supportedActions.Add("Humidity");
                supportedActions.Add("Pressure");
                supportedActions.Add("SkyBrightness");
                supportedActions.Add("SkyQuality");
                supportedActions.Add("SkyTemperature");
                supportedActions.Add("Temperature");
                supportedActions.Add("AveragePeriod");
                return supportedActions;
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        public void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
        }

        public bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            string ret = CommandString(command, raw);
            if (ret == "OK")
            {
                return true;
            }
            else return false;
        }

        // use this to send a command to the controller when a response is required
        // send the command string to the controller and wait for a response
        public string CommandString(string command, bool raw)
        {
            string cmd = command;		// save the command into cmd
            string recbuf = "";			// clear the receive buffer
            string tempstr = "";
            int mypos = 0;

            tl.LogMessage("CommandString: ", command);

            try
            {
                objSerial.ClearBuffers();
                tl.LogMessage("CS:", command);
            }
            catch (NotConnectedException Ex)     // objSerial.ClearBuffers();
            {
                tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
            }
            catch (ASCOM.DriverException Ex)
            {
                tl.LogMessage("CS: ASCOM.DriverException when clearing serial port buffers", "ERROR: Possible serial port disconnect.\n" + Ex);
                throw new ASCOM.DriverException("Cannot connect to controller", Ex);
            }
            catch (Exception Ex)            // objSerial.ClearBuffers();
            {
                tl.LogMessage("CS: Exception", Ex.ToString());
                throw new ASCOM.DriverException("Serial port connection error", Ex);
            }
            try
            {
                objSerial.Transmit(command);
            }

            catch (UnauthorizedAccessException Ex)
            {
                tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
            }
            catch (ASCOM.DriverException Ex)
            {
                tl.LogMessage("CS: ASCOM.DriverException when trying to transmit data", "ERROR: Possible serial port disconnect.\n" + Ex);
                throw new ASCOM.DriverException("Cannot connect to controller", Ex);
            }
            catch (Exception Ex)          // objSerial.Transmit(command);
            {
                tl.LogMessage("CS: Exception", Ex.ToString());
                throw new ASCOM.DriverException("Serial port connection error", Ex);
            }

            // get ambient
            // 34#	cvalue$		Returns BME280 sensor Ambient temperature in C
            if (cmd == ":34#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " raw response recd: " + recbuf);
                    // no need to strip off terminator
                    // update the position
                    // this is ambient #value$
                    if (recbuf == "")
                    {
                        tl.LogMessage("CommandString:", " Null response A#");
                    }
                    else
                    {
                        // recbuff is cambient$, extract just the value
                        tempstr = "";
                        for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                        {
                            tempstr = tempstr + recbuf[mypos];
                        }
                        // this is ambient temp
                        try
                        {
                            Temperaturevalue = Double.Parse(tempstr, newCulture);
                            // controller always returns centigrade values
                            tl.LogMessage("CommandString:", " Temperature=" + Temperaturevalue.ToString(newCulture));
                        }
                        catch (Exception)
                        {
                            tl.LogMessage("CommandString:", " conversion Temperature error");
                        }
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 34# Temperature Timeout exception raised");
                    return recbuf;
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

            // get BME280 humidity
            // 32#	avalue$		Returns Relative Humidity
            else if (cmd == ":32#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    // this is relative humidity
                    try
                    {
                        humidityvalue = Double.Parse(tempstr, newCulture);
                        // controller always returns centigrade values
                        tl.LogMessage("CommandString:", " convert OK humidity = " + humidityvalue.ToString(newCulture));
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion humidity error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 32# Humidity Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

            // get BME280 dewpoint
            // 35#	dvalue$		Returns Dew Point in C
            else if (cmd == ":35#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    try
                    {
                        dewpointvalue = Double.Parse(tempstr, newCulture);
                        tl.LogMessage("CommandString:", " convert OK dewpoint = " + dewpointvalue.ToString(newCulture));
                        // controller always returns centigrade values
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion dewpoint error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 35# Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

            // get BME280 pressure
            // 33#	bvalue$		Returns pressure in hPa (it is returned from controller as unsigned long int)
            else if (cmd == ":33#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    try
                    {
                        pressurevalue = Double.Parse(tempstr, newCulture);
                        tl.LogMessage("CommandString:", " convert OK pressure = " + pressurevalue.ToString(newCulture));
                        // controller always returns centigrade values
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion pressure error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 33# Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

           // get lux - sky brightness
            // 21#	Uvalue$		Returns sky brightness
            else if (cmd == ":21#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: D: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    try
                    {
                        SkyBrightnessvalue = Double.Parse(tempstr, newCulture);
                        tl.LogMessage("CommandString:", " convert OK Sky Brightness = " + SkyBrightnessvalue.ToString(newCulture));
                        // controller always returns centigrade values
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion for response SkyBrightness error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 21# SkyBrightness Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

           // get SQM - Sky Quality
            // 01#	Avalue$		Returns sky brightness
            else if (cmd == ":01#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: D: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    try
                    {
                        SkyQualityvalue = Double.Parse(tempstr, newCulture);
                        tl.LogMessage("CommandString:", " convert OK Sky Quality = " + SkyQualityvalue.ToString(newCulture));
                        // controller always returns centigrade values
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion for response SkyQuality error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 01# SkyQuality Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }

            // get Sky temperature - IR sensor Object
            // 19#	Ovalue$		Returns sky temperature
            else if (cmd == ":19#")
            {
                tempstr = "";
                // try to read response from serial port
                try
                {
                    recbuf = objSerial.ReceiveTerminated("#");

                    tl.LogMessage("CommandString:", " response recd: D: " + recbuf);
                    tempstr = "";
                    for (mypos = 1; Convert.ToChar(recbuf[mypos]) != '#'; mypos++)
                    {
                        tempstr = tempstr + recbuf[mypos];
                    }
                    try
                    {
                        SkyTemperaturevalue = Double.Parse(tempstr, newCulture);
                        tl.LogMessage("CommandString:", " convert OK Sky Quality = " + SkyTemperaturevalue.ToString(newCulture));
                        // controller always returns centigrade values
                    }
                    catch (Exception)
                    {
                        tl.LogMessage("CommandString:", " conversion for response SkyTemperature error");
                    }
                }
                // handle exception
                catch (TimeoutException)
                {
                    tl.LogMessage("CommandString:", " 19# SkyTemperature Timeout exception raised");
                }
                catch (UnauthorizedAccessException Ex)
                {
                    tl.LogMessage("CS: NotConnectedException", Ex.ToString());
                    throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("CS: Exception", Ex.ToString());
                    throw new ASCOM.DriverException("Serial port connection error", Ex);
                }
            }
            // other commands here
            else     // invalid command received
            {
                tl.LogMessage("CS: Invalid Command", cmd);
                recbuf = "";
                tl.LogMessage("CS: Recvd", recbuf);
                throw new ASCOM.InvalidValueException("Invalid command Received=" + cmd);
            }
            // recbuf holds response or NULL string
            // response is char indictating cmd then parameter
            if (recbuf == "")
            {
                tl.LogMessage("CS:", "Null value returned");
                return recbuf;
            }
            else
            {
                tl.LogMessage("CS:", recbuf.Substring(1, recbuf.Length - 1));
                return recbuf.Substring(1, recbuf.Length - 1); 	// all except start character
                // return recbuf.Substring(1, recbuf.Length - 2); 	// all except start character
                // tl.LogMessage("CommandString 7", recbuf.Substring(1, recbuf.Length - 2));
            }
        }

        public void Dispose()
        {
            // Clean up the tracelogger and util objects
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public void PauseForTime(int myseconds, int mymseconds)
        {
            System.DateTime ThisMoment = System.DateTime.Now;
            System.TimeSpan duration = new System.TimeSpan(0, 0, 0, myseconds, mymseconds);
            // System.TimeSpan( days, hrs, mins, secs, millisecs);
            System.DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = System.DateTime.Now;
            }
        }

        public bool Connected
        {
            get
            {
                tl.LogMessage("Connected Start:", IsConnected.ToString());
                if (objSerial == null)
                    return false;
                return objSerial.Connected;
            }
            set
            {
                tl.LogMessage("Connected", value.ToString());
                if (value == IsConnected)
                    return;

                try
                {
                    if (value)
                    {
                        // this is to calculate how long it takes to connect
                        tl.LogMessage("START", "Connect Started");

                        // connect to the device
                        tl.LogMessage("Connected", "Connecting to port " + Properties.Settings.Default.MyComPort);
                        objSerial.PortName = Properties.Settings.Default.MyComPort;
                        if (string.IsNullOrEmpty(objSerial.PortName))
                        {
                            tl.LogMessage("Connected", "Cannot connect to COM port: Null");
                            // report a problem with the port name 
                            throw new ASCOM.NotConnectedException("no Com port selected");
                        }

                        switch (Properties.Settings.Default.ComPortSpeed)
                        {
                            case 9600: objSerial.Speed = SerialSpeed.ps9600; break;
                            case 14400: objSerial.Speed = SerialSpeed.ps14400; break;
                            case 19200: objSerial.Speed = SerialSpeed.ps19200; break;
                            case 28800: objSerial.Speed = SerialSpeed.ps28800; break;
                            case 38400: objSerial.Speed = SerialSpeed.ps38400; break;
                            case 57600: objSerial.Speed = SerialSpeed.ps57600; break;
                            case 115200: objSerial.Speed = SerialSpeed.ps115200; break;
                            default: objSerial.Speed = SerialSpeed.ps9600; break;
                        }

                        tl.LogMessage("Setting baudrate to ", Properties.Settings.Default.ComPortSpeed.ToString());

                        if (Properties.Settings.Default.ResetControllerOnConnect == true)
                        {
                            objSerial.DTREnable = true;
                            objSerial.RTSEnable = true;
                        }
                        else
                        {
                            objSerial.DTREnable = false;
                            objSerial.RTSEnable = false;
                        }
                        // objSerial.DataBits = 8;
                        // objSerial.Parity =  SerialParity.None
                        objSerial.ReceiveTimeout = 5;
                        objSerial.ReceiveTimeoutMs = 5000;
                        // try to connect
                        try
                        {
                            objSerial.Connected = true;
                        }
                        catch (UnauthorizedAccessException Ex)
                        {
                            tl.LogMessage("Connected NotConnectedException", Ex.ToString());
                            throw new ASCOM.NotConnectedException("Serial port connection error", Ex);
                        }
                        catch (Exception Ex)
                        {
                            // report any error 
                            tl.LogMessage("Connected", "Cannot connect to COM port: Missing?");
                            objSerial.Connected = false;
                            throw new ASCOM.DriverException("Serial port connection error", Ex);
                        }
                        if (objSerial.Connected == true)
                        {
                            connectedState = true;

                            objSerial.ClearBuffers();
                        }
                    }
                    else
                    {
                        // Serial port not connected - disconnect from device
                        objSerial.Connected = false;
                        connectedState = false;
                        tl.LogMessage("Connected", "Disconnecting from port " + Properties.Settings.Default.MyComPort);
                        Properties.Settings.Default.Save();
                        objSerial.DTREnable = false;
                        objSerial.RTSEnable = false;
                        objSerial.Dispose();
                    } // end of if(value) 
                    // this is to calculate how long it takes to connect
                }
                catch (ASCOM.DriverException Ex)
                {
                    tl.LogMessage("Connected", "ASCOM.Driver Exception ERROR- if( value ): Possible serial port disconnect.\n" + Ex);
                    throw new ASCOM.DriverException("Cannot connect to controller", Ex);
                }
                catch (System.InvalidOperationException Ex)
                {
                    tl.LogMessage("Connected", "System.InvalidOperationException ERROR- if( value ): Possible serial port disconnect.\n" + Ex);
                    throw new ASCOM.DriverException("Cannot connect to controller", Ex);
                }
                catch (Exception Ex)
                {
                    tl.LogMessage("Connected", "Exception ERROR- if( value ): Possible serial port disconnect.\n" + Ex);
                    throw new ASCOM.DriverException("Cannot connect to controller", Ex);
                }
                tl.LogMessage("START", "Connect Finished");
            } // end of set
        }

        public string Description
        {
            get
            {
                tl.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                // Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // string driverInfo = "Arduino myFocuser2 driver by RBB. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                // Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                tl.LogMessage("InterfaceVersion Get", "1");
                return Convert.ToInt16("1");
            }
        }

        public string Name
        {
            get
            {
                string name = "mySQMPRO ASCOM driver";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IObservingConditions Implementation

        /// <summary>
        /// Gets and sets the time period over which observations wil be averaged
        /// </summary>
        /// <remarks>
        /// Get must be implemented, if it can't be changed it must return 0
        /// Time period (hours) over which the property values will be averaged 0.0 =
        /// current value, 0.5= average for the last 30 minutes, 1.0 = average for the
        /// last hour
        /// </remarks>
        /// 
        public double humidityvalue = 0;       // relative humidity reading
        public double dewpointvalue = 0;
        public double pressurevalue = 0;
        public double SkyBrightnessvalue = 0;
        public double SkyQualityvalue = 0;
        public double SkyTemperaturevalue = 0;
        public double Temperaturevalue = 0;

        public double AveragePeriod
        {
            get
            {
                tl.LogMessage("AveragePeriod", "get - 0");
                return 0;
            }
            set
            {
                LogMessage("AveragePeriod", "set - {0}", value);
                if (value != 0)
                    throw new PropertyNotImplementedException("AveragePeriod", true);
            }
        }

        /// <summary>
        /// Amount of sky obscured by cloud
        /// </summary>
        /// <remarks>0%= clear sky, 100% = 100% cloud coverage</remarks>
        public double CloudCover
        {
            get
            {
                tl.LogMessage("CloudCover", "get - not implemented");
                throw new PropertyNotImplementedException("CloudCover", false);
            }
        }

        /// <summary>
        /// Atmospheric dew point at the observatory in deg C
        /// </summary>
        /// <remarks>
        /// Normally optional but mandatory if <see cref=" ASCOM.DeviceInterface.IObservingConditions.Humidity"/>
        /// Is provided
        /// </remarks>
        public double DewPoint
        {
            get
            {
                String ret = "";
                tl.LogMessage("DewPoint", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= DewPointNextCall)
                    {
                        DewPointNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("DewPoint", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":35#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("DewPoint", "Mutex released");
                        if (ret != "")
                        {
                            return dewpointvalue;
                        }
                        else
                            return 0.0;
                    }
                    else
                        return dewpointvalue;

                }
                return dewpointvalue;
            }
        }

        /// <summary>
        /// Atmospheric relative humidity at the observatory in percent
        /// </summary>
        /// <remarks>
        /// Normally optional but mandatory if <see cref="ASCOM.DeviceInterface.IObservingConditions.DewPoint"/> 
        /// Is provided
        /// </remarks>
        public double Humidity
        {
            get
            {
                String ret = "";
                // :32#
                tl.LogMessage("Humidity", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= HumidityNextCall)
                    {
                        HumidityNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("Humidity", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":32#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("Humidity", "Mutex released");
                        if (ret != "")
                        {
                            return humidityvalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return humidityvalue;

                }
                return humidityvalue;
            }
        }

        /// <summary>
        /// Atmospheric pressure at the observatory in hectoPascals (mB)
        /// </summary>
        /// <remarks>
        /// This must be the pressure at the observatory and not the "reduced" pressure
        /// at sea level. Please check whether your pressure sensor delivers local pressure
        /// or sea level pressure and adjust if required to observatory pressure.
        /// </remarks>
        public double Pressure
        {
            get
            {
                String ret = "";
                // :33#
                tl.LogMessage("Pressure", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= PressureNextCall)
                    {
                        PressureNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("Pressure", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":33#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("Pressure", "Mutex released");
                        if (ret != "")
                        {
                            return pressurevalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return pressurevalue;

                }
                return pressurevalue;
            }
        }

        /// <summary>
        /// Rain rate at the observatory
        /// </summary>
        /// <remarks>
        /// This property can be interpreted as 0.0 = Dry any positive nonzero value
        /// = wet.
        /// </remarks>
        public double RainRate
        {
            get
            {
                tl.LogMessage("RainRate", "get - not implemented");
                throw new PropertyNotImplementedException("RainRate", false);
            }
        }

        /// <summary>
        /// Forces the driver to immediatley query its attached hardware to refresh sensor
        /// values
        /// </summary>
        public void Refresh()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Provides a description of the sensor providing the requested property
        /// </summary>
        /// <param name="PropertyName">Name of the property whose sensor description is required</param>
        /// <returns>The sensor description string</returns>
        /// <remarks>
        /// PropertyName must be one of the sensor properties, 
        /// properties that are not implemented must throw the MethodNotImplementedException
        /// </remarks>
        public string SensorDescription(string PropertyName)
        {
            switch (PropertyName)
            {
                case "AveragePeriod":
                    return "Average period in hours, immediate values are only available";
                case "DewPoint":
                    return "Calculated dewpoint from BME280 sensor";
                case "Humidity":
                    return "Relative Humdity BME280 sensor";
                case "Pressure":
                    return "Atmospheric Pressure BME280 sensor";
                case "SkyBrightness":
                    return "Sky brightness in Lux";
                case "SkyQuality":
                    return "SQM reading from light sensor";
                case "SkyTemperature":
                    return "IR Sky temperature from MLX90614 sensor";
                case "Temperature":
                    return "Ambient temperature from BME280 sensor";
                case "RainRate":
                case "StarFWHM":
                case "WindDirection":
                case "WindGust":
                case "WindSpeed":
                    tl.LogMessage("SensorDescription", PropertyName + " - not implemented");
                    throw new MethodNotImplementedException("SensorDescription(" + PropertyName + ")");
                default:
                    tl.LogMessage("SensorDescription", PropertyName + " - unrecognised");
                    throw new ASCOM.InvalidValueException("SensorDescription(" + PropertyName + ")");
            }
        }

        /// <summary>
        /// Sky brightness at the observatory
        /// </summary>
        public double SkyBrightness
        {
            get
            {
                String ret = "";
                // :21#
                tl.LogMessage("SkyBrightness", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= SkyBrightnessNextCall)
                    {
                        SkyBrightnessNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("SkyBrightness", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":21#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("SkyBrightness", "Mutex released");
                        if (ret != "")
                        {
                            return SkyBrightnessvalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return SkyBrightnessvalue;

                }
                return SkyBrightness;
            }
        }

        /// <summary>
        /// Sky quality at the observatory
        /// </summary>
        public double SkyQuality
        {
            get
            {
                String ret = "";
                // :01#
                tl.LogMessage("SkyQuality", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= SkyQualityNextCall)
                    {
                        SkyQualityNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("SkyQuality", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":01#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("SkyQuality", "Mutex released");
                        if (ret != "")
                        {
                            return SkyQualityvalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return SkyQualityvalue;

                }
                return SkyQuality;
            }
        }

        /// <summary>
        /// Seeing at the observatory
        /// </summary>
        public double StarFWHM
        {
            get
            {
                tl.LogMessage("StarFWHM", "get - not implemented");
                throw new PropertyNotImplementedException("StarFWHM", false);
            }
        }

        /// <summary>
        /// Sky temperature at the observatory in deg C
        /// </summary>
        public double SkyTemperature
        {
            get
            {
                String ret = "";
                // :19#
                tl.LogMessage("SkyTemperature", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= SkyTemperatureNextCall)
                    {
                        SkyTemperatureNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("SkyTemperature", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":19#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("SkyTemperature", "Mutex released");
                        if (ret != "")
                        {
                            return SkyTemperaturevalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return SkyTemperaturevalue;

                }
                return SkyTemperaturevalue;
            }
        }

        /// <summary>
        /// Temperature at the observatory in deg C
        /// </summary>
        public double Temperature
        {
            get
            {
                String ret = "";
                // :34#
                tl.LogMessage("Temperature", "Calling controller");
                if (Connected)
                {
                    if (DateTime.Now >= TemperatureNextCall)
                    {
                        TemperatureNextCall = System.DateTime.Now + Delay_Between_Calls;
                        tl.LogMessage("Temperature", "Waiting on Mutex");
                        Sermutex.WaitOne();                             // wait till mutex is free
                        ret = CommandString(":34#", true);
                        Sermutex.ReleaseMutex();                        // Release the Mutex
                        tl.LogMessage("Temperature", "Mutex released");
                        if (ret != "")
                        {
                            return Temperaturevalue;
                        }
                        else
                            return 0;
                    }
                    else
                        return Temperaturevalue;

                }
                return Temperaturevalue;
            }
        }

        /// <summary>
        /// Provides the time since the sensor value was last updated
        /// </summary>
        /// <param name="PropertyName">Name of the property whose time since last update Is required</param>
        /// <returns>Time in seconds since the last sensor update for this property</returns>
        /// <remarks>
        /// PropertyName should be one of the sensor properties Or empty string to get
        /// the last update of any parameter. A negative value indicates no valid value
        /// ever received.
        /// </remarks>
        public double TimeSinceLastUpdate(string PropertyName)
        {
            tl.LogMessage("TimeSinceLastUpdate", PropertyName + " - not implemented");
            throw new MethodNotImplementedException("TimeSinceLastUpdate(" + PropertyName + ")");
        }

        /// <summary>
        /// Wind direction at the observatory in degrees
        /// </summary>
        /// <remarks>
        /// 0..360.0, 360=N, 180=S, 90=E, 270=W. When there Is no wind the driver will
        /// return a value of 0 for wind direction
        /// </remarks>
        public double WindDirection
        {
            get
            {
                tl.LogMessage("WindDirection", "get - not implemented");
                throw new PropertyNotImplementedException("WindDirection", false);
            }
        }

        /// <summary>
        /// Peak 3 second wind gust at the observatory over the last 2 minutes in m/s
        /// </summary>
        public double WindGust
        {
            get
            {
                tl.LogMessage("WindGust", "get - not implemented");
                throw new PropertyNotImplementedException("WindGust", false);
            }
        }

        /// <summary>
        /// Wind speed at the observatory in m/s
        /// </summary>
        public double WindSpeed
        {
            get
            {
                tl.LogMessage("WindSpeed", "get - not implemented");
                throw new PropertyNotImplementedException("WindSpeed", false);
            }
        }

        #endregion

        #region private methods

        #region calculate the gust strength as the largest wind recorded over the last two minutes

        // save the time and wind speed values
        private Dictionary<DateTime, double> winds = new Dictionary<DateTime, double>();

        private double gustStrength;

        private void UpdateGusts(double speed)
        {
            Dictionary<DateTime, double> newWinds = new Dictionary<DateTime, double>();
            var last = DateTime.Now - TimeSpan.FromMinutes(2);
            winds.Add(DateTime.Now, speed);
            var gust = 0.0;
            foreach (var item in winds)
            {
                if (item.Key > last)
                {
                    newWinds.Add(item.Key, item.Value);
                    if (item.Value > gust)
                        gust = item.Value;
                }
            }
            gustStrength = gust;
            winds = newWinds;
        }

        #endregion

        private void LogMessage(string identifier, string message, params object[] args)
        {
            tl.LogMessage(identifier, string.Format(message, args));
        }

        #endregion

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "ObservingConditions";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                tl.LogMessage("IsConnected Get", Convert.ToString(connectedState));
                return connectedState;
            }
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        #endregion

    }
}
