namespace VisualParser.Data
{
    public class ChromeBrowserInfo {
        public string? PathToBinary { get; private set; }
        public string? Version { get; private set; }
        public string? UserAgent { get; private set; }
        public string? ZoomLevel { get; private set; }
        public string? ScreenResolution { get; private set; }
        
        /// <param name="pathToBinary">Path to chrome binary</param>
        /// <param name="version">Chrome version</param>
        /// <param name="userAgent">User agent used in chrome</param>
        /// <param name="zoomLevel">Zoom level used in browser</param>
        /// <param name="screenResolution">Format: "{width}, {height}"</param>
        public ChromeBrowserInfo(string? pathToBinary=null, string? version=null, string? userAgent=null,
            string? zoomLevel=null, string? screenResolution=null) 
        {
            PathToBinary = pathToBinary;
            Version = version;
            UserAgent = userAgent;
            ZoomLevel = zoomLevel;
            ScreenResolution = screenResolution;
        }
        
        #region Property settors
        public void SetBinaryLocation(string? path) { PathToBinary = path; }
        public void SetChromeVersion(string? version) { Version = version; }
        public void SetUserAgent(string? userAgentString) { UserAgent = userAgentString; }
        public void SetZoomLevel(string? zoomLevel) { ZoomLevel = zoomLevel; }
        /// <param name="resolutionString">Format: "{width}, {height}"</param>
        public void SetScreenResolution(string? resolutionString) { ScreenResolution = resolutionString; }
        #endregion
    }
}