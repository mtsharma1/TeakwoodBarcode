Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Drawing.Printing

Friend Module SelfTests
    Private Sub Check(condition As Boolean, message As String)
        If Not condition Then Throw New Exception(message)
    End Sub
    Public Sub Run(args As String())
        Dim root = If(args.Length > 0, args(0), Directory.GetCurrentDirectory())
        Dim output = Path.Combine(root, "build", "desktop-tests")
        Directory.CreateDirectory(output)
        Using main As New MainForm()
            Check(main.Controls.Find("upload", True).Length = 1, "Upload control is present before Load")
            Check(main.Controls.Find("ClearData", True).Length = 1, "Designer Clear data control")
            Check(main.Controls.Find("grid", True).Length = 1, "Designer grid")
            Check(main.grid.Columns.Count = 10 AndAlso main.colQuantity.DataPropertyName = "Quantity", "Designer preserves all ten bound columns")
        End Using
        Using details As New DetailsForm()
            Check(details.Controls.Find("txtEmail", True).Length = 1, "Designer details fields")
        End Using
        Using preview As New BatchPreviewForm()
            Check(preview.Controls.Find("current", True).Length = 1, "Designer preview navigation")
        End Using
        Dim samplePath = Path.Combine(output, "integration-sample.csv")
        File.WriteAllText(samplePath, "SKU Name,ItemSku,VendorName ,VendorSKu,ItemSize,ItemColor,MRP" & vbCrLf &
                          "02224567891016,T_BT_728_HY_BL44_A,Teakwood Leathers Belt,T_BT_728_HY_BL44_A,44,Black,2599" & vbCrLf &
                          "02224567891018,T_BT_728_HY_BL44_A,Teakwood Leathers Belt,T_BT_728_HY_BL44_A,44,Black,2599")
        Dim sheets = ImportService.Read(samplePath)
        Dim rows = ItemMapper.Map(sheets(0))
        Check(rows.Count = 2, "CSV row count")
        Check(rows(0).Barcode = "02224567891016" AndAlso rows(1).Barcode = sheets(0).Rows(2)(0), "CSV leading zeros")
        Check(String.Join(",", Code128.Encode(rows(0).Barcode)) = "105,2,22,45,67,89,10,16,38,106", "Code128 checksum")
        Check(String.Join(",", Code128.Encode("12345")) = "105,12,34,100,21,54,106", "Odd barcode encoding")
        Check(String.Join(",", Code128.Encode("207420-9")) = "105,20,74,20,100,13,25,21,106", "Code128 mixed B/C checksum")
        Dim settings As New LabelSettings
        Dim scenes = LabelEngine.Batch(rows, settings)
        Check(scenes.Count = 2, "Default batch")
        rows(0).Quantity = 3 : rows(1).Quantity = 2
        Check(LabelEngine.Batch(rows, settings).Count = 5, "Per-row quantities")
        rows(1).Print = False
        Check(LabelEngine.Batch(rows, settings).Count = 3, "Selected rows")
        rows(0).Print = False
        Dim rejected = False
        Try
            LabelEngine.Batch(rows, settings)
        Catch ex As ArgumentException
            rejected = True
        End Try
        Check(rejected, "Empty selection rejected")
        rows(0).Print = True : rows(1).Print = True : rows(0).Quantity = 1 : rows(1).Quantity = 1
        PdfExporter.Save(Path.Combine(output, "Desktop_All_Labels.pdf"), scenes)
        For Each dims In {New Single() {75, 50}, New Single() {100, 50}, New Single() {100, 75}, New Single() {50, 30}, New Single() {40, 25}, New Single() {85, 60}}
            settings.WidthMm = CDec(dims(0)) : settings.HeightMm = CDec(dims(1)) : settings.Compact = dims(0) < 75
            Dim scene = LabelEngine.Build(rows(0), settings)
            Check(scene.Width = dims(0) AndAlso scene.Height = dims(1), "Dimensions")
            Check(scene.Bars.All(Function(r) r.Left >= 0 AndAlso r.Right <= scene.Width AndAlso r.Top >= 0 AndAlso r.Bottom <= scene.Height), "Bar bounds")
            Using bmp As New Bitmap(CInt(dims(0) * 8), CInt(dims(1) * 8)), g = Graphics.FromImage(bmp)
                g.Clear(Color.White)
                LabelEngine.Draw(g, scene, New RectangleF(0, 0, bmp.Width, bmp.Height))
                bmp.Save(Path.Combine(output, $"label-{dims(0)}x{dims(1)}.png"))
            End Using
            PdfExporter.Save(Path.Combine(output, $"label-{dims(0)}x{dims(1)}.pdf"), New List(Of LabelScene) From {scene})
            For Each dpi In {203, 300}
                Using bmp As New Bitmap(CInt(dims(0) / 25.4F * dpi), CInt(dims(1) / 25.4F * dpi)), g = Graphics.FromImage(bmp)
                    bmp.SetResolution(dpi, dpi)
                    g.Clear(Color.White)
                    LabelEngine.Draw(g, scene, New RectangleF(0, 0, bmp.Width, bmp.Height), True)
                    bmp.Save(Path.Combine(output, $"printer-{dims(0)}x{dims(1)}-{dpi}.png"))
                End Using
            Next
        Next
        settings.WidthMm = 50 : settings.HeightMm = 30 : settings.Compact = False
        rejected = False
        Try
            LabelEngine.Build(rows(0), settings)
        Catch ex As ArgumentException
            rejected = True
        End Try
        Check(rejected, "Small full label rejected")
        'Exercise the existing XLSM shared strings, inline cells and source workbook data.
        Dim existing = ImportService.Read(Path.Combine(root, "outputs", "barcode", "Teakwood_Barcode.xlsm"))
        Dim data = ItemMapper.Map(existing.First(Function(s) s.Name = "Data"))
        Check(data.Count = 2 AndAlso data(0).Barcode = rows(0).Barcode, "XLSM import")
        Dim quoted = Path.Combine(output, "quoted.csv")
        File.WriteAllText(quoted, "SKU Name,ItemSku,VendorName,VendorSKu,ItemSize,ItemColor,MRP" & vbCrLf & "000123,STYLE,""Product, with comma"",STYLE,44,Black,100.50")
        Dim q = ItemMapper.Map(ImportService.Read(quoted)(0))
        Check(q(0).Barcode = "000123" AndAlso q(0).Product = "Product, with comma" AndAlso q(0).MRP = 100.5D, "Quoted CSV")
        Dim invalid = Path.Combine(output, "invalid.csv")
        File.WriteAllText(invalid, "SKU Name,ItemSku,VendorName,VendorSKu,ItemSize,ItemColor,MRP" & vbCrLf & "000123,STYLE,Product,STYLE,44,Black,-1")
        rejected = False
        Try
            ItemMapper.Map(ImportService.Read(invalid)(0))
        Catch ex As InvalidDataException
            rejected = True
        End Try
        Check(rejected, "Invalid price")
        Dim shreePath = Path.Combine(output, "shree-sample.csv")
        File.WriteAllText(shreePath, "QTY,ARTICLE NO.,BARCODE,COLOUR,MRP,SIZE,QUANTITY,PRODUCT NAME,PKD.ON,MARKETED BY,CUSTOTMER CARE NO.,Email,website,Manufactured by" & vbCrLf &
                          "700,207420,207420-9,9,""Rs. 370 (Inclusive of All Taxes)"",OS,1NO.(1PC),HARD LUGGAGE,46327,""Sreeleathers, 384 Lindsay Street, Kolkatta-700087 Sreeleathers Limited,6 Tottee Lane, Kolkatta-700016"",`03322520170,sreeleathers@sreeleathers.com,www.sreeleathers.com,""TEAKWOOD LEATHERS 56,IDC MEHRAULI ROAD GURGAON-122001 (HARYANA)""")
        Dim shree = ItemMapper.Map(ImportService.Read(shreePath)(0))
        Check(shree.Count = 1 AndAlso shree(0).LabelKind = LabelKind.ShreeLeathers, "Shree format detection")
        Check(shree(0).Quantity = 700 AndAlso shree(0).Barcode = "207420-9" AndAlso shree(0).PackedOn = "NOV 2026", "Shree field mapping")
        settings.WidthMm = 75 : settings.HeightMm = 50 : settings.Compact = False
        Dim shreeScene = LabelEngine.Build(shree(0), settings)
        Check(shreeScene.Shapes.Count > 0 AndAlso shreeScene.Texts.Any(Function(t) t.Value.Contains("ARTICLE NO:- 207420")), "Shree label design")
        Check(shreeScene.Bars.All(Function(r) r.Left >= 0 AndAlso r.Right <= shreeScene.Width AndAlso r.Top >= 0 AndAlso r.Bottom <= shreeScene.Height), "Shree barcode bounds")
        Using shreeBmp As New Bitmap(600, 400), shreeGraphics = Graphics.FromImage(shreeBmp)
            shreeGraphics.Clear(Color.White)
            LabelEngine.Draw(shreeGraphics, shreeScene, New RectangleF(0, 0, shreeBmp.Width, shreeBmp.Height))
            shreeBmp.Save(Path.Combine(output, "shree-label-75x50.png"))
        End Using
        For Each dpi In {203, 300}
            Using shreePrinterBmp As New Bitmap(CInt(75 / 25.4F * dpi), CInt(50 / 25.4F * dpi)), shreePrinterGraphics = Graphics.FromImage(shreePrinterBmp)
                shreePrinterBmp.SetResolution(dpi, dpi)
                shreePrinterGraphics.Clear(Color.White)
                LabelEngine.Draw(shreePrinterGraphics, shreeScene, New RectangleF(0, 0, shreePrinterBmp.Width, shreePrinterBmp.Height), True)
                shreePrinterBmp.Save(Path.Combine(output, $"shree-printer-75x50-{dpi}.png"))
            End Using
        Next
        PdfExporter.Save(Path.Combine(output, "shree-label-75x50.pdf"), New List(Of LabelScene) From {shreeScene})
        Dim suppliedShree = Path.Combine(Directory.GetParent(root).FullName, "SHREE LEATHERS New Barcode to be added.xlsx")
        If File.Exists(suppliedShree) Then
            Dim suppliedRows = ItemMapper.Map(ImportService.Read(suppliedShree)(0))
            Check(suppliedRows.Count = 9 AndAlso suppliedRows.Sum(Function(i) i.Quantity) = 5900, "Supplied Shree workbook import")
        End If
        Dim workbookFixture = Path.Combine(output, "import-fixture.xlsx")
        If File.Exists(workbookFixture) Then
            Dim fixture = ImportService.Read(workbookFixture)
            Check(fixture.Count = 2, "Multiple XLSX sheets")
            Dim mapped = ItemMapper.Map(fixture(1))
            Check(mapped(0).Barcode = "000123" AndAlso mapped(0).MRP = 150.25D, "XLSX zero mask and cached formula")
        End If
        If PrinterSettings.InstalledPrinters.Cast(Of String)().Contains("Microsoft Print to PDF") Then
            Using doc = LabelPrinting.CreateDocument(scenes, "Microsoft Print to PDF")
                Dim controller As New PreviewPrintController
                doc.PrintController = controller
                doc.Print()
                Dim preview = controller.GetPreviewPageInfo()
                Check(preview.Length = 2, "PrintDocument page count")
                For Each page In preview : page.Image.Dispose() : Next
            End Using
        End If
        File.WriteAllText(Path.Combine(output, "test-results.txt"), "PASS: Teakwood and Shree imports, leading zeros, Code128 B/C checksums, quantities, row selection, 6 sizes, PDF export, invalid data and layout guards.")
    End Sub
End Module
