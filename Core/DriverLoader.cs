using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HtmlAgilityPack;

namespace VisualParser.Core
{
    /*
    "static abstract" or "static virtual" feature not available in C# yet, so:
    1) Have to create Instance field in subclasses of BaseDriverLoader
    for calling '_Load' from base class. (If i created instance in base class
    it would call base virtual method)
    2) For easier use there is public static method Load
    (instead of calling if from Instance field)
    
    Can't find better solution.
    */

    class BaseDriverLoader {
        /// <summary>
        /// Method that downloads driver that matches given browser version
        /// and return true if driver is new (was downloaded recently) otherwise false
        /// (used to format driver. If driver is new it needs to be formated)  
        /// </summary>
        /// <param name="browserVersion">Version of browser</param>
        /// <returns>true if driver is new (was downloaded recently) otherwise false</returns>
        protected bool _Load(string browserVersion) {
            ColoredConsole.WriteLine("Looking for a driver...", ConsoleColor.DarkYellow);
            // Creating "Drivers" folder if it doesn't exists
            // TODO: for no reason Linux might crash there (research)
            Directory.CreateDirectory(Globals.PathToDriverFolder);
            // Check if driver is downloaded already
            if (Directory.GetFiles(Globals.PathToDriverFolder).Length != 0) {
                // If driver is already there setting path to driver
                Globals.SetPathToDriverBinary();
                // If user want to override existing driver old one will be deleted
                bool overrideFile = Utils.AskUserInput("Driver already exist, do you want to override it? [y/n] ", 
                    ConsoleColor.Yellow);
                if (!overrideFile)
                    return false;
                // To make sure program delete all files inside Driver folder
                foreach (string path in Directory.GetFiles(Globals.PathToDriverFolder))
                    File.Delete(path);
                ColoredConsole.WriteLine("Old driver deleted, downloading new one...", ConsoleColor.DarkYellow);
            }
            // Relative and absolute paths to .zip with downloaded driver
            string pathToDriver = Globals.PathToDriverFolder + $"{Path.DirectorySeparatorChar}driver.zip";
            // Downloading driver
            Utils.DownloadFileByUrl(_GetDownloadUrl(browserVersion), Path.GetFullPath(pathToDriver));
            // Extracting downloaded .zip
            try {
                ZipFile.ExtractToDirectory(pathToDriver, Globals.PathToDriverFolder);
                ColoredConsole.WriteLine($"[Green]Driver was loaded[/Green]. Relative path to folder: [Yellow]{Globals.PathToDriverFolder}[/Yellow]");
                // Setting path to driver after it was downloaded
                Globals.SetPathToDriverBinary();
            }
            catch (IOException) {
                ColoredConsole.WriteLine("[Red]ERROR![/Red] Driver is downloaded already or something went wrong");
                ColoredConsole.WriteLine($"Relative path to folder: [Red]{Globals.PathToDriverFolder}[/Red]");
            }
            // Removing .zip file
            File.Delete(pathToDriver);
            return true;
        }
        
        // Method that parse download url for _Load method
        protected virtual string _GetDownloadUrl(string browserVersion) { throw new NotImplementedException(); }
    }

    class ChromeDriverLoader : BaseDriverLoader {
        private static readonly ChromeDriverLoader Instance = new ChromeDriverLoader();
        // Link with all chrome drivers data
        private const string ChromeDriversLink = "https://chromedriver.chromium.org/downloads";
        public static bool Load(string browserVersion) { return Instance._Load(browserVersion); }
        
        protected override string _GetDownloadUrl(string browserVersion) {
            StringBuilder downloadUrl = new StringBuilder();
            downloadUrl.Append("https://chromedriver.storage.googleapis.com/");
            // All drivers versions
            var driversVersions = GetAllVersions();
            // Adding driver version to url
            downloadUrl.Append($"{driversVersions[browserVersion.Split('.')[0]]}/");
            // Adding filename for each platform
            if (Globals.CurrentUserInfo.OS == OSPlatform.Windows) {
                downloadUrl.Append("chromedriver_win32.zip");
            }
            else if (Globals.CurrentUserInfo.OS == OSPlatform.Linux) {
                downloadUrl.Append("chromedriver_linux64.zip");
            }
            else if (Globals.CurrentUserInfo.OS == OSPlatform.OSX) {
                downloadUrl.Append("chromedriver_mac64.zip");
            }
            
            return downloadUrl.ToString();
        }
        
        private static Dictionary<string, string> GetAllVersions() {
            // Result with data like this: [*major version*]: *full version*
            var result = new Dictionary<string, string>();
            // Load html to document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(new HtmlWeb().Load(ChromeDriversLink).Text);
            // Selecting needed nodes
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='XqQF9c'][@target='_blank']");
            // Selecting only drivers version
            // TODO: optimise this later
            var driverVersions = nodes.Where(node => node.InnerText.Contains('.')).Select(
                x => x.InnerText.Split(' ')[1].Split('.'));
            // Adding only latest versions to result
            foreach (var version in driverVersions) {
                if (!result.ContainsKey(version[0])) {
                    result.Add(version[0], String.Join('.', version));
                }
            }
            return result;
        }

    }
    
    class FirefoxDriverLoader : BaseDriverLoader {
        private static readonly FirefoxDriverLoader Instance = new FirefoxDriverLoader();
        // Link to docs with compatible driver and browser versions
        private const string CompatibleVersionLink =
            "https://firefox-source-docs.mozilla.org/testing/geckodriver/Support.html";
        // Link to driver itself
        private const string FirefoxDriverLink = "https://github.com/mozilla/geckodriver/releases/download/";
        
        public static bool Load(string browserVersion) { return Instance._Load(browserVersion); }
        
        protected override string _GetDownloadUrl(string browserVersion) {
            var downloadUrl = new StringBuilder(FirefoxDriverLink);
            // Parsing compatible driver version
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(new HtmlWeb().Load(CompatibleVersionLink).Text);
            // Major version of browser
            ColoredConsole.Debug(browserVersion.Split('.')[0]);
            short searchingBrowserVersion = short.Parse(browserVersion.Split('.')[0]);
            // Selecting first driver version that match to given browser version
            var tableNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"supported-platforms\"]/table");
            foreach (HtmlNode columnNode in tableNode.SelectNodes("./tr")) {
                short minVersion = short.Parse(columnNode.ChildNodes[3].InnerText.Trim());
                // Check if limit for browser version exists
                short maxVersion = short.MaxValue;
                string formattedVersion = columnNode.LastChild.InnerText.Trim();
                if (formattedVersion != "n/a") {
                    maxVersion = short.Parse(formattedVersion);
                }
                // Compare given browser version to min and max versions range
                if (minVersion <= searchingBrowserVersion && searchingBrowserVersion <= maxVersion) {
                    var driverVersion = columnNode.ChildNodes[1].InnerText.Trim();
                    downloadUrl.Append($"v{driverVersion}/geckodriver-v{driverVersion}-");
                    break;
                }
            }
            // Adding OS to download url
            if (Globals.CurrentUserInfo.OS == OSPlatform.Windows) {
                downloadUrl.Append("win32.zip");
            }
            else if (Globals.CurrentUserInfo.OS == OSPlatform.Linux) {
                downloadUrl.Append("linux32.tar.gz");
            }
            else if (Globals.CurrentUserInfo.OS == OSPlatform.OSX) {
                downloadUrl.Append("macos.tar.gz");
            }
            else {
                throw new NotSupportedException("Are you running this on toaster?");
            }
            return downloadUrl.ToString();
        }
    }
}