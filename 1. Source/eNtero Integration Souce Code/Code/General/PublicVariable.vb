Imports System.Data.SqlClient

Public Class PublicVariable
    Public Shared oCompanyInfo As SAPbobsCOM.Company = New SAPbobsCOM.Company
    Public Shared IntegrationConnection As SqlConnection
    Public Shared IntegrationConnectionString As String = ""

    Public Shared SAPConnection As SqlConnection
    Public Shared SAPConnectionString As String = ""
    Public Shared Timer As Integer = 1 'minute
    Public Shared AutoRetry As Boolean = False

    '----------SEND EMAIL INFORMATION--------------------
    Public Shared ToEmail As String = ""
    Public Shared ToEmailName As String = ""
    Public Shared smtpServer As String = ""
    Public Shared smtpPort As String = ""
    Public Shared smtpSenderEmail As String = ""
    Public Shared smtpPwd As String = ""
    Public Shared EmailSub As String = ""

    '---------PAYMENT ACCOUNT----------------
    Public Shared pmCashAcct As String = "1003002003"
    Public Shared pmTransferAcct As String = "1003002003"

    '---------OTHER-------------------
    Public Shared TransitWhs As String = "" 'Not use yet
    Public Shared GSTCode As String = ""
    Public Shared NonGSTCode As String = ""
    Public Shared ToErrorEmail As String
End Class
