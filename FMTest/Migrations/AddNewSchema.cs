using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMTest.Migrations
{
    [Migration(202006231441)]
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
}
