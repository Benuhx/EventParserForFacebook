using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FacebookEventParser.Parser;
using FacebookEventParser.Selenium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using WordpressPublisher;

namespace FacebookEventParser
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var container = KonfiguriereDependencyInjectionUndErstelleContainer())
            {
                var mainRunner = container.GetInstance<ISeleniumRunner>();
                await mainRunner.Run();

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Ende, Taste zum beenden drücken");
                    Console.ReadKey();
                }
            }
        }

        private static Container KonfiguriereDependencyInjectionUndErstelleContainer()
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure
                    .AddConsole())
                    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Warning);

            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(x =>
                {
                    x.AssemblyContainingType<Program>();
                    x.AssemblyContainingType<WordPressApi>();
                    x.WithDefaultConventions();
                });
                config.For<ISeleniumInstanceService>().Singleton().Use<SeleniumInstanceService>();
                config.For<IRegExContainer>().Singleton().Use<RegExContainer>();
                config.Populate(services);
            });

            return container;
        }
    }
}