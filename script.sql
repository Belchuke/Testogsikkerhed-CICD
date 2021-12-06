create database ProjectDB
Go
Use ProjectDB

create table users
(
    userid int IDENTITY(1,1) PRIMARY KEY,
    Firstname varchar(255),
    LastName varchar(255),
)
