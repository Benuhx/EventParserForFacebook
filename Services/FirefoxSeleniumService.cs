using System;
using System.Drawing;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace JuLiMl.Services
{
    public interface ISeleniumService
    {
        Image<Rgba32> ErstelleScreenshotVonWebsite(string facebookEventUrl);
    }

    public class SeleniumService : ISeleniumService
    {
        public Image<Rgba32> ErstelleScreenshotVonWebsite(string facebookEventUrl)
        {
            using (var driver = new FirefoxDriver())
            {
                driver.Manage().Window.Size = new Size(1000, 2000);
                driver.Navigate().GoToUrl(facebookEventUrl);
                driver.ExecuteScript("document.body.style.webkitTransform = 'scale(2.0)'");
                
                while (driver.FindElementByXPath("//*[contains(text(), 'Bevorstehende Veranstaltungen')]") == null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }

                if (driver.FindElementById("u_0_o") != null)
                {
                    driver.ExecuteScript("document.getElementById('u_0_o').style.display = 'none';");
                }

                
                var screenshot = driver.GetScreenshot();
                return Image.Load(screenshot.AsByteArray);
            }
        }
    }
}