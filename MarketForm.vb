Public Class MarketForm

    Inherits Form 'inherits form's function

    Private WithEvents runnerList As New ListBox

    Private WithEvents selectedList As New ListBox

    Private WithEvents startButton As New Button

    Private marketId As String
    Private course As String
    Private runners As New List(Of Form1.ChartDetail)

    Sub New(ByVal marketId As String, ByVal course As String, ByVal runners As List(Of Form1.ChartDetail))

        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Size = New Size(240, (runners.Count * 14) + 80)
        Me.Text = course
        Me.Show()

        Me.marketId = marketId
        Me.course = course
        Me.runners = runners 'copy list of runners to local copy of list

        With runnerList
            .Left = 10
            .Top = 10
            .Size = New Size(100, (runners.Count * 14))
        End With
        Controls.Add(runnerList)

        For n As Integer = 0 To runners.Count - 1

            runnerList.Items.Add(runners.Item(n).runnerName)
        Next
        With selectedList
            .Left = 120
            .Top = 10
            .Size = New Size(100, (runners.Count * 14))
        End With
        Controls.Add(selectedList)

        With startButton
            .Left = 78
            .Top = 20 + runners.Count * 14
            .Text = "Start"
        End With
        Controls.Add(startButton)
    End Sub

    Private Sub runnerList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles runnerList.SelectedIndexChanged

        selectedList.Items.Add(runners.Item(runnerList.SelectedIndex).runnerName)
        runners.Item(runnerList.SelectedIndex).chartBot = True

    End Sub

    Private Sub startButton_Click(sender As Object, e As EventArgs) Handles startButton.Click

        Dim chartBotForm As New ChartBotForm(Me.marketId, Me.course, Me.runners, selectedList.Items.Count)

        Me.Close()
    End Sub
End Class
