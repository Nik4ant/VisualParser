using Newtonsoft.Json;

namespace VisualParser.Data
{
    public class ApplicationInfo
    {
        /// <summary> Info about chrome on user's machine </summary>
        // TODO: WTF?! Figure out that magic (i have no idea why it makes code work)
        [JsonProperty("Chrome")]
        public ChromeBrowserInfo Chrome { get; private set; }
        /// <summary> Path to last used chrome driver </summary>
        public string? PathToDriver { get; private set; }

        public ApplicationInfo(string? pathToDriver, ChromeBrowserInfo chromeInfo) {
            PathToDriver = pathToDriver;
            Chrome = chromeInfo;
        }
        
        #region property setters
        public void SetPathToDriver(string? path) { PathToDriver = path; }
        public void SetChromeInfo(ChromeBrowserInfo chromeInfo) { Chrome = chromeInfo; }
        #endregion
        
        #region json save/load
        public static ApplicationInfo? LoadFromJson() {
            using var streamReader = new StreamReader(File.OpenRead(PathToAppInfo));
            using var jsonReader = new JsonTextReader(streamReader);
            return new JsonSerializer().Deserialize<ApplicationInfo>(jsonReader);
        }
        public void SaveToJson() {
            using var streamWriter = new StreamWriter(File.OpenWrite(PathToAppInfo));
            using var jsonWriter = new JsonTextWriter(streamWriter);
            new JsonSerializer().Serialize(jsonWriter, this);
        }
        #endregion
    }
}