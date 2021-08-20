using System;
using System.Runtime.InteropServices;

namespace VisualParser.Data
{
    public class UserInfoContainer {
        // User's OS
        public OSPlatform OS { get; }
        // User's browser
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public BrowserType Browser { get; set; }
        
        /// <summary>
        /// Constructor for default UserInfoContainer, only with OS field set
        /// </summary>
        public UserInfoContainer() {
            OS = GetOSPlatform();
        }
        
        public UserInfoContainer(string browserName, string browserVersion, 
            BrowserType browserType) {
            BrowserName = browserName;
            BrowserVersion = browserVersion;
            Browser = browserType;
            OS = GetOSPlatform();
        }

        private static OSPlatform GetOSPlatform() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return OSPlatform.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return OSPlatform.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return OSPlatform.OSX;
            }
            throw new NotSupportedException("Are you running this on toaster?!");
        }
    }
}