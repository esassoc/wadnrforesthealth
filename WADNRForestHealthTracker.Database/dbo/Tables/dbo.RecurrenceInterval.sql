CREATE TABLE [dbo].[RecurrenceInterval](
    [RecurrenceIntervalID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RecurrenceInterval_RecurrenceIntervalID] PRIMARY KEY,
    [RecurrenceIntervalInYears] [int] NOT NULL,
    [RecurrenceIntervalDisplayName] [varchar](100) NOT NULL,
    [RecurrenceIntervalName] [varchar](100) NOT NULL,
)
GO
