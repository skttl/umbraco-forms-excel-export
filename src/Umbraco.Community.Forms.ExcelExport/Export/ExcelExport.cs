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
        Id = Guid.Parse("3f9c9d6e-5b98-4f1f-9d3a-6d0b5c0d6f01");
        Alias = "excel";
    }
}
