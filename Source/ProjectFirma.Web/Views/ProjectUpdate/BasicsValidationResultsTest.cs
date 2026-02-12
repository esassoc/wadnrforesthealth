/*-----------------------------------------------------------------------
<copyright file="BasicsValidationResultsTest.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

using System;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.UnitTestCommon;
using NUnit.Framework;

namespace ProjectFirma.Web.Views.ProjectUpdate
{
    [TestFixture]
    public class BasicsValidationResultsTest
    {
        [Test]
        [Ignore("No more validation is completed on Project Updates so this test is no longer valid")]
        public void ProjectUpdateYearRangesTest()
        {
            var projectUpdate = TestFramework.TestProjectUpdate.Create();

            projectUpdate.ProjectStageID = ProjectStage.Completed.ProjectStageID;
            var warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();

            Assert.That(warningMessages.Contains(BasicsValidationResult.ImplementationStartYearIsRequired));
            Assert.That(warningMessages.Contains(BasicsValidationResult.CompletionDateIsRequired));
                                    
            projectUpdate.CompletionDate = new DateTime(2007, 1, 1);
            warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();

            Assert.That(warningMessages.Contains(BasicsValidationResult.ImplementationStartYearIsRequired));
            Assert.That(!warningMessages.Contains(BasicsValidationResult.CompletionDateIsRequired));

            projectUpdate.PlannedDate = new DateTime(2010, 1, 1);
            warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();

            Assert.That(!warningMessages.Contains(BasicsValidationResult.ImplementationStartYearIsRequired));
            Assert.That(!warningMessages.Contains(BasicsValidationResult.CompletionDateIsRequired));
            Assert.That(warningMessages.Contains(FirmaValidationMessages.CompletionDateGreaterThanEqualToImplementationStartYear));

            projectUpdate.PlannedDate = new DateTime(2006, 1, 1);
            warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();            

            Assert.That(!warningMessages.Contains(FirmaValidationMessages.CompletionDateGreaterThanEqualToImplementationStartYear));

            projectUpdate.ProjectStageID = ProjectStage.Planned.ProjectStageID;
            projectUpdate.PlannedDate = new DateTime(2035,1,1);
            warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();

            Assert.That(warningMessages.Contains(BasicsValidationResult.PlannedDateShouldBeLessThanCurrentYear));


            projectUpdate.ProjectStageID = ProjectStage.Implementation.ProjectStageID;
            projectUpdate.PlannedDate = new DateTime(2035, 1, 1);
            warningMessages = new BasicsValidationResult(projectUpdate).GetWarningMessages();

            Assert.That(warningMessages.Contains(BasicsValidationResult.ImplementationStartYearShouldBeLessThanCurrentYear));

            
        }
    }
}
