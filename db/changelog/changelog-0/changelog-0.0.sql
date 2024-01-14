-- changeset luis8h:1705222978601-1
CREATE TABLE "ratings" ("id" INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 76) NOT NULL, "recipe" INTEGER NOT NULL, "user_id" INTEGER NOT NULL, "rating" INTEGER, CONSTRAINT "ratings_pkey" PRIMARY KEY ("id"));

-- changeset luis8h:1705222978601-2
CREATE TABLE "recipe_tags" ("id" INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 88) NOT NULL, "tag" INTEGER NOT NULL, "recipe" INTEGER NOT NULL, CONSTRAINT "recipe_tags_pkey" PRIMARY KEY ("id"));

-- changeset luis8h:1705222978601-3
CREATE TABLE "recipes" ("id" INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 69) NOT NULL, "name" VARCHAR(100) NOT NULL, "description" VARCHAR(1000), "preptime" INTEGER, "cooktime" INTEGER, "worktime" INTEGER, "difficulty" INTEGER, "created_at" TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW() NOT NULL, "created_by" INTEGER NOT NULL, "edited_at" TIMESTAMP WITHOUT TIME ZONE, "edited_by" INTEGER, CONSTRAINT "recipes_pkey" PRIMARY KEY ("id"));

-- changeset luis8h:1705222978601-4
CREATE TABLE "tags" ("id" INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 4) NOT NULL, "name" VARCHAR(50), CONSTRAINT "tags_pkey" PRIMARY KEY ("id"));

-- changeset luis8h:1705222978601-5
CREATE TABLE "users" ("id" INTEGER GENERATED BY DEFAULT AS IDENTITY (START WITH 3) NOT NULL, "username" VARCHAR(50) NOT NULL, "firstname" VARCHAR(50), "lastname" VARCHAR(50), "password" VARCHAR(50) NOT NULL, "email" VARCHAR(100) NOT NULL, "signup_date" TIMESTAMP WITHOUT TIME ZONE NOT NULL, CONSTRAINT "users_pkey" PRIMARY KEY ("id"));

-- changeset luis8h:1705222978601-6
ALTER TABLE "recipes" ADD CONSTRAINT "fk_created_by_user" FOREIGN KEY ("created_by") REFERENCES "users" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset luis8h:1705222978601-7
ALTER TABLE "recipes" ADD CONSTRAINT "fk_edited_by_user" FOREIGN KEY ("edited_by") REFERENCES "users" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset luis8h:1705222978601-8
ALTER TABLE "ratings" ADD CONSTRAINT "fk_recipe" FOREIGN KEY ("recipe") REFERENCES "recipes" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset luis8h:1705222978601-9
ALTER TABLE "ratings" ADD CONSTRAINT "fk_users" FOREIGN KEY ("user_id") REFERENCES "users" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset luis8h:1705222978601-10
ALTER TABLE "recipe_tags" ADD CONSTRAINT "recipe_tags_recipe_fkey" FOREIGN KEY ("recipe") REFERENCES "recipes" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset luis8h:1705222978601-11
ALTER TABLE "recipe_tags" ADD CONSTRAINT "recipe_tags_tag_fkey" FOREIGN KEY ("tag") REFERENCES "tags" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;
