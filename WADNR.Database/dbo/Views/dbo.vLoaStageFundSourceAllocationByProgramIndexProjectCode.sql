
create view dbo.vLoaStageFundSourceAllocationByProgramIndexProjectCode
as


select x.LoaStageID, min(x.FundSourceAllocationID) as FundSourceAllocationID, min(x.FundSourceID) as FundSourceID, x.IsNortheast, x.IsSoutheast, x.ProgramIndex, x.ProjectCode from (

select distinct x.LoaStageID, ga.FundSourceAllocationID, ga.FundSourceID , x.IsNortheast, x.IsSoutheast, x.ProgramIndex, x.ProjectCode
from dbo.LoaStage x
join dbo.ProgramIndex pri on pri.ProgramIndexCode = cast(x.ProgramIndex as varchar)
join dbo.ProjectCode pc on pc.ProjectCodeName = x.ProjectCode
join dbo.FundSourceAllocationProgramIndexProjectCode y on y.ProgramIndexID = pri.ProgramIndexID and y.ProjectCodeID = pc.ProjectCodeID
join dbo.FundSourceAllocation ga on y.FundSourceAllocationID = ga.FundSourceAllocationID
) x 
where isnull(ltrim(rtrim(x.ProgramIndex)), '') != '99C'
group by x.LoaStageID, x.IsNortheast, x.IsSoutheast, x.ProgramIndex, x.ProjectCode having count(*) = 1

go

/*
select * from dbo.vLoaStageFundSourceAllocationByProgramIndexProjectCode

*/