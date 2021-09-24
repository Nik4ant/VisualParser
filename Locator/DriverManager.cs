using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace VisualParser.Locator
{
    public static class DriverManager {
        /// <summary>
        /// Method changing chromedriver.exe to avoid detection.
        /// (For example it renames cdc_ string to avoid detection)
        /// </summary>
        private static void FormatChromeDriverBinary() {
            ColoredConsole.Warning("WARNING! DRIVER IS UNFORMATED, YOU CAN BE DETECTED!");
            return;
            // TODO: replace cdc_ string and all that stuff from .exe (please help)
            /*
            // Size of one chunk (for reading binary file)
            const int bufferChunkSize = 1024;
            // Path to temp file for writing all formatted bytes
            string tempDriverPath = $"{Globals.PathToDriverFolder}{Path.DirectorySeparatorChar}temp.exe";
            // Converting all string to bytes for replacing them
            // Note(Nik4ant): Should better make it const, but i can make this customizable in the future
            var bytesToReplace = new Dictionary<byte[], byte[]> {
                {"cdc_".ToByteArray(), "cat_".ToByteArray()},
            };
            int maxLengthToReplace = bytesToReplace.Keys.Aggregate((bytes1, bytes2) =>
                bytes1.Length > bytes2.Length ? bytes1 : bytes2).Length;
            int minLengthToReplace = bytesToReplace.Keys.Min(bytes => bytes.Length);
            // Creating file with size of current driver (for easier memory management)
            long currentDriverFileSize = new FileInfo(Globals.PathToDriverBinary).Length;
            // TODO: bench this thing as well
            using var binaryWriter = new BinaryWriter(File.Create(tempDriverPath, (int)currentDriverFileSize));
            using var binaryReader = new BinaryReader(File.OpenRead(Globals.PathToDriverBinary));
            // Chunk with current data from BinaryReader
            byte[] bufferChunk = binaryReader.ReadBytes(bufferChunkSize);
            // TODO: bench 2 versions:
            // 1) Format and write immediately
            // 2) Load formatted bytes to temp array and then write them
            while (bufferChunk.Length != 0) {
                // Step 1. Looping through bytes
                for (int i=0; i<=bufferChunk.Length - minLengthToReplace; i+=minLengthToReplace) {
                    // Updating last bytes sequence and check if it in array
                    var currentByteSequence = bufferChunk[i..(i + minLengthToReplace)];
                    for (int counter = minLengthToReplace; counter <= maxLengthToReplace; counter++) {
                        // Step 2. Format bytes (if needed) and write them to file
                        if (bytesToReplace.ContainsKey(currentByteSequence)) {
                            ColoredConsole.Debug("HOLY SHIT IT WORKED!");
                            break;
                        }
                        if (currentByteSequence == new byte[]{0x63, 064, 0x63, 0x5f})
                            ColoredConsole.Debug("YES!");
                        currentByteSequence = bufferChunk[i..(i + counter)];
                    }
                    binaryWriter.Write(currentByteSequence);
                }
                // Step 3. Read bytes again
                bufferChunk = binaryReader.ReadBytes(bufferChunkSize);
            }
            binaryWriter.Write(binaryReader.ReadBytes(maxLengthToReplace));
            binaryWriter.Close();
            binaryReader.Close();
            File.Move(tempDriverPath, Globals.PathToDriverBinary, true);
            */
        }

        public static ChromeDriver GetConfiguredChromeDriver(bool isNewDriver=true) {
            // Format driver only if it wasn't formated before
            if (isNewDriver)
                FormatChromeDriverBinary();
            // ColoredConsole.WriteLine("Driver binary was formatted successful!", ConsoleColor.Green);
            List<string> chromeArgumentsToAdd = new List<string> {
                "start-maximized",
                "--no-sandbox",
                "--disable-blink-features",
                "--disable-blink-features=AutomationControlled",
            };
            // Arguments to remove from driver's default arguments list
            List<string> chromeArgumentsToExclude = new List<string> {
                "enable-automation",
                "enable-logging",
                "disable-default-apps",
                "disable-background-networking",
                "disable-backgrounding-occluded-windows",
                "disable-client-side-phishing-detection",
                "disable-hang-monitor",
                "disable-popup-blocking",
                "disable-prompt-on-repost",
                "enable-blink-features=ShadowDOMV0",
                "enable-blink-features=AutomationControlled",
                "log-level=0", 
                "disable-sync",
            };
            // Capabilities
            Dictionary<string, object> chromeCapabilities = new Dictionary<string, object> {
                // TODO: figure out how to remove security extension from browser
                // {"chrome.switches", new List<string>{"--disable-extensions"}},
            };
            // Options for driver
            var driverOptions = new ChromeOptions {
                BrowserVersion = Globals.AppInfo.User.BrowserVersion,
                PageLoadStrategy = PageLoadStrategy.Eager,
            };
            // Arguments
            driverOptions.AddArguments(chromeArgumentsToAdd);
            driverOptions.AddExcludedArguments(chromeArgumentsToExclude);
            // Capabilities
            foreach (var keyValue in chromeCapabilities) {
                driverOptions.AddAdditionalCapability(keyValue.Key, keyValue.Value);
            }
            // TODO: add profile creation feature (it reads all data from main profile, but i will be profile for parsing)
            // Internet speed throughput for driver in kb/s
            // long internetSpeedForDriver = CalculateInternetThroughput();
            // Driver
            var driver = new ChromeDriver(Globals.AppInfo.PathToDriverFolder, driverOptions);
            // Command to hide "navigator" property and be undetected
            driver.ExecuteChromeCommand("Page.addScriptToEvaluateOnNewDocument", 
                new Dictionary<string, object> {
                    {"source", "Object.defineProperty(navigator, 'webdriver', { get: () => undefined })"}
            });
            // User agent
            driver.ExecuteChromeCommand("Network.setUserAgentOverride", 
                new Dictionary<string, object> {
                    {"userAgent", $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{Globals.AppInfo.User.BrowserVersion} Safari/537.36"}
            });
            return driver;
            /*
             Note(Nik4ant): Need better params for this later. Now it just slows all down 
            NetworkConditions = new ChromeNetworkConditions {
                DownloadThroughput = internetSpeedForDriver,
                Latency = new TimeSpan(0, 0, 0, 1, new Random().Next(1, 999)),
                UploadThroughput = internetSpeedForDriver,
            }*/
        }

        // NOTE(Nik4ant): Maybe this can be useful for avoiding selenium detection
        // TODO: not working properly for now (improve later)
        /// <summary>
        /// Method calculates download throughput based on
        /// user's average internet speed
        /// </summary>
        /// <returns>Download throughput</returns>
        private static long CalculateInternetThroughput() {
            long result = 0;
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces()[..^1]) {
                result += adapter.Speed / 8000;
            }
            return result / (NetworkInterface.GetAllNetworkInterfaces().Length - 1);
        }
    }
}