Imports System.Data.SqlClient
Imports System.Configuration


Module modCommon

    Function ExecuteSQLQuery_DT(ByVal sConnectionString As String, ByVal sQuery As String) As DataTable

        Dim oDT_INTDBInformations As DataTable
        Dim sFuncName As String = String.Empty
        Dim oConnection As SqlConnection = Nothing
        Dim oSQLCommand As SqlCommand = Nothing
        Dim oSQLAdapter As SqlDataAdapter = New SqlDataAdapter

        Try
            sFuncName = "ExecuteSQLQuery_DT()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            oConnection = New SqlConnection(sConnectionString)

            If (oConnection.State = ConnectionState.Closed) Then
                oConnection.Open()
            End If

            oDT_INTDBInformations = New DataTable
            oSQLCommand = New SqlCommand(sQuery, oConnection)
            oSQLAdapter.SelectCommand = oSQLCommand
            oSQLCommand.CommandTimeout = 0
            oSQLAdapter.Fill(oDT_INTDBInformations)


            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            Return oDT_INTDBInformations

        Catch ex As Exception
            Call WriteToLogFile(ex.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Return Nothing
        Finally
            oSQLAdapter.Dispose()
            oSQLCommand.Dispose()
            oConnection.Close()
        End Try
    End Function

    Function ExecuteSQLQuery_DT(ByVal sConnectionString As String, ByVal sQuery As String, ByRef sErrDesc As String) As Long


        Dim oDT_INTDBInformations As DataTable
        Dim sFuncName As String = String.Empty
        Dim oConnection As SqlConnection = Nothing
        Dim oSQLCommand As SqlCommand = Nothing
        Dim oSQLAdapter As SqlDataAdapter = New SqlDataAdapter

        Try
            sFuncName = "ExecuteSQLQuery_DT()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            oConnection = New SqlConnection(sConnectionString)

            If (oConnection.State = ConnectionState.Closed) Then
                oConnection.Open()
            End If

            oDT_INTDBInformations = New DataTable
            oSQLCommand = New SqlCommand(sQuery, oConnection)
            oSQLAdapter.SelectCommand = oSQLCommand
            oSQLCommand.CommandTimeout = 0
            oSQLCommand.ExecuteNonQuery()
            ''Try
            ''    oSQLAdapter.Fill(oDT_INTDBInformations)
            ''Catch ex As Exception
            ''End Try

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            Return RTN_SUCCESS

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(ex.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Return RTN_ERROR
        Finally
            oSQLAdapter.Dispose()
            oSQLCommand.Dispose()
            oConnection.Close()
        End Try
    End Function

    Function IntegrityValidation(ByVal oDT_Invoice As DataTable, ByVal oDT_InvoiceDetails As DataTable, ByVal oDT_Payments As DataTable, ByRef oDICompany As SAPbobsCOM.Company, _
                      ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim sDocEntry As String = String.Empty
        Dim sTransID As String = String.Empty
        Dim sWhsCode As String = String.Empty
        Dim sPOSNumber As String = String.Empty
        Dim oDV_InvoiceInform As DataView = Nothing
        Dim oDV_InvoiceDetails As DataView = Nothing
        Dim oDV_PaymentsInform As DataView = Nothing
        Dim oDV_ARInvoice As DataView = Nothing
        Dim oDT_Distinct As DataTable = New DataTable
        Dim oDT_InvoiceStatus As DataTable = New DataTable
        Dim sProductCode As String = String.Empty
        Dim sQuery As String = String.Empty
        Dim sErrDisplay As String = String.Empty
        Dim sManBatchItem As String = String.Empty
        Dim oBatchDT As DataTable = New DataTable
        Dim oARInvoice As SAPbobsCOM.Documents
        Dim sSQL As String = String.Empty
        Dim fError As Boolean = False
        Dim sItemCode As String = String.Empty

        Try
            sFuncName = "IntegrityValidation()"
            '  Console.WriteLine("Starting Function", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            oDT_InvoiceStatus.Columns.Add("HID", GetType(String))
            oDT_InvoiceStatus.Columns.Add("LItem", GetType(String))
            oDT_InvoiceStatus.Columns.Add("Status", GetType(String))
            oDT_InvoiceStatus.Columns.Add("HErrorMsg", GetType(String))
            oDT_InvoiceStatus.Columns.Add("LErrorMsg", GetType(String))
            oDT_InvoiceStatus.Columns.Add("Time", GetType(String))
            oDT_InvoiceStatus.Columns.Add("Docentry", GetType(String))
            oDT_InvoiceStatus.Columns.Add("DocNum", GetType(String))
            oDT_InvoiceStatus.Columns.Add("FileID", GetType(String))

            oDV_InvoiceInform = New DataView(oDT_Invoice)
            oDV_InvoiceDetails = New DataView(oDT_InvoiceDetails)
            oDV_PaymentsInform = New DataView(oDT_Payments)
            oDV_ARInvoice = New DataView(oDT_Arinvoice)
            ' oDT_Distinct = oDV_InvoiceInform.Table.DefaultView.ToTable(True, "HTransID")
            oDT_Distinct = oDV_InvoiceInform.Table.DefaultView.ToTable(True, "FileID")
            For imjs As Integer = 0 To oDT_Distinct.Rows.Count - 1

                ''''''''''--------------------------------------
                '''''---------- checking FileID exists or not
                ''''' -------------------------------------------

                'oDV_ARInvoice.RowFilter = "NumAtCard='" & oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim() & "'"
                'If oDV_ARInvoice.Count > 0 Then
                '    sErrDisplay = oDT_Distinct.Rows(imjs).Item("FileID") & " - File ID already exists in AR Invoice " & oDV_ARInvoice.Item(0)("DocNum")
                '    oDT_InvoiceStatus.Rows.Add(oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim, _
                '                                                                              "", "FAIL", _
                '                                                   sErrDisplay, "", Now.ToShortTimeString, "", "", oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim)
                '    fError = True
                '    GoTo ErrorDis
                'End If


                oDV_InvoiceInform.RowFilter = "FileID='" & oDT_Distinct.Rows(imjs).Item("FileID") & "'"
                ''''''''''--------------------------------------
                '''''----------  Payment Code Validation
                ''''' -------------------------------------------
                fError = False
                Console.WriteLine("Integration starts for the  FileID " & oDT_Distinct.Rows(imjs).Item("FileID"), sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Function AR_InvoiceCreation() TransID " & oDT_Distinct.Rows(imjs).Item("FileID"), sFuncName)
                oDV_InvoiceDetails.RowFilter = "FileID = '" & oDT_Distinct.Rows(imjs).Item("FileID") & "'"
                If oDV_InvoiceDetails.Count = 0 Then
                    sErrDisplay = "Line Information not found for File ID " & oDT_Distinct.Rows(imjs).Item("FileID")
                    oDT_InvoiceStatus.Rows.Add(oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim, _
                                                                                              "", "FAIL", _
                                                                   sErrDisplay, "", Now.ToShortTimeString, "", "", oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim)

                Else
                    sItemCode = String.Empty
                    For Each odrl As DataRowView In oDV_InvoiceDetails
                        If String.IsNullOrEmpty(odrl("POSItemCode").ToString.Trim) Then
                            sItemCode = sItemCode & "," & odrl("POSItemCode").ToString.Trim
                        End If
                    Next

                    If sItemCode.Length > 0 Then
                        sErrDisplay = sItemCode & " - Item Codes not mapped in SAP B1 for File ID " & oDT_Distinct.Rows(imjs).Item("FileID")
                        oDT_InvoiceStatus.Rows.Add(oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim, _
                                                                                                  "", "FAIL", _
                                                                       sErrDisplay, "", Now.ToShortTimeString, "", "", oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim)
                        fError = True
                        GoTo ErrorDis

                    End If

                    oDV_PaymentsInform.RowFilter = "FileID = '" & oDT_Distinct.Rows(imjs).Item("FileID") & "'"
                    If oDV_PaymentsInform.Count > 0 Then
                        For Each odvr As DataRowView In oDV_PaymentsInform
                            If String.IsNullOrEmpty(odvr("CardName").ToString.Trim) Then
                                sErrDisplay = "Credit Cards are not mapped in SAP B1 for File ID " & oDT_Distinct.Rows(imjs).Item("FileID")
                                oDT_InvoiceStatus.Rows.Add(oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim, _
                                                                                                          "", "FAIL", _
                                                                               sErrDisplay, "", Now.ToShortTimeString, "", "", oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim)
                                fError = True
                                Exit For
                            End If
                        Next
                    Else
                        sErrDisplay = "Payment Information Could Found in 'SAP_POS_PAYMENT' for File ID " & oDT_Distinct.Rows(imjs).Item("FileID")
                        oDT_InvoiceStatus.Rows.Add(oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim, _
                                                                                                       "", "FAIL", _
                                                                            sErrDisplay, "", Now.ToShortTimeString, "", "", oDT_Distinct.Rows(imjs).Item("FileID").ToString.Trim)
                        fError = True
                    End If
                End If

ErrorDis:

                If fError = True Then
                    If oDT_InvoiceStatus Is Nothing Then
                    Else
                        Dim sTrandID As String = String.Empty
                        Dim dSyncDatetime As DateTime

                        For imjd As Integer = 0 To oDT_InvoiceStatus.Rows.Count - 1

                            sSQL += "UPDATE [SalesTransHeader]" & _
    "SET [SAPSyncFileStatus] = '" & oDT_InvoiceStatus.Rows(imjd).Item("Status").ToString.Trim & "' ,[SAPSyncErrMsg] = '" & oDT_InvoiceStatus.Rows(imjd).Item("HErrorMsg").ToString.Trim & "' , " & _
    "[ERRDateTime] = GETDATE() " & _
    "WHERE [FileID] = '" & oDT_InvoiceStatus.Rows(imjd).Item("FileID").ToString.Trim & "' and [POSSyncCompletionStatus]=1"

                        Next imjd
                        oDT_InvoiceStatus.Clear()
                        sTrandID = String.Empty
                    End If

                    If sSQL.Length > 1 Then
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Validation Update SQL " & sSQL, sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Function ExecuteSQLQuery_DT() ", sFuncName)
                        Console.WriteLine("Calling Function ExecuteSQLQuery_DT()")
                        If ExecuteSQLQuery_DT(P_sConString, sSQL, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                    End If

                Else
                    oDT_InvoiceStatus.Clear()
                    oDV_InvoiceInform.RowFilter = "FileID = '" & oDT_Distinct.Rows(imjs).Item("FileID") & "'"
                    MarketingDocuments_Sync(oDV_InvoiceInform, oDV_InvoiceDetails, oDV_PaymentsInform, p_oCompany, oDT_InvoiceStatus, sErrDesc)

                End If


            Next

            Console.WriteLine("Completed with SUCCESS", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)


            Return RTN_SUCCESS
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            Console.WriteLine("Completed with ERROR", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_Status() Function" & sPOSNumber, sFuncName)
            ''  Update_Status(sTransID, sErrDesc, "FAIL", "", "SalesTransHDR")
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the Transaction. POS Number : " & sPOSNumber, sFuncName)
            If oDICompany.InTransaction = True Then oDICompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Disconnecting the Company and Release the Object ", sFuncName)
            p_oCompany.Disconnect()
            oDICompany.Disconnect()
            oDICompany = Nothing
            p_oCompany = Nothing
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Return RTN_ERROR

        End Try
    End Function

    Public Function MarketingDocuments_Sync(ByVal oDVARInvoice As DataView, ByVal oDVARInvoiceLine As DataView, ByVal oDVPayment As DataView, ByRef oCompany As SAPbobsCOM.Company, _
                                         ByVal oDTStatus As DataTable, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim dIncomeDate As Date
        Dim sPostxdatetime As String
        Dim dPostxdatetime As Date
        Dim tDocTime As DateTime
        Dim sWhsCode As String = String.Empty
        Dim sFileID As String = String.Empty
        Dim sQuery As String = String.Empty
        Dim sQueryup As String = String.Empty
        Dim sDocEntry As String = String.Empty
        Dim sDocNum As String = String.Empty
        Dim lRetCode As Integer
        Dim irow As Integer = 0
        Dim dDocTotal As Double = 0.0
        Dim sARInvoice As String = String.Empty
        oDTStatus.Clear()
        Dim sSql As String = String.Empty
        Dim sARInvoiceNo As String = String.Empty
        Dim sIncomingpaymentno As String = String.Empty
        Dim sARCreditnote As String = String.Empty
        Dim sOutgoingpayment As String = String.Empty

        Dim sCardCode As String = String.Empty
        Dim oDT_Payamount As DataTable = New DataTable
        Dim dPayamount As Double = 0

        If oDVPayment.Count > 0 Then
            oDT_Payamount = oDVPayment.ToTable
        End If

        Dim oRset As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oRset_Batch As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Try
            sFuncName = "MarketingDocuments_Sync()"
            ' Console.WriteLine("Starting Function ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function ", sFuncName)
            ''  dIncomeDate = Convert.ToDateTime(oDVARInvoice.Item(0).Row("DocDate").ToString.Trim)
            dIncomeDate = DateTime.ParseExact(oDVARInvoice.Item(0).Row("DocDate").ToString.Trim, "yyyyMMdd", Nothing)
            sFileID = CStr(oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
            sWhsCode = Left(CStr(oDVARInvoice.Item(0).Row("Outlet").ToString.Trim), 5)
            If String.IsNullOrEmpty(p_oCompDef.p_sCardCode) Then ''CD0001              
                sCardCode = "CASH"
            Else
                sCardCode = p_oCompDef.p_sCardCode
            End If

            '' AR Invoice & Incoming payments

            '************************************ AR Invoice Started ************************************************************************************

            If oDVARInvoice Is Nothing Then
                Console.WriteLine("No matching records found in Sales Header Table : AR Invoice DocEntry " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching records found in Sales Header Table : AR Invoice DocEntry " & sDocEntry, sFuncName)
            Else
                If oDVARInvoice.Count > 0 Then
                    Console.WriteLine("Calling Funcion AR_InvoiceCreation() " & sDocEntry, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Funcion AR_InvoiceCreation() : AR Invoice DocEntry " & sDocEntry, sFuncName)


                    If AR_InvoiceCreation(oDVARInvoice, oDVARInvoiceLine, oDVPayment, oCompany, sDocEntry, sDocNum, sCardCode, sErrDesc) <> RTN_SUCCESS Then
                        Call WriteToLogFile(sErrDesc, sFuncName)
                        Console.WriteLine("Completed with ERROR", sFuncName)
                        oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("Outlet").ToString.Trim, "", "FAIL", "AR Invoice:- " & sErrDesc, "", Now.ToShortTimeString, "", "", oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
                        Console.WriteLine("Rollback the SAP Transaction ", sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
                        If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        MarketingDocuments_Sync = Nothing
                        GoTo ERRORDISPLAY
                    End If


                Else
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching records found in Payement Table : AR Invoice DocEntry " & sDocEntry, sFuncName)
                    Console.WriteLine("No matching records found in Payement Table " & sDocEntry, sFuncName)
                End If
            End If



            '************************************ Incoming Payment Started ************************************************************************************
            If oDVPayment Is Nothing Then
                Console.WriteLine("No matching records found in Payement Table " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching records found in Payment Table : AR Invoice DocEntry " & sDocEntry, sFuncName)
                oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("FileID").ToString.Trim, "", "SUCCESS", "", "", Now.ToShortTimeString, sDocEntry, sDocNum, oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
            Else
                If oDVPayment.Count > 0 Then
                    Console.WriteLine("Calling Funcion AR_IncomingPayment() " & sDocEntry, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Funcion AR_IncomingPayment() : AR Invoice DocEntry " & sDocEntry, sFuncName)
                    If AR_IncomingPayment(oDVPayment, oCompany, sDocEntry, dIncomeDate, "" _
                                                             , sWhsCode, sCardCode, sErrDesc) <> RTN_SUCCESS Then

                        Call WriteToLogFile(sErrDesc, sFuncName)
                        oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("Outlet").ToString.Trim, "", "FAIL", "Incoming Payments :- " & sErrDesc, "", Now.ToShortTimeString, "", "", oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
                        Console.WriteLine("Completed with ERROR", sFuncName)
                        Console.WriteLine("Rollback the SAP Transaction ", sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
                        If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        MarketingDocuments_Sync = Nothing
                        ''  Return RTN_ERROR
                    Else

                        oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("FileID").ToString.Trim, "", "SUCCESS", "", "", Now.ToShortTimeString, sDocEntry, sDocNum, oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
                        ''  Return RTN_ERROR
                    End If
                Else
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching records found in Payement Table : AR Invoice DocEntry " & sDocEntry, sFuncName)
                    Console.WriteLine("No matching records found in Payement Table " & sDocEntry, sFuncName)
                    oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("FileID").ToString.Trim, "", "SUCCESS", "", "", Now.ToShortTimeString, sDocEntry, sDocNum, oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
                End If
            End If

            sErrDesc = ""
            ''  oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, "", "SUCCESS", "", "", Now.ToShortTimeString)

ERRORDISPLAY: If oDTStatus Is Nothing Then
            Else
                Dim sTrandID As String = String.Empty
                Dim dSyncDatetime As DateTime
                For imjs As Integer = 0 To oDTStatus.Rows.Count - 1

                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Date Time " & Now.Date & " " & oDTStatus.Rows(imjs).Item("Time").ToString.Trim, sFuncName)
                    dSyncDatetime = Now.Date & " " & oDTStatus.Rows(imjs).Item("Time").ToString.Trim
                    sQueryup += "UPDATE " & p_oCompDef.p_sIntDBName & ".. [SalesTransHeader]" & _
"SET [SAPSyncFileStatus] = '" & oDTStatus.Rows(imjs).Item("Status").ToString.Trim & "' ,[SAPSyncErrMsg] = '" & Replace(oDTStatus.Rows(imjs).Item("HErrorMsg").ToString.Trim, "'", "''") & "' , " & _
"[ERRDateTime] =  DATEADD(day,datediff(day,0,GETDATE()),0)  " & _
"WHERE [FileID] = '" & oDTStatus.Rows(imjs).Item("FileID").ToString.Trim & "' and [POSSyncCompletionStatus] = 1"



                Next imjs
                oDTStatus.Clear()
                sTrandID = String.Empty

            End If

            If sQueryup.Length > 1 Then
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Validation Update SQL " & sQueryup, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Executing Query", sFuncName)
                oRset.DoQuery(sQueryup)
            End If

            If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
            ''Console.WriteLine("Committed the Transaction for TransID " & oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Committed the Transaction Reference File ID : " & sFileID, sFuncName)
            ''System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
            '' If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Disconnecting the Company and Release the Object ", sFuncName)

            Return RTN_SUCCESS


        Catch ex As Exception
            sErrDesc = ex.Message
            Console.WriteLine("Completed with ERROR", sFuncName)
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Console.WriteLine("Rollback the SAP Transaction ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
            If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            Return RTN_ERROR
        End Try
    End Function

    Public Function GetSystemIntializeInfo(ByRef oCompDef As CompanyDefault, ByRef sErrDesc As String) As Long

        ' **********************************************************************************
        '   Function    :   GetSystemIntializeInfo()
        '   Purpose     :   This function will be providing information about the initialing variables
        '               
        '   Parameters  :   ByRef oCompDef As CompanyDefault
        '                       oCompDef =  set the Company Default structure
        '                   ByRef sErrDesc AS String 
        '                       sErrDesc = Error Description to be returned to calling function
        '               
        '   Return      :   0 - FAILURE
        '                   1 - SUCCESS
        '   Author      :   JOHN
        '   Date        :   MAY 2014
        ' **********************************************************************************

        Dim sFuncName As String = String.Empty
        Dim sConnection As String = String.Empty
        Dim sQuery As String = String.Empty
        Try

            sFuncName = "GetSystemIntializeInfo()"
            Console.WriteLine("Starting Function", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)


            oCompDef.p_sServerName = String.Empty
            oCompDef.p_sLicServerName = String.Empty
            oCompDef.p_sDBUserName = String.Empty
            oCompDef.p_sDBPassword = String.Empty

            oCompDef.p_sDataBaseName = String.Empty
            oCompDef.p_sSAPUserName = String.Empty
            oCompDef.p_sSAPPassword = String.Empty

            oCompDef.p_sLogDir = String.Empty
            oCompDef.p_sDebug = String.Empty
            oCompDef.p_sIntDBName = String.Empty
            oCompDef.p_sCardCode = String.Empty
            oCompDef.p_sDiscountItem = String.Empty
            oCompDef.p_sGLAccount = String.Empty
            oCompDef.p_szStips = String.Empty
            oCompDef.p_szSExcess = String.Empty
            oCompDef.p_szSRounding = String.Empty
            oCompDef.p_szSServiceCharge = String.Empty
            oCompDef.p_szeroTax = String.Empty

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Server")) Then
                oCompDef.p_sServerName = ConfigurationManager.AppSettings("Server")
            End If


            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("LicenseServer")) Then
                oCompDef.p_sLicServerName = ConfigurationManager.AppSettings("LicenseServer")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SAPDBName")) Then
                oCompDef.p_sDataBaseName = ConfigurationManager.AppSettings("SAPDBName")

            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SAPUserName")) Then
                oCompDef.p_sSAPUserName = ConfigurationManager.AppSettings("SAPUserName")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SAPPassword")) Then
                oCompDef.p_sSAPPassword = ConfigurationManager.AppSettings("SAPPassword")
            End If


            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DBUser")) Then
                oCompDef.p_sDBUserName = ConfigurationManager.AppSettings("DBUser")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DBPwd")) Then
                oCompDef.p_sDBPassword = ConfigurationManager.AppSettings("DBPwd")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("SQLType")) Then
                oCompDef.p_sSQLType = ConfigurationManager.AppSettings("SQLType")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("CardCode")) Then
                oCompDef.p_sCardCode = ConfigurationManager.AppSettings("CardCode")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("IntegrationDBName")) Then
                oCompDef.p_sIntDBName = ConfigurationManager.AppSettings("IntegrationDBName")
            End If


            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DiscountItem")) Then
                oCompDef.p_sDiscountItem = ConfigurationManager.AppSettings("DiscountItem")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("GLAccount")) Then
                oCompDef.p_sGLAccount = ConfigurationManager.AppSettings("GLAccount")
            End If


            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("zSExcess")) Then
                oCompDef.p_szSExcess = ConfigurationManager.AppSettings("zSExcess")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("zSRounding")) Then
                oCompDef.p_szSRounding = ConfigurationManager.AppSettings("zSRounding")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("zSServiceCharge")) Then
                oCompDef.p_szSServiceCharge = ConfigurationManager.AppSettings("zSServiceCharge")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("zStips")) Then
                oCompDef.p_szStips = ConfigurationManager.AppSettings("zStips")
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("ZeroTax")) Then
                oCompDef.p_szeroTax = ConfigurationManager.AppSettings("ZeroTax")
            End If

            ' folder
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("LogDir")) Then
                oCompDef.p_sLogDir = ConfigurationManager.AppSettings("LogDir")
            Else
                oCompDef.p_sLogDir = System.IO.Directory.GetCurrentDirectory()
            End If

            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Debug")) Then
                oCompDef.p_sDebug = ConfigurationManager.AppSettings("Debug")
                If p_oCompDef.p_sDebug.ToUpper = "ON" Then
                    p_iDebugMode = 1
                Else
                    p_iDebugMode = 0
                End If
            Else
                p_iDebugMode = 0
            End If

            P_sConString = String.Empty
            P_sConString = "Data Source=" & p_oCompDef.p_sServerName & ";Initial Catalog=" & p_oCompDef.p_sIntDBName & ";User ID=" & p_oCompDef.p_sDBUserName & "; Password=" & p_oCompDef.p_sDBPassword
            ''p_oCompDef.p_sDataBaseName
            sQuery = "SELECT [SeqID] ,[FileID] ,[Outlet] , CONVERT( NVARCHAR(20), [DocDate],112) [DocDate],[TotalGrossAmt],[SvcCharge],[GST],[Rounding],[Excess],[Tips],[Covers],[POSSyncCompletionStatus] " & _
                     " FROM [dbo].[SalesTransHeader] where [POSSyncCompletionStatus] = 1 and isnull([SAPSyncFileStatus],'')  <> 'SUCCESS' "

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching INT DB SalesTransHeader Query : " & sQuery, sFuncName)

            'Getting the Data from Invoice Table as DataSet
            ''  Console.WriteLine("Calling ExecuteSQLQuery_DT() ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)
            oDT_InvoiceHeader = ExecuteSQLQuery_DT(P_sConString, sQuery)

            sQuery = "SELECT [SeqID],[FileID],CONVERT( NVARCHAR(20), [BusinessDate],112) [BusinessDate], [OITM].[ItemCode] [POSItemCode],[POSItemCode] [POSCode],[Outlet],[Qty],[LineTotal],[DiscCode],[Disc],[SalesCategory],[POSSyncCompletionStatus] " & _
          " FROM [dbo].[SalesTransDetails] left join [" & p_oCompDef.p_sDataBaseName & "] ..[OITM] on [SalesTransDetails].[POSItemCode] COLLATE SQL_Latin1_General_CP850_CI_AS = [OITM].[U_RAPTOR_CODE]  where [FileID] in (SELECT [FileID] FROM [dbo].[SalesTransHeader] where [POSSyncCompletionStatus] = 1 and isnull([SAPSyncFileStatus],'')  <> 'SUCCESS') order by [FileID],[SeqID] "

          
            '  sQuery = "SELECT [SeqID],[FileID],CONVERT( NVARCHAR(20), [BusinessDate],112) [BusinessDate],  [POSItemCode],[Outlet],[Qty],[LineTotal],[DiscCode],[Disc],[SalesCategory],[POSSyncCompletionStatus] " & _
            '" FROM [dbo].[SalesTransDetails] where [FileID] in (SELECT [FileID] FROM [dbo].[SalesTransHeader] where [POSSyncCompletionStatus] = 1 and isnull([SAPSyncFileStatus],'')  <> 'SUCCESS') "

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching INT DB SalesTransDetails Query : " & sQuery, sFuncName)

            'Getting the Data from Invoice Table as DataSet
            '' Console.WriteLine("Calling ExecuteSQLQuery_DT() ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)
            oDT_InvoiceDetails = ExecuteSQLQuery_DT(P_sConString, sQuery)


            sQuery = "SELECT  [SalesOutlet],CONVERT( NVARCHAR(20), [BusinessDate],112) [BusinessDate] ,T0.[PaymentCode],T1.CardName  ,[PaymentAmt],[BatchCode] ,[POSSyncCompletionStatus] ,[FileID]  " & _
                    " FROM [CollectionDetails] T0 left join [" & p_oCompDef.p_sDataBaseName & "] ..[ocrc] T1 ON T0.PaymentCode COLLATE SQL_Latin1_General_CP850_CI_AS = T1.CreditCard "

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Payment Query : " & sQuery, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)
            oDT_PaymentData = ExecuteSQLQuery_DT(P_sConString, sQuery)

            sQuery = "SELECT T0.[WhsCode], T0.[WhsName] FROM [" & p_oCompDef.p_sDataBaseName & "].. OWHS T0"

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Warehouse : " & sQuery, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)
            oDT_Warehouse = ExecuteSQLQuery_DT(P_sConString, sQuery)



            sQuery = "SELECT T1.[ItemCode], T0.[U_CATER_SALES], T0.[U_CATER_COGS] FROM [" & p_oCompDef.p_sDataBaseName & "].. OITB T0  INNER JOIN [" & p_oCompDef.p_sDataBaseName & "].. OITM T1 ON T0.[ItmsGrpCod] = T1.[ItmsGrpCod]"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item Group : " & sQuery, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)

            oDT_ItemGroup = ExecuteSQLQuery_DT(P_sConString, sQuery)


            sQuery = "SELECT T0.[NumAtCard], T0.[DocNum] FROM [" & p_oCompDef.p_sDataBaseName & "].. OINV T0"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item Group : " & sQuery, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)

            oDT_Arinvoice = ExecuteSQLQuery_DT(P_sConString, sQuery)

            ' AE_STUTTGART_DLL.p_iDebugMode = p_iDebugMode

            'IntegrationDBName

            Console.WriteLine("Completed with SUCCESS ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            GetSystemIntializeInfo = RTN_SUCCESS

        Catch ex As Exception
            WriteToLogFile(ex.Message, sFuncName)
            Console.WriteLine("Completed with ERROR ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            GetSystemIntializeInfo = RTN_ERROR
        End Try
    End Function

    Public Function ConnectToTargetCompany(ByRef oCompany As SAPbobsCOM.Company, _
                                          ByRef sErrDesc As String) As Long

        ' **********************************************************************************
        '   Function    :   ConnectToTargetCompany()
        '   Purpose     :   This function will be providing to proceed the connectivity of 
        '                   using SAP DIAPI function
        '               
        '   Parameters  :   ByRef oCompany As SAPbobsCOM.Company
        '                       oCompany =  set the SAP DI Company Object
        '                   ByRef sErrDesc AS String 
        '                       sErrDesc = Error Description to be returned to calling function
        '               
        '   Return      :   0 - FAILURE
        '                   1 - SUCCESS
        '   Author      :   JOHN
        '   Date        :   MAY 2013 21
        ' **********************************************************************************

        Dim sFuncName As String = String.Empty
        Dim iRetValue As Integer = -1
        Dim iErrCode As Integer = -1
        Dim sSQL As String = String.Empty
        Dim oDs As New DataSet

        Try
            sFuncName = "ConnectToTargetCompany()"
            ' Console.WriteLine("Starting function", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)
            ' Console.WriteLine("Initializing the Company Object", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Initializing the Company Object", sFuncName)

            oCompany = New SAPbobsCOM.Company
            ' Console.WriteLine("Assigning the representing database name", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Assigning the representing database name", sFuncName)

            oCompany.Server = p_oCompDef.p_sServerName
            oCompany.LicenseServer = p_oCompDef.p_sLicServerName
            oCompany.DbUserName = p_oCompDef.p_sDBUserName
            oCompany.DbPassword = p_oCompDef.p_sDBPassword
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English
            oCompany.UseTrusted = False

            If p_oCompDef.p_sSQLType = 2012 Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
            ElseIf p_oCompDef.p_sSQLType = 2008 Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
            ElseIf p_oCompDef.p_sSQLType = 2014 Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014
            ElseIf p_oCompDef.p_sSQLType = 2016 Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016
            End If

            oCompany.CompanyDB = p_oCompDef.p_sDataBaseName
            oCompany.UserName = p_oCompDef.p_sSAPUserName
            oCompany.Password = p_oCompDef.p_sSAPPassword

            Console.WriteLine("Connecting to the Company Database. " & p_oCompDef.p_sDataBaseName, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Connecting to the Company Database.  " & p_oCompDef.p_sDataBaseName, sFuncName)
            oCompany.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode
            iRetValue = oCompany.Connect()

            If iRetValue <> 0 Then
                oCompany.GetLastError(iErrCode, sErrDesc)

                sErrDesc = String.Format("Connection to Database ({0}) {1} {2} {3}", _
                    oCompany.CompanyDB, System.Environment.NewLine, _
                                vbTab, sErrDesc)

                Throw New ArgumentException(sErrDesc)
            End If
            Console.WriteLine("Completed with SUCCESS " & p_oCompDef.p_sDataBaseName, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            ConnectToTargetCompany = RTN_SUCCESS
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(ex.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            ConnectToTargetCompany = RTN_ERROR
        End Try
    End Function

    Public Function GetSingleValue(ByVal Query As String, ByRef p_oDICompany As SAPbobsCOM.Company, ByRef sErrDesc As String) As String

        ' ***********************************************************************************
        '   Function   :    GetSingleValue()
        '   Purpose    :    This function is handles - Return single value based on Query
        '   Parameters :    ByVal Query As String
        '                       sDate = Passing Query 
        '                   ByRef oCompany As SAPbobsCOM.Company
        '                       oCompany = Passing the Company which has been connected
        '                   ByRef sErrDesc As String
        '                       sErrDesc=Error Description to be returned to calling function
        '   Author     :    SRINIVASAN
        '   Date       :    15/08/2014 
        '   Change     :   
        '                   
        ' ***********************************************************************************

        Dim sFuncName As String = String.Empty

        Try
            sFuncName = "GetSingleValue()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Query : " & Query, sFuncName)

            Dim objRS As SAPbobsCOM.Recordset = p_oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            objRS.DoQuery(Query)
            If objRS.RecordCount > 0 Then
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
                GetSingleValue = RTN_SUCCESS

                Return objRS.Fields.Item(0).Value.ToString
            End If
        Catch ex As Exception
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            WriteToLogFile(ex.Message, sFuncName)
            GetSingleValue = RTN_SUCCESS
            Return ""
        End Try
        Return Nothing
    End Function

    Public Function ConvertRecordset(ByVal SAPRecordset As SAPbobsCOM.Recordset, ByRef sErrDesc As String) As DataTable

        '\ This function will take an SAP recordset from the SAPbobsCOM library and convert it to a more
        '\ easily used ADO.NET datatable which can be used for data binding much easier.
        Dim sFuncName As String = String.Empty
        Dim dtTable As New DataTable
        Dim NewCol As DataColumn
        Dim NewRow As DataRow
        Dim ColCount As Integer

        Try
            sFuncName = "ConvertRecordset()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            For ColCount = 0 To SAPRecordset.Fields.Count - 1
                NewCol = New DataColumn(SAPRecordset.Fields.Item(ColCount).Name)
                dtTable.Columns.Add(NewCol)
            Next

            Do Until SAPRecordset.EoF

                NewRow = dtTable.NewRow
                'populate each column in the row we're creating
                For ColCount = 0 To SAPRecordset.Fields.Count - 1

                    NewRow.Item(SAPRecordset.Fields.Item(ColCount).Name) = SAPRecordset.Fields.Item(ColCount).Value

                Next

                'Add the row to the datatable
                dtTable.Rows.Add(NewRow)


                SAPRecordset.MoveNext()
            Loop

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            Return dtTable

        Catch ex As Exception

            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Return Nothing

        End Try

    End Function

End Module
