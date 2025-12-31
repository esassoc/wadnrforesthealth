merge into dbo.FirmaPageRenderType as Target
using (values
(1, 'IntroductoryText', 'Introductory Text'),
(2, 'PageContent', 'Page Content')
) as Source (FirmaPageRenderTypeID, FirmaPageRenderTypeName, FirmaPageRenderTypeDisplayName)
on Target.FirmaPageRenderTypeID = Source.FirmaPageRenderTypeID
when matched then
    update set
        FirmaPageRenderTypeName = Source.FirmaPageRenderTypeName,
        FirmaPageRenderTypeDisplayName = Source.FirmaPageRenderTypeDisplayName
when not matched by target then
    insert (FirmaPageRenderTypeID, FirmaPageRenderTypeName, FirmaPageRenderTypeDisplayName)
    values (FirmaPageRenderTypeID, FirmaPageRenderTypeName, FirmaPageRenderTypeDisplayName)
when not matched by source then
    delete;