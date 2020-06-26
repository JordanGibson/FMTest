using System;
using System.Net;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FMTest.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace FMTestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var ws = new WebServer(SendResponse, "http://localhost:8080/newTenant/");
            ws.Run();
            Console.WriteLine("Web Server running... Press any key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        private static string SendResponse(HttpListenerRequest request)
        {
            string tenantName = request.QueryString.GetValues("name")?[0];
            CreateTenant(tenantName);
            return $"<HTML><BODY>Created the new schema \"{tenantName}\"</BODY></HTML>";
        }

        private static void CreateTenant(String tenantName)
        {
            var serviceProvider = CreateDiServices(tenantName);

            // Placed in a using scope to ensure disposal of resources
            using (var scope = serviceProvider.CreateScope())
            {
                // Instantiate the runner from the services scope
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                //runner.ListMigrations();

                // Execute the migrations
                runner.MigrateUp();
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateDiServices(String tenantName)
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