SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InteractionEventContact](
	[InteractionEventContactID] [int] IDENTITY(1,1) NOT NULL,
	[InteractionEventID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
 CONSTRAINT [PK_InteractionEventContact_InteractionEventContactID] PRIMARY KEY CLUSTERED 
(
	[InteractionEventContactID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_InteractionEventContact_InteractionEventID_PersonID] UNIQUE NONCLUSTERED 
(
	[InteractionEventID] ASC,
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[InteractionEventContact]  WITH CHECK ADD  CONSTRAINT [FK_InteractionEventContact_InteractionEvent_InteractionEventID] FOREIGN KEY([InteractionEventID])
REFERENCES [dbo].[InteractionEvent] ([InteractionEventID])
GO
ALTER TABLE [dbo].[InteractionEventContact] CHECK CONSTRAINT [FK_InteractionEventContact_InteractionEvent_InteractionEventID]
GO
ALTER TABLE [dbo].[InteractionEventContact]  WITH CHECK ADD  CONSTRAINT [FK_InteractionEventContact_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[InteractionEventContact] CHECK CONSTRAINT [FK_InteractionEventContact_Person_PersonID]