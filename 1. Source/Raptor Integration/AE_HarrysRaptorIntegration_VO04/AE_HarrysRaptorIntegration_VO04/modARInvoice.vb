Module modARInvoice


    Public Function AR_InvoiceCreation(ByVal oDVARInvoice As DataView, ByVal oDVARInvoiceLine As DataView, ByVal oDVPayment As DataView, ByRef oCompany As SAPbobsCOM.Company, _
                                       ByRef sDocEntry As String, ByRef sDocNum As String, ByVal sCardCode As String, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim oARInvoice As SAPbobsCOM.Documents = Nothing
        Dim oARInvoice_Doc As SAPbobsCOM.Documents = Nothing
        Dim dIncomeDate As Date
        Dim tDocTime As DateTime
        Dim sWhsCode As String = String.Empty
        Dim sWhsCodeL As String = String.Empty
        Dim sFileID As String = String.Empty
        Dim sProductCode As String = String.Empty
        Dim sBOMCode As String = String.Empty
        Dim sQuery As String = String.Empty
        Dim sQueryup As String = String.Empty
        Dim sManBatchItem As String = String.Empty
        Dim oBatchDT As DataTable = Nothing
        Dim dBatchQuantity As Double = 0
        Dim dRemBatchQuantity As Double = 0
        Dim dBatchNumber As String = String.Empty
        Dim dInvQuantity As Double
        Dim lRetCode As Integer
        Dim irow As Integer = 0
        Dim dDocTotal As Double = 0.0
        Dim oDV_BOM As DataView = New DataView(oDT_BOM)
        Dim oDT_Batch As DataTable = New DataTable
        Dim oDV_Batch As DataView = Nothing
        Dim oRow() As Data.DataRow = Nothing
        Dim SARDraft As String = String.Empty
        Dim dPostxdatetime As Date
        Dim oDT_Payamount As DataTable = New DataTable
        Dim dPayamount As Double = 0
        Dim dHDocTotal As Double = 0
        oDT_Payamount = oDVPayment.ToTable
        Dim fBatch As Boolean = False
        Dim oDT_Distinct As DataTable = New DataTable
        Dim DQTY As Double = 0
        Dim dPrice As Double = 0
        Dim oDTtmp As DataTable = Nothing
        Dim oDVTmp As DataView = Nothing
        Dim oDVItemGroup As DataView = Nothing
        Dim sprojectcode As String = String.Empty
        Dim dPOSDocTotal As Double = 0
        Dim sDiscItemCodes() As String = New String() {"HDUTY", "HSPOI", "HMKTG", "HFLUS", "HENTM"}
        If oDT_Payamount.Rows.Count > 0 Then
            dPayamount = Convert.ToDecimal(oDT_Payamount.Compute("sum(PaymentAmt)", String.Empty).ToString)
        End If


        oDT_Batch.Columns.Add("ItemCode", GetType(String))
        oDT_Batch.Columns.Add("BatchNum", GetType(String))
        oDT_Batch.Columns.Add("Quantity", GetType(Decimal))
        '' oDT_Batch.Columns.Add("date", GetType(Date))


        Dim oRset As SAPbobsCOM.Recordset = Nothing
        Dim oRset_Batch As SAPbobsCOM.Recordset = Nothing

        Try

            oDVItemGroup = New DataView(oDT_ItemGroup)

            oDVTmp = New DataView
            oDVTmp = oDVARInvoiceLine
            oARInvoice = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
            oARInvoice_Doc = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)

            oRset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRset_Batch = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            sFuncName = "AR_InvoiceCreation()"

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function ", sFuncName)

            dIncomeDate = DateTime.ParseExact(oDVARInvoice.Item(0).Row("DocDate").ToString.Trim, "yyyyMMdd", Nothing)
            sFileID = CStr(oDVARInvoice.Item(0).Row("FileID").ToString.Trim)
            sWhsCode = CStr(oDVARInvoice.Item(0).Row("Outlet").ToString.Trim)
            dPOSDocTotal = CDbl(oDVARInvoice.Item(0).Row("TotalGrossAmt").ToString.Trim)
            oARInvoice.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInvoices

            oARInvoice.CardCode = sCardCode
            oARInvoice.DocDate = dIncomeDate
            oARInvoice.DocDueDate = dIncomeDate
            oARInvoice.TaxDate = dIncomeDate
            oARInvoice.NumAtCard = sFileID ''sWhsCode & " - " & CStr(CDate(dIncomeDate).ToString("yyyyMMdd"))
            ''sWhsCode & " - " & sPOSNumber

            If Not String.IsNullOrEmpty(sWhsCode) Then
                oARInvoice.UserFields.Fields.Item("U_Outlet").Value = sWhsCode
            End If
            oARInvoice.UserFields.Fields.Item("U_Covers").Value = oDVARInvoice.Item(0).Row("Covers").ToString.Trim
            oDT_Distinct = oDVARInvoiceLine.ToTable
            
            For Each dvr As DataRowView In oDVARInvoiceLine
                '' If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("SAPItemCode  " & dvr("POSItemCode").ToString.Trim & "  -  " & "  Qty " & DQTY & "  -  " & " Unit Price " & dPrice, sFuncName)

                If Not String.IsNullOrEmpty(dvr("POSItemCode").ToString.Trim) Then
                    oARInvoice.Lines.ItemCode = dvr("POSItemCode").ToString.Trim
                Else
                    sErrDesc = dvr("POSCode").ToString.Trim & " - Raptor Code not matched in SAP " & dvr("POSItemCode").ToString.Trim
                    Return RTN_ERROR
                End If

                oARInvoice.Lines.Quantity = Math.Abs(CDbl(dvr("Qty").ToString.Trim))
                sWhsCodeL = Left(dvr("Outlet").ToString.Trim, 5)
                oDT_Warehouse.DefaultView.RowFilter = "WhsCode='" & sWhsCodeL & "'"
                If oDT_Warehouse.DefaultView.Count = 0 Then
                    sErrDesc = "No matching records found in OWHS table " & sWhsCodeL
                    Return RTN_ERROR
                End If
                ''HMKTG
                ''  for HSPOI, HFLUS, HDUTY, HMKTN, BOHKITUSE, MNGRRECOVERY, RND, LNDTRAINING, LNDAUDIT
                 
                Select Case dvr("DiscCode").ToString.Trim
                    Case "HSPOI", "HFLUS", "HDUTY", "HMKTN", "BOHKITUSE", "MNGRRECOVERY", "RND", "LNDTRAINING", "LNDAUDIT"
                        oARInvoice.Lines.LineTotal = 0
                        oDVItemGroup.RowFilter = "ItemCode='" & dvr("POSItemCode").ToString.Trim & "'"
                        If oDVItemGroup.Count > 0 Then
                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("DiscCode  " & dvr("DiscCode").ToString.Trim, sFuncName)
                            If Not String.IsNullOrEmpty(oDVItemGroup.Item(0)(dvr("DiscCode").ToString.Trim).ToString) Then
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("COGSAccountCode  " & oDVItemGroup.Item(0)(dvr("DiscCode").ToString.Trim).ToString, sFuncName)
                                oARInvoice.Lines.COGSAccountCode = oDVItemGroup.Item(0)(dvr("DiscCode").ToString.Trim).ToString
                            Else
                                sErrDesc = "COGS Account not found in the ItemGroup " & oDVItemGroup.Item(0)("ItmsGrpNam").ToString & " for this ItemCode " & dvr("POSItemCode").ToString.Trim
                                Return RTN_ERROR
                            End If
                        End If
                    Case Else
                        oARInvoice.Lines.LineTotal = CDbl(dvr("LineTotal").ToString.Trim)
                End Select
                oARInvoice.Lines.CostingCode = sWhsCodeL
                oARInvoice.Lines.COGSCostingCode = sWhsCodeL
                oARInvoice.Lines.WarehouseCode = sWhsCodeL
                oARInvoice.Lines.UserFields.Fields.Item("U_DiscCode").Value = dvr("DiscCode").ToString.Trim ''Outlet
                If Not String.IsNullOrEmpty(dvr("DiscCode").ToString.Trim) Then
                    oARInvoice.Lines.UserFields.Fields.Item("U_DiscItem").Value = dvr("POSItemCode").ToString.Trim ''Outlet
                End If

                Select Case dvr("SalesCategory").ToString.Trim.ToUpper
                    Case "C"
                        oDVItemGroup.RowFilter = "ItemCode='" & dvr("POSItemCode").ToString.Trim & "'"
                        If oDVItemGroup.Count > 0 Then
                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("SalesCategory  " & dvr("SalesCategory").ToString.Trim.ToUpper, sFuncName)
                            oARInvoice.Lines.AccountCode = oDVItemGroup.Item(0)("U_CATER_SALES").ToString
                            oARInvoice.Lines.COGSAccountCode = oDVItemGroup.Item(0)("U_CATER_COGS").ToString
                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(oDVItemGroup.Item(0)("U_CATER_SALES").ToString & "   -   " & oDVItemGroup.Item(0)("U_CATER_COGS").ToString, sFuncName)
                
                        End If
                    Case "X"

                    Case Else

                        If Left(dvr("SalesCategory").ToString.Trim.ToUpper, 1) = "X" Then
                            sprojectcode = Right(dvr("SalesCategory").ToString.Trim, dvr("SalesCategory").ToString.Trim.Length - 1)
                            oARInvoice.Lines.ProjectCode = sprojectcode

                        ElseIf Left(dvr("SalesCategory").ToString.Trim.ToUpper, 1) = "C" Then

                            sprojectcode = Right(dvr("SalesCategory").ToString.Trim, dvr("SalesCategory").ToString.Trim.Length - 1)
                            oARInvoice.Lines.ProjectCode = sprojectcode
                            oDVItemGroup.RowFilter = "ItemCode='" & dvr("POSItemCode").ToString.Trim & "'"
                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("SalesCategory  " & dvr("SalesCategory").ToString.Trim.ToUpper, sFuncName)
                            If oDVItemGroup.Count > 0 Then
                                ''  If Not String.IsNullOrEmpty(oDVItemGroup.Item(0)("U_CATER_SALES").ToString) Then
                                oARInvoice.Lines.AccountCode = oDVItemGroup.Item(0)("U_CATER_SALES").ToString
                                ''End If
                                '' If Not String.IsNullOrEmpty(oDVItemGroup.Item(0)("U_CATER_COGS").ToString) Then
                                oARInvoice.Lines.COGSAccountCode = oDVItemGroup.Item(0)("U_CATER_COGS").ToString
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(oDVItemGroup.Item(0)("U_CATER_SALES").ToString & "   -   " & oDVItemGroup.Item(0)("U_CATER_COGS").ToString, sFuncName)
                                ''End If
                            End If
                        End If
                End Select
                oARInvoice.Lines.Add()

                'For Each odvr As DataRowView In oDVTmp
                If Not String.IsNullOrEmpty(dvr("DiscCode").ToString.Trim) Then
                    oARInvoice.Lines.ItemCode = p_oCompDef.p_sDiscountItem
                    Select Case dvr("DiscCode").ToString.Trim
                     
                        Case "HSPOI", "HFLUS", "HDUTY", "HMKTN", "BOHKITUSE", "MNGRRECOVERY", "RND", "LNDTRAINING", "LNDAUDIT"

                        Case Else
                            oARInvoice.Lines.Quantity = -1
                            oARInvoice.Lines.UnitPrice = CDbl(dvr("Disc").ToString.Trim)
                            oARInvoice.Lines.WarehouseCode = sWhsCodeL
                            oARInvoice.Lines.CostingCode = sWhsCodeL
                            oARInvoice.Lines.COGSCostingCode = sWhsCodeL
                            oARInvoice.Lines.Add()
                    End Select
                End If
            Next
            
            If CDbl(oDVARInvoice.Item(0).Row("Tips").ToString.Trim) <> 0.0 Then
                oARInvoice.Lines.ItemCode = p_oCompDef.p_szStips      'dvr("POSItemCode").ToString.Trim
                If CDbl(oDVARInvoice.Item(0).Row("Tips").ToString.Trim) > 0 Then
                    oARInvoice.Lines.Quantity = 1
                Else
                    oARInvoice.Lines.Quantity = -1
                End If
                oARInvoice.Lines.LineTotal = Math.Abs(CDbl(oDVARInvoice.Item(0).Row("Tips").ToString.Trim))
                oARInvoice.Lines.VatGroup = p_oCompDef.p_szeroTax
                oARInvoice.Lines.WarehouseCode = sWhsCodeL
                oARInvoice.Lines.CostingCode = sWhsCodeL
                oARInvoice.Lines.COGSCostingCode = sWhsCodeL
                oARInvoice.Lines.Add()
            End If

            If CDbl(oDVARInvoice.Item(0).Row("Excess").ToString.Trim) <> 0.0 Then
                oARInvoice.Lines.ItemCode = p_oCompDef.p_szSExcess     'dvr("POSItemCode").ToString.Trim
                If CDbl(oDVARInvoice.Item(0).Row("Excess").ToString.Trim) > 0 Then
                    oARInvoice.Lines.Quantity = 1
                Else
                    oARInvoice.Lines.Quantity = -1
                End If
                oARInvoice.Lines.LineTotal = Math.Abs(CDbl(oDVARInvoice.Item(0).Row("Excess").ToString.Trim))
                oARInvoice.Lines.VatGroup = p_oCompDef.p_szeroTax
                oARInvoice.Lines.WarehouseCode = sWhsCodeL
                oARInvoice.Lines.CostingCode = sWhsCodeL
                oARInvoice.Lines.COGSCostingCode = sWhsCodeL
                oARInvoice.Lines.Add()
            End If

            If CDbl(oDVARInvoice.Item(0).Row("SvcCharge").ToString.Trim) <> 0.0 Then
                oARInvoice.Lines.ItemCode = p_oCompDef.p_szSServiceCharge    'dvr("POSItemCode").ToString.Trim
                If CDbl(oDVARInvoice.Item(0).Row("SvcCharge").ToString.Trim) > 0 Then
                    oARInvoice.Lines.Quantity = 1
                Else
                    oARInvoice.Lines.Quantity = -1
                End If
                oARInvoice.Lines.LineTotal = Math.Abs(CDbl(oDVARInvoice.Item(0).Row("SvcCharge").ToString.Trim))
                oARInvoice.Lines.WarehouseCode = sWhsCodeL
                oARInvoice.Lines.CostingCode = sWhsCodeL
                oARInvoice.Lines.COGSCostingCode = sWhsCodeL
                oARInvoice.Lines.Add()
            End If

            ''oARInvoice.DocTotal = oDVARInvoice.Item(0).Row("HDocTotal").ToString.Trim

            If oCompany.InTransaction = False Then oCompany.StartTransaction()
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Attempting to Add Draft ", sFuncName)
            oARInvoice.SaveXML(p_oCompDef.p_sLogDir & "\ARNV.XML")
            Console.WriteLine("Attempting AR Invoice Draft", sFuncName)
            lRetCode = oARInvoice.Add()

            If lRetCode <> 0 Then
                sErrDesc = oCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR ", sFuncName)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                Return RTN_ERROR

            Else
                '' System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                ''oARInvoice = Nothing
                '----------------- AR Invoice Draft Created Successfully
                Console.WriteLine("Completed with SUCCESS", sFuncName)
                oCompany.GetNewObjectCode(sDocEntry)
                Console.WriteLine("Draft Added Successfully " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Draft Added Successfully  " & sDocEntry, sFuncName)
                ''  Console.WriteLine("Assigning Batch   " & sDocEntry, sFuncName)

                SARDraft = sDocEntry
                Console.WriteLine("Updating COGS Account " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Updating COGS Account  " & sDocEntry, sFuncName)
                '' Console.WriteLine("Assigning Batch   " & sDocEntry, sFuncName)
                If oARInvoice.GetByKey(sDocEntry) Then
                    dDocTotal = oARInvoice.DocTotal
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("dDocTotal  " & dDocTotal, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("dPOSDocTotal  " & dPOSDocTotal, sFuncName)
                    dDocTotal = dDocTotal - dPOSDocTotal
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("dDocTotal  " & dDocTotal, sFuncName)
                    If Update_Draft(oCompany, oARInvoice, dDocTotal, sWhsCodeL, "", sErrDesc) <> RTN_SUCCESS Then
                        Call WriteToLogFile(sErrDesc, sFuncName)
                        Console.WriteLine("Completed with ERROR (Updating COGS Account) " & sErrDesc, sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR (Updating COGS Account) ", sFuncName)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                        Return RTN_ERROR
                    End If
                End If
                Console.WriteLine("Completed with SUCCESS", sFuncName)
                Console.WriteLine("Attempting to Convert as a AR Invoice Document", sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS (Update AR Invoice Draft) ", sFuncName)

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Attempting to Convert as a AR Invoice Document ", sFuncName)

                lRetCode = oARInvoice.SaveDraftToDocument()

                If lRetCode <> 0 Then
                    sErrDesc = oCompany.GetLastErrorDescription
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Exception " & sErrDesc, sFuncName)
                    If Left(sErrDesc.ToUpper(), 14) = "INTERNAL ERROR" Then
                        sErrDesc = "Quantity falls into negative inventory  [INV1.ItemCode][line: 2]"
                    End If
                    Call WriteToLogFile(sErrDesc, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR (Convert as a AR Invoice Document) ", sFuncName)
                    Return RTN_ERROR
                End If
                oCompany.GetNewObjectCode(sDocEntry)
                oARInvoice_Doc.GetByKey(sDocEntry)
                sDocNum = oARInvoice_Doc.DocNum
                Console.WriteLine("Converted To AR Invoice Successful " & sDocEntry, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS (Convert as a AR Invoice Document) " & sDocEntry, sFuncName)

                sErrDesc = String.Empty

                Return RTN_SUCCESS
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Console.WriteLine("Completed with ERROR", sFuncName)
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Console.WriteLine("Rollback the SAP Transaction ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
            If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            oARInvoice = Nothing
            Return RTN_ERROR
        Finally
            If Not String.IsNullOrEmpty(SARDraft) Then
                If oARInvoice.GetByKey(SARDraft) Then
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Attempting to Remove te Draft ", sFuncName)
                    lRetCode = oARInvoice.Remove()
                    If lRetCode <> 0 Then
                        sErrDesc = oCompany.GetLastErrorDescription
                        Call WriteToLogFile(sErrDesc, sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR ", sFuncName)
                        ''System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                    End If
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
                End If
            End If
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice_Doc)
            oARInvoice = Nothing
            oARInvoice_Doc = Nothing
            oRset = Nothing
            oRset_Batch = Nothing
        End Try
    End Function


    Public Function AR_InvoiceCreation_OLD1(ByVal oDVARInvoice As DataView, ByVal oDVPayment As DataView, ByRef oCompany As SAPbobsCOM.Company, ByRef oDTStatus As DataTable, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim oARInvoice As SAPbobsCOM.Documents = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
        Dim dIncomeDate As Date
        Dim tDocTime As DateTime
        Dim sWhsCode As String = String.Empty
        Dim sPOSNumber As String = String.Empty
        Dim sProductCode As String = String.Empty
        Dim sBOMCode As String = String.Empty
        Dim sQuery As String = String.Empty
        Dim sManBatchItem As String = String.Empty
        Dim oBatchDT As DataTable = Nothing
        Dim dBatchQuantity As Double = 0
        Dim dBatchNumber As String = String.Empty
        Dim dInvQuantity As Double
        Dim sDocEntry As String = String.Empty
        Dim lRetCode As Integer
        Dim irow As Integer = 0

        Dim oDV_BOM As DataView = New DataView(oDT_BOM)

        Dim oRset As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Try
            sFuncName = "AR_InvoiceCreation()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function ", sFuncName)

            dIncomeDate = DateTime.ParseExact(oDVARInvoice.Item(0).Row("PHOSTxDate").ToString.Trim, "yyyyMMdd", Nothing)
            sPOSNumber = CStr(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim)
            sWhsCode = CStr(oDVARInvoice.Item(0).Row("HOutlet").ToString.Trim)

            '' oARInvoice.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInvoices

            tDocTime = tDocTime.AddHours(0)
            tDocTime = tDocTime.AddMinutes(0)

            oARInvoice.CardCode = p_oCompDef.p_sCardCode
            oARInvoice.DocDate = dIncomeDate
            oARInvoice.DocDueDate = dIncomeDate
            oARInvoice.TaxDate = dIncomeDate
            oARInvoice.NumAtCard = sWhsCode & " - " & sPOSNumber

            oARInvoice.UserFields.Fields.Item("U_AB_POSTxNo").Value = oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim
            oARInvoice.UserFields.Fields.Item("U_AB_Date").Value = dIncomeDate
            oARInvoice.UserFields.Fields.Item("U_AB_Time").Value = tDocTime

            '' oDV_BOM.RowFilter = "HeaderID = '" & oDVARInvoice.Item(0).Row("HTransID").ToString.Trim & "'"

            For Each dvr As DataRowView In oDVARInvoice
                '' sProductCode = dvr("ItemCode").ToString.Trim
                oARInvoice.Lines.ItemCode = dvr("DItemCode").ToString.Trim
                oARInvoice.Lines.Quantity = CDbl(dvr("DQuantity").ToString.Trim)
                oARInvoice.Lines.Price = CDbl(dvr("DPrice").ToString.Trim)
                oARInvoice.Lines.LineTotal = CDbl(dvr("DLineTotal").ToString.Trim)
                oARInvoice.Lines.WarehouseCode = sWhsCode
                '' oARInvoice.Lines.VatGroup = dvr("VatGourpSa").ToString.Trim

                sManBatchItem = dvr("ManBtchNum").ToString.Trim
                If sManBatchItem.ToUpper() = "Y" Then

                    sQuery = " SELECT BatchNum ,Quantity , SysNumber  FROM OIBT WITH (NOLOCK) WHERE ItemCode ='" & sProductCode & "' and Quantity >0 " & _
                                          "AND WhsCode ='" & sWhsCode & "' ORDER BY InDate ASC "
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Batch Query " & sQuery, sFuncName)
                    oRset.DoQuery(sQuery)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConvertRecordset() ", sFuncName)
                    oBatchDT = ConvertRecordset(oRset, sErrDesc)  ' Get_DataTable(sQuery, P_sSAPConString, sErrDesc)

                    For iBatchRow As Integer = 0 To oBatchDT.Rows.Count - 1

                        dBatchQuantity = CDbl(oBatchDT.Rows(iBatchRow)("Quantity").ToString().Trim())
                        dBatchNumber = oBatchDT.Rows(iBatchRow)("BatchNum").ToString().Trim()
                        dInvQuantity = CDbl(dvr("Quantity").ToString.Trim)

                        '' oARInvoice.Lines.BatchNumbers.InternalSerialNumber = oBatchDT.Rows(iBatchRow)("SysNumber").ToString().Trim()
                        ''oARInvoice.Lines.BatchNumbers.Location = irow
                        oARInvoice.Lines.BatchNumbers.SetCurrentLine(0)
                        oARInvoice.Lines.BatchNumbers.BatchNumber = dBatchNumber
                        If dInvQuantity > dBatchQuantity Then
                            'If Balance Qty>Batch Qty, then get full Batch Qty
                            oARInvoice.Lines.BatchNumbers.Quantity = dBatchQuantity
                            'minus current qty with Batch Qty
                            dInvQuantity = dInvQuantity - dBatchQuantity
                        Else
                            oARInvoice.Lines.BatchNumbers.Quantity = dInvQuantity
                            dInvQuantity = dInvQuantity - dInvQuantity
                        End If

                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Attempting to Add BatchNumbers ", sFuncName)
                        oARInvoice.Lines.BatchNumbers.Add()
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Success - Add BatchNumbers ", sFuncName)
                        If dInvQuantity <= 0 Then Exit For
                    Next

                End If

                irow += 1
                oARInvoice.Lines.Add()
                If irow = 2 Then
                    Exit For
                End If
            Next



            oARInvoice.Rounding = SAPbobsCOM.BoYesNoEnum.tYES
            oARInvoice.RoundingDiffAmount = CDbl(oDVARInvoice.Table.Rows(0).Item("HRounding").ToString.Trim)
            ''If oARInvoice.GetByKey(24216) Then
            ''    oARInvoice.SaveXML("E:\invoice1.xml")
            ''End If

            '' If oCompany.InTransaction = False Then oCompany.StartTransaction()

            ''  oARInvoice.SaveXML("E:\Test123.xml")
            lRetCode = oARInvoice.Add()

            If lRetCode <> 0 Then
                sErrDesc = oCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, "", "FAIL", sErrDesc, "", Now.ToShortTimeString)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_Status() ", sFuncName)
                ''System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                Return RTN_ERROR

            Else
                '' System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                oARInvoice = Nothing
                oCompany.GetNewObjectCode(sDocEntry)

                If oDVARInvoice.Item(0).Row("HPOSTxType").ToString.Trim = "S" Then
                    '************************************ Incoming Payment Started ************************************************************************************

                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Funcion AR_IncomingPayment() : AR Invoice DocEntry " & sDocEntry, sFuncName)

                    If AR_IncomingPayment(oDVPayment, oCompany, sDocEntry, dIncomeDate, sPOSNumber _
                                       , sWhsCode, p_oCompDef.p_sCardCode, sErrDesc) <> RTN_SUCCESS Then

                        Call WriteToLogFile(sErrDesc, sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                        oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, "", "FAIL", sErrDesc, "", Now.ToShortTimeString)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
                        If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        oARInvoice = Nothing
                        Return RTN_ERROR
                    End If
                ElseIf oDVARInvoice.Item(0).Row("HPOSTxType").ToString.Trim = "V" Then
                    '************************************ AR Credit Memo ************************************************************************************

                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Funcion AR_IncomingPayment() : AR Invoice DocEntry " & sDocEntry, sFuncName)

                    If AR_IncomingPayment(oDVPayment, oCompany, sDocEntry, dIncomeDate, sPOSNumber _
                                       , sWhsCode, p_oCompDef.p_sCardCode, sErrDesc) <> RTN_SUCCESS Then

                        Call WriteToLogFile(sErrDesc, sFuncName)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                        oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, "", "FAIL", sErrDesc, "", Now.ToShortTimeString)
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Rollback the SAP Transaction ", sFuncName)
                        If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                        oARInvoice = Nothing
                        Return RTN_ERROR
                    End If

                End If



                sErrDesc = ""

                ''  Update_Status(sTransID, sErrDesc, "SUCCESS", sDocEntry, "SalesTransHDR")
                oDTStatus.Rows.Add(oDVARInvoice.Item(0).Row("HPOSTxNo").ToString.Trim, "", "SUCCESS", "", "", Now.ToShortTimeString)
                If oCompany.InTransaction = True Then oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Committed the Transaction Reference POSNumber : " & sPOSNumber, sFuncName)
                ''System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
                '' If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Disconnecting the Company and Release the Object ", sFuncName)
                Return RTN_SUCCESS
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Return RTN_ERROR
        End Try
    End Function



    Function AR_Invoice_Cancel(ByRef oDICompany As SAPbobsCOM.Company, _
                               ByRef sInvoice As String, ByVal dDate As Date, ByVal spostdate As String, ByRef sErrDesc As String) As Long

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim oARInvoice As SAPbobsCOM.Documents = Nothing
        Dim oARInvoiceCancellation As SAPbobsCOM.Documents = Nothing
        Dim oRset As SAPbobsCOM.Recordset = Nothing
        oARInvoice = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
        oRset = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim stime As String = String.Empty
        Dim sSQL As String = String.Empty
        Dim sDocEntry As String = String.Empty

        Try


            sFuncName = "AR_Invoice_Cancel"
            Console.WriteLine("Starting Function", sFuncName)

            Dim sString() As String = spostdate.Split(" ")
            stime = Left(sString(1).Replace(":", ""), 4)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Time Split " & spostdate, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("After Time Split " & stime, sFuncName)
            If oARInvoice.GetByKey(sInvoice) Then
                oARInvoiceCancellation = oARInvoice.CreateCancellationDocument()
                ''oARInvoiceCancellation.DocDate = dDate
                ''oARInvoiceCancellation.DocDueDate = dDate
                oARInvoiceCancellation.UserFields.Fields.Item("U_AB_Date").Value = dDate
                ''  oARInvoiceCancellation.UserFields.Fields.Item("U_AB_Time").Value = "1132"

                lRetCode = oARInvoiceCancellation.Add()

                If lRetCode <> 0 Then
                    sErrDesc = oDICompany.GetLastErrorDescription
                    Call WriteToLogFile(sErrDesc, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                    AR_Invoice_Cancel = RTN_ERROR
                Else
                    Console.WriteLine("Completed with SUCCESS ", sFuncName)
                    oDICompany.GetNewObjectCode(sDocEntry)
                    '' sSQL = "Update OINV set [U_AB_Time] = '" & stime & "' FROM OINV T0 WHERE DocEntry = '" & sDocEntry & "'"
                    sSQL = "Update OINV set [U_AB_Time] = '" & stime & "' where DocEntry = '" & sDocEntry & "'"
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Time Change (Invoice) " & sSQL, sFuncName)
                    oRset.DoQuery(sSQL)
                    sErrDesc = String.Empty
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS.", sFuncName)
                    AR_Invoice_Cancel = RTN_SUCCESS
                    sErrDesc = String.Empty
                End If
            Else

                sErrDesc = "No matching records found in the AR Invoice " & sInvoice
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                AR_Invoice_Cancel = RTN_ERROR
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            AR_Invoice_Cancel = RTN_ERROR

        Finally
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Releasing the Objects", sFuncName)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoice)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oARInvoiceCancellation)
            oARInvoice = Nothing
            oARInvoiceCancellation = Nothing
            oRset = Nothing
        End Try
    End Function


    Function Update_Draft(ByRef oDICompany As SAPbobsCOM.Company, ByRef oDraft As SAPbobsCOM.Documents, ByVal dGrossTotal As Double, _
                            ByVal sWhsCode As String, ByVal sProjectCode As String, ByRef sErrDesc As String) As Long

        Dim lRetCode As Long
        Dim iCount As Integer = 0
        Dim sFuncName As String = String.Empty
        Dim dDiffAmount As Double = 0.0
        Dim sDraftNew As String = String.Empty
        Dim oDTDraftDetails As DataTable = New DataTable
        Dim oDTBOMCount As DataTable = New DataTable
        Dim sQuery As String = String.Empty
        Dim sTreeType As String = String.Empty
        Dim sCOGSAcctCode As String = String.Empty
        Dim sDiscItemCode As String = String.Empty
        Dim sWarehouse As String = String.Empty
        '' Dim oDraft As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
        Dim oRS As SAPbobsCOM.Recordset = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim sVatGroup As String = String.Empty
        Dim sDocEntry As String = String.Empty
        Dim sProject As String = String.Empty
        Dim sGLAccount As String = String.Empty

        Try

            sFuncName = "Update_Draft()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)
            sDocEntry = oDraft.DocEntry
            sQuery = "select T0.[ItemCode],T0.[Project],T0.[CogsAcct],T0.[WhsCode],T0.[LineNum],T0.[TreeType], T0.[AcctCode], T0.[U_DiscItem] from DRF1 T0 where DocEntry='" & sDocEntry & "'"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Executing the Query, Query String : " & sQuery, sFuncName)
            oRS.DoQuery(sQuery)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConvertRecordset()", sFuncName)
            oDTDraftDetails = ConvertRecordset(oRS, sErrDesc)
            '' oDraft.GetByKey(sDraftDocNum)

            If dGrossTotal <> 0 Then
                oDraft.Lines.Add()
                oDraft.Lines.ItemCode = p_oCompDef.p_szSRounding
                If dGrossTotal > 0 Then
                    oDraft.Lines.Quantity = -1
                Else
                    oDraft.Lines.Quantity = 1
                End If
                oDraft.Lines.UnitPrice = Math.Abs(dGrossTotal)
                oDraft.Lines.VatGroup = p_oCompDef.p_szeroTax
                oDraft.Lines.WarehouseCode = sWhsCode
                oDraft.Lines.COGSCostingCode = sWhsCode
                oDraft.Lines.CostingCode = sWhsCode

            End If

            For IDraftRow As Integer = 0 To oDTDraftDetails.Rows.Count - 1

                sCOGSAcctCode = oDTDraftDetails.Rows.Item(IDraftRow)("CogsAcct").ToString().Trim()
                sProject = oDTDraftDetails.Rows.Item(IDraftRow)("Project").ToString().Trim()
                sGLAccount = oDTDraftDetails.Rows.Item(IDraftRow)("AcctCode").ToString().Trim()
                sDiscItemCode = oDTDraftDetails.Rows.Item(IDraftRow)("U_DiscItem").ToString().Trim()

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Project Code : " & sProject & "  sCOGSAcctCode " & sCOGSAcctCode & " IDraftRow " & IDraftRow, sFuncName)
                sQuery = " WITH BOM (Code, Level, TreeType) AS ( SELECT T0.Code, 0 as Level, T2.TreeType  FROM dbo.OITT T0 WITH(NOLOCK) inner join OITM T2 WITH(NOLOCK) on T0.Code = T2.ItemCode " & _
                        " WHERE T0.Code = '" & oDTDraftDetails.Rows.Item(IDraftRow)("ItemCode").ToString().Trim() & "' UNION ALL   SELECT T1.Code , Level +1 , T2.TreeType   FROM " & _
                        " dbo.ITT1 AS T1 WITH(NOLOCK) inner join OITM T2 WITH(NOLOCK) on T1.Code = T2.ItemCode JOIN BOM ON T1.Father = BOM.Code AND BOM.TreeType ='S' ) " & _
                        " SELECT COUNT(Code) FROM BOM  WHERE Code NOT IN (SELECT Code FROM OITT WITH(NOLOCK) WHERE TreeType ='S') OPTION (MAXRECURSION 99) "

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Executing the Query, Query String : " & sQuery, sFuncName)

                oRS.DoQuery(sQuery)

                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConvertRecordset()", sFuncName)

                oDTBOMCount = ConvertRecordset(oRS, sErrDesc)

                iCount = oDTBOMCount.Rows(0)(0).ToString()

                If iCount = 0 Then Continue For

                For iRowCount As Integer = IDraftRow + 1 To IDraftRow + iCount

                    oDraft.Lines.SetCurrentLine(iRowCount)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Project Code : " & sProject & "  sCOGSAcctCode " & sCOGSAcctCode & " IDraftRow " & IDraftRow, sFuncName)
                    oDraft.Lines.WarehouseCode = sWhsCode
                    oDraft.Lines.COGSAccountCode = sCOGSAcctCode
                    oDraft.Lines.COGSCostingCode = sWhsCode
                    oDraft.Lines.CostingCode = sWhsCode
                    oDraft.Lines.ProjectCode = sProject
                    '' oDraft.Lines.UserFields.Fields.Item("U_DiscItem").Value = sDiscItemCode
                    If Not String.IsNullOrEmpty(sProject) Then
                        oDraft.Lines.AccountCode = sGLAccount
                    End If
                    'oDraftAftUpdate.Lines.Add()
                Next
                IDraftRow = IDraftRow + iCount
            Next


            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Update the Draft With Update Draft", sFuncName)
            lRetCode = oDraft.Update()
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("After Update the Draft. Return Value : " & lRetCode, sFuncName)
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Throw New ArgumentException(sErrDesc)
            End If
            Console.WriteLine("Completed with SUCCESS ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            Update_Draft = RTN_SUCCESS
            sErrDesc = String.Empty
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Update_Draft = RTN_ERROR
        Finally

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Releasing the Objects", sFuncName)
            ''    System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft)
            ''   oDraft = Nothing

        End Try

    End Function

End Module
