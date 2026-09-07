# Umbraco Forms Excel Export

Adds two `.xlsx` export types to Umbraco Forms:

- **Excel** preserves dates, booleans, and safe integers as native Excel cells.
- **Excel (display values)** replaces stored prevalue keys with their captions
  and writes the result as text.

Both exports use the standard Umbraco Forms filters and create an `Entries`
worksheet containing the selected columns and records. Fields with multiple
values are joined with commas.

Workbooks are streamed directly to the download instead of being assembled
entirely in memory. The exporter checks Excel's row, column, and cell-length
limits before it writes an invalid workbook.

## Supported versions

- Package version 17.x supports Umbraco Forms 17.
- Package version 18.x supports Umbraco Forms 18.

Install the package matching the major version of Umbraco Forms in your site.
No configuration is required. The Excel options appear alongside the built-in
export types after installation.

## Links

- [Documentation](https://github.com/skttl/umbraco-forms-excel-export#readme)
- [Issues](https://github.com/skttl/umbraco-forms-excel-export/issues)
- [Source code](https://github.com/skttl/umbraco-forms-excel-export)
