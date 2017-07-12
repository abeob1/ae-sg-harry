Imports System.IO

Public Class frmMornitor
#Region "Service"
    'Dim LiveServiceName As String = "SAPIntegration"
    Dim TESTServiceName As String = "SAPIntegration_TEST" 'not yet
    Private Sub RefreshStatus()
        Dim a As New ServiceController(TESTServiceName)

        If a.Status = "" Then
            btnReg.Enabled = True
            btnUnReg.Enabled = False
            btnStart.Enabled = False
            btnStop.Enabled = False
        ElseIf a.Status = "Stopped" Then
            btnStart.Enabled = True
            btnStop.Enabled = False
            btnReg.Enabled = False
            btnUnReg.Enabled = True
        ElseIf a.Status = "Running" Then
            btnStart.Enabled = False
            btnStop.Enabled = True
            btnReg.Enabled = False
            btnUnReg.Enabled = True
        End If
    End Sub
    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        Dim a As New ServiceController(TESTServiceName)
        Dim str As String
        str = a.Start()
        RefreshStatus()
    End Sub
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        Dim a As New ServiceController(TESTServiceName)
        Dim str As String = a.Stop()
        RefreshStatus()
    End Sub
    Private Sub btnReg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReg.Click
        Dim a As New ServiceController(TESTServiceName)
        a.Description = TESTServiceName
        a.DisplayName = TESTServiceName
        a.ServiceName = TESTServiceName
        a.StartupType = ServiceController.ServiceStartupType.Automatic

        Dim sReturn As String
        sReturn = a.Register(Application.ExecutablePath + " -service")
        If sReturn = "" Then
            MessageBox.Show("Register Sucessfull! - Application will be closed.")
            Application.Exit()
        Else
            MessageBox.Show("Error: " + sReturn)
        End If
        RefreshStatus()
    End Sub
    Private Sub btnUnReg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnReg.Click
        Dim a As New ServiceController(TESTServiceName)
        Dim sReturn As String
        sReturn = a.Unregister()
        If sReturn = "" Then
            MessageBox.Show("UnRegister Sucessfull!")
        Else
            MessageBox.Show("Error: " + sReturn)
        End If
        RefreshStatus()
    End Sub
#End Region
#Region "Events"
    Private Sub frmMornitor_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Dim a As New Functions
            a.AutoRun()
            Me.Close()
            'Dim sErrMsg As String
            'sErrMsg = Functions.SystemInitial
            'If sErrMsg <> "" Then
            '    MessageBox.Show("Load:" + sErrMsg)
            'End If
            'RefreshStatus()
        Catch ex As Exception
            MessageBox.Show("Load" + ex.ToString)
            Me.Cursor = Cursors.Default
        End Try
    End Sub
    Private Sub btnRefresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
        RefreshMonitor()
    End Sub
    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Me.Close()
        Application.Exit()
    End Sub
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        If ckAutoRef.Checked Then
            RefreshMonitor()
        End If
        Timer1.Enabled = True
    End Sub
    Private Sub btnLog_Click(sender As System.Object, e As System.EventArgs) Handles btnLog.Click
        Dim LogFileName As String = Application.StartupPath + "\logfile.txt"
        If File.Exists(LogFileName) Then
            System.Diagnostics.Process.Start(LogFileName)
        End If
    End Sub
    Private Sub btnRetryAll_Click(sender As System.Object, e As System.EventArgs) Handles btnRetryAll.Click
        Dim cn As New Connection
        Dim dt As DataTable = cn.Integration_RunQuery("sp_RetryAll")
        RefreshMonitor()
    End Sub
   
    Private Sub btnRetry_Click(sender As System.Object, e As System.EventArgs) Handles btnRetry.Click

    End Sub
   
    Private Sub grMonitor_SelectionChanged(sender As System.Object, e As System.EventArgs) Handles grMonitor.SelectionChanged
        Dim HeaderID As String = ""
        Try
            Dim HeaderIDFieldName As String = "ID"
            If cbFilter.Text = "Goods Receipt" Or cbFilter.Text = "Goods Issue" Then
                HeaderIDFieldName = "DocEntry"
            End If
            HeaderID = grMonitor.SelectedRows.Item(0).Cells(HeaderIDFieldName).Value

        Catch ex As Exception

        End Try

        If HeaderID <> "" Then
            Dim strQuery As String = ""
            Dim cn As New Connection
            strQuery = BuildMonitorSubQuery(cbFilter.Text, HeaderID)
            If strQuery <> "" Then
                Dim dt As DataTable = cn.Integration_RunQuery(strQuery)
                grDetail.DataSource = dt
            Else
                grDetail.DataSource = Nothing
            End If
        End If
    End Sub
#End Region
#Region "Functions"
    Private Sub RefreshMonitor()
        Dim cn As New Connection
        Dim strQuery As String = ""
        strQuery = BuildMonitorQuery(cbFilter.Text, cbResult.Text)
        Dim dt As DataTable = cn.Integration_RunQuery(strQuery)
        grMonitor.DataSource = dt
    End Sub
    Private Function BuildMonitorQuery(opt1 As String, opt2 As String)
        '-----------option 2-----------
        'All
        'Pending
        'Successfull
        'Failed
        Dim strEnd As String = ""
        Select Case opt2
            Case "All"
                strEnd = ""
            Case "Pending"
                strEnd = " where ReceiveDate is null"
            Case "Successfull"
                strEnd = " where ReceiveDate is not null and isnull(ErrMsg,'')=''"
            Case "Failed"
                strEnd = " where ReceiveDate is not null and isnull(ErrMsg,'')<>''"
            Case Else
                strEnd = ""
        End Select

        If cbSendDate.Checked Then
            If strEnd <> "" Then
                strEnd = strEnd + " AND "
            Else : strEnd = " WHERE "
            End If

            strEnd = strEnd + "  datediff(dd,SendDate,'" + cbSendDate.Value.ToString("MM/dd/yyyy") + "')=0"
        End If

        '-----------option 1-----------
        Dim str As String = ""
        Select Case opt1
            Case "EmployeeMaster"
                str = "select * from EmployeeMaster" + strEnd
            Case "ItemMaster"
                str = "select * from ItemMaster" + strEnd
            Case "POSItems"
                str = "select * from POSItemMaster" + strEnd
            Case "CustomerMaster"
                str = "select * from CustomerMaster" + strEnd
            Case "HotelMaster"
                str = "select * from [@Hotel]" + strEnd
            Case "RestaurantTables"
                str = "select * from [@POSTABLEMASTER]" + strEnd
            Case "MeetingRoom"
                str = "select * from [@MEETINGROOMMASTER]" + strEnd
            Case "POSInvoice"
                str = "select * from POSInvoiceHeader" + strEnd
            Case "FloorMaster"
                str = "select * from [@Floor]" + strEnd
            Case "RoomMaster"
                str = "select * from [@Room]" + strEnd
            Case "DivisionMaster"
                str = "select * from [@Division]" + strEnd
            Case "ShiftMaster"
                str = "select * from [@ShiftMaster]" + strEnd
            Case "PaymentMethod"
                str = "select * from PaymentMethod" + strEnd
            Case "OutletMaster"
                str = "select * from OutletMaster" + strEnd
            Case "CustomerGroup"
                str = "select * from CustomerGroupMaster" + strEnd
            Case "CostCenter"
                str = "select * from CostCenter" + strEnd
            Case "AdvancePayment"
                If strEnd = "" Then
                    strEnd = " where  PaymentType='ADV'"
                Else
                    strEnd = strEnd + " and PaymentType='ADV'"
                End If

                str = "select * from PaymentDetail" + strEnd + " and PaymentType='ADV'"
            Case "Invoice"
                str = "select * from InvoiceHeader" + strEnd

            Case "MarketingSourceMaster"
                str = "select * from MarketingSourceMaster" + strEnd
            Case "PaymentDetail"
                str = "select * from PaymentDetail" + strEnd
        End Select

        Return str
    End Function
    Private Function BuildMonitorSubQuery(opt1 As String, HeaderID As String)

        Dim str As String = ""
        Select Case opt1
            Case "Invoice"
                str = "select * from InvoiceLine where HeaderID=" + HeaderID
            Case Else
                str = ""
        End Select

        Return str
    End Function
#End Region

    Private Sub cbFilter_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbFilter.SelectedIndexChanged
        RefreshMonitor()
    End Sub

    Private Sub cbResult_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbResult.SelectedIndexChanged
        RefreshMonitor()
    End Sub

    Private Sub cbSendDate_ValueChanged(sender As System.Object, e As System.EventArgs) Handles cbSendDate.ValueChanged
        RefreshMonitor()
    End Sub

    Private Sub txtService_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtService.Leave
        RefreshStatus()
    End Sub
End Class