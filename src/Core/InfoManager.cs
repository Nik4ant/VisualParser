using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using VisualParser.Data;

namespace VisualParser.Core
{
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
                while (chromeVersion is null) {
                    ColorConsole.Error("Couldn't parse chrome version");
                    chromeVersion = TryGetChromeVersion();
                    Environment.Exit(0);
                }
            }
            ColorConsole.WriteLine($"Parsed chrome version: {chromeVersion}", ConsoleColor.Green);
            // Check if there is .json file with app info already
            if (File.Exists(Globals.AppInfoPath)) {
                // TODO: Create method with loading all global data (or smth like that)? In Globals.cs
                var loadedInfo = AppInfoContainer.LoadFromJson();
                // Set info to Globals
                // Note(Nik4ant): More stuff (for example, app settings) might be loaded here later
                Globals.SetAppInfo(new AppInfoContainer {
                    ChromeVersion = chromeVersion,
                });
                // If there is new version of chrome, don't have to check old driver
                if (loadedInfo?.ChromeVersion != chromeVersion) {
                    ColorConsole.Warning("Chrome was updated. Installing new driver...");
                    ChromeDriverLoader.Load(chromeVersion);
                    // Saving updated info after driver was downloaded
                    Globals.Info.SaveToJson();
                    return;
                }
            }
            else {
                // Note(Nik4ant): If there is no .json file yet we save ONLY necessary properties
                Globals.SetAppInfo(new AppInfoContainer {
                    ChromeVersion = chromeVersion,
                });
                Globals.Info.SaveToJson();
            }
            // Check if driver already installed
            string? oldDriverPath = TryGetExistingDriverPath();
            if (oldDriverPath is not null) {
                ColorConsole.WriteLine($"Program detected driver: {oldDriverPath}", ConsoleColor.Green);
                if (!Utils.AskUserInput("Do you want to override it (install new one)?"))
                    return;
                    // Note(Nik4ant): Don't have to clear folder here, because old driver will be overwritten
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
                if (Utils.AskUserInput("Do you want to delete them all (Recommend)?")) {
                    for (int i = 0; i < files.Length; i++)
                        File.Delete(files[i]);
                }
            }
            return null;
        }
        
        private static string? TryGetChromeVersion() {
            string? version = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Path in registry to check version
                const string registryPathBase = @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*";
                const string registryPath64Bit = @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
                // Note(Nik4ant): Could use Microsoft.Win32.Registry class, but this implementation is easier
                if (Environment.Is64BitOperatingSystem) {
                    version = Utils.GetProcessData("powershell",  
                        $"/command (Get-ItemProperty {registryPathBase}, {registryPath64Bit} | Where-Object DisplayName -Like \"*chrome*\").DisplayVersion");
                }
                else {
                    version = Utils.GetProcessData("powershell",
                        $"/command (Get-ItemProperty {registryPathBase} | Where-Object DisplayName -Like \"*chrome*\").DisplayVersion");
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var output = Utils.GetProcessData("/bin/bash", "-c \"echo `google-chrome --version`\"");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var output = Utils.GetProcessData("Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "--version");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            // Format version if it was successfully parsed
            if (!string.IsNullOrWhiteSpace(version))
                return version.Trim();
            return null;
        }

        private static void HandleChromeInstallation()  {
            // In case if user has chrome installed we ask him to select it
            while (Utils.AskUserInput("Do you want to select path to [Yellow]chrome executable[/Yellow]?", 
                true)) {
                string? newPathToChrome = Utils.SelectSingleFileDialog("Chrome");
                if (!string.IsNullOrEmpty(newPathToChrome)) {
                    Globals.Info.SetCustomPathToChrome(newPathToChrome);
                    // FIXME: that's actually doesn't affect anything...
                    // Because if program can't parse version it will not use newPathToChrome next time
                    // TODO: use newPathToChrome in version parsing if it was specified
                    ColorConsole.WriteLine($"[Green]Path to chrome was specified[/Green]: {newPathToChrome}");
                    return;
                }
                ColorConsole.Error("Couldn't get selected file! Try again if you want");
            }
            // In case if user hasn't chrome...
            ColorConsole.Warning("Program can install latest chrome installer and run it");
            if (Utils.AskUserInput("Install and run chrome installer?")) {
                // Note(Nik4ant) On linux there is only installation package.
                // And program don't have enough permission to automatically unpack it.
                // Way to somehow unpack it would be awesome improvement for installation process
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    ChromeInstallationWindows();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    ChromeInstallationLinux();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    ChromeInstallationMac();
            }
            else {
                ColorConsole.Error("Can't continue without chrome installed");
                Environment.Exit(0);
            }
            ColorConsole.WriteLine("Seams like chrome was installed successfully", ConsoleColor.Green);
        }
        
        #region Cross platform chrome installation
        private static void ChromeInstallationWindows() {
            string installerFilename;
            if (Environment.Is64BitOperatingSystem) {
                Utils.DownloadFileByUrl("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BB772F1AF-7BAF-C006-B980-CC577C94EBC7%7D%26lang%3Den%26browser%3D3%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-stable-statsdef_1%26installdataindex%3Dempty/chrome/install/ChromeStandaloneSetup64.exe", 
                    ".");
                installerFilename = "ChromeStandaloneSetup64.exe";
            }
            else {
                Utils.DownloadFileByUrl("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BB772F1AF-7BAF-C006-B980-CC577C94EBC7%7D%26lang%3Den%26browser%3D3%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dstable-arch_x86-statsdef_1%26installdataindex%3Dempty/chrome/install/ChromeStandaloneSetup.exe", 
                    ".");
                installerFilename = "ChromeStandaloneSetup.exe";
            }
            // Running installer and removing it after it was closed
            var process = Process.Start(new ProcessStartInfo {
                CreateNoWindow = false,
                FileName = installerFilename
            });
            process!.WaitForExit();
            process.Dispose();
            File.Delete(installerFilename);
        }
        private static void ChromeInstallationLinux() {
            const string packageName = "google-chrome-stable_current_amd64.deb";
            Utils.DownloadFileByUrl($"https://dl.google.com/linux/direct/{packageName}", 
                packageName);
            var process = Process.Start(new ProcessStartInfo("bash") {
                // This argument is useless, but clearly shows that the program
                // isn't able to intercept the sudo password
                RedirectStandardInput = false,
                // TODO: add colors for messages (move them to const string?)
                Arguments = $"-c \"echo Trying to unpack chromes .deb package... && sudo dpkg -i {packageName} && echo Exit terminal if nothing happened\""
            });
            // -1 is infinite waiting until process ends or user exit
            process!.WaitForExit(-1);
            // Deleting .deb package
            File.Delete(packageName);
            // Closing process
            process.Close();
            process.Dispose();
        }
        private static void ChromeInstallationMac() {
            Utils.DownloadFileByUrl("https://dl.google.com/chrome/mac/stable/GGRO/googlechrome.dmg", 
                "googlechrome.dmg");
            throw new NotImplementedException("There is no .dmp unpack yet");
        }
        
        #endregion
    }
}   