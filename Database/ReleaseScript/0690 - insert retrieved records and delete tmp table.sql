






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
	join dbo.Project as p on p.ProjectID = tmpgar.ProjectID
	where
	ProjectGrantAllocationRequestID not in (select 
												pgar.ProjectGrantAllocationRequestID 
											from 
												dbo.tmpProjectGrantAllocationRequestBackup as pgar
												join dbo.ProjectFundSourceAllocationRequest as fsar on pgar.ProjectID = fsar.ProjectID and pgar.GrantAllocationID = fsar.FundSourceAllocationID)



	drop table dbo.tmpProjectGrantAllocationRequestBackup



/*
2023-SPS-0014
2025-NWS-0012


	select 
		p.ProjectName,
		fsa.FundSourceAllocationName,
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
		join dbo.Project as p on p.ProjectID = tmpgar.ProjectID
		join dbo.FundSourceAllocation as fsa on fsa.FundSourceAllocationID = tmpgar.GrantAllocationID
	where

		p.ProjectName in ('2023-SPS-0014','2025-NWS-0012')

		ProjectGrantAllocationRequestID not in (select 
													pgar.ProjectGrantAllocationRequestID 
												from 
													dbo.tmpProjectGrantAllocationRequestBackup as pgar
													join dbo.ProjectFundSourceAllocationRequest as fsar on pgar.ProjectID = fsar.ProjectID and pgar.GrantAllocationID = fsar.FundSourceAllocationID)




*/