using System.Drawing;
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
            Driver.Navigate().GoToUrl("http://www.google.com");
            Driver.FindElement(By.Name("q")).SendKeys("Selenium");
        }
    }
}
