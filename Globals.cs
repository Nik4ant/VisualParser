using System;
using System.IO;
using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Filename of .json file with app info
        public const string AppInfoFilename = "app_info.json";
        // Filename of .json file with user info
        public const string UserInfoFilename = "user_info.json";
        // Current user info (OS, browser and etc.)
        public static readonly UserInfoContainer UserInfo = new UserInfoContainer();
        // App info (customizable stuff)
        public static readonly AppInfoContainer AppInfo = InitAppInfo();

        private static AppInfoContainer InitAppInfo() {
            if (File.Exists(AppInfoFilename))
                return AppInfoContainer.LoadFromJson(AppInfoFilename);
            ColoredConsole.Debug("I AM A FUCKING IDIOT THIS SHIT DOESN'T WORKING AT ALL");
            ColoredConsole.Debug("I NEED SOMEHOW MERGE USER INFO AND APP INFO");
            ColoredConsole.Debug("(MAYBE MAKE GLOBALS.CS TO JSON IDK)");
            ColoredConsole.Debug("\n\nPLEASE HELP ME!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return new AppInfoContainer();;
        }
        
        public static void SetUserInfo(UserInfoContainer container) {
            UserInfo.BrowserName = container.BrowserName;
            UserInfo.BrowserVersion = container.BrowserVersion;
            UserInfo.Browser = container.Browser;
        }    
        
        public static void SetUserInfo(string browserName, string browserVersion, 
            BrowserType browserType) {
            UserInfo.BrowserName = browserName;
            UserInfo.BrowserVersion = browserVersion;
            UserInfo.Browser = browserType;
        }
    }
}