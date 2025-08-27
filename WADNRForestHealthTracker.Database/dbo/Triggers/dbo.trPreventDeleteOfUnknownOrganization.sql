CREATE TRIGGER dbo.trPreventDeleteOfUnknownOrganization ON dbo.Organization
FOR DELETE AS
    
    if exists (select d.OrganizationID from deleted as d where d.isEditable = 0)
    begin
         RAISERROR('Attempted to delete Organization with IsEditable set to false!', 16, 1)
         rollback tran
         return
    end
GO