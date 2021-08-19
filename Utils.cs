using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VisualParser.Data;

namespace VisualParser
{
    static class Utils {
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

        public static async Task DownloadFileByUrlAsync(string url, string pathToSave) {
            // Downloading file
            var client = new WebClient();
            await Task.Run(() => client.DownloadFile(url, pathToSave));
            client.Dispose();
        }
    }

    static class ColoredConsole {
        /// <summary>
        /// Method for easier use of colored text in the console.
        /// It supports color defined in ConsoleColor class. Given text should
        /// be formatted like this: [*color string*]*colored text*[/*color string*]
        /// For example: [yellow]This is yellow[/yellow]. Cool. And [green]this is green[/green]
        /// </summary>
        /// <param name="text">Text with color indicators</param>
        public static void WriteLine(string text) {
            // For parsing color
            StringBuilder currentColorBuilder = new StringBuilder(12);
            // Last parsed color
            ConsoleColor lastParsedColor = ConsoleColor.DarkGray;
            // Index of last printed char
            int lastOutputStartIndex = 0;
            // Index of first char among last color text
            int lastColoredTextIndex = 0;
            // Length of last color name (needed for output data after for cycle)
            int lastColorNameLenght = 0;
            
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '[') {
                    // If it's end of color bracket print output
                    if (i + 1 != text.Length && text[i + 1] == '/') {
                        // Printing color text
                        Console.ForegroundColor = lastParsedColor;
                        Console.Write(text[lastColoredTextIndex..i]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        // Updating indexes
                        if (i + lastColorNameLenght + 2 < text.Length) { 
                            i += lastColorNameLenght + 2;
                        }
                        lastOutputStartIndex = i + 1;
                    }
                    // Else parsing color and move on
                    else {
                        if (i != 0) { Console.Write(text[lastOutputStartIndex..i]); }
                        i++;  // Skipping current '[' symbol
                        // Adding color symbols until it's end
                        while (text[i] != ']') {
                            currentColorBuilder.Append(text[i]);
                            i++;
                        }
                        // Parsing ConsoleColor enum value
                        if (Enum.TryParse(currentColorBuilder.ToString(),
                            out lastParsedColor)) {
                            lastColoredTextIndex = i + 1;
                            lastColorNameLenght = currentColorBuilder.Length;  // Storing length of parsed color
                            currentColorBuilder.Clear();  // Clearing builder for next color
                        }
                    }
                }
            }
            Console.Write(text[lastOutputStartIndex..]);
            Console.Write('\n');
        }
    
        /// <summary>
        /// Print text in console using given color
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="color">Color for text</param>
        public static void WriteLine(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}