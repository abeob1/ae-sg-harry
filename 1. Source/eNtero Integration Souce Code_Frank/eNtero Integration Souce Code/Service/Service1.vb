

Public Class Service1
#Region "General"
    Protected Overrides Sub OnStart(ByVal args() As String)
        Try

            Timer1.Enabled = True
            Timer1.Interval = PublicVariable.Timer * 60000
            Timer1.Start()

            Timer2.Enabled = True
            Timer2.Start()
        Catch ex As Exception
            Functions.WriteLog("OnStart: " + ex.ToString)
        End Try
    End Sub
    Protected Overrides Sub OnStop()
        Try
            Timer1.Enabled = False
            Timer1.Stop()
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
    Private Sub Timer1_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer1.Elapsed
        Try

            Timer1.Enabled = False

            Dim a As New Functions
            a.AutoRun()
            Timer1.Enabled = True
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
            Timer1.Enabled = True
        End Try
    End Sub
#End Region
    
    Private Sub Timer2_Elapsed(sender As System.Object, e As System.Timers.ElapsedEventArgs) Handles Timer2.Elapsed
        'send error to email
        Try
            Timer2.Enabled = False
            Dim sErrMsg As String
            sErrMsg = Functions.SystemInitial
            If sErrMsg <> "" Then
                Return
            End If
            '------------SEND ERROR EMAIL-------------
            If Integer.Parse(DateTime.Now.ToString("HH")) = 4 Or 1 = 1 Then
                Dim oerrm As New oEmailError
                oerrm.SendErrorEmail()
            End If
            Timer2.Enabled = True
        Catch ex As Exception
            Functions.WriteLog(ex.ToString)
        End Try
    End Sub
End Class
