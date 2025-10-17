



delete from dbo.Treatment where ProjectLocationID in (select ProjectLocationID from dbo.ProjectLocation where ProjectID = 28727)

delete from dbo.ProjectLocation where ProjectID = 28727





