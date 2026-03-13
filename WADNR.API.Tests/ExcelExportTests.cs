using WADNR.Common.ExcelWorkbookUtilities;

namespace WADNR.API.Tests;

[TestClass]
public class ExcelExportTests
{
    private record TestRow(
        DateTimeOffset Timestamp,
        DateTimeOffset? NullableTimestamp,
        DateOnly DateValue,
        DateOnly? NullableDateValue
    );

    [TestMethod]
    public void AddColumn_DateTimeOffset_CreatesColumn()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("Timestamp", x => x.Timestamp);

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("Timestamp", spec.Columnns[0].ColumnName);
    }

    [TestMethod]
    public void AddColumn_NullableDateTimeOffset_CreatesColumn()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("NullableTimestamp", x => x.NullableTimestamp);

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("NullableTimestamp", spec.Columnns[0].ColumnName);
    }

    [TestMethod]
    public void AddColumn_DateOnly_CreatesColumn()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("DateValue", x => x.DateValue);

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("DateValue", spec.Columnns[0].ColumnName);
    }

    [TestMethod]
    public void AddColumn_NullableDateOnly_CreatesColumn()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("NullableDateValue", x => x.NullableDateValue);

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("NullableDateValue", spec.Columnns[0].ColumnName);
    }

    [TestMethod]
    public void AddColumn_DateTimeOffsetWithFormat_AppliesFormat()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("NullableTimestamp", x => x.NullableTimestamp, "MM/dd/yyyy");

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("MM/dd/yyyy", spec.Columnns[0].NumberFormat);
    }

    [TestMethod]
    public void AddColumn_DateOnlyWithFormat_AppliesFormat()
    {
        var spec = new ExcelWorksheetSpec<TestRow>();
        spec.AddColumn("NullableDateValue", x => x.NullableDateValue, "MM/dd/yyyy");

        Assert.AreEqual(1, spec.Columnns.Count);
        Assert.AreEqual("MM/dd/yyyy", spec.Columnns[0].NumberFormat);
    }
}
