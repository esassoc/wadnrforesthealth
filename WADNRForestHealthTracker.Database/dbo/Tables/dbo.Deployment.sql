CREATE TABLE dbo.Deployment
(
    DeploymentID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Deployment_DeploymentID PRIMARY KEY,
    Version varchar(15) NOT NULL,
    Date datetime NOT NULL,
    DeployedBy varchar(100) NOT NULL,
    DeployedFrom varchar(100) NOT NULL,
    Source varchar(1000) NOT NULL,
    Script varchar(1000) NOT NULL
)
GO
