using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using VisualParser.Data;

namespace VisualParser.Core
{
    public static class UserInfoManager {
        /// <summary>
        /// Method formats given browser name and sets appropriate BrowserType
        /// </summary>
        /// <param name="browserName">Browser name</param>
        /// <param name="type">out param browser type</param>
        /// <returns>Formatted browser name</returns>
        private static string FormatBrowserName(string browserName, out BrowserType type) {
            // Default values
            string formattedName = browserName.ToLower().Trim();
            type = BrowserType.None;

            if (formattedName.Contains("chrome")) {
                type = BrowserType.Chrome;
                return "chrome";
            }
            if (formattedName.Contains("firefox")) {
                type = BrowserType.Firefox;
                return "firefox";
            }
            
            return formattedName;
        }

        /// <summary>
        /// Method gets user's default browser name
        /// </summary>
        /// <returns>User's default browser name</returns>
        private static string GetDefaultBrowserName() {
            if (Globals.CurrentUserInfo.OS == OSPlatform.Windows) {
                // Path in register of http handler (default browser)
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Shell\Associations\URLAssociations\http\UserChoice";
                return Utils.GetTerminalData("powershell", 
                    $"/command Get-ItemProperty -Path Registry::{keyName} -Name \"ProgId\" | Select -ExpandProperty \"ProgId\"");
            }
            if (Globals.CurrentUserInfo.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", "xdg-settings get default-web-browser");;
            }
            if (Globals.CurrentUserInfo.OS == OSPlatform.OSX) {
                // TODO: test this command before actual code this
                // plutil -p ~/Library/Preferences/com.apple.LaunchServices/com.apple.launchservices.secure.plist | grep 'https' -b3 |awk 'NR==3 {split($4, arr, "\""); print arr[2]}'
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        /// <summary>
        /// Method gets version of given browser by it's formatted and unformatted names
        /// (Depending on OS need different name. For example on Windows need formatted name)
        /// </summary>
        /// <param name="formattedBrowserName">Formatted name of browser</param>
        /// <param name="unformattedBrowserName">Unformatted name of browser</param>
        /// <returns>Browser's version</returns>
        private static string GetBrowserVersion(string formattedBrowserName, string unformattedBrowserName) {
            if (Globals.CurrentUserInfo.OS == OSPlatform.Windows) {
                // Path in register for getting browser's version
                string keyVersion = $@"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{formattedBrowserName}.exe";
                // Parse version
                return Utils.GetTerminalData("powershell", 
                    $"/command (Get-Item (Get-ItemProperty '{keyVersion}').'(Default)').VersionInfo.ProductVersion");
            }
            if (Globals.CurrentUserInfo.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", $"{unformattedBrowserName} --version");
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static void HandleInfo() {
            // This collects all needed data and store it to global info
            // object (static readonly Instance of UserInfoContainer)
            if (File.Exists(Globals.AppDataFilename)) {
                // Json data might become old after browser update, so have to ask user
                bool useJsonData = Utils.AskUserInput("Use old .json data? [y/n] ", 
                    ConsoleColor.Yellow);
                if (useJsonData) {
                    ColoredConsole.WriteLine("Loading from json...", ConsoleColor.Yellow);
                    // If program can't load data from .json file it will update manually
                    // and save new data to .json
                    try {
                        Update(Globals.AppDataFilename);
                    }
                    catch (Exception e) {
                        ColoredConsole.WriteLine("[Red]ERROR![/Red] Bad data or something went wrong");
                        Console.WriteLine("Updating data manually...");
                        Update();
                        SaveToJson(Globals.AppDataFilename);
                    }
                    return;
                }
            }
            Update();
            SaveToJson(Globals.AppDataFilename);
        }
        
        #region Update methods overloads
        /// <summary>
        /// Method gets current user's info and store it to Globals.CurrentUserInfo
        /// (All collected info described in Data/UserInfoContainer.cs class)
        /// </summary>
        private static void Update() {
            // Collecting formatted browser's name and type
            string unformattedBrowserName = GetDefaultBrowserName();
            string formattedBrowserName = FormatBrowserName(unformattedBrowserName, out var newBrowserType);
            // Updating data
            Update(formattedBrowserName, GetBrowserVersion(formattedBrowserName, unformattedBrowserName), 
                newBrowserType);
        }
        
        /// <summary>
        /// Method store given data to Globals.CurrentUserInfo
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
        /// Method loads user's info from json file and store it to Globals.CurrentUserInfo
        /// (All collected info described in Data/UserInfoContainer.cs class)
        /// </summary>
        /// <param name="pathToJson">Path to json file</param>
        private static void Update(string pathToJson) {
            var loadedContainer = LoadFromJson(pathToJson);
            Globals.SetUserInfo(loadedContainer.BrowserName, loadedContainer.BrowserVersion, 
                loadedContainer.Browser);
        }
        #endregion
        
        #region Json save/load
        private static UserInfoContainer LoadFromJson(string path) { 
            return JsonSerializer.Deserialize<UserInfoContainer>(File.ReadAllText(path));
        }

        private static void SaveToJson(string path) {
            File.WriteAllText(path, JsonSerializer.Serialize(Globals.CurrentUserInfo));
        }
        #endregion
    }
}