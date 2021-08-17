using System;
using System.Threading.Tasks;
using VisualParser.Data;

namespace VisualParser
{
    class Program {
        static async Task Main(string[] args) {
            // This collects all needed data and store it to global info object (static class)
            UserInfoCollector.UpdateGlobalInfo();

            Utils.ColoredWriteLine($"Name: {{y}}{GlobalUserInfo.BrowserName}\\{{y}}");
            Utils.ColoredWriteLine($"Version: {{g}}{GlobalUserInfo.BrowserVersion}\\{{g}}");

            await ChromeDriverLoader.LoadAsync(GlobalUserInfo.BrowserVersion);
            
            Console.Write("\nPress any key: ");
            Console.ReadKey();
        }
    }
}
