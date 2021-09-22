using System;
using System.IO;
using System.Text.Json.Serialization;

namespace VisualParser.Data
{
    public class AppInfoContainer : BaseContainer<AppInfoContainer>
    {
        // Paths to Static and Driver folders with default values
        #if DEBUG
            // Note(Nik4ant): Looks kinda bad, i know. Can't make it shorter
            public string PathToDriverFolder { get; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Drivers";
            public string PathToStaticFolder { get; } = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Static";
        #else
            public string PathToDriverFolder { get; } = $"{Path.DirectorySeparatorChar}Drivers";
            public string PathToStaticFolder { get; } = $"..{Path.DirectorySeparatorChar}Static";
        #endif
        
        // Path to driver binary itself (by default set after driver was
        // loaded or after existing driver was found)
        public string PathToDriverBinary { get; private set; }

        /// <summary>
        /// Gets first file from driver folder and set this to driver binary
        /// </summary>
        /// <exception cref="FileNotFoundException">If folder would be empty</exception>
        public void SetPathToDriverBinary() {
            var files = Directory.GetFiles(PathToDriverFolder);
            // > 2 because of temp file during driver downloading process
            if (files.Length > 2)
                ColoredConsole.Warning("WARNING! There is a files in drivers folder already. This might be an issue!");
            else if (files.Length == 0)
                throw new FileNotFoundException($"There is no files in \"{PathToDriverFolder}\"");
            PathToDriverBinary = files[0];
        }
        
        public void SetPathToDriverBinary(string path) { PathToDriverBinary = path; }
    }
}