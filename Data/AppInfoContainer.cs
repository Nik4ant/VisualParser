using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VisualParser.Data
{
    public class AppInfoContainer {
        // Paths to Static and Driver folders with default values
        #if DEBUG
            // Note(Nik4ant): Looks bad, i know. Can't make it better
            public string PathToDriverFolder = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Drivers";
            public string PathToStaticFolder = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            public string PathToDriverFolder = $"{Path.DirectorySeparatorChar}Drivers";
            public string PathToStaticFolder = $"..{Path.DirectorySeparatorChar}Static";
        #endif
        
        // Path to driver binary itself (by default set after driver was
        // loaded or after existing driver was found)
        // FIXME: for no reason this field deserialize as null?!!??!?!??!?!??
        public string PathToDriverBinary { get; private set; }
        
        // Current user info (OS, browser and etc.)
        public UserInfoContainer User { get; private set; } = new UserInfoContainer();
        
        #region Json save/load
        /// <summary>
        /// Method loads values from .json file
        /// </summary>
        /// <param name="path">Path to .json file</param>
        /// <returns>Container object</returns>
        public void LoadValuesFromJson(string path = Globals.AppInfoPath) {
            var container = JsonConvert.DeserializeObject<AppInfoContainer>(File.ReadAllText(path));
            PathToDriverBinary = container.PathToDriverBinary;
            PathToDriverFolder = container.PathToDriverFolder;
            PathToStaticFolder = container.PathToStaticFolder;
            User = container.User;
        }

        /// <summary>
        /// Saves current container to .json file
        /// </summary>
        /// <param name="path">Path to .json file</param>
        public void SaveToJson(string path) {
            using (StreamWriter streamWriter = new StreamWriter(path))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter)) {
                new JsonSerializer().Serialize(jsonWriter, this);
            }
        }

        /// <summary>
        /// Saves current container to .json file (but it using path from Globals.cs)
        /// </summary>
        public void SaveToJson() {
            using (StreamWriter streamWriter = new StreamWriter(Globals.AppInfoPath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter)) {
                new JsonSerializer().Serialize(jsonWriter, this);
            }
        }
        #endregion
        
        public void SetUserInfo(string browserName, string browserVersion, 
            BrowserType browserType) {
            User.BrowserName = browserName;
            User.BrowserVersion = browserVersion;
            User.Browser = browserType;
        }
        
        /// <summary>
        /// Gets first file from driver folder and set this to driver binary
        /// </summary>
        /// <exception cref="FileNotFoundException">If folder would be empty</exception>
        public void UpdatePathToDriverBinary() {
            var files = Directory.GetFiles(PathToDriverFolder);
            if (files.Length > 1)
                ColoredConsole.Warning("WARNING! There is a files in drivers folder already. This might be an issue!");
            else if (files.Length == 0)
                ColoredConsole.Warning("There is no driver downloaded already...");
            else
                PathToDriverBinary = files[0];
        }
        
        public void UpdatePathToDriverBinary(string path) { PathToDriverBinary = path; }
    }
}