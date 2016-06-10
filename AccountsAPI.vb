Imports System.Net
Imports System.Text
Imports System.IO
Imports Newtonsoft.Json

Module AccountsAPI
    Public ssoid As String
    Private Function SendAccReq(ByVal jsonString As String)

        Dim request As WebRequest = WebRequest.Create("https://api.betfair.com/exchange/account/json-rpc/v1")

        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonString)

        request.Method = "POST"
        request.ContentType = "application/json"
        request.ContentLength = byteArray.Length
        request.Headers.Add("X-Application: ")
        request.Headers.Add("X-Authentication: " & ssoid)

        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)

        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()

        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()

        Return responseFromServer

    End Function

    'Classes for getaccbal resp

    Public Class AccountBalanceResponse
        Public jsonrpc As String
        Public result As AccountBalanceResult

    End Class

    Public Class AccountBalanceResult
        Public availableToBetBalance As Double
        Public exposure As Double
        Public retainedCOmmission As Double
        Public exposureLimit As Double
        Public discountRate As Double
        Public pointsBalance As Integer

    End Class

    Function DeserializeAccountBalanceResponse(ByVal response As String)
        Return JsonConvert.DeserializeObject(Of AccountBalanceResponse())(SendAccReq(response))
    End Function

End Module
