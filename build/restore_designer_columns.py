from pathlib import Path
root=Path(__file__).resolve().parent.parent/'BarcodeDesktop'
p=root/'MainForm.Designer.vb'
text=p.read_text(encoding='utf-8-sig')
cols=[('colPrint','Print','Print',58,'DataGridViewCheckBoxColumn'),('colQuantity','Quantity','Qty',55,'DataGridViewTextBoxColumn'),('colRecord','Record','Row',48,'DataGridViewTextBoxColumn'),('colBarcode','Barcode','Barcode',165,'DataGridViewTextBoxColumn'),('colStyleCode','StyleCode','Style code',200,'DataGridViewTextBoxColumn'),('colProduct','Product','Product',220,'DataGridViewTextBoxColumn'),('colSize','Size','Size',55,'DataGridViewTextBoxColumn'),('colColour','Colour','Colour',75,'DataGridViewTextBoxColumn'),('colMRP','MRP','MRP (Rs.)',90,'DataGridViewTextBoxColumn'),('colVendorSku','VendorSku','Vendor SKU',160,'DataGridViewTextBoxColumn')]
anchor='        grid = New DataGridView()'
if 'colPrint = New' not in text:
    text=text.replace(anchor,anchor+'\n'+'\n'.join(f'        {n} = New {t}()' for n,_,_,_,t in cols))
props=[]
for n,prop,title,w,t in cols:
    props += [f'        {n}.Name = "{n}"',f'        {n}.HeaderText = "{title}"',f'        {n}.DataPropertyName = "{prop}"',f'        {n}.Width = {w}',f'        {n}.ReadOnly = {n not in ("colPrint","colQuantity")}',f'        {n}.SortMode = DataGridViewColumnSortMode.NotSortable']
props+=['        grid.AutoGenerateColumns = False','        grid.Columns.AddRange(New DataGridViewColumn() {'+', '.join(c[0] for c in cols)+'})']
text=text.replace('        grid.AllowUserToAddRows = False','\n'.join(props)+'\n        grid.AllowUserToAddRows = False')
p.write_text(text,encoding='utf-8')

p=root/'MainForm.vb'
text=p.read_text(encoding='utf-8-sig').replace('grid.Columns(e.ColumnIndex).Name <> "Quantity"','e.ColumnIndex <> colQuantity.Index')
text=text.replace('        settings = LabelSettings.Load()','        grid.AutoGenerateColumns = False\n        settings = LabelSettings.Load()')
p.write_text(text,encoding='utf-8')
print('Restored grid columns with matching Designer names.')
