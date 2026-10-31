Imports System.Windows.Forms
Imports System.Drawing

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BatchPreviewForm
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
        Me.canvas = New Global.Teakwood.Barcode.LabelCanvas()
        Me.navigation = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnPrevious = New System.Windows.Forms.Button()
        Me.lblPage = New System.Windows.Forms.Label()
        Me.current = New System.Windows.Forms.NumericUpDown()
        Me.lblPageCount = New System.Windows.Forms.Label()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.navigation.SuspendLayout()
        CType(Me.current, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'canvas
        '
        Me.canvas.Name = "canvas"
        Me.canvas.Dock = DockStyle.Fill
        '
        'navigation
        '
        Me.navigation.Name = "navigation"
        Me.navigation.Dock = DockStyle.Bottom
        Me.navigation.Height = 55
        Me.navigation.Padding = New Padding(12)
        Me.navigation.BackColor = Color.White
        '
        'btnPrevious
        '
        Me.btnPrevious.Name = "btnPrevious"
        Me.btnPrevious.Text = "Previous"
        Me.btnPrevious.Dock = System.Windows.Forms.DockStyle.None
        Me.btnPrevious.AutoSize = True
        Me.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrevious.BackColor = System.Drawing.Color.White
        Me.btnPrevious.ForeColor = System.Drawing.Color.FromArgb(34, 67, 54)
        Me.btnPrevious.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnPrevious.Margin = New System.Windows.Forms.Padding(3)
        Me.btnPrevious.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnPrevious.Size = New System.Drawing.Size(145, 34)
        Me.btnPrevious.UseVisualStyleBackColor = False
        '
        'lblPage
        '
        Me.lblPage.Name = "lblPage"
        Me.lblPage.Text = "Label"
        Me.lblPage.AutoSize = True
        Me.lblPage.Padding = New Padding(0, 5, 0, 0)
        '
        'current
        '
        Me.current.Name = "current"
        Me.current.Minimum = 1D
        Me.current.Maximum = 1D
        Me.current.Value = 1D
        Me.current.Width = 90
        '
        'lblPageCount
        '
        Me.lblPageCount.Name = "lblPageCount"
        Me.lblPageCount.Text = "of 1"
        Me.lblPageCount.AutoSize = True
        Me.lblPageCount.Padding = New Padding(0, 5, 15, 0)
        '
        'btnNext
        '
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Text = "Next"
        Me.btnNext.Dock = System.Windows.Forms.DockStyle.None
        Me.btnNext.AutoSize = True
        Me.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNext.BackColor = System.Drawing.Color.White
        Me.btnNext.ForeColor = System.Drawing.Color.FromArgb(34, 67, 54)
        Me.btnNext.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNext.Margin = New System.Windows.Forms.Padding(3)
        Me.btnNext.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnNext.Size = New System.Drawing.Size(145, 34)
        Me.btnNext.UseVisualStyleBackColor = False
        '
        'navigation
        '
        Me.navigation.Controls.Add(Me.btnPrevious)
        Me.navigation.Controls.Add(Me.lblPage)
        Me.navigation.Controls.Add(Me.current)
        Me.navigation.Controls.Add(Me.lblPageCount)
        Me.navigation.Controls.Add(Me.btnNext)
        '
        'Controls
        '
        Me.Controls.Add(Me.canvas)
        Me.Controls.Add(Me.navigation)
        '
        'AutoScaleDimensions = New SizeF(96!, 96!)
        '
        Me.AutoScaleDimensions = New SizeF(96!, 96!)
        '
        'AutoScaleMode = AutoScaleMode
        '
        Me.AutoScaleMode = AutoScaleMode.Dpi
        '
        'ClientSize = New Size(944, 691)
        '
        Me.ClientSize = New Size(944, 691)
        '
        'MinimumSize = New Size(650, 500)
        '
        Me.MinimumSize = New Size(650, 500)
        '
        'StartPosition = FormStartPosition
        '
        Me.StartPosition = FormStartPosition.CenterParent
        '
        'Font = New Font("Segoe UI", 10!)
        '
        Me.Font = New Font("Segoe UI", 10!)
        '
        'Name = "BatchPreviewForm"
        '
        Me.Name = "BatchPreviewForm"
        '
        'Text = "Print preview"
        '
        Me.Text = "Print preview"
        Me.navigation.ResumeLayout(False)
        Me.navigation.PerformLayout()
        CType(Me.current, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents canvas As Global.Teakwood.Barcode.LabelCanvas
    Friend WithEvents navigation As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnPrevious As System.Windows.Forms.Button
    Friend WithEvents lblPage As System.Windows.Forms.Label
    Friend WithEvents current As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblPageCount As System.Windows.Forms.Label
    Friend WithEvents btnNext As System.Windows.Forms.Button
End Class
