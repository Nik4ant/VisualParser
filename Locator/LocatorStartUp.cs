using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace VisualParser.Locator
{
    public static class LocatorStartUp {
        public static void Launch(RemoteWebDriver driver) { 
            // TODO: for no reason appears extension in browser?!?!?!??!???!?!?!!!?!?!?!?!?!?!?!???!?!?!?!!?!?!!?!??
            driver.Navigate().GoToUrl("chrome://settings/");
            driver.ExecuteScript("chrome.settingsPrivate.setDefaultZoom(0.8);");
            driver.Navigate().GoToUrl(new Uri(Path.GetFullPath(Globals.PathToHomePage)).AbsoluteUri);
            /*
            will use this rickroll later :D
            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            driver.FindElementByCssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button").Click();
            */
        }
    }
}