CREATE TABLE [dbo].[RecurrenceInterval](
    [RecurrenceIntervalID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RecurrenceInterval_RecurrenceIntervalID] PRIMARY KEY,
    [RecurrenceIntervalName] [varchar](100) NOT NULL CONSTRAINT [AK_RecurrenceInterval_RecurrenceIntervalName] UNIQUE,
    [RecurrenceIntervalDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_RecurrenceInterval_RecurrenceIntervalDisplayName] UNIQUE
)
GO
