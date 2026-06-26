
create view dbo.vServiceForestryStageProjectFundSourceAllocation
as

-- Matched: staged rows that resolved to a FundSourceAllocation, joined back to the
-- Landowner Assistance (Service Forestry) project by ProjectGisIdentifier.
select p.ProjectID
, p.ProjectGisIdentifier
, sf.DCMatchAmount
, sf.DCPayAmount
, sf.DCStatus
, x.FundSourceAllocationID
, sf.DCLetterDate
, sf.DCExpirationDate
, sf.ApprovalDate
, sf.PercentMatch
, x.ServiceForestryStageID
, x.DCProgramIndex
, x.DCProjectCode
from dbo.vServiceForestryStageFundSourceAllocation x
join dbo.ServiceForestryStage sf on x.ServiceForestryStageID = sf.ServiceForestryStageID
join dbo.Project p on p.ProjectGisIdentifier = sf.ProjectIdentifier
join dbo.ProjectProgram pp on pp.ProjectID = p.ProjectID
where pp.ProgramID = 3


union

-- Unmatched: staged rows for a known project that did not resolve to any allocation.
-- Carried through (FundSourceAllocationID = null) so project date/cost fields still update.
select
p.ProjectID
, p.ProjectGisIdentifier
, sf.DCMatchAmount
, sf.DCPayAmount
, sf.DCStatus
, null
, sf.DCLetterDate
, sf.DCExpirationDate
, sf.ApprovalDate
, sf.PercentMatch
, sf.ServiceForestryStageID
, sf.DCProgramIndex
, sf.DCProjectCode
from dbo.Project p
join dbo.ProjectProgram pp on pp.ProjectID = p.ProjectID
join dbo.ServiceForestryStage sf on sf.ProjectIdentifier = p.ProjectGisIdentifier
left join dbo.vServiceForestryStageFundSourceAllocation x on x.ServiceForestryStageID = sf.ServiceForestryStageID
where pp.ProgramID = 3 and x.ServiceForestryStageID is null

go

/*
select * from dbo.vServiceForestryStageProjectFundSourceAllocation

*/
