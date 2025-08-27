CREATE TRIGGER dbo.trPreventUpdateOfUnknownOrganization ON dbo.Organization
FOR UPDATE AS
    
    if exists (select d.OrganizationID from deleted as d where d.isEditable = 0)
    begin
         RAISERROR('Attempted to update Organization with IsEditable set to false!', 16, 1)
         rollback tran
         return
    end
GO