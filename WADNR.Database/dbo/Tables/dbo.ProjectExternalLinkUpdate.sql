CREATE TABLE [dbo].[ProjectExternalLinkUpdate](
    [ProjectExternalLinkUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectExternalLinkUpdate_ProjectExternalLinkUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectExternalLinkUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ExternalLinkLabel] [varchar](300) NOT NULL,
    [ExternalLinkUrl] [varchar](300) NOT NULL
)
GO