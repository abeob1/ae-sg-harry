﻿Module modIncomingPayments

    Function AR_IncomingPayment(ByRef oDVPayment As DataView, ByRef oDICompany As SAPbobsCOM.Company, _
                                ByVal sDocEntry As String, ByVal dIncomeDate As Date, _
                                ByVal sPOSNumber As String, ByVal sWhsCode As String, _
                               ByVal sCardCode As String, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim oIncomingPayment As SAPbobsCOM.Payments = Nothing
        Dim sPayDocEntry As String = String.Empty
        Dim oDTTmp As DataTable = Nothing
        Dim dPAyment As Double = 0
        Try
            sFuncName = "AR_IncomingPayment"
            '' Console.WriteLine("Starting Function", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            oDTTmp = New DataTable
            oDTTmp = oDVPayment.ToTable
            If oDTTmp.Rows.Count > 0 Then
                dPAyment = Convert.ToDecimal(oDTTmp.Compute("sum(PaymentAmt)", String.Empty).ToString)
            End If

            oIncomingPayment = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)

            Dim sCreditCard As String = String.Empty

            oIncomingPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer
            oIncomingPayment.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments
            oIncomingPayment.CardCode = CStr(sCardCode)
            oIncomingPayment.DocDate = dIncomeDate
            oIncomingPayment.DueDate = dIncomeDate
            oIncomingPayment.TaxDate = dIncomeDate
            oIncomingPayment.UserFields.Fields.Item("U_WHSCode").Value = sWhsCode
            ''  oIncomingPayment.CounterReference = Right(oDVPayment.Item(0).Row("POSTxNo").ToString.Trim, 8)

            If sDocEntry <> "" Then
                oIncomingPayment.Invoices.DocEntry = sDocEntry
                oIncomingPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice
                oIncomingPayment.Invoices.SumApplied = dPAyment
                oIncomingPayment.Invoices.DistributionRule = sWhsCode
                oIncomingPayment.Invoices.Add()
            End If

            For Each drv In oDVPayment
                If drv("PaymentAmt").ToString.Trim = 0.0 Then Continue For

                oIncomingPayment.CreditCards.CreditCard = drv("PaymentCode").ToString.Trim
                oIncomingPayment.CreditCards.CreditType = SAPbobsCOM.BoRcptCredTypes.cr_Regular
                oIncomingPayment.CreditCards.CardValidUntil = "01/12/9999"
                oIncomingPayment.CreditCards.CreditCardNumber = "1234" 'drv("CreditNumber").ToString.Trim
                '' oIncomingPayment.CreditCards.PaymentMethodCode = 0
                '' oIncomingPayment.CreditCards.VoucherNum = sWhsCode & "-" & CDate(dIncomeDate).ToString("yyMMdd") & "-" & sPOSNumber
                If String.IsNullOrEmpty(drv("BatchCode").ToString.Trim) Then
                    oIncomingPayment.CreditCards.VoucherNum = sWhsCode & "-" & CDate(dIncomeDate).ToString("yyMMdd")
                Else
                    oIncomingPayment.CreditCards.VoucherNum = drv("BatchCode").ToString.Trim
                End If

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Amount : " & CDbl(drv("PaymentAmt").ToString.Trim), sFuncName)
                oIncomingPayment.CreditCards.CreditSum = CDbl(drv("PaymentAmt").ToString.Trim)
                oIncomingPayment.CreditCards.Add()
            Next

            oIncomingPayment.CashSum = 0

            Console.WriteLine("Attempting to Add Payments ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Attempting to Add  ", sFuncName)
            lRetCode = oIncomingPayment.Add()

            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)

                AR_IncomingPayment = RTN_ERROR
            Else

                Console.WriteLine("Completed with SUCCESS " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS.", sFuncName)
                AR_IncomingPayment = RTN_SUCCESS

            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            AR_IncomingPayment = RTN_ERROR

        Finally
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Releasing the Objects", sFuncName)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment)
            oIncomingPayment = Nothing
        End Try
    End Function

    Function AR_IncomingPayment_Cancel(ByRef oDICompany As SAPbobsCOM.Company, _
                                   ByVal sIncomingPayment As String, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim oIncomingPayment As SAPbobsCOM.Payments
        Dim sSql As String = String.Empty
        oIncomingPayment = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments)

        Try
            sFuncName = "AR_IncomingPayment_Cancel"
            Console.WriteLine("Starting Function", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            If oIncomingPayment.GetByKey(sIncomingPayment) Then
                lRetCode = oIncomingPayment.Cancel()

                If lRetCode <> 0 Then
                    sErrDesc = oDICompany.GetLastErrorDescription
                    Call WriteToLogFile(sErrDesc, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                    AR_IncomingPayment_Cancel = RTN_ERROR
                Else
                    Console.WriteLine("Completed with SUCCESS " & oIncomingPayment.DocNum, sFuncName)
                    sErrDesc = String.Empty
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS.", sFuncName)
                    AR_IncomingPayment_Cancel = RTN_SUCCESS

                End If
            Else

                sErrDesc = "No matching records found in the Incoming Payments " & sIncomingPayment
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                AR_IncomingPayment_Cancel = RTN_ERROR
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            AR_IncomingPayment_Cancel = RTN_ERROR

        Finally
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Releasing the Objects", sFuncName)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment)
            oIncomingPayment = Nothing
        End Try
    End Function

  

    

End Module
