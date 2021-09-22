using System;
using System.IO;

namespace VisualParser.Core
{
    // TODO: adjust methods
    public class StaticManager {
        /// <summary>
        /// Method returns valid uri for given path
        /// (used to open local pages in WebDriver)
        /// </summary>
        /// <param name="pageFilenamePath">Absolute path to filename of page</param>
        /// <returns>Valid uri for path</returns>
        public static string GetPageUri(string pageFilenamePath) {
            return new Uri(pageFilenamePath).AbsoluteUri;
        }
        
        /// <summary>
        /// Method returns valid uri for file in page
        /// (used to open local pages in WebDriver)
        /// </summary>
        /// <param name="pageName">Name of page</param>
        /// <param name="filename">Filename inside of page</param>
        /// <returns>Valid uri for filename in page</returns>
        public static string GetPageUri(string pageName, string filename) {
            return GetPageUri(GetItemFullPath(pageName, filename));
        }
        
        /// <summary>
        /// Method return relative path to filename somewhere in Static folder
        /// </summary>
        /// <param name="folderPath">Path to folder with file</param>
        /// <param name="filename">Filename of file</param>
        /// <returns>Relative path to file</returns>
        public static string GetItemPath(string folderPath, string filename) {
            return $"{Globals.AppInfo.PathToStaticFolder}{Path.DirectorySeparatorChar}{folderPath}{Path.DirectorySeparatorChar}{filename}";
        }
        
        /// <summary>
        /// Method return absolute path to filename somewhere in Static folder
        /// </summary>
        /// <param name="folderPath">Path to folder with file</param>
        /// <param name="filename">Filename of file</param>
        /// <returns>Absolute path to file</returns>
        public static string GetItemFullPath(string folderPath, string filename) {
            return Path.GetFullPath(GetItemPath(folderPath, filename));
        }
    }
}