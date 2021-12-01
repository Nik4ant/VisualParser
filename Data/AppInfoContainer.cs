using System.IO;
using System.Text.Json;

namespace VisualParser.Data
{
    public class AppInfoContainer {
        public string ChromeVersion { get; init; }
        public string PathToDriver { get; private set; }
        
        // Paths to Static and Driver folders with default values
        #if DEBUG
            public string PathToDriverFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Driver";
            public string PathToStaticFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            public string PathToDriverFolder { get; private set; } = $"{Path.DirectorySeparatorChar}Driver";
            public string PathToStaticFolder { get; private set; } = $"..{Path.DirectorySeparatorChar}Static";
        #endif

        public void SetPathToDriver(string path) {
            PathToDriver = path;
        }

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