Public Class oJournal

#Region "Create JE"
    Public Function CreateJE(ByVal NightAuditheader As DataRow) As String
        Dim cn As New Connection
        Dim dt1 As DataTable = New DataTable
        Dim errMessage As String = ""
        Dim HeaderErr As String = ""
        '--Refund Negative Value
        dt1 = cn.Integration_RunQuery(String.Format("update NightAuditPayment set Amount=Amount*-1 where PaymentMethod='Refund' and Amount>0"))
        If dt1.Rows.Count > 0 Then
            'errMessage = CreateIncomingPayment_PMSDeposit(NightAuditheader, dt1)
            'If errMessage <> "" Then
            '    HeaderErr = errMessage
            'End If
        End If
        dt1.Dispose()
        dt1 = New DataTable

        '   --1.	PMS Deposit scenario
        dt1 = cn.Integration_RunQuery(String.Format("select [ID],[HeaderID],[PaymentCategory],[PaymentMethod],[PaymentBatchNo],isnull([Amount],0) Amount,[CreditCardNo],[EmpID],[CompanyCode],[Division],[Department],[TCode],[ReceiveDate],[ErrMsg],isnull([Voucher],'') [Voucher],isnull([OutletCode],'') [OutletCode],isnull([POSNo],'') [POSNo] from NightAuditPayment where PaymentCategory='ADV' and ReceiveDate is null and [Amount] <> 0 and HeaderID = {0}", NightAuditheader("ID")))
        If dt1.Rows.Count > 0 Then
            errMessage = CreateIncomingPayment_PMSDeposit(NightAuditheader, dt1)
            If errMessage <> "" Then
                HeaderErr = errMessage
            End If
        End If
        dt1.Dispose()
        dt1 = New DataTable


        '--3.	Revenue 
        dt1 = cn.Integration_RunQuery(String.Format("Select * from NightAuditRevenue where ReceiveDate is null and HeaderID = {0}", NightAuditheader("ID")))
        If dt1.Rows.Count > 0 Then
            errMessage = CreateJEForRevenue_PMS(NightAuditheader, dt1)
            If errMessage <> "" Then
                HeaderErr = errMessage
            End If
        End If
        dt1.Dispose()
        dt1 = New DataTable


        '--- 6.	City Ledger Settlement
        dt1 = cn.Integration_RunQuery(String.Format("select [ID],[HeaderID],isnull([BillNo],'') [BillNo],isnull([ReservationCode],'') [ReservationCode],isnull([CardCode],'') [CardCode],isnull([CardName],'') [CardName],[EmpID],[CheckInDate],[CheckOutDate],isnull([CompanyCode],'') [CompanyCode] ,[ReceiveDate],[ErrMsg],[paymenttype] from [NightAuditInvHeader]  where ReceiveDate is null and HeaderID = {0}", NightAuditheader("ID")))
        If dt1.Rows.Count > 0 Then
            errMessage = CreateDO_CityLedgerSettlement(NightAuditheader, dt1)
            If errMessage <> "" Then
                HeaderErr = errMessage
            End If
        End If
        dt1.Dispose()
        dt1 = New DataTable
        '---5.	Guest Ledger Settlement
        dt1 = cn.Integration_RunQuery(String.Format("select [ID],[HeaderID],[PaymentCategory],[PaymentMethod],[PaymentBatchNo],isnull([Amount],0) Amount,[CreditCardNo],[EmpID],[CompanyCode],[Division],[Department],[TCode],[ReceiveDate],[ErrMsg],isnull([Voucher],'') [Voucher],isnull([OutletCode],'') [OutletCode],isnull([POSNo],'') [POSNo] from NightAuditPayment where PaymentCategory='SET' and [Amount] <> 0 and ReceiveDate is null and HeaderID = {0}", NightAuditheader("ID")))
        If dt1.Rows.Count > 0 Then
            errMessage = CreateIncomingPayment_PMSSettlement(NightAuditheader, dt1)
            If errMessage <> "" Then
                HeaderErr = errMessage
            End If
        End If
        dt1.Dispose()
        dt1 = New DataTable

        Return HeaderErr
    End Function

#Region "Harrys"
    Private Function CreateIncomingPayment_PMSDeposit(ByVal header As DataRow, ByVal Detail As DataTable) As String
        Try
            Dim cn As New Connection
            Dim dtGLMapping As DataTable = New DataTable("GLMapping")
            Dim dtGLMapping1 As DataTable = New DataTable("GLMapping")
            Dim DebitAccount As String = ""
            Dim CreditAccount As String = ""
            Dim CreditTaxAccount As String = ""
            Dim ds As DataSet = New DataSet
            Dim DocType As String = "30"
            Dim ret As String = ""
            Dim xm As New oXML
            Dim query = ""
            Dim ErrMsg As String = ""
            Dim RetCode As Integer = 0
            Dim RetErrMsg As String = ""
            Dim oRecordSet As SAPbobsCOM.Recordset
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim oIncoming As SAPbobsCOM.Payments = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='Advance'")
            dtGLMapping = cn.SAP_RunQuery(query)
            If dtGLMapping.Rows.Count = 0 Then
                Functions.WriteLog("Can not get GL Account: Payment for PaymentCategory='Advance' in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get GL Account: Payment for PaymentCategory=Advance in [@ENTEROSETTING] Table"
                Throw New Exception(ErrMsg)
            Else
                CreditAccount = dtGLMapping.Rows(0)("U_Value").ToString
                oIncoming.DocType = SAPbobsCOM.BoRcptTypes.rAccount
                oIncoming.TaxDate = CDate(header("HotelDate"))
                oIncoming.DocDate = CDate(header("HotelDate"))
                oIncoming.Remarks = "Night Audit Deposit " & Format(CDate(header("HotelDate")), "dd/MM/yy")
                oIncoming.JournalRemarks = "Night Audit Deposit " & Format(CDate(header("HotelDate")), "dd/MM/yy")
                oIncoming.UserFields.Fields.Item("U_Ref").Value = "NIGHTAUDIT"
                For Each row As DataRow In Detail.Rows
                    Try
                        query = String.Format("select top 1 AcctCode from OCRC where CardName='" & row("PaymentMethod").ToString.Replace("'", "''") & "'")
                        oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRecordSet.DoQuery(query)
                        '  dtGLMapping1 = cn.SAP_RunQuery(query)
                            If oRecordSet.RecordCount = 0 Then
                                Functions.WriteLog(row("ID").ToString + "Can not get GL Account in Credi Card Master:" & row("PaymentMethod") & "")
                                ErrMsg = "Can not get GL Account in Credi Card Master:" & row("PaymentMethod") & ""
                                Throw New Exception(ErrMsg)
                            End If
                            DebitAccount = oRecordSet.Fields.Item(0).Value 'dtGLMapping1.Rows(0)("AcctCode").ToString
                            ValidateAccount(DebitAccount, CreditAccount)
                            Try

                                ' oIncoming.JournalRemarks = "Night Audit Deposit"
                                oRecordSet.DoQuery("SELECT T0.[U_Value] FROM [dbo].[@ENTEROSETTING]  T0 WHERE T0.[Name] ='CLTax'")
                                If oRecordSet.RecordCount = 0 Then
                                    Throw New Exception("CLTax Not Defined in [@ENTEROSETTING] Table")
                                End If
                                Dim TAXOUTPUTCODE As String = oRecordSet.Fields.Item(0).Value
                                oIncoming.AccountPayments.AccountCode = CreditAccount '"X3"
                                'oIncoming.AccountPayments.VatGroup = TAXOUTPUTCODE
                                oIncoming.AccountPayments.GrossAmount = row("Amount")
                                oIncoming.AccountPayments.Add()

                                oRecordSet.DoQuery("SELECT T0.[CreditCard] FROM OCRC T0 WHERE T0.[CardName] ='" & row("PaymentMethod").ToString.Replace("'", "''") & "'")
                                If oRecordSet.RecordCount = 0 Then
                                    Throw New Exception("Credit Cared Name Not Defined in SAP")
                                End If
                                Dim VouchNo As String = row("PaymentBatchNo").ToString.Trim
                                If VouchNo = "" Then
                                    VouchNo = "XXXX"
                                End If
                                oIncoming.CreditCards.CreditCard = oRecordSet.Fields.Item(0).Value
                                oIncoming.CreditCards.CreditCardNumber = "XXXX"
                                oIncoming.CreditCards.CardValidUntil = CDate("9999-12-12") 'CDate(oDV_CreditCard.Item(0).Row(2).ToString)
                                oIncoming.CreditCards.CreditAcct = DebitAccount
                                oIncoming.CreditCards.VoucherNum = VouchNo
                                oIncoming.CreditCards.CreditSum = row("Amount")
                                oIncoming.CreditCards.Add()
                               
                            Catch ex As Exception
                                RetErrMsg = ex.Message
                            UpdateNightAuditPayment_PMSDeposit(header("ID"), ex.Message.Replace("'", "''"))
                            End Try

                    Catch ex As Exception

                        UpdateNightAuditPayment_PMSDeposit(header("ID"), ex.Message.Replace("'", "''"))
                        Functions.WriteLog(ex.Message)
                        Return ex.Message
                    End Try
                Next
                RetCode = oIncoming.Add()
                If RetCode <> 0 Then
                    PublicVariable.oCompanyInfo.GetLastError(RetCode, ErrMsg)
                    RetErrMsg = ErrMsg
                    UpdateNightAuditPayment_PMSDeposit(header("ID"), ErrMsg.Replace("'", "''"))
                    Return RetErrMsg
                End If
                UpdateNightAuditPayment_PMSDeposit(header("ID"), "")
            End If
            Return RetErrMsg
        Catch ex As Exception
            Functions.WriteLog(ex.Message)
            Return ex.Message
        End Try
        Return ""
    End Function
    Private Function CreateDO_CityLedgerSettlement(ByVal header As DataRow, ByVal Detail As DataTable) As String
        Try
            Dim cn As New Connection
            Dim dtGLMapping As DataTable = New DataTable("GLMapping")
            Dim dtGLMapping1 As DataTable = New DataTable("GLMapping")
            Dim DebitAccount As String = ""
            Dim CreditAccount As String = ""
            Dim CreditTaxAccount As String = ""
            Dim ds As DataSet = New DataSet
            Dim DocType As String = "30"
            Dim ret As String = ""
            Dim xm As New oXML
            Dim query = ""
            Dim ErrMsg As String = ""
            Dim RetCode As Integer = 0
            Dim RetErrMsg As String = ""
            Dim AccountCode As String = ""
            Dim TaxCode As String = ""
            Dim Amount As Decimal = 0
            For Each row As DataRow In Detail.Rows
                Try
                    If row("CompanyCode") = "" Then
                        Functions.WriteLog(row("ID").ToString + "Company Code Can not Be Empty in NightAuditInvHeader Table")
                        ErrMsg = "Company Code Can not Be Empty in NightAuditInvHeader Table"
                        Throw New Exception(ErrMsg)
                    Else

                        Dim oRecordSet As SAPbobsCOM.Recordset
                        oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRecordSet.DoQuery("select isnull(U_Value,'') U_Value from [@ENTEROSETTING] where Name='CityLedger'")
                        If oRecordSet.RecordCount = 0 Then
                            Functions.WriteLog(row("ID").ToString + "Can not get GL Account for City Ledger Settlement in [@ENTEROSETTING] Table")
                            ErrMsg = "Can not get GL Account for City Ledger Settlement in [@ENTEROSETTING] Table"
                            Throw New Exception(ErrMsg)
                        Else
                            AccountCode = oRecordSet.Fields.Item(0).Value
                            ValidateAccount(AccountCode)
                        End If
                        oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                        oRecordSet.DoQuery("select isnull(U_Value,'') U_Value from [@ENTEROSETTING] where Name='CLTax'")
                        If oRecordSet.RecordCount = 0 Then
                            Functions.WriteLog(row("ID").ToString + "Can not get GL TaxCode for City Ledger Settlement in [@ENTEROSETTING] Table")
                            ErrMsg = "Can not get TaxCode for City Ledger Settlement in [@ENTEROSETTING] Table"
                            Throw New Exception(ErrMsg)
                        Else
                            TaxCode = oRecordSet.Fields.Item(0).Value
                        End If
                        query = String.Format("select isnull(SUm(isnull(Price,0)+isnull(GSTAmount,0)+isnull(SCAmount,0)),0) 'Amount' from NightAuditInvDetail where HeaderID=" & row("ID") & "")
                        dtGLMapping = cn.Integration_RunQuery(query)
                        Amount = dtGLMapping.Rows(0)("Amount")


                        Dim oDeliveryOrder As SAPbobsCOM.Documents = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes)
                        oDeliveryOrder.DocDate = CDate(header("HotelDate"))
                        oDeliveryOrder.TaxDate = CDate(header("HotelDate"))
                        oDeliveryOrder.CardCode = row("CompanyCode")
                        oDeliveryOrder.Comments = "Night Audit City Ledger"
                        oDeliveryOrder.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service
                        oDeliveryOrder.Lines.ItemDescription = row("CardName")
                        oDeliveryOrder.Lines.AccountCode = AccountCode
                        oDeliveryOrder.Lines.LineTotal = Amount
                        oDeliveryOrder.Lines.VatGroup = TaxCode
                        oDeliveryOrder.Lines.Add()

                        oDeliveryOrder.Lines.ItemDescription = "Check in Date: " & row("CheckInDate") & " ;Check Out Date:" & row("CheckOutDate")
                        oDeliveryOrder.Lines.AccountCode = AccountCode
                        oDeliveryOrder.Lines.LineTotal = 0
                        oDeliveryOrder.Lines.VatGroup = TaxCode
                        oDeliveryOrder.Lines.Add()
                        oDeliveryOrder.Lines.ItemDescription = "ReservationCode: " & row("ReservationCode") & " ;Front Office Bill No.: " & row("BillNo")
                        oDeliveryOrder.Lines.AccountCode = AccountCode
                        oDeliveryOrder.Lines.LineTotal = 0
                        oDeliveryOrder.Lines.VatGroup = TaxCode
                        oDeliveryOrder.Lines.Add()

                        RetCode = oDeliveryOrder.Add()
                        If RetCode <> 0 Then
                            PublicVariable.oCompanyInfo.GetLastError(RetCode, ErrMsg)
                            RetErrMsg = ErrMsg
                            Throw New Exception(ErrMsg)
                        End If
                        UpdateNightAuditInvoice_CityLedgerSettlement(row("ID"), "")

                    End If


                Catch ex As Exception
                    UpdateNightAuditInvoice_CityLedgerSettlement(row("ID"), ex.Message.Replace("'", "''"))
                    Functions.WriteLog(ex.Message)
                    Return ex.Message
                End Try
            Next
            Return RetErrMsg

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Private Function CreateJEForRevenue_PMS(ByVal header As DataRow, ByVal Detail As DataTable) As String
        Try
            Dim cn As New Connection
            Dim dtGLMapping As DataTable = New DataTable("GLMapping")
            Dim query As String = ""
            Dim Amount As Double = 0
            Dim RetErrMsg As String = ""
            Dim ErrMsg As String = ""
            Dim GuestLedger As String = ""
            Dim GST As String = ""
            Dim ServiceCharge As String = ""
            Dim RetCode As Integer = 0
            Dim oRecordSet As SAPbobsCOM.Recordset
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='GuestLedger'")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Functions.WriteLog("Can not get GL Account for GuestLedger in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get GL Account for GuestLedger in [@ENTEROSETTING] Table"
                Throw New Exception(ErrMsg)
            End If
            GuestLedger = oRecordSet.Fields.Item(0).Value.ToString
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='GST'")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Functions.WriteLog("Can not get GL Account for GST in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get GL Account for GST in [@ENTEROSETTING] Table "
                Throw New Exception(ErrMsg)
            End If
            GST = oRecordSet.Fields.Item(0).Value.ToString

            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='ServiceCharge'")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Functions.WriteLog("Can not get GL Account for ServiceCharge in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get GL Account for ServiceCharge in [@ENTEROSETTING] Table "
                Throw New Exception(ErrMsg)
            End If
            ServiceCharge = oRecordSet.Fields.Item(0).Value.ToString
            ValidateAccount(GuestLedger, GST, ServiceCharge)

            Dim oJE As SAPbobsCOM.JournalEntries
            oJE = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)
            oJE.ReferenceDate = CDate(header("HotelDate"))
            oJE.TaxDate = CDate(header("HotelDate"))
            'oJE.Reference = "Night Audit Revenue " & Format(CDate(header("HotelDate")), "dd/MM/yy")
            oJE.Memo = "Night Audit Revenue " & Format(CDate(header("HotelDate")), "dd/MM/yy")

            oJE.Reference3 = "NIGHTAUDIT"

            'oIncoming.Remarks = "Night Audit Settlement " & Format(CDate(header("HotelDate")), "dd/MM/yy")
            'oIncoming.JournalRemarks = "Night Audit Settlement " & Format(CDate(header("HotelDate")), "dd/MM/yy")

            Dim RevenueAccount As String = ""
            Dim ProfitCode1 As String = ""
            Dim ProfitCode2 As String = ""
            Dim ProfitCode3 As String = ""
            Dim ProfitCode4 As String = ""
            Dim ProfitCode5 As String = ""
            Dim ZeroAmt As Double = 0

            Dim GSTTaxCode As String = ""

            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='GSTTaxCode'")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Functions.WriteLog("Can not get GSTTaxCode in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get GSTTaxCode in [@ENTEROSETTING] Table"
                Throw New Exception(ErrMsg)
            End If
            GSTTaxCode = oRecordSet.Fields.Item(0).Value.ToString

            Dim ZRTaxCode As String = ""

            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='ZRTaxCode'")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Functions.WriteLog("Can not get ZRTaxCode in [@ENTEROSETTING] Table")
                ErrMsg = "Can not get ZRTaxCode in [@ENTEROSETTING] Table"
                Throw New Exception(ErrMsg)
            End If
            ZRTaxCode = oRecordSet.Fields.Item(0).Value.ToString

            For Each row As DataRow In Detail.Rows

                If row("TCode").ToString <> "GSTZERO" Then
                    ProfitCode1 = ReturnProfitCode(row("ItemGroup").ToString.Trim, "1")
                    ProfitCode2 = ReturnProfitCode(row("ItemGroup").ToString.Trim, "2")
                    ProfitCode3 = ReturnProfitCode(row("ItemGroup").ToString.Trim, "3")
                    ProfitCode4 = ReturnProfitCode(row("ItemGroup").ToString.Trim, "4")
                    ProfitCode5 = row("MarketSegment").ToString.Trim
                    Double.TryParse(row("RevenueAmount").ToString, Amount)
                    If Amount <> 0 Then

                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        RevenueAccount = ReturnRevenue(row("ItemGroup").ToString.Trim)
                        ValidateAccount(RevenueAccount)
                        oJE.Lines.Credit = Amount
                        oJE.Lines.AccountCode = RevenueAccount
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()

                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        oJE.Lines.Debit = Amount
                        oJE.Lines.AccountCode = GuestLedger
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()
                        oJE.Lines.CostingCode = "BFast"
                    End If

                    Amount = 0
                    Double.TryParse(row("TaxAmount").ToString, Amount)
                    If Amount <> 0 Then
                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        oJE.Lines.Credit = Amount
                        oJE.Lines.AccountCode = GST
                        oJE.Lines.TaxGroup = GSTTaxCode
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()
                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        oJE.Lines.Debit = Amount
                        oJE.Lines.AccountCode = GuestLedger
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()

                    End If

                    Amount = 0
                    Double.TryParse(row("SCAmount").ToString, Amount)
                    If Amount <> 0 Then
                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        oJE.Lines.Credit = Amount
                        oJE.Lines.AccountCode = ServiceCharge
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()
                        oJE.Lines.AdditionalReference = "NIGHTAUDIT"
                        oJE.Lines.Debit = Amount
                        oJE.Lines.AccountCode = GuestLedger
                        oJE.Lines.CostingCode = ProfitCode1
                        oJE.Lines.CostingCode2 = ProfitCode2
                        oJE.Lines.CostingCode3 = ProfitCode3
                        oJE.Lines.CostingCode4 = ProfitCode4
                        oJE.Lines.CostingCode5 = ProfitCode5
                        oJE.Lines.Add()
                    End If
                Else
                    Double.TryParse(row("RevenueAmount").ToString, Amount)
                    ZeroAmt = ZeroAmt + Amount
                End If
            Next

            If ZeroAmt <> 0 Then
                oJE.Lines.Credit = 0
                oJE.Lines.AccountCode = GST
                oJE.Lines.TaxGroup = ZRTaxCode
                oJE.Lines.BaseSum = ZeroAmt
                oJE.Lines.CostingCode = ProfitCode1
                oJE.Lines.CostingCode2 = ProfitCode2
                oJE.Lines.CostingCode3 = ProfitCode3
                oJE.Lines.CostingCode4 = ProfitCode4
                oJE.Lines.CostingCode5 = ProfitCode5
                oJE.Lines.Add()
            End If
            RetCode = oJE.Add
            If RetCode <> 0 Then
                PublicVariable.oCompanyInfo.GetLastError(RetCode, ErrMsg)
                RetErrMsg = ErrMsg
                Throw New Exception(ErrMsg)
            End If
            UpdateNightAuditRevenue_PMS(header("ID"), "")
            Return ""
        Catch ex As Exception
            UpdateNightAuditRevenue_PMS(header("ID"), ex.Message.Replace("'", "''"))
            Return ex.Message

        End Try
    End Function
    Private Function CreateIncomingPayment_PMSSettlement(ByVal header As DataRow, ByVal Detail As DataTable) As String
        Try
            Dim cn As New Connection
            Dim dtGLMapping As DataTable = New DataTable("GLMapping")
            Dim dtGLMapping1 As DataTable = New DataTable("GLMapping")
            Dim DebitAccount As String = ""
            Dim CreditAccount As String = ""
            Dim CreditTaxAccount As String = ""
            Dim ds As DataSet = New DataSet
            Dim DocType As String = "30"
            Dim ret As String = ""
            Dim xm As New oXML
            Dim query = ""
            Dim ErrMsg As String = ""
            Dim RetCode As Integer = 0
            Dim RetErrMsg As String = ""
            Dim GuestLedger As String = ""
            Dim BillToRoom As String = ""
            Try

                Dim oRecordSet As SAPbobsCOM.Recordset
                oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                Dim oIncoming As SAPbobsCOM.Payments = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)
                oIncoming.DocType = SAPbobsCOM.BoRcptTypes.rAccount
                oIncoming.TaxDate = CDate(header("HotelDate"))
                oIncoming.DocDate = CDate(header("HotelDate"))
                ' oIncoming.Remarks = "Night Audit Settlement"
                oIncoming.UserFields.Fields.Item("U_Ref").Value = "NIGHTAUDIT"
                oIncoming.Remarks = "Night Audit Settlement " & Format(CDate(header("HotelDate")), "dd/MM/yy")
                oIncoming.JournalRemarks = "Night Audit Settlement " & Format(CDate(header("HotelDate")), "dd/MM/yy")

                oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='GuestLedger'")
                oRecordSet.DoQuery(query)
                If oRecordSet.RecordCount = 0 Then
                    Functions.WriteLog("Can not get GL Account for GuestLedger in [@ENTEROSETTING] Table")
                    ErrMsg = "Can not get GL Account for GuestLedger in [@ENTEROSETTING] Table"
                    Throw New Exception(ErrMsg)
                End If
                GuestLedger = oRecordSet.Fields.Item(0).Value.ToString
                oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                query = String.Format("select top 1 U_Value from [@ENTEROSETTING] where Name='BillToRoom'")
                oRecordSet.DoQuery(query)
                If oRecordSet.RecordCount = 0 Then
                    Functions.WriteLog("Can not get GL Account for BillToRoom in [@ENTEROSETTING] Table")
                    ErrMsg = "Can not get GL Account for BillToRoom in [@ENTEROSETTING] Table "
                    Throw New Exception(ErrMsg)
                End If
                BillToRoom = oRecordSet.Fields.Item(0).Value.ToString
                ValidateAccount(GuestLedger, BillToRoom)
                Dim GuestLedgerAmount As Decimal = 0
                Dim BillToRoomAmount As Decimal = 0

                query = String.Format("select Sum(Isnull(Amount,0)) Amount from NightPaymentClearing where HeaderID=" & header("ID") & " and Type='Guest Ledger'")
                dtGLMapping = cn.Integration_RunQuery(query)
                GuestLedgerAmount = dtGLMapping.Rows(0)("Amount")
                dtGLMapping.Clear()

                query = String.Format("select isnull(Sum(Isnull(Amount,0)),0) Amount from NightPaymentClearing where HeaderID=" & header("ID") & " and Type='BILL TO ROOM'")
                dtGLMapping = cn.Integration_RunQuery(query)
                If dtGLMapping.Rows.Count <> 0 Then
                    BillToRoomAmount = dtGLMapping.Rows(0)("Amount")
                End If
                dtGLMapping.Clear()
                oRecordSet.DoQuery("SELECT T0.[U_Value] FROM [dbo].[@ENTEROSETTING]  T0 WHERE T0.[Name] ='CLTax'")
                If oRecordSet.RecordCount = 0 Then
                    Throw New Exception("CLTax Not Defined in [@ENTEROSETTING] Table")
                End If
                Dim TAXOUTPUTCODE As String = oRecordSet.Fields.Item(0).Value
                If GuestLedgerAmount <> 0 Then
                    oIncoming.AccountPayments.AccountCode = GuestLedger '"X3"
                    oIncoming.AccountPayments.GrossAmount = GuestLedgerAmount
                    oIncoming.AccountPayments.Add()
                End If
               
                If BillToRoomAmount <> 0 Then
                    oIncoming.AccountPayments.AccountCode = BillToRoom '"X3"
                    oIncoming.AccountPayments.GrossAmount = BillToRoomAmount
                    oIncoming.AccountPayments.Add()
                End If
               
                For Each row As DataRow In Detail.Rows

                    query = String.Format("select top 1 AcctCode from OCRC where CardName='" & row("PaymentMethod").ToString.Replace("'", "''") & "'")
                    oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                    oRecordSet.DoQuery(query)
                    If oRecordSet.RecordCount = 0 Then
                        Functions.WriteLog(row("ID").ToString + "Can not get GL Account in Credi Card Master:" & row("PaymentMethod") & "")
                        ErrMsg = "Can not get GL Account in Credi Card Master:" & row("PaymentMethod") & ""
                        Throw New Exception(ErrMsg)
                    End If
                    DebitAccount = oRecordSet.Fields.Item(0).Value
                    ValidateAccount(DebitAccount)
                    oRecordSet.DoQuery("SELECT T0.[CreditCard] FROM OCRC T0 WHERE T0.[CardName] ='" & row("PaymentMethod").ToString.Replace("'", "''") & "'")
                    If oRecordSet.RecordCount = 0 Then
                        Throw New Exception("Credit Cared Name Not Defined in SAP")
                    End If
                    Dim VouchNo As String = row("PaymentBatchNo").ToString.Trim
                    If VouchNo = "" Then
                        VouchNo = "XXXX"
                    End If
                    oIncoming.CreditCards.CreditCard = oRecordSet.Fields.Item(0).Value
                    oIncoming.CreditCards.CreditCardNumber = "XXXX"
                    oIncoming.CreditCards.CardValidUntil = CDate("9999-12-12") 'CDate(oDV_CreditCard.Item(0).Row(2).ToString)
                    oIncoming.CreditCards.CreditAcct = DebitAccount
                    oIncoming.CreditCards.VoucherNum = VouchNo
                    oIncoming.CreditCards.CreditSum = row("Amount")
                    oIncoming.CreditCards.Add()
                Next


                RetCode = oIncoming.Add()
                If RetCode <> 0 Then
                    PublicVariable.oCompanyInfo.GetLastError(RetCode, ErrMsg)
                    RetErrMsg = ErrMsg
                    Throw New Exception(RetErrMsg)
                End If
                UpdateNightAuditPayment_PMSSettlement(header("ID"), "")
            Catch ex As Exception
                UpdateNightAuditPayment_PMSSettlement(header("ID"), ex.Message.Replace("'", "''"))
                Functions.WriteLog(ex.Message)
                Return ex.Message
            End Try
            Return RetErrMsg
        Catch ex As Exception
            Functions.WriteLog(ex.Message)
            Return ex.Message
        End Try
        Return ""
    End Function
    Private Sub UpdateNightAuditPayment_PMSDeposit(ByVal ID As Integer, ByVal mess As String)
        Dim cn As New Connection
        If mess = "" Then
            cn.Integration_RunQuery(String.Format("Update NightAuditPayment set ReceiveDate = GETDATE() , ErrMsg = '' where PaymentCategory ='ADV' and HEADERID = {0}", ID))
        Else
            cn.Integration_RunQuery(String.Format("Update NightAuditPayment set ErrMsg = '{0}' where PaymentCategory ='ADV' and HEADERID = {1}", mess, ID))
        End If
    End Sub
    Private Sub UpdateNightAuditPayment_PMSSettlement(ByVal ID As Integer, ByVal mess As String)
        Dim cn As New Connection
        If mess = "" Then
            cn.Integration_RunQuery(String.Format("Update NightAuditPayment set ReceiveDate = GETDATE() , ErrMsg = '' where HeaderID = {0} and PaymentCategory='SET'", ID))
        Else
            cn.Integration_RunQuery(String.Format("Update NightAuditPayment set ErrMsg = '{0}' where HeaderID = {1} and PaymentCategory='SET'", mess, ID))
        End If
    End Sub
    Private Sub UpdateNightAuditRevenue_PMS(ByVal ID As Integer, ByVal mess As String)
        Dim cn As New Connection
        If mess = "" Then
            cn.Integration_RunQuery(String.Format("Update NightAuditRevenue set ReceiveDate = GETDATE() , ErrMsg = '' where HeaderID = {0} ", ID))
        Else
            cn.Integration_RunQuery(String.Format("Update NightAuditRevenue set ErrMsg = '{0}' where HeaderID = {1} a", mess, ID))
        End If
    End Sub
    Private Sub UpdateNightAuditInvoice_CityLedgerSettlement(ByVal ID As Integer, ByVal mess As String)
        Dim cn As New Connection
        If mess = "" Then
            cn.Integration_RunQuery(String.Format("Update NightAuditInvHeader set ReceiveDate = GETDATE() , ErrMsg = '' where ID = {0}", ID))
        Else
            cn.Integration_RunQuery(String.Format("Update NightAuditInvHeader set ErrMsg = '{0}' where ID = {1}", mess, ID))
        End If
    End Sub
    Private Function ReturnRevenue(ByVal GroupCode As String) As String
        Try
            Dim query As String = ""
            Dim oRecordSet As SAPbobsCOM.Recordset
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If GroupCode = "B" Then
                query = String.Format("SELECT T0.[ItmsGrpCod] FROM OITM T0 WHERE T0.[ItemCode] ='ZRCXBFAST'")
                oRecordSet.DoQuery(query) 'ZRCKING01
                If oRecordSet.RecordCount = 0 Then
                    Return "Can not get GL Account for RevenueCode B"
                End If
                GroupCode = oRecordSet.Fields.Item(0).Value
            ElseIf GroupCode = "R" Or GroupCode = "" Then
                query = String.Format("SELECT T0.[ItmsGrpCod] FROM OITM T0 WHERE T0.[ItemCode] ='ZRCKING01'")
                oRecordSet.DoQuery(query)
                If oRecordSet.RecordCount = 0 Then
                    Return "Can not get GL Account for RevenueCode R"
                End If
                GroupCode = oRecordSet.Fields.Item(0).Value
            End If
            If GroupCode = "" Then
                Return "GroupCode Can't Be Empty"
            End If
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("SELECT T0.[RevenuesAc] FROM OITB T0 WHERE T0.[ItmsGrpCod] =" & GroupCode & "")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Return "Can not get GL Account for GroupCode " & GroupCode
            End If

            Return oRecordSet.Fields.Item(0).Value
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Private Function ReturnProfitCode(ByVal GroupCode As String, ByVal Dimen As String) As String
        Try
            Dim query As String = ""
            Dim oRecordSet As SAPbobsCOM.Recordset
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            If GroupCode = "B" Then
                query = String.Format("SELECT Top 1 T0.[ItmsGrpCod] FROM OITM T0 WHERE T0.[U_RevenueCode] ='B'")
                oRecordSet.DoQuery(query)
                If oRecordSet.RecordCount = 0 Then
                    Return ""
                End If
                GroupCode = oRecordSet.Fields.Item(0).Value
            ElseIf GroupCode = "R" Or GroupCode = "" Then
                query = String.Format("SELECT Top 1 T0.[ItmsGrpCod] FROM OITM T0 WHERE T0.[U_RevenueCode] ='R'")
                oRecordSet.DoQuery(query)
                If oRecordSet.RecordCount = 0 Then
                    Return ""
                End If
                GroupCode = oRecordSet.Fields.Item(0).Value
            End If
            If GroupCode = "" Then
                Return ""
            End If
            oRecordSet = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            query = String.Format("SELECT top 1 T0.[U_Value] FROM [dbo].[@COSTGROUP]  T0 WHERE T0.[U_Dim] ='" & Dimen & "' and  T0.[U_GroupCode]=" & GroupCode & "")
            oRecordSet.DoQuery(query)
            If oRecordSet.RecordCount = 0 Then
                Return ""
            End If
            Return oRecordSet.Fields.Item(0).Value.ToString.Trim
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Sub ValidateAccount(ByVal ParamArray arrAcct() As String)

        Dim cn As Connection = New Connection
        Dim dt As DataTable = New DataTable
        Dim result As Boolean = False
        Dim accCode As String = ""
        If arrAcct.Length > 0 Then
            For i As Integer = 0 To arrAcct.Length - 1
                accCode = arrAcct(i)
                dt = cn.SAP_RunQuery(String.Format("select AcctName from OACT with(nolock) where AcctCode = '{0}'", accCode))
                result = dt.Rows.Count = 1
                If Not result Then
                    Exit For
                End If
            Next
        End If
        If result = False Then
            Throw New Exception(accCode + " is not valid")
        End If

    End Sub
#End Region
#Region "CS"
   
#End Region

    Private Sub test()
        Dim oIncoming As SAPbobsCOM.Payments = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)
    End Sub
#End Region
End Class
