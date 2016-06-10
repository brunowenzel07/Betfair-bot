Public Class ChartBotForm

    Inherits Form

    Private pictureBoxes As PictureBox() 'array of pictureBoxes to hold charts
    Private labels As Label() 'array of runner name labels

    Private urlList As New List(Of String)

    Private refreshLabel As New Label
    Private WithEvents nudSeconds As New NumericUpDown 'time itnerval selector
    Private secsLabel As New Label

    Private WithEvents time As New Timer 'time

    Sub New(ByVal marketId As String, ByVal course As String, ByVal runners As List(Of Form1.ChartDetail), ByVal selectedCount As Integer)

        Me.Width = 5 + (350 * selectedCount) + 5
        Me.Height = 320
        Me.Text = course
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.BackColor = Color.White
        Me.Show()

        'redim as youb know how many runners there are now
        ReDim pictureBoxes(selectedCount)
        ReDim labels(selectedCount)

        Dim leftPos As Integer = 0
        Dim pictureCount As Integer = 0

        For n As Integer = 0 To runners.Count - 1

            If runners.Item(n).chartBot = True Then

                pictureBoxes(pictureCount) = New PictureBox

                With pictureBoxes(pictureCount)

                    .Left = leftPos
                    .Top = -25
                    .Width = 350
                    .Height = 255
                    .SizeMode = PictureBoxSizeMode.AutoSize

                    Dim urlString As String = "http://uk.sportsiteeweb.betfair.com/betting/LoadRunnerInfoChartAction.do?marketId=" & marketId.Replace("1.", "") & "&selectionId=" & runners.Item(n).selectionId

                    pictureBoxes(pictureCount).Load(urlString)
                    urlList.Add(urlString)

                End With
                Controls.Add(pictureBoxes(pictureCount)) 'addPictureBox To form

                labels(pictureCount) = New Label

                Dim myfont As New Font("Sans Serif", 12, FontStyle.Regular)

                With labels(pictureCount)
                    .Left = leftPos 'set left pos
                    .Top = 231 'Set top pos
                    .Width = 350
                    .TextAlign = ContentAlignment.MiddleCenter
                    .Text = runners.Item(n).runnerName
                    .BackColor = Color.White
                    .Font = myfont

                End With
                Controls.Add(labels(pictureCount)) 'add label to Form

                pictureCount += 1
                leftPos += 351
            End If
        Next

        With refreshLabel
            .Left = 10
            .Top = 265
            .Width = 45
            .Text = "Refresh"
        End With
        Controls.Add(refreshLabel)

        With nudSeconds
            .Left = 60 'Set left pos
            .Top = 263
            .Width = 40
            .Minimum = 15 '15 in int
            .Maximum = 300
            .Increment = 15
            .Value = 60
        End With
        Controls.Add(nudSeconds)

        With secsLabel
            .Left = 110
            .Top = 265
            .Text = "seconds"
        End With
        Controls.Add(secsLabel)

        With time
            .Enabled = True
            .Interval = 60000 'initial interval of 60s
        End With
    End Sub

    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles time.Tick

        For n As Integer = 0 To urlList.Count - 1
            pictureBoxes(n).Load(Me.urlList.Item(n)) 'refresh charts
        Next
    End Sub
    Private Sub nudSeconds_ValueChanged(sender As Object, e As EventArgs) Handles nudSeconds.ValueChanged

        time.Interval = nudSeconds.Value * 1000 'change timer
    End Sub

    Private Sub ChartBotForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        time.Enabled = False 'stop timer

    End Sub
End Class
