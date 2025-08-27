CREATE TABLE [dbo].[ActivityType](
	[ActivityTypeID] [int] NOT NULL,
	[ActivityTypeName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ActivityTypeDisplayName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ActivityType_ActivityTypeID] PRIMARY KEY CLUSTERED 
(
	[ActivityTypeID] ASC
),
 CONSTRAINT [AK_ActivityType_ActivityTypeDisplayName] UNIQUE NONCLUSTERED 
(
	[ActivityTypeDisplayName] ASC
),
 CONSTRAINT [AK_ActivityType_ActivityTypeName] UNIQUE NONCLUSTERED 
(
	[ActivityTypeName] ASC
)
)
