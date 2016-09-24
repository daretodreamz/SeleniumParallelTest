using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using OpenQA.Selenium.IE;

namespace SeleniumParallelTest
{
    public enum BrowserType
    {
        Chrome,
        IE
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
                Driver = new ChromeDriver();
            else if (browserType == BrowserType.IE)
            {
                InternetExplorerOptions options = new InternetExplorerOptions();
                //options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                options.RequireWindowFocus = true;
                Driver = new InternetExplorerDriver(options);
            }
        }
    }
}
