Imports System.Drawing
Imports System.Windows.Forms

Public Class DetailsForm
    Inherits Form
    Public Sub New(settings As LabelSettings)
        Text = "Shared label details"
        Size = New Size(820, 660)
        MinimumSize = New Size(670, 540)
        StartPosition = FormStartPosition.CenterParent
        Font = New Font("Segoe UI", 10)
        Dim table As New TableLayoutPanel With {.Dock = DockStyle.Fill, .AutoScroll = True, .ColumnCount = 2, .Padding = New Padding(18)}
        table.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 210))
        table.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        Dim names = {"Manufacture", "NetQuantity", "Brand", "Origin", "Manufacturer", "Address", "Packer", "PackerAddress", "Phone", "Email"}
        Dim captions = {"Manufacture month / year", "Net quantity", "Brand", "Country of origin", "Manufacturer / marketer", "Manufacturer address", "Consumer service / packer", "Consumer service address", "Customer care phone", "Customer care email"}
        Dim fields As New List(Of TextBox)
        Dim notice As New Label With {.Text = "Defaults were copied from your sample. Review these details before printing.", .Dock = DockStyle.Top, .Height = 50, .Padding = New Padding(18, 12, 10, 0), .ForeColor = Color.FromArgb(122, 83, 15)}
        For i = 0 To names.Length - 1
            table.RowStyles.Add(New RowStyle(SizeType.Absolute, If(names(i).Contains("Address"), 68, 42)))
            table.Controls.Add(New Label With {.Text = captions(i), .AutoSize = True, .Margin = New Padding(0, 8, 10, 0)}, 0, i)
            Dim box As New TextBox With {.Text = CStr(GetType(LabelSettings).GetProperty(names(i)).GetValue(settings)), .Dock = DockStyle.Fill, .Multiline = names(i).Contains("Address"), .Margin = New Padding(0, 4, 0, 8)}
            fields.Add(box) : table.Controls.Add(box, 1, i)
        Next
        Dim buttons As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 56, .FlowDirection = FlowDirection.RightToLeft, .Padding = New Padding(10)}
        Dim save As New Button With {.Text = "Save details", .AutoSize = True}
        Dim cancel As New Button With {.Text = "Cancel", .AutoSize = True, .DialogResult = DialogResult.Cancel}
        buttons.Controls.AddRange({save, cancel})
        Controls.Add(table) : Controls.Add(notice) : Controls.Add(buttons)
        AcceptButton = save : CancelButton = cancel
        AddHandler save.Click,
            Sub()
                If fields.Any(Function(f) String.IsNullOrWhiteSpace(f.Text)) Then
                    MessageBox.Show(Me, "Complete every label detail.", Text) : Return
                End If
                For i = 0 To names.Length - 1
                    GetType(LabelSettings).GetProperty(names(i)).SetValue(settings, fields(i).Text.Trim().Replace(vbCr, " ").Replace(vbLf, " "))
                Next
                DialogResult = DialogResult.OK
            End Sub
    End Sub
End Class
