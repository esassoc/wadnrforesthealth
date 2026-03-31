CREATE TABLE [dbo].[FundSourceAllocationFileResource](
    [FundSourceAllocationFileResourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationFileResource_FundSourceAllocationFileResourceID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationFileResource_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationFileResource_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [DisplayName] [varchar](200) NOT NULL,
    [Description] [varchar](1000) NULL,
    CONSTRAINT [AK_FundSourceAllocationFileResource_FundSourceAllocationID_FileResourceID] UNIQUE ([FundSourceAllocationID], [FileResourceID])
)
GO