using System;
using System.IO;
using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Filename of .json file with app info
        public const string AppInfoPath = "app_info.json";
        // App info (customizable stuff and info about current user)
        public static readonly AppInfoContainer AppInfo = new AppInfoContainer();
    }
}