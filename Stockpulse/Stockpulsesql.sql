CREATE schema Stockpulse;
USE Stockpulse;
create table inventory(
Id int  auto_increment primary key,
ItemName varchar(50) not null,
Quantity int not null,
Price decimal(10,2) not null
);
select * from inventory;
insert into inventory(Id,ItemName,Quantity,Price) values(7,'Laptop-Lenovo',6,55000);
update  inventory set Price=50000 where Id=2;
select * from inventory where Quantity <5;
select * from inventory order by Quantity DESC; 
select * from inventory order by Quantity ASC; 


