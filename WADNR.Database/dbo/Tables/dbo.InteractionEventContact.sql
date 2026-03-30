CREATE TABLE [dbo].[InteractionEventContact](
    [InteractionEventContactID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InteractionEventContact_InteractionEventContactID] PRIMARY KEY,
    [InteractionEventID] [int] NOT NULL CONSTRAINT [FK_InteractionEventContact_InteractionEvent_InteractionEventID] FOREIGN KEY REFERENCES [dbo].[InteractionEvent]([InteractionEventID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_InteractionEventContact_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    CONSTRAINT [AK_InteractionEventContact_InteractionEventID_PersonID] UNIQUE ([InteractionEventID], [PersonID])
)
GO