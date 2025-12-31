CREATE TABLE [dbo].[InteractionEvent](
    [InteractionEventID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InteractionEvent_InteractionEventID] PRIMARY KEY,
    [InteractionEventTypeID] [int] NOT NULL CONSTRAINT [FK_InteractionEvent_InteractionEventType_InteractionEventTypeID] FOREIGN KEY REFERENCES [dbo].[InteractionEventType]([InteractionEventTypeID]),
    [StaffPersonID] [int] NULL CONSTRAINT [FK_InteractionEvent_Person_StaffPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [InteractionEventTitle] [varchar](255) NOT NULL,
    [InteractionEventDescription] [varchar](3000) NULL,
    [InteractionEventDate] [datetime] NOT NULL,
    [InteractionEventLocationSimple] [geometry] NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_InteractionEvent_InteractionEventLocationSimple] ON [dbo].[InteractionEvent]([InteractionEventLocationSimple]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-123, 47, -122, 48), CELLS_PER_OBJECT = 8)
GO