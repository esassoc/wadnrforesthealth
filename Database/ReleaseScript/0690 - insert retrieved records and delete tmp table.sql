






insert into dbo.ProjectFundSourceAllocationRequest (ProjectID, FundSourceAllocationID, TotalAmount, MatchAmount, PayAmount, CreateDate, UpdateDate, ImportedFromTabularData)
	select 
		tmpgar.ProjectID as ProjectID,
		tmpgar.GrantAllocationID as FundSourceAllocationID,
		tmpgar.TotalAmount as TotalAmount,
		tmpgar.MatchAmount,
		tmpgar.PayAmount,
		tmpgar.CreateDate,
		tmpgar.UpdateDate,
		tmpgar.ImportedFromTabularData
	from
	dbo.tmpProjectGrantAllocationRequestBackup as tmpgar
	where
	ProjectGrantAllocationRequestID not in (select 
	pgar.ProjectGrantAllocationRequestID 
from 
	dbo.tmpProjectGrantAllocationRequestBackup as pgar
	join dbo.ProjectFundSourceAllocationRequest as fsar on pgar.ProjectID = fsar.ProjectID and pgar.GrantAllocationID = fsar.FundSourceAllocationID)



	drop table dbo.tmpProjectGrantAllocationRequestBackup