create database source;
go

use source;
go

create table users
(
	id int,
	name nvarchar(20),
	email nvarchar(50)
)
go

insert into users (id, name, email) values (1, 'Jim', 'jim@jim.com');
go
insert into users (id, name, email) values (2, 'Bob', 'bob@bob.com');
go
insert into users (id, name, email) values (3, 'Tom', 'tom@tom.com');
go

create table products
(
	id int,
	name nvarchar(20),
	cost money
)
go

insert into products (id, name, cost) values (1, 'hat', 1.25);
go
insert into products (id, name, cost) values (2, 'coat', 15.87);
go
insert into products (id, name, cost) values (3, 'pant', 1200.99);
go

create database empty_dest;
go

create database single_exist_table_dest;
go
use single_exist_table_dest;
go
create table users
(
	id int,
	name nvarchar(20),
	email nvarchar(50)
)
go

create database conflict_dest;
go
use conflict_dest;
go
create table users
(
	id int,
	name nvarchar(40),
	email nvarchar(50)
)
go


create database tables_exist;
go

use tables_exist;
go

create table users
(
	id int,
	name nvarchar(20),
	email nvarchar(50)
)
go

create table products
(
	id int,
	name nvarchar(20),
	cost money
)
go
