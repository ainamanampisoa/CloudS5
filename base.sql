CREATE DATABASE IF NOT EXISTS clouds5;
\c clouds5;

CREATE TABLE type_user(
   id_type SERIAL,
   nom_type VARCHAR(50)  NOT NULL,
   PRIMARY KEY(id_type)
);

CREATE TABLE utilisateur(
   id_user SERIAL,
   email VARCHAR(60) ,
   username VARCHAR(50)  NOT NULL,
   password VARCHAR(50)  NOT NULL,
   id_type INTEGER NOT NULL,
   PRIMARY KEY(id_user, email),
   FOREIGN KEY(id_type) REFERENCES type_user(id_type)
);

INSERT INTO type_user (nom_type) values 
('admin'),
('membre_simple');