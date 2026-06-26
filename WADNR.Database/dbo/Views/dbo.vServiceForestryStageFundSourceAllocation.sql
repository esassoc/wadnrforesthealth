

create view dbo.vServiceForestryStageFundSourceAllocation
as


-- Singular fund source match: the staged FundSource text matches a FundSource that has
-- exactly one allocation (optionally stripping a 2-char prefix, as the LOA view does).
select y.ServiceForestryStageID, g.FundSourceID, x.FundSourceAllocationID, y.DCProgramIndex, y.DCProjectCode
from dbo.[FundSource] g
join dbo.vSingularFundSourceAllocation x on x.FundSourceID = g.FundSourceID
join dbo.ServiceForestryStage y on y.FundSource = RIGHT(g.FundSourceNumber, LEN(g.FundSourceNumber)-2) or y.FundSource = g.FundSourceNumber
where isnull(ltrim(rtrim(y.DCProgramIndex)), '') != '99C'


union

-- Program Index + Project Code match (single unambiguous allocation only).
select x.ServiceForestryStageID, min(x.FundSourceID), min(x.FundSourceAllocationID), x.DCProgramIndex, x.DCProjectCode
from dbo.vServiceForestryStageFundSourceAllocationByProgramIndexProjectCode x
where isnull(ltrim(rtrim(x.DCProgramIndex)), '') != '99C'
group by x.ServiceForestryStageID, x.DCProgramIndex, x.DCProjectCode having count(*) = 1


-- custom logic: '99C' special cases. Service Forestry is the same program (Landowner
-- Assistance) and same NE/SE regions as LOA, so the hardcoded allocation IDs mirror
-- dbo.vLoaStageFundSourceAllocation. (Validate against real Service Forestry data.)
union

select x.ServiceForestryStageID
, 65 as FundSourceID -- 2019-2021 DNR Forest Hazard Reduction Capital
, 313 as FundSourceAllocationID -- 2019-2021 DNR Forest Hazard Reduction Capital - SE Region LOA
, x.DCProgramIndex
, x.DCProjectCode
from dbo.ServiceForestryStage x
where (ltrim(rtrim(x.DCProgramIndex)) = '99C' and ltrim(rtrim(x.DCProjectCode)) = 'WAE') or ltrim(rtrim(x.DCProgramIndex)) = '99C-WAE'

union

select x.ServiceForestryStageID
, 65 as FundSourceID -- 2019-2021 DNR Forest Hazard Reduction Capital
, 312 as FundSourceAllocationID -- 2019-2021 DNR Forest Hazard Reduction Capital - NE Region LOA
, x.DCProgramIndex
, x.DCProjectCode
from dbo.ServiceForestryStage x
where (ltrim(rtrim(x.DCProgramIndex)) = '99C' and ltrim(rtrim(x.DCProjectCode)) = 'WAD') or ltrim(rtrim(x.DCProgramIndex)) = '99C-WAD'

union

select x.ServiceForestryStageID
, 66 as FundSourceID -- 2017-2019 DNR Forest Hazard Reduction Capital
, 324 as FundSourceAllocationID -- 2017-2019 DNR Forest Hazard Reduction Capital - NE/SE Region Landowner Assistance
, x.DCProgramIndex
, x.DCProjectCode
from dbo.ServiceForestryStage x
where ltrim(rtrim(x.DCProgramIndex)) = '99C' and (ltrim(rtrim(x.DCProjectCode)) = '---' or ltrim(rtrim(x.DCProjectCode)) = 'N/A' or ltrim(rtrim(x.DCProjectCode)) = '' or x.DCProjectCode is null)

go

/*
select * from dbo.vServiceForestryStageFundSourceAllocation x where x.DCProgramIndex like '%99c%'

*/
