using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231445)]
    public class CreateSecurityAddress : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("address")
                .WithColumn("id").AsInt32().Identity().PrimaryKey("pk_security_address")
                .WithColumn("street").AsString().NotNullable()
                .WithColumn("town").AsString().NotNullable();
        }
    }
}
