global using System;
using VisualParser.Data;

namespace VisualParser
{
    public static class Globals {
        // Filename of .json file with app info
        public const string AppInfoPath = "app_info.json";
        public static AppInfoContainer Info { get; private set; } = new();
        
        public static void SetAppInfo(AppInfoContainer newInfo) { Info = newInfo; }
    }
}