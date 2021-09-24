using System;
using System.IO;
using System.Runtime.InteropServices;
using VisualParser.Data;

namespace VisualParser.Core
{
    public static class InfoManager {
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
            if (Globals.AppInfo.User.OS == OSPlatform.Windows) {
                // Path in register of https handler (in this case default browser)
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Shell\Associations\URLAssociations\http\UserChoice";
                return Utils.GetTerminalData("powershell", 
                    $"/command Get-ItemProperty -Path Registry::{keyName} -Name \"ProgId\" | Select -ExpandProperty \"ProgId\"");
            }
            if (Globals.AppInfo.User.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", "xdg-settings get default-web-browser");;
            }
            if (Globals.AppInfo.User.OS == OSPlatform.OSX) {
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
            if (Globals.AppInfo.User.OS == OSPlatform.Windows) {
                const string registryPathBase = @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*";
                const string registryPath64Bit = @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
                // Parse version
                if (Environment.Is64BitOperatingSystem) 
                    return Utils.GetTerminalData("powershell", $"/command (Get-ItemProperty {registryPathBase}, {registryPath64Bit} | select DisplayName, DisplayVersion | where DisplayName -Like \"*{formattedBrowserName}*\").DisplayVersion");
                return Utils.GetTerminalData("powershell", $"/command (Get-ItemProperty {registryPathBase} | select DisplayName, DisplayVersion | where DisplayName -Like \"*{formattedBrowserName}*\").DisplayVersion");
            }
            if (Globals.AppInfo.User.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", $"{linuxOnlyBrowserName} --version");
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static void HandleInfo() {
            if (File.Exists(Globals.AppInfoPath)) {
                // Loading all info from .json
                try {
                    Globals.AppInfo.LoadValuesFromJson();
                    // During json deserialization PathToDriverBinary for no reason sets to null
                    // and also path may become old, so it's updates every time
                    Globals.AppInfo.UpdatePathToDriverBinary();
                }
                catch (Exception e) {
                    // If something goes wrong updating info manually
                    ColoredConsole.WriteLine("[Red]ERROR![/Red] Bad data in .json or something went wrong");
                    ColoredConsole.WriteLine("Program will [Yellow]try to fix issues automatically[/Yellow]");
                    
                    Console.WriteLine("Updating...");
                    Update();
                    ColoredConsole.Warning("WARNING! Default app info will be used");
                    
                    Globals.AppInfo.SaveToJson();
                    ColoredConsole.WriteLine("App data was updated and saved to .json", ConsoleColor.Green);
                    return;
                }
                // Check if user want to updated old user info
                bool useJsonUserInfo = Utils.AskUserInput("Use old user info .json data? [y/n] ", 
                    ConsoleColor.Yellow);
                if (useJsonUserInfo) 
                    return;
            }
            // Updating user info and saving whole app info with new data
            Update();
            Globals.AppInfo.SaveToJson();
        }
        
        #region Update method overloads
        /// <summary>
        /// Method gets current user's info and store it to global object
        /// </summary>
        /// <seealso cref="UserInfoContainer"/>
        private static void Update() {
            // Updating path to binary (in case driver is downloaded already)
            Globals.AppInfo.UpdatePathToDriverBinary();
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
        /// Method store given data to global object
        /// </summary>
        /// <param name="browserName">User's default browser name</param>
        /// <param name="browserVersion">User's default browser version</param>
        /// <param name="browserType">User's User's default browser type</param>
        /// <seealso cref="UserInfoContainer"/>
        private static void Update(string browserName, string browserVersion, 
            BrowserType browserType) {
            Globals.AppInfo.SetUserInfo(browserName, browserVersion.Trim(), browserType);
        }
        #endregion
    }
}