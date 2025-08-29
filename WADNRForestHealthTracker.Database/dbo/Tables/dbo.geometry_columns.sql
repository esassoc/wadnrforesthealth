CREATE TABLE [dbo].[geometry_columns](
    [f_table_catalog] [varchar](128) NOT NULL,
    [f_table_schema] [varchar](128) NOT NULL,
    [f_table_name] [varchar](256) NOT NULL,
    [f_geometry_column] [varchar](256) NOT NULL,
    [coord_dimension] [int] NOT NULL,
    [srid] [int] NOT NULL,
    [geometry_type] [varchar](30) NOT NULL,
    CONSTRAINT [PK_geometry_columns_f_table_catalog_f_table_schema_f_table_name_f_geometry_column] PRIMARY KEY ([f_table_catalog], [f_table_schema], [f_table_name], [f_geometry_column])
)
GO
