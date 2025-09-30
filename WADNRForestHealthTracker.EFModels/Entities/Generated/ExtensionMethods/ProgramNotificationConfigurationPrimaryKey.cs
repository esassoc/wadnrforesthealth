//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramNotificationConfiguration
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramNotificationConfigurationPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramNotificationConfiguration>
    {
        public ProgramNotificationConfigurationPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramNotificationConfigurationPrimaryKey(ProgramNotificationConfiguration programNotificationConfiguration) : base(programNotificationConfiguration){}

        public static implicit operator ProgramNotificationConfigurationPrimaryKey(int primaryKeyValue)
        {
            return new ProgramNotificationConfigurationPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramNotificationConfigurationPrimaryKey(ProgramNotificationConfiguration programNotificationConfiguration)
        {
            return new ProgramNotificationConfigurationPrimaryKey(programNotificationConfiguration);
        }
    }
}