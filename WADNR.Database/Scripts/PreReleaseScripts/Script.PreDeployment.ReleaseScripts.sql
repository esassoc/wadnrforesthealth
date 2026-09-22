/*
Pre-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'WADNR.Database - Script.PreDeployment.ReleaseScripts.sql';
GO

:r ".\0001 - WADNR-2290 Remove FocusArea lookup dependents.sql"
GO

