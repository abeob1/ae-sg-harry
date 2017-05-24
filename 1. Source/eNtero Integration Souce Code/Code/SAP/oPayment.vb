Public Class oPayment
    Dim DocEntry_Invoice As Integer = 0
#Region "Build Table Structure"
    Private Function BuildTableORCT() As DataTable 'Incoming Payment
        Dim dt As New DataTable("ORCT")
        dt.Columns.Add("DocDate")
        dt.Columns.Add("DocDueDate")
        dt.Columns.Add("CardCode")
        dt.Columns.Add("CashAcct")
        dt.Columns.Add("CashSum")
        dt.Columns.Add("TrsfrAcct")
        dt.Columns.Add("TrsfrSum")
        dt.Columns.Add("DocType")
        dt.Columns.Add("CounterRef")
        dt.Columns.Add("NoDocSum")
        dt.Columns.Add("U_Description")
        dt.Columns.Add("U_POSTxNo")
        Return dt
    End Function
    Private Function BuildTableRCT3() As DataTable 'Incoming Payment Detail
        Dim dt As New DataTable("RCT3")
        dt.Columns.Add("CreditCard")
        dt.Columns.Add("CardValid")
        dt.Columns.Add("CrCardNum")
        dt.Columns.Add("CreditSum")
        dt.Columns.Add("VoucherNum")
        dt.Columns.Add("CreditAcct")
        Return dt
    End Function
    Private Function BuildTableRCT2() As DataTable 'Incoming Payment Detail
        Dim dt As New DataTable("RCT2")
        dt.Columns.Add("DocEntry")
        dt.Columns.Add("InvType")
        dt.Columns.Add("SumApplied")
        dt.Columns.Add("DocLine")
        Return dt
    End Function
    Private Function BuildTableOJDT() As DataTable
        Dim dt As New DataTable("OJDT")
        dt.Columns.Add("RefDate")
        dt.Columns.Add("DueDate")
        dt.Columns.Add("Memo")
        dt.Columns.Add("Ref1")
        Return dt
    End Function
    Private Function BuildTableJDT1() As DataTable
        Dim dt As New DataTable("JDT1")
        dt.Columns.Add("ShortName")
        dt.Columns.Add("Debit")
        dt.Columns.Add("Credit")
        Return dt
    End Function
#End Region
#Region "Insert into Table"
    Private Function InsertIntoRCT3(dt As DataTable, dr As DataRow) As DataTable
        If dr("PaymentMethod") <> "CASH" And dr("PaymentMethod") <> "TRANSFER" Then
            Dim drNew As DataRow = dt.NewRow
            drNew("CreditCard") = GetCrCCodeByName(dr("PaymentMethod"))
            drNew("CardValid") = "99990101"
            drNew("CrCardNum") = "999"
            drNew("CreditSum") = dr("Amount")
            drNew("VoucherNum") = "1"
            drNew("CreditAcct") = GetCreditCardGL(dr("PaymentMethod"))
            dt.Rows.Add(drNew)
        End If
        Return dt
    End Function
    Private Function InsertIntoORCT(dt As DataTable, dr As DataRow) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("DocDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("DocDueDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("CardCode") = Functions.GetOneTimeCustomerCode(dr("CardCode").ToString)
        drNew("NoDocSum") = dr("Amount")
        drNew("DocType") = "C"
        drNew("CounterRef") = dr("ReceiptNo")
        drNew("U_Description") = dr("Description").ToString
        drNew("U_POSTxNo") = dr("ReservationCode").ToString

        dt.Rows.Add(drNew)
        Return dt
    End Function
    Private Function InsertIntoRCT2(dt As DataTable, InvoiceType As String) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("DocEntry") = DocEntry_Invoice
        If InvoiceType = "RES" Then
            drNew("InvType") = "203"
        Else
            drNew("InvType") = "13"
        End If
        'drNew("SumApplied") = dr("Amount")
        drNew("DocLine") = 0
        dt.Rows.Add(drNew)
        Return dt
    End Function
    
#End Region
#Region "Functions and Mapping"
    Private Function GetCrCCodeByName(CrCName As String) As Integer
        Dim CrCCode As Integer = 0
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select CreditCard from OCRC where isnull(CardName,'')='" + CrCName + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("CreditCard")
        Else
            Return 0
        End If
        Return CrCCode
    End Function
    Private Function GetInvoiceByResNo(ResNo As String) As DataTable
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select DocEntry,ObjType from OINV where isnull(U_ReservationNo,'')='" + ResNo + "'"
        strQuery = strQuery + " union all "
        strQuery = "select DocEntry,ObjType from ORCT where PayNoDoc='Y' and isnull(U_POSTxNo,'')='" + ResNo + "'"

        Return cn.SAP_RunQuery(strQuery)
    End Function
    Private Function GetCreditCardGL(CrCName As String) As Integer
        Dim CrCCode As Integer = 0
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select AcctCode from OCRC where isnull(CardName,'')='" + CrCName + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("AcctCode")
        Else
            Return 0
        End If
        Return CrCCode
    End Function
    
    Private Function LookupBPFromEmployee(EmpID As String) As String
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select T1.CardCode from OHEM T0"
        strQuery = strQuery + " join OCRD T1 on T0.U_Customer=T1.CardCode where empID=" + EmpID
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0)("CardCode").ToString
        Else
            Return ""
        End If

    End Function
#End Region
#Region "Create Payment"
    Public Sub CreatePayment()
        Dim DocType As String = "24"
        Dim cn As New Connection
        Dim xm As New oXML

        Try
            Dim dt As DataTable = cn.Integration_RunQuery("sp_PaymentDetail_NOR_LoadForSyn")
            If Not IsNothing(dt) Then

                Dim sErrMsg As String
                sErrMsg = Functions.SystemInitial
                If sErrMsg <> "" Then
                    Return
                End If
                For Each dr As DataRow In dt.Rows
                    Dim HeaderID As String = dr.Item("ID")
                    Dim ret As String = ""

                    Dim ds As New DataSet
                    Dim xmlstr As String
                    Dim dtORCT As DataTable = BuildTableORCT()
                    Dim dtRCT2 As DataTable = BuildTableRCT2()
                    Dim dtRCT3 As DataTable = BuildTableRCT3()

                    Dim PaymentType As String = Functions.GetPaymentType(dr("PaymentMethod"))
                    Select Case PaymentType
                        Case ""
                            ret = "Payment Type not found in system"
                        Case "CC"
                            If GetCrCCodeByName(dr("PaymentMethod")) = 0 Then
                                ret = "Credit Card Code/Payment Method not found!"
                            End If
                        Case "CO"
                            ret = Functions.CheckBPCode(dr("CompanyCode").ToString)
                        Case "OH"
                            If dr("empID").ToString = "" Then
                                ret = "Employee ID is missing"
                            End If
                    End Select

                    If ret <> "" Then
                        cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "',''")
                    Else
                        '-------------------------add payment header--------------------
                        dtORCT = InsertIntoORCT(dtORCT, dr)
                        Select Case PaymentType
                            Case "CA" '----------CASH------------
                                dtORCT.Rows(0)("CashAcct") = PublicVariable.pmCashAcct
                                dtORCT.Rows(0)("CashSum") = dr("Amount")
                            Case "CC" '----------CREDIT CARD-----
                                dtRCT3 = InsertIntoRCT3(dtRCT3, dr)
                            Case "RO" '----------TO ROOM---------
                                Return '----------do nothing------------
                            Case "CO" '--------TO COMPANY
                                CreateJE(dr, PaymentType)
                                Return
                            Case "OH" '------------TO EMPLOYEE---------
                                CreateJE(dr, PaymentType)
                                Return
                        End Select

                        If dr("Amount") < 0 Then
                            dtORCT.TableName = "OVPM"
                            dtRCT3.TableName = "VPM3"
                            DocType = "46"
                        Else
                            DocType = "24"
                            dtORCT.TableName = "ORCT"
                            dtRCT3.TableName = "RCT3"
                        End If

                        ds = New DataSet
                        ds.Tables.Add(dtORCT.Copy)
                        ds.Tables.Add(dtRCT3.Copy)

                        xmlstr = xm.ToXMLStringFromDS(DocType, ds)
                        ret = xm.CreateMarketingDocument("", xmlstr, DocType)

                        If ret.Contains("'") Then
                            ret = ret.Replace("'", " ")
                        End If
                        If xmlstr.Contains("'") Then
                            xmlstr = xmlstr.Replace("'", " ")
                        End If
                        cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "','" + xmlstr + "'")
                    End If
                Next
            End If
        Catch ex As Exception
            If PublicVariable.oCompanyInfo.InTransaction Then
                PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
    Private Function CreateJE(dr As DataRow, PaymentType As String) As String
        Dim dtOJDT As DataTable = BuildTableOJDT()
        Dim dtJDT1 As DataTable = BuildTableJDT1()
        Dim HeaderID As String = dr.Item("ID")

        Dim NewBPCode As String = dr("CompanyCode").ToString
        Dim Memo As String = "Pay to Company - " + dr("CompanyCOde").ToString
        If PaymentType = "OH" Then '----TO EMPLOYEE-----
            NewBPCode = LookupBPFromEmployee(dr("empID").ToString)
            Memo = "Pay to Employee - " + NewBPCode
        End If


        Dim drNew As DataRow = dtOJDT.NewRow
        drNew("RefDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("DueDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("Memo") = Memo
        drNew("Ref1") = ""
        dtOJDT.Rows.Add(drNew)

        drNew = dtJDT1.NewRow
        drNew("ShortName") = dr("CardCode").ToString
        drNew("Credit") = dr("Amount")
        drNew("Debit") = 0
        dtJDT1.Rows.Add(drNew)

        drNew = dtJDT1.NewRow
        drNew("ShortName") = NewBPCode
        drNew("Credit") = 0
        drNew("Debit") = dr("Amount")
        dtJDT1.Rows.Add(drNew)

        Dim ds As DataSet = New DataSet
        ds.Tables.Add(dtOJDT.Copy)
        ds.Tables.Add(dtJDT1.Copy)
        Dim DocType As String = "30"
        Dim ret As String = ""
        Dim xm As New oXML
        Dim cn As New Connection

        Dim xmlstr As String = xm.ToXMLStringFromDS(DocType, ds)
        ret = xm.CreateMarketingDocument("", xmlstr, DocType)


        If ret.Contains("'") Then
            ret = ret.Replace("'", " ")
        End If
        If xmlstr.Contains("'") Then
            xmlstr = xmlstr.Replace("'", " ")
        End If
        cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "','" + xmlstr + "'")
        Return ret

    End Function
#End Region

End Class
