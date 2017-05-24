Public Class oGuest

#Region "Create CUSTOMER"
    Public Sub CreateGuest()
        Dim DocType As String = "2"
        Dim cn As New Connection
        Dim xm As New oXML

        Try
            Dim dt As DataTable = cn.Integration_RunQuery("sp_BusinessParterMaster_LoadForSync")
            If Not IsNothing(dt) Then

                Dim sErrMsg As String
                sErrMsg = Functions.SystemInitial
                If sErrMsg <> "" Then
                    Return
                End If

                For Each dr As DataRow In dt.Rows
                    Dim OCRDID As String = dr.Item("ID")
                    Dim ret As String = ""

                    ret = xm.Create_Update_BPInfo(dr)

                    If ret.Contains("'") Then
                        ret = ret.Replace("'", " ")
                    End If
                    cn.Integration_RunQuery("sp_BusinessPartnerMaster_UpdateReceived '" + CStr(OCRDID) + "','" + ret + "'")
                Next
            End If
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub


#End Region
End Class
