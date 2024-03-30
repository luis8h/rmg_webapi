-- liquibase formatted sql


-- changeset luis8h:0.0.4:1
alter table recipe_tags drop constraint recipe_tags_tag_fkey;

--rollback changeSetId:0.0.0:11 changeSetAuthor:luis8h changeSetPath:changelog/0/0.0/0.0.0/changelog-0.0.0.sql


-- changeset luis8h:0.0.4:2
alter table recipe_tags
add constraint recipe_tags_tag_fkey
    foreign key (tag) references tags(id)
    on delete cascade;

-- rollback alter table recipe_tags drop constraint recipe_tags_tag_fkey



