using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231446)]
    public class AlterUserTable : Migration
    {
        public override void Up()
        {
            Alter.Table("user").InSchema("security").AddColumn("address_id").AsInt32()
                .ForeignKey("fk_security_user_address", "security", "address", "id");
        }

        public override void Down()
        {
            Delete.Column("address_id").FromTable("user").InSchema("security");
        }
    }
}
