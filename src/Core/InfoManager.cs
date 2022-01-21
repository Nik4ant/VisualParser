using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

namespace VisualParser.Core
{
    public static class InfoManager {
        public static void HandleAndSetInfo() {
            // Container with info about chrome
            ChromeBrowserInfo currentChromeInfo = new();
            // Step 1. Find path to chrome executable
            string? pathToChromeBinary = TryGetChromeBinaryPath();
            // If path wasn't detected automatically there are 2 options: 
            // 1) User somehow make it impossible to detect. Solution: Ask user to select chrome
            // 2) Chrome doesn't installed on machine. Solution: Ask user to install chrome
            if (pathToChromeBinary is null) {
                ColorConsole.Error("Couldn't find chrome on your computer");
                pathToChromeBinary = HandleSelectChromeDialog();
                if (pathToChromeBinary is null) {
                    ColorConsole.Error("No chrome detected");
                    HandleChromeInstallation();
                    if (pathToChromeBinary is null) {
                        ColorConsole.Error("Couldn't find chrome anyway. Try to rerun app");
                        Environment.Exit(1);
                    }
                }
            }
            currentChromeInfo.SetBinaryLocation(pathToChromeBinary);
            
            // Step 2. Find chrome version to install compatible driver
            string? chromeVersion = TryGetChromeVersion();
            if (chromeVersion is null) {
                chromeVersion = TryGetChromeVersion(pathToChromeBinary);
                // Note(Nik4ant): Have no idea what could possibly go wrong at this point. 
                if (chromeVersion is null){
                    ColorConsole.Error("Couldn't parse chrome version. Try to rerun app");
                    Environment.Exit(1);
                }
            }
            currentChromeInfo.SetChromeVersion(chromeVersion);
            
            // Step 3. Install driver if it wasn't installed already
            string pathToChromeDriver = TryGetExistingDriverPath() ?? ChromeDriverLoader.Load(chromeVersion);
            
            // Step 4. Handle saving/loading of .json file with app info.
            ApplicationInfo appInfo;
            // If data exist we load it and compare with current
            // otherwise saving current data to use it later
            if (File.Exists(PathToAppInfo)) {
                var loadedAppInfo = ApplicationInfo.LoadFromJson();
                if (loadedAppInfo is null) {
                    ColorConsole.Error("Couldn't deserialize app info from json.\nSomething is wrong with app save/load system");
                    Environment.Exit(1);
                }
                // Check if browser got updated or rollback to different version
                if (chromeVersion != loadedAppInfo.Chrome.Version) {
                    ColorConsole.Warning("Chrome version has changed. Updating info and installing new driver...");
                    // With different version details needs to be updated
                    AddDetailedBrowserInfo(target: currentChromeInfo);
                    // And new driver need to be installed
                    pathToChromeDriver = ChromeDriverLoader.Load(chromeVersion);
                    appInfo = new ApplicationInfo(pathToChromeDriver, currentChromeInfo);
                }
                else {
                    appInfo = loadedAppInfo;
                }
            }
            else {
                // Collect detailed info about chrome only if it wasn't parsed already
                AddDetailedBrowserInfo(target: currentChromeInfo);
                // Saving current info to json
                appInfo = new ApplicationInfo(pathToChromeDriver, currentChromeInfo);
            }
            ColorConsole.WriteLine($"Chrome version: [Green]{chromeVersion}[/Green]");
            ColorConsole.WriteLine($"Path to chrome binary: [Green]{pathToChromeBinary}[/Green]");
            // Saving app info every time is necessary because it could be updated or 
            SetGlobalAppInfo(appInfo);
            appInfo.SaveToJson();
        }
        
        // TODO: docs
        private static void AddDetailedBrowserInfo(ChromeBrowserInfo target) {
            // Http listener to parse request from js page
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8888/");
            httpListener.Start();
            // Open page that will send POST request to http listener with all info
            using var process = Process.Start(new ProcessStartInfo {
                FileName = target.PathToBinary,
                Arguments = $"{StaticManager.GetPageFile("InfoParsing", "index.html", true)} --new-window",
            });
            // If info wasn't parsed it most likely that it will be impossible
            // to continue (since program will not be able even to install driver)
            if (process is null) {
                ColorConsole.Error("Couldn't start chrome to parse info from JS");
                Environment.Exit(1);
            }
            // Reading json data from request
            var request = httpListener.GetContext().Request;
            using var requestInputStream = new StreamReader(request.InputStream, request.ContentEncoding);
            using var jsonReader = new JsonTextReader(requestInputStream);
            var parsedInfo = new JsonSerializer().Deserialize<Dictionary<string, string>>(jsonReader);
            // Closing chrome and localhost
            process.Close();
            httpListener.Stop();
            // Check if info was deserialized successfully
            if (parsedInfo is null) {
                requestInputStream.BaseStream.Seek(0, SeekOrigin.Begin);  // Used for read call below
                ColorConsole.Error("Couldn't deserialize info from JS request");
                ColorConsole.Error($"JS request input stream:\n{requestInputStream.ReadToEnd()}");
                Environment.Exit(1);
            }
            // Setting new info
            target.SetUserAgent(parsedInfo.GetValueOrDefault("userAgent", null));
            target.SetZoomLevel(parsedInfo.GetValueOrDefault("zoom", null));
            target.SetScreenResolution(parsedInfo.GetValueOrDefault("resolution", null));
        }
        
        /// <summary>
        /// Returns path to first file in driver folder. However there is a
        /// very small percent that this single file isn't chrome driver
        /// (because user can mess it up)
        /// </summary>
        private static string? TryGetExistingDriverPath() {
            // Check for "Driver" directory 
            if (!Directory.Exists(AbsolutePathToDriverFolder)) {
                Directory.CreateDirectory(AbsolutePathToDriverFolder);
                return null;
            }
            var files = Directory.GetFiles(AbsolutePathToDriverFolder);
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
                const string registryPath64Bit = @"HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var output = Utils.GetProcessData("/bin/bash", "-c \"echo `google-chrome --version`\"");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var output = Utils.GetProcessData("Applications/Google Chrome.app/Contents/MacOS/Google Chrome", "--version");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            
            if (!string.IsNullOrWhiteSpace(version))
                return version.Trim();
            return null;
        }
        
        private static string? TryGetChromeVersion(string pathToChromeBinary) {
            string? version = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                version = Utils.GetProcessData("powershell",
                    $"(Get-Item \"{pathToChromeBinary}\").VersionInfo.ProductVersion");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var output = Utils.GetProcessData("/bin/bash", $"-c \"echo `{pathToChromeBinary} --version`\"");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                var output = Utils.GetProcessData(pathToChromeBinary, "--version");
                // Trying to format value if it's not null
                if (output is not null)
                    version = output.Split(' ')[^1];
            }
            // Format version if it was successfully parsed
            if (!string.IsNullOrWhiteSpace(version))
                return version.Trim();
            return null;
        }

        private static string? TryGetChromeBinaryPath() {
            string? pathToChromeBinary = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Path in registry to check version
                const string registryPathBase = @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*";
                const string registryPath64Bit = @"HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*";
                // Note(Nik4ant): Could use Microsoft.Win32.Registry class, but this implementation is easier
                if (Environment.Is64BitOperatingSystem) {
                    pathToChromeBinary = Utils.GetProcessData("powershell",  
                        $"/command (Get-ItemProperty {registryPathBase}, {registryPath64Bit} | Where-Object DisplayName -Like \"*chrome*\").DisplayIcon");
                }
                else {
                    pathToChromeBinary = Utils.GetProcessData("powershell",
                        $"/command (Get-ItemProperty {registryPathBase} | Where-Object DisplayName -Like \"*chrome*\").DisplayIcon");
                }
                // String from registry ends with ",0" and spaces
                pathToChromeBinary = pathToChromeBinary!.TrimEnd().TrimEnd(',', '0');
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                pathToChromeBinary = Utils.GetProcessData("/bin/bash", "-c \"echo `whereis google-chrome`\"");
                // It's most likely that first one is google chrome executable
                // Output looks like this: google-chrome: *path 1* *path 2*
                pathToChromeBinary = pathToChromeBinary!.Split(' ')[1];
            }
            return pathToChromeBinary;
        }
        
        private static string? HandleSelectChromeDialog() {
            // In case if user has chrome installed we ask him to select it
            while (Utils.AskUserInput("Do you want to select path to [Yellow]chrome executable[/Yellow]?", 
                true)) {
                string? newPathToChrome = Utils.SelectSingleFileDialog("Chrome");
                if (!string.IsNullOrEmpty(newPathToChrome)) {
                    ColorConsole.WriteLine($"[Green]Path to chrome was specified[/Green]: {newPathToChrome}");
                    return newPathToChrome;
                }
                ColorConsole.Error("Couldn't get selected file! Try again if you want");
            }
            return null;
        }

        #region Cross platform chrome installation
        private static void HandleChromeInstallation()  {
            ColorConsole.Warning("Program can install latest chrome installer and run it");
            if (Utils.AskUserInput("Install and run chrome installer?")) {
                // Note(Nik4ant) On linux there is only installation package.
                // And program don't have enough permission to automatically unpack it.
                // Way to somehow unpack it would be awesome improvement
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