using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231443)]
    public class AddSecuritySchema : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("role")
                .WithColumn("id").AsInt32().NotNullable().Identity().PrimaryKey("pk_security_role")
                .WithColumn("name").AsString().NotNullable();

            Create.Table("user")
                .WithColumn("id").AsInt32().NotNullable().Identity().PrimaryKey("pk_security_user")
                .WithColumn("first_name").AsString().NotNullable()
                .WithColumn("last_name").AsString().NotNullable()
                .WithColumn("role_id").AsInt32().ForeignKey("fk_security_user_role", "role", "id");
        }
    }
}
