Attribute VB_Name = "Barcode"
Option Explicit

Private Type DOCINFO1
    pDocName As String
    pOutputFile As String
    pDatatype As String
End Type
#If VBA7 Then
Private Declare PtrSafe Function OpenPrinter Lib "winspool.drv" Alias "OpenPrinterA" (ByVal name As String, ByRef handle As LongPtr, ByVal defaults As LongPtr) As Long
Private Declare PtrSafe Function ClosePrinter Lib "winspool.drv" (ByVal handle As LongPtr) As Long
Private Declare PtrSafe Function StartDocPrinter Lib "winspool.drv" Alias "StartDocPrinterA" (ByVal handle As LongPtr, ByVal level As Long, ByRef info As DOCINFO1) As Long
Private Declare PtrSafe Function EndDocPrinter Lib "winspool.drv" (ByVal handle As LongPtr) As Long
Private Declare PtrSafe Function StartPagePrinter Lib "winspool.drv" (ByVal handle As LongPtr) As Long
Private Declare PtrSafe Function EndPagePrinter Lib "winspool.drv" (ByVal handle As LongPtr) As Long
Private Declare PtrSafe Function WritePrinter Lib "winspool.drv" (ByVal handle As LongPtr, ByRef buffer As Any, ByVal count As Long, ByRef written As Long) As Long
#Else
Private Declare Function OpenPrinter Lib "winspool.drv" Alias "OpenPrinterA" (ByVal name As String, ByRef handle As Long, ByVal defaults As Long) As Long
Private Declare Function ClosePrinter Lib "winspool.drv" (ByVal handle As Long) As Long
Private Declare Function StartDocPrinter Lib "winspool.drv" Alias "StartDocPrinterA" (ByVal handle As Long, ByVal level As Long, ByRef info As DOCINFO1) As Long
Private Declare Function EndDocPrinter Lib "winspool.drv" (ByVal handle As Long) As Long
Private Declare Function StartPagePrinter Lib "winspool.drv" (ByVal handle As Long) As Long
Private Declare Function EndPagePrinter Lib "winspool.drv" (ByVal handle As Long) As Long
Private Declare Function WritePrinter Lib "winspool.drv" (ByVal handle As Long, ByRef buffer As Any, ByVal count As Long, ByRef written As Long) As Long
#End If
Private z As String, draw As Boolean, dpmm As Long, pdfContent As String
Private Const MM As Double = 2.83464566929134
Private Const PREVIEW_SCALE As Double = 2.5

Private Function S(ByVal r As Long) As String
    S = Trim$(CStr(ThisWorkbook.Worksheets("Settings").Cells(r, 3).Value2))
End Function
Public Function RecordCount() As Long
    RecordCount = ThisWorkbook.Worksheets("Data").Cells(Rows.Count, 1).End(xlUp).Row - 1
End Function
Private Function Whole(ByVal v As Variant, ByVal title As String, ByVal maximum As Long) As Long
    If Not IsNumeric(v) Then Err.Raise 5, , title & " must be a whole number."
    If CDbl(v) < 1 Or CDbl(v) > maximum Or CDbl(v) <> Fix(CDbl(v)) Then Err.Raise 5, , title & " must be between 1 and " & maximum & "."
    Whole = CLng(v)
End Function
Private Function HexText(ByVal value As String) As String
    Dim i As Long, c As Long
    For i = 1 To Len(value)
        c = AscW(Mid$(value, i, 1))
        If c < 32 Or c > 126 Then Err.Raise 5, , "Label text must use printable English characters: " & value
        HexText = HexText & "_" & Right$("0" & Hex$(c), 2)
    Next
End Function
Private Function D(ByVal value As Double) As Long
    D = CLng(value * dpmm)
End Function
Private Function N(ByVal value As Double) As String
    N = Replace$(Format$(value, "0.000"), Application.International(xlDecimalSeparator), ".")
End Function
Private Function PdfEscape(ByVal value As String) As String
    PdfEscape = Replace$(Replace$(Replace$(value, "\", "\\"), "(", "\("), ")", "\)")
End Function
Private Sub TextLine(ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal value As String, ByVal pt As Double, Optional ByVal bold As Boolean = False)
    Dim sh As Shape, size As Long, cw As Long
    size = CLng(pt / 72 * 25.4 * dpmm)
    cw = CLng(size * 0.46)
    If Len(value) * cw > D(width) Then Err.Raise 5, , "Text is too long for this label field: " & value
    z = z & "^FO" & D(x) & "," & D(y) & "^A0N," & size & "," & cw & "^FH_^FD" & HexText(value) & "^FS" & vbCrLf
    Dim horizontalScale As Double
    horizontalScale = 100
    If Len(value) * pt * 0.55 > width * MM Then horizontalScale = width * MM / (Len(value) * pt * 0.55) * 100
    pdfContent = pdfContent & "BT /" & IIf(bold, "F2", "F1") & " " & N(pt) & " Tf " & N(horizontalScale) & " Tz " & N(x * MM) & " " & N((50 - y) * MM - pt) & " Td (" & PdfEscape(value) & ") Tj ET" & vbLf
    If Not draw Then Exit Sub
    Set sh = ThisWorkbook.Worksheets("Label").Shapes.AddTextbox(1, 20 + x * MM * PREVIEW_SCALE, 100 + y * MM * PREVIEW_SCALE, width * MM * PREVIEW_SCALE, pt * PREVIEW_SCALE * 1.6)
    With sh.TextFrame2
        .MarginLeft = 0: .MarginRight = 0: .MarginTop = 0: .MarginBottom = 0
        .WordWrap = 0
        .TextRange.Text = value
        .TextRange.Font.Name = "Arial"
        .TextRange.Font.Size = pt * PREVIEW_SCALE
        .TextRange.Font.Bold = IIf(bold, -1, 0)
        .TextRange.Font.Fill.ForeColor.RGB = RGB(0, 0, 0)
        If .TextRange.BoundWidth > width * MM * PREVIEW_SCALE + 1 Then
            .TextRange.Font.Size = .TextRange.Font.Size * width * MM * PREVIEW_SCALE / .TextRange.BoundWidth
        End If
    End With
    sh.Line.Visible = 0
    sh.Fill.Visible = 0
End Sub
Public Function CodeSymbols(ByVal value As String) As String
    Dim i As Long, total As Long, position As Long, n As Long, result As String
    If Len(value) = 0 Or Len(value) > 30 Or value Like "*[!0-9]*" Then Err.Raise 5, , "SKU Name must contain 1 to 30 digits stored as text."
    total = 105: result = "105": position = 1
    For i = 1 To Len(value) - 1 Step 2
        n = CLng(Mid$(value, i, 2))
        result = result & "," & n: total = total + position * n: position = position + 1
    Next
    If Len(value) Mod 2 = 1 Then
        result = result & ",100," & (Asc(Right$(value, 1)) - 32)
        total = total + position * 100 + (position + 1) * (Asc(Right$(value, 1)) - 32)
    End If
    CodeSymbols = result & "," & (total Mod 103) & ",106"
End Function
Private Sub Bars(ByVal value As String)
    Dim patterns As Variant, codes As Variant, item As Variant, pat As String
    Dim x As Double, j As Long, w As Double, bar As Shape
    patterns = Split("212222 222122 222221 121223 121322 131222 122213 122312 132212 221213 221312 231212 112232 122132 122231 113222 123122 123221 223211 221132 221231 213212 223112 312131 311222 321122 321221 312212 322112 322211 212123 212321 232121 111323 131123 131321 112313 132113 132311 211313 231113 231311 112133 112331 132131 113123 113321 133121 313121 211331 231131 213113 213311 213131 311123 311321 331121 312113 312311 332111 314111 221411 431111 111224 111422 121124 121421 141122 141221 112214 112412 122114 122411 142112 142211 241211 221114 413111 241112 134111 111242 121142 121241 114212 124112 124211 411212 421112 421211 212141 214121 412121 111143 111341 131141 114113 114311 411113 411311 113141 114131 311141 411131 211412 211214 211232 2331112", " ")
    codes = Split(CodeSymbols(value), ",")
    'Exact Code 128 symbols are sent as graphic bars as well as shown in Excel.
    'This preserves the same encoding and module widths in preview and print.
    x = 4.5
    For Each item In codes
        pat = patterns(CLng(item))
        For j = 1 To Len(pat)
            w = CLng(Mid$(pat, j, 1)) * 0.25
            If j Mod 2 = 1 Then
                pdfContent = pdfContent & N(x * MM) & " " & N(35 * MM) & " " & N(w * MM) & " " & N(8 * MM) & " re f" & vbLf
                z = z & "^FO" & D(x) & "," & D(7) & "^GB" & D(w) & "," & D(8) & "," & D(w) & ",B,0^FS" & vbCrLf
                If draw Then
                    Set bar = ThisWorkbook.Worksheets("Label").Shapes.AddShape(1, 20 + x * MM * PREVIEW_SCALE, 100 + 7 * MM * PREVIEW_SCALE, w * MM * PREVIEW_SCALE, 8 * MM * PREVIEW_SCALE)
                    bar.Line.Visible = 0: bar.Fill.ForeColor.RGB = RGB(0, 0, 0)
                End If
            End If
            x = x + w
        Next
    Next
    If x + 2.5 > 73 Then Err.Raise 5, , "Barcode does not fit the 75 mm label."
End Sub
Public Function Generate(ByVal record As Long, Optional ByVal showPreview As Boolean = False, Optional ByVal copies As Long = 1) As String
    Dim ws As Worksheet, r As Long, c As Long, code As String, b As Shape
    record = Whole(record, "Record", RecordCount())
    copies = Whole(copies, "Copies", 10000)
    If S(6) = "203" Then
        dpmm = 8
    ElseIf S(6) = "300" Then
        dpmm = 12
    Else
        Err.Raise 5, , "Choose 203 or 300 dpi in Settings."
    End If
    Set ws = ThisWorkbook.Worksheets("Data"): r = record + 1
    For c = 1 To 7
        If Len(Trim$(CStr(ws.Cells(r, c).Value2))) = 0 Then Err.Raise 5, , "Missing " & ws.Cells(1, c).Value2 & " in record " & record
    Next
    If VarType(ws.Cells(r, 1).Value2) <> vbString Then Err.Raise 5, , "SKU Name in record " & record & " must be text. Reimport the CSV to preserve leading zeros."
    If Not IsNumeric(ws.Cells(r, 7).Value2) Then Err.Raise 5, , "MRP is not numeric in record " & record
    If CDbl(ws.Cells(r, 7).Value2) < 0 Then Err.Raise 5, , "MRP cannot be negative."
    For c = 7 To 16
        If S(c) = "" Then Err.Raise 5, , "Complete Settings row " & c
    Next
    code = CStr(ws.Cells(r, 1).Value2)
    draw = showPreview
    pdfContent = "0 g" & vbLf
    z = "^XA^CI28^PW" & D(75) & "^LL" & D(50) & "^LH0,0^LS0^LT0^PON^FWN^PQ" & copies & vbCrLf
    If draw Then
        With ThisWorkbook.Worksheets("Label")
            Do While .Shapes.Count > 0: .Shapes(1).Delete: Loop
            Set b = .Shapes.AddShape(5, 430, 25, 120, 25)
            b.TextFrame2.TextRange.Text = "Back to controls": b.OnAction = "Home"
            Set b = .Shapes.AddShape(1, 20, 100, 75 * MM * PREVIEW_SCALE, 50 * MM * PREVIEW_SCALE)
            b.Fill.ForeColor.RGB = RGB(255, 255, 255): b.Line.ForeColor.RGB = RGB(180, 180, 180)
            .Range("B4").Value2 = "Record " & record & " / 75 x 50 mm / enlarged preview"
        End With
    End If
    TextLine 2, 2, 71, "Product: " & CStr(ws.Cells(r, 3).Value2), 8, True
    Bars code
    TextLine 4.5, 16, 68, code, 11
    TextLine 2, 21, 47, "Style Code: " & CStr(ws.Cells(r, 2).Value2), 6.4, True
    TextLine 2, 25, 47, "Month & Year of Manufacture " & S(7), 5.7, True
    TextLine 2, 29, 47, "Maximum Retail Price Rs." & Format$(CDbl(ws.Cells(r, 7).Value2), "0.00"), 6.5, True
    TextLine 17, 32, 32, "(Inclusive of all taxes)", 5.2
    TextLine 50, 21, 23, "Net Qty: " & S(8), 6
    TextLine 50, 24, 23, "Brand: " & S(9), 5.3
    TextLine 50, 27, 23, "Colour: " & CStr(ws.Cells(r, 6).Value2), 6
    TextLine 50, 30, 23, "Size: " & CStr(ws.Cells(r, 5).Value2), 6
    TextLine 50, 33, 23, "Country of Origin: " & S(10), 5.3
    TextLine 2, 35.5, 71, "Marketed and Manufactured By: " & S(11), 5.5, True
    TextLine 2, 38.4, 71, S(12), 4.8
    TextLine 2, 41.3, 71, "Consumer Service Contact and Packer: " & S(13), 5.5, True
    TextLine 2, 44.2, 71, S(14), 4.8
    TextLine 2, 47.1, 71, "Customer care: " & S(15) & " | " & S(16), 4.8
    Generate = z & "^XZ" & vbCrLf
End Function
Public Sub PreviewLabel()
    On Error GoTo failed
    Dim output As String
    output = Generate(Whole(ThisWorkbook.Worksheets("Control").Range("C7").Value2, "Preview record", RecordCount()), True)
    ThisWorkbook.Worksheets("Label").Activate
    Exit Sub
failed:
    MsgBox Err.Description, vbExclamation, "Label preview"
End Sub
Public Function BuildBatch(Optional ByVal oneLabel As Boolean = False) As String
    Dim first As Long, last As Long, copies As Long, i As Long
    With ThisWorkbook.Worksheets("Control")
        If oneLabel Then
            first = Whole(.Range("C7").Value2, "Preview record", RecordCount()): last = first: copies = 1
        Else
            first = Whole(.Range("C8").Value2, "First record", RecordCount())
            last = Whole(.Range("C9").Value2, "Last record", RecordCount())
            copies = Whole(.Range("C10").Value2, "Copies", 10000)
        End If
    End With
    If last < first Then Err.Raise 5, , "Last record must be at least the first record."
    If CDbl(last - first + 1) * copies > 10000 Then Err.Raise 5, , "Print at most 10,000 labels per batch."
    For i = first To last: BuildBatch = BuildBatch & Generate(i, False, copies): Next
End Function
Private Sub SendRaw(ByVal payload As String)
#If VBA7 Then
    Dim handle As LongPtr
#Else
    Dim handle As Long
#End If
    Dim info As DOCINFO1, bytes() As Byte, written As Long, ok As Long, started As Boolean, message As String
    On Error GoTo failed
    If OpenPrinter(S(5), handle, 0) = 0 Then Err.Raise 5, , "Printer queue not found: " & S(5) & ". Enter its exact Windows name in Settings."
    info.pDocName = "Teakwood 75x50 labels": info.pDatatype = "RAW"
    If StartDocPrinter(handle, 1, info) = 0 Then Err.Raise 5, , "Windows could not start the print job."
    started = True
    If StartPagePrinter(handle) = 0 Then Err.Raise 5, , "Windows could not start the print page."
    bytes = StrConv(payload, vbFromUnicode)
    ok = WritePrinter(handle, bytes(0), UBound(bytes) + 1, written)
    If ok = 0 Or written <> UBound(bytes) + 1 Then Err.Raise 5, , "Print transfer was incomplete. Check the queue before retrying to avoid duplicates."
    EndPagePrinter handle: EndDocPrinter handle: ClosePrinter handle
    Exit Sub
failed:
    message = Err.Description
    If handle <> 0 Then
        If started Then EndDocPrinter handle
        ClosePrinter handle
    End If
    Err.Raise 5, , message
End Sub
Private Sub PrintJob(ByVal oneLabel As Boolean)
    On Error GoTo failed
    Dim payload As String, count As Long
    payload = BuildBatch(oneLabel)
    If oneLabel Then
        count = 1
    Else
        With ThisWorkbook.Worksheets("Control"): count = (.Range("C9").Value2 - .Range("C8").Value2 + 1) * .Range("C10").Value2: End With
    End If
    If MsgBox("Send " & count & " label(s) to " & S(5) & " at " & S(6) & " dpi?" & vbCrLf & "Check 75 x 50 mm media and the manufacturer details.", vbOKCancel + vbQuestion, "Print labels") <> vbOK Then Exit Sub
    SendRaw payload
    MsgBox "Sent to the Windows print queue. Check the printer for completion.", vbInformation
    Exit Sub
failed:
    MsgBox Err.Description, vbExclamation, "Printing"
End Sub
Public Sub PrintTest(): PrintJob True: End Sub
Public Sub PrintRange(): PrintJob False: End Sub
Public Sub ExportZPL()
    On Error GoTo failed
    Dim payload As String, path As Variant, f As Integer
    payload = BuildBatch(False)
    path = Application.GetSaveAsFilename("Teakwood_labels.zpl", "Zebra print file (*.zpl),*.zpl")
    If VarType(path) = vbBoolean Then Exit Sub
    f = FreeFile: Open CStr(path) For Output As #f: Print #f, payload;: Close #f
    MsgBox "ZPL saved. It uses the resolution selected in Settings.", vbInformation
    Exit Sub
failed:
    If f <> 0 Then Close #f
    MsgBox Err.Description, vbExclamation
End Sub
Public Sub PrinterSettings(): ThisWorkbook.Worksheets("Settings").Activate: End Sub
Public Sub Home(): ThisWorkbook.Worksheets("Control").Activate: End Sub
Public Sub ShowData(): ThisWorkbook.Worksheets("Data").Activate: End Sub
Public Sub ExportAllPDF()
    On Error GoTo failed
    Dim path As Variant
    path = Application.GetSaveAsFilename("Teakwood_All_Labels.pdf", "PDF files (*.pdf),*.pdf")
    If VarType(path) = vbBoolean Then Exit Sub
    WriteAllPDF CStr(path)
    MsgBox RecordCount() & " label pages saved at 75 x 50 mm. Print at Actual size / 100%.", vbInformation
    Exit Sub
failed:
    MsgBox Err.Description, vbExclamation, "PDF export"
End Sub
Public Sub WriteAllPDF(ByVal path As String)
    Dim objects() As String, offsets() As Long, doc As String, kids As String, i As Long, k As Long, total As Long, xref As Long, f As Integer, dummy As String
    total = RecordCount()
    If total < 1 Or total > 10000 Then Err.Raise 5, , "PDF export supports 1 to 10,000 records."
    ReDim objects(1 To 4 + total * 2): ReDim offsets(1 To 4 + total * 2)
    objects(1) = "<< /Type /Catalog /Pages 2 0 R >>"
    objects(3) = "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
    objects(4) = "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"
    For i = 1 To total
        dummy = Generate(i, False, 1)
        k = 5 + (i - 1) * 2
        kids = kids & k & " 0 R "
        objects(k) = "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " & N(75 * MM) & " " & N(50 * MM) & "] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents " & k + 1 & " 0 R >>"
        objects(k + 1) = "<< /Length " & Len(pdfContent) & " >>" & vbLf & "stream" & vbLf & pdfContent & "endstream"
    Next
    objects(2) = "<< /Type /Pages /Count " & total & " /Kids [" & kids & "] >>"
    doc = "%PDF-1.4" & vbLf
    For i = 1 To UBound(objects)
        offsets(i) = Len(doc)
        doc = doc & i & " 0 obj" & vbLf & objects(i) & vbLf & "endobj" & vbLf
    Next
    xref = Len(doc)
    doc = doc & "xref" & vbLf & "0 " & UBound(objects) + 1 & vbLf & "0000000000 65535 f " & vbLf
    For i = 1 To UBound(objects): doc = doc & Format$(offsets(i), "0000000000") & " 00000 n " & vbLf: Next
    doc = doc & "trailer" & vbLf & "<< /Size " & UBound(objects) + 1 & " /Root 1 0 R >>" & vbLf & "startxref" & vbLf & xref & vbLf & "%%EOF" & vbLf
    f = FreeFile
    Open path For Output As #f: Close #f
    Open path For Binary Access Write As #f
    Put #f, , doc
    Close #f
End Sub
Public Sub ImportCSV()
    Dim path As Variant
    path = Application.GetOpenFilename("CSV files (*.csv),*.csv")
    If VarType(path) = vbBoolean Then Exit Sub
    If MsgBox("Replace the Data sheet with this CSV?", vbOKCancel + vbQuestion) <> vbOK Then Exit Sub
    On Error GoTo failed
    ImportFile CStr(path)
    MsgBox RecordCount() & " records imported. Leading zeros and duplicates preserved.", vbInformation
    Exit Sub
failed:
    MsgBox Err.Description, vbExclamation, "CSV import"
End Sub
Public Sub ImportFile(ByVal path As String)
    Dim target As Worksheet, i As Long, j As Long, n As Long, headers As Variant, values As Variant, msg As String
    Dim stream As Object, raw As String, records As Collection, fields As Collection
    On Error GoTo failed
    Set stream = CreateObject("ADODB.Stream")
    stream.Type = 2: stream.Charset = "utf-8": stream.Open: stream.LoadFromFile path
    raw = stream.ReadText: stream.Close
    If Left$(raw, 1) = ChrW$(-257) Then raw = Mid$(raw, 2)
    Set records = ParseCSV(raw)
    n = records.Count
    If n < 2 Or n > 100001 Then Err.Raise 5, , "CSV must contain 1 to 100,000 records."
    headers = Array("SKU Name", "ItemSku", "VendorName", "VendorSKu", "ItemSize", "ItemColor", "MRP")
    Set fields = records(1)
    If fields.Count <> 7 Then Err.Raise 5, , "CSV must contain exactly seven columns."
    For j = 1 To 7
        If LCase$(Trim$(fields(j))) <> LCase$(headers(j - 1)) Then Err.Raise 5, , "Expected CSV column " & j & ": " & headers(j - 1)
    Next
    ReDim values(1 To n - 1, 1 To 7)
    For i = 1 To UBound(values, 1)
        Set fields = records(i + 1)
        If fields.Count <> 7 Then Err.Raise 5, , "Expected seven fields in record " & i
        For j = 1 To 7
            values(i, j) = Trim$(CStr(fields(j)))
            If Len(values(i, j)) = 0 Then Err.Raise 5, , "Blank value in CSV record " & i & ", column " & headers(j - 1)
        Next
        msg = CodeSymbols(CStr(values(i, 1)))
        If Not IsNumeric(values(i, 7)) Then Err.Raise 5, , "Invalid MRP in CSV record " & i
        values(i, 7) = CDbl(values(i, 7))
        If values(i, 7) < 0 Then Err.Raise 5, , "Negative MRP in CSV record " & i
    Next
    Set target = ThisWorkbook.Worksheets("Data")
    target.Range("A2:G" & Application.Max(2, RecordCount() + 1)).ClearContents
    target.Range("A2:F" & n).NumberFormat = "@"
    target.Range("A2:G" & n).Value2 = values
    target.Range("G2:G" & n).NumberFormat = "0.00"
    target.Range("I1").Value2 = "Source: " & path
    With ThisWorkbook.Worksheets("Control")
        .Range("C7:C8").Value2 = 1: .Range("C9").Value2 = n - 1
    End With
    ThisWorkbook.Activate
    Exit Sub
failed:
    msg = Err.Description
    Err.Raise 5, , msg
End Sub
Private Function ParseCSV(ByVal raw As String) As Collection
    Dim result As New Collection, fields As New Collection, value As String, quoted As Boolean, i As Long, c As String
    For i = 1 To Len(raw)
        c = Mid$(raw, i, 1)
        If c = Chr$(34) Then
            If quoted And Mid$(raw, i + 1, 1) = Chr$(34) Then
                value = value & Chr$(34): i = i + 1
            Else
                quoted = Not quoted
            End If
        ElseIf c = "," And Not quoted Then
            fields.Add value: value = ""
        ElseIf (c = vbCr Or c = vbLf) And Not quoted Then
            If Len(value) > 0 Or fields.Count > 0 Then
                fields.Add value: result.Add fields: Set fields = New Collection: value = ""
            End If
            If c = vbCr And Mid$(raw, i + 1, 1) = vbLf Then i = i + 1
        Else
            value = value & c
        End If
    Next
    If quoted Then Err.Raise 5, , "CSV contains an unclosed quoted field."
    If Len(value) > 0 Or fields.Count > 0 Then fields.Add value: result.Add fields
    Set ParseCSV = result
End Function
