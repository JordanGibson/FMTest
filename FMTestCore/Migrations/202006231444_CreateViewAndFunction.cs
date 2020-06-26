using System;
using FluentMigrator;

namespace FMTest.Migrations
{
    //[Migration(202006231444)]
    public class CreateViewAndFunction : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"CREATE OR REPLACE FUNCTION user_to_json (name varchar, role_name varchar) RETURNS jsonb AS $$
                            BEGIN
                            RETURN json_build_object(
                                    'name', name,
                                    'role', json_build_object(
                                        'name', role_name
                                    )
                                );
                            END;
                        $$ LANGUAGE plpgsql;

                        CREATE OR REPLACE VIEW user_json AS
                        SELECT
                            user_to_json(
                                u.first_name || ' ' || u.last_name,
                                r.name)
                        FROM dev_2.user u
                        JOIN dev_2.role r
                            ON r.id = u.role_id;
                        ");
        }

        public override void Down()
        {
            Execute.Sql("DROP VIEW IF EXISTS user_json;DROP FUNCTION IF EXISTS user_to_json (name varchar, role_name varchar);");
        }
    }
}
