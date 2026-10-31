Imports System.Windows.Forms

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
        Text = "Print preview — " & pages.Count & " labels"
        current.Maximum = Math.Max(1, pages.Count)
        lblPageCount.Text = "of " & pages.Count
        ShowCurrentPage()
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
