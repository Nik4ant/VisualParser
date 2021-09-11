using System;
using System.IO;
using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Path to Static and Driver folders
        #if DEBUG
            // Note(Nik4ant): Looks kinda bad, i know. Can't make it shorter
            public static readonly string PathToDriverFolder = @$"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Drivers";
            public static readonly string PathToStaticFolder = @$"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            public static readonly string PathToDriverFolder = $".{Path.DirectorySeparatorChar}Drivers";
            public static readonly string PathToStaticFolder = @$".{Path.DirectorySeparatorChar}Static";
        #endif
        public static readonly string PathToHomePage = @$"{PathToStaticFolder}{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}HomePage{Path.DirectorySeparatorChar}index.html";
        // Path to driver binary itself (set after driver was loaded or
        // after existing driver was found)
        public static string PathToDriver { get; private set; }
        // Filename of json file with app's data
        public const string AppDataFilename = "app_data.json";
        // Info about current user (OS, browser and etc.)
        public static UserInfoContainer CurrentUserInfo { get; } = new UserInfoContainer();

        public static void SetPathToDriverBinary() {
            PathToDriver = Directory.GetFiles(PathToDriverFolder)[0];
        }
        
        public static void SetUserInfo(UserInfoContainer info) {
            CurrentUserInfo.BrowserName = info.BrowserName;
            CurrentUserInfo.BrowserVersion = info.BrowserVersion;
            CurrentUserInfo.Browser = info.Browser;
        }
        
        public static void SetUserInfo(string browserName, string browserVersion, 
            BrowserType browserType) {
            CurrentUserInfo.BrowserName = browserName;
            CurrentUserInfo.BrowserVersion = browserVersion;
            CurrentUserInfo.Browser = browserType;
        }
    }
}