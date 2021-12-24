using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using VisualParser.Core;

namespace VisualParser
{
    internal static class Program {
        static void Main(string[] args) {
            // Handles info and driver installation if needed
            InfoManager.HandleInfo();
            
            // This is just code sample for fun (it is using for tests)
            var driver = new ChromeDriver(Globals.Info.PathToDriverFolder);
            driver.Navigate().GoToUrl("https://youtu.be/dQw4w9WgXcQ");
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(
                webDriver => webDriver.FindElement(By.CssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button")));
            driver.FindElement(By.CssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button")).Click();
            Console.Write("Press any key to exit: ");
            Console.ReadKey();
        }
    }
}