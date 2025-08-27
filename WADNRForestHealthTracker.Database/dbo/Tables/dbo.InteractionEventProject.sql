SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InteractionEventProject](
	[InteractionEventProjectID] [int] IDENTITY(1,1) NOT NULL,
	[InteractionEventID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_InteractionEventProject_InteractionEventProjectID] PRIMARY KEY CLUSTERED 
(
	[InteractionEventProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_InteractionEventProject_InteractionEventID_ProjectID] UNIQUE NONCLUSTERED 
(
	[InteractionEventID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[InteractionEventProject]  WITH CHECK ADD  CONSTRAINT [FK_InteractionEventProject_InteractionEvent_InteractionEventID] FOREIGN KEY([InteractionEventID])
REFERENCES [dbo].[InteractionEvent] ([InteractionEventID])
GO
ALTER TABLE [dbo].[InteractionEventProject] CHECK CONSTRAINT [FK_InteractionEventProject_InteractionEvent_InteractionEventID]
GO
ALTER TABLE [dbo].[InteractionEventProject]  WITH CHECK ADD  CONSTRAINT [FK_InteractionEventProject_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[InteractionEventProject] CHECK CONSTRAINT [FK_InteractionEventProject_Project_ProjectID]