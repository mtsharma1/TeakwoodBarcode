Imports System.Windows.Forms
Imports System.Drawing

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DetailsForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.detailsTable = New System.Windows.Forms.TableLayoutPanel()
        Me.lblManufacture = New System.Windows.Forms.Label()
        Me.txtManufacture = New System.Windows.Forms.TextBox()
        Me.lblNetQuantity = New System.Windows.Forms.Label()
        Me.txtNetQuantity = New System.Windows.Forms.TextBox()
        Me.lblBrand = New System.Windows.Forms.Label()
        Me.txtBrand = New System.Windows.Forms.TextBox()
        Me.lblOrigin = New System.Windows.Forms.Label()
        Me.txtOrigin = New System.Windows.Forms.TextBox()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.txtManufacturer = New System.Windows.Forms.TextBox()
        Me.lblAddress = New System.Windows.Forms.Label()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.lblPacker = New System.Windows.Forms.Label()
        Me.txtPacker = New System.Windows.Forms.TextBox()
        Me.lblPackerAddress = New System.Windows.Forms.Label()
        Me.txtPackerAddress = New System.Windows.Forms.TextBox()
        Me.lblPhone = New System.Windows.Forms.Label()
        Me.txtPhone = New System.Windows.Forms.TextBox()
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.lblNotice = New System.Windows.Forms.Label()
        Me.buttons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.detailsTable.SuspendLayout()
        Me.buttons.SuspendLayout()
        Me.SuspendLayout()
        '
        'detailsTable
        '
        Me.detailsTable.Name = "detailsTable"
        Me.detailsTable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.detailsTable.ColumnCount = 2
        Me.detailsTable.RowCount = 10
        Me.detailsTable.AutoScroll = True
        Me.detailsTable.Padding = New Padding(18)
        Me.detailsTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210!))
        Me.detailsTable.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblManufacture
        '
        Me.lblManufacture.Name = "lblManufacture"
        Me.lblManufacture.Text = "Manufacture month / year"
        Me.lblManufacture.AutoSize = True
        Me.lblManufacture.Margin = New Padding(0, 8, 10, 0)
        '
        'txtManufacture
        '
        Me.txtManufacture.Name = "txtManufacture"
        Me.txtManufacture.Dock = DockStyle.Fill
        Me.txtManufacture.Multiline = False
        Me.txtManufacture.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblManufacture , 0, 0)
        Me.detailsTable.Controls.Add(Me.txtManufacture , 1, 0)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblNetQuantity
        '
        Me.lblNetQuantity.Name = "lblNetQuantity"
        Me.lblNetQuantity.Text = "Net quantity"
        Me.lblNetQuantity.AutoSize = True
        Me.lblNetQuantity.Margin = New Padding(0, 8, 10, 0)
        '
        'txtNetQuantity
        '
        Me.txtNetQuantity.Name = "txtNetQuantity"
        Me.txtNetQuantity.Dock = DockStyle.Fill
        Me.txtNetQuantity.Multiline = False
        Me.txtNetQuantity.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblNetQuantity , 0, 1)
        Me.detailsTable.Controls.Add(Me.txtNetQuantity , 1, 1)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblBrand
        '
        Me.lblBrand.Name = "lblBrand"
        Me.lblBrand.Text = "Brand"
        Me.lblBrand.AutoSize = True
        Me.lblBrand.Margin = New Padding(0, 8, 10, 0)
        '
        'txtBrand
        '
        Me.txtBrand.Name = "txtBrand"
        Me.txtBrand.Dock = DockStyle.Fill
        Me.txtBrand.Multiline = False
        Me.txtBrand.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblBrand , 0, 2)
        Me.detailsTable.Controls.Add(Me.txtBrand , 1, 2)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblOrigin
        '
        Me.lblOrigin.Name = "lblOrigin"
        Me.lblOrigin.Text = "Country of origin"
        Me.lblOrigin.AutoSize = True
        Me.lblOrigin.Margin = New Padding(0, 8, 10, 0)
        '
        'txtOrigin
        '
        Me.txtOrigin.Name = "txtOrigin"
        Me.txtOrigin.Dock = DockStyle.Fill
        Me.txtOrigin.Multiline = False
        Me.txtOrigin.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblOrigin , 0, 3)
        Me.detailsTable.Controls.Add(Me.txtOrigin , 1, 3)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblManufacturer
        '
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Text = "Manufacturer / marketer"
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Margin = New Padding(0, 8, 10, 0)
        '
        'txtManufacturer
        '
        Me.txtManufacturer.Name = "txtManufacturer"
        Me.txtManufacturer.Dock = DockStyle.Fill
        Me.txtManufacturer.Multiline = False
        Me.txtManufacturer.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblManufacturer , 0, 4)
        Me.detailsTable.Controls.Add(Me.txtManufacturer , 1, 4)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68!))
        '
        'lblAddress
        '
        Me.lblAddress.Name = "lblAddress"
        Me.lblAddress.Text = "Manufacturer address"
        Me.lblAddress.AutoSize = True
        Me.lblAddress.Margin = New Padding(0, 8, 10, 0)
        '
        'txtAddress
        '
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Dock = DockStyle.Fill
        Me.txtAddress.Multiline = True
        Me.txtAddress.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblAddress , 0, 5)
        Me.detailsTable.Controls.Add(Me.txtAddress , 1, 5)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblPacker
        '
        Me.lblPacker.Name = "lblPacker"
        Me.lblPacker.Text = "Consumer service / packer"
        Me.lblPacker.AutoSize = True
        Me.lblPacker.Margin = New Padding(0, 8, 10, 0)
        '
        'txtPacker
        '
        Me.txtPacker.Name = "txtPacker"
        Me.txtPacker.Dock = DockStyle.Fill
        Me.txtPacker.Multiline = False
        Me.txtPacker.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblPacker , 0, 6)
        Me.detailsTable.Controls.Add(Me.txtPacker , 1, 6)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68!))
        '
        'lblPackerAddress
        '
        Me.lblPackerAddress.Name = "lblPackerAddress"
        Me.lblPackerAddress.Text = "Consumer service address"
        Me.lblPackerAddress.AutoSize = True
        Me.lblPackerAddress.Margin = New Padding(0, 8, 10, 0)
        '
        'txtPackerAddress
        '
        Me.txtPackerAddress.Name = "txtPackerAddress"
        Me.txtPackerAddress.Dock = DockStyle.Fill
        Me.txtPackerAddress.Multiline = True
        Me.txtPackerAddress.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblPackerAddress , 0, 7)
        Me.detailsTable.Controls.Add(Me.txtPackerAddress , 1, 7)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblPhone
        '
        Me.lblPhone.Name = "lblPhone"
        Me.lblPhone.Text = "Customer care phone"
        Me.lblPhone.AutoSize = True
        Me.lblPhone.Margin = New Padding(0, 8, 10, 0)
        '
        'txtPhone
        '
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Dock = DockStyle.Fill
        Me.txtPhone.Multiline = False
        Me.txtPhone.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblPhone , 0, 8)
        Me.detailsTable.Controls.Add(Me.txtPhone , 1, 8)
        Me.detailsTable.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42!))
        '
        'lblEmail
        '
        Me.lblEmail.Name = "lblEmail"
        Me.lblEmail.Text = "Customer care email"
        Me.lblEmail.AutoSize = True
        Me.lblEmail.Margin = New Padding(0, 8, 10, 0)
        '
        'txtEmail
        '
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Dock = DockStyle.Fill
        Me.txtEmail.Multiline = False
        Me.txtEmail.Margin = New Padding(0, 4, 0, 8)
        '
        'detailsTable
        '
        Me.detailsTable.Controls.Add(Me.lblEmail , 0, 9)
        Me.detailsTable.Controls.Add(Me.txtEmail , 1, 9)
        '
        'lblNotice
        '
        Me.lblNotice.Name = "lblNotice"
        Me.lblNotice.Text = "Defaults were copied from your sample. Review these details before printing."
        Me.lblNotice.AutoSize = True
        Me.lblNotice.Dock = DockStyle.Top
        Me.lblNotice.Height = 50
        Me.lblNotice.Padding = New Padding(18, 12, 10, 0)
        Me.lblNotice.ForeColor = Color.FromArgb(122, 83, 15)
        '
        'buttons
        '
        Me.buttons.Name = "buttons"
        Me.buttons.Dock = DockStyle.Bottom
        Me.buttons.Height = 56
        Me.buttons.FlowDirection = FlowDirection.RightToLeft
        Me.buttons.Padding = New Padding(10)
        '
        'btnSave
        '
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Text = "Save details"
        Me.btnSave.Dock = System.Windows.Forms.DockStyle.None
        Me.btnSave.AutoSize = True
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.BackColor = System.Drawing.Color.FromArgb(34, 67, 54)
        Me.btnSave.ForeColor = System.Drawing.Color.White
        Me.btnSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSave.Margin = New System.Windows.Forms.Padding(3)
        Me.btnSave.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnSave.Size = New System.Drawing.Size(145, 34)
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.None
        Me.btnCancel.AutoSize = True
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.BackColor = System.Drawing.Color.White
        Me.btnCancel.ForeColor = System.Drawing.Color.FromArgb(34, 67, 54)
        Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(3)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnCancel.Size = New System.Drawing.Size(145, 34)
        Me.btnCancel.UseVisualStyleBackColor = False
        Me.btnCancel.DialogResult = DialogResult.Cancel
        '
        'buttons
        '
        Me.buttons.Controls.Add(Me.btnSave)
        Me.buttons.Controls.Add(Me.btnCancel)
        '
        'Controls
        '
        Me.Controls.Add(Me.detailsTable)
        Me.Controls.Add(Me.lblNotice)
        Me.Controls.Add(Me.buttons)
        '
        'AutoScaleDimensions = New SizeF(96!, 96!)
        '
        Me.AutoScaleDimensions = New SizeF(96!, 96!)
        '
        'AutoScaleMode = AutoScaleMode
        '
        Me.AutoScaleMode = AutoScaleMode.Dpi
        '
        'ClientSize = New Size(804, 621)
        '
        Me.ClientSize = New Size(804, 621)
        '
        'MinimumSize = New Size(670, 540)
        '
        Me.MinimumSize = New Size(670, 540)
        '
        'StartPosition = FormStartPosition
        '
        Me.StartPosition = FormStartPosition.CenterParent
        '
        'Font = New Font("Segoe UI", 10!)
        '
        Me.Font = New Font("Segoe UI", 10!)
        '
        'Name = "DetailsForm"
        '
        Me.Name = "DetailsForm"
        '
        'Text = "Shared label details"
        '
        Me.Text = "Shared label details"
        '
        'AcceptButton = Me
        '
        Me.AcceptButton = Me.btnSave
        '
        'CancelButton = Me
        '
        Me.CancelButton = Me.btnCancel
        Me.detailsTable.ResumeLayout(False)
        Me.detailsTable.PerformLayout()
        Me.buttons.ResumeLayout(False)
        Me.buttons.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents detailsTable As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblManufacture As System.Windows.Forms.Label
    Friend WithEvents txtManufacture As System.Windows.Forms.TextBox
    Friend WithEvents lblNetQuantity As System.Windows.Forms.Label
    Friend WithEvents txtNetQuantity As System.Windows.Forms.TextBox
    Friend WithEvents lblBrand As System.Windows.Forms.Label
    Friend WithEvents txtBrand As System.Windows.Forms.TextBox
    Friend WithEvents lblOrigin As System.Windows.Forms.Label
    Friend WithEvents txtOrigin As System.Windows.Forms.TextBox
    Friend WithEvents lblManufacturer As System.Windows.Forms.Label
    Friend WithEvents txtManufacturer As System.Windows.Forms.TextBox
    Friend WithEvents lblAddress As System.Windows.Forms.Label
    Friend WithEvents txtAddress As System.Windows.Forms.TextBox
    Friend WithEvents lblPacker As System.Windows.Forms.Label
    Friend WithEvents txtPacker As System.Windows.Forms.TextBox
    Friend WithEvents lblPackerAddress As System.Windows.Forms.Label
    Friend WithEvents txtPackerAddress As System.Windows.Forms.TextBox
    Friend WithEvents lblPhone As System.Windows.Forms.Label
    Friend WithEvents txtPhone As System.Windows.Forms.TextBox
    Friend WithEvents lblEmail As System.Windows.Forms.Label
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents lblNotice As System.Windows.Forms.Label
    Friend WithEvents buttons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
