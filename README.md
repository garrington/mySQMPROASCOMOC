# mySQMPROASCOMOC
(c) Robert Brown 2019. All rights reserved.

You may freely use and distribute these files as long as the authors name and copyright message is NOT removed from the source code or application. You must acknowledge the orginal author in all distributions and provide a link back to this repository.

## Build Instructions
- Download or clone this repository. Download and Install Microsoft Visual Studio 
- Make a folder on your home folder/desktop to hold the source files
- Unzip the repository zip file into that folder
- Start Visual Studio and Open the project "src\mySQMPROASCOM\mySQMPROASCOM.csproj"
- Build the solution
- To recreate the exceutable, you need to open the folder src\mySQMPROASCOM\bin\Release
- You will need to modify the paths in the mySQMPRO Setup.iss file
- Install the Inno Setup Compiler 5.5.6(a)
- Double click the mySQMPRO Setup.iss file
- Build - Compile to generate a new install .exe file
- Run the .exe to remove the old driver and replace it with the new driver

## When making a new version
- In the ASCOM driver.cs file, increment the following
-- // Implements:	ASCOM ObservingConditions interface version: 1.0.3
-- public const string driverVersion = "1.0.3";
-- The SetupDialogForm.cs header for the form (do this in visual studio)
- In the Inno setup file mySQMPRO Setup.iss change
-- AppVerName=ASCOM mySQMPRO ObservingConditions Driver 1.0.3
-- AppVersion=1.0.3
-- VersionInfoVersion=1.0.3
-- OutputBaseFilename="mySQMPROASCOMOCSetup103"
- Edit the mySQMPROReadMe.htm file and make changes to reflect updates
- Run the INNO setup, Build a new install driver (exe)
