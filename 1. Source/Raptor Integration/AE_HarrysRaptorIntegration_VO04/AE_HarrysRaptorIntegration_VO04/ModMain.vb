Module ModMain

    Public oDT_BOM As DataTable = Nothing
    Public oDT_InvoiceHeader As DataTable = Nothing
    Public oDT_InvoiceDetails As DataTable = Nothing
    Public oDT_PaymentData As DataTable = Nothing
    Public oDT_Warehouse As DataTable = Nothing
    Public oDT_ItemGroup As DataTable = Nothing


    Sub Main()

        Dim sFuncName As String = String.Empty
        Dim sErrDesc As String = String.Empty

        Dim sQuery As String = String.Empty

        Try
            p_iDebugMode = DEBUG_ON
            sFuncName = "Main()"
            Console.WriteLine("Raptor Integration Starting ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            'Getting the Parameter Values from App Cofig File
            Console.WriteLine("Getting the Prerequest Information ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling GetSystemIntializeInfo()", sFuncName)
            If GetSystemIntializeInfo(p_oCompDef, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            If Not oDT_InvoiceHeader Is Nothing And oDT_InvoiceHeader.Rows.Count > 0 Then
                '' Function to connect the Company
                If p_oCompany Is Nothing Then
                    Console.WriteLine("Calling ConnectToTargetCompany() ", sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTargetCompany()", sFuncName)
                    If ConnectToTargetCompany(p_oCompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                End If

                '  Console.WriteLine("Calling IntegrityValidation() ", sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrityValidation()", sFuncName)
                If IntegrityValidation(oDT_InvoiceHeader, oDT_InvoiceDetails, oDT_PaymentData, p_oCompany, sErrDesc) <> RTN_SUCCESS Then
                    Call WriteToLogFile(sErrDesc, sFuncName)
                    Console.WriteLine("Completed with ERROR : " & sErrDesc, sFuncName)
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                End If
            Else

                Console.WriteLine("There is No Pending Records Found in Integration DB", sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("There is No Pending Records Found in Integration DB", sFuncName)
            End If

            ''Console.WriteLine("Stock Checking Query :", sFuncName)
            ''sQuery = "[AE_SP002_GetNoStockItem]'[" & p_oCompDef.p_sDataBaseName & "]'"
            ''If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Stock Checking Query : " & sQuery, sFuncName)
            ''If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ExecuteSQLQuery_DT()", sFuncName)
            ''ExecuteSQLQuery_DT(P_sConString, sQuery, sErrDesc)


            Console.WriteLine("Completed with SUCCESS ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            Console.WriteLine("Completed with ERROR : " & sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)

        End Try

    End Sub

End Module
