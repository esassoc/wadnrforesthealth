using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.API.Tests;

[TestClass]
public class ProjectionDateTests
{
    [TestMethod]
    public void DateTimeOffset_To_EntityDateTime_ViaDateTimeProperty()
    {
        // Simulates what happens when a DTO DateTimeOffset? is written back to an EF entity DateTime?
        DateTimeOffset? dtoValue = new DateTimeOffset(2026, 3, 15, 10, 30, 0, TimeSpan.FromHours(-7));
        DateTime? entityValue = dtoValue?.DateTime;

        Assert.IsNotNull(entityValue);
        Assert.AreEqual(2026, entityValue.Value.Year);
        Assert.AreEqual(3, entityValue.Value.Month);
        Assert.AreEqual(15, entityValue.Value.Day);
        Assert.AreEqual(10, entityValue.Value.Hour);
        Assert.AreEqual(30, entityValue.Value.Minute);
    }

    [TestMethod]
    public void NullDateTimeOffset_To_NullEntityDateTime()
    {
        DateTimeOffset? dtoValue = null;
        DateTime? entityValue = dtoValue?.DateTime;

        Assert.IsNull(entityValue);
    }

    [TestMethod]
    public void InvoiceGridRow_DateOnly_PreservedInDto()
    {
        // Simulates what happens when EF entity DateOnly is projected to DTO DateOnly
        var entityDate = new DateOnly(2025, 6, 15);
        var dto = new InvoiceGridRow { InvoiceDate = entityDate };

        Assert.AreEqual(new DateOnly(2025, 6, 15), dto.InvoiceDate);
        // No timezone shift possible with DateOnly
    }

    [TestMethod]
    public void PersonDetail_DateTime_To_DateTimeOffset_ImplicitConversion()
    {
        // Entity has DateTime, DTO has DateTimeOffset — implicit conversion in EF projection
        DateTime entityCreateDate = new DateTime(2025, 1, 15, 8, 30, 0, DateTimeKind.Unspecified);
        DateTimeOffset dtoCreateDate = entityCreateDate; // implicit conversion

        Assert.AreEqual(2025, dtoCreateDate.Year);
        Assert.AreEqual(1, dtoCreateDate.Month);
        Assert.AreEqual(15, dtoCreateDate.Day);
        Assert.AreEqual(8, dtoCreateDate.Hour);
        Assert.AreEqual(30, dtoCreateDate.Minute);
    }

    [TestMethod]
    public void AgreementDetail_DateOnly_PreservedInDto()
    {
        var dto = new AgreementDetail
        {
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 12, 31),
        };

        Assert.AreEqual(new DateOnly(2025, 1, 1), dto.StartDate);
        Assert.AreEqual(new DateOnly(2025, 12, 31), dto.EndDate);
    }

    [TestMethod]
    public void TreatmentGridRow_DateOnly_PreservedInDto()
    {
        var dto = new TreatmentGridRow
        {
            TreatmentStartDate = new DateOnly(2025, 4, 1),
            TreatmentEndDate = new DateOnly(2025, 9, 30),
        };

        Assert.AreEqual(new DateOnly(2025, 4, 1), dto.TreatmentStartDate);
        Assert.AreEqual(new DateOnly(2025, 9, 30), dto.TreatmentEndDate);
    }
}
