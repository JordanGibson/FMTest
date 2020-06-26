using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FMTest.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace FMTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = CreateDIServices("dev_1");

            // Placed in a using scope to ensure disposal of resources
            using (var scope = serviceProvider.CreateScope())
            {
                // Instantiate the runner from the services scope
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                //runner.ListMigrations();

                // Execute the migrations
                runner.MigrateUp();

                Console.ReadLine();
                
                runner.MigrateDown(2);
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateDIServices(String tenantName)
        {
            return new ServiceCollection()
                .AddSingleton<IConventionSet>(new DefaultConventionSet(tenantName, null))
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString("Host=localhost;Database=test;Username=postgres;Password=")
                    .ScanIn(typeof(AddSecuritySchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
    }
}
