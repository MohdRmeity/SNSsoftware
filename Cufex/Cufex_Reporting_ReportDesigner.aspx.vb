Imports System.Data.SqlClient
Imports System.Web.Services
Imports Oracle.ManagedDataAccess.Client
Imports System.Web.Script.Services
Imports NLog

Partial Public Class Cufex_Reporting_ReportDesigner
    Inherits MultiLingualPage

    Private Shared dbconx As String = ConfigurationManager.ConnectionStrings("DBConnect").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
            myMasterPage.FormParentName = "ReportsAndKPIs"
            myMasterPage.FormName = "Report Designer"
            myMasterPage.section = Cufex_Site.SectionName.Reporting
            myMasterPage.Subsection = Cufex_Site.SubSectionName.Reporting_ViewReports
            SetReportDesigner()
        End If
    End Sub
    Private Sub SetReportDesigner()
        Try
            Dim task As DesignerTask = New DesignerTask()
            task.mode = ReportEdditingMode.NewReport
            If task IsNot Nothing Then
                InitDesignerPage(task)
            End If
        Catch ex As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(ex, "", "")
        End Try
    End Sub
    Private Sub InitDesignerPage(ByVal task As DesignerTask)
        Select Case task.mode
            Case ReportEdditingMode.NewReport
                ASPxReportDesigner.OpenReport(New EmptyReport())
            Case ReportEdditingMode.ModifyReport
                ASPxReportDesigner.OpenReport(task.reportID)
        End Select
    End Sub
    <WebMethod()>
    Public Shared Function DeleteReport() As String
        Dim records As Boolean = False

        If Not String.IsNullOrEmpty(UserInfo.ReportID) Then
            Dim reportid As Integer = Integer.Parse(UserInfo.ReportID)
            Dim repotProfiledts As String
            Dim reportPortal As String

            If (ConfigurationManager.AppSettings("DatabaseType")).ToLower() = "sql" Then
                repotProfiledts = "IF (EXISTS(SELECT * FROM dbo.REPORTSPROFILEDETAIL  where Report= @pname)) BEGIN  DELETE FROM dbo.REPORTSPROFILEDETAIL WHERE Report=@pname END ELSE BEGIN select '' END"
                reportPortal = "IF (EXISTS(SELECT * FROM dbo.REPORTSPORTAL  where ReportID=@pname)) BEGIN  DELETE FROM dbo.REPORTSPORTAL WHERE ReportID=@pname END ELSE BEGIN select '' END"
            Else
                repotProfiledts = "DELETE FROM SYSTEM.REPORTSPROFILEDETAIL   WHERE Report=:pname and EXISTS (SELECT * FROM SYSTEM.REPORTSPROFILEDETAIL  where Report=:pname)"
                reportPortal = "DELETE FROM SYSTEM.REPORTSPORTAL   WHERE ReportID=:pname and EXISTS (SELECT * FROM SYSTEM.REPORTSPORTAL  where ReportID=:pname)"
            End If

            records = deleteRecord(repotProfiledts, reportPortal, reportid)
            UserInfo.ReportID = ""
        End If

        Return ""
    End Function
    Private Shared Function deleteRecord(ByVal repotProfiledts As String, ByVal reportPortal As String, ByVal key As Integer) As Boolean
        Try
            If CommonMethods.dbtype = "sql" Then
                Dim conn As SqlConnection = New SqlConnection(dbconx)
                conn.Open()
                Dim cmdusers As SqlCommand = New SqlCommand(repotProfiledts, conn)
                cmdusers.Parameters.AddWithValue("@pname", key)
                cmdusers.ExecuteNonQuery()
                Dim cmddts As SqlCommand = New SqlCommand(reportPortal, conn)
                cmddts.Parameters.AddWithValue("@pname", key)
                cmddts.ExecuteNonQuery()
                conn.Close()
            Else
                Dim conn As OracleConnection = New OracleConnection(dbconx)
                conn.Open()
                Dim cmdusers As OracleCommand = New OracleCommand(repotProfiledts, conn)
                cmdusers.Parameters.Add(New OracleParameter("pname", key))
                cmdusers.ExecuteNonQuery()
                Dim cmddts As OracleCommand = New OracleCommand(reportPortal, conn)
                cmddts.Parameters.Add(New OracleParameter("pname", key))
                cmddts.ExecuteNonQuery()
                conn.Close()
            End If
            Return True
        Catch e1 As Exception
            Dim logger As Logger = LogManager.GetCurrentClassLogger()
            logger.Error(e1, "", "")
            Return False
        End Try
    End Function
End Class