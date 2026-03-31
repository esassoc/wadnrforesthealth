CREATE TABLE [dbo].[ProjectLocationSimpleType](
    [ProjectLocationSimpleTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectLocationSimpleType_ProjectLocationSimpleTypeID] PRIMARY KEY,
    [ProjectLocationSimpleTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectLocationSimpleType_ProjectLocationSimpleTypeName] UNIQUE,
    [DisplayInstructions] [varchar](100) NOT NULL,
    [DisplayOrder] [int] NOT NULL
)
GO
