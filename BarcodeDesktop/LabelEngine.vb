Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Globalization
Imports System.IO
Imports System.Text

Public Module Code128
    Public ReadOnly Patterns As String() = "212222 222122 222221 121223 121322 131222 122213 122312 132212 221213 221312 231212 112232 122132 122231 113222 123122 123221 223211 221132 221231 213212 223112 312131 311222 321122 321221 312212 322112 322211 212123 212321 232121 111323 131123 131321 112313 132113 132311 211313 231113 231311 112133 112331 132131 113123 113321 133121 313121 211331 231131 213113 213311 213131 311123 311321 331121 312113 312311 332111 314111 221411 431111 111224 111422 121124 121421 141122 141221 112214 112412 122114 122411 142112 142211 241211 221114 413111 241112 134111 111242 121142 121241 114212 124112 124211 411212 421112 421211 212141 214121 412121 111143 111341 131141 114113 114311 411113 411311 113141 114131 311141 411131 211412 211214 211232 2331112".Split(" "c)
    Public Function Encode(value As String) As List(Of Integer)
        If String.IsNullOrEmpty(value) OrElse value.Length > 30 OrElse value.Any(Function(c) AscW(c) < 32 OrElse AscW(c) > 126) Then Throw New ArgumentException("Barcode must contain 1 to 30 printable English characters.")
        If value.Any(Function(c) c < "0"c OrElse c > "9"c) Then Return EncodeMixed(value)
        Dim codes As New List(Of Integer) From {105}
        For i = 0 To value.Length - 2 Step 2
            codes.Add(Integer.Parse(value.Substring(i, 2), CultureInfo.InvariantCulture))
        Next
        If value.Length Mod 2 = 1 Then
            codes.Add(100)
            codes.Add(AscW(value.Last()) - 32)
        End If
        Dim checksum = codes(0)
        For i = 1 To codes.Count - 1 : checksum += codes(i) * i : Next
        codes.Add(checksum Mod 103)
        codes.Add(106)
        Return codes
    End Function

    Private Function EncodeMixed(value As String) As List(Of Integer)
        Dim leadingDigits = value.TakeWhile(Function(c) c >= "0"c AndAlso c <= "9"c).Count()
        Dim pairedDigits = leadingDigits - (leadingDigits Mod 2)
        If pairedDigits >= 4 Then
            Dim mixed As New List(Of Integer) From {105}
            For i = 0 To pairedDigits - 1 Step 2
                mixed.Add(Integer.Parse(value.Substring(i, 2), CultureInfo.InvariantCulture))
            Next
            If pairedDigits < value.Length Then
                mixed.Add(100)
                For Each c In value.Substring(pairedDigits) : mixed.Add(AscW(c) - 32) : Next
            End If
            Dim mixedChecksum = mixed(0)
            For i = 1 To mixed.Count - 1 : mixedChecksum += mixed(i) * i : Next
            mixed.Add(mixedChecksum Mod 103)
            mixed.Add(106)
            Return mixed
        End If
        Dim codes As New List(Of Integer) From {104}
        For Each c In value : codes.Add(AscW(c) - 32) : Next
        Dim checksum = codes(0)
        For i = 1 To codes.Count - 1 : checksum += codes(i) * i : Next
        codes.Add(checksum Mod 103)
        codes.Add(106)
        Return codes
    End Function
End Module

Public Class LabelText
    Public Property X As Single
    Public Property Y As Single
    Public Property Value As String = ""
    Public Property Points As Single
    Public Property Bold As Boolean
    Public Property ForeColor As Color = Color.Black
End Class
Public Class LabelShape
    Public Property Bounds As RectangleF
    Public Property FillColor As Color = Color.Black
End Class
Public Class LabelScene
    Public Property Width As Single
    Public Property Height As Single
    Public Property Bars As New List(Of RectangleF)
    Public Property Texts As New List(Of LabelText)
    Public Property Shapes As New List(Of LabelShape)
    Public Property ModuleWidth As Single = 0.25F
    Public Property BarcodeLeft As Single = 4.5F
    Public Property LabelName As String = "Teakwood barcode labels"
End Class

Public Module LabelEngine
    Public Function Build(item As ItemRow, settings As LabelSettings) As LabelScene
        If settings.WidthMm < 25 OrElse settings.WidthMm > 200 OrElse settings.HeightMm < 15 OrElse settings.HeightMm > 200 Then Throw New ArgumentException("Label dimensions must be 25–200 mm wide and 15–200 mm high.")
        If item.LabelKind = LabelKind.ShreeLeathers Then Return BuildShree(item, settings)
        If Not settings.Compact AndAlso (settings.WidthMm < 75 OrElse settings.HeightMm < 50) Then Throw New ArgumentException("The full retail layout needs at least 75 × 50 mm. Choose Compact for smaller labels.")
        If settings.Compact AndAlso (settings.WidthMm < 40 OrElse settings.HeightMm < 25) Then Throw New ArgumentException("The compact layout needs at least 40 × 25 mm.")
        Dim width = If(settings.Compact, 50.0F, 75.0F), height = If(settings.Compact, 30.0F, 50.0F)
        Dim scene As New LabelScene With {.Width = width, .Height = height}
        Dim price = item.MRP.ToString("0.00", CultureInfo.InvariantCulture)
        ' Keep extra clearance from the rounded label edge and the printer's right-side tolerance.
        ' Leave a full top safety band because PDF viewers and thermal drivers can clip font ascenders.
        AddText(scene, 4.5F, 4.5F, width - 9, "Product: " & item.Product, If(settings.Compact, 6.4F, 6.8F), True)
        AddBarcode(scene, item.Barcode, 4.5F, 8, width - 9, If(settings.Compact, 7.0F, 7.0F), 0.25F)
        AddText(scene, 4.5F, 16, width - 7, item.Barcode, If(settings.Compact, 9.0F, 11.0F))
        If settings.Compact Then
            AddText(scene, 4.5F, 21, 42, item.StyleCode, 6.0F, True)
            AddText(scene, 4.5F, 25, 42, $"Rs.{price}   Size: {item.Size}   {item.Colour}", 6.0F, True)
        Else
            For Each v In {settings.Manufacture, settings.NetQuantity, settings.Brand, settings.Origin, settings.Manufacturer, settings.Address, settings.Packer, settings.PackerAddress, settings.Phone, settings.Email}
                If String.IsNullOrWhiteSpace(v) Then Throw New ArgumentException("Complete the shared label details before printing the full retail label.")
            Next
            AddText(scene, 4.5F, 21, 44.5F, "Style Code: " & item.StyleCode, 6.4F, True)
            AddText(scene, 4.5F, 25, 44.5F, "Month & Year of Manufacture " & settings.Manufacture, 5.7F, True)
            AddText(scene, 4.5F, 29, 44.5F, "Maximum Retail Price Rs." & price, 6.5F, True)
            AddText(scene, 17, 32, 32, "(Inclusive of all taxes)", 5.2F, True)
            AddText(scene, 50, 21, 22, "Net Qty: " & settings.NetQuantity, 6, True)
            AddText(scene, 50, 24, 22, "Brand: " & settings.Brand, 5.3F, True)
            AddText(scene, 50, 27, 22, "Colour: " & item.Colour, 6, True)
            AddText(scene, 50, 30, 22, "Size: " & item.Size, 6, True)
            AddText(scene, 50, 33, 22, "Country of Origin: " & settings.Origin, 5.3F, True)
            AddText(scene, 4.5F, 35, 67.5F, "Marketed and Manufactured By: " & settings.Manufacturer, 5.5F, True)
            AddText(scene, 4.5F, 37.7F, 67.5F, settings.Address, 4.8F, True)
            AddText(scene, 4.5F, 40.4F, 67.5F, "Consumer Service Contact and Packer: " & settings.Packer, 5.5F, True)
            AddText(scene, 4.5F, 43.1F, 67.5F, settings.PackerAddress, 4.8F, True)
            AddText(scene, 4.5F, 45.8F, 67.5F, "Customer care: " & settings.Phone & " | " & settings.Email, 4.8F, True)
        End If
        ScaleScene(scene, settings.WidthMm, settings.HeightMm)
        Return scene
    End Function

    Private Function BuildShree(item As ItemRow, settings As LabelSettings) As LabelScene
        If settings.WidthMm < 75 OrElse settings.HeightMm < 50 Then Throw New ArgumentException("The Shree Leathers layout needs at least 75 × 50 mm.")
        Dim scene As New LabelScene With {.Width = 75, .Height = 50, .LabelName = "Shree Leathers barcode labels"}
        Dim logoColor = Color.Black

        'Vector approximation of the supplied Sreeleathers mark; it stays sharp in PDF and thermal output.
        AddShape(scene, 5.5F, 4.3F, 3.6F, 0.75F, logoColor)
        AddShape(scene, 5.5F, 5.05F, 0.9F, 1.5F, logoColor)
        AddShape(scene, 5.5F, 6.55F, 3.6F, 0.75F, logoColor)
        AddShape(scene, 8.2F, 7.3F, 0.9F, 1.5F, logoColor)
        AddShape(scene, 5.5F, 8.8F, 3.6F, 0.75F, logoColor)
        AddShape(scene, 9.7F, 4.3F, 0.9F, 5.25F, logoColor)
        AddShape(scene, 10.6F, 8.65F, 2.8F, 0.9F, logoColor)
        AddText(scene, 5.2F, 9.55F, 9.5F, "Sreeleathers", 4.1F, True, logoColor)
        AddText(scene, 18.8F, 8.5F, 10, "Kolkatta", 5.0F, True)

        AddBarcode(scene, item.Barcode, 32.5F, 5.7F, 39.5F, 4.4F, 0.39F)
        AddCenteredText(scene, 52.0F, 10.6F, 39, item.Barcode, 5.0F, True)

        Dim price = item.MRP.ToString("0.##", CultureInfo.InvariantCulture)
        Dim priceText = "MRP: Rs." & price & " (Inclusive of All Taxes)"
        Dim unit = item.UnitQuantity.Replace("NO.(", " NO.(", StringComparison.OrdinalIgnoreCase).Replace("PC)", " PC)", StringComparison.OrdinalIgnoreCase)
        Dim marketed = SplitAtPhrase(item.MarketedBy, "Sreeleathers Limited")
        Dim manufactured = SplitManufacturer(item.ManufacturedBy)

        AddText(scene, 4, 13.0F, 25, "ARTICLE NO:- " & item.StyleCode, 5.5F, True)
        AddText(scene, 4, 16.0F, 19, "COLOUR:- " & item.Colour, 5.3F, True)
        AddText(scene, 25, 16.0F, 46, priceText & "  SIZE: " & item.Size, 4.9F, True)
        AddText(scene, 4, 19.3F, 34, priceText, 4.9F, True)
        AddText(scene, 39, 19.3F, 18, "SIZE: " & item.Size, 5.1F, True)
        AddText(scene, 4, 22.9F, 31, "QUANTITY:-" & unit, 5.2F, True)
        AddText(scene, 39, 22.9F, 28, "Pkd. on: " & item.PackedOn, 5.2F, True)
        AddText(scene, 4, 26.4F, 67, "PRODUCT NAME: " & item.Product, 5.4F, True)
        AddText(scene, 4, 30.0F, 30, "MARKETED BY:", 5.2F, True)
        AddText(scene, 4, 33.0F, 67, marketed(0), 4.4F, True)
        AddText(scene, 4, 35.4F, 67, marketed(1), 4.4F, True)
        AddText(scene, 4, 37.8F, 67, "Customer Care No." & item.CustomerCare, 4.4F, True)
        AddText(scene, 4, 40.2F, 67, "Email:" & item.Email & ",Website:" & item.Website, 4.2F, True)
        AddText(scene, 4, 42.5F, 32, "MANUFACTURED BY:", 5.0F, True)
        AddText(scene, 4, 44.8F, 67, manufactured(0), 4.8F, True)
        AddText(scene, 4, 46.6F, 67, manufactured(1), 4.2F, True)

        ScaleScene(scene, settings.WidthMm, settings.HeightMm)
        Return scene
    End Function

    Private Sub ScaleScene(scene As LabelScene, targetWidth As Decimal, targetHeight As Decimal)
        Dim width = scene.Width, height = scene.Height
        Dim factor = Math.Min(CSng(targetWidth) / width, CSng(targetHeight) / height)
        Dim offsetX = (CSng(targetWidth) - width * factor) / 2
        Dim offsetY = (CSng(targetHeight) - height * factor) / 2
        scene.Width = CSng(targetWidth) : scene.Height = CSng(targetHeight)
        scene.ModuleWidth *= factor
        scene.BarcodeLeft = offsetX + scene.BarcodeLeft * factor
        scene.Bars = scene.Bars.Select(Function(r) New RectangleF(offsetX + r.X * factor, offsetY + r.Y * factor, r.Width * factor, r.Height * factor)).ToList()
        For Each shape In scene.Shapes
            Dim r = shape.Bounds
            shape.Bounds = New RectangleF(offsetX + r.X * factor, offsetY + r.Y * factor, r.Width * factor, r.Height * factor)
        Next
        For Each t In scene.Texts
            t.X = offsetX + t.X * factor : t.Y = offsetY + t.Y * factor : t.Points *= factor
        Next
    End Sub

    Private Sub AddText(scene As LabelScene, x As Single, y As Single, width As Single, value As String, pt As Single, Optional bold As Boolean = False, Optional foreColor As Color = Nothing)
        If value.Any(Function(c) Char.IsControl(c)) Then Throw New ArgumentException("A label field contains a line break or control character: " & value)
        Using bmp As New Bitmap(1, 1), g = Graphics.FromImage(bmp), fmt = CType(StringFormat.GenericTypographic.Clone(), StringFormat)
            fmt.FormatFlags = fmt.FormatFlags Or StringFormatFlags.MeasureTrailingSpaces
            Dim fits = False
            Do While pt >= 4.1F
                Using f As New Font("Arial", pt, If(bold, FontStyle.Bold, FontStyle.Regular), GraphicsUnit.Point)
                    fits = g.MeasureString(value, f, 10000, fmt).Width / g.DpiX * 25.4F <= width
                End Using
                If fits Then Exit Do
                pt -= 0.1F
            Loop
            If Not fits Then Throw New ArgumentException("Text is too long for the label: " & value & ". Shorten this field.")
        End Using
        scene.Texts.Add(New LabelText With {.X = x, .Y = y, .Value = value, .Points = pt, .Bold = bold, .ForeColor = If(foreColor.IsEmpty, Color.Black, foreColor)})
    End Sub

    Private Sub AddCenteredText(scene As LabelScene, centerX As Single, y As Single, width As Single, value As String, pt As Single, Optional bold As Boolean = False)
        Using bmp As New Bitmap(1, 1), g = Graphics.FromImage(bmp), font As New Font("Arial", pt, If(bold, FontStyle.Bold, FontStyle.Regular), GraphicsUnit.Point)
            Dim measured = g.MeasureString(value, font, 10000, StringFormat.GenericTypographic).Width / g.DpiX * 25.4F
            AddText(scene, centerX - Math.Min(measured, width) / 2, y, width, value, pt, bold)
        End Using
    End Sub

    Private Sub AddShape(scene As LabelScene, x As Single, y As Single, width As Single, height As Single, color As Color)
        scene.Shapes.Add(New LabelShape With {.Bounds = New RectangleF(x, y, width, height), .FillColor = color})
    End Sub

    Private Function SplitAtPhrase(value As String, phrase As String) As String()
        Dim at = value.IndexOf(phrase, StringComparison.OrdinalIgnoreCase)
        If at <= 0 Then Return {value, ""}
        Return {value.Substring(0, at).Trim(), value.Substring(at).Trim()}
    End Function

    Private Function SplitManufacturer(value As String) As String()
        Dim at = value.IndexOf("56,", StringComparison.OrdinalIgnoreCase)
        If at <= 0 Then Return {value, ""}
        Return {value.Substring(0, at).Trim(), value.Substring(at).Trim()}
    End Function

    Private Sub AddBarcode(scene As LabelScene, value As String, x As Single, y As Single, maxWidth As Single, height As Single, moduleWidth As Single)
        Dim symbols = Code128.Encode(value)
        Dim runs = String.Concat(symbols.Select(Function(c) Code128.Patterns(c)))
        Dim modules = runs.Sum(Function(c) AscW(c) - 48)
        If modules * moduleWidth > maxWidth Then moduleWidth = maxWidth / modules
        If moduleWidth < 0.18F Then Throw New ArgumentException("This barcode needs more horizontal space on the label.")
        scene.ModuleWidth = moduleWidth
        scene.BarcodeLeft = x
        For i = 0 To runs.Length - 1
            Dim w = (AscW(runs(i)) - 48) * moduleWidth
            If i Mod 2 = 0 Then scene.Bars.Add(New RectangleF(x, y, w, height))
            x += w
        Next
    End Sub
    Public Sub Draw(g As Graphics, scene As LabelScene, target As RectangleF, Optional snapBarcodeToPrinterDots As Boolean = False)
        Dim state = g.Save()
        Try
            Dim factor = Math.Min(target.Width / scene.Width, target.Height / scene.Height)
            Dim left = target.Left + (target.Width - scene.Width * factor) / 2
            Dim top = target.Top + (target.Height - scene.Height * factor) / 2
            g.FillRectangle(Brushes.White, left, top, scene.Width * factor, scene.Height * factor)
            g.SmoothingMode = SmoothingMode.None
            g.PixelOffsetMode = PixelOffsetMode.Half
            For Each shape In scene.Shapes
                Using brush As New SolidBrush(shape.FillColor)
                    g.FillRectangle(brush, left + shape.Bounds.X * factor, top + shape.Bounds.Y * factor, shape.Bounds.Width * factor, shape.Bounds.Height * factor)
                End Using
            Next
            Dim barcodeX = CSng(Math.Round(left + scene.BarcodeLeft * factor))
            Dim modulePixels = CSng(Math.Max(2, Math.Round(scene.ModuleWidth * factor)))
            If snapBarcodeToPrinterDots AndAlso scene.Bars.Count > 0 Then
                Dim barcodeModules = CInt(Math.Round((scene.Bars.Max(Function(r) r.Right) - scene.BarcodeLeft) / scene.ModuleWidth))
                Dim availablePixels = left + scene.Width * factor - barcodeX
                Dim maximumModulePixels = CInt(Math.Floor(availablePixels / (barcodeModules + 10)))
                If maximumModulePixels < 2 Then Throw New ArgumentException("This barcode is too wide at the printer resolution. Choose a wider label.")
                modulePixels = Math.Min(modulePixels, maximumModulePixels)
            End If
            For Each r In scene.Bars
                If snapBarcodeToPrinterDots Then
                    Dim x = barcodeX + CSng(Math.Round((r.X - scene.BarcodeLeft) / scene.ModuleWidth)) * modulePixels
                    Dim w = CSng(Math.Round(r.Width / scene.ModuleWidth)) * modulePixels
                    If x + w + 10 * modulePixels > left + scene.Width * factor Then Throw New ArgumentException("This barcode is too wide at the printer resolution. Choose a wider label.")
                    g.FillRectangle(Brushes.Black, x, CSng(Math.Round(top + r.Y * factor)), w, CSng(Math.Round(r.Height * factor)))
                Else
                    g.FillRectangle(Brushes.Black, left + r.X * factor, top + r.Y * factor, r.Width * factor, r.Height * factor)
                End If
            Next
            ' Thermal printers reproduce solid one-bit glyphs more reliably than grey anti-aliased edges.
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit
            Using fmt = CType(StringFormat.GenericTypographic.Clone(), StringFormat)
                fmt.FormatFlags = fmt.FormatFlags Or StringFormatFlags.NoWrap
                For Each t In scene.Texts
                    Using f As New Font("Arial", t.Points * 25.4F / 72 * factor, If(t.Bold, FontStyle.Bold, FontStyle.Regular), GraphicsUnit.Pixel)
                        Using brush As New SolidBrush(t.ForeColor)
                            g.DrawString(t.Value, f, brush, left + t.X * factor, top + t.Y * factor, fmt)
                        End Using
                    End Using
                Next
            End Using
        Finally
            g.Restore(state)
        End Try
    End Sub
    Public Function Batch(items As IEnumerable(Of ItemRow), settings As LabelSettings) As List(Of LabelScene)
        Dim rows = items.Where(Function(i) i.Print).ToList()
        If rows.Count = 0 Then Throw New ArgumentException("Tick at least one row in the Print column.")
        If rows.Any(Function(i) i.Quantity < 1 OrElse i.Quantity > 10000) Then Throw New ArgumentException("Each quantity must be between 1 and 10,000.")
        If rows.Sum(Function(i) CLng(i.Quantity)) > 10000 Then Throw New ArgumentException("A batch can contain up to 10,000 labels.")
        Dim result As New List(Of LabelScene)
        For Each row In rows
            Dim scene As LabelScene
            Try
                scene = Build(row, settings)
            Catch ex As ArgumentException
                Throw New ArgumentException($"Record {row.Record}: {ex.Message}", ex)
            End Try
            For copy = 1 To row.Quantity : result.Add(scene) : Next
        Next
        Return result
    End Function
End Module

Public Module PdfExporter
    Private Function N(v As Single) As String
        Return v.ToString("0.###", CultureInfo.InvariantCulture)
    End Function
    Private Function Esc(value As String) As String
        If value.Any(Function(c) AscW(c) > 126 OrElse AscW(c) < 32) Then Throw New ArgumentException("PDF export supports printable English text. Unsupported text: " & value)
        Return value.Replace("\", "\\").Replace("(", "\(").Replace(")", "\)")
    End Function
    Private Function PdfColor(value As Color) As String
        Return $"{N(value.R / 255.0F)} {N(value.G / 255.0F)} {N(value.B / 255.0F)} rg"
    End Function
    Public Sub Save(path As String, pages As IReadOnlyList(Of LabelScene))
        If pages.Count = 0 Then Throw New ArgumentException("No label pages to export.")
        Const mm As Single = 72.0F / 25.4F
        Dim objects As New List(Of String) From {"", "", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"}
        Dim kids As New List(Of String)
        For Each scene In pages
            Dim content As New StringBuilder
            For Each shape In scene.Shapes
                content.AppendLine(PdfColor(shape.FillColor))
                Dim r = shape.Bounds
                content.AppendLine($"{N(r.X * mm)} {N((scene.Height - r.Y - r.Height) * mm)} {N(r.Width * mm)} {N(r.Height * mm)} re f")
            Next
            content.AppendLine("0 0 0 rg")
            For Each r In scene.Bars
                content.AppendLine($"{N(r.X * mm)} {N((scene.Height - r.Y - r.Height) * mm)} {N(r.Width * mm)} {N(r.Height * mm)} re f")
            Next
            For Each t In scene.Texts
                content.AppendLine(PdfColor(t.ForeColor))
                content.AppendLine($"BT /{If(t.Bold, "F2", "F1")} {N(t.Points)} Tf {N(t.X * mm)} {N((scene.Height - t.Y) * mm - t.Points)} Td ({Esc(t.Value)}) Tj ET")
            Next
            Dim number = objects.Count + 1
            kids.Add($"{number} 0 R")
            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {N(scene.Width * mm)} {N(scene.Height * mm)}] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {number + 1} 0 R >>")
            objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(content.ToString())} >>" & vbLf & "stream" & vbLf & content.ToString() & "endstream")
        Next
        objects(0) = "<< /Type /Catalog /Pages 2 0 R >>"
        objects(1) = $"<< /Type /Pages /Count {pages.Count} /Kids [{String.Join(" ", kids)}] >>"
        Dim doc As New StringBuilder("%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Integer)
        For i = 0 To objects.Count - 1
            offsets.Add(doc.Length)
            doc.Append((i + 1).ToString(CultureInfo.InvariantCulture)).Append(" 0 obj").Append(vbLf).Append(objects(i)).Append(vbLf).Append("endobj").Append(vbLf)
        Next
        Dim xref = doc.Length
        doc.Append("xref").Append(vbLf).Append($"0 {objects.Count + 1}").Append(vbLf).Append("0000000000 65535 f ").Append(vbLf)
        For Each offset In offsets : doc.Append(offset.ToString("0000000000", CultureInfo.InvariantCulture)).Append(" 00000 n ").Append(vbLf) : Next
        doc.Append("trailer").Append(vbLf).Append($"<< /Size {objects.Count + 1} /Root 1 0 R >>").Append(vbLf).Append("startxref").Append(vbLf).Append(xref).Append(vbLf).Append("%%EOF").Append(vbLf)
        File.WriteAllText(path, doc.ToString(), Encoding.ASCII)
    End Sub
End Module
