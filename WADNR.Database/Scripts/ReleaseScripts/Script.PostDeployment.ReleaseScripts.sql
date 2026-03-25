/*
Post-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'WADNR.Database - Script.PostDeployment.ReleaseScripts.sql';
GO

:r ".\0001 - Update FieldDefinitionDatum to use the default values from fielddefinition.sql"
GO
:r ".\0002 - Add FirmaPage for Classifications.sql"
GO
:r ".\0003 - Populate IsUser column on Person table.sql"
GO
:r ".\0004 - Update RTE links to new Angular routes.sql"
GO

