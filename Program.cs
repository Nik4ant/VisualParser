using System;
using OpenQA.Selenium.Remote;
using VisualParser.Core;
using VisualParser.Data;
using VisualParser.Locator;

namespace VisualParser
{
    static class Program {
        static void Main(string[] args) {
            // Processing all needed info
            UserInfoManager.HandleInfo();
            ColoredConsole.WriteLine($"Name: [Yellow]{Globals.UserInfo.BrowserName}[/Yellow]");
            ColoredConsole.WriteLine($"Version: [Yellow]{Globals.UserInfo.BrowserVersion}[/Yellow]\n");
            // Driver for locator (will be init down below)
            RemoteWebDriver driver = default;
            // Handling each driver before launching locator
            switch (Globals.UserInfo.Browser) {
                case BrowserType.Chrome:
                    bool isDriverLoadedRecently = ChromeDriverLoader.Load(Globals.UserInfo.BrowserVersion);
                    driver = DriverManager.GetConfiguredChromeDriver(isDriverLoadedRecently);
                    break;
                default:
                    ColoredConsole.WriteLine("[Red]ERROR![/Red] Your browser isn't supported");
                    throw new NotSupportedException();
            }
            Locator.Locator.Launch(driver);
            // Updating app info
            // TODO: figure out better way for saving customizable stuff
            Globals.AppInfo.SaveToJson(Globals.AppInfoFilename);
        }
    }
}
