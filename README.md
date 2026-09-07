# Umbraco Forms Excel Export

![Umbraco Forms Excel Export logo](docs/logo.png)

Umbraco Forms Excel Export adds native `.xlsx` exports to Umbraco Forms. It
registers two export types in the existing Forms export menu and streams the
workbook directly to the browser. No configuration is required.

The exported workbook contains an `Entries` worksheet with the columns and
records selected by the standard Umbraco Forms export filters.

## Export types

- **Excel** preserves dates, booleans, and safe integer values as native Excel
  cells. Other values are written as text, and fields with multiple values are
  joined with commas.
- **Excel (display values)** writes values as text and replaces stored prevalue
  keys with their captions. This is useful for fields such as dropdowns,
  checkboxes, and radio buttons where the value shown to an editor differs from
  the stored value.

The package writes workbooks as a stream, so it does not need to build the
entire file in memory before the download starts. It also checks Excel's limits
for rows, columns, and cell length and reports an error instead of creating an
invalid workbook.

## Supported versions

- Package version 17.x supports Umbraco Forms 17.
- Package version 18.x supports Umbraco Forms 18.

Install the package version that matches the major version of Umbraco Forms in
your site.

```shell
dotnet add package Umbraco.Community.Forms.ExcelExport
```

After installation, the two Excel options appear alongside the built-in export
types in Umbraco Forms.

## License

This package is licensed under the MIT License. See [LICENSE](LICENSE) for
details.

## Links

- [Documentation](https://github.com/skttl/umbraco-forms-excel-export#readme)
- [Issues](https://github.com/skttl/umbraco-forms-excel-export/issues)
- [Repository](https://github.com/skttl/umbraco-forms-excel-export)
