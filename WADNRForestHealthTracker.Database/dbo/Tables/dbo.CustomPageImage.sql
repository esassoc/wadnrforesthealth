CREATE TABLE dbo.CustomPageImage
(
    CustomPageImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomPageImage_CustomPageImageID PRIMARY KEY,
    ImageUrl varchar(256) NOT NULL,
    AltText varchar(256) NULL
)
GO