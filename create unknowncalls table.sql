CREATE TABLE unknowncalls
 (
 calldate datetime not null,
 calltime datetime not null,
 calltype nvarchar(50) not null,
 placedto nvarchar(50) not null,
 placedfrom nvarchar(50) not null,
 duration float not null,
 rate nvarchar(50) not null,
 charge nvarchar(50) not null,
 acctcode nvarchar(50) not null
 ); 