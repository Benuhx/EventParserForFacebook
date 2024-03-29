using System;
using System.Linq;
using System.Threading;
using FacebookEventParser.DTO;
using OpenQA.Selenium;

namespace FacebookEventParser.Selenium
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
            
            driver.Navigate().GoToUrl(facebookEventUrl);

            while (driver.FindElementByXPath("//*[contains(text(), 'Bevorstehende Veranstaltungen')]") == null)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            //Anmelden- / Cookie-Banner ausblenden
            driver.ExecuteScript("if(document.getElementById('u_0_o')) document.getElementById('u_0_o').style.display = 'none';");

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
}