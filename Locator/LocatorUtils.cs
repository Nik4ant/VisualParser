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
        /// Method waits for attribute change in element by given selector
        /// and returns new attribute value
        /// (Method waits without timeout)
        /// </summary>
        /// <param name="driver">Driver</param>
        /// <param name="elementSelector">Selector for element with attribute</param>
        /// <param name="attributeName">Attribute's name</param>
        /// <param name="attributeOldValue">Old attribute value</param>
        /// <returns>New attribute value</returns>
        public static string WaitForAttributeChange(RemoteWebDriver driver, By elementSelector, 
            string attributeName, string attributeOldValue) {
            var element = driver.FindElement(elementSelector);
            new WebDriverWait(driver, TimeSpan.FromDays(365)).Until(x => element.GetAttribute(attributeName) != attributeOldValue);
            return element.GetAttribute(attributeName);
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