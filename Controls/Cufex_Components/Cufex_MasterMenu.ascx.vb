Partial Public Class Cufex_MasterMenu
    Inherits MultiLingualControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim myMasterPage As Cufex_Site = CType(Page.Master, Cufex_Site)
        End If
    End Sub
End Class