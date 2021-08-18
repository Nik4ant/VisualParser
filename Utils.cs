using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VisualParser.Data;

namespace VisualParser
{
    public static class Utils {
        // Dictionary with all available colors for ColoredWriteLine method
        public static readonly Dictionary<char, ConsoleColor> ForegroundColorsData = new Dictionary<char, ConsoleColor>() {
            { 'y', ConsoleColor.Yellow },
            { 'g', ConsoleColor.Green },
            { 'r', ConsoleColor.Red },
        };

        /// <summary>
        /// Method for getting data from terminal/command line
        /// </summary>
        /// <param name="filename">Filename of file to run</param>
        /// <param name="command">Command with all params</param>
        /// <returns>Data from terminal/command line</returns>
        public static string GetTerminalData(string filename, string command) {
            var process = Process.Start(new ProcessStartInfo {
                FileName = filename,
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            // Reading result and closing process
            string result = process.StandardOutput.ReadToEnd();
            process.Close();
            return result;
        }
        
        // TODO: support for '{' and '}' symbols
        // TODO: IT'S SO BAD. Improve performance and try make it easy to use
        /// <summary>
        /// Method for easier use of colored text in the console.
        /// It supports limited amount of colors set in ForegroundColorsData.
        /// Given text should be formatted like this: {*color char*}*colored text*\{*color char*}
        /// For example: {y}This is yellow\{y}. Cool. And {g}this is green\{g}
        /// NOT WORKING PROPERLY WITH '{' or '}' SYMBOLS!
        /// </summary>
        /// <param name="text">Text with color indicators</param>
        public static void ColoredWriteLine(string text) {
            int lastIndex = 0;
            StringBuilder remainingText = new StringBuilder(text.Length);
            
            for (int i = 0; i < text.Length; i++) {
                // Check to match format
                if (text[i] == '{' && text[i + 2] == '}' && 
                    ForegroundColorsData.TryGetValue(text[i + 1], out var currentColor)) {
                    // If it's end of color block we output it to console with color
                    if (i != 0 && text[i - 1] == '\\') {
                        Console.ForegroundColor = currentColor;
                        Console.Write(text[lastIndex..(i - 1)]);
                        remainingText.Clear();
                        Console.ForegroundColor = ConsoleColor.Gray;  // default color
                    }
                    // Else output other text data and set lastIndex
                    else {
                        Console.Write(text[lastIndex..i]);
                        remainingText.Clear();
                        lastIndex = i + 3;
                    }
                }
                // Adding remaining text for output, if it was not printed earlier
                if (text[i] != '{' && text[i] != '}' && i != 0 && text[i - 1] != '{') { remainingText.Append(text[i]); }
                
            }
            Console.Write($"{remainingText}\n");
        }
        
        public static async Task DownloadFileByUrlAsync(string url, string pathToSave) {
            // Downloading file
            var client = new WebClient();
            await Task.Run(() => client.DownloadFile(url, pathToSave));
            client.Dispose();
        }
    }
}