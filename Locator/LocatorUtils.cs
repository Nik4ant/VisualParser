using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;


namespace VisualParser.Locator
{
    public static class LocatorUtils {
        // TODO: REMOVE THIS ******* PIECE OF ****
        public static object ExecuteScriptFromFile(RemoteWebDriver driver, string pathToJSFile, 
                params object[] arguments) {
            return driver.ExecuteScript(File.ReadAllText(pathToJSFile), arguments);
        }
        
        /// <summary>
        /// Method waits for url to change and returns new url
        /// (Method waits without timeout)
        /// </summary>
        /// <param name="webDriver">Driver</param>
        /// <returns>New url</returns>
        public static string WaitForUrlChange(RemoteWebDriver webDriver) {
            string oldUrl = webDriver.Url;
            new WebDriverWait(webDriver, TimeSpan.FromDays(365)).Until(driver => driver.Url != oldUrl);
            return webDriver.Url;
        }
        
        // Note(Nik4ant): This is useless for now maybe will help in the future
        public static IWebElement FindElement(IWebDriver webDriver, By by, int timeoutSeconds) {
            if (timeoutSeconds > 0) {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(driver => driver.FindElement(by));
            }
            return webDriver.FindElement(by);
        }
    }
}