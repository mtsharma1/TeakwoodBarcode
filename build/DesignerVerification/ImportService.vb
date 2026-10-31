Imports System.IO
Imports System.IO.Compression
Imports System.Globalization
Imports System.Xml.Linq
Imports Microsoft.VisualBasic.FileIO

Public Module ImportService
    Private ReadOnly ns As XNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main"
    Private ReadOnly rel As XNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
    Private ReadOnly packageRel As XNamespace = "http://schemas.openxmlformats.org/package/2006/relationships"

    Public Function Read(path As String) As List(Of ImportedSheet)
        Select Case IO.Path.GetExtension(path).ToLowerInvariant()
            Case ".csv" : Return New List(Of ImportedSheet) From {ReadCsv(path)}
            Case ".xlsx", ".xlsm" : Return ReadOpenXml(path)
            Case Else : Throw New InvalidDataException("Please choose an .xlsx, .xlsm or .csv file. Save legacy .xls workbooks as .xlsx first.")
        End Select
    End Function
    Private Function ReadCsv(path As String) As ImportedSheet
        Dim result As New ImportedSheet With {.Name = IO.Path.GetFileName(path)}
        Using parser As New TextFieldParser(path, Text.Encoding.UTF8, True)
            parser.SetDelimiters(",")
            parser.HasFieldsEnclosedInQuotes = True
            parser.TrimWhiteSpace = False
            While Not parser.EndOfData
                result.Rows.Add(parser.ReadFields())
                If result.Rows.Count > 100001 Then Throw New InvalidDataException("Import up to 100,000 records at a time.")
            End While
        End Using
        Return result
    End Function
    Private Function Xml(zip As ZipArchive, name As String) As XDocument
        Dim entry = zip.GetEntry(name)
        If entry Is Nothing Then Throw New InvalidDataException("Missing Excel workbook part: " & name)
        If entry.Length > 100 * 1024 * 1024 Then Throw New InvalidDataException("This workbook part exceeds the 100 MB import limit.")
        Using stream = entry.Open()
            Return XDocument.Load(stream)
        End Using
    End Function
    Private Function ReadOpenXml(path As String) As List(Of ImportedSheet)
        Using file As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), zip As New ZipArchive(file, ZipArchiveMode.Read)
            Dim sharedStrings As New List(Of String)
            If zip.GetEntry("xl/sharedStrings.xml") IsNot Nothing Then
                sharedStrings = Xml(zip, "xl/sharedStrings.xml").Descendants(ns + "si").Select(Function(si) String.Concat(si.Descendants(ns + "t").Select(Function(t) t.Value))).ToList()
            End If
            Dim formats As New Dictionary(Of Integer, String)
            Dim styleFormats As New List(Of Integer)
            If zip.GetEntry("xl/styles.xml") IsNot Nothing Then
                Dim styles = Xml(zip, "xl/styles.xml")
                For Each fmt In styles.Descendants(ns + "numFmt")
                    formats(CInt(fmt.Attribute("numFmtId").Value)) = fmt.Attribute("formatCode").Value
                Next
                Dim xfs = styles.Root.Element(ns + "cellXfs")
                If xfs IsNot Nothing Then styleFormats = xfs.Elements(ns + "xf").Select(Function(x) CInt(x.Attribute("numFmtId").Value)).ToList()
            End If
            Dim relationships = Xml(zip, "xl/_rels/workbook.xml.rels").Descendants(packageRel + "Relationship").Where(Function(x) CStr(x.Attribute("TargetMode")) <> "External").ToDictionary(Function(x) x.Attribute("Id").Value, Function(x) x.Attribute("Target").Value)
            Dim result As New List(Of ImportedSheet)
            For Each sheet In Xml(zip, "xl/workbook.xml").Descendants(ns + "sheet")
                Dim id = sheet.Attribute(rel + "id").Value
                If Not relationships.ContainsKey(id) Then Continue For
                Dim target = relationships(id)
                Dim entryPath = New Uri(New Uri("http://workbook/xl/workbook.xml"), target).AbsolutePath.TrimStart("/"c)
                Dim document = Xml(zip, entryPath)
                'Chart sheets have no cell grid.
                If document.Root.Name.LocalName <> "worksheet" Then Continue For
                Dim imported As New ImportedSheet With {.Name = sheet.Attribute("name").Value}
                For Each row In document.Descendants(ns + "sheetData").Elements(ns + "row")
                    Dim cells As New SortedDictionary(Of Integer, String)
                    For Each cell In row.Elements(ns + "c")
                        Dim address = CStr(cell.Attribute("r"))
                        Dim col = ColumnIndex(address)
                        If col >= 256 Then Throw New InvalidDataException("The import supports up to 256 columns per worksheet.")
                        Dim kind = CStr(cell.Attribute("t"))
                        Dim raw = If(cell.Element(ns + "v")?.Value, "")
                        If cell.Element(ns + "f") IsNot Nothing AndAlso cell.Element(ns + "v") Is Nothing Then Throw New InvalidDataException($"{imported.Name}!{address}: formula has no saved value. Recalculate and save it in Excel first.")
                        If kind = "s" Then
                            Dim index As Integer
                            If Not Integer.TryParse(raw, index) OrElse index < 0 OrElse index >= sharedStrings.Count Then Throw New InvalidDataException("Invalid shared string reference at " & address)
                            raw = sharedStrings(index)
                        ElseIf kind = "inlineStr" Then
                            raw = String.Concat(cell.Descendants(ns + "t").Select(Function(t) t.Value))
                        ElseIf kind = "e" Then
                            Throw New InvalidDataException($"{imported.Name}!{address} contains Excel error {raw}.")
                        ElseIf kind <> "str" AndAlso raw <> "" Then
                            Dim number As Decimal
                            If Decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, number) Then
                                raw = number.ToString("0.############################", CultureInfo.InvariantCulture)
                                Dim style As Integer
                                If Integer.TryParse(CStr(cell.Attribute("s")), style) AndAlso style < styleFormats.Count AndAlso formats.ContainsKey(styleFormats(style)) Then
                                    Dim fmt = formats(styleFormats(style)).Split(";"c)(0)
                                    If fmt.Length <= 30 AndAlso fmt.Length > 1 AndAlso fmt.All(Function(c) c = "0"c) AndAlso Decimal.Truncate(number) = number AndAlso number >= 0 Then raw = raw.PadLeft(fmt.Length, "0"c)
                                End If
                            End If
                        End If
                        cells(col) = raw
                    Next
                    If cells.Count = 0 OrElse cells.Values.All(Function(v) String.IsNullOrWhiteSpace(v)) Then Continue For
                    Dim values(cells.Keys.Max()) As String
                    Array.Fill(values, "")
                    For Each pair In cells : values(pair.Key) = pair.Value : Next
                    imported.Rows.Add(values)
                    If imported.Rows.Count > 100001 Then Throw New InvalidDataException("Import up to 100,000 records at a time.")
                Next
                result.Add(imported)
            Next
            If result.Count = 0 Then Throw New InvalidDataException("No worksheets found in this workbook.")
            Return result
        End Using
    End Function
    Private Function ColumnIndex(address As String) As Integer
        If String.IsNullOrEmpty(address) Then Throw New InvalidDataException("A cell is missing its address.")
        Dim n = 0
        For Each c In address
            If c < "A"c OrElse c > "Z"c Then Exit For
            n = n * 26 + AscW(c) - AscW("A"c) + 1
        Next
        If n = 0 Then Throw New InvalidDataException("Invalid cell address: " & address)
        Return n - 1
    End Function
End Module
