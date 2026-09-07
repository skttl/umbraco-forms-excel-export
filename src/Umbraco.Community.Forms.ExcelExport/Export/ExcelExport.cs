using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.ExcelExport.Export;

public sealed class ExcelExport : ExcelExportTypeBase
{
    public ExcelExport(
        IFormRecordSearcher recordSearcher,
        IFormService formService,
        IFieldTypeStorage fieldTypeStorage,
        IFieldPreValueSourceService fieldPreValueSourceService,
        IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
        : base(recordSearcher, formService, fieldTypeStorage, fieldPreValueSourceService, fieldPreValueSourceTypeService, false)
    {
        Alias = "excel";
    }
}
