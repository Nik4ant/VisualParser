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
                    Utils.ColoredWriteLine("{r}ERROR!\\{r} Bad data or something went wrong");
                    Console.WriteLine("Updating data manually...");
                    UserInfoContainer.UpdateInfo();
                    await UserInfoContainer.SaveToJsonAsync(AppDataFilename);
                }
            }
            else {
                UserInfoContainer.UpdateInfo();
                await UserInfoContainer.SaveToJsonAsync(AppDataFilename);
            }
            
            Utils.ColoredWriteLine($"Name: {{y}}{UserInfoContainer.Instance.BrowserName}\\{{y}}");
            Utils.ColoredWriteLine($"Version: {{g}}{UserInfoContainer.Instance.BrowserVersion}\\{{g}}");

            await ChromeDriverLoader.LoadAsync(UserInfoContainer.Instance.BrowserVersion);
            
            Console.Write("\nPress any key: ");
            Console.ReadKey();
        }
    }
}
