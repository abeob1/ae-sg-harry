Imports System.Data.SqlClient

Public Class Connection
#Region "ADO Integration"
    
    Public Function Integration_RunQuery(ByVal querystr As String) As DataTable

        If querystr = "" Then Return Nothing

        Dim IntegrationConnection As New SqlConnection
        Dim MyArr As Array = System.Configuration.ConfigurationSettings.AppSettings.Get("IntegrationConnectionString").ToString.Split(";")
        IntegrationConnection.ConnectionString = "server= " + MyArr(1).ToString() + ";database=" + MyArr(0).ToString() + " ;uid=" + MyArr(2).ToString() + "; pwd=" + MyArr(3).ToString() + ";"

        Try
            IntegrationConnection.Open()
        Catch ex As Exception
            Functions.WriteLog("Integration_RunQuery: " + querystr + " ERROR:" + ex.Message)
            Return Nothing
        End Try

        Try
            Dim MyCommand As SqlCommand = New SqlCommand(querystr, IntegrationConnection)
            MyCommand.CommandType = CommandType.Text
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim mytable As DataTable = New DataTable()
            da.SelectCommand = MyCommand
            da.Fill(mytable)
            IntegrationConnection.Close()
            If mytable Is Nothing Then Return Nothing
            Return mytable
        Catch ex As Exception
            Functions.WriteLog("Integration_RunQuery: " + querystr + " ERROR:" + ex.Message)
            Return Nothing
        End Try
    End Function
#End Region
#Region "ADO SAP"
    
    Public Function SAP_RunQuery(ByVal querystr As String) As DataTable
        Try
            If querystr = "" Then Return Nothing

            Dim SAPConnection As SqlConnection
            Dim MyArr As Array = System.Configuration.ConfigurationSettings.AppSettings.Get("SAPConnectionString").ToString.Split(";")
            SAPConnection = New SqlConnection
            SAPConnection.ConnectionString = "server= " + MyArr(3).ToString() + ";database=" + MyArr(0).ToString() + " ;uid=" + MyArr(4).ToString() + "; pwd=" + MyArr(5).ToString() + ";"
           
            SAPConnection.Open()
            Dim MyCommand As SqlCommand = New SqlCommand(querystr, SAPConnection)
            MyCommand.CommandType = CommandType.Text
            Dim da As SqlDataAdapter = New SqlDataAdapter()
            Dim mytable As DataTable = New DataTable()
            da.SelectCommand = MyCommand
            da.Fill(mytable)
            SAPConnection.Close()
            If mytable Is Nothing Then Return Nothing
            Return mytable
        Catch ex As Exception
            Functions.WriteLog("SAP_RunQuery: " + querystr + " " + ex.Message)
            Return Nothing
        End Try
    End Function
#End Region
End Class
