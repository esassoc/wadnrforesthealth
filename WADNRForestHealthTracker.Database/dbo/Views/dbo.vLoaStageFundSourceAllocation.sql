if exists (select * from dbo.sysobjects where id = object_id('dbo.vLoaStageFundSourceAllocation'))
	drop view dbo.vLoaStageFundSourceAllocation
go

create view dbo.vLoaStageFundSourceAllocation
as



select y.LoaStageID, g.FundSourceID, x.FundSourceAllocationID, y.IsNortheast, y.IsSoutheast, y.ProgramIndex, y.ProjectCode
from dbo.[FundSource] g
join dbo.vSingularFundSourceAllocation x on x.FundSourceID = g.FundSourceID
join dbo.LoaStage y on y.FundSourceNumber =   RIGHT(g.FundSourceNumber, LEN(g.FundSourceNumber)-2) or y.FundSourceNumber = g.FundSourceNumber
where isnull(ltrim(rtrim(y.ProgramIndex)), '') != '99C'


union

select x.LoaStageID, min(x.FundSourceID), min(x.FundSourceAllocationID), x.IsNortheast, x.IsSoutheast, x.ProgramIndex, x.ProjectCode
from dbo.vLoaStageFundSourceAllocationByProgramIndexProjectCode x
where isnull(ltrim(rtrim(x.ProgramIndex)), '') != '99C'
group by x.LoaStageID, x.IsNortheast, x.IsSoutheast, x.ProgramIndex, x.ProjectCode having count(*) = 1





-- custom logic
union

select x.LoaStageID
, 65 as FundSourceID -- 2019-2021 DNR Forest Hazard Reduction Capital
, 313 as FundSourceAllocationID -- 2019-2021 DNR Forest Hazard Reduction Capital - SE Region LOA
, x.IsNortheast
, x.IsSoutheast
, x.ProgramIndex
, x.ProjectCode
from dbo.LoaStage x
where (ltrim(rtrim(x.ProgramIndex)) = '99C' and ltrim(rtrim(x.ProjectCode)) = 'WAE') or ltrim(rtrim(x.ProgramIndex)) = '99C-WAE'

union

select x.LoaStageID
, 65 as FundSourceID -- 2019-2021 DNR Forest Hazard Reduction Capital
, 312 as FundSourceAllocationID -- 2019-2021 DNR Forest Hazard Reduction Capital - NE Region LOA
, x.IsNortheast
, x.IsSoutheast
, x.ProgramIndex
, x.ProjectCode
from dbo.LoaStage x
where (ltrim(rtrim(x.ProgramIndex)) = '99C' and ltrim(rtrim(x.ProjectCode)) = 'WAD') or ltrim(rtrim(x.ProgramIndex)) = '99C-WAD'

union

select x.LoaStageID
, 66 as FundSourceID -- 2017-2019 DNR Forest Hazard Reduction Capital
, 324 as FundSourceAllocationID -- 2017-2019 DNR Forest Hazard Reduction Capital - NE/SE Region Landowner Assistance
, x.IsNortheast
, x.IsSoutheast
, x.ProgramIndex
, x.ProjectCode
from dbo.LoaStage x
where ltrim(rtrim(x.ProgramIndex)) = '99C' and (ltrim(rtrim(x.ProjectCode)) = '---' or ltrim(rtrim(x.ProjectCode)) = 'N/A' or ltrim(rtrim(x.ProjectCode)) = '' or x.ProjectCode is null)


/*
select * from dbo.vLoaStageFundSourceAllocation x where x.ProgramIndex like '%99c%'

*/
