IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetProjectDnrUploadRegion'))
drop function dbo.fGetProjectDnrUploadRegion
GO
CREATE FUNCTION dbo.fGetProjectDnrUploadRegion
(
    @piGisUploadAttemptID int,
    @piGisMetadataAttributeID int,
    @programID int
)
returns table
as return(


    select number = ROW_NUMBER() OVER (ORDER BY x.ProjectID, x.DNRUplandRegionID), x.ProjectID, x.DNRUplandRegionID 
    
    from(

    
     select distinct x.ProjectID, landscape.DNRUplandRegionID

    from dbo.ProjectLocation pl
        join (
        select distinct p.CreateGisUploadAttemptID
        , p.LastUpdateGisUploadAttemptID
        , p.ProjectID
        , pl.ProjectLocationID 
        , p.ProjectGisIdentifier
        , pp.ProgramID
        from dbo.ProjectLocation pl
        join dbo.Project p on pl.ProjectID = p.ProjectID
        join dbo.ProjectProgram pp on pp.ProjectID = p.ProjectID
        ) x on x.ProjectLocationID = pl.ProjectLocationID
        join dbo.DNRUplandRegion landscape on  landscape.DNRUplandRegionLocation.STIntersects(pl.ProjectLocationGeometry) = 1
         where  x.ProjectGisIdentifier in (select distinct gfma.GisFeatureMetadataAttributeValue from dbo.GisFeature gf 
                        join dbo.GisFeatureMetadataAttribute gfma on gfma.GisFeatureID = gf.GisFeatureID 
                        where gfma.GisMetadataAttributeID = @piGisMetadataAttributeID and gf.GisUploadAttemptID = @piGisUploadAttemptID)
               and x.ProgramID = @programID 



        ) x

)
go

