use Project0;
/*
select * from dbo.Customer;
select * from dbo.Product;
select * from dbo."Order";
select * from dbo.OrderLine;
select * from dbo.Location;
select * from dbo.LocationLine;
*/


/*Default data*/
insert into dbo.Product values('Wrench', 20, 'Red');
insert into dbo.Product values('Hammer', 15, 'Black');
insert into dbo.Product values('Saw', 25, 'Yellow');
insert into dbo.Location values('Queens');
insert into dbo.Location values('Brooklyn');
insert into dbo.LocationLine values(100, 1, 1);
insert into dbo.LocationLine values(50, 2, 1);
insert into dbo.LocationLine values(70, 1, 2);
insert into dbo.LocationLine values(40, 3, 2);

/* Reset */

/* Clear */
/*
drop table dbo.Customer;
drop table dbo.Location;
drop table dbo.LocationLine;
drop table dbo."Order";
drop table dbo.OrderLine;
drop table dbo.Product;
delete from dbo.Customer where "fName" = '';
*/