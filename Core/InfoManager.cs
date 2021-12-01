using System.IO;
using System.Runtime.InteropServices;
using VisualParser.Data;

namespace VisualParser.Core
{
    // TODO: add docs here + clean up ChromeDriverLoader.cs
    public static class InfoManager {
        public static void HandleInfo() {
            string? chromeVersion = TryGetChromeVersion();
            // If version is null, chrome doesn't installed
            // (Also don't have to search path to chrome.exe Selenium will do this itself)
            if (chromeVersion is null) {
                ColorConsole.Error("Couldn't find chrome on your computer during version parsing");
                HandleChromeInstallation();
                // After chrome was installed trying to get version again
                // Note(Nik4ant): Not 100% that user installed chrome, so have to check again
                // and exit if couldn't parse it again
                chromeVersion = TryGetChromeVersion();
                if (chromeVersion is null) {
                    ColorConsole.Error("Couldn't parse chrome version again. Try to rerun program");
                    Environment.Exit(0);
                }
            }
            ColorConsole.WriteLine($"Parsed chrome version: {chromeVersion}", ConsoleColor.Green);
            // Check if there is .json file with app info already
            if (File.Exists(Globals.AppInfoPath)) {
                var loadedInfo = AppInfoContainer.LoadFromJson();
                // Set info to Globals
                // Note(Nik4ant): More stuff (for example, app settings) might be loaded here later
                Globals.SetAppInfo(new AppInfoContainer {
                    ChromeVersion = chromeVersion,
                });
                // If there is new version of chrome, don't have to check old driver
                if (loadedInfo?.ChromeVersion != chromeVersion) {
                    ColorConsole.Warning("Chrome was updated. Installing new driver...");
                    // TODO: В этом методе может устанавливаться путь до драйвера и сохраняться настройки...
                    // поэтому надо либо: а) Указать это где-то б) *вряд ли* отделить этот код оттуда
                    ChromeDriverLoader.Load(chromeVersion);
                    // TODO: разобраться с save и load'ом
                    // Saving updated info after driver was downloaded
                    Globals.Info.SaveToJson();
                    return;
                }
            }
            // Check if driver already installed
            string? oldDriverPath = TryGetExistingDriverPath();
            if (oldDriverPath is not null) {
                ColorConsole.WriteLine($"Program detected driver: {oldDriverPath}", ConsoleColor.Green);
                if (!Utils.AskUserInput("Do you want to override it (install new one)? [y/n] "))
                    return;
                // Note(Nik4ant): Don't have to clear folder here, because old driver will be overriden
            }
            ChromeDriverLoader.Load(chromeVersion);
        }
        
        private static string? TryGetExistingDriverPath() {
            // Check for "Driver" directory 
            if (!Directory.Exists(Globals.Info.PathToDriverFolder)) {
                Directory.CreateDirectory(Globals.Info.PathToDriverFolder);
                return null;
            }
            var files = Directory.GetFiles(Globals.Info.PathToDriverFolder);
            // Only one file in folder
            if (files.Length == 1) {
                return files[0];
            }
            // More than 1 file in folder
            if (files.Length != 0) {
                ColorConsole.Warning("WARNING! There are more than 1 files in driver folder");
                if (Utils.AskUserInput("Do you want to delete them all? [y/n] ")) {
                    // TODO: for no reason path to files are weird
                    for (int i = 0; i < files.Length; i++)
                        File.Delete(files[i]);
                }
            }
            return null;
        }
        
        private static string? TryGetChromeVersion() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Path in registry to check version
                const string registryPathBase = @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*";
                const string registryPath64Bit = @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
                // Note(Nik4ant): Could use Microsoft.Win32.Registry class, but this implementation is easier
                if (Environment.Is64BitOperatingSystem) {
                    return Utils.GetProcessData("powershell",  $"/command (Get-ItemProperty {registryPathBase}, {registryPath64Bit} | Where-Object DisplayName -Like \"*chrome*\").DisplayVersion");
                }
                return Utils.GetProcessData("powershell", $"/command (Get-ItemProperty {registryPathBase} | Where-Object DisplayName -Like \"*chrome*\").DisplayVersion");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return Utils.GetProcessData("/bin/bash", "google-chrome --version");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var output = Utils.GetProcessData("Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "--version");
                // Trying to format value if it's not null
                if (output is not null)
                    return output.Replace("Google Chrome ", "");
                return null;
            }
            throw new NotSupportedException();
        }

        private static void HandleChromeInstallation()  {
            // In case if user has chrome installed we asking him to select .exe file
            if (Utils.AskUserInput("Do you want to select path to \"chrome.exe\"? [y/n]")) {
                // TODO: reuse old dialog stuff (put it in Utils)
                /*
                Add-Type -AssemblyName System.Windows.Forms;
                $FileDialog = New-Object System.Windows.Forms.OpenFileDialog -Property @{ InitialDirectory = [Environment]::GetFolderPath('Desktop')\nFilter = '(*.json)|*.json|All files (*.*)|*.*'}
                $FileDialog.ShowDialog();
                echo $FileDialog.FileName;
                 */
            }
            else {
                ColorConsole.Warning("Program can automatically install chrome browser (NOT TESTED YET)");
                // Use auto installation or "semi auto" installation
                if (Utils.AskUserInput("Use automatic installation? [y/n]")) {
                    // TODO: auto install stuff
                }
                else {
                    // Install and run chrome installer for the user, if he wants so
                    ColorConsole.Warning("Program will install latest chrome installer and run it for you");
                    if (Utils.AskUserInput("Install and run chrome installer? [y/n]")) {
                        // TODO: download + executing
                    }
                    else {
                        ColorConsole.Error("Can't continue without chrome installed");
                        Environment.Exit(0);
                    }
                }
            }
            ColorConsole.WriteLine("Seams like chrome was installed successfully", ConsoleColor.Green);
        }
    }
}   