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
            ColoredConsole.WriteLine($"Name: [Yellow]{Globals.CurrentUserInfo.BrowserName}[/Yellow]");
            ColoredConsole.WriteLine($"Version: [Yellow]{Globals.CurrentUserInfo.BrowserVersion}[/Yellow]\n");
            // Driver for locator (will be init down below)
            RemoteWebDriver driver = default;
            // Handling each driver before launching locator
            switch (Globals.CurrentUserInfo.Browser) {
                case BrowserType.Chrome:
                    bool isDriverLoadedRecently = ChromeDriverLoader.Load(Globals.CurrentUserInfo.BrowserVersion);
                    driver = DriverManager.GetConfiguredChromeDriver(isDriverLoadedRecently);
                    break;
                case BrowserType.Firefox:
                    FirefoxDriverLoader.Load(Globals.CurrentUserInfo.BrowserVersion);
                    ColoredConsole.Debug("Only driver loading process working now");
                    throw new NotImplementedException();
                    break;
                case BrowserType.Edge:
                    throw new NotImplementedException();
                    break;
                case BrowserType.Opera:
                    throw new NotImplementedException();
                    break;
                case BrowserType.Safari:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotSupportedException();
            }
            LocatorStartUp.Launch(driver);
        }
    }
}
