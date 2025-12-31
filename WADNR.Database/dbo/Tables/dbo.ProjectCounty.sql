CREATE TABLE [dbo].[ProjectCounty](
    [ProjectCountyID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectCounty_ProjectCountyID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectCounty_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [CountyID] [int] NOT NULL CONSTRAINT [FK_ProjectCounty_County_CountyID] FOREIGN KEY REFERENCES [dbo].[County]([CountyID]),
    CONSTRAINT [AK_ProjectCounty_ProjectID_CountyID] UNIQUE ([ProjectID], [CountyID])
)
GO