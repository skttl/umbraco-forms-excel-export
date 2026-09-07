using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadCheetah;
using Umbraco.Community.Forms.ExcelExport.Export;
using OpenXmlCell = DocumentFormat.OpenXml.Spreadsheet.Cell;

namespace Umbraco.Community.Forms.ExcelExport.Tests;

public class ExcelCellFactoryTests
{
    [Fact]
    public void SanitizeTextRemovesXmlControlCharacters()
    {
        var result = ExcelCellFactory.SanitizeText("before\0\u000Bafter\n", "field");

        Assert.Equal("beforeafter\n", result);
    }

    [Theory]
    [InlineData(123456789012345L, true)]
    [InlineData(1234567890123456L, false)]
    public void IsSafeIntegerHonoursExcelPrecision(long value, bool expected)
    {
        Assert.Equal(expected, ExcelCellFactory.IsSafeInteger(value, out _));
    }

    [Fact]
    public async Task TextBeginningWithFormulaMarkerIsNotFormula()
    {
        await using var output = new MemoryStream();
        await using (var spreadsheet = await Spreadsheet.CreateNewAsync(output, new SpreadCheetahOptions()))
        {
            await spreadsheet.StartWorksheetAsync("Entries");
            await spreadsheet.AddRowAsync(new[] { ExcelCellFactory.CreateTextCell("=SUM(A1)", "field") });
            await spreadsheet.FinishAsync();
        }

        output.Position = 0;
        using var document = SpreadsheetDocument.Open(output, false);
        var cell = document.WorkbookPart!.WorksheetParts.Single().Worksheet.Descendants<OpenXmlCell>().Single();

        Assert.Null(cell.CellFormula);
        Assert.NotNull(cell.InlineString);
    }

    [Fact]
    public async Task TypedCellsRemainTypedInWorkbook()
    {
        await using var output = new MemoryStream();
        await using (var spreadsheet = await Spreadsheet.CreateNewAsync(output, new SpreadCheetahOptions()))
        {
            await spreadsheet.StartWorksheetAsync("Entries");
            await spreadsheet.AddRowAsync(new[]
            {
                ExcelCellFactory.CreateIntegerCell(42),
                ExcelCellFactory.CreateBooleanCell(true),
                ExcelCellFactory.CreateDateCell(new DateTime(2026, 9, 4, 12, 30, 0, DateTimeKind.Unspecified)),
            });
            await spreadsheet.FinishAsync();
        }

        output.Position = 0;
        using var document = SpreadsheetDocument.Open(output, false);
        var cells = document.WorkbookPart!.WorksheetParts.Single().Worksheet.Descendants<OpenXmlCell>().ToArray();

        Assert.Equal("42", cells[0].CellValue!.Text);
        Assert.Equal(CellValues.Boolean, cells[1].DataType!.Value);
        Assert.NotNull(cells[2].CellValue);
    }
}
