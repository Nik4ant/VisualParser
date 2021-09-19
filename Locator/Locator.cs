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
            // Waiting for parser to start. It starts if JS on page redirects user
            string urlToParse = LocatorUtils.WaitForUrlChange(driver);
            ColoredConsole.Debug($"Your url to parse is: {urlToParse}");
            ColoredConsole.Debug("But that's doesn't matter, because we will rickroll you anyway");
            // Rickroll :D
            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            driver.FindElementByCssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button").Click();
        }
    }
}