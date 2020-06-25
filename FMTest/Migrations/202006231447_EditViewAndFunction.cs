﻿using System;
using FluentMigrator;

namespace FMTest.Migrations
{
    [Migration(202006231447)]
    public class EditViewAndFunction : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"CREATE OR REPLACE FUNCTION security.user_to_json (name varchar, role_name varchar, street varchar, town varchar) RETURNS jsonb AS $$
	                        BEGIN
 	                        RETURN json_build_object(
 			                        'name', name,
			                        'role', json_build_object(
				                        'name', role_name
			                        ),
			                        'address', json_build_object(
				                        'street', street,
				                        'town', town
			                        )
 		                        );
	                        END;
                        $$ LANGUAGE plpgsql;

                        CREATE OR REPLACE VIEW security.user_json AS
                        SELECT
	                        security.user_to_json(
		                        u.first_name || ' ' || u.last_name,
		                        r.name,
		                        a.street,
		                        a.town)
                        FROM security.user u
                        JOIN security.role r
	                        ON r.id = u.role_id
                        LEFT JOIN security.address a
	                        ON a.id = u.address_id;");
        }

        public override void Down()
        {
            Execute.Sql("DROP VIEW IF EXISTS security.user_json;DROP FUNCTION IF EXISTS security.user_to_json (name varchar, role_name varchar, street varchar, town varchar);");
        }
    }
}
