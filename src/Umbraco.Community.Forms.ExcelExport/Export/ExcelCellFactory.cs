using System.Globalization;
using System.Text;
using SpreadCheetah;

namespace Umbraco.Community.Forms.ExcelExport.Export;

internal static class ExcelCellFactory
{
    private const int MaximumCellLength = 32_767;

    public static Cell CreateTextCell(string value, string? column)
    {
        return new Cell(SanitizeText(value, column));
    }

    public static string SanitizeText(string value, string? column)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character is '\t' or '\n' or '\r' || character >= ' ')
            {
                builder.Append(character);
            }
        }

        var sanitized = builder.ToString();
        if (sanitized.Length > MaximumCellLength)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var message = language.Equals("da", StringComparison.OrdinalIgnoreCase)
                ? $"Værdien for '{column}' overskrider Excels maksimale cellelængde."
                : $"The value for '{column}' exceeds Excel's maximum cell length.";
            throw new InvalidOperationException(message);
        }

        return sanitized;
    }

    public static Cell CreateDateCell(DateTime value) => new(value);

    public static Cell CreateBooleanCell(bool value) => new(value);

    public static Cell CreateIntegerCell(long value) => new(value);

    public static bool IsSafeInteger(object? value, out long integer)
    {
        integer = 0;
        if (value is not sbyte and not byte and not short and not ushort and not int and not uint and not long and not ulong)
        {
            return false;
        }

        try
        {
            integer = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }
        catch (OverflowException)
        {
            return false;
        }

        return integer.ToString(CultureInfo.InvariantCulture).TrimStart('-').Length <= 15;
    }
}
