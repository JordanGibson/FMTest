# Fluent Migrator

## About

Fluent Migrator is a migration framework for .NET much like Ruby 
on Rails Migrations. Migrations are a structured way to alter 
database schema and are an alternative to creating lots of sql 
scripts that have to be run manually. Database schema 
changes are described in classes written in C#.

## Support

There are a substantial about of questions regarding the package on [StackOverflow](https://stackoverflow.com/questions/tagged/fluent-migrator)

The [docs](https://fluentmigrator.github.io/articles/intro.html) contain good information for getting started, and some more advanced topics such as migration version enforcing etc

If you notice a bug, you can [open an issue here](https://github.com/fluentmigrator/fluentmigrator/issues)

## Set up

For adding Fluent Migrator to a project, 2 nuget packages are required from the Nuget Package Manager:

Package      | Version   | Downloads |
------------------|----------|------------|
FluentMigrator    | [![FluentMigrator](https://img.shields.io/nuget/v/FluentMigrator.svg)](https://www.nuget.org/packages/FluentMigrator/) | [![FluentMigrator](https://img.shields.io/nuget/dt/FluentMigrator.svg)](https://www.nuget.org/packages/FluentMigrator/) |
FluentMigrator.Runner    | [![FluentMigratorRunner](https://img.shields.io/nuget/v/FluentMigrator.Runner.svg)](https://www.nuget.org/packages/FluentMigrator.Runner/) | [![FluentMigratorRunner](https://img.shields.io/nuget/dt/FluentMigrator.Runner.svg)](https://www.nuget.org/packages/FluentMigrator.Runner/) |

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

Set the `dbConnectionString` in the format of:

`Host=<HOSTNAME>;Database=<DB_NAME>;Username=<USERNAME>;Password=<PASSWORD>`

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

The process of adding another migration is adding it to the same directory, (ensuring it is within the same
assembly), just by changing the version within the migration constructor in the annotation. There is an
example of a second migration with in the Migrations folder in this project.

## WebServer implementation

This example has been adapted to instead run on localhost, and create a server to listen for requests
to create a new tenant. A request can be made by going to the web browser and navigating to

    http://localhost:8080/newTenant?name=dev_1
    
while the server is running. This will create a new schema in the database named `test` with the name `dev_1`.
The argument in the URL can be changed to create a schema with a different name.

## dot-fm tool

To install the `dot-fm` tool, execute the following command:

    dotnet tool install -g FluentMigrator.DotNet.Cli

The tool can be ran with either dotnet fm or dotnet-fm, however both may need
to be tried due to a .NET Core CLI tool bug.

Here will be a few of the basic necessary commands, other can be found at [Fluent
Migrator Docs](https://fluentmigrator.github.io/articles/runners/dotnet-fm.html). 
For all commands, the connection string, processor and assembly must all be supplied 
in the command using the parameters -c, -p and -a respectively.


<b>Applies all found migrations in the assembly</b>

    migrate

<b>Applies all found migrations in the assembly up to a given version</b>

    migrate [params] up -t 202006231100

<b>Applied all found migrations in the assembly down to a given version</b>

    migrate [params] down -t 202006231100

<b>Rollsback the last applied migration</b>

    rollback

<b>Rollsback to a given version</b>

    rollback [params] to 202006231100

<b>Rollsback by x steps</b>

    rollback [params] by x

<b>Rollsback all migrations</b>

    rollback [params] all

<b>Validates the order of all applied migrations</b>

    validate versions

    
