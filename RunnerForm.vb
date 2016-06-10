Public Class RunnerForm

    Inherits Form
    Private Control As New UserControl1
    Private marketId As String
    Private selectionId As String
    Private openPosition As Boolean

    Sub New(ByVal details As String, ByVal marketId As String, ByVal selectionId As Integer)

        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Size = New Size(360, 450)
        Me.Text = details
        Me.Show()

        Me.marketId = marketId
        Me.selectionId = selectionId

        With Control
            .Left = 0
            .Top = 0
        End With
        Controls.Add(Control)

        'control handlers
        AddHandler Control.betButton.Click, AddressOf betButton_Click
        Control.cancelButton.Enabled = False
        AddHandler Control.cancelButton.Click, AddressOf cancelButton_Click
        AddHandler Control.hedgeButton.Click, AddressOf hedgeButton_Click

        openPosition = False

    End Sub

    Public Sub MonitorBets()
        Control.backedLabel.Text = "£" & Form1.runnerDictionary(Me.selectionId).sumBacked & " @ " & Form1.runnerDictionary(Me.selectionId).avgBackPrice

        Control.laidLabel.Text = "£" & Form1.runnerDictionary(Me.selectionId).sumLaid & " @ " & Form1.runnerDictionary(Me.selectionId).avgLayPrice

        Control.hedgeValueLabel.Text = "£" & Form1.runnerDictionary(Me.selectionId).hedge

        'back side stop handler

        If Control.backCheckBox.Checked = True Then 'hedge out
            If Form1.runnerDictionary(Me.selectionId).backPrice >= CDbl(Control.backStopTextBox.Text) Then

                hedgePosition()
            End If
        End If

        'lay side stop handler
        If Control.layCheckBox.Checked = True Then 'hedge out

            If Form1.runnerDictionary(Me.selectionId).layPrice <= CDbl(Control.layStopTextBox.Text) Then

                hedgePosition()
            End If
        End If

        'monitor open position handler
        If openPosition = True Then

            Dim lastRowIndex = Control.DataGridView1.Rows.Count - 1
            Dim betId = Control.DataGridView1.Item("betId", lastRowIndex).Value

            Control.DataGridView1.Item("sizeMacthed", lastRowIndex).Value = Form1.betDictionary(betId).sizeMatched

            Control.DataGridView1.Item("avgPriceMatched", lastRowIndex).Value = Form1.betDictionary(betId).averagePriceMatched

            If Form1.betDictionary(betId).fillOrKill > 0 Then

                Form1.betDictionary(betId).fillOrKill -= 1

                Control.DataGridView1.Item("fillOrKill", Control.DataGridView1.Rows.Count - 1).Value = Form1.betDictionary(betId).fillOrKill

            ElseIf Form1.betDictionary(betId).fillOrKill = 0 Then

                Form1.CancelOrders(Control.DataGridView1.Item("betId", Control.DataGridView1.Rows.Count - 1).Value, Me.marketId, Control.DataGridView1.Item("sizeRequested", Control.DataGridView1.Rows.Count - 1).Value)

                Control.DataGridView1.Item("betId", Control.DataGridView1.Rows.Count - 1).Value = "CANCELLED"

                Control.betButton.Enabled = True
                Control.cancelButton.Enabled = False
                openPosition = False

            End If

            'bet macthed handler
            If Form1.betDictionary(betId).status = "EXECUTION_COMPLETE" Then

                If Not Control.returnSizeTextBox.Text = "" And Not Control.returnPriceTextBox.Text = "" Then

                    If Control.backRadioButton.Checked = True Then 'if backed then lay

                        PlaceBet("LAY", CDbl(Control.returnSizeTextBox.Text), CDbl(Control.returnPriceTextBox.Text), -1)

                    Else 'else back
                        PlaceBet("BACK", CDbl(Control.returnSizeTextBox.Text), CDbl(Control.returnPriceTextBox.Text), -1)
                    End If

                    Control.betButton.Enabled = False
                    Control.cancelButton.Enabled = True 'enable cancel button
                    openPosition = True 'flag position opened
                End If

                Control.betButton.Enabled = True
                openPosition = False
            End If
        End If
    End Sub

    Private Sub betButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim side As String = "BACK"
        If Control.layRadioButton.Checked = True Then
            side = "LAY"

        End If

        Dim size As Double = CDbl(Control.sizeTextBox.Text)
        Dim price As Double = CDbl(Control.priceTextBox.Text)
        Dim fillOrKill As Integer

        If Control.fillOrKillTextBox.Text = "" Then
            fillOrKill = -1
        Else
            fillOrKill = CDbl(Control.fillOrKillTextBox.Text)
        End If
        PlaceBet(side, size, price, fillOrKill)
    End Sub

    Private Sub PlaceBet(ByVal side As String, ByVal size As Double, ByVal price As Double, ByVal fillOrKill As Integer)

        Dim betId As String

        betId = Form1.SendBet(Me.marketId, Me.selectionId, side, size, price)

        Dim detail As New Form1.BetDetail
        detail.fillOrKill = fillOrKill
        Form1.betDictionary.Add(betId, detail)

        Control.DataGridView1.Rows.Add(side, size, price, fillOrKill, 0, 0, betId)
        Control.betButton.Enabled = False
        Control.cancelButton.Enabled = True
        Control.sizeTextBox.Text = ""
        Control.priceTextBox.Text = ""
        Control.fillOrKillTextBox.Text = ""

        openPosition = True

    End Sub

    Private Sub cancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Form1.CancelOrders(Control.DataGridView1.Item("betId", Control.DataGridView1.Rows.Count - 1).Value, Me.marketId, Control.DataGridView1.Item("sizeRequested", Control.DataGridView1.Rows.Count - 1).Value)
        Control.DataGridView1.Item("betId", Control.DataGridView1.Rows.Count - 1).Value = "CANCELLED"

        Control.betButton.Enabled = True
        Control.cancelButton.Enabled = False
        openPosition = False
    End Sub

    Private Sub hedgeButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        hedgePosition()
    End Sub

    Private Sub hedgePosition()

        If Form1.runnerDictionary(Me.selectionId).backReturn > Form1.runnerDictionary(Me.selectionId).layLiability Then

            PlaceBet("Lay", Form1.runnerDictionary(Me.selectionId).hedgeStake, Form1.runnerDictionary(Me.selectionId).layPrice, -1)

        End If

        If Form1.runnerDictionary(Me.selectionId).layLiability > Form1.runnerDictionary(Me.selectionId).backReturn Then

            PlaceBet("BACK", Form1.runnerDictionary(Me.selectionId).hedgeStake, Form1.runnerDictionary(Me.selectionId).backPrice, -1)

        End If
    End Sub

    Private Sub RunnerForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        Form1.runnerFormDictionary.Remove(Me.selectionId)
    End Sub

End Class
