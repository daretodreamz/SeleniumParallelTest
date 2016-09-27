using System;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace SeleniumParallelTest
{
    [TestFixture]
    [Parallelizable]
    public class FirefoxTesting : Hooks
    {

        public FirefoxTesting() : base(BrowserType.Firefox)
        {
            
        }
        [Test]
        public void FirefoxTradeMeTest()
        {
            Driver.Navigate().GoToUrl("http://www.trademe.co.nz/");


            IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(3000));

            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));

            ////////IJavaScriptExecutor js = Driver as IJavaScriptExecutor;
            ////////string title = js.ExecuteScript("return document.readyState").ToString();

            ITakesScreenshot screenshotDriver = Driver as ITakesScreenshot;
            Screenshot screenshot = screenshotDriver.GetScreenshot();
            screenshot.SaveAsFile(@"D:\IECapture\TradeMe.jpg", ImageFormat.Jpeg);
        }
    }
}
