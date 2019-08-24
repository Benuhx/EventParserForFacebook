using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace JuLiMl.Selenium
{
    public interface ISeleniumService
    {
        ParserResults IdentifiziereEventTabelle(string facebookEventUrl);
    }

    public class SeleniumService : ISeleniumService
    {
        private readonly ISeleniumInstanceService _seleniumInstance;

        public SeleniumService(ISeleniumInstanceService seleniumInstance)
        {
            _seleniumInstance = seleniumInstance;
        }

        public ParserResults IdentifiziereEventTabelle(string facebookEventUrl)
        {
            var driver = _seleniumInstance.GetFirefoxDriver();

            //driver.Manage().Window.Size = new Size(1920, 1080);
            driver.Navigate().GoToUrl(facebookEventUrl);

            while (driver.FindElementByXPath("//*[contains(text(), 'Bevorstehende Veranstaltungen')]") == null)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            driver.ExecuteScript(
                "if(document.getElementById('u_0_o')) document.getElementById('u_0_o').style.display = 'none';");

            var tablesOhneInnereTabellen = driver
                .FindElementsByTagName("table")
                .Where(x => x.Displayed
                            && !x.FindElements(By.TagName("table")).Any())
                .OrderBy(x => x.Location.Y)
                .ToList();

            var table = tablesOhneInnereTabellen.Last();
            var eventTexte = table.Text;
            var linkTexte = table
                .FindElements(By.TagName("a"))
                .Select(x => x.GetAttribute("aria-label"))
                .ToList();

            var resultConatiner = new ParserResults() {EventText = eventTexte, LinkTexte = linkTexte};
            return resultConatiner;
        }
    }

    public class ParserResults
    {
        public string EventText { get; set; }
        public List<string> LinkTexte { get; set; }
    }
}