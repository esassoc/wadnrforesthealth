CREATE TABLE [dbo].[LoaStage](
    [LoaStageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LoaStage_LoaStageID] PRIMARY KEY,
    [ProjectIdentifier] [varchar](600) NOT NULL,
    [ProjectStatus] [varchar](600) NULL,
    [FundSourceNumber] [varchar](600) NULL,
    [FocusAreaName] [varchar](600) NULL,
    [ProjectExpirationDate] [date] NULL,
    [LetterDate] [date] NULL,
    [MatchAmount] [money] NULL,
    [PayAmount] [money] NULL,
    [ProgramIndex] [varchar](50) NULL,
    [ProjectCode] [varchar](50) NULL,
    [IsNortheast] [bit] NOT NULL,
    [IsSoutheast]  AS (CONVERT([bit],case when [IsNortheast]=(1) then (0) else (1) end)) PERSISTED NOT NULL,
    [ForesterLastName] [varchar](200) NULL,
    [ForesterFirstName] [varchar](200) NULL,
    [ForesterPhone] [varchar](200) NULL,
    [ForesterEmail] [varchar](200) NULL,
    [ApplicationDate] [date] NULL,
    [DecisionDate] [date] NULL
)
GO
CREATE NONCLUSTERED INDEX [IDX_LoaStageGrantNumber] ON [dbo].[LoaStage]([FundSourceNumber]) INCLUDE([FocusAreaName],[IsSoutheast])
GO