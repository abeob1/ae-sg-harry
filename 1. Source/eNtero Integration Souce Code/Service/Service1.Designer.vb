﻿Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Service1
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
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

    ' The main entry point for the process
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New Service1}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Timer1 = New System.Timers.Timer()
        Me.Timer2 = New System.Timers.Timer()
        CType(Me.Timer1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Timer2, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 6000.0R
        '
        'Timer2
        '
        Me.Timer2.Enabled = True
        Me.Timer2.Interval = 3600000.0R
        '
        'Service1
        '
        Me.ServiceName = "Service1"
        CType(Me.Timer1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Timer2, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Timer1 As System.Timers.Timer
    Friend WithEvents Timer2 As System.Timers.Timer

End Class
