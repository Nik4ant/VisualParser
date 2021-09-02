using System.IO;
using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Path for downloading driver
        #if DEBUG
            // Note(Nik4ant): Looks kinda bad, i know. Have no idea how make it shorter
            public static readonly string PathToDriverFolder = @$"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Drivers";
        #else
            public static readonly string PathToDriverFolder = $".{Path.DirectorySeparatorChar}Drivers";
        #endif
        // Filename of json file with app's data
        public const string AppDataFilename = "app_data.json";
        // Info about current user (OS, browser and etc.)
        public static UserInfoContainer CurrentUserInfo { get; private set; } = new UserInfoContainer();
        
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