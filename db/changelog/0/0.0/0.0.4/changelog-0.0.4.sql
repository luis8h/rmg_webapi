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


-- changeset luis8h:0.0.4:3
alter table recipe_tags drop constraint recipe_tags_recipe_fkey;
--rollback changeSetId:0.0.0:10 changeSetAuthor:luis8h changeSetPath:changelog/0/0.0/0.0.0/changelog-0.0.0.sql


-- changeset luis8h:0.0.4:4
alter table recipe_tags
add constraint recipe_tags_recipe_fkey
    foreign key (recipe) references recipes(id)
    on delete cascade;
-- rollback alter table recipe_tags drop constraint recipe_tags_recipe_fkey


-- changeset luis8h:0.0.4:5
alter table ratings drop constraint fk_recipe;
--rollback changeSetId:0.0.0:8 changeSetAuthor:luis8h changeSetPath:changelog/0/0.0/0.0.0/changelog-0.0.0.sql


-- changeset luis8h:0.0.4:6
alter table ratings
add constraint ratings_recipe_fkey
    foreign key (recipe) references recipes(id)
    on delete cascade;
-- rollback alter table recipe_tags drop constraint recipe_tags_recipe_fkey


