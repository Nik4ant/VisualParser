using System;
using System.IO;
using System.Threading.Tasks;
using VisualParser.Data;

namespace VisualParser
{
    class Program
    {
        const string AppDataFilename = "app_data.json";
        static async Task Main(string[] args) {
            // This collects all needed data and store it to global info
            // object (static readonly Instance of UserInfoContainer)
            if (File.Exists(AppDataFilename)) {
                Console.WriteLine("Loading from json...");
                // If program can't load data from .json file it will update manually
                // and save new data to .json
                try {
                    await UserInfoContainer.UpdateInfoAsync(AppDataFilename);
                }
                catch (Exception e) {
                    ColoredConsole.WriteLine("[Red]ERROR![/Red] Bad data or something went wrong");
                    Console.WriteLine("Updating data manually...");
                    UserInfoContainer.UpdateInfo();
                    await UserInfoContainer.SaveToJsonAsync(AppDataFilename);
                }
            }
            else {
                UserInfoContainer.UpdateInfo();
                await UserInfoContainer.SaveToJsonAsync(AppDataFilename);
            }
            
            ColoredConsole.WriteLine($"Name: [Yellow]{UserInfoContainer.Instance.BrowserName}[/Yellow]");
            ColoredConsole.WriteLine($"Version: [Yellow]{UserInfoContainer.Instance.BrowserVersion}[/Yellow]\n");
            // Downloading needed driver
            ColoredConsole.WriteLine("Looking for a driver...", ConsoleColor.DarkYellow);
            switch (UserInfoContainer.Instance.Browser) {
                case BrowserType.Chrome: 
                    await ChromeDriverLoader.LoadAsync(UserInfoContainer.Instance.BrowserVersion);
                    break;
                case BrowserType.Firefox:
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
            }
            
            /*Console.Write("\nPress any key: ");
            Console.ReadKey();*/
        }
    }
}
