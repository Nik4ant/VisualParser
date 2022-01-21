namespace VisualParser.Core
{
    /// <summary> Small class for loading resources from Static folder</summary>
    public static class StaticManager
    {
        /// <returns>Returns absolute path or uri to page</returns>
        public static string GetPage(string pageName, bool useUriFormat=false) {
            string result = $"{AbsolutePathToStaticFolder}{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}{pageName}";
            if (useUriFormat) {
                return new Uri(result).AbsoluteUri;
            }
            return result;
        }

        /// <returns>Returns absolute path or uri to file in page</returns>
        public static string GetPageFile(string pageName, string filename, bool useUriFormat=false) {
            // First part of path is already absolute
            string result = $"{GetPage(pageName)}{Path.DirectorySeparatorChar}{filename}";
            if (useUriFormat) {
                return new Uri(result).AbsoluteUri;
            }
            return result;
        }
    }
}