Imports System.IO
Imports Newtonsoft.Json
Public Class CarrierEvents
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub GetCarrierEventsDetails(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim CarrierDetailType As String = HttpContext.Current.Request.Item("CarrierDetailType")
        Dim ExternalOrderKey As String = HttpContext.Current.Request.Item("ExternalOrderKey")
        Dim StorerKey As String = HttpContext.Current.Request.Item("StorerKey")
        Dim ConsignmentID As String = HttpContext.Current.Request.Item("ConsignmentID")
        Dim Category As String = HttpContext.Current.Request.Item("Category")
        Dim ArticleID As String = HttpContext.Current.Request.Item("ConsignmentID")


        Dim sql As String = ""
        Dim tb As SQLExec = New SQLExec
        Dim AndFilter As String = " And ExternalOrderKey = '" & ExternalOrderKey & "' And StorerKey = '" & StorerKey & "' And ConsignmentID = '" & ConsignmentID & "'"

        If CarrierDetailType = "Articles" Then
            sql += " select CO.CONSIGNMENTID, CO.ARTICLEID, MAX(CO.EVENTDATE) EVENTDATE, (SELECT TOP 1 (CA.EVENTSTATUSDESCR) FROM dbo.CARRIEREVENTS CA WHERE CA.EXTERNALORDERKEY = CO.EXTERNALORDERKEY AND CA.STORERKEY = CO.STORERKEY AND CA.CONSIGNMENTID = CO.CONSIGNMENTID AND CA.articleid = CO.ARTICLEID AND CA.EVENTDATE = MAX(CO.EVENTDATE)) AS EVENTSTATUS, (select ce.EVENTLOCATION from dbo.CARRIEREVENTS ce where CE.EXTERNALORDERKEY = CO.EXTERNALORDERKEY AND CE.STORERKEY = CO.STORERKEY AND ce.CONSIGNMENTID = CO.CONSIGNMENTID AND ce.articleid = CO.ARTICLEID AND ce.EVENTDATE = MAX(CO.EVENTDATE)) AS EVENTLOCATION, CO.PACKAGETYPE, " & Category & " As Category from dbo.CARRIEREVENTS CO WHERE 1=1 " & AndFilter & " GROUP BY CO.ARTICLEID, CO.CONSIGNMENTID, CO.PACKAGETYPE, CO.EXTERNALORDERKEY, CO.STORERKEY ORDER BY ARTICLEID ASC "
        ElseIf CarrierDetailType = "Events" Then
            AndFilter += " And ArticleID = '" & ArticleID & "'"
            sql += " select EVENTDATE, EVENTSTATUSDESCR, EVENTLOCATION, EVENTNOTES, POD, IMG from dbo.CARRIEREVENTS CO where 1=1 " & AndFilter & " order by EVENTDATE desc "
        End If

        Dim DS As DataSet = tb.Cursor(sql)
        Dim OBJTable As DataTable = DS.Tables(0)

        Dim CarrierDetailsRecords As String = ""

        If Not OBJTable Is Nothing Then
            If CarrierDetailType = "Articles" Then
                GetArticlesRecords(OBJTable, CarrierDetailsRecords)
            ElseIf CarrierDetailType = "Events" Then
                'GetEventsRecords(OBJTable, CarrierDetailsRecords)
            End If
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Newtonsoft.Json.Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("CarrierDetailsRecords")
            writer.WriteValue(CarrierDetailsRecords)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub
    Private Sub GetArticlesRecords(ByVal OBJTable As DataTable, ByRef CarrierDetailsRecords As String)
        For i = 0 To OBJTable.Rows.Count - 1
            With OBJTable.Rows(i)
                For j = 0 To 1
                    CarrierDetailsRecords += "<div class='ArticleDiv'>"
                    CarrierDetailsRecords += "   <div class='ArticleDivContainer'>"
                    CarrierDetailsRecords += "      <div class='ArticleID'>" & !ArticleID & IIf(!PackageType <> "", " - <span>" & !PackageType & "</span>", "")
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div class='CarrierEventDescription Category" & !Category & " Big'>" & !EventStatus
                    CarrierDetailsRecords += "         <div class='EventDescriptionBullet Category" & !Category & "'></div>"
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div class='MostRecentUpdate'> Most Recent Update"
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div class='ArticleEventLocation'>" & !EventLocation
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div class='ArticleEventDate'>" & Format(!EventDate, "ddd MMMM dd - hh:mm tt")
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "   </div>"
                    CarrierDetailsRecords += "   <div class='TrackingHistoryDiv'> Tracking History"
                    CarrierDetailsRecords += "   </div>"
                    CarrierDetailsRecords += "   <div class='EventsDiv'>"
                    CarrierDetailsRecords += "      <div class='TrackingEventsDetailsDiv'> Tracking Events Details"
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div class='HLine'>"
                    CarrierDetailsRecords += "      </div>"
                    CarrierDetailsRecords += "      <div style='height:200px'></div>"
                    'CONT HERE !!!!!!!!!!!!
                    CarrierDetailsRecords += "   </div>"
                    CarrierDetailsRecords += "</div>"
                Next
            End With
        Next
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class