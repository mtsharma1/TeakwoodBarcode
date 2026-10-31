from pathlib import Path
from xml.sax.saxutils import escape

root=Path(__file__).resolve().parent.parent/'BarcodeDesktop'
GREEN='System.Drawing.Color.FromArgb(34, 67, 54)'
def q(s):return '"'+s.replace('"','""')+'"'
class Designer:
    def __init__(self,name):self.name=name;self.controls={};self.lines=[];self.styles=[]
    def add(self,n,t,**props):
        self.controls[n]=t
        if t in ('OpenFileDialog','SaveFileDialog'):self.prop(n,**props)
        else:self.prop(n,Name=q(n),**props)
        return n
    def prop(self,n,**p):
        for k,v in p.items():self.lines.append(f'Me.{n}.{k} = {v}')
    def line(self,s):self.lines.append(s)
    def child(self,parent,name,*pos):self.line(f'Me.{parent}.Controls.Add(Me.{name}'+(' , '+', '.join(map(str,pos)) if pos else '')+')')
    def columns(self,n,styles):
        for typ,w in styles:self.line(f'Me.{n}.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.{typ}, {w}!))')
    def rows(self,n,styles):
        for typ,h in styles:self.line(f'Me.{n}.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.{typ}, {h}!))')
    def table(self,n,cols,rows):return self.add(n,'TableLayoutPanel',Dock='System.Windows.Forms.DockStyle.Fill',ColumnCount=cols,RowCount=rows)
    def label(self,n,text,**kw):return self.add(n,'Label',Text=q(text),AutoSize='True',**kw)
    def button(self,n,text,primary=False,flow=False):
        return self.add(n,'Button',Text=q(text),Dock='System.Windows.Forms.DockStyle.'+('None' if flow else 'Fill'),AutoSize=str(flow),FlatStyle='System.Windows.Forms.FlatStyle.Flat',BackColor=GREEN if primary else 'System.Drawing.Color.White',ForeColor='System.Drawing.Color.White' if primary else GREEN,Cursor='System.Windows.Forms.Cursors.Hand',Margin='New System.Windows.Forms.Padding(3)',MinimumSize='New System.Drawing.Size(0, 32)',Size='New System.Drawing.Size(145, 34)',UseVisualStyleBackColor='False')
    def combo(self,n,values=None):
        self.add(n,'ComboBox',Dock='System.Windows.Forms.DockStyle.Fill',DropDownStyle='System.Windows.Forms.ComboBoxStyle.DropDownList',FormattingEnabled='True')
        if values:self.line(f'Me.{n}.Items.AddRange(New Object() '+'{'+', '.join(q(v) for v in values)+'})')
    def write(self):
        n=self.name
        lines=['Imports System.Windows.Forms','Imports System.Drawing','', '<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>',f'Partial Class {n}','    Inherits System.Windows.Forms.Form','', '    Private components As System.ComponentModel.IContainer','', '    <System.Diagnostics.DebuggerNonUserCode()>','    Protected Overrides Sub Dispose(disposing As Boolean)','        Try','            If disposing AndAlso components IsNot Nothing Then components.Dispose()','        Finally','            MyBase.Dispose(disposing)','        End Try','    End Sub','','    <System.Diagnostics.DebuggerStepThrough()>','    Private Sub InitializeComponent()','        Me.components = New System.ComponentModel.Container()']
        for s in self.styles:lines.append('        Dim '+s+' As New System.Windows.Forms.DataGridViewCellStyle()')
        for c,t in self.controls.items():
            typ='Global.Teakwood.Barcode.LabelCanvas' if t=='LabelCanvas' else 'System.Windows.Forms.'+t
            lines.append(f'        Me.{c} = New {typ}()')
        for c,t in self.controls.items():
            if t in ('DataGridView','NumericUpDown'):lines.append(f'        CType(Me.{c}, System.ComponentModel.ISupportInitialize).BeginInit()')
            if t in ('TableLayoutPanel','FlowLayoutPanel','TabControl','TabPage'):lines.append(f'        Me.{c}.SuspendLayout()')
        lines.append('        Me.SuspendLayout()')
        last=None
        for line in self.lines:
            field=line.split('.')[1] if line.startswith('Me.') else None
            if field and field!=last:
                lines.extend(['        \'',f'        \'{field}','        \'']);last=field
            lines.append('        '+line)
        for c,t in self.controls.items():
            if t in ('DataGridView','NumericUpDown'):lines.append(f'        CType(Me.{c}, System.ComponentModel.ISupportInitialize).EndInit()')
            if t in ('TableLayoutPanel','FlowLayoutPanel','TabControl','TabPage'):lines.extend([f'        Me.{c}.ResumeLayout(False)',f'        Me.{c}.PerformLayout()'])
        lines.extend(['        Me.ResumeLayout(False)','    End Sub',''])
        for c,t in self.controls.items():
            typ='Global.Teakwood.Barcode.LabelCanvas' if t=='LabelCanvas' else 'System.Windows.Forms.'+t
            lines.append(f'    Friend WithEvents {c} As {typ}')
        lines.append('End Class')
        (root/f'{n}.Designer.vb').write_text('\n'.join(lines)+'\n',encoding='utf-8')
        (root/f'{n}.resx').write_text('''<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, System.Windows.Forms</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, System.Windows.Forms</value></resheader>
</root>
''',encoding='utf-8')

d=Designer('MainForm')
d.table('shell',1,4);d.prop('shell',Padding='New Padding(22, 16, 22, 12)');d.rows('shell',[('Absolute',66),('Absolute',70),('Percent',100),('Absolute',45)])
d.table('header',2,1);d.columns('header',[('Percent',100),('Absolute',185)])
d.label('heading','TEAKWOOD  /  Label studio',Dock='DockStyle.Fill',Font='New Font("Segoe UI", 21!, FontStyle.Bold)',ForeColor=GREEN,TextAlign='ContentAlignment.MiddleLeft')
d.button('btnLabelDetails','Shared label details');d.child('header','heading',0,0);d.child('header','btnLabelDetails',1,0);d.child('shell','header',0,0)
d.table('fileBar',4,2);d.columns('fileBar',[('Absolute',180),('Percent',100),('Absolute',215),('Absolute',112)]);d.rows('fileBar',[('Percent',50),('Percent',50)])
d.button('upload','Upload Excel / CSV',True);d.button('ClearData','Clear data')
d.label('sourceLabel','No file uploaded',AutoEllipsis='True',Dock='DockStyle.Fill',Padding='New Padding(12, 7, 5, 0)')
d.label('fileNote','.xlsx, .xlsm and .csv • Files remain unchanged',ForeColor='Color.DimGray',Dock='DockStyle.Fill',Padding='New Padding(12, 0, 0, 0)')
d.label('lblWorksheet','Worksheet');d.combo('sheetCombo')
for c,x,y in [('upload',0,0),('sourceLabel',1,0),('fileNote',1,1),('lblWorksheet',2,0),('sheetCombo',2,1),('ClearData',3,0)]:d.child('fileBar',c,x,y)
d.line('Me.fileBar.SetRowSpan(Me.upload, 2)');d.line('Me.fileBar.SetRowSpan(Me.ClearData, 2)');d.child('shell','fileBar',0,1)
d.table('content',2,1);d.columns('content',[('Percent',100),('Absolute',440)]);d.child('shell','content',0,2)
d.table('dataArea',1,3);d.prop('dataArea',Margin='New Padding(0, 10, 18, 0)');d.rows('dataArea',[('Absolute',38),('Percent',100),('Absolute',85)])
d.label('summary','Upload a file to get started',Dock='DockStyle.Fill',Font='New Font("Segoe UI", 9.5!, FontStyle.Bold)');d.child('dataArea','summary',0,0)
d.add('tabs','TabControl',Dock='DockStyle.Fill',SelectedIndex=0)
d.add('printTab','TabPage',Text=q('Print items'),UseVisualStyleBackColor='True');d.add('rawTab','TabPage',Text=q('Uploaded worksheet'),UseVisualStyleBackColor='True')
for g in ['grid','rawGrid']:
    d.add(g,'DataGridView',Dock='DockStyle.Fill',BackgroundColor='Color.White',BorderStyle='BorderStyle.None',AllowUserToAddRows='False',AllowUserToDeleteRows='False',AllowUserToResizeRows='False',RowHeadersVisible='False',SelectionMode='DataGridViewSelectionMode.FullRowSelect',EnableHeadersVisualStyles='False',ColumnHeadersHeight=45,CellBorderStyle='DataGridViewCellBorderStyle.SingleHorizontal',GridColor='Color.FromArgb(231, 234, 231)')
    d.prop(g,**{'RowTemplate.Height':31})
    for suffix,props in [('Header',{'BackColor':GREEN,'ForeColor':'Color.White','Font':'New Font("Segoe UI", 9.5!, FontStyle.Bold)'}),('Body',{'SelectionBackColor':'Color.FromArgb(215, 233, 223)','SelectionForeColor':'Color.Black'}),('Alternate',{'BackColor':'Color.FromArgb(246, 248, 246)'})]:
        s=g+suffix+'Style';d.styles.append(s)
        for k,v in props.items():d.line(f'{s}.{k} = {v}')
        p={'Header':'ColumnHeadersDefaultCellStyle','Body':'DefaultCellStyle','Alternate':'AlternatingRowsDefaultCellStyle'}[suffix];d.prop(g,**{p:s})
d.prop('grid',AutoGenerateColumns='False');d.prop('rawGrid',ReadOnly='True',AutoSizeColumnsMode='DataGridViewAutoSizeColumnsMode.DisplayedCells')
columns=[('colPrint','Print','Print',58,'DataGridViewCheckBoxColumn'),('colQuantity','Quantity','Qty',55,'DataGridViewTextBoxColumn'),('colRecord','Record','Row',48,'DataGridViewTextBoxColumn'),('colBarcode','Barcode','Barcode',165,'DataGridViewTextBoxColumn'),('colStyleCode','StyleCode','Style code',200,'DataGridViewTextBoxColumn'),('colProduct','Product','Product',220,'DataGridViewTextBoxColumn'),('colSize','Size','Size',55,'DataGridViewTextBoxColumn'),('colColour','Colour','Colour',75,'DataGridViewTextBoxColumn'),('colMRP','MRP','MRP (Rs.)',90,'DataGridViewTextBoxColumn'),('colVendorSku','VendorSku','Vendor SKU',160,'DataGridViewTextBoxColumn')]
for n,prop,title,w,typ in columns:
    d.add(n,typ,DataPropertyName=q(prop),HeaderText=q(title),Width=w,ReadOnly=str(n not in ('colPrint','colQuantity')),SortMode='DataGridViewColumnSortMode.NotSortable')
    # Preserve column names used by validation/business code.
    d.prop(n,Name=q(prop))
d.styles.extend(['quantityStyle','priceStyle'])
d.line('quantityStyle.BackColor = Color.FromArgb(255, 244, 211)');d.line('quantityStyle.Alignment = DataGridViewContentAlignment.MiddleRight');d.prop('colQuantity',DefaultCellStyle='quantityStyle')
d.line('priceStyle.Format = "0.00"');d.line('priceStyle.Alignment = DataGridViewContentAlignment.MiddleRight');d.prop('colMRP',DefaultCellStyle='priceStyle')
d.line('Me.grid.Columns.AddRange(New DataGridViewColumn() {'+', '.join('Me.'+c[0] for c in columns)+'})')
d.child('printTab','grid');d.child('rawTab','rawGrid');d.child('tabs','printTab');d.child('tabs','rawTab');d.child('dataArea','tabs',0,1)
d.add('actions','FlowLayoutPanel',Dock='DockStyle.Fill',Padding='New Padding(0, 9, 0, 0)',WrapContents='True')
d.button('btnTickAll','Tick all',flow=True);d.button('btnClearTicks','Clear ticks',flow=True);d.label('lblQuantity','Quantity',Margin='New Padding(12, 10, 0, 0)')
d.add('quantity','NumericUpDown',Minimum='1D',Maximum='10000D',Value='1D',Width=75,Margin='New Padding(6, 7, 6, 0)');d.button('btnApplyQuantity','Apply qty to ticked',flow=True)
for n in ['btnTickAll','btnClearTicks','lblQuantity','quantity','btnApplyQuantity']:d.child('actions',n)
d.child('dataArea','actions',0,2);d.child('content','dataArea',0,0)
d.table('previewArea',1,6);d.prop('previewArea',Margin='New Padding(0, 10, 0, 0)');d.rows('previewArea',[('Absolute',34),('Percent',100),('Absolute',130),('Absolute',75),('Absolute',90),('Absolute',76)])
d.label('lblPreview','Selected row preview',Dock='DockStyle.Fill',Font='New Font("Segoe UI", 9.5!, FontStyle.Bold)');d.add('canvas','LabelCanvas',Dock='DockStyle.Fill',BackColor='Color.FromArgb(229, 234, 231)');d.child('previewArea','lblPreview',0,0);d.child('previewArea','canvas',0,1)
d.table('dimensions',4,3);d.prop('dimensions',Padding='New Padding(0, 10, 0, 0)');d.columns('dimensions',[('Absolute',70),('Percent',50),('Absolute',68),('Percent',50)]);d.rows('dimensions',[('Percent',34),('Percent',33),('Percent',33)])
d.label('lblSize','Size');d.combo('sizeCombo',['75 × 50 mm','100 × 50 mm','100 × 75 mm','50 × 30 mm','40 × 25 mm','Custom'])
d.label('lblWidth','W (mm)');d.label('lblHeight','H (mm)')
for n,minimum,default in [('labelWidth',25,75),('labelHeight',15,50)]:d.add(n,'NumericUpDown',DecimalPlaces=1,Increment='0.5D',Maximum='200D',Minimum=f'{minimum}D',Value=f'{default}D',Dock='DockStyle.Fill')
d.label('lblLayout','Layout');d.combo('layoutCombo',['Full retail details','Compact: barcode + price'])
for c,x,y in [('lblSize',0,0),('sizeCombo',1,0),('lblWidth',0,1),('labelWidth',1,1),('lblHeight',2,1),('labelHeight',3,1),('lblLayout',0,2),('layoutCombo',1,2)]:d.child('dimensions',c,x,y)
d.line('Me.dimensions.SetColumnSpan(Me.sizeCombo, 3)');d.line('Me.dimensions.SetColumnSpan(Me.layoutCombo, 3)');d.child('previewArea','dimensions',0,2)
d.table('printers',2,2);d.columns('printers',[('Percent',100),('Absolute',90)]);d.rows('printers',[('Absolute',25),('Percent',100)])
d.label('lblPrinter','Windows printer');d.combo('printerCombo');d.button('btnRefreshPrinters','Refresh')
d.child('printers','lblPrinter',0,0);d.child('printers','printerCombo',0,1);d.child('printers','btnRefreshPrinters',1,1);d.child('previewArea','printers',0,3)
d.table('commands',2,2);d.columns('commands',[('Percent',50),('Percent',50)]);d.rows('commands',[('Percent',50),('Percent',50)])
for n,t,x,y,primary in [('btnPreviewBatch','Preview print batch',0,0,False),('btnExportPdf','Export batch PDF',1,0,False),('btnTestPrint','Print 1 test label',0,1,False),('btnPrintBatch','Print batch',1,1,True)]:d.button(n,t,primary);d.child('commands',n,x,y)
d.child('previewArea','commands',0,4)
d.label('lblPrintHelp','Set the same label size in the printer driver. Compact labels omit manufacturer details. Check one printed label before a full batch.',Dock='DockStyle.Fill',ForeColor='Color.FromArgb(117, 82, 26)',Padding='New Padding(0, 8, 0, 0)');d.child('previewArea','lblPrintHelp',0,5);d.child('content','previewArea',1,0)
d.label('status','Barcodes are preserved as text. Excel does not need to be installed.',Dock='DockStyle.Fill',ForeColor='Color.DimGray',Padding='New Padding(0, 8, 0, 0)');d.child('shell','status',0,3)
d.line('Me.AutoScaleDimensions = New SizeF(96!, 96!)');d.line('Me.AutoScaleMode = AutoScaleMode.Dpi');d.line('Me.ClientSize = New Size(1464, 921)');d.line('Me.MinimumSize = New Size(1150, 820)');d.line('Me.Font = New Font("Segoe UI", 9.5!)');d.line('Me.BackColor = Color.FromArgb(247, 248, 246)');d.line('Me.StartPosition = FormStartPosition.CenterScreen');d.line('Me.Name = "MainForm"');d.line('Me.Text = "Teakwood Labels"');d.line('Me.Controls.Add(Me.shell)')
d.add('uploadDialog','OpenFileDialog',Title=q('Upload Excel or CSV data'),Filter=q('Excel / CSV (*.xlsx;*.xlsm;*.csv)|*.xlsx;*.xlsm;*.csv|All files (*.*)|*.*'))
d.add('pdfDialog','SaveFileDialog',Title=q('Export barcode labels'),Filter=q('PDF (*.pdf)|*.pdf'),FileName=q('Teakwood_Labels.pdf'))
d.write()

d=Designer('DetailsForm')
d.table('detailsTable',2,10);d.prop('detailsTable',AutoScroll='True',Padding='New Padding(18)');d.columns('detailsTable',[('Absolute',210),('Percent',100)])
fields=[('Manufacture','Manufacture month / year'),('NetQuantity','Net quantity'),('Brand','Brand'),('Origin','Country of origin'),('Manufacturer','Manufacturer / marketer'),('Address','Manufacturer address'),('Packer','Consumer service / packer'),('PackerAddress','Consumer service address'),('Phone','Customer care phone'),('Email','Customer care email')]
for i,(n,t) in enumerate(fields):
    d.rows('detailsTable',[('Absolute',68 if 'Address' in n else 42)])
    d.label('lbl'+n,t,Margin='New Padding(0, 8, 10, 0)')
    d.add('txt'+n,'TextBox',Dock='DockStyle.Fill',Multiline=str('Address' in n),Margin='New Padding(0, 4, 0, 8)')
    d.child('detailsTable','lbl'+n,0,i);d.child('detailsTable','txt'+n,1,i)
d.label('lblNotice','Defaults were copied from your sample. Review these details before printing.',Dock='DockStyle.Top',Height=50,Padding='New Padding(18, 12, 10, 0)',ForeColor='Color.FromArgb(122, 83, 15)')
d.add('buttons','FlowLayoutPanel',Dock='DockStyle.Bottom',Height=56,FlowDirection='FlowDirection.RightToLeft',Padding='New Padding(10)')
d.button('btnSave','Save details',True,True);d.button('btnCancel','Cancel',flow=True);d.prop('btnCancel',DialogResult='DialogResult.Cancel')
d.child('buttons','btnSave');d.child('buttons','btnCancel')
for n in ['detailsTable','lblNotice','buttons']:d.line(f'Me.Controls.Add(Me.{n})')
for k,v in {'AutoScaleDimensions':'New SizeF(96!, 96!)','AutoScaleMode':'AutoScaleMode.Dpi','ClientSize':'New Size(804, 621)','MinimumSize':'New Size(670, 540)','StartPosition':'FormStartPosition.CenterParent','Font':'New Font("Segoe UI", 10!)','Name':'"DetailsForm"','Text':'"Shared label details"','AcceptButton':'Me.btnSave','CancelButton':'Me.btnCancel'}.items():d.line(f'Me.{k} = {v}')
d.write()

d=Designer('BatchPreviewForm')
d.add('canvas','LabelCanvas',Dock='DockStyle.Fill');d.add('navigation','FlowLayoutPanel',Dock='DockStyle.Bottom',Height=55,Padding='New Padding(12)',BackColor='Color.White')
d.button('btnPrevious','Previous',flow=True);d.label('lblPage','Label',Padding='New Padding(0, 5, 0, 0)');d.add('current','NumericUpDown',Minimum='1D',Maximum='1D',Value='1D',Width=90);d.label('lblPageCount','of 1',Padding='New Padding(0, 5, 15, 0)');d.button('btnNext','Next',flow=True)
for n in ['btnPrevious','lblPage','current','lblPageCount','btnNext']:d.child('navigation',n)
d.line('Me.Controls.Add(Me.canvas)');d.line('Me.Controls.Add(Me.navigation)')
for k,v in {'AutoScaleDimensions':'New SizeF(96!, 96!)','AutoScaleMode':'AutoScaleMode.Dpi','ClientSize':'New Size(944, 691)','MinimumSize':'New Size(650, 500)','StartPosition':'FormStartPosition.CenterParent','Font':'New Font("Segoe UI", 10!)','Name':'"BatchPreviewForm"','Text':'"Print preview"'}.items():d.line(f'Me.{k} = {v}')
d.write()
print('Created three standard Designer files and their .resx files.')
