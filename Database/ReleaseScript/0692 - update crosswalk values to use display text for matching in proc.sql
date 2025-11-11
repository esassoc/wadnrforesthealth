



update dbo.GisCrossWalkDefault set GisCrossWalkMappedValue = 'Non-Commercial' where GisCrossWalkMappedValue = 'NonCommercial'
	update dbo.GisCrossWalkDefault set GisCrossWalkMappedValue = 'Prescribed Fire' where GisCrossWalkMappedValue = 'PrescribedFire'
	update dbo.GisCrossWalkDefault set GisCrossWalkMappedValue = 'Broadcast Burn' where GisCrossWalkMappedValue = 'BroadcastBurn'