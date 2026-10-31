Imports System.Windows.Forms
Imports System.Drawing

Public Partial Class BatchPreviewForm
    Private pages As IReadOnlyList(Of LabelScene) = Array.Empty(Of LabelScene)()

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(value As IReadOnlyList(Of LabelScene))
        Me.New()
        pages = value
    End Sub

    Private Sub BatchPreviewForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If DesignMode Then Return
        FitPreviewToScreen()
        Dim labelName = If(pages.Count > 0, pages(0).LabelName, "Barcode labels")
        Text = labelName & " — print preview — " & pages.Count & " labels"
        current.Maximum = Math.Max(1, pages.Count)
        lblPageCount.Text = "of " & pages.Count
        ShowCurrentPage()
    End Sub

    Private Sub FitPreviewToScreen()
        Dim work = Screen.FromControl(Me).WorkingArea
        MinimumSize = New Size(Math.Min(520, work.Width), Math.Min(380, work.Height))
        Dim targetWidth = Math.Min(944, CInt(work.Width * 0.88))
        Dim targetHeight = Math.Min(691, CInt(work.Height * 0.82))
        Size = New Size(Math.Max(MinimumSize.Width, targetWidth), Math.Max(MinimumSize.Height, targetHeight))
        Location = New Point(work.Left + (work.Width - Width) \ 2, work.Top + (work.Height - Height) \ 2)
    End Sub

    Private Sub Current_ValueChanged(sender As Object, e As EventArgs) Handles current.ValueChanged
        ShowCurrentPage()
    End Sub

    Private Sub Previous_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        current.Value = Math.Max(current.Minimum, current.Value - 1)
    End Sub

    Private Sub Next_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        current.Value = Math.Min(current.Maximum, current.Value + 1)
    End Sub

    Private Sub ShowCurrentPage()
        If pages.Count = 0 Then Return
        canvas.Scene = pages(CInt(current.Value) - 1)
        canvas.Invalidate()
        btnPrevious.Enabled = current.Value > 1
        btnNext.Enabled = current.Value < pages.Count
    End Sub
End Class
