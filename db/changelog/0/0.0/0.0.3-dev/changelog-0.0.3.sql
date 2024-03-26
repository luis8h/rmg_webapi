-- liquibase formatted sql

-- changeset luis8h:0.0.3:1
update users set password_key = '\xfa7a82beffa02ae7e3bf515df30eb245c159676bc608d6bebe662ce0cc206a019d2f027c01e5ed040934b950b66098c6af4c6c6bf2c75beb451d2779041cd423' where id in (1, 2, 3, 4 ,5);

-- rollback  update users set password_key = null where id in (1, 2, 3, 4, 5);


-- changeset luis8h:0.0.3:2
update users set password_hashed = '\x8d40c68f71f2f05d352cd30eee84bd7bc8b51bd245e5534e36a23a5291968945' where id in (1, 2, 3, 4 ,5);

-- rollback  update users set password_hashed = null where id in (1, 2, 3, 4, 5);
