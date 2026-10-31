from pathlib import Path
import re, json, zipfile
from pypdf import PdfReader
from reportlab.graphics.barcode.code128 import Code128, _patterns

root=Path(__file__).resolve().parent.parent
reader=PdfReader(root/'outputs/barcode/Teakwood_All_Labels.pdf', strict=True)
assert len(reader.pages)==2
for p in reader.pages:
    assert abs(float(p.mediabox.width)*25.4/72-75)<0.001
    assert abs(float(p.mediabox.height)*25.4/72-50)<0.001
    text=p.extract_text()
    for expected in ['02224567891016','2599.00','Black','Teakwood Leathers Belt','Sep 2026']:
        assert expected in text, (expected,text)

source=(root/'build/Barcode.bas').read_text()
patterns=re.search(r'patterns = Split\("([0-9 ]+)"',source).group(1).split()
assert len(patterns)==107
for i,pat in enumerate(patterns):
    independent=''.join(str(ord(c.lower())-ord('a')+1) for c in _patterns[i])
    assert pat==independent,(i,pat,independent)
code=Code128('02224567891016');code.validate();symbols=code.encode()
assert symbols==[105,2,22,45,67,89,10,16,38,106]
expected_runs=[int(c) for s in symbols for c in patterns[s]]
expected_bars=[];x=0
for i,w in enumerate(expected_runs):
    if i%2==0: expected_bars.append((x,w))
    x+=w
for dpi,dpmm in [(203,8),(300,12)]:
    zpl=(root/f'build/sample{dpi}.zpl').read_text()
    assert f'^PW{75*dpmm}^LL{50*dpmm}' in zpl
    bars=re.findall(r'\^FO(\d+),(\d+)\^GB(\d+),(\d+),(\d+),B,0\^FS',zpl)
    expected=[(int((4.5+pos*.25)*dpmm),7*dpmm,int(w*.25*dpmm),8*dpmm,int(w*.25*dpmm)) for pos,w in expected_bars]
    assert [tuple(map(int,b)) for b in bars]==expected
with zipfile.ZipFile(root/'outputs/barcode/Teakwood_Barcode.xlsm') as f:
    assert 'xl/vbaProject.bin' in f.namelist()
print(json.dumps({'pdf_pages':len(reader.pages),'page_mm':[75,50],'barcode':'02224567891016','code128_patterns_verified':107,'zpl_resolutions_verified':[203,300],'embedded_vba':True}))
