using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231445)]
    public class CreateSecurityTable : Migration
    {
        public override void Up()
        {
            Create.Table("security").InSchema("new_schema")
                .WithColumn("user").AsString().PrimaryKey()
                .WithColumn("role").AsString().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("security").InSchema("new_schema");
        }
    }
}
