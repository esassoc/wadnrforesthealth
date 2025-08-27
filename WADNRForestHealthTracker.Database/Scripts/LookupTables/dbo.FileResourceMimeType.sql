merge into dbo.FileResourceMimeType as Target
using (values
(1, 'application/pdf', 'PDF', 'PDF', '/Content/img/MimeTypeIcons/pdf_20x20.png', '/Content/img/MimeTypeIcons/pdf_48x48.png'),
(2, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'Word (DOCX)', 'Word (DOCX)', '/Content/img/MimeTypeIcons/word_20x20.png', '/Content/img/MimeTypeIcons/word_48x48.png'),
(3, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'Excel (XLSX)', 'Excel (XLSX)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),
(4, 'image/x-png', 'X-PNG', 'X-PNG', null, null),
(5, 'image/png', 'PNG', 'PNG', null, null),
(6, 'image/tiff', 'TIFF', 'TIFF', null, null),
(7, 'image/bmp', 'BMP', 'BMP', null, null),
(8, 'image/gif', 'GIF', 'GIF', null, null),
(9, 'image/jpeg', 'JPEG', 'JPEG', null, null),
(10, 'image/pjpeg', 'PJPEG', 'PJPEG', null, null),
(11, 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'Powerpoint (PPTX)', 'Powerpoint (PPTX)', '/Content/img/MimeTypeIcons/powerpoint_20x20.png', '/Content/img/MimeTypeIcons/powerpoint_48x48.png'),
(12, 'application/vnd.ms-powerpoint', 'Powerpoint (PPT)', 'Powerpoint (PPT)', '/Content/img/MimeTypeIcons/powerpoint_20x20.png', '/Content/img/MimeTypeIcons/powerpoint_48x48.png'),
(13, 'application/vnd.ms-excel', 'Excel (XLS)', 'Excel (XLS)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),
(14, 'application/msword', 'Word (DOC)', 'Word (DOC)', '/Content/img/MimeTypeIcons/word_20x20.png', '/Content/img/MimeTypeIcons/word_48x48.png'),
(15, 'application/x-excel', 'x-Excel (XLSX)', 'x-Excel (XLSX)', '/Content/img/MimeTypeIcons/excel_20x20.png', '/Content/img/MimeTypeIcons/excel_48x48.png'),
(16, 'text/css', 'CSS', 'CSS', null, null),
(17, 'application/x-zip-compressed', 'X-ZIP', 'X-ZIP', null, null),
(18, 'application/gzip', 'GZIP', 'GZIP', null, null),
(19, 'application/x-gzip', 'X-GZIP', 'X-GZIP', null, null),
(20, 'application/x-compressed', 'TGZ', 'TGZ', null, null),
(21, 'application/x-tar', 'TAR', 'TAR', null, null),
(22, 'application/zip', 'ZIP', 'ZIP', null, null),
(23, 'text/plain', 'TXT', 'TXT', null, null)
) as Source (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
on Target.FileResourceMimeTypeID = Source.FileResourceMimeTypeID
when matched then
    update set
        FileResourceMimeTypeContentTypeName = Source.FileResourceMimeTypeContentTypeName,
        FileResourceMimeTypeName = Source.FileResourceMimeTypeName,
        FileResourceMimeTypeDisplayName = Source.FileResourceMimeTypeDisplayName,
        FileResourceMimeTypeIconSmallFilename = Source.FileResourceMimeTypeIconSmallFilename,
        FileResourceMimeTypeIconNormalFilename = Source.FileResourceMimeTypeIconNormalFilename
when not matched by target then
    insert (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
    values (FileResourceMimeTypeID, FileResourceMimeTypeContentTypeName, FileResourceMimeTypeName, FileResourceMimeTypeDisplayName, FileResourceMimeTypeIconSmallFilename, FileResourceMimeTypeIconNormalFilename)
when not matched by source then
    delete;