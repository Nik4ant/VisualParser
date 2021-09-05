﻿using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace VisualParser.Locator
{
    public static class LocatorStartUp {
        public static void Launch(RemoteWebDriver driver) {
            /*
             * check out: https://github.com/SeleniumHQ/selenium/wiki/DesiredCapabilities
             * check out: https://stackoverflow.com/questions/33225947/can-a-website-detect-when-you-are-using-selenium-with-chromedriver/41220267#41220267
             * Useful list's: https://stackoverflow.com/questions/38335671/where-can-i-find-a-list-of-all-available-chromeoption-arguments
             * 
             * TODO: Trick this websites:
             * https://support.discord.com/hc/en-us/community/posts/360038398572-Hyperlink-Markdown (ESPECIALLY THIS ONE)
             * https://www.411.com/  (I don't know where directly)
             */

            // TODO: for no reason appears extension in browser?!?!?!??!???!?!?!!!?!?!?!?!?!?!?!???!?!?!?!!?!?!!?!??
            // Note(Nik4ant): This is only placeholder for easier testing (later all that stuff will be loaded from profile)
            driver.Navigate().GoToUrl("chrome://settings/");
            driver.ExecuteScript("chrome.settingsPrivate.setDefaultZoom(0.8);");
            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            driver.FindElementByCssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button").Click();
        }
    }
}