//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramNotificationType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramNotificationTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramNotificationType>
    {
        public ProgramNotificationTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramNotificationTypePrimaryKey(ProgramNotificationType programNotificationType) : base(programNotificationType){}

        public static implicit operator ProgramNotificationTypePrimaryKey(int primaryKeyValue)
        {
            return new ProgramNotificationTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramNotificationTypePrimaryKey(ProgramNotificationType programNotificationType)
        {
            return new ProgramNotificationTypePrimaryKey(programNotificationType);
        }
    }
}