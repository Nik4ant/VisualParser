using System.IO;
using System.Text.Json;

namespace VisualParser.Data
{
    public class BaseContainer<T> where T : class {
        /// <summary>
        /// Loads container from .json file
        /// </summary>
        /// <param name="path">Path to .json file</param>
        /// <returns>Container object</returns>
        public static T LoadFromJson(string path) {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
        }
        
        /// <summary>
        /// Saves current container to .json file
        /// </summary>
        /// <param name="path">Path to .json file</param>
        public void SaveToJson(string path) {
            File.WriteAllText(path, JsonSerializer.Serialize(this));
        }
    }
}