using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace VisualParser
{
    static class Utils {
        /// <summary>
        /// Method for getting data from process
        /// </summary>
        /// <param name="filename">Filename of file to run</param>
        /// <param name="command">Command with all params</param>
        /// <param name="redirectError">If true, returns null on error occurs</param>
        /// <returns>Data from terminal/command line or null</returns>
        public static string? GetProcessData(string filename, string command, bool redirectError=false) {
            var process = Process.Start(new ProcessStartInfo {
                FileName = filename,
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = redirectError
            });
            if (process is null) {
                ColorConsole.Error($"Can't run process {filename} with args: {command}");
                return null;
            }
            // Check for errors
            if (redirectError && process.StandardError.ReadToEnd() != "")
                return null;
            
            string result = process.StandardOutput.ReadToEnd();
            process.Close();
            return result;
        }

        /// <summary>
        /// Method opens select file dialog and returns path to selected file.
        /// </summary>
        /// <param name="selectionTitle">Title for dialog</param>
        /// <param name="filterString">Filter for file ("*.*" by default)</param>
        /// <returns>Path to selected file or null</returns>
        public static string? SelectSingleFileDialog(string selectionTitle="Any file", string filterString = "*.*") {
            StringBuilder commandBuilder = new();
            // Name of future process to execute (Windows - powershell, macOS/Linux - terminal)
            string processName = "powershell";
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                commandBuilder.AppendLine("Add-Type -AssemblyName System.Windows.Forms");
                commandBuilder.AppendLine("$FileDialog = New-Object System.Windows.Forms.OpenFileDialog");
                commandBuilder.AppendLine("$FileDialog.InitialDirectory = [Environment]::GetFolderPath('Desktop')");
                commandBuilder.AppendLine($"$FileDialog.Filter = '{selectionTitle} ({filterString}) | {filterString}'");
                commandBuilder.AppendLine("$FileDialog.ShowDialog()");
                commandBuilder.AppendLine("echo $FileDialog.Filename");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                processName = "/bin/bash";
                commandBuilder.Append($"-c \"echo `zenity --file-selection --title='{selectionTitle}' --file-filter='{filterString}'`\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                processName = "terminal";
                // TODO: test
                commandBuilder.Append($"-c \" echo $(zenity --file-selection --title='{selectionTitle}' --file-filter='{filterString}')\"");
            }
            return GetProcessData(processName, commandBuilder.ToString());
        }
        
        /// <summary>
        /// Method executes process with given args
        /// </summary>
        /// <param name="filename">Process to execute</param>
        /// <param name="arguments">Arguments to pass</param>
        public static void ExecuteProcess(string filename, string arguments) {
            var process = Process.Start(new ProcessStartInfo {
                FileName = filename,
                Arguments = arguments,
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
        
        /// <summary>
        /// Method downloads file by given url
        /// </summary>
        public static void DownloadFileByUrl(string url, string filename) {
            WebClient webClient = new();
            webClient.DownloadFile(url, filename);
            webClient.Dispose();
        }
        
        
        /// <summary>
        /// Method for reading [y/n] questions input
        /// </summary>
        /// <returns>true if 'y', false if 'n'</returns>
        private static bool AskUserInput() {
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
        
        /// <summary>
        /// Method for [y/n] questions for user.
        /// (Automatically add " [y/n] " string to question)
        /// </summary>
        /// <param name="textQuestion">Question</param>
        /// <param name="questionColor">Color for printing question</param>
        /// <returns>true if 'y' false if 'n'</returns>
        public static bool AskUserInput(string textQuestion, ConsoleColor questionColor = ConsoleColor.DarkGray) {
            ColorConsole.Write(textQuestion, questionColor);
            ColorConsole.Write(" [y/n] ", questionColor);
            return AskUserInput();
        }
        
        /// <summary>
        /// Method for [y/n] questions, but with more colors.
        /// (Automatically add " [y/n] " string to question)
        /// </summary>
        /// <param name="textQuestion">Question</param>
        /// <param name="useCustomColorQuestion">Parsing colors from string if true else default color</param>
        /// <returns>true if 'y', false if 'n'</returns>
        public static bool AskUserInput(string textQuestion, bool useCustomColorQuestion) {
            if (useCustomColorQuestion)
                ColorConsole.Write(textQuestion);
            else
                return AskUserInput(textQuestion, ConsoleColor.Gray);
            Console.Write(" [y/n] ");
            return AskUserInput();
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