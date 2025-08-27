IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.procImportTreatmentsFromGisUploadAttempt'))
    drop procedure dbo.procImportTreatmentsFromGisUploadAttempt
go
create procedure dbo.procImportTreatmentsFromGisUploadAttempt
(
    @piGisUploadAttemptID int,
    @projectIdentifierGisMetadataAttributeID int,
    @footprintAcresMetadataAttributeID int,
    @treatedAcresMetadataAttributeID int,
    @treatmentTypeMetadataAttributeID int,
    @treatmentDetailedActivityTypeMetadataAttributeID int,
    @treatmentTypeID int,
    @treatmentDetailedActivityTypeID int,
    @isFlattened bit,
    @pruningAcresMetadataAttributeID int,
    @thinningAcresMetadataAttributeID int,
    @chippingAcresMetadataAttributeID int,
    @masticationAcresMetadataAttributeID int,
    @grazingAcresMetadataAttributeID int,
    @lopScatterAcresMetadataAttributeID int,
    @biomassRemovalAcresMetadataAttributeID int,
    @handPileAcresMetadataAttributeID int,
    @handPileBurnAcresMetadataAttributeID int,
    @machineBurnAcresMetadataAttributeID int,
    @broadcastBurnAcresMetadataAttributeID int,
    @otherBurnAcresMetadataAttributeID int,
    @startDateMetadataAttributeID int,
    @endDateMetadataAttributeID int
)
as


declare @programID int;
set @programID = (select giuso.ProgramID from dbo.GisUploadAttempt gia join dbo.GisUploadSourceOrganization giuso on giuso.GisUploadSourceOrganizationID = gia.GisUploadSourceOrganizationID where gia.GisUploadAttemptID = @piGisUploadAttemptID);


if object_id('tempdb.dbo.#tempTreatmentsForDelete') is not null drop table #tempTreatmentsForDelete
select p.ProjectID, t.TreatmentID, pl.ProjectLocationID 
into #tempTreatmentsForDelete
from dbo.Project p
join dbo.Treatment t on t.ProjectID = p.ProjectID
join dbo.ProjectLocation pl on t.ProjectLocationID = pl.ProjectLocationID
where  p.ProjectGisIdentifier in (
	select distinct gfma.GisFeatureMetadataAttributeValue 
	from dbo.GisFeature gf 
    join dbo.GisFeatureMetadataAttribute gfma on gfma.GisFeatureID = gf.GisFeatureID 
    where gfma.GisMetadataAttributeID = @projectIdentifierGisMetadataAttributeID and gf.GisUploadAttemptID = @piGisUploadAttemptID)
		and t.ProgramID = @programID

delete from dbo.Treatment where TreatmentID in (select TreatmentID from #tempTreatmentsForDelete)

delete from dbo.ProjectLocation where ProjectLocationID in (select ProjectLocationID from #tempTreatmentsForDelete)


if object_id('tempdb.dbo.#tempTreatments') is not null drop table #tempTreatments

CREATE TABLE #tempTreatments(TemporaryTreatmentCacheID [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
    ProjectLocationGeometry [geometry] NOT NULL,
	[TreatmentStartDate] [datetime] NULL,
	[TreatmentEndDate] [datetime] NULL,
	[TreatmentFootprintAcres] [decimal](38, 10) NOT NULL,
    [TreatedAcres] [decimal](38, 10) NULL,
	[TreatmentNotes] [varchar](2000) NULL,
	[TreatmentTypeID] [int] NOT NULL,
    [TreatmentDetailedActivityTypeID] [int] not null,
    TreatmentTypeImportedText varchar(200),
    [TreatmentDetailedActivityTypeImportedText] varchar(200),
    PruningAcres [decimal](38, 10) NULL,
    ThinningAcres [decimal](38, 10) NULL,
    ChippingAcres [decimal](38, 10) NULL,
    MasticationAcres [decimal](38, 10) NULL,
    GrazingAcres [decimal](38, 10) NULL,
    LopScatterAcres [decimal](38, 10) NULL,
    BiomassRemovalAcres [decimal](38, 10) NULL,
    HandPileAcres [decimal](38, 10) NULL,
    HandPileBurnAcres [decimal](38, 10) NULL,
    MachineBurnAcres [decimal](38, 10) NULL,
    BroadcastBurnAcres [decimal](38, 10) NULL,
    OtherAcres [decimal](38, 10) NULL)



INSERT INTO #tempTreatments
           ([ProjectID]
           ,ProjectLocationGeometry
           ,[TreatmentStartDate]
           ,[TreatmentEndDate]
           ,[TreatmentFootprintAcres]
           ,[TreatedAcres]
           ,[TreatmentNotes]
           ,[TreatmentTypeID]
           ,[TreatmentDetailedActivityTypeID]
           ,[TreatmentTypeImportedText]
           ,[TreatmentDetailedActivityTypeImportedText]
           ,PruningAcres
           ,ThinningAcres
           ,ChippingAcres
           ,MasticationAcres
           ,GrazingAcres
           ,LopScatterAcres
           ,BiomassRemovalAcres
           ,HandPileAcres
           ,HandPileBurnAcres
           ,MachineBurnAcres
           ,BroadcastBurnAcres
           ,OtherAcres)



select p.ProjectID
, x.GisFeatureGeometry.MakeValid()
, case when guso.ApplyStartDateToTreatments = 1 then coalesce(TRY_PARSE(x.StartDate AS DATETIME), dbo.fConvertDateTimeUtcToPacificStandardTime(dbo.fUnixTimeToDateTime2(TRY_PARSE(x.StartDate AS bigint))), null) else null end as TreatmentStartDate
, case when guso.ApplyEndDateToTreatments = 1 then coalesce(TRY_PARSE(x.EndDate AS DATETIME), dbo.fConvertDateTimeUtcToPacificStandardTime(dbo.fUnixTimeToDateTime2(TRY_PARSE(x.EndDate AS bigint))), null) else null end as TreatmentEndDate
, case  when @footprintAcresMetadataAttributeID = -1 then isnull(CalculatedArea,0)
        else isnull(TRY_PARSE(x.FootprintAcres AS decimal(38,10)),0) end  as [TreatmentFootprintAcres]
, case  when @treatedAcresMetadataAttributeID = -1 then isnull(CalculatedArea,0)
        else isnull(TRY_PARSE(x.TreatedAcres AS decimal(38,10)),0) end  as [TreatedAcres]
, null
, @treatmentTypeID -- TreatmentTypeID
, @treatmentDetailedActivityTypeID -- TreatmentDetailedActivityTypeID
, x.TreatmentTypeImportedText
, x.TreatmentDetailedActivityTypeImportedText
, isnull(TRY_PARSE(x.PruningAcres AS decimal(38,10)),0)  as PruningAcres
, isnull(TRY_PARSE(x.ThinningAcres AS decimal(38,10)),0)  as ThinningAcres
, isnull(TRY_PARSE(x.ChippingAcres AS decimal(38,10)),0)  as ChippingAcres
, isnull(TRY_PARSE(x.MasticationAcres AS decimal(38,10)),0)  as MasticationAcres
, isnull(TRY_PARSE(x.GrazingAcres AS decimal(38,10)),0)  as GrazingAcres
, isnull(TRY_PARSE(x.LopScatterAcres AS decimal(38,10)),0)  as LopScatterAcres
, isnull(TRY_PARSE(x.BiomassRemovalAcres AS decimal(38,10)),0)  as BiomassRemovalAcres
, isnull(TRY_PARSE(x.HandPileAcres AS decimal(38,10)),0)  as HandPileAcres
, isnull(TRY_PARSE(x.HandPileBurnAcres AS decimal(38,10)),0)  as HandPileBurnAcres
, isnull(TRY_PARSE(x.MachineBurnAcres AS decimal(38,10)),0)  as MachineBurnAcres
, isnull(TRY_PARSE(x.BroadcastBurnAcres AS decimal(38,10)),0)  as BroadcastBurnAcres
, isnull(TRY_PARSE(x.OtherAcres AS decimal(38,10)),0)  as OtherAcres
 from dbo.Project p
 join dbo.ProjectProgram pp on p.ProjectID = pp.ProjectID
join (
        select gf.GisFeatureGeometry
        , gf.CalculatedArea
        , gfma.GisFeatureMetadataAttributeValue
        , gfmaFootprint.GisFeatureMetadataAttributeValue as FootprintAcres
        , gfmaTreated.GisFeatureMetadataAttributeValue as TreatedAcres 
        , gfmaTreatmentType.GisFeatureMetadataAttributeValue as TreatmentTypeImportedText
        , gfmaTreatmentDetailedActivityType.GisFeatureMetadataAttributeValue as TreatmentDetailedActivityTypeImportedText
        , gfmaPruning.GisFeatureMetadataAttributeValue as PruningAcres
        , gfmaThinning.GisFeatureMetadataAttributeValue as ThinningAcres
        , gfmaChipping.GisFeatureMetadataAttributeValue as ChippingAcres
        , gfmaMastication.GisFeatureMetadataAttributeValue as MasticationAcres
        , gfmaGrazing.GisFeatureMetadataAttributeValue as GrazingAcres
        , gfmaLopScatter.GisFeatureMetadataAttributeValue as LopScatterAcres
        , gfmaBiomassRemoval.GisFeatureMetadataAttributeValue as BiomassRemovalAcres
        , gfmaHandPile.GisFeatureMetadataAttributeValue as HandPileAcres
        , gfmaHandPileBurn.GisFeatureMetadataAttributeValue as HandPileBurnAcres
        , gfmaMachineBurn.GisFeatureMetadataAttributeValue as MachineBurnAcres
        , gfmaBroadcastBurn.GisFeatureMetadataAttributeValue as BroadcastBurnAcres
        , gfmaOther.GisFeatureMetadataAttributeValue as OtherAcres
        , gfmaStartDate.GisFeatureMetadataAttributeValue as StartDate
        , gfmaEndDate.GisFeatureMetadataAttributeValue as EndDate
        from dbo.GisFeature gf
        join dbo.GisFeatureMetadataAttribute gfma on gfma.GisFeatureID = gf.GisFeatureID
        left join dbo.GisFeatureMetadataAttribute gfmaFootprint on gfmaFootprint.GisFeatureID = gf.GisFeatureID and gfmaFootprint.GisMetadataAttributeID = @footprintAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaTreated on gfmaTreated.GisFeatureID = gf.GisFeatureID and gfmaTreated.GisMetadataAttributeID = @treatedAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaTreatmentType on gfmaTreatmentType.GisFeatureID = gf.GisFeatureID and gfmaTreatmentType.GisMetadataAttributeID = @treatmentTypeMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaTreatmentDetailedActivityType on gfmaTreatmentDetailedActivityType.GisFeatureID = gf.GisFeatureID and gfmaTreatmentDetailedActivityType.GisMetadataAttributeID = @treatmentDetailedActivityTypeMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaPruning on gfmaPruning.GisFeatureID = gf.GisFeatureID and gfmaPruning.GisMetadataAttributeID = @pruningAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaThinning on gfmaThinning.GisFeatureID = gf.GisFeatureID and gfmaThinning.GisMetadataAttributeID = @thinningAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaChipping on gfmaChipping.GisFeatureID = gf.GisFeatureID and gfmaChipping.GisMetadataAttributeID = @chippingAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaMastication on gfmaMastication.GisFeatureID = gf.GisFeatureID and gfmaMastication.GisMetadataAttributeID = @masticationAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaGrazing on gfmaGrazing.GisFeatureID = gf.GisFeatureID and gfmaGrazing.GisMetadataAttributeID = @grazingAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaLopScatter on gfmaLopScatter.GisFeatureID = gf.GisFeatureID and gfmaLopScatter.GisMetadataAttributeID = @lopScatterAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaBiomassRemoval on gfmaBiomassRemoval.GisFeatureID = gf.GisFeatureID and gfmaBiomassRemoval.GisMetadataAttributeID = @biomassRemovalAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaHandPile on gfmaHandPile.GisFeatureID = gf.GisFeatureID and gfmaHandPile.GisMetadataAttributeID = @handPileAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaHandPileBurn on gfmaHandPileBurn.GisFeatureID = gf.GisFeatureID and gfmaHandPileBurn.GisMetadataAttributeID = @handPileBurnAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaMachineBurn on gfmaMachineBurn.GisFeatureID = gf.GisFeatureID and gfmaMachineBurn.GisMetadataAttributeID = @machineBurnAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaBroadcastBurn on gfmaBroadcastBurn.GisFeatureID = gf.GisFeatureID and gfmaBroadcastBurn.GisMetadataAttributeID = @broadcastBurnAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaOther on gfmaOther.GisFeatureID = gf.GisFeatureID and gfmaOther.GisMetadataAttributeID = @otherBurnAcresMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaStartDate on gfmaStartDate.GisFeatureID = gf.GisFeatureID and gfmaStartDate.GisMetadataAttributeID = @startDateMetadataAttributeID
        left join dbo.GisFeatureMetadataAttribute gfmaEndDate on gfmaEndDate.GisFeatureID = gf.GisFeatureID and gfmaEndDate.GisMetadataAttributeID = @endDateMetadataAttributeID
        where gfma.GisMetadataAttributeID = @projectIdentifierGisMetadataAttributeID 
        and gf.GisUploadAttemptID = @piGisUploadAttemptID)
   x on x.GisFeatureMetadataAttributeValue = p.ProjectGisIdentifier
   join dbo.GisUploadAttempt as gua on gua.GisUploadAttemptID = p.CreateGisUploadAttemptID
   join dbo.GisUploadSourceOrganization as guso on guso.GisUploadSourceOrganizationID = gua.GisUploadSourceOrganizationID
where  p.ProjectGisIdentifier in (select distinct gfma.GisFeatureMetadataAttributeValue from dbo.GisFeature gf 
                        join dbo.GisFeatureMetadataAttribute gfma on gfma.GisFeatureID = gf.GisFeatureID 
                        where gfma.GisMetadataAttributeID = @projectIdentifierGisMetadataAttributeID and gf.GisUploadAttemptID = @piGisUploadAttemptID)
               and pp.ProgramID = @programID


--ProjectLocationTypeID 2 is TreatmentArea
insert into dbo.ProjectLocation(ProjectLocationGeometry, TemporaryTreatmentCacheID, ProjectID, ProjectLocationTypeID, ProjectLocationName)
select 
	x.ProjectLocationGeometry
	, x.TemporaryTreatmentCacheID
	, x.ProjectID
	, 2 as ProjectLocationTypeID
	, CONCAT('Imported Treatment Area ', x.ProjectID, ' ', x.TemporaryTreatmentCacheID, @piGisUploadAttemptID) as ProjectLocationName 
from #tempTreatments x


if(@isFlattened = 0)
begin


    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID
               , x.TreatmentTypeID
               , x.TreatmentDetailedActivityTypeID
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.TreatedAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID
end

if(@isFlattened = 1)
begin

    ----Pruning--------------------------------------------------------------------------------
     insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
               , @programID
               , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 2 -- Pruning
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.PruningAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    ----Thinning--------------------------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                , @programID
                , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 3 -- Thinning
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.ThinningAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID



    --Chipping-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
      , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 1 -- chipping
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.ChippingAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Mastication-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 4 -- Mastication
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.MasticationAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID

    --Grazing-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
               , @programID
               , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 5 -- Grazing
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.GrazingAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Lop and Scatter-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 6 -- Lop and Scatter
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.LopScatterAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID

    --Biomass Removal-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 7 -- Biomass Removal
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.BiomassRemovalAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID

    --Hand Pile-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 3 -- non-commercial
               , 8 -- Hand Pile
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.HandPileAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Hand Pile Burn-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 ,@programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 2 -- burn
               , 10 -- Hand Pile Burn
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.HandPileBurnAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Machine Burn-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID 
               , 2 -- burn
               , 11 -- Machine Burn
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.MachineBurnAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Broadcast Burn-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID
               , 2 -- fire
               , 9 -- Broadcast Burn
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.BroadcastBurnAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID


    --Other-------------------------------------------------------------
    insert into dbo.Treatment ([ProjectID]
               , [ProgramID]
               , ImportedFromGis
               ,[TreatmentStartDate]
               ,[TreatmentEndDate]
               ,[TreatmentFootprintAcres]
               ,[TreatmentNotes]
               , ProjectLocationID
               , TreatmentTypeID
               , TreatmentDetailedActivityTypeID
               , TreatmentTypeImportedText
               , TreatmentDetailedActivityTypeImportedText
               , CreateGisUploadAttemptID
               , TreatmentTreatedAcres)

    select 

                 x.ProjectID
                 , @programID
                 , 1
               , x.TreatmentStartDate
               , x.TreatmentEndDate
               , x.TreatmentFootprintAcres
               , x.TreatmentNotes
               , pl.ProjectLocationID
               , 3 -- non-commercial
               , 13 -- Other
               , x.TreatmentTypeImportedText
               , x.TreatmentDetailedActivityTypeImportedText
               , @piGisUploadAttemptID
               , x.OtherAcres

    from #tempTreatments x
    join dbo.ProjectLocation pl on pl.TemporaryTreatmentCacheID = x.TemporaryTreatmentCacheID
end


update dbo.ProjectLocation
set TemporaryTreatmentCacheID = null


if(@isFlattened = 0)
begin
    update dbo.Treatment
    set TreatmentTypeID = tt.TreatmentTypeID
    from dbo.Treatment t
    join dbo.GisUploadAttempt gua on t.CreateGisUploadAttemptID = gua.GisUploadAttemptID
    join dbo.GisUploadSourceOrganization guso on guso.GisUploadSourceOrganizationID = gua.GisUploadSourceOrganizationID
    join dbo.GisCrossWalkDefault gcwd on gcwd.GisUploadSourceOrganizationID = guso.GisUploadSourceOrganizationID and gcwd.GisCrossWalkSourceValue = t.TreatmentTypeImportedText
    join dbo.TreatmentType tt on tt.TreatmentTypeDisplayName = gcwd.GisCrossWalkMappedValue
    where gcwd.FieldDefinitionID = 468



    update dbo.Treatment
    set TreatmentDetailedActivityTypeID = tt.TreatmentDetailedActivityTypeID
    from dbo.Treatment t
    join dbo.GisUploadAttempt gua on t.CreateGisUploadAttemptID = gua.GisUploadAttemptID
    join dbo.GisUploadSourceOrganization guso on guso.GisUploadSourceOrganizationID = gua.GisUploadSourceOrganizationID
    join dbo.GisCrossWalkDefault gcwd on gcwd.GisUploadSourceOrganizationID = guso.GisUploadSourceOrganizationID and gcwd.GisCrossWalkSourceValue = t.TreatmentDetailedActivityTypeImportedText
    join dbo.TreatmentDetailedActivityType tt on tt.TreatmentDetailedActivityTypeDisplayName = gcwd.GisCrossWalkMappedValue
    where gcwd.FieldDefinitionID = 469
end


update dbo.Project
set ProjectLocationPoint = SimplePoint, ProjectLocationSimpleTypeID = 1
from dbo.Project x join (

select p.ProjectID,  geometry::UnionAggregate(pl.ProjectLocationGeometry).STCentroid() as SimplePoint  from dbo.Project p
join dbo.Treatment t on p.ProjectID = t.ProjectID
join dbo.ProjectLocation pl on pl.ProjectLocationID = t.ProjectLocationID
group by p.ProjectID) y on x.ProjectID = y.ProjectID
where x.CreateGisUploadAttemptID = @piGisUploadAttemptID


/*

exec dbo.procImportTreatmentsFromGisUploadAttempt @piGisUploadAttemptID = 1, @projectIdentifierGisMetadataAttributeID = 39, 13

*/