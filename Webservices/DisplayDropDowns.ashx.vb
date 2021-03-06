﻿Imports System.IO
Imports Newtonsoft.Json

Public Class DisplayDropDowns
    Implements IHttpHandler
    Implements IRequiresSessionState

    Sub DisplayDropDown(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim tb As SQLExec = New SQLExec
        Dim mySearchTable As String = HttpContext.Current.Request.Item("SearchTable")

        Dim sql As String = ""
        Dim ds As DataSet = Nothing
        Dim DropDownFields As String = ""
        Dim DTable1 As DataTable = Nothing
        Dim DTable2 As DataTable = Nothing

        If mySearchTable = "USERCONTROL" Or mySearchTable = "Warehouse_PO" Or mySearchTable = "Warehouse_SO" Or mySearchTable = "Warehouse_OrderManagement" Then
            DTable1 = CommonMethods.getFacilities()

            DropDownFields += "Facility:::"
            For i = 0 To DTable1.Rows.Count - 1
                With DTable1.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !db_alias
                End With
            Next

            If mySearchTable = "USERCONTROL" Then
                DTable2 = CommonMethods.getUsers()
                DropDownFields += ";;;UserKey:::"
                For i = 0 To DTable2.Rows.Count - 1
                    With DTable2.Rows(i)
                        DropDownFields += IIf(i <> 0, ",", "") & !UserKey
                    End With
                Next

                For i = 0 To 2
                    sql += " select StorerKey from enterprise.storer where type = " & IIf(i = 0, "1", IIf(i = 1, "2", "5"))
                Next
                ds = (New SQLExec).Cursor(sql)

                If ds.Tables(0).Rows.Count > 0 Then
                    DropDownFields += ";;;StorerKey:::"
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        With ds.Tables(0).Rows(i)
                            DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                        End With
                    Next
                End If

                If ds.Tables(1).Rows.Count > 0 Then
                    DropDownFields += ";;;ConsigneeKey:::"
                    For i = 0 To ds.Tables(1).Rows.Count - 1
                        With ds.Tables(1).Rows(i)
                            DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                        End With
                    Next
                End If

                If ds.Tables(2).Rows.Count > 0 Then
                    DropDownFields += ";;;SupplierKey:::"
                    For i = 0 To ds.Tables(2).Rows.Count - 1
                        With ds.Tables(2).Rows(i)
                            DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                        End With
                    Next
                End If

            End If
        ElseIf mySearchTable = "USERPROFILE" Then
            DTable1 = CommonMethods.getProfiles()
            DTable2 = CommonMethods.getUsers()

            DropDownFields += "ProfileName:::"
            For i = 0 To DTable1.Rows.Count - 1
                With DTable1.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !PROFILENAME
                End With
            Next

            DropDownFields += ";;;UserKey:::"
            For i = 0 To DTable2.Rows.Count - 1
                With DTable2.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !USERKEY
                End With
            Next
        ElseIf mySearchTable.Contains("enterprise.storer") Then
            DTable1 = CommonMethods.getCountries()

            DropDownFields += "Country:::"
            For i = 0 To DTable1.Rows.Count - 1
                With DTable1.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !Description
                End With
            Next
        ElseIf mySearchTable = "enterprise.sku" Then
            Dim where As String = ""
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            If Not owners Is Nothing Then
                Dim ownersstr As String = String.Join("','", owners)
                ownersstr = "'" & ownersstr & "'"
                If Not UCase(ownersstr).Contains("'ALL'") Then
                    where += " and STORERKEY IN (" + ownersstr + ") "
                End If
                '0 
            End If
            sql += "select StorerKey from enterprise.storer where type = 1 " & where
            sql += "select top 1000 packkey from enterprise.Pack "

            ds = (New SQLExec).Cursor(sql)

            DTable1 = ds.Tables(0)
            DTable2 = ds.Tables(1)

            DropDownFields += "StorerKey:::"
            For i = 0 To DTable1.Rows.Count - 1
                With DTable1.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !StorerKey
                End With
            Next

            DropDownFields += ";;;PackKey:::"
            For i = 0 To DTable2.Rows.Count - 1
                With DTable2.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !PackKey
                End With
            Next
        ElseIf mySearchTable = "SKUCATALOGUE" Then
            Dim owners As String() = CommonMethods.getOwnerPerUser(HttpContext.Current.Session("userkey").ToString)
            Dim consignees As String() = CommonMethods.getConsigneePerUser(HttpContext.Current.Session("userkey").ToString)
            sql += "select StorerKey, Type from enterprise.storer where Type in (1,2) order by StorerKey "
            sql += " select Currency from " & IIf(CommonMethods.dbtype <> "sql", "System.", "") & "CurrencyCodes"
            ds = (New SQLExec).Cursor(sql)
            DTable1 = ds.Tables(0)
            DTable2 = ds.Tables(1)

            DropDownFields += "StorerKey:::"
            Dim DropDownFields2 As String = ";;;ConsigneeKey:::"
            For Each row As DataRow In DTable1.Rows
                If row("Type").ToString = "1" Then
                    If owners.Any(Function(x) x = "ALL") Then
                        DropDownFields += row("StorerKey").ToString & ","
                    ElseIf owners.Any(Function(x) x = row("StorerKey").ToString) Then
                        DropDownFields += row("StorerKey").ToString & ","
                    End If
                Else
                    If consignees.Any(Function(x) x = "ALL") Then
                        DropDownFields2 += row("StorerKey").ToString & ","
                    ElseIf consignees.Any(Function(x) x = row("StorerKey").ToString) Then
                        DropDownFields2 += row("StorerKey").ToString & ","
                    End If
                End If
            Next

            If DropDownFields.EndsWith(",") Then DropDownFields = DropDownFields.Remove(DropDownFields.Length - 1)
            If DropDownFields2.EndsWith(",") Then DropDownFields2 = DropDownFields2.Remove(DropDownFields2.Length - 1)
            DropDownFields += DropDownFields2 & ";;;Currency:::"
            For i = 0 To DTable2.Rows.Count - 1
                With DTable2.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !Currency
                End With
            Next
        ElseIf mySearchTable = "Warehouse_ASN" Then
            DTable1 = CommonMethods.getFacilities()
            DTable2 = CommonMethods.getCountries()

            DropDownFields += "Facility:::"
            For i = 0 To DTable1.Rows.Count - 1
                With DTable1.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !db_alias
                End With
            Next

            DropDownFields += ";;;OriginCountry:::"
            For i = 0 To DTable2.Rows.Count - 1
                With DTable2.Rows(i)
                    DropDownFields += IIf(i <> 0, ",", "") & !DESCRIPTION
                End With
            Next
        End If

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)
        Using writer As JsonWriter = New JsonTextWriter(sw)
            writer.Formatting = Formatting.Indented
            writer.WriteStartObject()

            writer.WritePropertyName("DropDownFields")
            writer.WriteValue(DropDownFields)

            writer.WriteEndObject()
        End Using
        context.Response.Write(sw)
        context.Response.End()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class