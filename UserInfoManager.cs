using System;
using System.Runtime.InteropServices;
using VisualParser.Data;

namespace VisualParser
{
    public static class UserInfoManager
    {
        /// <summary>
        /// Method formats given browser name and sets appropriate BrowserType
        /// </summary>
        /// <param name="browserName">Browser name</param>
        /// <param name="type">out param browser type</param>
        /// <returns>Formatted browser name</returns>
        public static string FormatBrowserName(string browserName, out BrowserType type) {
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

        public static string GetDefaultBrowserName() {
            if (UserInfoContainer.OS == OSPlatform.Windows) {
                // Path in register of http handler (default browser)
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Shell\Associations\URLAssociations\http\UserChoice";
                return Utils.GetTerminalData("powershell", 
                    $"/command Get-ItemProperty -Path Registry::{keyName} -Name \"ProgId\" | Select -ExpandProperty \"ProgId\"");
            }
            if (UserInfoContainer.OS == OSPlatform.Linux) {
                // TODO: patch out this shit after testing on Linux
                Console.WriteLine("Better pray and cross your fingers. I have no fucking idea will it work on Linux...");
                string name = Utils.GetTerminalData("/bin/bash", "xdg-settings get default-web-browser");
                Console.WriteLine($"Detected unformatted name: {name}");
                return name;
            }
            if (UserInfoContainer.OS == OSPlatform.OSX) {
                // TODO: test this command before actual code this
                // plutil -p ~/Library/Preferences/com.apple.LaunchServices/com.apple.launchservices.secure.plist | grep 'https' -b3 |awk 'NR==3 {split($4, arr, "\""); print arr[2]}'
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static string GetBrowserVersion(string browserName) {
            if (UserInfoContainer.OS == OSPlatform.Windows) {
                // Path in register for getting browser's version
                string keyVersion = $@"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{browserName}.exe";
                // Parse version
                return Utils.GetTerminalData("powershell", 
                    $"/command (Get-Item (Get-ItemProperty '{keyVersion}').'(Default)').VersionInfo.ProductVersion");
            }
            if (UserInfoContainer.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", $"{browserName} --version");
            }
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }
    }
}