Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.ServiceProcess
Imports Microsoft.Reporting.WinForms
Imports System.Net.Mail
Imports System.IO.Packaging
Imports System
Imports System.Text
Imports System.Web.Services.Protocols
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Net

Public Class Functions

    Public Shared Sub WriteLog(ByVal Str As String)
        Try
            Dim oWrite As IO.StreamWriter
            Dim FilePath As String
            FilePath = Application.StartupPath + "\logfile_" & Format(Now.Date, "ddMMyyyy") & ".txt"
            If IO.File.Exists(FilePath) Then
                oWrite = IO.File.AppendText(FilePath)
            Else
                oWrite = IO.File.CreateText(FilePath)
            End If
            oWrite.Write(Now.ToString() + ":" + Str + vbCrLf)
            oWrite.Close()
        Catch ex As Exception

        End Try
    End Sub
    
    Public Sub AutoRun()
        Try
            SystemInitial()
            'PublicVariable.AutoRetry = PublicVariable.AutoRetry

            'Dim xm As New oXML

            'If PublicVariable.AutoRetry Then
            '    Dim cn As New Connection
            '    cn.Integration_RunQuery("exec sp_RetryAll")
            'End If


            'Dim sErrMsg As String = Functions.SystemInitial
            'If sErrMsg <> "" Then
            '    Functions.WriteLog(sErrMsg)
            '    Return
            'End If
            '------------CREATE BUSINESS PARTNER--------------
            'Dim obp As New oGuest
            'obp.CreateGuest()

            '------------RECEIVE ADVANCE PAYMENT--------------
            'Dim odv As New oAdvance
            'odv.CreateAdvance()

            ''------------RECEIVE INVOICE--------------
            'Dim oin As New oInvoice
            'oin.CreateInvoice()

            '------------RECEIVE POS INVOICE--------------
            'Dim poin As New oPOSInvoice
            'poin.CreateInvoice()

            ''------------RECEIVE PAYMENT--------------
            'Dim opay As New oPayment
            'opay.CreatePayment()

            '---------CANCELLATION FEE----------
            'Dim ocanfe As New oCancelCharge
            'ocanfe.CreateJE()

            '---------NightAuditHandler----------
            Dim NAH As New NightAuditHandler
            NAH.CreateDocumentForAudit()

        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub
    Public Shared Function SystemInitial() As String
        Try
            PublicVariable.AutoRetry = CBool(System.Configuration.ConfigurationSettings.AppSettings.Get("AutoRetry"))
            PublicVariable.IntegrationConnectionString = System.Configuration.ConfigurationSettings.AppSettings.Get("IntegrationConnectionString")
            PublicVariable.SAPConnectionString = System.Configuration.ConfigurationSettings.AppSettings.Get("SAPConnectionString")
            PublicVariable.Timer = System.Configuration.ConfigurationSettings.AppSettings.Get("Timer")
            PublicVariable.ToErrorEmail = System.Configuration.ConfigurationSettings.AppSettings.Get("ErrorReceiver")

            Dim smtp As Array = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPServer").Split(";")
            PublicVariable.smtpServer = smtp(0)
            PublicVariable.smtpSenderEmail = smtp(1)
            PublicVariable.smtpPwd = smtp(2)
            PublicVariable.smtpPort = smtp(3)
            PublicVariable.pmCashAcct = System.Configuration.ConfigurationSettings.AppSettings.Get("CashAct")
            PublicVariable.pmTransferAcct = System.Configuration.ConfigurationSettings.AppSettings.Get("CashAct")


            If Not PublicVariable.oCompanyInfo.Connected Then
                Dim MyArr As Array
                MyArr = PublicVariable.SAPConnectionString.Split(";")

                PublicVariable.oCompanyInfo.CompanyDB = MyArr(0).ToString()
                PublicVariable.oCompanyInfo.UserName = MyArr(1).ToString()
                PublicVariable.oCompanyInfo.Password = MyArr(2).ToString()
                PublicVariable.oCompanyInfo.Server = MyArr(3).ToString()
                PublicVariable.oCompanyInfo.DbUserName = MyArr(4).ToString()
                PublicVariable.oCompanyInfo.DbPassword = MyArr(5).ToString()
                PublicVariable.oCompanyInfo.LicenseServer = MyArr(6)
                If MyArr(7).ToString = "2008" Then
                    PublicVariable.oCompanyInfo.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
                ElseIf MyArr(7).ToString = "2012" Then
                    PublicVariable.oCompanyInfo.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
                End If



                Dim lRetCode As Integer
                Dim lErrCode As Integer
                Dim sErrMsg As String = ""
                lRetCode = PublicVariable.oCompanyInfo.Connect
                If lRetCode <> 0 Then
                    PublicVariable.oCompanyInfo.GetLastError(lErrCode, sErrMsg)
                    Functions.WriteLog("SystemInitial:" + sErrMsg)
                    Return sErrMsg
                Else
                    'WriteLog("SystemInitial: " + " Connected")
                    Return ""
                End If
            End If


        Catch ex As Exception
            WriteLog("SystemInitial: " + ex.ToString)
            Return ex.ToString
        End Try

    End Function

    Public Shared Function GetPaymentType(PaymentMethodCode As String) As String
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select U_PaymentType from [@PAYMENTMETHOD] where Code='" + PaymentMethodCode + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("U_PaymentType")
        Else
            Return ""
        End If
    End Function
    Public Shared Function CheckBPCode(CompanyCode As String) As String

        If CompanyCode = "" Then
            Return "Company Code is missing"
        End If


        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select * from OCRD where CardCode='" + CompanyCode + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return ""
        Else
            Return "Company Code is missing"
        End If

    End Function
    Public Shared Function GetCostCenterByItem(ItemCode As String) As String
        Dim strQuery As String = ""
        Dim cn As New Connection
        strQuery = "select isnull(U_CostCenter,'') U_CostCenter from OITM where ItemCode='" + ItemCode + "'"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("U_CostCenter").ToString
        Else
            Return ""
        End If
    End Function
    Public Shared Function GetOneTimeCustomerCode(CardCode As String) As String
        If CardCode <> "" Then
            Return CardCode
        End If
        Dim cn As New Connection
        Dim strQuery As String = ""
        strQuery = "select top(1) dfltcard CardCode from oacp"
        Dim dt As DataTable = cn.SAP_RunQuery(strQuery)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("CardCode").ToString
        Else
            Return ""
        End If
    End Function
#Region "Build Table Structure"
    ''' <param name="tableName">table name , example : T </param>
    ''' <param name="fieldName">list of fields, example: A;B;C;D.</param>
    Public Shared Function BuildTable(tableName As String, fieldName As String) As DataTable
        Dim dt As New DataTable(tableName)
        If fieldName <> "" Then
            Dim arrFieldName As Array = fieldName.Split(";")
            For i As Integer = 0 To arrFieldName.Length - 1
                dt.Columns.Add(arrFieldName(i).ToString)
            Next
        End If
        Return dt
    End Function
#End Region
End Class
