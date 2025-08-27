if exists (select * from dbo.sysobjects where id = object_id('dbo.vTotalTreatedAcresByProject'))
	drop view dbo.vTotalTreatedAcresByProject
go

create view dbo.vTotalTreatedAcresByProject
as



select 
p.ProjectID
, sum(isnull(t.TreatmentTreatedAcres, 0.00)) as TotalTreatedAcres


 from dbo.Project p
left join dbo.Treatment t on p.ProjectID = t.ProjectID
group by p.ProjectID

go

/*
select * from dbo.vTotalTreatedAcresByProject

*/