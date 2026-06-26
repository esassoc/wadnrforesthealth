
create view dbo.vServiceForestryStageFundSourceAllocationByProgramIndexProjectCode
as


select x.ServiceForestryStageID, min(x.FundSourceAllocationID) as FundSourceAllocationID, min(x.FundSourceID) as FundSourceID, x.DCProgramIndex, x.DCProjectCode from (

-- Branch 1: PI + PC match
select distinct x.ServiceForestryStageID, ga.FundSourceAllocationID, ga.FundSourceID, x.DCProgramIndex, x.DCProjectCode
from dbo.ServiceForestryStage x
join dbo.ProgramIndex pri on pri.ProgramIndexCode = cast(x.DCProgramIndex as varchar)
join dbo.ProjectCode pc on pc.ProjectCodeName = x.DCProjectCode
join dbo.FundSourceAllocationProgramIndexProjectCode y
    on y.ProgramIndexID = pri.ProgramIndexID
    and y.ProjectCodeID = pc.ProjectCodeID
join dbo.FundSourceAllocation ga on y.FundSourceAllocationID = ga.FundSourceAllocationID

union

-- Branch 2: PI-only match (NULL ProjectCodeID on the allocation; blank/null ProjectCode on the stage row)
select distinct x.ServiceForestryStageID, ga.FundSourceAllocationID, ga.FundSourceID, x.DCProgramIndex, x.DCProjectCode
from dbo.ServiceForestryStage x
join dbo.ProgramIndex pri on pri.ProgramIndexCode = cast(x.DCProgramIndex as varchar)
join dbo.FundSourceAllocationProgramIndexProjectCode y
    on y.ProgramIndexID = pri.ProgramIndexID
    and y.ProjectCodeID is null
join dbo.FundSourceAllocation ga on y.FundSourceAllocationID = ga.FundSourceAllocationID
where x.DCProjectCode is null
   or ltrim(rtrim(x.DCProjectCode)) = ''
   or ltrim(rtrim(x.DCProjectCode)) = '---'
   or ltrim(rtrim(x.DCProjectCode)) = 'N/A'

) x
where isnull(ltrim(rtrim(x.DCProgramIndex)), '') != '99C'
group by x.ServiceForestryStageID, x.DCProgramIndex, x.DCProjectCode having count(*) = 1

go

/*
select * from dbo.vServiceForestryStageFundSourceAllocationByProgramIndexProjectCode

*/
