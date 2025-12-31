CREATE TABLE [dbo].[InteractionEventFileResource](
	[InteractionEventFileResourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InteractionEventFileResource_InteractionEventFileResourceID] PRIMARY KEY,
	[InteractionEventID] [int] NOT NULL CONSTRAINT [FK_InteractionEventFileResource_InteractionEvent_InteractionEventID] FOREIGN KEY REFERENCES [dbo].[InteractionEvent]([InteractionEventID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT [FK_InteractionEventFileResource_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]) ON DELETE CASCADE,
	[DisplayName] [varchar](200) NOT NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [AK_InteractionEventFileResource_FileResourceID] UNIQUE ([FileResourceID])
) 
GO