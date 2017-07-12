Public Class oPOSInvoice
    Dim DocEntry_Invoice As Integer = 0
#Region "Build Table Structure"
    Private Function BuildTableOINV() As DataTable ' Invoice
        Dim dt As New DataTable("OINV")
        dt.Columns.Add("U_POSTxNo")
        dt.Columns.Add("CardCode")
        dt.Columns.Add("DocDate")
        dt.Columns.Add("OwnerCode")
        dt.Columns.Add("DiscPrcnt")
        dt.Columns.Add("U_ReservationNo")
        Return dt
    End Function
    Private Function BuildTableINV1() As DataTable 'Invoice detail
        Dim dt As New DataTable("INV1")
        dt.Columns.Add("ItemCode")
        dt.Columns.Add("Dscription")
        dt.Columns.Add("WhsCode")
        dt.Columns.Add("Quantity")
        dt.Columns.Add("PriceBefDi")
        dt.Columns.Add("Price")
        dt.Columns.Add("DiscPrcnt")
        dt.Columns.Add("OcrCode")
        dt.Columns.Add("OwnerCode")
        dt.Columns.Add("U_ShiftCode")
        Return dt
    End Function
    Private Function BuildTableINV2() As DataTable 'Invoice detail
        Dim dt As New DataTable("INV2")
        dt.Columns.Add("ExpnsCode")
        dt.Columns.Add("LineNum")
        dt.Columns.Add("LineTotal")
        Return dt
    End Function
#End Region
#Region "Insert into Table"
    Private Function InsertIntoOINV(dt As DataTable, dr As DataRow) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("U_POSTxNo") = dr("InvoiceNo")
        drNew("U_ReservationNo") = dr("ReservationCode")
        drNew("CardCode") = Functions.GetOneTimeCustomerCode(dr("CardCode").ToString)
        drNew("DocDate") = CDate(dr("DocDate")).ToString("yyyyMMdd")
        drNew("OwnerCode") = dr("EmpID").ToString
        'drNew("DiscPrcnt") = dr("Discount")
        dt.Rows.Add(drNew)
        Return dt
    End Function
    Private Function InsertIntoINV1(dt As DataTable, dr As DataRow) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("ItemCode") = dr("ItemCode")
        drNew("Dscription") = dr("Description")
        drNew("WhsCode") = dr("WarehouseCode")
        drNew("Quantity") = dr("Quantity")
        drNew("PriceBefDi") = dr("Price")
        drNew("DiscPrcnt") = dr("DiscountPercent")
        drNew("Price") = dr("PriceAfterDiscount")
        Dim cc As String = Functions.GetCostCenterByItem(dr("ItemCode"))
        If cc <> "" Then
            drNew("OcrCode") = cc
        End If

        drNew("U_ShiftCode") = dr("ShiftCode")
        dt.Rows.Add(drNew)
        Return dt
    End Function
    Private Function InsertIntoINV2(dt As DataTable, dr As DataRow, LineNum As Integer) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("ExpnsCode") = GetFreightCodeByName("SC")
        drNew("LineNum") = LineNum
        drNew("LineTotal") = dr("SCAmount")
        dt.Rows.Add(drNew)
        Return dt
    End Function
#End Region

#Region "Functions and Mapping"
    Private Function GetInvoiceEntryByPOSNo(POSTxNo As String, DocType As String) As Integer
        Dim cn As New Connection
        Dim strQuery As String = ""
        If DocType = "13" Then
            strQuery = "select max(DocEntry) DocEntry from OINV where isnull(U_POSTxNo,'')='" + POSTxNo + "'"
        Else
            strQuery = "select max(DocEntry) DocEntry from ODPI where isnull(U_POSTxNo,'')='" + POSTxNo + "'"
        End If
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("DocEntry")
        Else
            Return 0
        End If
    End Function
    Private Function GetFreightCodeByName(Name As String) As String
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
#End Region

#Region "Create Invoice"
    Public Sub CreateInvoice()
        Dim DocType As String = "13"
        Dim cn As New Connection
        Dim xm As New oXML

        Try
            Dim dt As DataTable = cn.Integration_RunQuery("sp_POSInvoice_LoadForSync")
            If Not IsNothing(dt) Then

                Dim sErrMsg As String
                sErrMsg = Functions.SystemInitial
                If sErrMsg <> "" Then
                    Return
                End If
                For Each dr As DataRow In dt.Rows
                    Dim HeaderID As String = dr.Item("InvoiceNo")
                    Dim ret As String = ""

                    Dim dtOINV As DataTable = BuildTableOINV()
                    Dim dtINV1 As DataTable = BuildTableINV1()
                    Dim dtINV2 As DataTable = BuildTableINV2()
                    '----------add Invoice header----------
                    dtOINV = InsertIntoOINV(dtOINV, dr)

                    '----------add Invoice line------------
                    Dim dtLine As DataTable = cn.Integration_RunQuery("sp_POSInvoiceLine_LoadByID '" + CStr(HeaderID) + "'")
                    If dtLine.Rows.Count = 0 Then
                        ret = "Service Return: Invoice has no line item."
                        cn.Integration_RunQuery("sp_POSInvoice_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "',''")
                    Else
                        Dim i As Integer = 0

                        For Each drLine As DataRow In dtLine.Rows
                            dtINV1 = InsertIntoINV1(dtINV1, drLine)
                            '-------------Service charge: Freight-----------
                            If drLine("SCAmount") <> 0 Then
                                dtINV2 = InsertIntoINV2(dtINV2, drLine, i)
                            End If
                            i = i + 1
                        Next

                        Dim ds As New DataSet
                        ds.Tables.Add(dtOINV.Copy)
                        ds.Tables.Add(dtINV1.Copy)
                        ds.Tables.Add(dtINV2.Copy)

                        Dim xmlstr As String = xm.ToXMLStringFromDS(DocType, ds)
                        ret = xm.CreateMarketingDocument("", xmlstr, DocType)

                        If ret.Contains("'") Then
                            ret = ret.Replace("'", " ")
                        End If

                        If xmlstr.Contains("'") Then
                            xmlstr = xmlstr.Replace("'", " ")
                        End If

                        cn.Integration_RunQuery("sp_POSInvoice_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "','" + xmlstr + "'")

                    End If
                Next
            End If
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
#End Region
End Class
