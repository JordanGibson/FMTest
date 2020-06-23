# Fluent Migrator

## Set up

For adding Fluent Migrator to a project, 2 nuget packages are required from the Nuget Package Manager:
 - FluentMigrator
 - FluentMigrator.Runner

or execute the following command in the Package Manager Console (Tools > Nuget Packet Manager > Package Manager Console):

	Install-Package FluentMigrator

## Getting started

Standard procedure for Fluent Migrator is to place all migration classes
within a `Migrations` folder. This will be the assembly for all 
migrations used by FM

#### Creating the first migration

Migrations are defined by creating a class which extends from the 
abstract class `Migration` and annotating the class as `Migration`,
 passing the migration version number into the constructor of the annotation, for example:
	
    [Migration(202006231437)]
    public class AddNewSchema : Migration
    {
        public override void Up()
        {
            Create.Schema("new_schema");
        }

        public override void Down()
        {
            Delete.Schema("new_schema");
        }
    }

All migrations must override the `Up()` and `Down()` methods, 
`Up` will be called when the version is migrated up (upgrading 
etc), and by the same token, `Down` will be called when migrated 
down (rolling back etc). Intuitively, `Down` should do the exact 
opposite of `Up`.

#### Running the migration

There are two ways of running the migrations - In-Process, 
and Out-of-process. In-Process is the recommended way of running the migrations,
and Out-of-process should not be used unless absolutely necessary.
For demonstration purposes, only In-Process will be covered.

The dependency injection services must be defined and configured, 
so we'll do that first.

    private static IServiceProvider CreateDIServices()
            {
                return new ServiceCollection()
                    // Add common FluentMigrator services
                    .AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb
                        // Add Postgres support to FluentMigrator
                        .AddPostgres()
                        // Set the connection string
                        .WithGlobalConnectionString(connectionString)
                        // Define the assembly containing the migrations
                        .ScanIn(typeof(AddNewSchema).Assembly).For.Migrations())
                    // Enable logging to console in the FluentMigrator way
                    .AddLogging(lb => lb.AddFluentMigratorConsole())
                    // Build the service provider
                    .BuildServiceProvider(false);
            }

We are defining the assembly containing the migrations,
meaning that any migrations out of this assembly will not be included
 in any transactions.

To actually apply the migrations using the previously defined services:

    static void Main(string[] args)
            {
                var serviceProvider = CreateDIServices();

                // Placed in a using scope to ensure disposal of resources
                using (var scope = serviceProvider.CreateScope())
                {
                    // Instantiate the runner from the services scope
                    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                    // Execute the migrations
                    runner.MigrateUp();

                    // To allow the output to remain visible
                    Console.ReadLine();
                }
            }

This should provide the output of the following:


    202006231437: AddNewSchema migrating
    Beginning Transaction
    CreateSchema new_schema
    Committing Transaction
    202006231437: AddNewSchema migrated

## dot-fm tool

To install the `dot-fm` tool, execute the following command:

    dotnet tool install -g FluentMigrator.DotNet.Cli

The tool can be ran with either dotnet fm or dotnet-fm, however both may need
to be tried due to a .NET Core CLI tool bug.

Here will be a few of the basic necessary commands, other can be found at [Fluent
Migrator Docs](https://fluentmigrator.github.io/articles/runners/dotnet-fm.html). 
For all commands, the connection string, processor and assembly must all be supplied 
in the command using the parameters -c, -p and -a respectively.


<b>Applies all found migrations in the assembly

    migrate

Applies all found migrations in the assembly up to a given version

    migrate [params] up -t 202006231100

Applied all found migrations in the assembly down to a given version

    migrate [params] down -t 202006231100

Rollsback the last applied migration

    rollback

Rollsback to a given version

    rollback [params] to 202006231100

Rollsback by x steps

    rollback [params] by x

Rollsback all migrations

    rollback [params] all

Validates the order of all applied migrations</b>

    validate versions




    