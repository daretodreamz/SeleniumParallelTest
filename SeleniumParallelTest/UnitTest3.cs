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

            wait.Until<bool>(
                delegate(IWebDriver drv)
                {
                    IJavaScriptExecutor js = drv as IJavaScriptExecutor;
                    return js.ExecuteScript("return document.readyState").Equals("complete");
                }
            );
            
            ////////wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));

            ////////wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("An Object ID")));
            
            //////////wait.Until(ExpectedConditionsExtension.ProcessBarDisappears());

            ////////////////////IJavaScriptExecutor js = Driver as IJavaScriptExecutor;
            ////////////////////string title = js.ExecuteScript("return document.readyState").ToString();

            ITakesScreenshot screenshotDriver = Driver as ITakesScreenshot;
            Screenshot screenshot = screenshotDriver.GetScreenshot();
            screenshot.SaveAsFile(@"D:\IECapture\TradeMe.jpg", ImageFormat.Jpeg);
        }
    }


    /// <summary>
    /// 自定义的扩展条件
    /// </summary>
    public class ExpectedConditionsExtension
    {
        /// <summary>
        /// 等待进度条消失
        /// </summary>
        /// <param name="driver">WebDriver对象</param>
        /// <returns>操作Func对象</returns>
        public static Func<IWebDriver, bool> ProcessBarDisappears()
        {
            return delegate (IWebDriver driver)
            {
                IWebElement element = null;
                try
                {
                    element = driver.FindElement(By.XPath(".//div[@id='divProcessBar']"));
                }
                catch (NoSuchElementException) { return true; }
                return !element.Displayed;
            };
        }
    }


}
