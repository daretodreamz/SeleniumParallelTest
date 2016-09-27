using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SeleniumParallelTest
{
    public enum BrowserType
    {
        Chrome,
        IE,
        Firefox
    }


    [TestFixture]
    public class Hooks : Base
    {
        private BrowserType _browserType;

        public Hooks(BrowserType browser)
        {
            _browserType = browser;
        }

        [SetUp]
        public void InitializeTest()
        {
            ChooseDriverInstance(_browserType);
        }


        private void ChooseDriverInstance(BrowserType browserType)
        {
            if(browserType == BrowserType.Chrome)
            {
                Driver = new ChromeDriver();
            }                
            else if (browserType == BrowserType.IE)
            {
                InternetExplorerOptions options = new InternetExplorerOptions();
                //options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                options.RequireWindowFocus = true;
                Driver = new InternetExplorerDriver(options);
            }
            else if (browserType == BrowserType.Firefox)
            {
                FirefoxBinary ffbinary = new FirefoxBinary(@"C:\Program Files\Mozilla Firefox\firefox.exe");
                FirefoxProfile ffprofile = new FirefoxProfile();
                Driver = new FirefoxDriver(ffbinary, ffprofile);
            }
        }
    }
}
