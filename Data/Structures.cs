using System;
using System.Runtime.InteropServices;

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

    public static class GlobalUserInfo {
        // User's OS
        public static OSPlatform OS { get; }
        // User's browser
        public static string BrowserName { get; set; }
        public static string BrowserVersion { get; set; }
        public static BrowserType Browser { get; set; }
        
        static GlobalUserInfo() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                OS = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                OS = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                OS = OSPlatform.OSX;
            }
        }
    }
}