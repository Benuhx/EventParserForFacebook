﻿using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using FacebookEventParser.OutputServices;
using FacebookEventParser.Parser;
using FacebookEventParser.Selenium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using TelegramApi;
using WordpressPublisher;

namespace FacebookEventParser
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var container = KonfiguriereDependencyInjectionUndErstelleContainer())
            {
                var mainRunner = container.GetInstance<IMainRunner>();
                await mainRunner.Run();

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
            }
        }

        private static Container KonfiguriereDependencyInjectionUndErstelleContainer()
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure
                    .AddConsole())
                    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(x =>
                {
                    x.AssemblyContainingType<Program>();
                    x.AssemblyContainingType<WordPressApi>();
                    x.AssemblyContainingType<ITelegramApi>();
                    x.WithDefaultConventions();
                });
                config.For<ISeleniumInstanceService>().Singleton().Use<SeleniumInstanceService>();
                config.For<IRegExContainer>().Singleton().Use<RegExContainer>();
                config.For<ITelegramApi>().Singleton().Use<TelegramApi.TelegramApi>();
                
                config.For<IHtmlService>().Use<HtmlService>();

                config.Populate(services);
            });

            return container;
        }
    }
}