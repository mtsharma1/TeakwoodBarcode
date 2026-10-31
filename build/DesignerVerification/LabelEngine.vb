Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Globalization
Imports System.IO
Imports System.Text

Public Module Code128
    Public ReadOnly Patterns As String() = "212222 222122 222221 121223 121322 131222 122213 122312 132212 221213 221312 231212 112232 122132 122231 113222 123122 123221 223211 221132 221231 213212 223112 312131 311222 321122 321221 312212 322112 322211 212123 212321 232121 111323 131123 131321 112313 132113 132311 211313 231113 231311 112133 112331 132131 113123 113321 133121 313121 211331 231131 213113 213311 213131 311123 311321 331121 312113 312311 332111 314111 221411 431111 111224 111422 121124 121421 141122 141221 112214 112412 122114 122411 142112 142211 241211 221114 413111 241112 134111 111242 121142 121241 114212 124112 124211 411212 421112 421211 212141 214121 412121 111143 111341 131141 114113 114311 411113 411311 113141 114131 311141 411131 211412 211214 211232 2331112".Split(" "c)
    Public Function Encode(value As String) As List(Of Integer)
        If String.IsNullOrEmpty(value) OrElse value.Length > 30 OrElse value.Any(Function(c) c < "0"c OrElse c > "9"c) Then Throw New ArgumentException("Barcode must contain 1 to 30 digits.")
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
End Module

Public Class LabelText
    Public Property X As Single
    Public Property Y As Single
    Public Property Value As String = ""
    Public Property Points As Single
    Public Property Bold As Boolean
End Class
Public Class LabelScene
    Public Property Width As Single
    Public Property Height As Single
    Public Property Bars As New List(Of RectangleF)
    Public Property Texts As New List(Of LabelText)
    Public Property ModuleWidth As Single = 0.25F
    Public Property BarcodeLeft As Single = 4.5F
End Class

Public Module LabelEngine
    Public Function Build(item As ItemRow, settings As LabelSettings) As LabelScene
        If settings.WidthMm < 25 OrElse settings.WidthMm > 200 OrElse settings.HeightMm < 15 OrElse settings.HeightMm > 200 Then Throw New ArgumentException("Label dimensions must be 25–200 mm wide and 15–200 mm high.")
        If Not settings.Compact AndAlso (settings.WidthMm < 75 OrElse settings.HeightMm < 50) Then Throw New ArgumentException("The full retail layout needs at least 75 × 50 mm. Choose Compact for smaller labels.")
        If settings.Compact AndAlso (settings.WidthMm < 40 OrElse settings.HeightMm < 25) Then Throw New ArgumentException("The compact layout needs at least 40 × 25 mm.")
        Dim width = If(settings.Compact, 50.0F, 75.0F), height = If(settings.Compact, 30.0F, 50.0F)
        Dim scene As New LabelScene With {.Width = width, .Height = height}
        Dim price = item.MRP.ToString("0.00", CultureInfo.InvariantCulture)
        AddText(scene, 2, 2, width - 4, "Product: " & item.Product, If(settings.Compact, 7.0F, 8.0F), True)
        AddBarcode(scene, item.Barcode, width, If(settings.Compact, 8.0F, 8.0F))
        AddText(scene, 4.5F, 16, width - 7, item.Barcode, If(settings.Compact, 9.0F, 11.0F))
        If settings.Compact Then
            AddText(scene, 2, 21, 46, item.StyleCode, 6.0F, True)
            AddText(scene, 2, 25, 46, $"Rs.{price}   Size: {item.Size}   {item.Colour}", 6.0F)
        Else
            For Each v In {settings.Manufacture, settings.NetQuantity, settings.Brand, settings.Origin, settings.Manufacturer, settings.Address, settings.Packer, settings.PackerAddress, settings.Phone, settings.Email}
                If String.IsNullOrWhiteSpace(v) Then Throw New ArgumentException("Complete the shared label details before printing the full retail label.")
            Next
            AddText(scene, 2, 21, 47, "Style Code: " & item.StyleCode, 6.4F, True)
            AddText(scene, 2, 25, 47, "Month & Year of Manufacture " & settings.Manufacture, 5.7F, True)
            AddText(scene, 2, 29, 47, "Maximum Retail Price Rs." & price, 6.5F, True)
            AddText(scene, 17, 32, 32, "(Inclusive of all taxes)", 5.2F)
            AddText(scene, 50, 21, 23, "Net Qty: " & settings.NetQuantity, 6)
            AddText(scene, 50, 24, 23, "Brand: " & settings.Brand, 5.3F)
            AddText(scene, 50, 27, 23, "Colour: " & item.Colour, 6)
            AddText(scene, 50, 30, 23, "Size: " & item.Size, 6)
            AddText(scene, 50, 33, 23, "Country of Origin: " & settings.Origin, 5.3F)
            AddText(scene, 2, 35.5F, 71, "Marketed and Manufactured By: " & settings.Manufacturer, 5.5F, True)
            AddText(scene, 2, 38.4F, 71, settings.Address, 4.8F)
            AddText(scene, 2, 41.3F, 71, "Consumer Service Contact and Packer: " & settings.Packer, 5.5F, True)
            AddText(scene, 2, 44.2F, 71, settings.PackerAddress, 4.8F)
            AddText(scene, 2, 47.1F, 71, "Customer care: " & settings.Phone & " | " & settings.Email, 4.8F)
        End If
        Dim factor = Math.Min(CSng(settings.WidthMm) / width, CSng(settings.HeightMm) / height)
        Dim offsetX = (CSng(settings.WidthMm) - width * factor) / 2
        Dim offsetY = (CSng(settings.HeightMm) - height * factor) / 2
        scene.Width = CSng(settings.WidthMm) : scene.Height = CSng(settings.HeightMm)
        scene.ModuleWidth *= factor
        scene.BarcodeLeft = offsetX + scene.BarcodeLeft * factor
        scene.Bars = scene.Bars.Select(Function(r) New RectangleF(offsetX + r.X * factor, offsetY + r.Y * factor, r.Width * factor, r.Height * factor)).ToList()
        For Each t In scene.Texts
            t.X = offsetX + t.X * factor : t.Y = offsetY + t.Y * factor : t.Points *= factor
        Next
        Return scene
    End Function
    Private Sub AddText(scene As LabelScene, x As Single, y As Single, width As Single, value As String, pt As Single, Optional bold As Boolean = False)
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
        scene.Texts.Add(New LabelText With {.X = x, .Y = y, .Value = value, .Points = pt, .Bold = bold})
    End Sub
    Private Sub AddBarcode(scene As LabelScene, value As String, width As Single, height As Single)
        Dim symbols = Code128.Encode(value)
        Dim runs = String.Concat(symbols.Select(Function(c) Code128.Patterns(c)))
        Dim moduleWidth = 0.25F
        Dim total = runs.Sum(Function(c) AscW(c) - 48) * moduleWidth
        If total + 9 > width Then Throw New ArgumentException("This barcode needs a wider label. Choose the full retail layout or a shorter barcode.")
        Dim x = 4.5F
        For i = 0 To runs.Length - 1
            Dim w = (AscW(runs(i)) - 48) * moduleWidth
            If i Mod 2 = 0 Then scene.Bars.Add(New RectangleF(x, 7, w, height))
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
            Dim modulePixels = CSng(Math.Max(2, Math.Round(scene.ModuleWidth * factor)))
            Dim barcodeX = CSng(Math.Round(left + scene.BarcodeLeft * factor))
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
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            Using fmt = CType(StringFormat.GenericTypographic.Clone(), StringFormat)
                fmt.FormatFlags = fmt.FormatFlags Or StringFormatFlags.NoWrap
                For Each t In scene.Texts
                    Using f As New Font("Arial", t.Points * 25.4F / 72 * factor, If(t.Bold, FontStyle.Bold, FontStyle.Regular), GraphicsUnit.Pixel)
                        g.DrawString(t.Value, f, Brushes.Black, left + t.X * factor, top + t.Y * factor, fmt)
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
    Public Sub Save(path As String, pages As IReadOnlyList(Of LabelScene))
        If pages.Count = 0 Then Throw New ArgumentException("No label pages to export.")
        Const mm As Single = 72.0F / 25.4F
        Dim objects As New List(Of String) From {"", "", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"}
        Dim kids As New List(Of String)
        For Each scene In pages
            Dim content As New StringBuilder("0 g" & vbLf)
            For Each r In scene.Bars
                content.AppendLine($"{N(r.X * mm)} {N((scene.Height - r.Y - r.Height) * mm)} {N(r.Width * mm)} {N(r.Height * mm)} re f")
            Next
            For Each t In scene.Texts
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
