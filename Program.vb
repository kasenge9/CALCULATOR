Imports System
Imports System.Windows.Forms

Namespace CalculatorApp
    Friend Module Program
        <STAThread>
        Friend Sub Main()
            ApplicationConfiguration.Initialize()
            Application.Run(New MainForm())
        End Sub
    End Module
End Namespace
