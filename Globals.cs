using System;
using System.IO;
using VisualParser.Data;

namespace VisualParser {
    public static class Globals {
        // Paths to Static and Driver folders
        #if DEBUG
            // Note(Nik4ant): Looks kinda bad, i know. Can't make it shorter
            public static readonly string PathToDriverFolder = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Drivers";
            public static readonly string PathToStaticFolder = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            public static readonly string PathToDriverFolder = $".{Path.DirectorySeparatorChar}Drivers";
            public static readonly string PathToStaticFolder = $".{Path.DirectorySeparatorChar}Static";
        #endif
        // Path to driver binary itself (set after driver was loaded or
        // after existing driver was found)
        public static string PathToDriverBinary { get; private set; }
        // Filename of json file with app's data
        public const string AppDataFilename = "app_data.json";
        // Info about current user (OS, browser and etc.)
        public static UserInfoContainer CurrentUserInfo { get; } = new UserInfoContainer();
        
        /// <summary>
        /// Method returns valid uri for given path
        /// (used to open local pages in WebDriver)
        /// </summary>
        /// <param name="pageFilenamePath">Absolute path to filename of page</param>
        /// <returns>Valid uri for path</returns>
        public static string GetPageUri(string pageFilenamePath) {
            return new Uri(pageFilenamePath).AbsoluteUri;
        }
        
        /// <summary>
        /// Method returns valid uri for file in page
        /// (used to open local pages in WebDriver)
        /// </summary>
        /// <param name="pageName">Name of page</param>
        /// <param name="filename">Filename inside of page</param>
        /// <returns>Valid uri for filename in page</returns>
        public static string GetPageUri(string pageName, string filename) {
            return GetPageUri(GetStaticItemFullPath(pageName, filename));
        }
        
        /// <summary>
        /// Method return relative path to filename somewhere in Static folder
        /// </summary>
        /// <param name="folderPath">Path to folder with file</param>
        /// <param name="filename">Filename of file</param>
        /// <returns>Relative path to file</returns>
        public static string GetStaticItemPath(string folderPath, string filename) {
            return $"{PathToStaticFolder}{Path.DirectorySeparatorChar}{folderPath}{Path.DirectorySeparatorChar}{filename}";
        }
        
        /// <summary>
        /// Method return absolute path to filename somewhere in Static folder
        /// </summary>
        /// <param name="folderPath">Path to folder with file</param>
        /// <param name="filename">Filename of file</param>
        /// <returns>Absolute path to file</returns>
        public static string GetStaticItemFullPath(string folderPath, string filename) {
            return Path.GetFullPath(GetStaticItemPath(folderPath, filename));
        }
        
        public static void SetPathToDriverBinary() {
            PathToDriverBinary = Directory.GetFiles(PathToDriverFolder)[0];
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