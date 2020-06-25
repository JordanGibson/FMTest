using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Runner;
using FMTest.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace FMTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = CreateDIServices();

            // Placed in a using scope to ensure disposal of resources
            using (var scope = serviceProvider.CreateScope())
            {
                // Instantiate the runner from the services scope
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                //runner.ListMigrations();

                // Execute the migrations
                runner.MigrateUp();

                // Delays the completion of execution to observe console output
                Console.ReadLine();

                //runner.MigrateDown(2);
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateDIServices()
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add Postgres support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString("Host=localhost;Database=test;Username=postgres;Password=")
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(AddSecuritySchema).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }
    }
}
