-- liquibase formatted sql


-- changeset luis8h:0.0.4:1
alter table recipe_tags drop constraint recipe_tags_tag_fkey

--rollback luis8h:0.0.0:11 changeSetPath:folder1/db-changelog1.sql



