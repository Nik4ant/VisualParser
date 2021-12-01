using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace VisualParser
{
    static class Utils {
        /// <summary>
        /// Method for getting data from process
        /// </summary>
        /// <param name="filename">Filename of file to run</param>
        /// <param name="command">Command with all params</param>
        /// <returns>Data from terminal/command line</returns>
        public static string? GetProcessData(string filename, string command) {
            var process = Process.Start(new ProcessStartInfo {
                FileName = filename,
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            // Reading result and closing process
            string? result = process?.StandardOutput.ReadToEnd();
            process?.Close();
            return result;
        }
        
        // TODO: docs
        public static void ExecuteProcess(string filename, string command) {
            var process = Process.Start(new ProcessStartInfo {
                FileName = filename,
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true
            });
            string? error = process?.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error)) {
                ColorConsole.Error($"An error occurred during process execution:\n{error}");
                Environment.Exit(1);
            }
        }
        
        // TODO: docs
        public static void DownloadFileByUrl(string url, string pathToSave) {
            WebClient webClient = new();
            webClient.DownloadFile(url, pathToSave);
            webClient.Dispose();
        }
        
        /// <summary>
        /// Method for easier use of [y/n] questions for user
        /// </summary>
        /// <param name="textQuestion">Question</param>
        /// <param name="questionColor">Color for printing question</param>
        /// <returns>true if 'y' false if 'n'</returns>
        public static bool AskUserInput(string textQuestion, ConsoleColor questionColor = ConsoleColor.DarkGray) {
            // TODO: add [y/n] string here + mention it in docs
            ColorConsole.Write(textQuestion, questionColor);
            bool result;
            while (true) {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y) {
                    result = true;
                    break;
                }
                if (key == ConsoleKey.N) {
                    result = false;
                    break;
                }
            }
            Console.Write('\n');
            return result;
        }
    }

    static class ColorConsole {
        /// <summary>
        /// Method for easier use of colored text in the console.
        /// It supports color defined in ConsoleColor class. Given text should
        /// be formatted like this: [*color string*]*colored text*[/*color string*].
        /// Where *color string* is one of the values from ConsoleColor enum
        /// For example: [Yellow]This is yellow[/Yellow]. Cool. And [Green]this is green[/Green]
        /// </summary>
        /// <param name="text">Text with color indicators</param>
        public static void Write(string text) {
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
        }
        
        /// <summary>
        /// Print text in console using given color
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="color">Color for text</param>
        public static void Write(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        
        /// <summary>
        /// Same as ColoredConsole.Write (without color param) but with '\n' at the end
        /// </summary>
        /// <param name="text">Text</param>
        public static void WriteLine(string text) {
            Write(text);
            Console.Write('\n');
        }
        
        // Same as Write (with color param) but with '\n' at the end
        public static void WriteLine(string text, ConsoleColor color) {
            Write(text, color);
            Console.Write('\n');
        }
        
        public static void Warning(string text) { WriteLine(text, ConsoleColor.DarkYellow); }
        
        public static void Error(string text) { WriteLine(text, ConsoleColor.Red); }
    }
}