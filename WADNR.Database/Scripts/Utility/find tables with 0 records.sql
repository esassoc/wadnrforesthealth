SELECT 
    t.NAME AS TableName,
	t.schema_id,
    p.rows AS RowCounts
FROM 
    sys.tables t
INNER JOIN 
    sys.partitions p ON t.object_id = p.OBJECT_ID 
WHERE 
    t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND p.rows = 1
	and t.schema_id = 1
GROUP BY 
    t.Name,t.schema_id, p.Rows
ORDER BY 
    t.Name