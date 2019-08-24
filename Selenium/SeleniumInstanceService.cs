using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Firefox;

namespace JuLiMl.Selenium
{
    public interface ISeleniumInstanceService : IDisposable
    {
        FirefoxDriver GetFirefoxDriver();
    }
    
    public class SeleniumInstanceService : ISeleniumInstanceService
    {
        private FirefoxDriver _firefoxDriver;
        public FirefoxDriver GetFirefoxDriver()
        {
            if (_firefoxDriver != null) return _firefoxDriver;
            _firefoxDriver = new FirefoxDriver();
            return _firefoxDriver;
        }

        public void Dispose()
        {
            _firefoxDriver?.Dispose();
            _firefoxDriver = null;
        }
    }
}
