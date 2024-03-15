-- liquibase formatted sql

-- changeset luis8h:0.0.2:1
alter table users add column password_hashed bytea;

-- rollback alter table users drop column password_hashed;


-- changeset luis8h:0.0.2:2
alter table users add column password_key bytea;

-- rollback alter table users drop column password_key;

