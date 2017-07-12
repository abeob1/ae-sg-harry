<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMornitor
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMornitor))
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.txtService = New System.Windows.Forms.TextBox()
        Me.btnUnReg = New System.Windows.Forms.Button()
        Me.btnReg = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.cbSendDate = New System.Windows.Forms.DateTimePicker()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbResult = New System.Windows.Forms.ComboBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ckAutoRef = New System.Windows.Forms.CheckBox()
        Me.cbFilter = New System.Windows.Forms.ComboBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.grMonitor = New System.Windows.Forms.DataGridView()
        Me.grDetail = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnLog = New System.Windows.Forms.Button()
        Me.btnRetryAll = New System.Windows.Forms.Button()
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        CType(Me.grMonitor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.txtService)
        Me.Panel2.Controls.Add(Me.btnUnReg)
        Me.Panel2.Controls.Add(Me.btnReg)
        Me.Panel2.Controls.Add(Me.btnStop)
        Me.Panel2.Controls.Add(Me.btnStart)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 402)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(955, 36)
        Me.Panel2.TabIndex = 1
        '
        'txtService
        '
        Me.txtService.Location = New System.Drawing.Point(338, 10)
        Me.txtService.Name = "txtService"
        Me.txtService.Size = New System.Drawing.Size(214, 20)
        Me.txtService.TabIndex = 4
        '
        'btnUnReg
        '
        Me.btnUnReg.Location = New System.Drawing.Point(246, 5)
        Me.btnUnReg.Name = "btnUnReg"
        Me.btnUnReg.Size = New System.Drawing.Size(75, 28)
        Me.btnUnReg.TabIndex = 3
        Me.btnUnReg.Text = "Un-Register"
        Me.btnUnReg.UseVisualStyleBackColor = True
        '
        'btnReg
        '
        Me.btnReg.Location = New System.Drawing.Point(165, 5)
        Me.btnReg.Name = "btnReg"
        Me.btnReg.Size = New System.Drawing.Size(75, 28)
        Me.btnReg.TabIndex = 2
        Me.btnReg.Text = "Register"
        Me.btnReg.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(84, 5)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 28)
        Me.btnStop.TabIndex = 1
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(3, 5)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 28)
        Me.btnStart.TabIndex = 0
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(7, 327)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 28)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.cbSendDate)
        Me.Panel3.Controls.Add(Me.Label3)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.cbResult)
        Me.Panel3.Controls.Add(Me.btnRefresh)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.ckAutoRef)
        Me.Panel3.Controls.Add(Me.cbFilter)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(955, 42)
        Me.Panel3.TabIndex = 3
        '
        'cbSendDate
        '
        Me.cbSendDate.Checked = False
        Me.cbSendDate.CustomFormat = "dd/MM/yyyy"
        Me.cbSendDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.cbSendDate.Location = New System.Drawing.Point(635, 14)
        Me.cbSendDate.Name = "cbSendDate"
        Me.cbSendDate.ShowCheckBox = True
        Me.cbSendDate.Size = New System.Drawing.Size(118, 20)
        Me.cbSendDate.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(568, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Send Date"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(307, 18)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Result Filter"
        '
        'cbResult
        '
        Me.cbResult.FormattingEnabled = True
        Me.cbResult.Items.AddRange(New Object() {"All", "Pending", "Successfull", "Failed"})
        Me.cbResult.Location = New System.Drawing.Point(394, 11)
        Me.cbResult.Name = "cbResult"
        Me.cbResult.Size = New System.Drawing.Size(142, 21)
        Me.cbResult.TabIndex = 6
        Me.cbResult.Text = "Pending"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(759, 6)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 28)
        Me.btnRefresh.TabIndex = 5
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(81, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Document Filter"
        '
        'ckAutoRef
        '
        Me.ckAutoRef.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ckAutoRef.AutoSize = True
        Me.ckAutoRef.Checked = True
        Me.ckAutoRef.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckAutoRef.Location = New System.Drawing.Point(858, 18)
        Me.ckAutoRef.Name = "ckAutoRef"
        Me.ckAutoRef.Size = New System.Drawing.Size(85, 17)
        Me.ckAutoRef.TabIndex = 1
        Me.ckAutoRef.Text = "Auto Refesh"
        Me.ckAutoRef.UseVisualStyleBackColor = True
        '
        'cbFilter
        '
        Me.cbFilter.FormattingEnabled = True
        Me.cbFilter.Items.AddRange(New Object() {"ItemMaster", "EmployeeMaster", "CustomerMaster", "HotelMaster", "RestaurantTables", "MeetingRoom", "FloorMaster", "RoomMaster", "ShiftMaster", "PaymentMethod", "OutletMaster", "CustomerGroup", "CostCenter", "AdvancePayment", "Invoice", "POSItems", "POSInvoice", "MarketingSourceMaster", "PaymentDetail"})
        Me.cbFilter.Location = New System.Drawing.Point(99, 11)
        Me.cbFilter.Name = "cbFilter"
        Me.cbFilter.Size = New System.Drawing.Size(192, 21)
        Me.cbFilter.TabIndex = 0
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.grMonitor)
        Me.Panel4.Controls.Add(Me.grDetail)
        Me.Panel4.Controls.Add(Me.Panel1)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 42)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(955, 360)
        Me.Panel4.TabIndex = 4
        '
        'grMonitor
        '
        Me.grMonitor.AllowUserToAddRows = False
        Me.grMonitor.AllowUserToDeleteRows = False
        Me.grMonitor.AllowUserToOrderColumns = True
        Me.grMonitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grMonitor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grMonitor.Location = New System.Drawing.Point(0, 0)
        Me.grMonitor.Name = "grMonitor"
        Me.grMonitor.Size = New System.Drawing.Size(868, 234)
        Me.grMonitor.TabIndex = 6
        '
        'grDetail
        '
        Me.grDetail.AllowUserToAddRows = False
        Me.grDetail.AllowUserToDeleteRows = False
        Me.grDetail.AllowUserToOrderColumns = True
        Me.grDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grDetail.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.grDetail.Location = New System.Drawing.Point(0, 234)
        Me.grDetail.Name = "grDetail"
        Me.grDetail.Size = New System.Drawing.Size(868, 126)
        Me.grDetail.TabIndex = 5
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnLog)
        Me.Panel1.Controls.Add(Me.btnClose)
        Me.Panel1.Controls.Add(Me.btnRetryAll)
        Me.Panel1.Controls.Add(Me.btnRetry)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel1.Location = New System.Drawing.Point(868, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(87, 360)
        Me.Panel1.TabIndex = 3
        '
        'btnLog
        '
        Me.btnLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLog.Location = New System.Drawing.Point(7, 72)
        Me.btnLog.Name = "btnLog"
        Me.btnLog.Size = New System.Drawing.Size(75, 28)
        Me.btnLog.TabIndex = 8
        Me.btnLog.Text = "Log File"
        Me.btnLog.UseVisualStyleBackColor = True
        '
        'btnRetryAll
        '
        Me.btnRetryAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRetryAll.Location = New System.Drawing.Point(7, 39)
        Me.btnRetryAll.Name = "btnRetryAll"
        Me.btnRetryAll.Size = New System.Drawing.Size(75, 28)
        Me.btnRetryAll.TabIndex = 7
        Me.btnRetryAll.Text = "Retry All"
        Me.btnRetryAll.UseVisualStyleBackColor = True
        '
        'btnRetry
        '
        Me.btnRetry.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRetry.Enabled = False
        Me.btnRetry.Location = New System.Drawing.Point(7, 6)
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.Size = New System.Drawing.Size(75, 28)
        Me.btnRetry.TabIndex = 6
        Me.btnRetry.Text = "Retry"
        Me.btnRetry.UseVisualStyleBackColor = True
        Me.btnRetry.Visible = False
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 10000
        '
        'frmMornitor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(955, 438)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMornitor"
        Me.Text = "Integration Mornitor (V 2014.10.31)"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        CType(Me.grMonitor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnUnReg As System.Windows.Forms.Button
    Friend WithEvents btnReg As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ckAutoRef As System.Windows.Forms.CheckBox
    Friend WithEvents cbFilter As System.Windows.Forms.ComboBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cbResult As System.Windows.Forms.ComboBox
    Friend WithEvents btnRetry As System.Windows.Forms.Button
    Friend WithEvents btnRetryAll As System.Windows.Forms.Button
    Friend WithEvents btnLog As System.Windows.Forms.Button
    Friend WithEvents grMonitor As System.Windows.Forms.DataGridView
    Friend WithEvents grDetail As System.Windows.Forms.DataGridView
    Friend WithEvents cbSendDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtService As System.Windows.Forms.TextBox
End Class
