--
-- PostgreSQL database dump
--

-- Dumped from database version 16.0 (Debian 16.0-1.pgdg120+1)
-- Dumped by pg_dump version 16.0 (Debian 16.0-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: ratings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ratings (
    id integer NOT NULL,
    recipe integer NOT NULL,
    user_id integer NOT NULL,
    rating integer
);


ALTER TABLE public.ratings OWNER TO postgres;

--
-- Name: ratings_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ratings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ratings_id_seq OWNER TO postgres;

--
-- Name: ratings_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ratings_id_seq OWNED BY public.ratings.id;


--
-- Name: recipe_tags; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipe_tags (
    id integer NOT NULL,
    tag integer NOT NULL,
    recipe integer NOT NULL
);


ALTER TABLE public.recipe_tags OWNER TO postgres;

--
-- Name: recipe_tags_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.recipe_tags_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.recipe_tags_id_seq OWNER TO postgres;

--
-- Name: recipe_tags_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipe_tags_id_seq OWNED BY public.recipe_tags.id;


--
-- Name: recipes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipes (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(1000),
    preptime integer,
    cooktime integer,
    worktime integer,
    difficulty integer,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    created_by integer NOT NULL,
    edited_at timestamp without time zone,
    edited_by integer
);


ALTER TABLE public.recipes OWNER TO postgres;

--
-- Name: recipes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.recipes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.recipes_id_seq OWNER TO postgres;

--
-- Name: recipes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_id_seq OWNED BY public.recipes.id;


--
-- Name: tags; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tags (
    id integer NOT NULL,
    name character varying(50)
);


ALTER TABLE public.tags OWNER TO postgres;

--
-- Name: tags_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tags_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tags_id_seq OWNER TO postgres;

--
-- Name: tags_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tags_id_seq OWNED BY public.tags.id;


--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(50) NOT NULL,
    firstname character varying(50),
    lastname character varying(50),
    password character varying(50) NOT NULL,
    email character varying(100) NOT NULL,
    signup_date timestamp without time zone NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO postgres;

--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: ratings id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings ALTER COLUMN id SET DEFAULT nextval('public.ratings_id_seq'::regclass);


--
-- Name: recipe_tags id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe_tags ALTER COLUMN id SET DEFAULT nextval('public.recipe_tags_id_seq'::regclass);


--
-- Name: recipes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN id SET DEFAULT nextval('public.recipes_id_seq'::regclass);


--
-- Name: tags id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tags ALTER COLUMN id SET DEFAULT nextval('public.tags_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: ratings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ratings (id, recipe, user_id, rating) FROM stdin;
\.


--
-- Data for Name: recipe_tags; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipe_tags (id, tag, recipe) FROM stdin;
\.


--
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (id, name, description, preptime, cooktime, worktime, difficulty, created_at, created_by, edited_at, edited_by) FROM stdin;
1	Test Rezept	Das ist ein simples Test Rezept	30	12	42	3	2023-12-12 19:17:10.790286	1	2023-12-12 19:17:10.790286	1
2	Test 2	Ein zweites Testrezept	8	40	10	2	2023-12-12 19:20:44.417077	1	\N	\N
3	3. Test	Nochmal ein Test einfach	\N	\N	\N	8	2023-12-12 19:23:48.99739	1	\N	\N
4	Luis	\N	\N	\N	\N	\N	2023-12-16 13:26:18.004172	1	\N	\N
5	string	\N	\N	\N	\N	\N	2023-12-16 13:26:57.474611	1	\N	\N
6	Luis	\N	\N	\N	\N	\N	2023-12-16 13:29:35.936688	1	\N	\N
7	Luis	\N	\N	\N	\N	\N	2023-12-16 13:32:42.185949	1	\N	\N
8	Timo	\N	\N	\N	\N	\N	2023-12-16 13:33:09.971746	1	\N	\N
9	wtf	\N	\N	\N	\N	\N	2023-12-17 09:33:00.598094	1	\N	\N
10	testst v3	\N	\N	\N	\N	\N	2023-12-17 09:39:53.783866	1	\N	\N
11	testv4	\N	\N	\N	\N	\N	2023-12-17 09:43:12.477511	1	\N	\N
12		\N	\N	\N	\N	\N	2023-12-17 10:53:32.146252	1	\N	\N
13	wtfv2	\N	\N	\N	\N	\N	2023-12-17 11:13:29.697942	1	\N	\N
14	wtfv2	\N	\N	\N	\N	\N	2023-12-17 11:14:39.272082	1	\N	\N
15	wtfv3	\N	\N	\N	\N	\N	2023-12-17 11:15:43.414608	1	\N	\N
16	wtfv3	\N	\N	\N	\N	\N	2023-12-17 11:27:54.915459	1	\N	\N
17	wtfv3		0	5	0	0	2023-12-17 11:32:13.974336	1	\N	\N
18	wtfv3		0	\N	0	0	2023-12-17 11:45:21.343741	1	\N	\N
19	wtfv3		0	\N	\N	0	2023-12-17 11:55:55.310872	1	\N	\N
20	wtfv3		0	\N	5	0	2023-12-17 11:56:19.265798	1	\N	\N
21	wtfv3		\N	\N	5	\N	2023-12-17 12:14:06.211849	1	\N	\N
22	wtfv4	\N	\N	\N	\N	\N	2023-12-17 12:15:05.046041	1	\N	\N
23	testv7	\N	\N	\N	\N	\N	2023-12-17 21:36:51.913208	1	\N	\N
24	testnull		3	\N	\N	\N	2023-12-17 22:04:23.161424	1	\N	\N
25	testnull2		\N	\N	\N	\N	2023-12-17 22:04:45.581576	1	\N	\N
26	testv10	12341234dfdfdf	34	1	44	\N	2023-12-17 22:06:28.99798	1	\N	\N
27	gdgssdf		345	\N	\N	\N	2023-12-17 22:07:57.222643	1	\N	\N
28	test		34	\N	\N	\N	2023-12-17 22:10:55.509451	1	\N	\N
\.


--
-- Data for Name: tags; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tags (id, name) FROM stdin;
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, username, firstname, lastname, password, email, signup_date) FROM stdin;
1	luis8h	Luis	Schmidmeister	test	vzbls@t-online.de	2023-12-10 14:56:40.293945
\.


--
-- Name: ratings_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ratings_id_seq', 1, false);


--
-- Name: recipe_tags_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipe_tags_id_seq', 1, false);


--
-- Name: recipes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_id_seq', 28, true);


--
-- Name: tags_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tags_id_seq', 1, false);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_id_seq', 1, true);


--
-- Name: ratings ratings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT ratings_pkey PRIMARY KEY (id);


--
-- Name: recipe_tags recipe_tags_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe_tags
    ADD CONSTRAINT recipe_tags_pkey PRIMARY KEY (id);


--
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (id);


--
-- Name: tags tags_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tags
    ADD CONSTRAINT tags_pkey PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: recipes fk_created_by_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_created_by_user FOREIGN KEY (created_by) REFERENCES public.users(id);


--
-- Name: recipes fk_edited_by_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_edited_by_user FOREIGN KEY (edited_by) REFERENCES public.users(id);


--
-- Name: ratings fk_recipe; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT fk_recipe FOREIGN KEY (recipe) REFERENCES public.recipes(id);


--
-- Name: recipe_tags fk_recipe; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe_tags
    ADD CONSTRAINT fk_recipe FOREIGN KEY (tag) REFERENCES public.recipes(id);


--
-- Name: recipe_tags fk_tag; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe_tags
    ADD CONSTRAINT fk_tag FOREIGN KEY (recipe) REFERENCES public.tags(id);


--
-- Name: ratings fk_users; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ratings
    ADD CONSTRAINT fk_users FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- PostgreSQL database dump complete
--

