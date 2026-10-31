# Teakwood Labels — VB.NET Windows Forms

## Run

Extract the application ZIP to a folder and run **TeakwoodBarcode.exe**. Keep the EXE, DLL, `.deps.json` and `.runtimeconfig.json` together.

The application requires Windows and the **.NET 8 Desktop Runtime**. It is installed on the development PC. On another PC, install the matching Windows Desktop Runtime from https://dotnet.microsoft.com/en-us/download/dotnet/8.0 if Windows asks for it. Microsoft Excel, EasierSoft add-ins and barcode fonts are not required.

## Upload and view data

1. Click **Upload Excel / CSV** and select `.xlsx`, `.xlsm` or `.csv`.
2. For Excel, select a worksheet from the drop-down.
3. **Print items** shows the barcode fields and editable print selection/quantities. **Uploaded worksheet** shows all imported columns, read-only.
4. The first populated row must contain these headers (order may vary; case and surrounding spaces are ignored):

   `SKU Name, ItemSku, VendorName, VendorSKu, ItemSize, ItemColor, MRP`

SKU Name is the barcode. ItemSku is the printed style code. VendorName is the product description. ItemSize, ItemColor and MRP supply the remaining item fields. VendorSKu is retained in the grid.

The same upload workflow also detects the Shree Leathers layout when the first row contains:

`QTY, ARTICLE NO., BARCODE, COLOUR, MRP, SIZE, QUANTITY, PRODUCT NAME, PKD.ON, MARKETED BY, CUSTOTMER CARE NO., Email, website, Manufactured by`

For Shree Leathers, `QTY` becomes the default number of labels for that row, `BARCODE` is encoded exactly as supplied (including hyphens), and the remaining fields are printed in the Shree reference layout. The app automatically selects 75 × 50 mm, changes the grid labels to Article no./Packed on and identifies the preview and print job as Shree Leathers. The Teakwood layout and shared details remain unchanged.

Store barcode identifiers as **Text** in Excel. Existing all-zero number formats, such as `00000000000000`, are supported. The app cannot reconstruct leading zeros or digits already lost in the source file. CSV values are read as text before validating MRP. Duplicate rows are intentionally preserved.

Legacy `.xls` files must be saved as `.xlsx`. Formula cells use their saved results; recalculate and save in Excel first. Password-protected workbooks are not supported. Macros in uploaded `.xlsm` files are never executed. Import limits: 100,000 records, 256 columns per sheet, 100 MB per uncompressed workbook XML part. The source file is never edited.

## Choose items, quantities and size

- Tick **Print** for each included row; edit **Qty** separately per item.
- To set a common quantity, enter it below the grid and click **Apply qty to ticked**.
- **Tick all** and **Clear ticks** change the included rows. A highlighted row controls the single-label preview, not batch inclusion.
- The summary shows the sum of quantities on ticked rows. A batch is limited to 10,000 labels.
- Presets: 75 × 50, 100 × 50, 100 × 75, 50 × 30 and 40 × 25 mm. Width/height fields support custom dimensions.
- **Full retail details** requires at least 75 × 50 mm and includes manufacturer/contact details from the sample.
- **Compact** requires at least 40 × 25 mm and includes product, barcode, style, price, size and colour. Selecting a small preset selects Compact; it deliberately omits manufacturer/contact information.
- Open **Shared label details** to change the manufacture month/year, brand, addresses and contact details. Review the sample-derived defaults before use.
- Settings are stored per Windows user at `%LOCALAPPDATA%\TeakwoodBarcode\settings.json`.

## Preview, PDF and printing

**Preview print batch** lets you page through all selected copies, without needing a printer. **Export batch PDF** saves the same selection and quantities with one exact-size label per page. Tick all and set quantity 1 to export every imported row once. PDF text supports printable English characters; Windows printing can render other characters supported by Arial.

Select the installed Windows queue for the Zebra ZD421 / EasierSoft driver. The app uses normal Windows driver printing, not RAW ZPL. Set the matching stock width/height in the driver and calibrate the Zebra for the label gap. Start with **Print 1 test label**, which prints the highlighted row exactly once. Then use **Print batch**.

The app requests a custom paper size and stops if the driver reports incompatible page dimensions. Some drivers require the custom media size to be defined in Windows printer preferences first. Barcode bars are aligned to whole printer dots. Print PDFs at **Actual size / 100%**, with scaling disabled. Verify label dimensions, margins, barcode scanning and small footer text on the actual printer before production.

## Developer information

Open `TeakwoodBarcode.sln` in Visual Studio 2022 with the **.NET desktop development** workload and .NET 8 targeting packs. The project retains your .NET 8 target. All application code is VB.NET.

Each window is a standard, visually editable Windows Form:

| Form | Layout and controls | Events and logic |
| --- | --- | --- |
| Main window | `MainForm.Designer.vb` and `MainForm.resx` | `MainForm.vb` |
| Shared label details | `DetailsForm.Designer.vb` and `DetailsForm.resx` | `DetailsForm.vb` |
| Batch preview | `BatchPreviewForm.Designer.vb` and `BatchPreviewForm.resx` | `BatchPreviewForm.vb` |

In Solution Explorer, right-click `MainForm.vb` and choose **View Designer** (Shift+F7). Select controls to move/resize them or change their properties. TableLayoutPanel containers arrange controls in rows and columns; edit their RowStyles/ColumnStyles or docking to change the layout. Double-click a button to open its named `Click` handler. The Properties window's Events tab shows the other event assignments. The main `.vb` file uses `Handles` clauses for `Click`, `SelectedIndexChanged`, `ValueChanged`, cell validation, loading and closing. It contains no control-construction or layout code.

The **Clear data** button clears the imported rows and preview only. It does not delete or edit the source file. All DataGridView columns, text boxes, size controls, upload/save dialogs and navigation buttons are declared in Designer files. `LabelCanvas` remains a custom drawing control for rendering the barcode preview; its position and size are editable in the Designer.

Build from the source folder in **Developer PowerShell for Visual Studio 2022**:

```powershell
msbuild TeakwoodBarcode.sln /restore /p:Configuration=Release
msbuild Teakwood.Barcode.vbproj /t:Publish /p:Configuration=Release /p:SelfContained=false /p:PublishDir=publish/
```

There are no third-party NuGet dependencies. `NuGet.Config` clears package sources so normal builds use installed .NET targeting packs. To produce a self-contained build for a PC without .NET, enable the official NuGet source in a separate build configuration and publish with the appropriate runtime identifier and `--self-contained true`.

Core components:

- `ImportService.vb`: local CSV and Open XML workbook reader.
- `Models.vb`: imported row mapping and per-user settings.
- `LabelEngine.vb`: Code 128 encoding, layout, screen/printer rendering and vector PDF output.
- `Printing.vb`: Windows printer integration and the custom barcode drawing control.
- `MainForm.vb`, `DetailsForm.vb`, `BatchPreviewForm.vb`: named UI event handlers and business logic.
- `SelfTests.vb`: integration checks, accessible through `--self-test <original workspace folder>` with the supplied sample CSV and original XLSM fixture.

## Validation completed

Release build succeeds with zero warnings and errors. Tests cover the original Teakwood CSV/XLSM format and the supplied nine-row Shree workbook, leading zeros, quoted CSV fields, Code 128 sets B/C, per-row quantities, selection, six page sizes, invalid inputs, PDF output and 203/300-DPI printer rendering. Physical Zebra/EasierSoft printing remains unverified because that printer is not installed on the development PC.
