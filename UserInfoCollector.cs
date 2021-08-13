using System;
using System.Runtime.InteropServices;
using VisualParser.Data;

namespace VisualParser
{
    public static class UserInfoCollector
    {
        public static void UpdateGlobalInfo() {
            // Collecting formatted browser's name and type
            string newBrowserName = Utils.FormatBrowserName(GetDefaultBrowserName(), out var newBrowserType);
            // Updating data
            GlobalUserInfo.BrowserName = newBrowserName;
            GlobalUserInfo.Browser = newBrowserType;
            GlobalUserInfo.BrowserVersion = GetBrowserVersion(newBrowserName);
        }

        public static string GetDefaultBrowserName() {
            if (GlobalUserInfo.OS == OSPlatform.Windows) {
                // Path in register of http handler (default browser)
                const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Shell\Associations\URLAssociations\http\UserChoice";
                return Utils.GetTerminalData("powershell", 
                    $"/command Get-ItemProperty -Path Registry::{keyName} -Name \"ProgId\" | Select -ExpandProperty \"ProgId\"");
            }
            if (GlobalUserInfo.OS == OSPlatform.Linux) {
                // TODO: patch out this shit after testing on Linux
                Console.WriteLine("Better pray and cross your fingers. I have no fucking idea will it work on Linux...");
                string name = Utils.GetTerminalData("/bin/bash", "xdg-settings get default-web-browser");
                Console.WriteLine($"Detected unformatted name: {name}");
                return name;
            }
            
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static string GetBrowserVersion(string browserName) {
            if (GlobalUserInfo.OS == OSPlatform.Windows) {
                // Path in register for getting browser's version
                string keyVersion = $@"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{browserName}.exe";
                // Parse version
                return Utils.GetTerminalData("powershell", 
                    $"/command (Get-Item (Get-ItemProperty '{keyVersion}').'(Default)').VersionInfo.ProductVersion");
            }
            if (GlobalUserInfo.OS == OSPlatform.Linux) {
                return Utils.GetTerminalData("/bin/bash", $"{browserName} --version");
            }
            
            throw new NotSupportedException("Are you running this on toaster?!?!");
        }

        public static string GetBrowserVersion() {
            return GetBrowserVersion(GlobalUserInfo.BrowserName);
        }
    }
}