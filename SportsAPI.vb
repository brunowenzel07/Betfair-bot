Imports System.Net
Imports System.Text
Imports System.IO
Imports Newtonsoft.Json

Module SportsAPI
    Public ssoid As String 'session token

    Private Function SendSportsReq(ByVal jsonString As String)

        Dim request As HttpWebRequest =
WebRequest.Create("https://api.betfair.com/exchange/betting/json-rpc/v1")

        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsonString)

        Dim responseFromServer As String = ""

        Try
            request.Method = "POST"
            request.ContentType = "application/json"
            request.ContentLength = byteArray.Length
            request.Headers.Add("X-Application: ")
            request.Headers.Add("X-Authentication: " & ssoid)
            request.AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
            request.ServicePoint.Expect100Continue = False
            request.Timeout = 2000

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)

            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()

            Dim reader As New StreamReader(dataStream)
            responseFromServer = reader.ReadToEnd()

            Form1.Print(responseFromServer) 'delete in chapter 5

            reader.Dispose()
            dataStream.Dispose()
            response.Dispose()

        Catch ex As WebException 'Exception
            Form1.Print("SendSportsReq Error: " & ex.Message)
        End Try

        Return responseFromServer

    End Function

    'Classes and function for MarketCatalogue request
    Public Class MarketCatalogueRequest
        Public jsonrpc As String = "2.0"
        Public method As String = "SportsAPING/v1.0/listMarketCatalogue"
        Public params As New Params
        Public id As Integer = 1
    End Class

    Public Class Params
        Public filter As New Filter
        Public sort As String = "FIRST_TO_START"
        Public maxResults As String = "200"
        Public marketProjection As New List(Of String)
    End Class

    Public Class Filter
        Public eventTypeIds As New List(Of String)
        Public marketCountries As New List(Of String)
        Public marketTypeCodes As New List(Of String)
        Public marketStartTime As New StartTime
    End Class

    Public Class StartTime
        Public from As String
        Public [to] As String
    End Class

    Function SerializeMarketCatalogueRequest(ByVal requestList As List(Of MarketCatalogueRequest))

        'Dim temp As String = JsonConvert.SerializeObject(requestList) 'd
        'Form1.Print(temp) 'd
        'Form1.Print("") 'd
        'Return temp 'd

        Return JsonConvert.SerializeObject(requestList)
    End Function

    'Classes and function for listMarketCatalogue response
    Public Class MarketCatalogueResponse
        Public jsonrpc As String
        Public result As List(Of MarketCatalogue)
        Public id As Integer
    End Class

    Public Class MarketCatalogue
        Public marketId As String
        Public marketName As String
        Public marketStartTime As String
        Public totalMatched As Double
        Public runners As New List(Of Runners)()
        Public [event] As New [Event]
    End Class

    Public Class Runners
        Public selectionId As Integer
        Public runnerName As String
        Public handicap As Double
        Public sortPriority As Integer
    End Class

    Public Class [Event]
        Public id As Integer
        Public name As String
        Public countryCode As String
        Public timezone As String
        Public venue As String
        Public openDate As String
    End Class

    Function DeserializeMarketCatalogueResponse(ByVal jsonResponse As String)

        Return JsonConvert.DeserializeObject(Of MarketCatalogueResponse())(SendSportsReq(jsonResponse))

    End Function

    'Classes and function for listMarektBook request
    Public Class MarketBookRequest
        Public jsonrpc As String = "2.0"
        Public method As String = "SportsAPING/v1.0/listMarketBook"
        Public params As New MarketBookParams
        Public id As Integer = 1
    End Class

    Public Class MarketBookParams
        Public marketIds As New List(Of String)
        Public priceProjection As New PriceProjection
        Public orderProjection As String
    End Class

    Public Class PriceProjection
        Public priceData As New List(Of String)
    End Class

    Function SerializeMarketBookRequest(ByVal request As List(Of MarketBookRequest))
        Return JsonConvert.SerializeObject(request)
    End Function

    'Classes and functions for listMarketBook response
    Public Class MarketBookResponse

        Public jsonrpc As String
        Public result As New List(Of MarketBook)
        Public id As Integer
    End Class

    Public Class MarketBook
        Public marketId As String
        Public isMarketDataDelayed As Boolean
        Public Status As String
        Public betDelay As Integer
        Public bspReconciled As Boolean
        Public complete As Boolean
        Public inplay As Boolean
        Public numberOfWinners As Integer
        Public numberOfRunners As Integer
        Public numberOfActiveRunners As Integer
        Public lastMatchTime As Date
        Public totalMatched As Double
        Public totalAvailable As Double
        Public crossMatching As Boolean
        Public runnersVoidable As Boolean
        Public version As Integer
        Public runners As New List(Of MarketBookRunnerClass)
    End Class


    Public Class MarketBookRUnnerClass
        Public selectionId As Integer
        Public handicap As Double
        Public status As String
        Public adjustmentFactor As Double
        Public lastPriceTraded As Double
        Public totalMatched As Double
        Public removalDate As Date
        Public ex As New ex
        Public orders As New List(Of Order)
    End Class

    Public Class Order
        Public betId As String
        Public orderType As String
        Public status As String
        Public persistenceType As String
        Public side As String
        Public price As Double
        Public size As Double
        Public bspLiability As Double
        Public placedDate As Date
        Public avgPriceMatched As Double
        Public sizeMatched As Double
        Public sizeRemaining As Double
        Public sizeLapsed As Double
        Public sizeCancelled As Double
        Public sizeVoided As Double
    End Class

    Public Class ex
        Public availableToBack As New List(Of AvailableToBack)
        Public availableToLay As New List(Of AvailableToLay)
        Public tradedVolume As New List(Of TradedVolume)
    End Class

    Public Class AvailableToBack
        Public price As Double
        Public size As Double
    End Class

    Public Class AvailableToLay
        Public price As Double
        Public size As Double
    End Class

    Public Class TradedVolume
        Public price As Double
        Public size As Double
    End Class

    Function DeserializeMarketBookResponse(ByVal jsonRequest As String)
        Return JsonConvert.DeserializeObject(Of MarketBookResponse())(SendSportsReq(jsonRequest))
    End Function

    Function GetRawBook(ByVal jsonRequest As String)
        Return SendSportsReq(jsonRequest)
    End Function

    Function DeserializeRawBook(ByVal jsonResponse As String)
        Return JsonConvert.DeserializeObject(Of MarketBookResponse())(jsonResponse)
    End Function

    'Classes and function for placeOrders request
    Public Class PlaceOrdersRequest
        Public jsonrpc As String = "2.0"
        Public method As String = "SportsAPING/v1.0/placeOrders"
        Public params As New PlaceOrdersParams
        Public id As Integer
    End Class

    Public Class PlaceOrdersParams
        Public marketId As String
        Public instructions As New List(Of PlaceInstructions)
    End Class

    Public Class PlaceInstructions
        Public selectionId As Integer
        Public handicap As Double
        Public side As String
        Public orderType As String
        Public limitOrder As LimitOrder
    End Class

    Public Class LimitOrder
        Public size As Double
        Public price As Double
        Public persistenceType As String
    End Class

    Function SerializePlaceOrdersRequest(ByVal requestList As List(Of PlaceOrdersRequest))
        Return JsonConvert.SerializeObject(requestList)
    End Function

    'Classes and functions for PlaceOrders response
    Public Class PlaceOrdersResponse
        Public jsonrpc As String
        Public result As New PlaceExecutionReport
        Public id As Integer
    End Class

    Public Class PlaceExecutionReport
        Public status As String
        Public marketId As String
        Public instructionReports As New List(Of PlaceInstructionReports)
    End Class

    Public Class PlaceInstructionReports
        Public status As String
        Public instruction As New PlaceInstruction
        Public betId As String
        Public placedDate As Date
        Public averagePriceMatched As Double
        Public sizeMacthed As Double
    End Class

    Public Class PlaceInstruction
        Public selectionId As Integer
        Public handicap As Double
        Public limitorder As LimitOrder
        Public orderType As String
        Public side As String
    End Class

    Function DeserializePlaceOrdersResponse(ByVal jsonResponse As String)

        Return JsonConvert.DeserializeObject(Of PlaceOrdersResponse())(SendSportsReq(jsonResponse))

    End Function

    'Classes and functions for CancelOrders request
    Public Class CancelOrdersRequest
        Public jsonrpc As String = "2.0"
        Public method As String = "SportsAPING/v1.0/cancelOrders"
        Public params As New CancelOrdersParams
        Public id As Integer = 1
    End Class

    Public Class CancelOrdersParams
        Public marketId As String
        Public instructions As New List(Of CancelInstructions)
    End Class

    Public Class CancelInstructions
        Public betId As String
        Public sizeReduction As Double
    End Class

    Function SerializeCancelOrdersRequest(ByVal requestList As List(Of CancelOrdersRequest))
        Return JsonConvert.SerializeObject(requestList)
    End Function

    'Classes and functions for CancelOrders response
    Public Class CancelOrdersResponse
        Public jsonrpc As String
        Public result As New CancelExecutionReport
        Public id As Integer
    End Class

    Public Class CancelExecutionReport
        Public status As String
        Public marketId As String
        Public InstructionReports As New List(Of CancelInstructionReport)
    End Class

    Public Class CancelInstructionReport
        Public status As String
        Public instruction As New CancelInstruction
        Public sizeCancelled As Double
    End Class

    Public Class CancelInstruction
        Public betId As String
        Public sizeReduction As Double
    End Class

    Function DeserializeCancelOrdersResponse(ByVal jsonResponse As String)

        Return JsonConvert.DeserializeObject(Of CancelOrdersResponse())(SendSportsReq(jsonResponse))
    End Function

    'Classes and function for ReplaceOrders request
    Public Class ReplaceOrdersRequest
        Public jsonrpc As String = "2.0"
        Public method As String = "SportsAPING/v1.0/replaceOrders"
        Public params As New ReplaceOrdersParams
        Public id As String = 1
    End Class

    Public Class ReplaceOrdersParams
        Public marketId As String
        Public instructions As New List(Of ReplaceInstructions)
    End Class

    Public Class ReplaceInstructions
        Public betId As String
        Public newPrice As Double
    End Class

    Function SerializeReplaceOrdersRequest(ByVal requestList As List(Of ReplaceOrdersRequest))
        Return JsonConvert.SerializeObject(requestList)
    End Function

    'Classes and function for ReplaceOrders response
    Public Class ReplaceOrdersResponse
        Public jsonrpc As String
        Public result As New ReplaceExecutionReport
        Public id As Integer
    End Class

    Public Class ReplaceExecutionReport
        Public status As String
        Public marketId As String
        Public instructionReports As New List(Of ReplaceInstructionReport)
    End Class

    Public Class ReplaceInstructionReport
        Public status As String
        Public cancelInstructionReport As New CancelInstructionRep
        Public placeInstructionReport As New PlaceInstructionRep
    End Class

    Public Class CancelInstructionRep
        Public status As String
        Public instruction As New CancelInstruct
        Public sizeCancelled As Double
    End Class

    Public Class CancelInstruct
        Public betId As String
    End Class

    Public Class PlaceInstructionRep
        Public status As String
        Public instruction As New PlaceInstruct
        Public betId As String
        Public placedDate As Date
        Public averagePriceMatched As Double
        Public sizeMatched As Double
        Public sizeReduction As Double
    End Class

    Public Class PlaceInstruct
        Public selectionId As Integer
        Public limitOrder As New LimitOrder
        Public orderTyper As String
        Public size As String
    End Class

    Function DeserializeReplaceOrdersResponse(ByVal jsonResponse As String)

        Return JsonConvert.DeserializeObject(Of ReplaceOrdersResponse())(SendSportsReq(jsonResponse))
    End Function


End Module
