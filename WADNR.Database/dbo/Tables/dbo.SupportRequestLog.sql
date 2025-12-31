CREATE TABLE [dbo].[SupportRequestLog](
    [SupportRequestLogID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupportRequestLog_SupportRequestLogID PRIMARY KEY,
    [RequestDate] [datetime] NOT NULL,
    [RequestPersonName] [varchar](200) NOT NULL,
    [RequestPersonEmail] [varchar](256) NOT NULL,
    [RequestPersonID] [int] NULL CONSTRAINT FK_SupportRequestLog_Person_RequestPersonID FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [SupportRequestTypeID] [int] NOT NULL CONSTRAINT FK_SupportRequestLog_SupportRequestType_SupportRequestTypeID FOREIGN KEY REFERENCES [dbo].[SupportRequestType]([SupportRequestTypeID]),
    [RequestDescription] [varchar](2000) NOT NULL,
    [RequestPersonOrganization] [varchar](500) NULL,
    [RequestPersonPhone] [varchar](50) NULL
)
GO