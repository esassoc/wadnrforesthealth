//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.RecurrenceInterval
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class RecurrenceIntervalPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<RecurrenceInterval>
    {
        public RecurrenceIntervalPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public RecurrenceIntervalPrimaryKey(RecurrenceInterval recurrenceInterval) : base(recurrenceInterval){}

        public static implicit operator RecurrenceIntervalPrimaryKey(int primaryKeyValue)
        {
            return new RecurrenceIntervalPrimaryKey(primaryKeyValue);
        }

        public static implicit operator RecurrenceIntervalPrimaryKey(RecurrenceInterval recurrenceInterval)
        {
            return new RecurrenceIntervalPrimaryKey(recurrenceInterval);
        }
    }
}