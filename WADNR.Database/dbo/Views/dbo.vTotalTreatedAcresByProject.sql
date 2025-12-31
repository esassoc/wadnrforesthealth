
create view dbo.vTotalTreatedAcresByProject
as

select 
p.ProjectID
, sum(isnull(t.TreatmentTreatedAcres, 0.00)) as TotalTreatedAcres


 from dbo.Project p
left join dbo.Treatment t on p.ProjectID = t.ProjectID
group by p.ProjectID

go
