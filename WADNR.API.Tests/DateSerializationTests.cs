using System.Text.Json;
using WADNR.API.Tests.Helpers;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Invoice;

namespace WADNR.API.Tests;

[TestClass]
public class DateSerializationTests
{
    private JsonSerializerOptions _options = null!;

    [TestInitialize]
    public void Setup()
    {
        _options = TestJsonSerializerOptions.Create();
    }

    [TestMethod]
    public void DateOnly_Serializes_AsIsoDate()
    {
        var date = new DateOnly(2026, 3, 15);
        var json = JsonSerializer.Serialize(date, _options);
        Assert.AreEqual("\"2026-03-15\"", json);
    }

    [TestMethod]
    public void DateOnly_Deserializes_FromIsoDate()
    {
        var result = JsonSerializer.Deserialize<DateOnly>("\"2026-03-15\"", _options);
        Assert.AreEqual(new DateOnly(2026, 3, 15), result);
    }

    [TestMethod]
    public void NullableDateOnly_Serializes_AsNull()
    {
        DateOnly? date = null;
        var json = JsonSerializer.Serialize(date, _options);
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void NullableDateOnly_Serializes_AsIsoDate()
    {
        DateOnly? date = new DateOnly(2026, 3, 15);
        var json = JsonSerializer.Serialize(date, _options);
        Assert.AreEqual("\"2026-03-15\"", json);
    }

    [TestMethod]
    public void DateTimeOffset_Serializes_WithTimezone()
    {
        var dto = new DateTimeOffset(2026, 3, 15, 10, 30, 0, TimeSpan.Zero);
        var json = JsonSerializer.Serialize(dto, _options);
        Assert.AreEqual("\"2026-03-15T10:30:00+00:00\"", json);
    }

    [TestMethod]
    public void DateTimeOffset_Deserializes_FromIso()
    {
        var result = JsonSerializer.Deserialize<DateTimeOffset>("\"2026-03-15T10:30:00+00:00\"", _options);
        var expected = new DateTimeOffset(2026, 3, 15, 10, 30, 0, TimeSpan.Zero);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void NullableDateTimeOffset_Serializes_AsNull()
    {
        DateTimeOffset? dto = null;
        var json = JsonSerializer.Serialize(dto, _options);
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void InvoiceGridRow_RoundTrip()
    {
        var row = new InvoiceGridRow
        {
            InvoiceID = 1,
            InvoicePaymentRequestID = 10,
            ProjectID = 100,
            ProjectName = "Test Project",
            InvoiceNumber = "INV-001",
            InvoiceDate = new DateOnly(2025, 6, 15),
            InvoiceStatusID = 1,
            InvoiceStatusDisplayName = "Submitted",
            InvoiceApprovalStatusID = 1,
            InvoiceApprovalStatusName = "Pending",
        };

        var json = JsonSerializer.Serialize(row, _options);
        var deserialized = JsonSerializer.Deserialize<InvoiceGridRow>(json, _options)!;

        Assert.AreEqual(new DateOnly(2025, 6, 15), deserialized.InvoiceDate);
        // Verify the JSON contains a date-only string (no time component)
        Assert.IsTrue(json.Contains("\"2025-06-15\""), $"Expected date-only format in JSON but got: {json}");
    }

    [TestMethod]
    public void ProjectNoteGridRow_RoundTrip()
    {
        var row = new ProjectNoteGridRow
        {
            ProjectNoteID = 1,
            Note = "Test note",
            CreatedByPersonName = "Tester",
            CreateDate = new DateTimeOffset(2026, 3, 15, 14, 30, 0, TimeSpan.FromHours(-7)),
            UpdateDate = new DateTimeOffset(2026, 3, 16, 9, 0, 0, TimeSpan.FromHours(-7)),
        };

        var json = JsonSerializer.Serialize(row, _options);
        var deserialized = JsonSerializer.Deserialize<ProjectNoteGridRow>(json, _options)!;

        Assert.AreEqual(row.CreateDate, deserialized.CreateDate);
        Assert.AreEqual(row.UpdateDate, deserialized.UpdateDate);
    }

    [TestMethod]
    public void PersonDetail_RoundTrip()
    {
        var detail = new PersonDetail
        {
            PersonID = 1,
            FirstName = "Test",
            CreateDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdateDate = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero),
            LastActivityDate = new DateTimeOffset(2025, 6, 20, 8, 30, 0, TimeSpan.Zero),
        };

        var json = JsonSerializer.Serialize(detail, _options);
        var deserialized = JsonSerializer.Deserialize<PersonDetail>(json, _options)!;

        Assert.AreEqual(detail.CreateDate, deserialized.CreateDate);
        Assert.AreEqual(detail.UpdateDate, deserialized.UpdateDate);
        Assert.AreEqual(detail.LastActivityDate, deserialized.LastActivityDate);
    }

    [TestMethod]
    public void AgreementDetail_RoundTrip()
    {
        var detail = new AgreementDetail
        {
            AgreementID = 1,
            AgreementTitle = "Test Agreement",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 12, 31),
        };

        var json = JsonSerializer.Serialize(detail, _options);
        var deserialized = JsonSerializer.Deserialize<AgreementDetail>(json, _options)!;

        Assert.AreEqual(new DateOnly(2025, 1, 1), deserialized.StartDate);
        Assert.AreEqual(new DateOnly(2025, 12, 31), deserialized.EndDate);
    }

    [TestMethod]
    public void No_DateTimeConverter_Registered()
    {
        // Verify that the old DateTimeConverter is not in the converter list
        foreach (var converter in _options.Converters)
        {
            Assert.IsFalse(
                converter.GetType().Name == "DateTimeConverter",
                "DateTimeConverter should not be registered — it was removed as part of the date migration.");
        }
    }
}
