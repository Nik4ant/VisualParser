using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisualParser.Data
{
    public class AppInfoContainer {
        public string ChromeVersion { get; init; }
        public string PathToDriver { get; private set; }
        public string? CustomPathToChrome { get; private set; }
        
        // Paths to Static and Driver folders with default values
        #if DEBUG
            [JsonIgnore]
            public string PathToDriverFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Driver";
            [JsonIgnore]
            public string PathToStaticFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            [JsonIgnore]
            public string PathToDriverFolder { get; private set; } = $"{Path.DirectorySeparatorChar}Driver";
            [JsonIgnore]            
            public string PathToStaticFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}Static";
        #endif

        public void SetPathToDriver(string path) {
            PathToDriver = path;
        }
        public void SetCustomPathToChrome(string path) {
            CustomPathToChrome = path;
        }
        
        // TODO: fix it
        #region json stuff
        public static AppInfoContainer? LoadFromJson() {
            return JsonSerializer.Deserialize<AppInfoContainer>(Globals.AppInfoPath);
        }
        public void SaveToJson() {
            JsonSerializer.Serialize(this);
        }
        #endregion
    }
}