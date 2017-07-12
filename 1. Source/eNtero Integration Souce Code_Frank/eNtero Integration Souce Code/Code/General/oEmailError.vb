Imports System.Net.Mail
Imports System.Net
Imports System.Globalization
Imports System.IO

Public Class oEmailError
    Public Sub SendErrorEmail()
        Try
            Dim cn As New Connection
            Dim dt As DataTable = cn.Integration_RunQuery("sp_ErrorList")

            Dim ret As String = ""
            ret = SendMailByDS(dt, PublicVariable.ToErrorEmail, "Integration Error Notice", Application.StartupPath + "\EmailError.htm")
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
    Public Function SendMailByDS(dt As DataTable, ToEmailList As String, Subject As String, TemplatePath As String) As String

        Dim l_SenderEmail As String = PublicVariable.smtpSenderEmail
        If ToEmailList.Trim().Equals(String.Empty) Then
            ToEmailList = l_SenderEmail
        End If

        Dim msg As New System.Net.Mail.MailMessage()
        msg.From = New MailAddress(l_SenderEmail, "AUTO MAILER")
        msg.[To].Add(ToEmailList)
        msg.Subject = Subject
        msg.Body = GetTemplateforDS(dt, TemplatePath)
        msg.IsBodyHtml = True
        Try
            Dim client As New SmtpClient(PublicVariable.smtpServer, PublicVariable.smtpPort)
            client.EnableSsl = True
            client.Timeout = 0
            client.Credentials = New NetworkCredential(l_SenderEmail, PublicVariable.smtpPwd)
            client.Send(msg)
        Catch ex As SmtpException
            Return ex.Message
        End Try

        Return ""
    End Function
    Private Function GetTemplateforDS(dt As DataTable, TemplatePath As String) As String
        Dim l_Rs As String = ""
        Try
            Dim l_PathTemplate As String = String.Empty

            If TemplatePath.Trim().Equals(String.Empty) Then
                l_Rs = String.Format("Template is empty")
            Else
                l_Rs = File.ReadAllText(TemplatePath)

                '-----------line-----------------
                Dim str As String = ""
                For i As Integer = 0 To dt.Rows.Count - 1
                    str = str & "<tr>"
                    str = str & "<td style=""border: thin solid #008080;""><@Type" & i.ToString() & "></td>"
                    str = str & "<td style=""border: thin solid #008080;""><@ID" & i.ToString() & "></td>"
                    str = str & "<td style=""border: thin solid #008080;""><@ErrMsg" & i.ToString() & "></td>"
                    str = str & "<td style=""border: thin solid #008080;""><@XMLString" & i.ToString() & "></td>"
                    str = str & "</tr>"
                Next
                l_Rs = l_Rs.Replace("<@ITEMLINEHERE>", str)
                Dim j As Integer = 0
                For Each dr1 As DataRow In dt.Rows
                    l_Rs = l_Rs.Replace("<@Type" & j.ToString() & ">", dr1("DocType").ToString())
                    l_Rs = l_Rs.Replace("<@ID" & j.ToString() & ">", dr1("DocKey").ToString())
                    l_Rs = l_Rs.Replace("<@ErrMsg" & j.ToString() & ">", dr1("ErrMsg"))
                    l_Rs = l_Rs.Replace("<@XMLString" & j.ToString() & ">", dr1("XMLString"))
                    j += 1
                Next
            End If
        Catch ex As Exception
            Functions.WriteLog(ex.ToString())
            Return ""
        End Try
        Return l_Rs
    End Function
End Class
