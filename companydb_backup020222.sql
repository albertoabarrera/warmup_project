--
-- PostgreSQL database dump
--

-- Dumped from database version 14.1
-- Dumped by pg_dump version 14.1

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

--
-- Name: assign2proj(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.assign2proj() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
  x int;
BEGIN 
   x = (SELECT COUNT(*) FROM ProjectEmp WHERE ssn = OLD.ssn);
   IF (x<=1) THEN 
   	 INSERT INTO ProjectEmp VALUES (OLD.proj_id,OLD.ssn, OLD.begindate);
   END IF;
   RETURN NEW;
END
$$;


ALTER FUNCTION public.assign2proj() OWNER TO postgres;

--
-- Name: definedept(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.definedept() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN 
   INSERT INTO Dept 
   (SELECT NEW.dno as dno,NULL as dname,NULL as mgr
   WHERE NOT EXISTS (SELECT * 
                     FROM Dept
                     WHERE Dept.dno=NEW.dno));
   RETURN NEW;
END
$$;


ALTER FUNCTION public.definedept() OWNER TO postgres;

--
-- Name: insertemp(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.insertemp() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN 
	INSERT INTO Emp_backup VALUES (NEW.ssn, NEW.ename,111);
    RETURN NEW;
END
$$;


ALTER FUNCTION public.insertemp() OWNER TO postgres;

--
-- Name: numemployees(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.numemployees() RETURNS integer
    LANGUAGE plpgsql
    AS $$
declare
   empNum integer;
BEGIN
   SELECT count(*) into empNum FROM Emp;
   RETURN empNum;
END;
$$;


ALTER FUNCTION public.numemployees() OWNER TO postgres;

--
-- Name: updateempcount(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.updateempcount() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN 
   UPDATE  Dept
   SET  numEmp= numEmp+1
   WHERE Dept.dno=NEW.dno;
   RETURN NEW;
END
$$;


ALTER FUNCTION public.updateempcount() OWNER TO postgres;

--
-- Name: updatesalary(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.updatesalary() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN 
   UPDATE  Emp
         SET  salary= OLD.salary
         WHERE  ename = NEW.ename;
   RETURN NEW;
END
$$;


ALTER FUNCTION public.updatesalary() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: dept; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.dept (
    dno integer NOT NULL,
    dname character varying(30),
    mgr character varying(30),
    numemp integer
);


ALTER TABLE public.dept OWNER TO postgres;

--
-- Name: emp; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.emp (
    ssn character(11) NOT NULL,
    ename character varying(50) NOT NULL,
    dno integer NOT NULL,
    sal double precision
);


ALTER TABLE public.emp OWNER TO postgres;

--
-- Name: proj; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.proj (
    proj_id integer NOT NULL,
    ptitle character varying(200) NOT NULL,
    startdate date,
    enddate date,
    numemp integer
);


ALTER TABLE public.proj OWNER TO postgres;

--
-- Name: projectemp; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.projectemp (
    proj_id integer DEFAULT 0 NOT NULL,
    ssn character(11) NOT NULL,
    begindate date
);


ALTER TABLE public.projectemp OWNER TO postgres;

--
-- Data for Name: dept; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.dept (dno, dname, mgr, numemp) FROM stdin;
111	HR	Alice	2
444	Hardware	John	3
888	Purchasing	Robin	5
777	Marketing	Kelly	2
555	Accounting and Finance	Jared	2
222	R&D	Lisa	2
666	Testing	Sam	6
333	Production	Mary	6
\.


--
-- Data for Name: emp; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.emp (ssn, ename, dno, sal) FROM stdin;
112-00-1111	Lisa	222	\N
113-00-1111	Tom	333	\N
114-00-1111	Mary	333	65000
115-00-1111	Pete	444	50000
116-00-1111	Ali	444	55000
117-00-1111	John	444	75000
118-00-1111	Jared	555	65000
119-00-1111	Jim	555	45000
120-00-1111	Sam	666	55000
121-00-1111	Joe	666	47000
122-00-1111	Matthew	666	45000
123-00-1111	Brendon	666	46000
124-00-1111	Jimmy	666	45000
125-00-1111	Casey	666	45000
126-00-1111	Kelly	777	65000
127-00-1111	Rose	777	45000
128-00-1111	Robin	888	60000
129-00-1111	Sophia	888	\N
130-00-1111	Sammy	888	50000
131-00-1111	Amy	888	50000
133-00-1111	Alli	222	52000
134-00-1111	O'Fallon	333	64000
132-00-1111	Jack	888	82000
132-00-1111	Jack	111	82000
111-00-1111	Alice	111	70000
\.


--
-- Data for Name: proj; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.proj (proj_id, ptitle, startdate, enddate, numemp) FROM stdin;
1	Educational Software	2017-01-20	\N	6
2	Tax Software	2016-12-15	2017-07-15	3
3	K-12 Education	2017-04-01	\N	5
\.


--
-- Data for Name: projectemp; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.projectemp (proj_id, ssn, begindate) FROM stdin;
1	111-00-1111	2017-01-20
1	112-00-1111	2017-01-20
1	113-00-1111	2017-01-30
1	114-00-1111	2017-02-01
1	115-00-1111	2017-02-01
1	116-00-1111	2017-02-01
2	111-00-1111	2016-12-30
2	117-00-1111	2016-12-15
2	118-00-1111	2016-12-15
3	114-00-1111	2017-04-20
3	119-00-1111	2017-04-30
3	120-00-1111	2017-04-01
3	121-00-1111	2017-04-01
3	117-00-1111	2017-04-01
1	122-00-1111	2017-01-01
1	123-00-1111	2017-01-01
1	124-00-1111	2017-01-01
1	125-00-1111	2017-01-01
2	126-00-1111	2017-01-01
2	127-00-1111	2017-01-01
2	129-00-1111	2017-01-01
3	130-00-1111	2017-01-01
3	131-00-1111	2017-01-01
3	132-00-1111	2017-01-01
3	133-00-1111	2017-01-01
3	134-00-1111	2017-01-01
2	134-00-1111	2017-01-01
3	128-00-1111	2017-01-01
3	111-00-1111	2019-02-18
\.


--
-- Name: dept dept_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.dept
    ADD CONSTRAINT dept_pkey PRIMARY KEY (dno);


--
-- Name: emp emp_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.emp
    ADD CONSTRAINT emp_pkey PRIMARY KEY (ssn, dno);


--
-- Name: proj proj_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.proj
    ADD CONSTRAINT proj_pkey PRIMARY KEY (proj_id);


--
-- Name: projectemp projectemp_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projectemp
    ADD CONSTRAINT projectemp_pkey PRIMARY KEY (proj_id, ssn);


--
-- Name: emp emp_dno_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.emp
    ADD CONSTRAINT emp_dno_fkey FOREIGN KEY (dno) REFERENCES public.dept(dno);


--
-- Name: projectemp projectemp_proj_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.projectemp
    ADD CONSTRAINT projectemp_proj_id_fkey FOREIGN KEY (proj_id) REFERENCES public.proj(proj_id);


--
-- PostgreSQL database dump complete
--

