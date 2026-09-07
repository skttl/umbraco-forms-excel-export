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
        Alias = "excelDisplayValues";
    }
}
