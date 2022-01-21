global using System.IO;
global using System;
global using VisualParser.Data;
global using System.Runtime.InteropServices;
global using static VisualParser.Globals;

namespace VisualParser
{
    public static class Globals {
        // Filename of .json file to save/load application info
        public const string PathToAppInfo = "app_info.json";
        // Paths to Static and Driver folders with default values
        #if DEBUG
            public static string AbsolutePathToDriverFolder { get; } = Path.GetFullPath($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Driver");
            public static string AbsolutePathToStaticFolder { get; } = Path.GetFullPath($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Static");
        #else
            public static string AbsolutePathToDriverFolder { get; } = Path.GetFullPath("Driver");
            public static string AbsolutePathToStaticFolder { get; } = Path.GetFullPath("Static");
        #endif
        
        public static ApplicationInfo AppInfo { get; private set; }
        
        public static void SetGlobalAppInfo(ApplicationInfo newInfo) { AppInfo = newInfo; }
    }
}