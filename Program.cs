using System;
using System.IO;
using System.Threading.Tasks;
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
            ColoredConsole.WriteLine("Looking for a driver...", ConsoleColor.DarkYellow);
            // Handling each driver before launching locator
            switch (Globals.CurrentUserInfo.Browser) {
                case BrowserType.Chrome:
                    ChromeDriverLoader.Load(Globals.CurrentUserInfo.BrowserVersion);
                    break;
                case BrowserType.Firefox:
                    FirefoxDriverLoader.Load(Globals.CurrentUserInfo.BrowserVersion);
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
            }
            LocatorStartUp.Launch();
        }
    }
}
