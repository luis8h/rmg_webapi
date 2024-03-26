-- liquibase formatted sql

-- changeset luis8h:0.0.1:1
insert into users (username, firstname, lastname, password, email)
values
    ('luis8h', 'Luis', 'Schmidmeister', 'test1', 'vzbls@t-online.de'),
    ('timo9h', 'Timo', 'Schmidmeister', 'test2', 'vzbts@t-online.de'),
    ('luis8h', 'Noah', 'Schmidmeister', 'test3', 'vzbns@t-online.de'),
    ('Wolfgang', 'Wolfgang', 'Schmidmeister', 'test4', 'vzbws@t-online.de'),
    ('ulli', 'Ulrike', 'Schmidmeister', 'test5', 'vzbus@t-online.de');

-- rollback  truncate users cascade;


-- changeset luis8h:0.0.1:2
insert into tags (name)
values
    ('Vegan'),
    ('Vegetarisch'),
    ('Süß'),
    ('Hauptspeiße'),
    ('Dip'),
    ('Suppe'),
    ('Desert'),
    ('Beilage'),
    ('Grillen'),
    ('Fleisch'),
    ('Salat'),
    ('zum Mitnehmen'),
    ('Weihnachten'),
    ('Plätzchen'),
    ('Kuchen'),
    ('Herd'),
    ('Mikrowelle'),
    ('Ofen'),
    ('Soße'),
    ('Thermomix');

-- rollback truncate tags cascade;


-- changeset luis8h:0.0.1:3
insert into recipes (name, description, preptime, cooktime, worktime, difficulty, created_by)
values
    ('Gewürzkuchen', '', 4, 60, 20, 2, 1),
    ('Tomatensoße', 'In erster Linie für Spaghetti, kann aber mit allen Nudelvarianten genossen werden', 0, 25, 15, 1, 1),
    ('Nudelauflauf (Tomaten/Mozarella)', '', 0, 40, 30, 2, 2),
    ('Schweinebraten', '', null, null, null, 4, 3),
    ('Germknödel', 'Kann gegebenfalls auch ohne Füllung gemacht werden.', null, null, null, 4, 3),
    ('leer', 'Dieses Rezept hat keine Tags, Bewertungen, Zeiten etc.', null, null, null, null, 1);

-- rollback truncate recipes cascade;


-- changeset luis8h:0.0.1:4
insert into recipe_tags (recipe, tag)
values
    (1, 2), (1, 3), (1, 15), (1, 12), (1, 18),
    (2, 19), (2, 1), (2, 2), (2, 4), (2, 16),
    (3, 2), (3, 4), (3, 16), (3, 18),
    (4, 10), (4, 4), (4, 18),
    (5, 2), (5, 7), (5, 3), (5, 20);

-- rollback truncate recipe_tags cascade;


-- changeset luis8h:0.0.1:5
insert into ratings (recipe, user_id, rating)
values
    (1, 1, 5), (1, 2, 3), (1, 3, 4), (1, 4, 3), (1, 5, 5),
    (2, 1, 5), (2, 2, 5), (2, 3, 4), (2, 4, 5), (2, 5, 5),
    (3, 1, 5), (3, 2, 5), (3, 3, 3), (3, 4, 5), (3, 5, 5),
    (4, 1, 3), (4, 2, 5), (4, 3, 5), (4, 4, 5), (4, 5, 4),
    (5, 1, 5), (5, 2, 4), (5, 3, 5), (5, 4, 5), (5, 5, 2);

-- rollback truncate ratings cascade;
