using System.Linq;
using OpenQA.Selenium.Chrome;

namespace VisualParser.Core
{
    public static class ChromeOptionsExtension {
        /// <summary> Adds arguments to options only if all params aren't null </summary>
        public static void TryAddArgument(this ChromeOptions options, 
            string argumentFormatString, object?[] argumentParams) {
            if (argumentParams.Any(item => item is null)) {
                ColorConsole.Error($"Couldn't set \"{argumentFormatString}\" argument.\nParams contained null");
                return;
            }
            options.AddArgument(string.Format(argumentFormatString, argumentParams));
        }
        
        /// <summary> Adds argument to options only if param isn't null </summary>
        public static void TryAddArgument(this ChromeOptions options, 
            string argumentFormatString, object? argumentParam) {
            if (argumentParam is null) {
                ColorConsole.Error($"Couldn't set \"{argumentFormatString}\" argument.\nParam is null");
                return;
            }
            options.AddArgument(string.Format(argumentFormatString, argumentParam));
        }
        
        /// <summary> Adds option only if value isn't null </summary>
        public static void TryAddAdditionalOption(this ChromeOptions options, 
            string optionName, object? optionValue) {
            if (optionValue is null) {
                ColorConsole.Error($"Couldn't set \"{optionValue}\" to option \"{optionName}\".\nValue is null");
                return;
            }
            options.AddAdditionalOption(optionName, optionValue);
        }
    }
    
    public class ChromeDriverBuilder {
        private ChromeOptions DriverOptions { get; } = new();
        private ChromeBrowserInfo InfoForChromeBrowser { get; }

        public ChromeDriverBuilder(ChromeBrowserInfo infoForChromeBrowser) {
            InfoForChromeBrowser = infoForChromeBrowser;

            // Options by default
            DriverOptions.AddArgument("start-maximized");
            DriverOptions.AddExcludedArgument("enable-automation");
            DriverOptions.AddAdditionalOption("useAutomationExtension", false);

            SetBinaryLocation(InfoForChromeBrowser.PathToBinary);
            SetWindowSize(InfoForChromeBrowser.ScreenResolution);
            SetUserAgent(InfoForChromeBrowser.UserAgent);
        }

        public void SetUserAgent(string? userAgentString) {
            DriverOptions.TryAddArgument("user-agent={0}", userAgentString);
        }

        /// <param name="resolution">Screen size in format: "{width}, {height}</param>
        public void SetWindowSize(string? resolution) {
            DriverOptions.TryAddArgument("window-size={0}", resolution);
        }
        
        public void SetBinaryLocation(string? path) {
            DriverOptions.TryAddAdditionalOption("BinaryLocation", path);
        }

        private void PostBuildConfigurations(ChromeDriver driver) {
            // Default zoom level
            // Note(Nik4ant): Maybe it will be set by default in profile, but idk for now
            driver.Navigate().GoToUrl("chrome://settings/");
            driver.ExecuteScript($"chrome.settingsPrivate.setDefaultZoom({InfoForChromeBrowser.ZoomLevel});");
            driver.Navigate().Back();
        }
        
        public ChromeDriver Build() {
            var driver = new ChromeDriver(AbsolutePathToDriverFolder, DriverOptions);
            PostBuildConfigurations(driver);
            return driver;
        }
    }
}