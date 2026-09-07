using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers.Extensions;
using Umbraco.Community.Forms.ExcelExport.Export;
using ExcelExporter = Umbraco.Community.Forms.ExcelExport.Export.ExcelExport;
using ExcelDisplayExporter = Umbraco.Community.Forms.ExcelExport.Export.ExcelDisplayValuesExport;

namespace Umbraco.Community.Forms.ExcelExport;

public sealed class ExcelExportComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.FormsExporters()
            .Add<ExcelExporter>()
            .Add<ExcelDisplayExporter>();
    }
}
