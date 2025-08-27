IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pSetMeToWADNRAdminRole'))
DROP PROCEDURE dbo.pSetMeToWADNRAdminRole
GO

CREATE PROCEDURE dbo.pSetMeToWADNRAdminRole 
(
	@FirstName varchar(100),
	@LastName varchar(100)
)
AS  
begin

	insert into dbo.PersonRole(PersonID, RoleID)
	select PersonID, 8 from dbo.Person as p where p.FirstName = @FirstName and p.LastName = @LastName and p.PersonID not in 
	(select ps.PersonID 
	from 
	dbo.Person as ps
	join dbo.PersonRole as pr on ps.PersonID = pr.PersonID
	where 
	ps.FirstName = @FirstName
	and ps.LastName = @LastName
	and pr.RoleID = 8) 

	--update dbo.Person 
	--set RoleID = 8
	--where FirstName = @FirstName 
	--and LastName = @LastName
end
go
