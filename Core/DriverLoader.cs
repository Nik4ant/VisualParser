using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VisualParser.Core
{
    /*
    "static abstract" or "static virtual" feature not available in C# yet, so:
    1) Have to create Instance field in subclasses of BaseDriverLoader
    for calling '_LoadAsync' from base class. (If i created instance in base class
    it would call base virtual method)
    2) For easier use there is public static method LoadAsync 
    (instead of calling if from Instance field)
    
    Can't find another proper solution.
    */

    class BaseDriverLoader {
        protected async Task _LoadAsync(string browserVersion) {
            // Creating "Drivers" folder if it doesn't exists
            Directory.CreateDirectory(Globals.PathToDriverFolder);
            // Relative and absolute paths to .zip with downloaded driver
            var pathToDriver = Globals.PathToDriverFolder + "\\driver.zip";
            // Downloading driver
            await Utils.DownloadFileByUrlAsync(await _GetDownloadUrlAsync(browserVersion), Path.GetFullPath(pathToDriver));
            // Extracting downloaded .zip
            try {
                ZipFile.ExtractToDirectory(pathToDriver, Globals.PathToDriverFolder);
                ColoredConsole.WriteLine($"[Green]Driver was loaded[/Green]. Relative path to folder: [Yellow]{Globals.PathToDriverFolder}[/Yellow]");
            }
            catch (IOException) {
                ColoredConsole.WriteLine("[Red]ERROR![/Red] Driver is downloaded already or something went wrong");
                ColoredConsole.WriteLine($"Relative path to folder: [Red]{Globals.PathToDriverFolder}[/Red]");
            }
            // Removing .zip file
            File.Delete(pathToDriver);
        }
        protected virtual async Task<string> _GetDownloadUrlAsync(string browserVersion) { throw new NotImplementedException(); }
    }

    class ChromeDriverLoader : BaseDriverLoader {
        private static readonly ChromeDriverLoader Instance = new ChromeDriverLoader();
        // Link with all chrome drivers data
        private const string ChromeDriversLink = "https://chromedriver.chromium.org/downloads";
        // TODO: small one line comment for each method
        public static async Task LoadAsync(string browserVersion) { await Instance._LoadAsync(browserVersion); }
        
        protected override async Task<string> _GetDownloadUrlAsync(string browserVersion) {
            StringBuilder downloadUrl = new StringBuilder();
            downloadUrl.Append("https://chromedriver.storage.googleapis.com/");
            // All drivers versions
            var driversVersions = await GetAllVersionsAsync();
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
        
        private static async Task<Dictionary<string, string>> GetAllVersionsAsync() {
            // Result with data like this: [*major version*]: *full version*
            var result = new Dictionary<string, string>();
            // Load html to document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(await new HttpClient().GetStringAsync(ChromeDriversLink));
            // Selecting needed nodes
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='XqQF9c'][@target='_blank']");
            // Selecting only drivers version
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
}