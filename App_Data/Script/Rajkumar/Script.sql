Drop table tblUserType

ALTER TABLE tblUser DROP column UserType;


--Date for table modfication 03/08/2018

Alter table tblDepartment ADD CreatedUserId int,LastUpdateUserId int,CreatedTS datetime,UpdatedTS datetime,IsDeleted bit
--set IsDeleted defalut value =0

Alter table tblRole ADD CreatedUserId int,CreatedTS datetime,LastUpdateUserId int,UpdatedTS datetime,IsDeleted bit
--set IsDeleted defalut value =0

Alter table tblUser ADD CreatedUserId int,LastUpdateUserId int 