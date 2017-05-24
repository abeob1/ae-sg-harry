Public Class oInvoice
    Dim DocEntry_Invoice As Integer = 0
#Region "Build Table Structure"
    Private Function BuildTableOINV() As DataTable ' Invoice
        Dim dt As New DataTable("OINV")
        dt.Columns.Add("U_POSTxNo")
        dt.Columns.Add("CardCode")
        dt.Columns.Add("DocDate")
        dt.Columns.Add("DocDueDate")
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
        dt.Columns.Add("U_RoomCode")
        dt.Columns.Add("U_ShiftCode")
        dt.Columns.Add("U_CommAmt")
        dt.Columns.Add("FreeTxt")
        dt.Columns.Add("LineNum")
        Return dt
    End Function
    Private Function BuildTableINV2() As DataTable 'Invoice detail
        Dim dt As New DataTable("INV2")
        dt.Columns.Add("ExpnsCode")
        dt.Columns.Add("LineNum")
        dt.Columns.Add("LineTotal")
        dt.Columns.Add("OcrCode")
        Return dt
    End Function
#End Region
#Region "Insert into Table"
    Private Function InsertIntoOINV(dt As DataTable, dr As DataRow) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("U_POSTxNo") = dr("InvoiceNo")
        drNew("U_ReservationNo") = dr("ReservationCode")
        drNew("CardCode") = dr("CardCode").ToString
        'drNew("DocDate") = CDate(dr("CheckInDate")).ToString("yyyyMMdd")
        drNew("DocDueDate") = CDate(dr("CheckOutDate")).ToString("yyyyMMdd")
        'drNew("OwnerCode") = dr("EmpID").ToString
        'drNew("DiscPrcnt") = dr("Discount")
        dt.Rows.Add(drNew)
        Return dt
    End Function
    Private Function InsertIntoINV1(dt As DataTable, dr As DataRow, CostCenter As String) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("ItemCode") = dr("ItemCode").ToString
        drNew("Dscription") = dr("Description").ToString
        'drNew("WhsCode") = dr("WarehouseCode").ToString
        drNew("Quantity") = dr("Quantity")
        drNew("PriceBefDi") = dr("Price")
        drNew("DiscPrcnt") = dr("Discount")
        drNew("Price") = dr("PriceAfterDiscount")
        drNew("OcrCode") = CostCenter
        drNew("U_RoomCode") = dr("RoomCode").ToString
        drNew("U_ShiftCode") = dr("ShiftCode").ToString
        drNew("LineNum") = dr.Table.Rows.IndexOf(dr)
        dt.Rows.Add(drNew)
        Return dt
    End Function
    Private Function InsertIntoINV2(dt As DataTable, dr As DataRow, LineNum As Integer, CostCenter As String) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("ExpnsCode") = GetFreightCodeByName("SC")
        drNew("LineNum") = dr.Table.Rows.IndexOf(dr) 'LineNum
        drNew("LineTotal") = dr("SCAmount")
        drNew("OcrCode") = CostCenter
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
            Dim dt As DataTable = cn.Integration_RunQuery("sp_Invoice_LoadForSync")
            If Not IsNothing(dt) Then

                Dim sErrMsg As String
                sErrMsg = Functions.SystemInitial
                If sErrMsg <> "" Then
                    Return
                End If
                

                '------------------L1: INTEGRATION HEADER-----------------------
                For Each dr As DataRow In dt.Rows
                    Dim xmlstr As String = ""

                    If PublicVariable.oCompanyInfo.InTransaction Then
                        PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    End If
                    PublicVariable.oCompanyInfo.StartTransaction()



                    Dim HeaderID As String = dr.Item("InvoiceNo")
                    Dim ret As String = ""

                    Dim dtheader1 As DataTable
                    '---------------------L2:GET DISTINCT BY STAY DATE------------------------
                    dtheader1 = cn.Integration_RunQuery("select distinct convert(nvarchar(10),StayDate,112) StayDate from InvoiceDetail T0 with(Nolock) where T0.InvoiceNo='" + HeaderID + "'")
                    For Each dr1 As DataRow In dtheader1.Rows

                        '----------add Invoice header----------
                        Dim dtOINV As DataTable = BuildTableOINV()
                        Dim dtINV1 As DataTable = BuildTableINV1()
                        Dim dtINV2 As DataTable = BuildTableINV2()

                        dtOINV = InsertIntoOINV(dtOINV, dr)


                        '----------L3: add Invoice line------------
                        Dim dtLine As DataTable = cn.Integration_RunQuery("sp_InvoiceLine_LoadByID '" + CStr(HeaderID) + "','" + dr1("StayDate").ToString + "'")
                        If dtLine.Rows.Count = 0 Then
                            ret = "Service Return: Invoice has no line item."
                            cn.Integration_RunQuery("sp_Invoice_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "',''")
                        Else
                            Dim i As Integer = 0
                            For Each drLine As DataRow In dtLine.Rows
                                Dim CostCenter As String = Functions.GetCostCenterByItem(drLine("ItemCode").ToString)
                                dtINV1 = InsertIntoINV1(dtINV1, drLine, CostCenter)
                                '-------------Service charge: Freight-----------
                                If drLine("SCAmount") <> 0 Then
                                    dtINV2 = InsertIntoINV2(dtINV2, drLine, drLine.Table.Rows.IndexOf(drLine), CostCenter)
                                End If

                                dtOINV.Rows(0)("DocDate") = CDate(drLine("StayDate")).ToString("yyyyMMdd")
                                i = i + 1
                            Next
                        End If
                        Dim ds As New DataSet
                        ds.Tables.Add(dtOINV.Copy)
                        ds.Tables.Add(dtINV1.Copy)
                        ds.Tables.Add(dtINV2.Copy)

                        xmlstr = xm.ToXMLStringFromDS(DocType, ds)
                        ret = xm.CreateMarketingDocument("", xmlstr, DocType)

                        If ret.Contains("'") Then
                            ret = ret.Replace("'", " ")
                        End If

                        If ret <> "" Then
                            Exit For
                        End If
                    Next

                    If PublicVariable.oCompanyInfo.InTransaction Then
                        If ret <> "" Then
                            PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        Else
                            PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                        End If
                    End If
                    cn.Integration_RunQuery("sp_Invoice_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "','" + xmlstr + "'")

                Next
            End If
        Catch ex As Exception
            If PublicVariable.oCompanyInfo.InTransaction Then
                PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
#End Region
End Class
