Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Text.Json
Imports System.Text.RegularExpressions

Public Enum LabelKind
    Teakwood
    ShreeLeathers
End Enum

Public Class ItemRow
    Public Property Print As Boolean = True
    Public Property Quantity As Integer = 1
    Public Property Record As Integer
    Public Property Barcode As String = ""
    Public Property StyleCode As String = ""
    Public Property Product As String = ""
    Public Property VendorSku As String = ""
    Public Property Size As String = ""
    Public Property Colour As String = ""
    Public Property MRP As Decimal
    Public Property LabelKind As LabelKind = LabelKind.Teakwood
    Public Property UnitQuantity As String = ""
    Public Property PackedOn As String = ""
    Public Property MarketedBy As String = ""
    Public Property CustomerCare As String = ""
    Public Property Email As String = ""
    Public Property Website As String = ""
    Public Property ManufacturedBy As String = ""
End Class

Public Class LabelSettings
    Public Property WidthMm As Decimal = 75D
    Public Property HeightMm As Decimal = 50D
    Public Property Compact As Boolean
    Public Property PrinterName As String = ""
    Public Property Manufacture As String = "Sep 2026"
    Public Property NetQuantity As String = "1 Pcs"
    Public Property Brand As String = "Teakwood Leathers"
    Public Property Origin As String = "India"
    Public Property Manufacturer As String = "Teakwood"
    Public Property Address As String = "Plot No. 56, 2nd Floor, IDC Mehrauli Road, Gurugram, Haryana 122001"
    Public Property Packer As String = "Teakwood"
    Public Property PackerAddress As String = "Plot No. 56, 2nd Floor, IDC Mehrauli Road, Gurugram, Haryana 122001"
    Public Property Phone As String = "+91-9818185074"
    Public Property Email As String = "care@teakwoodleathers.com"

    Public Shared ReadOnly Property FilePath As String
        Get
            Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TeakwoodBarcode", "settings.json")
        End Get
    End Property
    Public Shared Function Load() As LabelSettings
        Try
            If File.Exists(FilePath) Then Return If(JsonSerializer.Deserialize(Of LabelSettings)(File.ReadAllText(FilePath)), New LabelSettings())
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is JsonException OrElse TypeOf ex Is UnauthorizedAccessException
            'A missing or damaged settings file does not prevent opening the app.
        End Try
        Return New LabelSettings()
    End Function
    Public Sub Save()
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath))
        File.WriteAllText(FilePath, JsonSerializer.Serialize(Me, New JsonSerializerOptions With {.WriteIndented = True}))
    End Sub
End Class

Public Class ImportedSheet
    Public Property Name As String = ""
    Public Property Rows As New List(Of String())
End Class

Public Module ItemMapper
    Public ReadOnly Headers As String() = {"SKU Name", "ItemSku", "VendorName", "VendorSKu", "ItemSize", "ItemColor", "MRP"}
    Public ReadOnly ShreeHeaders As String() = {"QTY", "ARTICLE NO.", "BARCODE", "COLOUR", "MRP", "SIZE", "QUANTITY", "PRODUCT NAME", "PKD.ON", "MARKETED BY", "Email", "website", "Manufactured by"}

    Public Function CanMap(sheet As ImportedSheet) As Boolean
        If sheet.Rows.Count = 0 Then Return False
        Dim names = sheet.Rows(0).Select(Function(v) v.Trim()).ToArray()
        Return HasHeaders(names, Headers) OrElse HasHeaders(names, ShreeHeaders)
    End Function

    Public Function Map(sheet As ImportedSheet) As BindingList(Of ItemRow)
        If sheet.Rows.Count < 2 Then Throw New InvalidDataException("The worksheet needs a header row and at least one data row.")
        Dim names = sheet.Rows(0).Select(Function(v) v.Trim()).ToArray()
        If HasHeaders(names, ShreeHeaders) Then Return MapShree(sheet, names)
        Return MapTeakwood(sheet, names)
    End Function

    Private Function HasHeaders(names As String(), required As IEnumerable(Of String)) As Boolean
        Return required.All(Function(h) names.Any(Function(n) String.Equals(n, h, StringComparison.OrdinalIgnoreCase)))
    End Function

    Private Function MapTeakwood(sheet As ImportedSheet, names As String()) As BindingList(Of ItemRow)
        Dim indices = Headers.Select(Function(h) Array.FindIndex(names, Function(n) String.Equals(n, h, StringComparison.OrdinalIgnoreCase))).ToArray()
        Dim missing = Headers.Where(Function(h, i) indices(i) < 0)
        If missing.Any() Then Throw New InvalidDataException("Missing columns: " & String.Join(", ", missing) & ". Headers must be in the first populated row.")
        For Each h In Headers
            If names.Count(Function(n) String.Equals(n, h, StringComparison.OrdinalIgnoreCase)) > 1 Then Throw New InvalidDataException("Duplicate header: " & h)
        Next
        Dim result As New BindingList(Of ItemRow)
        For r = 1 To sheet.Rows.Count - 1
            Dim source = sheet.Rows(r)
            If source.All(Function(v) String.IsNullOrWhiteSpace(v)) Then Continue For
            Dim values = indices.Select(Function(i) If(i < source.Length, source(i).Trim(), "")).ToArray()
            For c = 0 To 6
                If values(c) = "" Then Throw New InvalidDataException($"Row {r + 1}: {Headers(c)} is blank.")
            Next
            Dim price As Decimal
            If Not Decimal.TryParse(values(6), NumberStyles.Number, CultureInfo.InvariantCulture, price) OrElse price < 0 Then Throw New InvalidDataException($"Row {r + 1}: MRP must be a non-negative number.")
            If values(0).Any(Function(c) c < "0"c OrElse c > "9"c) OrElse values(0).Length > 30 Then Throw New InvalidDataException($"Row {r + 1}: SKU Name must contain 1 to 30 digits. Store barcodes as Text in Excel.")
            result.Add(New ItemRow With {.Record = result.Count + 1, .Barcode = values(0), .StyleCode = values(1), .Product = values(2), .VendorSku = values(3), .Size = values(4), .Colour = values(5), .MRP = price})
        Next
        If result.Count = 0 Then Throw New InvalidDataException("This worksheet has no data records.")
        Return result
    End Function

    Private Function MapShree(sheet As ImportedSheet, names As String()) As BindingList(Of ItemRow)
        Dim required = ShreeHeaders.ToList()
        Dim indices = required.ToDictionary(Function(h) h, Function(h) Array.FindIndex(names, Function(n) String.Equals(n, h, StringComparison.OrdinalIgnoreCase)), StringComparer.OrdinalIgnoreCase)
        Dim phoneIndex = Array.FindIndex(names, Function(n) String.Equals(n, "CUSTOTMER CARE NO.", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(n, "CUSTOMER CARE NO.", StringComparison.OrdinalIgnoreCase))
        If phoneIndex < 0 Then Throw New InvalidDataException("Missing column: CUSTOTMER CARE NO. Headers must be in the first populated row.")
        Dim result As New BindingList(Of ItemRow)
        For r = 1 To sheet.Rows.Count - 1
            Dim source = sheet.Rows(r)
            If source.All(Function(v) String.IsNullOrWhiteSpace(v)) Then Continue For
            Dim value = Function(header As String) If(indices(header) < source.Length, source(indices(header)).Trim(), "")
            For Each header In required
                If value(header) = "" Then Throw New InvalidDataException($"Row {r + 1}: {header} is blank.")
            Next
            Dim quantity As Integer
            If Not Integer.TryParse(value("QTY"), NumberStyles.Integer, CultureInfo.InvariantCulture, quantity) OrElse quantity < 1 OrElse quantity > 10000 Then Throw New InvalidDataException($"Row {r + 1}: QTY must be a whole number from 1 to 10,000.")
            Dim barcode = value("BARCODE")
            If barcode.Length > 30 OrElse barcode.Any(Function(c) AscW(c) < 32 OrElse AscW(c) > 126) Then Throw New InvalidDataException($"Row {r + 1}: BARCODE must contain 1 to 30 printable English characters.")
            Dim priceMatch = Regex.Match(value("MRP"), "\d+(?:\.\d+)?")
            Dim price As Decimal
            If Not priceMatch.Success OrElse Not Decimal.TryParse(priceMatch.Value, NumberStyles.Number, CultureInfo.InvariantCulture, price) OrElse price < 0 Then Throw New InvalidDataException($"Row {r + 1}: MRP does not contain a valid non-negative amount.")
            Dim phone = If(phoneIndex < source.Length, source(phoneIndex).Trim().TrimStart("`"c), "")
            If phone = "" Then Throw New InvalidDataException($"Row {r + 1}: CUSTOTMER CARE NO. is blank.")
            result.Add(New ItemRow With {
                .Record = result.Count + 1,
                .Print = True,
                .Quantity = quantity,
                .Barcode = barcode,
                .StyleCode = value("ARTICLE NO."),
                .Product = value("PRODUCT NAME"),
                .VendorSku = FormatPackedOn(value("PKD.ON")),
                .Size = value("SIZE"),
                .Colour = value("COLOUR"),
                .MRP = price,
                .LabelKind = LabelKind.ShreeLeathers,
                .UnitQuantity = value("QUANTITY"),
                .PackedOn = FormatPackedOn(value("PKD.ON")),
                .MarketedBy = value("MARKETED BY"),
                .CustomerCare = phone,
                .Email = value("Email"),
                .Website = value("website"),
                .ManufacturedBy = value("Manufactured by")
            })
        Next
        If result.Count = 0 Then Throw New InvalidDataException("This worksheet has no data records.")
        Return result
    End Function

    Private Function FormatPackedOn(value As String) As String
        Dim serial As Double
        If Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, serial) AndAlso serial > 20000 AndAlso serial < 100000 Then
            Return DateTime.FromOADate(serial).ToString("MMM yyyy", CultureInfo.InvariantCulture).ToUpperInvariant()
        End If
        Dim parsed As DateTime
        If DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, parsed) OrElse DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, parsed) Then
            Return parsed.ToString("MMM yyyy", CultureInfo.InvariantCulture).ToUpperInvariant()
        End If
        Return value.ToUpperInvariant()
    End Function
End Module
