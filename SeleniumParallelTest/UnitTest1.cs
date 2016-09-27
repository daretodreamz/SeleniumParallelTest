using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  NUnit.Framework;

namespace SeleniumParallelTest
{
    [TestFixture]
    [Parallelizable]
    public class IETesting : Hooks
    {
        public IETesting() : base(BrowserType.IE)
        {
            
        }

        [Test]
        public void IESohuTest()
        {
            Driver.Navigate().GoToUrl("http://www.sohu.com");
            IEScreenShot.TakeScreenShot();
        }
    }
}
