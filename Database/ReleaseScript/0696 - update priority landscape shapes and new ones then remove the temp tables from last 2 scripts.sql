

UPDATE pl
SET
    pl.PriorityLandscapeLocation = pleast.PriorityLandscapeLocation    
FROM 
    dbo.PriorityLandscape as pl
    INNER JOIN dbo.PriorityLandscapeEastTemp as pleast ON pl.PriorityLandscapeName = pleast.PriorityLandscapeName


INSERT INTO dbo.PriorityLandscape (PriorityLandscapeID, PriorityLandscapeName, PriorityLandscapeLocation, PlanYear, PriorityLandscapeCategoryID, PriorityLandscapeAboveMapText)
SELECT 
    (SELECT ISNULL(MAX(PriorityLandscapeID), 0) FROM dbo.PriorityLandscape) + ROW_NUMBER() OVER (ORDER BY pleast.PriorityLandscapeName) AS PriorityLandscapeID,
    pleast.PriorityLandscapeName,
    pleast.PriorityLandscapeLocation,
    pleast.PlanYear,
    pleast.PriorityLandscapeCategoryID,
    'This map displays the simple location of forest health projects in this priority landscape along with optional additional layers that users can select to view including detailed project and treatment locations.' as PriorityLandscapeAboveMapText
FROM dbo.PriorityLandscapeEastTemp as pleast
WHERE pleast.PriorityLandscapeName not in (select PriorityLandscapeName from dbo.PriorityLandscape as pl2 where pl2.PriorityLandscapeCategoryID = 1)



UPDATE pl
SET
    pl.PriorityLandscapeLocation = plwest.PriorityLandscapeLocation    
FROM 
    dbo.PriorityLandscape as pl
    INNER JOIN dbo.PriorityLandscapeWestTemp as plwest ON pl.PriorityLandscapeName = plwest.PriorityLandscapeName


INSERT INTO dbo.PriorityLandscape (PriorityLandscapeID, PriorityLandscapeName, PriorityLandscapeLocation, PlanYear, PriorityLandscapeCategoryID, PriorityLandscapeAboveMapText)
SELECT 
    (SELECT ISNULL(MAX(PriorityLandscapeID), 0) FROM dbo.PriorityLandscape) + ROW_NUMBER() OVER (ORDER BY plwest.PriorityLandscapeName) AS PriorityLandscapeID,
    plwest.PriorityLandscapeName,
    plwest.PriorityLandscapeLocation,
    plwest.PlanYear,
    plwest.PriorityLandscapeCategoryID,
    'This map displays the simple location of forest health projects in this priority landscape along with optional additional layers that users can select to view including detailed project and treatment locations.' as PriorityLandscapeAboveMapText
FROM dbo.PriorityLandscapeWestTemp as plwest
WHERE plwest.PriorityLandscapeName not in (select PriorityLandscapeName from dbo.PriorityLandscape as pl2 where pl2.PriorityLandscapeCategoryID = 2)



select PriorityLandscapeID into #tmpPriorityLandscapesToDelete from dbo.PriorityLandscape as pl where pl.PriorityLandscapeCategoryID = 1 and pl.PriorityLandscapeName not in (select PriorityLandscapeName from dbo.PriorityLandscapeEastTemp as pl2)


insert into #tmpPriorityLandscapesToDelete(PriorityLandscapeID) select PriorityLandscapeID from dbo.PriorityLandscape as pl where pl.PriorityLandscapeCategoryID = 2 and pl.PriorityLandscapeName not in (select PriorityLandscapeName from dbo.PriorityLandscapeWestTemp as pl2)


delete from dbo.ProjectPriorityLandscapeUpdate where PriorityLandscapeID in (select PriorityLandscapeID from #tmpPriorityLandscapesToDelete)
delete from dbo.ProjectPriorityLandscape where PriorityLandscapeID in (select PriorityLandscapeID from #tmpPriorityLandscapesToDelete)
delete from dbo.PriorityLandscapeFileResource where PriorityLandscapeID in (select PriorityLandscapeID from #tmpPriorityLandscapesToDelete)
delete from dbo.PriorityLandscape where PriorityLandscapeID in (select PriorityLandscapeID from #tmpPriorityLandscapesToDelete)


drop table dbo.PriorityLandscapeEastTemp
go

drop table dbo.PriorityLandscapeWestTemp
go
