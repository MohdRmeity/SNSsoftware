Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports NLog
Imports Oracle.ManagedDataAccess.Client

Public Class FileUploadDashboard
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub Upload(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            Dim files As HttpFileCollection = context.Request.Files
            Dim request As HttpRequest = context.Request
            If files.Count <> 0 Then
                For i As Integer = 0 To files.Count - 1
                    Dim fileHttp As HttpPostedFile = files(i)
                    If fileHttp.ContentLength <> 0 Then
                        Dim fileName As String = String.Empty
                        If request.Browser.Browser.Contains("InternetExplorer") Then
                            fileName = IO.Path.GetFileName(fileHttp.FileName)
                        Else
                            fileName = fileHttp.FileName
                        End If
                        Dim path1 As String = context.Server.MapPath("~/ExportImages/") & fileName
                        fileHttp.SaveAs(path1)
                        Dim xmlstring = File.ReadAllText(path1)

                        Dim dashboardStorage As CustomDashboardStorage = New CustomDashboardStorage(HttpContext.Current.Session("userkey").ToString)

                        Dim xdoc As XDocument = New XDocument()

                        xdoc = XDocument.Parse(xmlstring)

                        Dim title As String = (CType((CType(xdoc.FirstNode, XContainer)).FirstNode, XElement)).LastAttribute.Value

                        Dim Finfo As FileInfo = New FileInfo(path1)
                        dashboardStorage.AddDashboard(xdoc, title & "_" & DateTime.Now.ToString("ddMMyyyyHHmmss"))
                    End If
                Next i
            End If


        Catch ex As Exception
            Dim tmp As String = "Error: " & ex.Message & vbTab + ex.GetType.ToString & "<br/>"
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")


            context.Response.ClearHeaders()
            context.Response.ClearContent()
            context.Response.StatusCode = HttpStatusCode.NotFound

            context.Response.StatusDescription = ex.Message
            context.Response.Flush()
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class