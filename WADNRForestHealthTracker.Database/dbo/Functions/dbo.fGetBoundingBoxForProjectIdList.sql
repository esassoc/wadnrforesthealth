
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetBoundingBoxForProjectIdList'))
    drop function dbo.fGetBoundingBoxForProjectIdList
go

create function dbo.fGetBoundingBoxForProjectIdList(@ProjectIDList varchar(max))
returns @rtnTable TABLE 
(
    -- columns returned by the function
    SWLatitude float NOT NULL,
	SWLongitude float NOT NULL,
    NELatitude float NOT NULL,
	NELongitude float NOT NULL
)
begin
	declare @myBoundingBox varchar(1000);

	SELECT @myBoundingBox = geometry::EnvelopeAggregate(ProjectLocationPoint).ToString()
	FROM dbo.Project
	where ProjectID in (select cast(value as int) from string_split(@ProjectIDList, ',')) and ProjectLocationPoint is not null;

	declare @g geometry = geometry::STGeomFromText(@myBoundingBox, 0);
	insert into @rtnTable
	select @g.STPointN(1).STX as [SWLatitude], @g.STPointN(2).STY as [SWLongitude], @g.STPointN(3).STX as [NELatitude], @g.STPointN(4).STY as [NELongitude];

	return
end

go



/*

select * IDList


declare @g geometry = geometry::STGeomFromText('POLYGON ((-179.231086 51.175092, 179.859681 51.175092, 179.859681 71.441059, -179.231086 71.441059, -179.231086 51.175092))', 0);
select @g.STPointN(1) as [SW], @g.STPointN(3) as [NE]


declare @projectIDList dbo.IDList 
insert into @projectIDList(id) select top 100 ProjectID from dbo.Project
select * from dbo.fGetBoundingBoxForProjectIdList(@projectIDList) 




	declare @myBoundingBox varchar(max)
	declare @projectIDList dbo.IDList 
	insert into @projectIDList(id) select top 100 ProjectID from dbo.Project

	SELECT geometry::EnvelopeAggregate(ProjectLocationPoint).ToString() 
	FROM dbo.Project
	where ProjectID in (select ID from @projectIDList);

	return @myBoundingBox


*/


