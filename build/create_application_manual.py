from pathlib import Path
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK, WD_LINE_SPACING
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_CELL_VERTICAL_ALIGNMENT
from docx.enum.section import WD_SECTION
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

ROOT = Path(r"E:\My Client\Teakwood\Jitender\Barcode Work")
OUT = ROOT / "outputs" / "documentation" / "Barcode_Label_Studio_Application_Manual.docx"
MAIN_UI = Path(r"C:\Users\Mithlesh\AppData\Local\Temp\codex-clipboard-6a10a360-9efe-4341-b555-24fbd3cb0ad8.png")
TEAKWOOD_LABEL = ROOT / "build" / "desktop-tests" / "label-75x50.png"
SHREE_LABEL = ROOT / "build" / "desktop-tests" / "shree-label-75x50.png"
SHREE_PREVIEW = Path(r"C:\Users\Mithlesh\AppData\Local\Temp\codex-clipboard-f8eb0da6-5f8d-4b17-85df-35ff7493e73e.png")

DARK_GREEN = "214436"
MID_GREEN = "DDEAE3"
PALE_GREEN = "F2F7F4"
LIGHT_GRAY = "F2F2F2"
BORDER = "D9D9D9"
WHITE = "FFFFFF"
BLACK = RGBColor(0, 0, 0)


def set_cell_shading(cell, fill):
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = tc_pr.find(qn("w:shd"))
    if shd is None:
        shd = OxmlElement("w:shd")
        tc_pr.append(shd)
    shd.set(qn("w:fill"), fill)


def set_cell_borders(cell, color=BORDER, size="5"):
    tc_pr = cell._tc.get_or_add_tcPr()
    borders = tc_pr.first_child_found_in("w:tcBorders")
    if borders is None:
        borders = OxmlElement("w:tcBorders")
        tc_pr.append(borders)
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        tag = "w:" + edge
        element = borders.find(qn(tag))
        if element is None:
            element = OxmlElement(tag)
            borders.append(element)
        element.set(qn("w:val"), "single")
        element.set(qn("w:sz"), size)
        element.set(qn("w:color"), color)


def set_cell_margins(cell, top=90, start=110, bottom=90, end=110):
    tc = cell._tc
    tc_pr = tc.get_or_add_tcPr()
    tc_mar = tc_pr.first_child_found_in("w:tcMar")
    if tc_mar is None:
        tc_mar = OxmlElement("w:tcMar")
        tc_pr.append(tc_mar)
    for name, value in (("top", top), ("start", start), ("bottom", bottom), ("end", end)):
        node = tc_mar.find(qn("w:" + name))
        if node is None:
            node = OxmlElement("w:" + name)
            tc_mar.append(node)
        node.set(qn("w:w"), str(value))
        node.set(qn("w:type"), "dxa")


def set_repeat_table_header(row):
    tr_pr = row._tr.get_or_add_trPr()
    tbl_header = OxmlElement("w:tblHeader")
    tbl_header.set(qn("w:val"), "true")
    tr_pr.append(tbl_header)


def set_run_font(run, name="Aptos", size=None, bold=None, color=BLACK):
    run.font.name = name
    run._element.get_or_add_rPr().rFonts.set(qn("w:ascii"), name)
    run._element.get_or_add_rPr().rFonts.set(qn("w:hAnsi"), name)
    if size is not None:
        run.font.size = Pt(size)
    if bold is not None:
        run.bold = bold
    run.font.color.rgb = color


def add_page_number(paragraph):
    paragraph.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    run = paragraph.add_run("Page ")
    set_run_font(run, size=8.5, color=RGBColor(90, 90, 90))
    fld = OxmlElement("w:fldSimple")
    fld.set(qn("w:instr"), "PAGE")
    paragraph._p.append(fld)


def add_image_alt(inline_shape, title, description):
    doc_pr = inline_shape._inline.docPr
    doc_pr.set("title", title)
    doc_pr.set("descr", description)


def add_picture(doc, path, width, caption, alt):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.keep_with_next = True
    shape = p.add_run().add_picture(str(path), width=Inches(width))
    add_image_alt(shape, caption, alt)
    cap = doc.add_paragraph(caption)
    cap.style = doc.styles["Caption"]
    cap.alignment = WD_ALIGN_PARAGRAPH.CENTER
    cap.paragraph_format.space_after = Pt(8)
    return shape


def add_table(doc, headers, rows, widths=None):
    table = doc.add_table(rows=1, cols=len(headers))
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = False
    header = table.rows[0]
    set_repeat_table_header(header)
    for i, text in enumerate(headers):
        cell = header.cells[i]
        set_cell_shading(cell, DARK_GREEN)
        set_cell_borders(cell)
        set_cell_margins(cell)
        cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
        p = cell.paragraphs[0]
        p.paragraph_format.space_after = Pt(0)
        r = p.add_run(text)
        set_run_font(r, size=9, bold=True, color=RGBColor(255, 255, 255))
        if widths:
            cell.width = Inches(widths[i])
    for row_index, values in enumerate(rows):
        cells = table.add_row().cells
        for i, value in enumerate(values):
            cell = cells[i]
            set_cell_shading(cell, PALE_GREEN if row_index % 2 else WHITE)
            set_cell_borders(cell)
            set_cell_margins(cell)
            cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
            p = cell.paragraphs[0]
            p.paragraph_format.space_after = Pt(0)
            r = p.add_run(str(value))
            set_run_font(r, size=8.6)
            if widths:
                cell.width = Inches(widths[i])
    doc.add_paragraph().paragraph_format.space_after = Pt(0)
    return table


def add_bullets(doc, items, level=0):
    for item in items:
        p = doc.add_paragraph(style="List Bullet" if level == 0 else "List Bullet 2")
        p.add_run(item)
        p.paragraph_format.space_after = Pt(2)


def add_steps(doc, items):
    for item in items:
        p = doc.add_paragraph(style="List Number")
        p.add_run(item)
        p.paragraph_format.space_after = Pt(4)


def add_note(doc, lead, text):
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(3)
    p.paragraph_format.space_after = Pt(7)
    r = p.add_run(lead + " ")
    r.bold = True
    p.add_run(text)


def page_break(doc):
    doc.add_page_break()


doc = Document()
section = doc.sections[0]
section.top_margin = Inches(0.68)
section.bottom_margin = Inches(0.65)
section.left_margin = Inches(0.72)
section.right_margin = Inches(0.72)

styles = doc.styles
styles["Normal"].font.name = "Aptos"
styles["Normal"]._element.rPr.rFonts.set(qn("w:ascii"), "Aptos")
styles["Normal"]._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos")
styles["Normal"].font.size = Pt(9.5)
styles["Normal"].font.color.rgb = BLACK
styles["Normal"].paragraph_format.space_after = Pt(5)
styles["Normal"].paragraph_format.line_spacing = 1.08

for style_name, size, before, after in (("Title", 27, 0, 12), ("Heading 1", 18, 12, 6), ("Heading 2", 13, 9, 4), ("Heading 3", 10.5, 7, 3)):
    style = styles[style_name]
    style.font.name = "Aptos Display" if style_name in ("Title", "Heading 1") else "Aptos"
    style._element.rPr.rFonts.set(qn("w:ascii"), style.font.name)
    style._element.rPr.rFonts.set(qn("w:hAnsi"), style.font.name)
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = BLACK
    style.paragraph_format.space_before = Pt(before)
    style.paragraph_format.space_after = Pt(after)
    style.paragraph_format.keep_with_next = True

styles["Caption"].font.name = "Aptos"
styles["Caption"].font.size = Pt(8.5)
styles["Caption"].font.italic = True
styles["Caption"].font.color.rgb = RGBColor(80, 80, 80)

for sec in doc.sections:
    header = sec.header
    hp = header.paragraphs[0]
    hp.text = "Cunnect IT  |  Barcode Label Studio"
    hp.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    for run in hp.runs:
        set_run_font(run, size=8.5, bold=True, color=RGBColor(70, 70, 70))
    add_page_number(sec.footer.paragraphs[0])

# Cover
title = doc.add_paragraph(style="Title")
title.alignment = WD_ALIGN_PARAGRAPH.LEFT
title.add_run("Barcode Label Studio Application Manual")
subtitle = doc.add_paragraph()
subtitle.paragraph_format.space_after = Pt(16)
r = subtitle.add_run("Teakwood and Shree Leathers label printing")
set_run_font(r, size=14, bold=True, color=RGBColor(55, 55, 55))

meta = add_table(doc, ["Document", "Details"], [
    ("Application version", "1.1.2"),
    ("Prepared for", "Barcode printing operators and application maintainers"),
    ("Prepared by", "Cunnect IT"),
    ("Label media", "75 × 50 mm primary retail label"),
    ("Printer integration", "Windows printer driver, including Zebra ZD421 queues"),
    ("Document date", "31 October 2026"),
], widths=[1.8, 4.6])

p = doc.add_paragraph()
p.paragraph_format.space_before = Pt(8)
p.paragraph_format.space_after = Pt(12)
p.add_run("This manual explains how to install, configure, operate, troubleshoot and maintain the Barcode Label Studio application. It covers the original Teakwood format and the automatically detected Shree Leathers format. Operators should print one test label and verify dimensions and scan quality before releasing a production batch.")

image_table = doc.add_table(rows=1, cols=2)
image_table.alignment = WD_TABLE_ALIGNMENT.CENTER
image_table.autofit = False
row_properties = image_table.rows[0]._tr.get_or_add_trPr()
row_properties.append(OxmlElement("w:tblHeader"))
for cell in image_table.rows[0].cells:
    set_cell_margins(cell, 30, 40, 30, 40)
    cell.width = Inches(3.1)
if TEAKWOOD_LABEL.exists():
    s = image_table.cell(0, 0).paragraphs[0].add_run().add_picture(str(TEAKWOOD_LABEL), width=Inches(2.85))
    add_image_alt(s, "Teakwood label", "Current 75 by 50 millimetre Teakwood barcode label")
if SHREE_LABEL.exists():
    s = image_table.cell(0, 1).paragraphs[0].add_run().add_picture(str(SHREE_LABEL), width=Inches(2.85))
    add_image_alt(s, "Shree Leathers label", "Current 75 by 50 millimetre Shree Leathers barcode label")
for cell in image_table.rows[0].cells:
    p = cell.paragraphs[0]
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER

page_break(doc)

# Contents and overview
doc.add_heading("Contents", level=1)
contents = [
    "1  Application overview",
    "2  Installation and first launch",
    "3  Main window and controls",
    "4  Supported files and data requirements",
    "5  Importing and reviewing data",
    "6  Selecting labels and quantities",
    "7  Label sizes and shared details",
    "8  Preview and PDF export",
    "9  Printing with a Windows label printer",
    "10 Teakwood label format",
    "11 Shree Leathers label format",
    "12 Troubleshooting",
    "13 Security and distribution",
    "14 Developer and maintenance guide",
    "15 Operational acceptance checklist",
]
for line in contents:
    p = doc.add_paragraph(line)
    p.paragraph_format.left_indent = Inches(0.15)
    p.paragraph_format.space_after = Pt(3)

doc.add_heading("1 Application overview", level=1)
doc.add_paragraph("Barcode Label Studio is a Windows Forms application for importing product data, previewing exact-size barcode labels, exporting label batches to PDF and printing through an installed Windows printer driver. Microsoft Excel is not required on the operator’s computer.")
add_bullets(doc, [
    "Imports .xlsx, .xlsm and .csv files without changing the source file.",
    "Automatically detects Teakwood and Shree Leathers worksheet formats.",
    "Preserves leading zeros when identifiers are stored correctly in the source.",
    "Supports per-row selection and quantities up to 10,000 labels per batch.",
    "Produces one exact-size label per PDF page.",
    "Prints through normal Windows printer queues and aligns barcode modules to printer dots.",
    "Includes a scalable preview for lower-resolution and high-DPI screens.",
])

doc.add_heading("Application components", level=2)
add_table(doc, ["Component", "Purpose"], [
    ("Main window", "Upload data, select a worksheet, choose rows and quantities, preview labels, export PDF and print."),
    ("Shared label details", "Maintain Teakwood manufacture, brand, address and contact information."),
    ("Print preview", "Review every selected label copy before PDF export or printing."),
    ("Label engine", "Build Teakwood and Shree Leathers layouts and encode Code 128 barcodes."),
    ("Windows print integration", "Send exact-size pages to the selected printer driver."),
], widths=[1.7, 4.7])

page_break(doc)

# Installation
doc.add_heading("2 Installation and first launch", level=1)
doc.add_heading("System requirements", level=2)
add_table(doc, ["Requirement", "Supported configuration"], [
    ("Operating system", "64-bit Windows capable of running .NET 8 desktop applications."),
    ("Display", "The application fits lower-resolution screens automatically; 1024 × 768 or higher is recommended."),
    ("Runtime", "Included in the self-contained installer."),
    ("Printer", "An installed Windows printer queue. Zebra ZD421 is the primary target."),
    ("Source files", ".xlsx, .xlsm or .csv. Legacy .xls must be saved as .xlsx first."),
], widths=[1.7, 4.7])

doc.add_heading("Install the application", level=2)
add_steps(doc, [
    "Close any previous Barcode Label Studio instance.",
    "Run TeakwoodBarcode_Setup.exe.",
    "Enter the installation password supplied separately by the Cunnect IT administrator.",
    "Accept the default per-user installation folder unless your IT policy requires another location.",
    "Complete Setup. Desktop and Start Menu shortcuts are created.",
    "Launch the application and verify that the main window opens within the available screen area.",
])
add_note(doc, "Password handling.", "The installer is encrypted and password protected. Do not include the installation password in documentation, source control, email chains or public repositories.")
add_note(doc, "Windows Application Control.", "The current executable is not digitally signed. A managed laptop may block it with error 4551 before application code runs. The organisation must digitally sign the application with a trusted code-signing certificate or explicitly allowlist it under the applicable Windows policy.")

doc.add_heading("Uninstall or upgrade", level=2)
doc.add_paragraph("Install a newer version over the existing installation to upgrade it. To remove the application, open Windows Settings, choose Apps, locate Teakwood Barcode Label Studio and select Uninstall. Per-user settings may remain in the local application data folder so they can be reused after an upgrade.")

# UI
page_break(doc)
doc.add_heading("3 Main window and controls", level=1)
if MAIN_UI.exists():
    add_picture(doc, MAIN_UI, 6.6, "Main application window", "Main Barcode Label Studio window showing uploaded records, controls, printer selection and a Teakwood label preview")
add_table(doc, ["Area", "Function"], [
    ("Upload Excel or CSV", "Choose a supported source file."),
    ("Worksheet", "Select one worksheet from an Excel workbook."),
    ("Clear data", "Remove imported rows from the application without changing the source file."),
    ("Print items", "Show mapped barcode records with Print selection and editable quantity."),
    ("Uploaded worksheet", "Show all imported source columns as read-only data."),
    ("Selected row preview", "Display the label for the highlighted record."),
    ("Size and layout", "Choose a preset or custom label size and the full or compact Teakwood layout."),
    ("Windows printer", "Choose and refresh installed printer queues."),
    ("Preview print batch", "Page through every selected copy."),
    ("Export batch PDF", "Create an exact-size, one-label-per-page PDF."),
    ("Print 1 test label", "Print the highlighted row once."),
    ("Print batch", "Print all selected rows using their quantities."),
], widths=[1.85, 4.55])

page_break(doc)

# Source formats
doc.add_heading("4 Supported files and data requirements", level=1)
doc.add_paragraph("The application reads data locally. Uploaded files remain unchanged, and macros in .xlsm workbooks are never executed. The first populated row must contain the expected headers. Header order and letter case may vary; surrounding spaces are ignored.")
add_bullets(doc, [
    "Maximum 100,000 data records per import.",
    "Maximum 256 worksheet columns.",
    "Maximum 100 MB per uncompressed workbook XML part.",
    "Formula cells must contain saved calculated values.",
    "Duplicate rows remain separate printable records.",
    "Password-protected Excel workbooks are not supported.",
])

doc.add_heading("Teakwood columns", level=2)
add_table(doc, ["Source column", "Printed or application use"], [
    ("SKU Name", "Barcode value. Store as text to preserve leading zeros."),
    ("ItemSku", "Style code."),
    ("VendorName", "Product description."),
    ("VendorSKu", "Retained in the print grid."),
    ("ItemSize", "Printed size."),
    ("ItemColor", "Printed colour."),
    ("MRP", "Maximum retail price."),
], widths=[1.65, 4.75])

doc.add_heading("Shree Leathers columns", level=2)
add_table(doc, ["Source column", "Printed or application use"], [
    ("QTY", "Default number of labels for the row."),
    ("ARTICLE NO.", "Article number."),
    ("BARCODE", "Exact Code 128 value, including a hyphen when present."),
    ("COLOUR", "Colour identifier."),
    ("MRP", "Price and inclusive-tax statement."),
    ("SIZE", "Printed size."),
    ("QUANTITY", "Pack quantity text printed on each label."),
    ("PRODUCT NAME", "Printed product description."),
    ("PKD.ON", "Packing month and year."),
    ("MARKETED BY", "Marketing company and address lines."),
    ("CUSTOTMER CARE NO.", "Customer care number; the supplied workbook spelling is supported."),
    ("Email and website", "Customer contact information."),
    ("Manufactured by", "Manufacturer name and address."),
], widths=[1.8, 4.6])

add_note(doc, "Leading zeros.", "Excel must retain barcode identifiers as text or with a supported all-zero number format. The application cannot reconstruct digits already removed by Excel.")

# Import workflow
doc.add_heading("5 Importing and reviewing data", level=1)
add_steps(doc, [
    "Select Upload Excel or CSV.",
    "Choose the source file. The application reads it locally.",
    "For Excel, select the required worksheet. The first compatible worksheet is selected automatically when possible.",
    "Review the read-only Uploaded worksheet tab to confirm the source values.",
    "Open Print items and check the mapped barcode, product, size, colour, price and quantity fields.",
    "Select a row and confirm the label preview before continuing.",
])
doc.add_paragraph("When a Shree Leathers file is detected, the title and preview identify Shree Leathers, Article no. and Packed on replace the corresponding grid captions, the label size is set to 75 × 50 mm and shared Teakwood details are disabled. Import controls otherwise behave exactly as they do for Teakwood.")

doc.add_heading("6 Selecting labels and quantities", level=1)
add_steps(doc, [
    "Tick Print for every row to include in the batch.",
    "Edit Qty directly for an individual record, or enter a common quantity below the grid and select Apply qty to ticked.",
    "Use Tick all or Clear ticks when the same selection applies to every row.",
    "Check the summary for record count, selected count and total labels.",
    "Highlight one row to control the on-screen preview and the single test print.",
])
add_note(doc, "Batch limit.", "Each row quantity must be between 1 and 10,000, and the combined batch must not exceed 10,000 labels. Shree Leathers QTY values are loaded as the initial print quantities and can be edited before printing.")

page_break(doc)

# Sizes/settings
doc.add_heading("7 Label sizes and shared details", level=1)
doc.add_heading("Size presets", level=2)
add_table(doc, ["Preset", "Typical use"], [
    ("75 × 50 mm", "Primary Teakwood retail label and required Shree Leathers format."),
    ("100 × 50 mm", "Wider full-detail label."),
    ("100 × 75 mm", "Larger full-detail label."),
    ("50 × 30 mm", "Compact Teakwood label."),
    ("40 × 25 mm", "Small compact Teakwood label."),
    ("Custom", "25–200 mm width and 15–200 mm height, subject to layout minimums."),
], widths=[1.6, 4.8])
doc.add_paragraph("The full Teakwood retail layout requires at least 75 × 50 mm. Compact Teakwood requires at least 40 × 25 mm and omits manufacturer details. The Shree Leathers layout requires at least 75 × 50 mm and always uses its full workbook-driven design.")

doc.add_heading("Shared Teakwood label details", level=2)
doc.add_paragraph("Open Shared label details to maintain the manufacture month and year, net quantity, brand, country of origin, manufacturer, addresses, packer, phone and email. These settings apply only to the Teakwood full-detail layout and are stored per Windows user.")
add_note(doc, "Settings location.", r"The settings file is stored at %LOCALAPPDATA%\TeakwoodBarcode\settings.json. Review all sample-derived values before production printing.")

doc.add_heading("8 Preview and PDF export", level=1)
add_steps(doc, [
    "Select Preview print batch to page through all selected copies.",
    "Use Previous, Next or the label-number control to inspect the batch.",
    "Confirm barcode value, product data, margins and footer details.",
    "Select Export batch PDF and choose a destination.",
    "Open the PDF and print at Actual size or 100%. Disable Fit, Shrink and Scale options.",
])
add_note(doc, "PDF structure.", "Each PDF page is exactly one label. Page dimensions follow the selected label size, and selected quantities determine the number of pages.")

# Printing
doc.add_heading("9 Printing with a Windows label printer", level=1)
add_steps(doc, [
    "Install the correct Windows printer driver and confirm the queue appears in Windows Settings.",
    "Create or select a media size matching the application label dimensions.",
    "Load matching stock and calibrate the printer for its label gap.",
    "Select Refresh, then choose the printer queue in Barcode Label Studio.",
    "Highlight a representative row and print one test label.",
    "Measure the printed label, scan the barcode and inspect all small text.",
    "After the test passes, select Print batch and confirm the displayed media size and quantity.",
])
add_bullets(doc, [
    "The application uses the Windows printer driver, not raw ZPL output.",
    "The driver must accept the requested custom paper dimensions.",
    "Barcode modules are aligned to whole printer dots for 203 and 300 DPI output.",
    "The printer driver may need the media size configured before the application starts.",
])

page_break(doc)

# Label formats
doc.add_heading("10 Teakwood label format", level=1)
add_picture(doc, TEAKWOOD_LABEL, 5.7, "Teakwood 75 × 50 mm label", "Teakwood full retail barcode label with product, barcode, style, price, item details and contact information")
add_table(doc, ["Region", "Content"], [
    ("Top", "Product name, Code 128 barcode and human-readable barcode value."),
    ("Middle left", "Style code, manufacture date, MRP and inclusive-tax statement."),
    ("Middle right", "Net quantity, brand, colour, size and country of origin."),
    ("Bottom", "Manufacturer, packer, addresses, phone and email."),
], widths=[1.5, 4.9])

doc.add_heading("11 Shree Leathers label format", level=1)
add_picture(doc, SHREE_LABEL, 5.7, "Shree Leathers 75 × 50 mm label", "Shree Leathers barcode label with logo, article, repeated price and size, quantity, packing month, market contact and manufacturer")
add_table(doc, ["Region", "Content"], [
    ("Top left", "Sreeleathers mark and Kolkatta text reproduced from the supplied reference."),
    ("Top right", "Mixed Code 128 B/C barcode and human-readable workbook value."),
    ("Upper body", "Article, colour, repeated MRP and size, pack quantity and packing month."),
    ("Middle", "Product name and marketed-by contact information."),
    ("Bottom", "Manufacturer name and address with a protected bottom margin."),
], widths=[1.5, 4.9])
add_note(doc, "Workbook authority.", "The application prints the BARCODE value from the workbook, such as 207420-9. The demonstration number in the original reference image is not substituted for workbook data.")

page_break(doc)

# Troubleshooting
doc.add_heading("12 Troubleshooting", level=1)
add_table(doc, ["Message or symptom", "Reason", "Recommended action"], [
    ("Application Control policy has blocked this file, error 4551", "Windows blocks the unsigned executable before it starts.", "Digitally sign the executable with a trusted certificate or ask the organisation’s IT administrator to allowlist it."),
    ("Missing columns", "The first populated row does not contain the required Teakwood or Shree headers.", "Correct the workbook headers and upload again."),
    ("Barcode lost leading zeros", "Excel stored the identifier as a number and removed digits.", "Restore the source identifier and store it as text before import."),
    ("Formula has no saved value", "The workbook formula was not recalculated before saving.", "Open the workbook in Excel, recalculate, save and retry."),
    ("Driver did not accept selected label size", "The printer queue rejected the requested paper dimensions.", "Create/select the exact media size in printer preferences and retry."),
    ("Label prints too large or too small", "PDF or driver scaling is enabled.", "Use Actual size or 100% and confirm driver media dimensions."),
    ("Text appears faint", "Thermal darkness, speed or driver settings are unsuitable.", "Increase darkness, reduce print speed and verify thermal media."),
    ("Print preview extends beyond the screen", "Old application version or extreme display scaling.", "Install version 1.0.2 or later; the current version sizes forms to the working area."),
    ("Installer requests a password", "Distribution is intentionally restricted.", "Obtain the current password from the Cunnect IT administrator."),
    ("Cannot push repository to GitHub", "Generated binaries, node_modules or files over 100 MB were committed.", "Keep only source under version control and publish installers through GitHub Releases."),
], widths=[1.85, 2.05, 2.5])

doc.add_heading("Printer verification", level=2)
doc.add_paragraph("Automated tests cover 203 and 300 DPI rendering, page sizes, Code 128 checksums, PDF output and Windows PrintDocument page creation. A physical printer test remains necessary because printer calibration, stock, darkness, speed, driver offsets and sensor settings cannot be validated in software alone.")

# Security
doc.add_heading("13 Security and distribution", level=1)
add_bullets(doc, [
    "The installer is encrypted and password protected.",
    "The application and installer are currently unsigned.",
    "Installer password protection controls distribution but does not replace digital code signing.",
    "Do not commit the password, installer outputs, private client workbooks or generated binaries to source control.",
    "Use a trusted code-signing certificate for managed environments that enforce Windows Application Control.",
    "Publish setup files through a controlled file share or GitHub Release, not inside the source tree.",
])

page_break(doc)

# Developer guide
doc.add_heading("14 Developer and maintenance guide", level=1)
doc.add_heading("Source locations", level=2)
add_table(doc, ["Item", "Current development location"], [
    ("Visual Studio solution", r"E:\My Client\Teakwood\Jitender\Barcode Work\BarcodeDesktop\TeakwoodBarcode.sln"),
    ("Project", r"BarcodeDesktop\Teakwood.Barcode.vbproj"),
    ("Installer definition", r"build\TeakwoodBarcodeInstaller.iss"),
    ("Setup output", r"outputs\desktop\TeakwoodBarcode_Setup.exe"),
], widths=[1.7, 4.7])

doc.add_heading("Application architecture", level=2)
add_table(doc, ["File", "Responsibility"], [
    ("Program.vb", "Application startup and self-test command routing."),
    ("MainForm.Designer.vb", "Physical WinForms controls and layout."),
    ("MainForm.vb", "Upload, worksheet selection, grid events, preview, PDF and print commands."),
    ("DetailsForm files", "Teakwood shared-label detail editor."),
    ("BatchPreviewForm files", "Responsive multi-page label preview."),
    ("ImportService.vb", "Local CSV and Open XML workbook reading."),
    ("Models.vb", "Teakwood/Shree format detection, validation, mapping and settings."),
    ("LabelEngine.vb", "Code 128 B/C encoding, both label layouts, PDF and graphics rendering."),
    ("Printing.vb", "Windows PrintDocument integration and preview canvas."),
    ("SelfTests.vb", "Regression coverage for imports, layouts, PDF, quantities and printer DPI."),
], widths=[2.05, 4.35])

doc.add_heading("Build and test", level=2)
doc.add_paragraph("Use Developer PowerShell for Visual Studio 2022 with the .NET desktop development workload and .NET 8 targeting packs.")
commands = [
    "msbuild TeakwoodBarcode.sln /restore /p:Configuration=Release",
    r".\bin\Release\net8.0-windows\TeakwoodBarcode.exe --self-test \"E:\My Client\Teakwood\Jitender\Barcode Work\"",
]
for cmd in commands:
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.25)
    r = p.add_run(cmd)
    set_run_font(r, name="Consolas", size=8.5)

doc.add_heading("Source control rules", level=2)
doc.add_paragraph("The Git repository should contain source files only. A root .gitignore should exclude Visual Studio caches, build outputs, test artifacts, node_modules and installers. The installer definition currently contains protected configuration and must not be committed to a public repository without first removing secrets.")
code_lines = [
    ".vs/",
    "**/.vs/",
    "**/bin/",
    "**/obj/",
    "**/node_modules/",
    "build/",
    "outputs/",
    "*.user",
    "*.suo",
    "*.vsidx",
]
for line in code_lines:
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.25)
    p.paragraph_format.space_after = Pt(0)
    r = p.add_run(line)
    set_run_font(r, name="Consolas", size=8.3)

doc.add_heading("Regression expectations", level=2)
add_bullets(doc, [
    "Any Shree-only layout adjustment must leave the Teakwood rendered image and mapping unchanged.",
    "Any import change must test both the original Teakwood columns and the supplied Shree workbook structure.",
    "Any barcode change must verify numeric Code 128 C, mixed B/C values and checksum output.",
    "Any printing change must render at both 203 and 300 DPI and preserve page dimensions.",
    "Any UI change must remain usable on lower-resolution and high-DPI screens.",
])

# Checklist
doc.add_heading("15 Operational acceptance checklist", level=1)
checklist = [
    "The correct application version is installed and opens without a policy warning.",
    "The intended Excel or CSV worksheet imports without missing-column errors.",
    "The application identifies the correct Teakwood or Shree Leathers format.",
    "Record count, selected count and total label quantity are correct.",
    "The highlighted row preview matches the source data.",
    "Label size is 75 × 50 mm for the primary retail formats.",
    "The correct Windows printer queue and media size are selected.",
    "One test label prints at the correct physical dimensions.",
    "The barcode scans to the exact displayed value.",
    "Small text is readable and remains inside every label edge.",
    "The PDF is printed at Actual size or 100% with scaling disabled.",
    "The production quantity is confirmed before Print batch is selected.",
]
for item in checklist:
    p = doc.add_paragraph()
    p.paragraph_format.space_after = Pt(4)
    r = p.add_run("☐  " + item)
    set_run_font(r, size=9.4)

doc.add_paragraph()
closing = doc.add_paragraph()
closing.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = closing.add_run("Cunnect IT  |  Barcode Label Studio  |  Version 1.1.2")
set_run_font(r, size=9, bold=True, color=RGBColor(55, 55, 55))

OUT.parent.mkdir(parents=True, exist_ok=True)
doc.core_properties.title = "Barcode Label Studio Application Manual"
doc.core_properties.subject = "Operation, printing, troubleshooting and maintenance guide"
doc.core_properties.author = "Cunnect IT"
doc.core_properties.keywords = "barcode, Teakwood, Shree Leathers, Zebra ZD421, label printing"
doc.save(OUT)
print(OUT)
