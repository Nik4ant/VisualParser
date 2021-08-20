using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Path for downloading driver
        #if DEBUG
            public const string PathToDriverFolder = @"..\..\..\Drivers";
        #else
            // TODO: test this path
            public const string PathToDriverFolder = ".\Drivers";
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