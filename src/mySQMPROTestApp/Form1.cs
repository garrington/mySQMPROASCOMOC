// 104 11122017
// Small changes to interface

// 103 28102017
// Change certificate signing to 2022

// 102 25102017
// Prevent form from being resized

// 101 25102017
// Add version number to title bar of window
// Remove desktop icon on install - fails to install due to permission issue for desktop

using System;
using System.Windows.Forms;

namespace ASCOM.mySQMPRO
{
    public partial class Form1 : Form
    {
        String myVersion = "mySQMPRO ASCOM Tester App ";

        private ASCOM.DriverAccess.ObservingConditions driver;

        public Form1()
        {
            InitializeComponent();          // modified
            SetUIState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnected)
                driver.Connected = false;

            Properties.Settings.Default.Save();
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DriverId = ASCOM.DriverAccess.ObservingConditions.Choose(Properties.Settings.Default.DriverId);
            SetUIState();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                driver.Connected = false;
            }
            else
            {
                driver = new ASCOM.DriverAccess.ObservingConditions(Properties.Settings.Default.DriverId);
                driver.Connected = true;
            }
            SetUIState();
        }

        private void SetUIState()
        {
            buttonConnect.Enabled = !string.IsNullOrEmpty(Properties.Settings.Default.DriverId);
            buttonChoose.Enabled = !IsConnected;
            buttonConnect.Text = IsConnected ? "Disconnect" : "Connect";
        }

        private bool IsConnected
        {
            get
            {
                return ((this.driver != null) && (driver.Connected == true));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            this.Location = Properties.Settings.Default.FormLocation;

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                myVersion = myVersion + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                this.Text = myVersion;
            }
            else
            {
                this.Text = myVersion;
            }
        }

        private void TemperatureBtn_Click(object sender, EventArgs e)
        {
            // get temperature
            if (IsConnected)
            {
                MessageTxtBox.Text = "Temperature = " + driver.Temperature.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void HumidityBtn_Click(object sender, EventArgs e)
        {
            // get humidity
            if (IsConnected)
            {
                MessageTxtBox.Text = "Humidity = " + driver.Humidity.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void DewPointBtn_Click(object sender, EventArgs e)
        {
            // get dew-point
            if (IsConnected)
            {
                MessageTxtBox.Text = "Dew-point = " + driver.DewPoint.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FormLocation = this.Location;
            Properties.Settings.Default.Save();
            Close();
        }

        private void SkyTemperatureBtn_Click(object sender, EventArgs e)
        {
            // get Sky Temperature
            if (IsConnected)
            {
                MessageTxtBox.Text = "Sky-Temperature = " + driver.SkyTemperature.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void LuxBtn_Click(object sender, EventArgs e)
        {
            // get Sky Brightness in Lux
            if (IsConnected)
            {
                MessageTxtBox.Text = "Sky-Brightness in Lux = " + driver.SkyBrightness.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void SQMBtn_Click(object sender, EventArgs e)
        {
            // get Sky Reading
            if (IsConnected)
            {
                MessageTxtBox.Text = "Sky-Reading SQM = " + driver.SkyQuality.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }

        private void PressureBtn_Click(object sender, EventArgs e)
        {
            // get Atmospheric Pressure
            if (IsConnected)
            {
                MessageTxtBox.Text = "Atmospheric Pressure = " + driver.Pressure.ToString();
                MessageTxtBox.Update();
            }
            else
            {
                MessageTxtBox.Text = "Not connected";
                MessageTxtBox.Update();
            }
        }
    }
}
