Imports System.Windows.Forms

Public Partial Class DetailsForm
    Private settings As New LabelSettings()

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(value As LabelSettings)
        Me.New()
        settings = value
    End Sub

    Private Sub DetailsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If DesignMode Then Return
        txtManufacture.Text = settings.Manufacture
        txtNetQuantity.Text = settings.NetQuantity
        txtBrand.Text = settings.Brand
        txtOrigin.Text = settings.Origin
        txtManufacturer.Text = settings.Manufacturer
        txtAddress.Text = settings.Address
        txtPacker.Text = settings.Packer
        txtPackerAddress.Text = settings.PackerAddress
        txtPhone.Text = settings.Phone
        txtEmail.Text = settings.Email
    End Sub

    Private Sub Save_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim fields = {txtManufacture, txtNetQuantity, txtBrand, txtOrigin, txtManufacturer, txtAddress, txtPacker, txtPackerAddress, txtPhone, txtEmail}
        If fields.Any(Function(f) String.IsNullOrWhiteSpace(f.Text)) Then
            MessageBox.Show(Me, "Complete every label detail.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        settings.Manufacture = CleanText(txtManufacture)
        settings.NetQuantity = CleanText(txtNetQuantity)
        settings.Brand = CleanText(txtBrand)
        settings.Origin = CleanText(txtOrigin)
        settings.Manufacturer = CleanText(txtManufacturer)
        settings.Address = CleanText(txtAddress)
        settings.Packer = CleanText(txtPacker)
        settings.PackerAddress = CleanText(txtPackerAddress)
        settings.Phone = CleanText(txtPhone)
        settings.Email = CleanText(txtEmail)
        DialogResult = DialogResult.OK
    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Shared Function CleanText(field As TextBox) As String
        Return field.Text.Trim().Replace(vbCr, " ").Replace(vbLf, " ")
    End Function
End Class
