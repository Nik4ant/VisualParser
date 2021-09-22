using System;
using System.IO;
using System.Resources;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using VisualParser.Core;

namespace VisualParser.Locator
{
    public static class Locator
    {
        private static RemoteWebDriver Driver { get; set; }
        
        public static void Launch(RemoteWebDriver driver) {
            Driver = driver;
            // TODO: for no reason appears extension in browser?!?!?!??!???!?!?!!!?!?!?!?!?!?!?!???!?!?!?!!?!?!!?!??
            // Open home page
            Driver.Navigate().GoToUrl(StaticManager.GetPageUri($"Pages{Path.DirectorySeparatorChar}HomePage", "index.html"));
            // Waiting for parser to start. It starts if JS on page redirects user
            string urlToParse = LocatorUtils.WaitForUrlChange(driver);
            ColoredConsole.WriteLine($"Your url to parse is: {urlToParse}", ConsoleColor.Green);
            // Inserting some js code for demonstration
            LocatorUtils.ExecuteScriptFromFile(Driver, StaticManager.GetItemPath("Scripts", "selector.js"));
            /*
            Don't think that's good idea to have this on demonstration...
            I'll use this rickroll later :D

            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            driver.FindElementByCssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button").Click();
            */
        }
    }
}