using System;
using System.IO;
using System.Threading.Tasks;
using VisualParser.Core;
using VisualParser.Data;
using VisualParser.Selector;

namespace VisualParser
{
    class Program {
        static async Task Main(string[] args) {
            // This collects all needed data and store it to global info
            // object (static readonly Instance of UserInfoContainer)
            if (File.Exists(Globals.AppDataFilename)) {
                Console.WriteLine("Loading from json...");
                // If program can't load data from .json file it will update manually
                // and save new data to .json
                try {
                    await UserInfoManager.UpdateAsync(Globals.AppDataFilename);
                }
                catch (Exception e) {
                    ColoredConsole.WriteLine("[Red]ERROR![/Red] Bad data or something went wrong");
                    Console.WriteLine("Updating data manually...");
                    UserInfoManager.Update();
                    await UserInfoManager.SaveToJsonAsync(Globals.AppDataFilename);
                }
            }
            else {
                UserInfoManager.Update();
                await UserInfoManager.SaveToJsonAsync(Globals.AppDataFilename);
            }
            
            ColoredConsole.WriteLine($"Name: [Yellow]{Globals.CurrentUserInfo.BrowserName}[/Yellow]");
            ColoredConsole.WriteLine($"Version: [Yellow]{Globals.CurrentUserInfo.BrowserVersion}[/Yellow]\n");
            ColoredConsole.WriteLine("Looking for a driver...", ConsoleColor.DarkYellow);
            // Downloading needed driver only if it's not exists already
            if (Directory.GetFiles(Globals.PathToDriverFolder).Length == 0) {
                switch (Globals.CurrentUserInfo.Browser) {
                    case BrowserType.Chrome:
                        await ChromeDriverLoader.LoadAsync(Globals.CurrentUserInfo.BrowserVersion);
                        break;
                    case BrowserType.Firefox:
                        await FirefoxDriverLoader.LoadAsync(Globals.CurrentUserInfo.BrowserVersion);
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
            }
            else {
               ColoredConsole.WriteLine("Driver already exists\n", ConsoleColor.Green); 
            }
            LocatorStartUp.Launch();
            
            /*Console.Write("\nPress any key: ");
            Console.ReadKey();*/
        }
    }
}
