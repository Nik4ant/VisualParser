using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using VisualParser.Data;

namespace VisualParser.Core
{
    public static class UserInfoManager {
        /// <summary>
        /// Method formats given browser name and sets appropriate BrowserType.
        /// Also for linux sets different browser name because of OS depended stuff)
        /// </summary>
        /// <param name="browserName">Browser name</param>
        /// <param name="linuxBrowserName">Browser name for linux only</param>
        /// <param name="type">out param browser type</param>
        /// <returns>Formatted browser name</returns>
        private static string FormatBrowserName(string browserName, out string linuxBrowserName, out BrowserType type) {
            // Default values
            string formattedName = browserName.ToLower().Trim();
            linuxBrowserName = browserName.ToLower().Trim();
            type = BrowserType.None;

            if (formattedName.Contains("chrome")) {
                type = BrowserType.Chrome;
                linuxBrowserName = "google-chrome";
                return "chrome";
            }
            if (formattedName.Contains("firefox")) {
                type = BrowserType.Firefox;
                linuxBrowserName = "firefox";
                return "firefox";
            }
            
            return formattedName;
        }

        /// <summary>
        /// Method gets user's default browser name
        /// </summary>
        /// <returns>User's default browser name</returns>
        private static string GetDefaultBrowserName() {
            if (Globals.UserInfo.OS == OSPlatform.Windows) {
                // Path in register of https handler (in this case default browser)
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Shell\Associations\URLAssociations\http\UserChoice";
                return Utils.GetTerminalData("powershell", 
                    $"/command Get-ItemProperty -Path Registry::{keyName} -Name \"ProgId\" | Select -ExpandProperty \"ProgId\"");
            }
            if (Globals.UserInfo.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", "xdg-settings get default-web-browser");;
            }
            if (Globals.UserInfo.OS == OSPlatform.OSX) {
                // TODO: test this command before actual code this
                // plutil -p ~/Library/Preferences/com.apple.LaunchServices/com.apple.launchservices.secure.plist | grep 'https' -b3 |awk 'NR==3 {split($4, arr, "\""); print arr[2]}'
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        /// <summary>
        /// Method gets version of given browser by it's formatted and unformatted names
        /// (Depending on OS need different name)
        /// </summary>
        /// <param name="formattedBrowserName">Formatted name of browser</param>
        /// <param name="linuxOnlyBrowserName">Browser name for linux only</param>
        /// <returns>Browser's version</returns>
        private static string GetBrowserVersion(string formattedBrowserName, string linuxOnlyBrowserName) {
            if (Globals.UserInfo.OS == OSPlatform.Windows) {
                const string registryPathBase = @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*";
                const string registryPath64Bit = @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
                // Parse version
                if (Environment.Is64BitOperatingSystem) 
                    return Utils.GetTerminalData("powershell", $"/command (Get-ItemProperty {registryPathBase}, {registryPath64Bit} | select DisplayName, DisplayVersion | where DisplayName -Like \"*{formattedBrowserName}*\").DisplayVersion");
                return Utils.GetTerminalData("powershell", $"/command (Get-ItemProperty {registryPathBase} | select DisplayName, DisplayVersion | where DisplayName -Like \"*{formattedBrowserName}*\").DisplayVersion");
            }
            if (Globals.UserInfo.OS == OSPlatform.Linux) {
                // FIXME: For chrome this is "google-chrome --version"
                // https://linuxhint.com/check-google-chrome-browser-version/
                return Utils.GetTerminalData("/bin/bash", $"{linuxOnlyBrowserName} --version");
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static void HandleInfo() {
            // This collects all needed data and store it to global info
            // object (static readonly Instance of UserInfoContainer)
            if (File.Exists(Globals.UserInfoFilename)) {
                // Json data might become old after browser update, so have to ask user
                bool useJsonData = Utils.AskUserInput("Use old .json data? [y/n] ", 
                    ConsoleColor.Yellow);
                if (useJsonData) {
                    ColoredConsole.WriteLine("Loading from json...", ConsoleColor.Yellow);
                    // If program can't load data from .json file it will update manually
                    // and save new data to .json
                    try {
                        Update(Globals.UserInfoFilename);
                    }
                    catch (Exception e) {
                        ColoredConsole.WriteLine("[Red]ERROR![/Red] Bad data or something went wrong");
                        Console.WriteLine("Updating data manually...");
                        Update();
                        Globals.UserInfo.SaveToJson(Globals.UserInfoFilename);
                        ColoredConsole.WriteLine("Data were updated and saved to .json", ConsoleColor.Green);
                    }
                    return;
                }
            }
            Update();
            Globals.UserInfo.SaveToJson(Globals.UserInfoFilename);
        }
        
        #region Update methods overloads
        /// <summary>
        /// Method gets current user's info and store it to Globals.UserInfo
        /// (All collected info described in Data/UserInfoContainer.cs class)
        /// </summary>
        private static void Update() {
            // Collecting formatted browser's name and type
            // (For different OS same browser can have different names.
            // For example "*chrome*" on windows and "google-chrome" on linux)
            string formattedBrowserName = FormatBrowserName(GetDefaultBrowserName(), 
                out var linuxOnlyBrowserName, out var newBrowserType);
            // Updating data
            Update(formattedBrowserName, GetBrowserVersion(formattedBrowserName, linuxOnlyBrowserName), 
                newBrowserType);
        }
        
        /// <summary>
        /// Method store given data to Globals.UserInfo
        /// (All collected info described in Data/UserInfoContainer.cs class)
        /// </summary>
        /// <param name="browserName">User's default browser name</param>
        /// <param name="browserVersion">User's default browser version</param>
        /// <param name="browserType">User's User's default browser type</param>
        private static void Update(string browserName, string browserVersion, 
            BrowserType browserType) {
            Globals.SetUserInfo(browserName, browserVersion.Trim(), browserType);
        }
        
        /// <summary>
        /// Method loads user's info from json file and store it to Globals.UserInfo
        /// (All collected info described in Data/UserInfoContainer.cs class)
        /// </summary>
        /// <param name="pathToJson">Path to json file</param>
        private static void Update(string pathToJson) {
            var loadedContainer = UserInfoContainer.LoadFromJson(pathToJson);
            Globals.SetUserInfo(loadedContainer);
        }
        #endregion
    }
}