using System;
using System.Reflection;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

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

            //profile.SetPreference() braucht zusätzliche Encodings, sonst gibt es eine Exception
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            //https://stackoverflow.com/questions/46836472/selenium-with-net-core-performance-impact-multiple-threads-in-iwebelement
            var driverPath = $@"{AppDomain.CurrentDomain.BaseDirectory}";
            var service = FirefoxDriverService.CreateDefaultService(driverPath);
            
            var options = new FirefoxOptions();
            options.AddArguments("--headless");

            var profile = new FirefoxProfile();
            profile.SetPreference("intl.accept_languages", "de-de");
            profile.WriteToDisk();
            options.Profile = profile;
            
            _firefoxDriver = new FirefoxDriver(service, options);
            FixDriverCommandExecutionDelay(_firefoxDriver);

            return _firefoxDriver;
        }

        public void Dispose()
        {
            _firefoxDriver?.Dispose();
            _firefoxDriver = null;
        }

        private static void FixDriverCommandExecutionDelay(IWebDriver driver)
        {
            var commandExecutorProperty = typeof(RemoteWebDriver).GetProperty("CommandExecutor",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
            var commandExecutor = (ICommandExecutor) commandExecutorProperty.GetValue(driver);

            var remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);

            if (remoteServerUriField == null)
            {
                var internalExecutorField = commandExecutor.GetType().GetField("internalExecutor",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                commandExecutor = (ICommandExecutor) internalExecutorField.GetValue(commandExecutor);
                remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
            }

            if (remoteServerUriField != null)
            {
                var remoteServerUri = remoteServerUriField.GetValue(commandExecutor).ToString();

                var localhostUriPrefix = "http://localhost";

                if (remoteServerUri.StartsWith(localhostUriPrefix))
                {
                    remoteServerUri = remoteServerUri.Replace(localhostUriPrefix, "http://127.0.0.1");

                    remoteServerUriField.SetValue(commandExecutor, new Uri(remoteServerUri));
                }
            }
        }
    }
}