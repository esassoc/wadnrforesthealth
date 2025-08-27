IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pImportLoaTabularData'))
    DROP PROCEDURE dbo.pImportLoaTabularData
GO

CREATE PROCEDURE dbo.pImportLoaTabularData
AS
begin
              if object_id('tempdb.dbo.#projectFundSourceAllocation') is not null drop table #projectFundSourceAllocation
              select x.ProjectID
              , x.FundSourceAllocationID
              , x.MatchAmount
              , x.PayAmount
              , x.LetterDate
              , x.ProjectExpirationDate
              , x.ProjectStatus
			  , x.ApplicationDate
			  , x.IsNortheast
			  , x.IsSoutheast
			  , x.LoaStageID
			  , x.DecisionDate
              into #projectFundSourceAllocation
              from dbo.vLoaStageProjectFundSourceAllocation x


              if object_id('tempdb.dbo.#projectFocusArea') is not null drop table #projectFocusArea
              select distinct p.ProjectID, fa.FocusAreaID
              into #projectFocusArea
              from dbo.LoaStage x
              join dbo.FocusArea fa on (fa.FocusAreaName like '%'+ x.FocusAreaName + '%') or 
                                       ((fa.FocusAreaName like '%'+(case when LEN(x.FocusAreaName) > 5 then LEFT(x.FocusAreaName, LEN(x.FocusAreaName)-5) else x.FocusAreaName end) + '%') and x.IsSoutheast = 1)
              join dbo.Project p on p.ProjectGisIdentifier = x.ProjectIdentifier
              where x.FocusAreaName is not null 

              if object_id('tempdb.dbo.#projectFocusAreaForUpdate') is not null drop table #projectFocusAreaForUpdate
              select distinct x.ProjectID, min(x.FocusAreaID) as FocusAreaID
              into #projectFocusAreaForUpdate
              from #projectFocusArea x
              group by x.ProjectID having count(*) =1


              update dbo.Project
              set FocusAreaID = pfa.FocusAreaID
              from dbo.Project p
              join #projectFocusAreaForUpdate pfa on pfa.ProjectID = p.ProjectID


              if object_id('tempdb.dbo.#projectFundSourceAllocationRequestPart') is not null drop table #projectFundSourceAllocationRequestPart
              select x.ProjectID,
                     x.FundSourceAllocationID
                     , x.MatchAmount + x.PayAmount as TotalAmount
                     , x.MatchAmount as MatchAmount
                     , x.PayAmount as PayAmount
                     into #projectFundSourceAllocationRequestPart
                      from #projectFundSourceAllocation x where x.FundSourceAllocationID is not null


            if object_id('tempdb.dbo.#projectFundSourceAllocationRequest') is not null drop table #projectFundSourceAllocationRequest
              select x.ProjectID,
                     x.FundSourceAllocationID
                     , sum(x.TotalAmount) as TotalAmount
                     , sum(x.MatchAmount) as MatchAmount
                     , sum(x.PayAmount) as PayAmount
                     into #projectFundSourceAllocationRequest
                      from #projectFundSourceAllocationRequestPart x group by x.ProjectID, x.FundSourceAllocationID




              delete from dbo.ProjectFundSourceAllocationRequest where ProjectFundSourceAllocationRequestID in (select ProjectFundSourceAllocationRequestID 
                                                                                                      from 
                                                                                                      dbo.ProjectFundSourceAllocationRequest pgar
                                                                                                      join dbo.Project p on pgar.ProjectID = p.ProjectID
                                                                                                      join dbo.ProjectProgram pp on p.ProjectID = pp.ProjectID
                                                                                                      where pp.ProgramID = 3 -- LoaProgram)
                                                                                                      )

              insert into dbo.ProjectFundSourceAllocationRequest(ProjectID, FundSourceAllocationID, TotalAmount, MatchAmount, PayAmount, CreateDate, ImportedFromTabularData)

              select x.ProjectID,
                     x.FundSourceAllocationID
                     , x.TotalAmount
                     , x.MatchAmount
                     , x.PayAmount
                     , getdate()
                     , 1
                      from #projectFundSourceAllocationRequest x 


                update dbo.Project 
                set EstimatedTotalCost = y.EstimatedTotalCost
                ,   ExpirationDate = y.ExpirationDate
				,   PlannedDate = y.LetterDate
				,	SubmissionDate = y.ApplicationDate
				,	ApprovalDate = y.DecisionDate
                from dbo.Project p
                join (select x.ProjectID
                , sum(x.MatchAmount) + sum(x.PayAmount) as EstimatedTotalCost
                , max(x.ProjectExpirationDate) as ExpirationDate
				, min(x.LetterDate) as LetterDate
				, min(x.ApplicationDate) as ApplicationDate
				, min(x.DecisionDate) as DecisionDate
                  from  #projectFundSourceAllocation x group by x.ProjectID) y on y.ProjectID = p.ProjectID



              if object_id('tempdb.dbo.#projectForesterInfo') is not null drop table #projectForesterInfo
              select distinct p.ProjectID, person.PersonID, x.ForesterFirstName, x.ForesterLastName, x.ForesterPhone, x.ForesterEmail, pp.ProjectPersonID, ppOld.ProjectPersonID as OldProjectPersonID
              into #projectForesterInfo
              from dbo.LoaStage x
              join dbo.Project p on p.ProjectGisIdentifier = x.ProjectIdentifier
              left join dbo.Person person on person.Email = x.ForesterEmail
              left join dbo.ProjectPerson pp on pp.PersonID = person.PersonID and pp.ProjectID = p.ProjectID and pp.ProjectPersonRelationshipTypeID = (select top 1 x.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType x where x.ProjectPersonRelationshipTypeName = 'PrimaryContact')
              left join dbo.ProjectPerson ppOld on ppOld.ProjectID = p.ProjectID and ppOld.ProjectPersonRelationshipTypeID = (select top 1 x.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType x where x.ProjectPersonRelationshipTypeName = 'PrimaryContact')

                delete from dbo.ProjectPerson where ProjectPersonID in 
                (select x.OldProjectPersonID from #projectForesterInfo x where (x.ProjectPersonID is not null and x.OldProjectPersonID is not null and x.ProjectPersonID != x.OldProjectPersonID) or 
                                                                                          (x.ProjectPersonID is null and x.OldProjectPersonID is not null and x.ForesterFirstName is not null and x.ForesterLastName is not null) )

              insert into dbo.ProjectPerson(ProjectID, PersonID, ProjectPersonRelationshipTypeID, CreatedAsPartOfBulkImport)
              select 
              x.ProjectID
              , x.PersonID
              , (select top 1 y.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType y where y.ProjectPersonRelationshipTypeName = 'PrimaryContact')
              , 1
              from #projectForesterInfo x
              where x.PersonID is not null and x.ProjectPersonID is null


              insert into dbo.Person (FirstName, LastName, Phone, Email, CreatedAsPartOfBulkImport, CreateDate, IsActive, ReceiveSupportEmails)
              select distinct x.ForesterFirstName, x.ForesterLastName, x.ForesterPhone, x.ForesterEmail, 1, GETDATE(), 1, 0
              from #projectForesterInfo x
              where x.ForesterFirstName is not null and x.ForesterLastName is not null and x.ForesterEmail is not null and x.ForesterEmail != '#N/A' and x.PersonID is null


              if object_id('tempdb.dbo.#projectForesterInfoRoundTwo') is not null drop table #projectForesterInfoRoundTwo
              select distinct p.ProjectID, person.PersonID, x.ForesterFirstName, x.ForesterLastName, x.ForesterPhone, x.ForesterEmail, pp.ProjectPersonID, ppOld.ProjectPersonID as OldProjectPersonID
              into #projectForesterInfoRoundTwo
              from dbo.LoaStage x
              join dbo.Project p on p.ProjectGisIdentifier = x.ProjectIdentifier
              left join dbo.Person person on person.Email = x.ForesterEmail
              left join dbo.ProjectPerson pp on pp.PersonID = person.PersonID and pp.ProjectID = p.ProjectID and pp.ProjectPersonRelationshipTypeID = (select top 1 x.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType x where x.ProjectPersonRelationshipTypeName = 'PrimaryContact')
              left join dbo.ProjectPerson ppOld on ppOld.ProjectID = p.ProjectID and ppOld.ProjectPersonRelationshipTypeID = (select top 1 x.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType x where x.ProjectPersonRelationshipTypeName = 'PrimaryContact')


              delete from dbo.ProjectPerson where ProjectPersonID in 
                (select x.OldProjectPersonID from #projectForesterInfoRoundTwo x where (x.ProjectPersonID is not null and x.OldProjectPersonID is not null and x.ProjectPersonID != x.OldProjectPersonID) or 
                                                                                          (x.ProjectPersonID is null and x.OldProjectPersonID is not null and x.ForesterFirstName is not null and x.ForesterLastName is not null) )

              insert into dbo.ProjectPerson(ProjectID, PersonID, ProjectPersonRelationshipTypeID, CreatedAsPartOfBulkImport)
              select 
              x.ProjectID
              , x.PersonID
              , (select top 1 y.ProjectPersonRelationshipTypeID from dbo.ProjectPersonRelationshipType y where y.ProjectPersonRelationshipTypeName = 'PrimaryContact')
              , 1
              from #projectForesterInfoRoundTwo x
              where x.PersonID is not null and x.ProjectPersonID is null


end

/*

exec dbo.pImportLoaTabularData

*/