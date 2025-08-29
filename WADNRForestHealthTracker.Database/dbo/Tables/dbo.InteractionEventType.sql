CREATE TABLE [dbo].[InteractionEventType](
    [InteractionEventTypeID] [int] NOT NULL CONSTRAINT [PK_InteractionEventType_InteractionEventTypeID] PRIMARY KEY,
    [InteractionEventTypeName] [varchar](200) NOT NULL,
    [InteractionEventTypeDisplayName] [varchar](255) NOT NULL
)
GO
