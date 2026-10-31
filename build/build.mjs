import fs from 'node:fs/promises';
import {Workbook,SpreadsheetFile} from '@oai/artifact-tool';
const wb=Workbook.create();
const ui=wb.worksheets.add('Control');
const label=wb.worksheets.add('Label');
const data=wb.worksheets.add('Data');
const settings=wb.worksheets.add('Settings');
for(const s of [ui,label,data,settings]){s.showGridLines=false;s.getRange('A1:L40').format.font={name:'Arial',size:11};s.getRange('A1:L40').format.rowHeight=23;}
function put(s,c,v){s.getRange(c).values=[[v]];}
function title(s,t){put(s,'B2',t);s.getRange('B2').format.font={name:'Arial',size:17,bold:true,color:'#243B35'};}
function band(s,r,t){s.getRange(r).merge();s.getRange(r).values=[[t]];s.getRange(r).format={fill:'#243B35',font:{name:'Arial',size:11,bold:true,color:'#FFFFFF'}};}
ui.getRange('A:A').format.columnWidth=3;ui.getRange('B:B').format.columnWidth=25;ui.getRange('C:D').format.columnWidth=17;ui.getRange('E:E').format.columnWidth=3;ui.getRange('F:J').format.columnWidth=13;
title(ui,'TEAKWOOD / Label printing');put(ui,'B4','75 × 50 mm labels • Code 128');
band(ui,'B6:D6','Print selection');
const controls=[[7,'Preview record',1],[8,'First record',1],[9,'Last record',2],[10,'Copies per record',1]];
for(const [r,k,v] of controls){put(ui,`B${r}`,k);put(ui,`C${r}`,v);ui.getRange(`C${r}`).format.fill='#FFF0C2';ui.getRange(`C${r}`).dataValidation={rule:{type:'whole',operator:'between',formula1:1,formula2:100000}};}
put(ui,'B12','Records loaded');ui.getRange('C12').formulas=[['=COUNTA(Data!A2:A100001)']];put(ui,'B13','Labels in range');ui.getRange('C13').formulas=[['=(C9-C8+1)*C10']];
band(ui,'F6:J6','Actions');
band(ui,'B17:J17','How to use');
const notes=['1. Enable macros in desktop Excel, then review Settings.','2. Import a CSV or edit Data. Keep SKU Name stored as text.','3. Select the record number and click Preview label.','4. Set first / last record and copies. Print one test label first.','5. Select the installed Zebra queue and its 203 / 300 dpi resolution.','Printing uses Zebra ZPL through the Windows queue; no barcode font is needed.','Duplicate CSV rows remain separate records. Each row prints the selected copies.','The Label sheet is a preview. Use the Control buttons for exact-size printing.'];
notes.forEach((n,i)=>{ui.getRange(`B${19+i}:J${19+i}`).merge();put(ui,`B${19+i}`,n);});
ui.getRange('B29:J30').merge();put(ui,'B29','Printer not verified on this PC. Review sample-derived manufacturer details and manufacture date before printing.');ui.getRange('B29:J30').format.wrapText=true;ui.getRange('B29:J30').format.fill='#FFF0C2';
data.getRange('A1:H3').values=[['SKU Name','ItemSku','VendorName','VendorSKu','ItemSize','ItemColor','MRP',''],['02224567891016','T_BT_728_HY_BL44_A','Teakwood Leathers Belt','T_BT_728_HY_BL44_A','44','Black',2599,''],['02224567891016','T_BT_728_HY_BL44_A','Teakwood Leathers Belt','T_BT_728_HY_BL44_A','44','Black',2599,'']];
data.getRange('A:F').setNumberFormat('@');data.getRange('G:G').setNumberFormat('0.00');data.getRange('A:D').format.columnWidth=28;data.getRange('E:G').format.columnWidth=14;data.getRange('A1:G1').format={fill:'#243B35',font:{name:'Arial',bold:true,color:'#FFFFFF'}};data.freezePanes.freezeRows(1);put(data,'I1','Source: ITEMLABLE7776.csv');data.getRange('I:I').format.columnWidth=40;
title(settings,'Label and printer settings');settings.getRange('A:A').format.columnWidth=3;settings.getRange('B:B').format.columnWidth=31;settings.getRange('C:C').format.columnWidth=85;
const fields=[['Windows printer queue','ZEBRA ZD421'],['Printer resolution (dpi)',203],['Manufacture month / year','Sep 2026'],['Net quantity','1 Pcs'],['Brand','Teakwood Leathers'],['Country of origin','India'],['Manufacturer / marketer','Teakwood'],['Manufacturer address','Plot No. 56, 2nd Floor, IDC Mehrauli Road, Gurugram, Haryana 122001'],['Consumer service / packer','Teakwood'],['Consumer service address','Plot No. 56, 2nd Floor, IDC Mehrauli Road, Gurugram, Haryana 122001'],['Customer care phone','+91-9818185074'],['Customer care email','care@teakwoodleathers.com']];
settings.getRange('B5:C16').values=fields;settings.getRange('C5:C16').format.fill='#FFF0C2';settings.getRange('C6').dataValidation={rule:{type:'list',values:['203','300']}};put(settings,'B19','Source');put(settings,'C19','Shared label details transcribed from the supplied sample; review before use.');put(settings,'B21','Printer setup');put(settings,'C21','Enter the exact Windows queue name. Zebra must accept ZPL commands.');put(settings,'C22','Use 75 mm wide × 50 mm high media and calibrate the printer for its label gap.');put(settings,'C23','Choose the actual resolution. 203 dpi = 8 dots/mm; 300 dpi = 12 dots/mm.');put(settings,'C24','EasierSoft fonts/toolkit are not required by this workbook.');put(settings,'C26','Zebra specifications: https://www.zebra.com/us/en/products/printers/desktop/zd400-series/zd421.html');
label.getRange('A:A').format.columnWidth=3;label.getRange('B:K').format.columnWidth=9;title(label,'Label preview');put(label,'B4','Use Preview label after editing data or settings.');
wb.recalculate();console.log((await wb.inspect({kind:'table',range:'Control!B7:D13',include:'values,formulas',tableMaxRows:7,tableMaxCols:3})).ndjson);
console.log((await wb.inspect({kind:'match',searchTerm:'#REF!|#DIV/0!|#VALUE!|#NAME\\?|#NUM!',options:{useRegex:true,maxResults:10}})).ndjson);
await(await SpreadsheetFile.exportXlsx(wb)).save('build/base.xlsx');
for(const s of ['Control','Data','Settings']){const img=await wb.render({sheetName:s,range:s==='Control'?'A1:K31':s==='Data'?'A1:G4':'A1:D26',scale:1.3});await fs.writeFile(`build/${s}.png`,new Uint8Array(await img.arrayBuffer()));}
