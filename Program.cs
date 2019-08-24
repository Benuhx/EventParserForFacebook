﻿using JuLiMl.Selenium;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JuLiMl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var container = InitializeDependencyInjection())
            {
                var mainRunner = container.GetInstance<ISeleniumRunner>();
                mainRunner.Run();
            }
        }

        private static Container InitializeDependencyInjection()
        {
            var services = new ServiceCollection();

            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                });
                config.For<ISeleniumInstanceService>().Singleton().Use<SeleniumInstanceService>();
                config.Populate(services);
            });

            return container;
        }
    }
}