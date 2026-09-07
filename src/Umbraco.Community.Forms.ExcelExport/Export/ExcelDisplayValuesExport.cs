using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.ExcelExport.Export;

public sealed class ExcelDisplayValuesExport : ExcelExportTypeBase
{
    public ExcelDisplayValuesExport(
        IFormRecordSearcher recordSearcher,
        IFormService formService,
        IFieldTypeStorage fieldTypeStorage,
        IFieldPreValueSourceService fieldPreValueSourceService,
        IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
        : base(recordSearcher, formService, fieldTypeStorage, fieldPreValueSourceService, fieldPreValueSourceTypeService, true)
    {
        Id = Guid.Parse("7b8e4f3a-2c61-4e8d-a5f7-1c9b6d2e8a02");
        Alias = "excelDisplayValues";
    }
}
