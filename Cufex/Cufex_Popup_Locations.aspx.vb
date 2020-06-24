
Partial Public Class Cufex_Popup_Locations
    Inherits MultiLingualPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            QueryUrlStr.Value = Page.Request.Url.Query
        End If
    End Sub
End Class