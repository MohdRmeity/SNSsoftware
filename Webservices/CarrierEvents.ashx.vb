Imports System.IO
Imports Newtonsoft.Json
Public Class CarrierEvents
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub GetCarrierEventsDetails(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim ExternalOrderKey As String = HttpContext.Current.Request.Item("ExternalOrderKey")
        Dim StorerKey As String = HttpContext.Current.Request.Item("StorerKey")
        Dim ConsignmentID As String = HttpContext.Current.Request.Item("ConsignmentID")
        Dim Category As String = HttpContext.Current.Request.Item("Category")
        Dim ArticleID As String = HttpContext.Current.Request.Item("ConsignmentID")

        Dim tb As SQLExec = New SQLExec
        Dim AndFilter As String = " And ExternalOrderKey = '" & ExternalOrderKey & "' And StorerKey = '" & StorerKey & "' And ConsignmentID = '" & ConsignmentID & "'"

        Dim sql As String = ""
        sql += " select CO.CONSIGNMENTID, CO.ARTICLEID, MAX(CO.EVENTDATE) EVENTDATE, (SELECT TOP 1 (CA.EVENTSTATUSDESCR) FROM dbo.CARRIEREVENTS CA WHERE CA.EXTERNALORDERKEY = CO.EXTERNALORDERKEY AND CA.STORERKEY = CO.STORERKEY AND CA.CONSIGNMENTID = CO.CONSIGNMENTID AND CA.articleid = CO.ARTICLEID AND CA.EVENTDATE = MAX(CO.EVENTDATE)) AS EVENTSTATUS, (select ce.EVENTLOCATION from dbo.CARRIEREVENTS ce where CE.EXTERNALORDERKEY = CO.EXTERNALORDERKEY AND CE.STORERKEY = CO.STORERKEY AND ce.CONSIGNMENTID = CO.CONSIGNMENTID AND ce.articleid = CO.ARTICLEID AND ce.EVENTDATE = MAX(CO.EVENTDATE)) AS EVENTLOCATION, CO.PACKAGETYPE, " & Category & " As Category from dbo.CARRIEREVENTS CO WHERE 1=1 " & AndFilter & " GROUP BY CO.ARTICLEID, CO.CONSIGNMENTID, CO.PACKAGETYPE, CO.EXTERNALORDERKEY, CO.STORERKEY ORDER BY ARTICLEID ASC "
        sql += " select ARTICLEID, EVENTDATE, EVENTSTATUSDESCR, EVENTLOCATION, EVENTNOTES, POD, IMG, " & Category & " as Category from dbo.CARRIEREVENTS CO where 1=1 " & AndFilter & " order by EVENTDATE desc "

        Dim DS As DataSet = tb.Cursor(sql)

        Dim CarrierDetailsRecords As String = ""

        If Not DS Is Nothing Then
            GetCarrierEventsDetailsRecords(DS, CarrierDetailsRecords)
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
    Private Sub GetCarrierEventsDetailsRecords(ByVal DS As DataSet, ByRef CarrierDetailsRecords As String)
        For i = 0 To DS.Tables(0).Rows.Count - 1
            With DS.Tables(0).Rows(i)
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
                If CommonMethods.ShowCarrierEventsMap = "true" Then
                    CarrierDetailsRecords += "      <div class='map' id='map" & i & "' data-location='" & !EventLocation & "'>"
                    CarrierDetailsRecords += "      </div>"
                End If
                CarrierDetailsRecords += "      <div class='TrackingEventsDetailsDiv'> Tracking Events Details"
                CarrierDetailsRecords += "      </div>"
                CarrierDetailsRecords += "      <div class='HLine'>"
                CarrierDetailsRecords += "      </div>"
                CarrierDetailsRecords += "      <div class='EventsDivContainer'>"
                CarrierDetailsRecords += "         <div class='iWantMyChildrenFloatHeight'>"
                CarrierDetailsRecords += "            <div class='floatL Width100'>"
                Dim dr As DataRow() = DS.Tables(1).Select("ArticleID = '" & !ArticleID & "'")
                For j = 0 To dr.Count - 1
                    With dr(j)
                        CarrierDetailsRecords += "           <div class='floatL Width100 EventDiv'>"
                        CarrierDetailsRecords += "               <div class='floatL EventsInfoDiv'>"
                        CarrierDetailsRecords += "                  <div class='floatL CarrierEventDescription Category" & !Category & " Small'>" & !EventStatusDescr
                        CarrierDetailsRecords += "                     <div class='EventDescriptionBullet Category" & !Category & "'>"
                        CarrierDetailsRecords += "                     </div>"
                        If j <> dr.Count - 1 Then
                            CarrierDetailsRecords += "                 <div class='EventDivLine'>"
                            CarrierDetailsRecords += "                 </div>"
                        End If
                        CarrierDetailsRecords += "                  </div>"
                        CarrierDetailsRecords += "                  <div class='floatL VLine'>"
                        CarrierDetailsRecords += "                  </div>"
                        CarrierDetailsRecords += "                  <div class='floatL EventDate'>" & Format(!EventDate, "ddd MMMM dd - hh:mm tt")
                        CarrierDetailsRecords += "                  </div>"
                        CarrierDetailsRecords += "                  <div class='floatL Width100 EventInfo Location'>" & !EventLocation
                        CarrierDetailsRecords += "                  </div>"
                        CarrierDetailsRecords += "                  <div class='floatL Width100 EventInfo'>" & !EventNotes
                        CarrierDetailsRecords += "                  </div>"
                        CarrierDetailsRecords += "               </div>"
                        CarrierDetailsRecords += "               <div class='floatL EventsImgDiv'>"
                        CarrierDetailsRecords += "               </div>"
                        CarrierDetailsRecords += "           </div>"
                    End With
                Next
                CarrierDetailsRecords += "            </div>"
                CarrierDetailsRecords += "         </div>"
                CarrierDetailsRecords += "      </div>"
                CarrierDetailsRecords += "   </div>"
                CarrierDetailsRecords += "</div>"
            End With
        Next
    End Sub
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class