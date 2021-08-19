using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using VisualParser;

namespace VisualParser.Data
{
    /// Enum with browsers
    public enum BrowserType : byte {
        Chrome,
        Firefox,
        Opera,
        Safari,
        Edge,
        None
    }
    
    public class UserInfoContainer {
        // TODO: figure out better solution in long future, because calling static field every time sucks
        // TODO: maybe use fields that return Instance field, but that sucks
        public static UserInfoContainer Instance { get; set; } = new UserInfoContainer();
        
        // User's OS
        [NonSerialized]
        public static OSPlatform OS = GetOSPlatform();
        // User's browser
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public BrowserType Browser { get; set; }
        
        public static void UpdateInfo() {
            // Collecting formatted browser's name and type
            string newBrowserName = UserInfoManager.FormatBrowserName(
                UserInfoManager.GetDefaultBrowserName(), out var newBrowserType);
            // Updating data
            UpdateInfo(newBrowserName, UserInfoManager.GetBrowserVersion(newBrowserName), 
                newBrowserType);
        }
        
        public static void UpdateInfo(string browserName, string browserVersion, 
            BrowserType browserType) {
            Instance.BrowserName = browserName;
            Instance.BrowserVersion = browserVersion.Trim();
            Instance.Browser = browserType;
        }
        
        public static void UpdateInfo(string pathToJson) {
            var loadedContainer = LoadFromJson(pathToJson);
            Instance.BrowserName = loadedContainer.BrowserName;
            Instance.BrowserVersion = loadedContainer.BrowserVersion;
            Instance.Browser = loadedContainer.Browser;
        }
        
        public static async Task UpdateInfoAsync(string pathToJson) {
            var loadedContainer = await LoadFromJsonAsync(pathToJson);
            Instance.BrowserName = loadedContainer.BrowserName;
            Instance.BrowserVersion = loadedContainer.BrowserVersion;
            Instance.Browser = loadedContainer.Browser;
        }
        
        private static UserInfoContainer LoadFromJson(string path) { 
            return JsonSerializer.Deserialize<UserInfoContainer>(File.ReadAllText(path));
        }
        
        private static async Task<UserInfoContainer> LoadFromJsonAsync(string path) { 
            using FileStream readStream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<UserInfoContainer>(readStream);
        }
        
        public static async Task SaveToJsonAsync(string path) {
            using FileStream saveStream = File.Create(path);
            await JsonSerializer.SerializeAsync(saveStream, Instance);
        }
        
        public static void SaveToJson(string path) {
            JsonSerializer.Serialize(Instance);
        }

        private static OSPlatform GetOSPlatform() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return OSPlatform.OSX;
            }
            throw new NotSupportedException("Are you running this on toaster?!");
        }
    }
}