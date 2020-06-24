using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231445)]
    public class CreateSecurityAddress : Migration
    {
        public override void Up()
        {
            Create.Table("address").InSchema("security")
                .WithColumn("id").AsInt32().Identity().PrimaryKey("pk_security_address")
                .WithColumn("street").AsString().NotNullable()
                .WithColumn("town").AsString().NotNullable();

            Alter.Table("user").InSchema("security").AddColumn("address_id").AsInt32()
                .ForeignKey("fk_security_user_address", "security", "address", "id");
        }

        public override void Down()
        {
            
        }
    }
}
