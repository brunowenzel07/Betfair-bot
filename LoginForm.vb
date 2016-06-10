Public Class LoginForm

    Private ssoid As String

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted

        Dim cookieArray As String() = WebBrowser1.Document.Cookie.Split(";")

        If WebBrowser1.Document.Cookie.Count > 0 Then

            For Each cookie In cookieArray

                Dim Idx As Long = cookie.IndexOf("=")

                If Idx <> -1 Then

                    Dim cookieName As String = cookie.Substring(0, Idx).Trim
                    Dim cookieValue As String = cookie.Substring(Idx + 1).Trim

                    If cookieName = "ssoid" Then
                        ssoid = cookieValue

                    End If

                End If

            Next

        End If

        If Not ssoid = "" Then
            'Form1.Print(ssoid) 'd
            SportsAPI.ssoid = ssoid 'copy ssoid to SportsAPI module
            AccountsAPI.ssoid = ssoid 'copy ssoid to accapi module
            Form1.Initialise()
            Me.Close()
        End If
    End Sub
End Class