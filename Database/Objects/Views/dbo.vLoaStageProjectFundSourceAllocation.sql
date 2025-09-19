if exists (select * from dbo.sysobjects where id = object_id('dbo.vLoaStageProjectFundSourceAllocation'))
	drop view dbo.vLoaStageProjectFundSourceAllocation
go

create view dbo.vLoaStageProjectFundSourceAllocation
as

select p.ProjectID
, p.ProjectGisIdentifier
, lan.MatchAmount
, lan.PayAmount 
, lan.ProjectStatus
, x.FundSourceAllocationID
, lan.LetterDate as LetterDate
, lan.ProjectExpirationDate as ProjectExpirationDate
, lan.ApplicationDate as ApplicationDate
, lan.DecisionDate as DecisionDate
, x.LoaStageID
, x.IsNortheast
, x.IsSoutheast
, x.ProgramIndex
, x.ProjectCode
from dbo.vLoaStageFundSourceAllocation x
join dbo.LoaStage lan on x.LoaStageID = lan.LoaStageID
join dbo.Project p on p.ProjectGisIdentifier = lan.ProjectIdentifier
join dbo.ProjectProgram pp on pp.ProjectID = p.ProjectID
where pp.ProgramID = 3



union

select 
p.ProjectID
, p.ProjectGisIdentifier
, lan.MatchAmount
, lan.PayAmount
, lan.ProjectStatus
, null
, lan.LetterDate as LetterDate
, lan.ProjectExpirationDate as ProjectExpirationDate
, lan.ApplicationDate as ApplicationDate
, lan.DecisionDate as DecisionDate
, lan.LoaStageID
, lan.IsNortheast
, lan.IsSoutheast
, lan.ProgramIndex
, lan.ProjectCode
 from dbo.Project p
 join dbo.ProjectProgram pp on pp.ProjectID = p.ProjectID
join dbo.LoaStage lan on lan.ProjectIdentifier = p.ProjectGisIdentifier
left join dbo.vLoaStageFundSourceAllocation x on x.LoaStageID = lan.LoaStageID
where pp.ProgramID = 3 and x.LoaStageID is null

go

/*
select * from dbo.vLoaStageProjectFundSourceAllocation x where x.grantallocationid = 324

*/