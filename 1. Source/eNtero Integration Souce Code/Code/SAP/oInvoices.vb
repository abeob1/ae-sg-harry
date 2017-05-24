Public Class oInvoices
    Public Function CreateInvoice(ByVal NightAuditheader As DataRow)
        Dim cn As New Connection
        Dim dtHeader As DataTable = New DataTable
        Dim dtLine As DataTable = New DataTable
        Dim errMessage As String = ""
        Dim ds As DataSet = New DataSet
        Dim DocType As String = "13"
        Dim ret As String = ""
        Dim xm As New oXML
        Dim tableOINV As DataTable = Functions.BuildTable("OINV", "CardCode;DocDate")
        Dim tableINV1 As DataTable = Functions.BuildTable("INV1", "ItemCode;Dscription;WhsCode;Quantity;PriceBefDi;OcrCode2;OcrCode3;OcrCode4")
        Dim sctable As DataTable = Functions.BuildTable("INV2", "ExpnsCode;LineNum;LineTotal")
        'drNew("ExpnsCode") = GetFreightCodeByName("SC")
        'drNew("LineNum") = dr.Table.Rows.IndexOf(dr) 'LineNum
        'drNew("LineTotal") = dr("SCAmount")
        'drNew("OcrCode") = CostCenter


        dtHeader = cn.Integration_RunQuery(String.Format("select * from NightAuditInvHeader where HeaderID = {0}", NightAuditheader("ID")))
        If dtHeader.Rows.Count > 0 Then
            For Each rowHeader As DataRow In dtHeader.Rows
                dtLine = cn.Integration_RunQuery(String.Format("select * from NightAuditInvDetail where HeaderID = {0}", rowHeader("ID")))
                ds.Tables.Clear()
                tableOINV.Clear()
                tableINV1.Clear()
                sctable.Clear()

                If dtLine.Rows.Count > 0 Then
                    Try
                        tableOINV.Rows.Add(rowHeader("CompanyCode"), Format(rowHeader("CheckOutDate"), "yyyyMMdd"))
                        For Each rowLine As DataRow In dtLine.Rows
                            tableINV1.Rows.Add(rowLine("ItemCode"), rowLine("Description"), rowLine("OutLet"), rowLine("Quantity"), rowLine("Price"), rowLine("Division"), rowLine("Department"), rowLine("MarketSegment"))
                            sctable.Rows.Add(GetFreightCodeByName("SC"), rowLine.Table.Rows.IndexOf(rowLine), rowLine("SCAmount"))
                        Next


                        ds.Tables.Add(tableOINV.Copy)
                        ds.Tables.Add(tableINV1.Copy)
                        ds.Tables.Add(sctable.Copy)
                        Dim sErrMsg As String
                        sErrMsg = Functions.SystemInitial
                        If sErrMsg <> "" Then
                            Throw New Exception(sErrMsg)
                        End If

                        'Do somethings
                        Dim xmlstr As String = xm.ToXMLStringFromDS(DocType, ds)
                        ret = xm.CreateMarketingDocument("", xmlstr, DocType)

                        If ret <> "" Then
                            Throw New Exception(ret)
                        End If


                    Catch ex As Exception
                        Return ex.Message
                    End Try
                End If
            Next
            Return ""

        End If
        Return errMessage
    End Function

    Private Function GetFreightCodeByName(ByVal Name As String) As String
        Dim cn As New Connection
        Dim strQuery As String = ""
        strQuery = "select ExpnsCode from OEXD where ExpnsName='" + Name + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("ExpnsCode").ToString
        Else
            Return ""
        End If
    End Function
End Class
