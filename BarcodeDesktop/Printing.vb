Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports System.ComponentModel

Public Module LabelPrinting
    Public Function CreateDocument(pages As IReadOnlyList(Of LabelScene), printer As String) As PrintDocument
        If pages.Count = 0 Then Throw New ArgumentException("There are no labels to print.")
        Dim doc As New PrintDocument With {.DocumentName = pages(0).LabelName, .OriginAtMargins = False}
        doc.PrinterSettings.PrinterName = printer
        If Not doc.PrinterSettings.IsValid Then
            doc.Dispose()
            Throw New ArgumentException("Choose an installed Windows printer.")
        End If
        Dim width = CInt(Math.Round(pages(0).Width / 25.4 * 100))
        Dim height = CInt(Math.Round(pages(0).Height / 25.4 * 100))
        Dim paper As PaperSize = Nothing
        For Each supported As PaperSize In doc.PrinterSettings.PaperSizes
            If Math.Abs(supported.Width - width) <= 1 AndAlso Math.Abs(supported.Height - height) <= 1 Then
                paper = supported : Exit For
            End If
        Next
        If paper Is Nothing Then paper = New PaperSize($"Label {pages(0).Width} x {pages(0).Height} mm", width, height)
        doc.DefaultPageSettings.PaperSize = paper
        doc.DefaultPageSettings.Margins = New Margins(0, 0, 0, 0)
        doc.DefaultPageSettings.Landscape = False
        doc.PrinterSettings.Copies = 1
        Dim index = 0
        AddHandler doc.BeginPrint, Sub(sender, e) index = 0
        AddHandler doc.PrintPage,
            Sub(sender, e)
                If Math.Abs(e.PageBounds.Width - width) > 3 OrElse Math.Abs(e.PageBounds.Height - height) > 3 Then
                    e.Cancel = True
                    Throw New InvalidOperationException("The driver did not accept the selected label size. Create/select that size in Printer preferences, then retry.")
                End If
                Dim scene = pages(index)
                Dim g = e.Graphics
                Dim state = g.Save()
                Try
                    g.PageUnit = GraphicsUnit.Pixel
                    g.TranslateTransform(-e.PageSettings.HardMarginX / 100 * g.DpiX, -e.PageSettings.HardMarginY / 100 * g.DpiY)
                    LabelEngine.Draw(g, scene, New RectangleF(0, 0, scene.Width / 25.4F * g.DpiX, scene.Height / 25.4F * g.DpiY), True)
                Finally
                    g.Restore(state)
                End Try
                index += 1
                e.HasMorePages = index < pages.Count
            End Sub
        Return doc
    End Function
End Module

Public Class LabelCanvas
    Inherits Control
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Scene As LabelScene
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ErrorText As String = "Upload an Excel or CSV file to preview a label."
    Public Sub New()
        DoubleBuffered = True
        BackColor = Color.FromArgb(229, 234, 231)
        Dock = DockStyle.Fill
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        If Scene Is Nothing Then
            TextRenderer.DrawText(e.Graphics, ErrorText, Font, Rectangle.Inflate(ClientRectangle, -20, -20), Color.DimGray, TextFormatFlags.WordBreak Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If
        Dim available = RectangleF.Inflate(ClientRectangle, -18, -18)
        Dim factor = Math.Min(available.Width / Scene.Width, available.Height / Scene.Height)
        Dim target As New RectangleF((Width - Scene.Width * factor) / 2, (Height - Scene.Height * factor) / 2, Scene.Width * factor, Scene.Height * factor)
        e.Graphics.FillRectangle(Brushes.DarkGray, target.X + 3, target.Y + 3, target.Width, target.Height)
        LabelEngine.Draw(e.Graphics, Scene, target)
    End Sub
End Class

