using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SeleniumParallelTest
{
    [TestFixture]
    [Parallelizable]
    public class ChromeTesting : Hooks
    {

        public ChromeTesting() : base(BrowserType.Chrome)
        {

        }
        [Test]
        public void ChromeGoogleTest()
        {
            Driver.Navigate().GoToUrl("http://www.baidu.com");
            //Driver.FindElement(By.Name("q")).SendKeys("Selenium");
            //Driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);

            Image img = ChromeScreenShot.GetEntireScreenshot(Driver);
            img.Save(@"D:\\IECapture\Chrome_Test.jpg", ImageFormat.Jpeg);
        }
    }
}
