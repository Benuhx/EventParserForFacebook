using System;
using System.Diagnostics;
using JuLiMl.Parser;
using JuLiMl.Selenium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace JuLiMl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var container = KonfiguriereDependencyInjectionUndErstelleContainer())
            {
                var mainRunner = container.GetInstance<ISeleniumRunner>();
                mainRunner.Run();

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
                    x.AssemblyContainingType(typeof(Program));
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