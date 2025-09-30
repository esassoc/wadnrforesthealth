//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FindYourForesterQuestion
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FindYourForesterQuestionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FindYourForesterQuestion>
    {
        public FindYourForesterQuestionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FindYourForesterQuestionPrimaryKey(FindYourForesterQuestion findYourForesterQuestion) : base(findYourForesterQuestion){}

        public static implicit operator FindYourForesterQuestionPrimaryKey(int primaryKeyValue)
        {
            return new FindYourForesterQuestionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FindYourForesterQuestionPrimaryKey(FindYourForesterQuestion findYourForesterQuestion)
        {
            return new FindYourForesterQuestionPrimaryKey(findYourForesterQuestion);
        }
    }
}