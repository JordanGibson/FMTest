using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace FMTest
{
    [Migration(4)]
    public class AddNewTenantSchema : Migration
    {
        public override void Up()
        {
            Create.Schema("dev_1");
            Create.Table("security.user").InSchema("dev_1")
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("Name").AsString();
            Create.Table("security.role").InSchema("dev_1")
                .WithColumn("Name").AsString();

            Alter.Table("security.user").InSchema("dev_1")
                .AddColumn("Password").AsString();

            Insert.IntoTable("security.user").InSchema("dev_1")
                .Row(new { Id = 21, Name = "Test Person", Password = "P@ssword" });
        }

        public override void Down()
        {
            Delete.Table("security.user").InSchema("dev_1");
            Delete.Table("security.role").InSchema("dev_1");
            Delete.Schema("dev_1");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }


        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString("Host=localhost;Database=test;Username=postgres;Password=pass")
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(AddNewTenantSchema).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();

            Console.ReadLine();

            runner.MigrateDown(3);
        }
    }
}
