using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.Compression;
using HtmlAgilityPack;

namespace VisualParser.Core
{
    public static class ChromeDriverLoader {
         // Link with all chrome drivers data
        private const string ChromeDriversLink = "https://chromedriver.chromium.org/downloads";
        
        /// <summary>
        /// Method loads compatible driver for given browser version.
        /// (If there is driver already it will be overwritten)
        /// </summary>
        /// <param name="browserVersion">Current chrome version</param>
        public static void Load(string browserVersion) {
            string pathToDriverZip = Globals.Info.PathToDriverFolder + $"{Path.DirectorySeparatorChar}driver.zip";
            string downloadUrl = GetDownloadUrl(browserVersion);
            // Creating "Driver" folder if it doesn't exists
            Directory.CreateDirectory(Globals.Info.PathToDriverFolder);
            // Downloading chrome driver
            Utils.DownloadFileByUrl(downloadUrl, Path.GetFullPath(pathToDriverZip));
            try {
                using (var readStream = File.OpenRead(pathToDriverZip))
                using (var zipArchive = new ZipArchive(readStream, ZipArchiveMode.Read)) {
                    // .zip file has only one entry with chromedriver.exe
                    var driverZipEntry = zipArchive.Entries[0];
                    string pathToDriverBinary = Path.GetFullPath(Path.Combine(Globals.Info.PathToDriverFolder,
                        driverZipEntry.FullName));
                    // Extracting file
                    driverZipEntry.ExtractToFile(pathToDriverBinary, true);
                    // Updating path to driver
                    // Note(Nik4ant): Could use "chromedriver.exe" string, but this can be an issue if name changes
                    Globals.Info.SetPathToDriver(pathToDriverBinary);
                }
                // Saving updated info after driver was downloaded
                Globals.Info.SaveToJson();
                // Deleting .zip file
                File.Delete(pathToDriverZip);
            }
            catch (IOException e) {
                ColorConsole.WriteLine("[Red]ERROR![/Red] Something went wrong during zip extracting");
                ColorConsole.WriteLine($"Path: [Red]{pathToDriverZip}[/Red]");
                ColorConsole.WriteLine($"Error text: [Red]{e.Message}[/Red]");
            }
            // On Linux/macOS, you need to add the executable permission (+x) to allow the execution of the chromedriver
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Utils.ExecuteProcess("chmod", $"+x {pathToDriverZip}");
            // TODO: cdc_ string
            RemoveCdcString();
            ColorConsole.WriteLine("Removed cdc_ string successfully (NOPE)", ConsoleColor.Green);
        }

        private static void RemoveCdcString() {
            // TODO:
        }
        
        private static string GetDownloadUrl(string browserVersion) {
            StringBuilder downloadUrl = new("https://chromedriver.storage.googleapis.com/");
            // Adding driver version to url
            downloadUrl.Append($"{GetCompatibleVersion(browserVersion.Split('.')[0])}/chromedriver_");
            // Adding filename for each platform
            // Note(Nik4ant): There is no check for 64 and 32 bit because only windows driver is 32-bit
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                downloadUrl.Append("win32.zip");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                downloadUrl.Append("linux64.zip");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                // Check for Mac M1 processor
                string? processorName = Utils.GetProcessData("terminal", "sysctl -a | grep machdep.cpu.brand_string");
                if (processorName!.Contains("M1"))
                    downloadUrl.Append("mac64_m1.zip");
                else
                    downloadUrl.Append("mac64.zip");
            }
            return downloadUrl.ToString();
        }
        
        private static string GetCompatibleVersion(string majorBrowserVersion) {
            // Load html to document
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(new HtmlWeb().Load(ChromeDriversLink).Text);
            // Selecting needed nodes
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='XqQF9c'][@target='_blank']");
            // Selecting only driver version and return it if it match given major version
            foreach (var node in nodes) {
                if (node.InnerText.Contains('.')) {
                    var version = node.InnerText.Split(' ')[1].Split('.');
                    if (version[0] == majorBrowserVersion)
                        return string.Join('.', version);
                }
            }
            throw new Exception(
                $"Couldn't find compatible driver for browser with major version: {majorBrowserVersion}");
;        }

    }
}