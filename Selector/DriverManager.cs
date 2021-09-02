using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;
using VisualParser.Data;

namespace VisualParser.Selector
{
    public static class DriverManager {
        public static ChromeDriver GetConfiguredChromeDriver() {
            // TODO: rewrite method from backup.txt (now this is just test setup)
            List<string> chromeArgumentsToAdd = new List<string> {
                "start-maximized",
                $"user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{Globals.CurrentUserInfo.BrowserVersion} Safari/537.36",
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
                "log-level=0", 
                "disable-sync",
            };
            // Capabilities
            Dictionary<string, object> chromeCapabilities = new Dictionary<string, object> {
                {"useAutomationExtension", false},
            };
            // Options for driver
            var driverOptions = new ChromeOptions {
                BrowserVersion = Globals.CurrentUserInfo.BrowserVersion,
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
            // TODO: more info: https://stackoverflow.com/questions/31067404/how-to-create-a-chrome-profile-programmatically
            // Internet speed throughput for driver in kb/s
            // long internetSpeedForDriver = CalculateInternetThroughput();
            
            // Driver
            return new ChromeDriver(Globals.PathToDriverFolder, driverOptions);
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