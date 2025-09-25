

create view dbo.vSingularFundSourceAllocation
as



select g.FundSourceID, ga.FundSourceAllocationID from dbo.[FundSource] g
join dbo.FundSourceAllocation ga on ga.FundSourceID = g.FundSourceID
join (
    select g.FundSourceID from dbo.[FundSource] g
    join dbo.FundSourceAllocation ga on ga.FundSourceID = g.FundSourceID
    group by g.FundSourceID having count(*) = 1)
x on x.FundSourceID = g.FundSourceID

go

/*
select * from dbo.vSingularFundSourceAllocation

*/