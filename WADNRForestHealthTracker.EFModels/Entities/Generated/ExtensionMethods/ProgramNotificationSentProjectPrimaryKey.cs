//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramNotificationSentProject
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramNotificationSentProjectPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramNotificationSentProject>
    {
        public ProgramNotificationSentProjectPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramNotificationSentProjectPrimaryKey(ProgramNotificationSentProject programNotificationSentProject) : base(programNotificationSentProject){}

        public static implicit operator ProgramNotificationSentProjectPrimaryKey(int primaryKeyValue)
        {
            return new ProgramNotificationSentProjectPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramNotificationSentProjectPrimaryKey(ProgramNotificationSentProject programNotificationSentProject)
        {
            return new ProgramNotificationSentProjectPrimaryKey(programNotificationSentProject);
        }
    }
}