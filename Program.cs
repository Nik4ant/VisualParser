using System;
using System.Collections.Generic;
using VisualParser.Data;

namespace VisualParser
{
    class Program {
        static void Main(string[] args) {
            // This collects all needed data and store it to global info object (static class)
            UserInfoCollector.UpdateGlobalInfo();

            Utils.ColoredWriteLine($"Name: {{y}}{GlobalUserInfo.BrowserName}\\{{y}}");
            Utils.ColoredWriteLine($"Version: {{g}}{GlobalUserInfo.BrowserVersion}\\{{g}}");
        }
    }
}
