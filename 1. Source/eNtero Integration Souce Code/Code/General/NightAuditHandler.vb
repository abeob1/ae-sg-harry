Public Class NightAuditHandler
    Public Sub CreateDocumentForAudit()
        Dim oJE As oJournal = New oJournal
        Dim oInvs As oInvoices = New oInvoices
        Dim cn As New Connection
        Dim dt As DataTable = New DataTable
        Dim dt1 As DataTable = New DataTable
        Dim errMessage As String = ""
        dt = cn.Integration_RunQuery("select * from NightAuditHeader where ReceiveDate is null")
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Try
                    Dim sErrMsg As String
                    sErrMsg = Functions.SystemInitial
                    If sErrMsg <> "" Then
                        Throw New Exception(sErrMsg)
                    End If
                    'Start Transaction
                    'If PublicVariable.oCompanyInfo.InTransaction Then
                    '    PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    'End If
                    'PublicVariable.oCompanyInfo.StartTransaction()

                    errMessage = oJE.CreateJE(row)
                    If errMessage <> "" Then
                        Throw New Exception(errMessage)
                    End If

                    'errMessage = oInvs.CreateInvoice(row)
                    'If errMessage <> "" Then
                    '    Throw New Exception(errMessage)
                    'End If

                    'Commit Transation 
                    'If PublicVariable.oCompanyInfo.InTransaction Then
                    '    PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit)
                    'End If
                    UpdateNightAuditHeader(row("ID"), "")
                Catch ex As Exception
                    'Rollback all action when error
                    'If PublicVariable.oCompanyInfo.InTransaction Then
                    '    PublicVariable.oCompanyInfo.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
                    'End If
                    UpdateNightAuditHeader(row("ID"), ex.Message.Replace("'", "''"))
                End Try
            Next
        End If
    End Sub
    Private Sub UpdateNightAuditHeader(ByVal ID As Integer, ByVal mess As String)
        Dim cn As New Connection
        If mess = "" Then
            cn.Integration_RunQuery(String.Format("Update NightAuditHeader set ReceiveDate = GETDATE() , ErrMsg = '' where ID = {0}", ID))
        Else
            cn.Integration_RunQuery(String.Format("Update NightAuditHeader set ErrMsg = '{0}' where ID = {1}", mess, ID))
        End If
    End Sub
End Class
