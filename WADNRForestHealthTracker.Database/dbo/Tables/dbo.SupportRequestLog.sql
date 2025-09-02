CREATE TABLE [dbo].[SupportRequestLog](
    [SupportRequestLogID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SupportRequestLog_SupportRequestLogID] PRIMARY KEY,
    [SupportRequestID] [int] NOT NULL CONSTRAINT [FK_SupportRequestLog_SupportRequest_SupportRequestID] FOREIGN KEY REFERENCES [dbo].[SupportRequest]([SupportRequestID]),
    [SupportRequestLogDate] [datetime] NOT NULL,
    [SupportRequestLogContent] [varchar](max) NULL
)
GO