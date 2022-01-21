using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using VisualParser.Core;

namespace VisualParser
{
    internal static class Program {
        static void Main(string[] args) {
            // Handles info and driver installation if needed
            InfoManager.HandleAndSetInfo();
            // Note(Nik4ant): For now this looks weird (why just don't use
            // static class with build() method instead). However there are most likely to be
            // features in future that will work perfectly with that
            var builder = new ChromeDriverBuilder(AppInfo.Chrome);
            var driver = builder.Build();
            // This is just code sample for fun (it is using for tests)
            driver.Navigate().GoToUrl("https://youtu.be/dQw4w9WgXcQ");
            new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(
                webDriver => webDriver.FindElement(By.CssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button")));
            driver.FindElement(By.CssSelector("#movie_player > div.ytp-cued-thumbnail-overlay > button")).Click();
            Console.Write("Press any key to exit: ");
            Console.ReadKey();
        }
    }
}