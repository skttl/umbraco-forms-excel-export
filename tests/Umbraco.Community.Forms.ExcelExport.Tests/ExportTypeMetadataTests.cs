using Umbraco.Community.Forms.ExcelExport.Export;
using ExcelExporter = Umbraco.Community.Forms.ExcelExport.Export.ExcelExport;

namespace Umbraco.Community.Forms.ExcelExport.Tests;

public class ExportTypeMetadataTests
{
    [Fact]
    public void ExcelExportExposesExpectedMetadata()
    {
        var exporter = new ExcelExporter(null!, null!, null!, null!, null!);

        Assert.Equal("excel", exporter.Alias);
        Assert.Equal("xlsx", exporter.FileExtension);
        Assert.Equal("icon-download", exporter.Icon);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exporter.MimeType);
    }

    [Fact]
    public void DisplayExportExposesExpectedMetadata()
    {
        var exporter = new ExcelDisplayValuesExport(null!, null!, null!, null!, null!);

        Assert.Equal("excelDisplayValues", exporter.Alias);
        Assert.Equal("xlsx", exporter.FileExtension);
    }
}
