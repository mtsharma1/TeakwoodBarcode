from pathlib import Path
from zipfile import ZipFile
from xml.sax.saxutils import escape
import sys
from pypdf import PdfReader
from PIL import Image
from reportlab.graphics.barcode.code128 import Code128, _patterns

out=Path(__file__).parent
if '--fixture' in sys.argv:
    headers=['SKU Name','ItemSku','VendorName','VendorSKu','ItemSize','ItemColor','MRP']
    ns='http://schemas.openxmlformats.org/spreadsheetml/2006/main'
    def row(n, values):
        return '<row r="%s">'%n+''.join(f'<c r="{chr(65+i)}{n}" t="inlineStr"><is><t>{escape(v)}</t></is></c>' for i,v in enumerate(values))+'</row>'
    records=row(1,headers)+row(2,['000123','TEST','Test item','TEST','42','Black','150.25'])
    records=records.replace('<c r="A2" t="inlineStr"><is><t>000123</t></is></c>','<c r="A2" s="1"><v>123</v></c>')
    records=records.replace('<c r="G2" t="inlineStr"><is><t>150.25</t></is></c>','<c r="G2"><f>100+50.25</f><v>150.25</v></c>')
    parts={
      '[Content_Types].xml':'<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/></Types>',
      'xl/workbook.xml':f'<workbook xmlns="{ns}" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Notes" sheetId="1" r:id="r1"/><sheet name="Items" sheetId="2" r:id="r2"/></sheets></workbook>',
      'xl/_rels/workbook.xml.rels':'<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="r1" Target="worksheets/sheet1.xml" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"/><Relationship Id="r2" Target="/xl/worksheets/sheet2.xml" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"/></Relationships>',
      'xl/worksheets/sheet1.xml':f'<worksheet xmlns="{ns}"><sheetData>{row(1,["Notes"])}</sheetData></worksheet>',
      'xl/worksheets/sheet2.xml':f'<worksheet xmlns="{ns}"><sheetData>{records}</sheetData></worksheet>',
      'xl/styles.xml':f'<styleSheet xmlns="{ns}"><numFmts count="1"><numFmt numFmtId="164" formatCode="000000"/></numFmts><cellXfs count="2"><xf numFmtId="0"/><xf numFmtId="164"/></cellXfs></styleSheet>'
    }
    with ZipFile(out/'import-fixture.xlsx','w') as z:
        for k,v in parts.items():z.writestr(k,v)
    print('Created XLSX import test fixture')
    sys.exit()

all_pdf=PdfReader(out/'Desktop_All_Labels.pdf',strict=True)
assert len(all_pdf.pages)==2
assert '02224567891016' in all_pdf.pages[0].extract_text()
assert '02224567891018' in all_pdf.pages[1].extract_text()
for w,h in [(75,50),(100,50),(100,75),(50,30),(40,25),(85,60)]:
    p=PdfReader(out/f'label-{w}x{h}.pdf',strict=True).pages[0]
    assert abs(float(p.mediabox.width)*25.4/72-w)<0.001
    assert abs(float(p.mediabox.height)*25.4/72-h)<0.001
    code=Code128('02224567891016');code.validate();symbols=code.encode()
    expected=[ord(c.lower())-ord('a')+1 for s in symbols for c in _patterns[s]]
    for dpi in (203,300):
        img=Image.open(out/f'printer-{w}x{h}-{dpi}.png').convert('RGB')
        base=(50,30) if w<75 else (75,50)
        factor=min(w/base[0],h/base[1]); off=(h-base[1]*factor)/2
        y=round((off+10*factor)/h*img.height)
        pixels=[sum(img.getpixel((x,y)))<200 for x in range(img.width)]
        start=pixels.index(True);end=len(pixels)-1-pixels[::-1].index(True)
        runs=[];last=True;n=0
        for v in pixels[start:end+1]:
            if v==last:n+=1
            else:runs.append(n);last=v;n=1
        runs.append(n)
        unit=min(runs)
        assert all(r%unit==0 for r in runs),(w,h,dpi,runs)
        assert [r//unit for r in runs]==expected,(w,h,dpi)
        assert start>=10*unit and img.width-end-1>=10*unit
print('PASS: PDF dimensions and all 12 printer raster barcodes match independent Code128 patterns, checksums and quiet zones.')
