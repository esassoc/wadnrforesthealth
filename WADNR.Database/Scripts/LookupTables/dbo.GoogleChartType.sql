merge into dbo.GoogleChartType as Target
using (values
(1, 'ColumnChart', 'ColumnChart', 'bars'),
(2, 'LineChart', 'LineChart', 'line'),
(3, 'ComboChart', 'ComboChart', null),
(4, 'AreaChart', 'AreaChart', 'area'),
(5, 'PieChart', 'PieChart', 'pie'),
(6, 'ImageChart', 'ImageChart', null),
(7, 'BarChart', 'BarChart', 'bars'),
(8, 'Histogram', 'Histogram', 'histogram'),
(9, 'BubbleChart', 'BubbleChart', null),
(10, 'ScatterChart', 'ScatterChart', null),
(11, 'SteppedAreaChart', 'SteppedAreaChart', 'steppedArea')
) as Source (GoogleChartTypeID, GoogleChartTypeName, GoogleChartTypeDisplayName, SeriesDataDisplayType)
on Target.GoogleChartTypeID = Source.GoogleChartTypeID
when matched then
    update set
        GoogleChartTypeName = Source.GoogleChartTypeName,
        GoogleChartTypeDisplayName = Source.GoogleChartTypeDisplayName,
        SeriesDataDisplayType = Source.SeriesDataDisplayType
when not matched by target then
    insert (GoogleChartTypeID, GoogleChartTypeName, GoogleChartTypeDisplayName, SeriesDataDisplayType)
    values (GoogleChartTypeID, GoogleChartTypeName, GoogleChartTypeDisplayName, SeriesDataDisplayType)
when not matched by source then
    delete;