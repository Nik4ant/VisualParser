using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace VisualParser.Locator
{
    public static class Locator
    {
        private static RemoteWebDriver Driver { get; set; }
        
        public static void Launch(RemoteWebDriver driver) {
            Driver = driver;
            // TODO: for no reason appears extension in browser?!?!?!??!???!?!?!!!?!?!?!?!?!?!?!???!?!?!?!!?!?!!?!??
            // Open home page
            Driver.Navigate().GoToUrl(Globals.GetPageUri($"Pages{Path.DirectorySeparatorChar}HomePage", "index.html"));
            // Waiting for parser to start. It starts if button on page was clicked and if
            // correct url was given. Then it is JS code will change attribute value to url
            string urlToParse = LocatorUtils.WaitForAttributeChange(Driver, By.Id("parser-start"),
                "url_to_parse", "");
            LocateToUrl(urlToParse);
            /*
            will use this rickroll later :D
            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            driver.FindElementByCssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button").Click();
            */
        }
        
        private static void LocateToUrl(string url) {
            Driver.Navigate().GoToUrl(url);
        }
    }
}