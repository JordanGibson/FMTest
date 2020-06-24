CREATE OR REPLACE FUNCTION security.user_to_json (name varchar, role_name varchar) RETURNS jsonb AS $$
    BEGIN
    RETURN json_build_object(
            'name', name,
            'role', json_build_object(
                'name', role_name
            )
        );
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE VIEW security.user_json AS
SELECT
    security.user_to_json(
        u.first_name || ' ' || u.last_name,
        r.name)
FROM security.user u
JOIN security.role r
    ON r.id = u.role_id;
