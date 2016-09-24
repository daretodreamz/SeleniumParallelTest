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
        public void IEGoogleTest()
        {
            Driver.Navigate().GoToUrl("http://www.google.com");
        }
    }
}
