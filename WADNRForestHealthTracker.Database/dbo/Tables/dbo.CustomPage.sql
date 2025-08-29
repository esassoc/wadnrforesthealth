CREATE TABLE dbo.CustomPage
(
    CustomPageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomPage_CustomPageID PRIMARY KEY,
    CustomPageDisplayName varchar(100) NOT NULL,
    CustomPageVanityUrl varchar(100) NOT NULL,
    CustomPageDisplayTypeID int NOT NULL,
    CustomPageContent dbo.html NULL,
    CustomPageNavigationSectionID int NOT NULL
)
GO