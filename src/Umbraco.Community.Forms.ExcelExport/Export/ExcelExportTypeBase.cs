using System.Globalization;
using SpreadCheetah;
using SpreadCheetah.Styling;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.ExcelExport.Export;

public abstract class ExcelExportTypeBase : ExportType
{
    private const long MaximumDataRows = 1_048_575;
    private const int MaximumCellLength = 32_767;

    private readonly IFormRecordSearcher _recordSearcher;
    private readonly IFormService _formService;
    private readonly IFieldTypeStorage _fieldTypeStorage;
    private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
    private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
    private readonly bool _displayValues;

    protected ExcelExportTypeBase(
        IFormRecordSearcher recordSearcher,
        IFormService formService,
        IFieldTypeStorage fieldTypeStorage,
        IFieldPreValueSourceService fieldPreValueSourceService,
        IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
        bool displayValues)
    {
        _recordSearcher = recordSearcher;
        _formService = formService;
        _fieldTypeStorage = fieldTypeStorage;
        _fieldPreValueSourceService = fieldPreValueSourceService;
        _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
        _displayValues = displayValues;
        FileExtension = "xlsx";
        MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Icon = "icon-download";
    }

    public override Task<string> ExportRecordsAsync(Guid formId, RecordExportFilter filter) =>
        throw new NotSupportedException("XLSX exports must be generated through the file export endpoint.");

    protected override async Task ExportToSteamAsync(Guid formId, RecordExportFilter filter, Stream stream)
    {
        var form = _formService.Get(formId) ?? throw new InvalidOperationException("The requested form could not be found.");
        var results = _recordSearcher.QueryDataBase(formId, filter);
        var schema = results.Schema.ToArray();
        var fields = form.AllFields.ToDictionary(field => field.Id);
        var prevalueMaps = await BuildPrevalueMapsAsync(form);

        await using var spreadsheet = await Spreadsheet.CreateNewAsync(stream, new SpreadCheetahOptions
        {
            DefaultDateTimeFormat = NumberFormat.Custom("yyyy-mm-dd hh:mm:ss"),
        });
        await spreadsheet.StartWorksheetAsync("Entries");

        var headers = schema.Select(item => ExcelCellFactory.SanitizeText(item.Name ?? string.Empty, "header")).ToArray();
        ValidateColumnCount(headers.Length);
        await spreadsheet.AddHeaderRowAsync(headers, default);

        long rowNumber = 0;
        foreach (var result in results.Results)
        {
            rowNumber++;
            if (rowNumber > MaximumDataRows)
            {
                throw new InvalidOperationException("The export contains more rows than Excel supports.");
            }

            var values = result.Fields.ToArray();
            var cells = new Cell[schema.Length];
            for (var index = 0; index < schema.Length; index++)
            {
                var schemaItem = schema[index];
                var fieldValue = index < values.Length ? values[index].Value : null;
                var field = TryGetField(schemaItem, fields);
                cells[index] = CreateCell(fieldValue, field, schemaItem, prevalueMaps);
            }

            await spreadsheet.AddRowAsync(cells);
        }

        await spreadsheet.FinishAsync();
    }

    private async Task<Dictionary<Guid, Dictionary<string, string>>> BuildPrevalueMapsAsync(Form form)
    {
        var maps = new Dictionary<Guid, Dictionary<string, string>>();
        if (!_displayValues)
        {
            return maps;
        }

        foreach (var field in form.AllFields)
        {
            Dictionary<string, string> valueMap = field.PreValues
                .ToDictionary(value => value.Value, value => value.Caption ?? string.Empty);
            if (field.PreValueSourceId != Guid.Empty)
            {
                var source = _fieldPreValueSourceService.GetById(field.PreValueSourceId);
                var sourceType = source is null ? null : _fieldPreValueSourceTypeService.GetById(source.FieldPreValueSourceTypeId);
                if (source is not null && sourceType is not null)
                {
                    sourceType.LoadSettings(source);
                    var values = await sourceType.GetPreValuesAsync(field, form);
                    valueMap = values.ToDictionary(value => value.Value, value => value.Caption ?? string.Empty);
                }
            }

            if (valueMap.Count > 0)
            {
                maps[field.Id] = valueMap;
            }
        }

        return maps;
    }

    private Cell CreateCell(
        object? value,
        Field? field,
        EntrySearchResultSchema schema,
        IReadOnlyDictionary<Guid, Dictionary<string, string>> prevalueMaps)
    {
        if (_displayValues || value is System.Collections.IEnumerable and not string)
        {
            return ExcelCellFactory.CreateTextCell(ToText(value, field, schema, prevalueMaps), schema.Name);
        }

        var fieldType = field is null ? null : _fieldTypeStorage.GetFieldTypeByField(field);
        var dataType = fieldType?.DataType;

        if (dataType == FieldDataType.DateTime && value is DateTime dateTime)
        {
            return ExcelCellFactory.CreateDateCell(dateTime);
        }

        if (dataType == FieldDataType.Bit && value is bool boolean)
        {
            return ExcelCellFactory.CreateBooleanCell(boolean);
        }

        if (dataType == FieldDataType.Integer && ExcelCellFactory.IsSafeInteger(value, out var integer))
        {
            return ExcelCellFactory.CreateIntegerCell(integer);
        }

        return ExcelCellFactory.CreateTextCell(ToText(value, field, schema, prevalueMaps), schema.Name);
    }

    private string ToText(
        object? value,
        Field? field,
        EntrySearchResultSchema schema,
        IReadOnlyDictionary<Guid, Dictionary<string, string>> prevalueMaps)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var values = value is string ? new[] { value } : value is System.Collections.IEnumerable enumerable
            ? enumerable.Cast<object?>().ToArray()
            : new[] { value };

        var textValues = values.Select(item =>
        {
            var text = Convert.ToString(item, CultureInfo.InvariantCulture) ?? string.Empty;
            if (_displayValues && field is not null && prevalueMaps.TryGetValue(field.Id, out var map) && map.TryGetValue(text, out var caption))
            {
                return caption;
            }

            return text;
        });

        return string.Join(", ", textValues);
    }

    private static Field? TryGetField(EntrySearchResultSchema schema, IReadOnlyDictionary<Guid, Field> fields)
    {
        return Guid.TryParse(schema.Id, out var fieldId) && fields.TryGetValue(fieldId, out var field) ? field : null;
    }

    private static void ValidateColumnCount(int count)
    {
        if (count > 16_384)
        {
            throw new InvalidOperationException("The export contains more columns than Excel supports.");
        }
    }

}
