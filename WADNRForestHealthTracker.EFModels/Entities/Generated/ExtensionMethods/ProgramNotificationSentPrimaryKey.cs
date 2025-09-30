//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramNotificationSent
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramNotificationSentPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramNotificationSent>
    {
        public ProgramNotificationSentPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramNotificationSentPrimaryKey(ProgramNotificationSent programNotificationSent) : base(programNotificationSent){}

        public static implicit operator ProgramNotificationSentPrimaryKey(int primaryKeyValue)
        {
            return new ProgramNotificationSentPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramNotificationSentPrimaryKey(ProgramNotificationSent programNotificationSent)
        {
            return new ProgramNotificationSentPrimaryKey(programNotificationSent);
        }
    }
}