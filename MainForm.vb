Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.Windows.Forms

Namespace CalculatorApp
    Public Class MainForm
        Inherits Form

        Private ReadOnly _display As TextBox
        Private ReadOnly _titleLabel As Label

        Private _firstOperand As Decimal = 0D
        Private _pendingOperator As String = String.Empty
        Private _isEnteringNewNumber As Boolean = True

        Public Sub New()
            Text = "Advanced Calculator"
            MinimumSize = New Size(380, 600)
            StartPosition = FormStartPosition.CenterScreen
            KeyPreview = True

            ApplyDarkTheme()

            _titleLabel = BuildTitleLabel()
            _display = BuildDisplayTextBox()

            Controls.Add(_titleLabel)
            Controls.Add(_display)
            InitializeButtons()

            AddHandler KeyDown, AddressOf OnCalculatorKeyDown
        End Sub

        ' Applies a consistent dark theme and typography.
        Private Sub ApplyDarkTheme()
            BackColor = Color.FromArgb(15, 15, 18)
            ForeColor = Color.White
            Font = New Font("Segoe UI", 12.0F, FontStyle.Regular)
        End Sub

        ' Builds the small heading above the calculator display.
        Private Function BuildTitleLabel() As Label
            Return New Label() With {
                .Text = "CALCULATOR",
                .ForeColor = Color.FromArgb(140, 140, 150),
                .Font = New Font("Segoe UI Semibold", 10.0F, FontStyle.Bold),
                .Location = New Point(20, 14),
                .Size = New Size(300, 22)
            }
        End Function

        ' Builds and returns the calculator display textbox.
        Private Function BuildDisplayTextBox() As TextBox
            Return New TextBox() With {
                .ReadOnly = True,
                .Text = "0",
                .TextAlign = HorizontalAlignment.Right,
                .Font = New Font("Segoe UI Semibold", 28.0F, FontStyle.Bold),
                .ForeColor = Color.White,
                .BackColor = Color.FromArgb(28, 28, 34),
                .BorderStyle = BorderStyle.FixedSingle,
                .Location = New Point(20, 42),
                .Size = New Size(332, 62),
                .TabStop = False,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            }
        End Function

        ' Creates all calculator buttons and places them in a responsive grid.
        Private Sub InitializeButtons()
            Dim panel As New TableLayoutPanel() With {
                .ColumnCount = 4,
                .RowCount = 5,
                .Location = New Point(20, 120),
                .Size = New Size(332, 420),
                .BackColor = Color.Transparent,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right,
                .Padding = New Padding(2)
            }

            For i As Integer = 0 To 3
                panel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
            Next

            For i As Integer = 0 To 4
                panel.RowStyles.Add(New RowStyle(SizeType.Percent, 20.0F))
            Next

            Dim layout As String(,) = {
                {"C", "⌫", "%", "/"},
                {"7", "8", "9", "*"},
                {"4", "5", "6", "-"},
                {"1", "2", "3", "+"},
                {"√", "0", ".", "="}
            }

            For row As Integer = 0 To layout.GetLength(0) - 1
                For col As Integer = 0 To layout.GetLength(1) - 1
                    panel.Controls.Add(CreateButton(layout(row, col)), col, row)
                Next
            Next

            Controls.Add(panel)
        End Sub

        ' Creates a styled rounded button and wires input events.
        Private Function CreateButton(label As String) As Button
            Dim isOperator As Boolean = "+-*/=%√".Contains(label, StringComparison.Ordinal)
            Dim isUtility As Boolean = (label = "C" OrElse label = "⌫")

            Dim defaultColor As Color = If(isOperator, Color.FromArgb(30, 136, 229), If(isUtility, Color.FromArgb(84, 84, 94), Color.FromArgb(44, 44, 54)))
            Dim hoverColor As Color = If(isOperator, Color.FromArgb(66, 165, 245), If(isUtility, Color.FromArgb(102, 102, 112), Color.FromArgb(62, 62, 74)))

            Dim button As New Button() With {
                .Text = label,
                .Dock = DockStyle.Fill,
                .FlatStyle = FlatStyle.Flat,
                .Margin = New Padding(6),
                .ForeColor = Color.White,
                .BackColor = defaultColor,
                .Font = New Font("Segoe UI", 15.0F, FontStyle.Bold),
                .Cursor = Cursors.Hand,
                .Tag = defaultColor
            }

            button.FlatAppearance.BorderSize = 0

            AddHandler button.Click, AddressOf OnButtonClick
            AddHandler button.Resize, Sub() ApplyRoundedCorners(button, 16)
            AddHandler button.MouseEnter, Sub() button.BackColor = hoverColor
            AddHandler button.MouseLeave, Sub() button.BackColor = CType(button.Tag, Color)

            ApplyRoundedCorners(button, 16)
            Return button
        End Function

        ' Applies rounded corners for cleaner modern button styling.
        Private Sub ApplyRoundedCorners(button As Button, radius As Integer)
            If button.Width <= 0 OrElse button.Height <= 0 Then Return

            Dim diameter As Integer = radius * 2
            Dim rect As New Rectangle(0, 0, button.Width, button.Height)

            Using path As New GraphicsPath()
                path.StartFigure()
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90)
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90)
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90)
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90)
                path.CloseFigure()

                Dim oldRegion As Region = button.Region
                button.Region = New Region(path)
                If oldRegion IsNot Nothing Then oldRegion.Dispose()
            End Using
        End Sub

        ' Keyboard support for quick data entry and operations.
        Private Sub OnCalculatorKeyDown(sender As Object, e As KeyEventArgs)
            Select Case e.KeyCode
                Case Keys.D0, Keys.NumPad0 : AppendDigit("0")
                Case Keys.D1, Keys.NumPad1 : AppendDigit("1")
                Case Keys.D2, Keys.NumPad2 : AppendDigit("2")
                Case Keys.D3, Keys.NumPad3 : AppendDigit("3")
                Case Keys.D4, Keys.NumPad4 : AppendDigit("4")
                Case Keys.D5, Keys.NumPad5 : AppendDigit("5")
                Case Keys.D6, Keys.NumPad6 : AppendDigit("6")
                Case Keys.D7, Keys.NumPad7 : AppendDigit("7")
                Case Keys.D8, Keys.NumPad8 : AppendDigit("8")
                Case Keys.D9, Keys.NumPad9 : AppendDigit("9")
                Case Keys.Decimal, Keys.OemPeriod : AppendDecimalPoint()
                Case Keys.Add : SetOperator("+")
                Case Keys.Oemplus
                    If e.Shift Then SetOperator("+") Else EvaluateExpression()
                Case Keys.Subtract, Keys.OemMinus : SetOperator("-")
                Case Keys.Multiply : SetOperator("*")
                Case Keys.Divide, Keys.OemQuestion : SetOperator("/")
                Case Keys.Enter, Keys.Return : EvaluateExpression()
                Case Keys.Back : Backspace()
                Case Keys.Delete, Keys.Escape : ClearAll()
                Case Keys.R, Keys.S : ApplySquareRoot()
                Case Keys.P : ApplyPercentage()
                Case Else
                    Return
            End Select

            e.SuppressKeyPress = True
        End Sub

        ' Routes button clicks to the right operation handler.
        Private Sub OnButtonClick(sender As Object, e As EventArgs)
            Dim input As String = DirectCast(sender, Button).Text

            Select Case input
                Case "0" To "9" : AppendDigit(input)
                Case "." : AppendDecimalPoint()
                Case "+", "-", "*", "/" : SetOperator(input)
                Case "=" : EvaluateExpression()
                Case "C" : ClearAll()
                Case "⌫" : Backspace()
                Case "%" : ApplyPercentage()
                Case "√" : ApplySquareRoot()
            End Select
        End Sub

        ' Adds digit input to the current number.
        Private Sub AppendDigit(digit As String)
            If _isEnteringNewNumber OrElse IsErrorState() OrElse _display.Text = "0" Then
                _display.Text = digit
                _isEnteringNewNumber = False
            Else
                _display.Text &= digit
            End If
        End Sub

        ' Adds a decimal point if one is not already present.
        Private Sub AppendDecimalPoint()
            If _isEnteringNewNumber OrElse IsErrorState() Then
                _display.Text = "0"
                _isEnteringNewNumber = False
            End If

            If Not _display.Text.Contains(".", StringComparison.Ordinal) Then
                _display.Text &= "."
            End If
        End Sub

        ' Stores the selected operator and handles chained operations.
        Private Sub SetOperator([operator] As String)
            If Not Decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.InvariantCulture, _firstOperand) Then
                ClearAll()
                Return
            End If

            If Not String.IsNullOrEmpty(_pendingOperator) AndAlso Not _isEnteringNewNumber Then
                EvaluateExpression()
                Decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.InvariantCulture, _firstOperand)
            End If

            _pendingOperator = [operator]
            _isEnteringNewNumber = True
        End Sub

        ' Converts current input to a percentage.
        Private Sub ApplyPercentage()
            Dim value As Decimal
            If Not Decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.InvariantCulture, value) Then Return

            If Not String.IsNullOrEmpty(_pendingOperator) Then
                value = (_firstOperand * value) / 100D
            Else
                value /= 100D
            End If

            _display.Text = value.ToString(CultureInfo.InvariantCulture)
            _isEnteringNewNumber = True
        End Sub

        ' Applies square root to the current display value.
        Private Sub ApplySquareRoot()
            Dim value As Decimal
            If Not Decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.InvariantCulture, value) Then Return

            If value < 0D Then
                _display.Text = "Invalid input"
                _isEnteringNewNumber = True
                Return
            End If

            Dim sqrtValue As Decimal = CDec(Math.Sqrt(CDbl(value)))
            _display.Text = sqrtValue.ToString(CultureInfo.InvariantCulture)
            _isEnteringNewNumber = True
        End Sub

        ' Evaluates the pending calculation and updates the display.
        Private Sub EvaluateExpression()
            If String.IsNullOrEmpty(_pendingOperator) Then Return

            Dim secondOperand As Decimal
            If Not Decimal.TryParse(_display.Text, NumberStyles.Number, CultureInfo.InvariantCulture, secondOperand) Then
                _display.Text = "Error"
                _isEnteringNewNumber = True
                Return
            End If

            Dim result As Decimal
            Select Case _pendingOperator
                Case "+" : result = _firstOperand + secondOperand
                Case "-" : result = _firstOperand - secondOperand
                Case "*" : result = _firstOperand * secondOperand
                Case "/"
                    If secondOperand = 0D Then
                        _display.Text = "Cannot divide by zero"
                        _pendingOperator = String.Empty
                        _isEnteringNewNumber = True
                        Return
                    End If
                    result = _firstOperand / secondOperand
                Case Else
                    Return
            End Select

            _display.Text = result.ToString(CultureInfo.InvariantCulture)
            _firstOperand = result
            _pendingOperator = String.Empty
            _isEnteringNewNumber = True
        End Sub

        ' Resets calculator state and display text.
        Private Sub ClearAll()
            _display.Text = "0"
            _firstOperand = 0D
            _pendingOperator = String.Empty
            _isEnteringNewNumber = True
        End Sub

        ' Removes one character from the current entry.
        Private Sub Backspace()
            If _isEnteringNewNumber OrElse IsErrorState() Then
                ClearAll()
                Return
            End If

            If _display.Text.Length > 1 Then
                _display.Text = _display.Text.Substring(0, _display.Text.Length - 1)
            Else
                _display.Text = "0"
                _isEnteringNewNumber = True
            End If
        End Sub

        ' Tracks display states that should be reset on next numeric input.
        Private Function IsErrorState() As Boolean
            Return _display.Text = "Error" OrElse _display.Text = "Cannot divide by zero" OrElse _display.Text = "Invalid input"
        End Function
    End Class
End Namespace
