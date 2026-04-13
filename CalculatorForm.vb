Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Class CalculatorForm
    Inherits Form

    Private firstNumber As Double
    Private operatorSelected As String = String.Empty
    Private secondNumber As Double
    Private result As Double

    Private ReadOnly DisplayTextBox As TextBox

    Private WithEvents Button0 As Button
    Private WithEvents Button1 As Button
    Private WithEvents Button2 As Button
    Private WithEvents Button3 As Button
    Private WithEvents Button4 As Button
    Private WithEvents Button5 As Button
    Private WithEvents Button6 As Button
    Private WithEvents Button7 As Button
    Private WithEvents Button8 As Button
    Private WithEvents Button9 As Button

    Private WithEvents PlusButton As Button
    Private WithEvents MinusButton As Button
    Private WithEvents MultiplyButton As Button
    Private WithEvents DivideButton As Button
    Private WithEvents EqualsButton As Button
    Private WithEvents ClearButton As Button

    Public Sub New()
        Text = "Calculator"
        StartPosition = FormStartPosition.CenterScreen
        ClientSize = New Size(320, 460)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False

        DisplayTextBox = New TextBox() With {
            .Name = "DisplayTextBox",
            .ReadOnly = False,
            .Location = New Point(16, 16),
            .Size = New Size(288, 44),
            .Font = New Font("Segoe UI", 20.0F, FontStyle.Regular),
            .TextAlign = HorizontalAlignment.Right
        }
        Controls.Add(DisplayTextBox)

        InitializeButtons()
    End Sub

    Private Sub InitializeButtons()
        Dim labels As String(,) = {
            {"7", "8", "9", "/"},
            {"4", "5", "6", "*"},
            {"1", "2", "3", "-"},
            {"C", "0", "=", "+"}
        }

        Dim startX As Integer = 16
        Dim startY As Integer = 80
        Dim buttonWidth As Integer = 66
        Dim buttonHeight As Integer = 66
        Dim gap As Integer = 8

        For row As Integer = 0 To 3
            For col As Integer = 0 To 3
                Dim text As String = labels(row, col)
                Dim btn As New Button() With {
                    .Text = text,
                    .Size = New Size(buttonWidth, buttonHeight),
                    .Location = New Point(startX + col * (buttonWidth + gap), startY + row * (buttonHeight + gap)),
                    .Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
                }

                Controls.Add(btn)

                Select Case text
                    Case "0" : Button0 = btn
                    Case "1" : Button1 = btn
                    Case "2" : Button2 = btn
                    Case "3" : Button3 = btn
                    Case "4" : Button4 = btn
                    Case "5" : Button5 = btn
                    Case "6" : Button6 = btn
                    Case "7" : Button7 = btn
                    Case "8" : Button8 = btn
                    Case "9" : Button9 = btn
                    Case "+" : PlusButton = btn
                    Case "-" : MinusButton = btn
                    Case "*" : MultiplyButton = btn
                    Case "/" : DivideButton = btn
                    Case "=" : EqualsButton = btn
                    Case "C" : ClearButton = btn
                End Select
            Next
        Next
    End Sub

    ' Number buttons (0–9)
    Private Sub NumericButton_Click(sender As Object, e As EventArgs) _
        Handles Button0.Click, Button1.Click, Button2.Click, Button3.Click,
                Button4.Click, Button5.Click, Button6.Click, Button7.Click,
                Button8.Click, Button9.Click

        Dim button As Button = DirectCast(sender, Button)
        DisplayTextBox.Text &= button.Text
    End Sub

    ' Operator buttons
    Private Sub OperatorButton_Click(sender As Object, e As EventArgs) _
        Handles PlusButton.Click, MinusButton.Click, MultiplyButton.Click, DivideButton.Click

        Dim button As Button = DirectCast(sender, Button)
        operatorSelected = button.Text

        If Double.TryParse(DisplayTextBox.Text, firstNumber) Then
            DisplayTextBox.Clear()
        Else
            DisplayTextBox.Text = "Invalid input"
        End If
    End Sub

    ' Equals button
    Private Sub EqualsButton_Click(sender As Object, e As EventArgs) _
        Handles EqualsButton.Click

        If Double.TryParse(DisplayTextBox.Text, secondNumber) Then
            Select Case operatorSelected
                Case "+"
                    result = firstNumber + secondNumber
                Case "-"
                    result = firstNumber - secondNumber
                Case "*"
                    result = firstNumber * secondNumber
                Case "/"
                    If secondNumber = 0 Then
                        DisplayTextBox.Text = "Error"
                        Exit Sub
                    Else
                        result = firstNumber / secondNumber
                    End If
            End Select

            DisplayTextBox.Text = result.ToString()
        Else
            DisplayTextBox.Text = "Invalid input"
        End If
    End Sub

    ' Clear button
    Private Sub ClearButton_Click(sender As Object, e As EventArgs) _
        Handles ClearButton.Click

        DisplayTextBox.Clear()
        firstNumber = 0
        secondNumber = 0
        result = 0
        operatorSelected = String.Empty
    End Sub
End Class
