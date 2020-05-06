Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Native
Imports DevExpress.DataAccess.Web
Imports System.Collections.Generic
' ...

Public Class DataSourceWizardConnectionStringsProvider
    Implements IDataSourceWizardConnectionStringsProvider

    Private connectionStrings As Dictionary(Of String, String)

    Public Sub New()
        connectionStrings = New Dictionary(Of String, String)()

        If CommonMethods.dbtype = "sql" Then
            connectionStrings.Add("SQLDBConnect", ConfigurationManager.ConnectionStrings("SQLDBConnect").ConnectionString)
        Else
            connectionStrings.Add("ReportOracleDBCon", ConfigurationManager.ConnectionStrings("ReportOracleDBCon").ConnectionString)
        End If
    End Sub

    Private Function IDataSourceWizardConnectionStringsProvider_GetConnectionDescriptions() As Dictionary(Of String, String) Implements IDataSourceWizardConnectionStringsProvider.GetConnectionDescriptions
        Return connectionStrings.ToDictionary(Function(k) k.Key, Function(k) k.Key)
    End Function

    Private Function IDataSourceWizardConnectionStringsProvider_GetDataConnectionParameters(name As String) As DataConnectionParametersBase Implements IDataSourceWizardConnectionStringsProvider.GetDataConnectionParameters
        Return New CustomStringConnectionParameters(connectionStrings(name))
    End Function
End Class


