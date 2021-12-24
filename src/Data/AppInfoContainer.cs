using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisualParser.Data
{
    public class AppInfoContainer {
        // Note(Nik4ant): Init properties is necessary for working
        // other could be null or loaded/set later
        public string ChromeVersion { get; init; }
        public string PathToDriver { get; private set; }
        public string? CustomPathToChrome { get; private set; }
        
        // Paths to Static and Driver folders with default values
        #if DEBUG
            [JsonIgnore]
            public string PathToDriverFolder { get; private set; } = Path.GetFullPath($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Driver");
            [JsonIgnore]
            public string PathToStaticFolder { get; private set; } = Path.GetFullPath($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Static");
        #else
            [JsonIgnore]
            public string PathToDriverFolder { get; private set; } = Path.GetFullPath("Driver");
            [JsonIgnore]      
            public string PathToStaticFolder { get; private set; } = Path.GetFullPath("Static");
        #endif

        public void SetPathToDriver(string path) {
            PathToDriver = path;
        }
        public void SetCustomPathToChrome(string path) {
            CustomPathToChrome = path;
        }
        
        #region json stuff
        public static AppInfoContainer? LoadFromJson() {
            using var streamRead = File.OpenRead(Globals.AppInfoPath);
            return JsonSerializer.Deserialize<AppInfoContainer>(streamRead);
        }
        public void SaveToJson() {
            using var streamWriter = File.OpenWrite(Globals.AppInfoPath);
            JsonSerializer.Serialize(streamWriter, this);
        }
        #endregion
    }
}