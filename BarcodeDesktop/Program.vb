Imports System.IO
Imports System.Windows.Forms

Friend Module Program
    <STAThread>
    Public Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        If args.Length > 0 AndAlso args(0) = "--self-test" Then
            Try
                SelfTests.Run(args.Skip(1).ToArray())
                Environment.ExitCode = 0
            Catch ex As Exception
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "test-error.txt"), ex.ToString())
                Environment.ExitCode = 1
            End Try
            Return
        End If
        AddHandler Application.ThreadException, Sub(sender, e) MessageBox.Show(e.Exception.Message, "Teakwood Labels", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Application.Run(New MainForm(If(args.Length > 0, args(0), Nothing)))
    End Sub
End Module
