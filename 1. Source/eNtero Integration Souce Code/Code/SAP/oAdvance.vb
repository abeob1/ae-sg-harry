Public Class oAdvance
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
        dt.Columns.Add("Comments")
        dt.Columns.Add("U_POSTxNo")
        dt.Columns.Add("NoDocSum")
        dt.Columns.Add("U_Description")
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
#End Region
#Region "Insert into Table"
    Private Function InsertIntoRCT3(dt As DataTable, dr As DataRow) As DataTable
        'If dr("PaymentMethod") <> "CASH" And dr("PaymentMethod") <> "TRANSFER" Then
        Dim drNew As DataRow = dt.NewRow
        drNew("CreditCard") = GetCrCCodeByName(dr("PaymentMethod"))
        drNew("CardValid") = "99990101"
        Dim ccno As String = dr("CreditCardNo")
        If ccno = "" Then
            ccno = "999"
        End If
        drNew("CrCardNum") = ccno
        drNew("CreditSum") = Math.Abs(dr("Amount"))
        drNew("VoucherNum") = ccno
        drNew("CreditAcct") = GetCreditCardGL(dr("PaymentMethod"))
        dt.Rows.Add(drNew)
        'End If

        Return dt
    End Function
    Private Function InsertIntoORCT(dt As DataTable, dr As DataRow) As DataTable
        Dim drNew As DataRow = dt.NewRow
        drNew("DocDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("DocDueDate") = CDate(dr("PaymentDate")).ToString("yyyyMMdd")
        drNew("CardCode") = dr("CardCode")
        drNew("NoDocSum") = Math.Abs(dr("Amount"))
        
        drNew("DocType") = "C"
        drNew("CounterRef") = dr("ReceiptNo").ToString
        drNew("U_Description") = dr("Description").ToString
        drNew("U_POSTxNo") = dr("ReservationCode").ToString
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
#End Region
#Region "Create Advance"
    Public Sub CreateAdvance()
        Dim DocType As String = "24"
        Dim cn As New Connection
        Dim xm As New oXML

        Try
            Dim dt As DataTable = cn.Integration_RunQuery("sp_PaymentDetail_ADV_LoadForSyn")
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
                    Dim dtRCT3 As DataTable = BuildTableRCT3()

                    Dim PaymentType As String = Functions.GetPaymentType(dr("PaymentMethod"))
                    'Dim ar() As String = {"CA", "CC"}
                    'If Not ar.Contains(PaymentType) Then
                    '    ret = "Invalid payment method for advance!"
                    'End If
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

                    If Math.Abs(dr("Amount")) = 0 Then
                        ret = "System error: Amount is zero"
                    End If
                    If ret <> "" Then
                        cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','" + ret + "',''")
                    Else
                        '----------add payment header: include cash and transfer----------
                        dtORCT = InsertIntoORCT(dtORCT, dr)
                        Select Case PaymentType
                            Case "CA" '----------CASH------------
                                dtORCT.Rows(0)("CashAcct") = PublicVariable.pmCashAcct
                                dtORCT.Rows(0)("CashSum") = Math.Abs(dr("Amount"))
                            Case "CC" '----------CREDIT CARD-----
                                dtRCT3 = InsertIntoRCT3(dtRCT3, dr)
                            Case "RO" '----------TO ROOM---------
                                cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','System Error: Cannot Advance to ROOM',''")
                                Continue For '----------do nothing------------
                            Case "CO" '--------TO COMPANY
                                cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','System Error: Cannot Advance to Company',''")
                                'CreateJE(dr, PaymentType)
                                Continue For
                            Case "OH" '------------TO EMPLOYEE---------
                                cn.Integration_RunQuery("sp_PaymentDetail_UpdateReceived '" + CStr(HeaderID) + "','System Error: Cannot Advance to Employee',''")
                                'CreateJE(dr, PaymentType)
                                Continue For
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
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
#End Region
End Class
