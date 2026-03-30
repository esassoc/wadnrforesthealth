CREATE TABLE [dbo].[InteractionEventProject](
    [InteractionEventProjectID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InteractionEventProject_InteractionEventProjectID] PRIMARY KEY,
    [InteractionEventID] [int] NOT NULL CONSTRAINT [FK_InteractionEventProject_InteractionEvent_InteractionEventID] FOREIGN KEY REFERENCES [dbo].[InteractionEvent]([InteractionEventID]),
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_InteractionEventProject_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    CONSTRAINT [AK_InteractionEventProject_InteractionEventID_ProjectID] UNIQUE ([InteractionEventID], [ProjectID])
)
GO