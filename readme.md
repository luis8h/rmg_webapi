# Recipe Manager web API

### db schema

Tables:

-   users
    -   id
    -   username
    -   password
    -   email
    -   firstname
    -   lastname
    -   signup_date
    -   profile_img

-   recipes
    -   id
    -   name
    -   description
    -   main_img
    -   img_gallery (maybe with mapping table)
    -   preptime
    -   cooktime
    -   worktime
    -   difficulty
    -   created_at
    -   created_by
    -   edited_at
    -   edited_by

-   recipe_tags
    -   id
    -   tag
    -   recipe

-   ratings
    -   id
    -   recipe
    -   userd
    -   rating

-   tags
    -   id
    -   name

-   auditlog
