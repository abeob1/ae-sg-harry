Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Reports
    Inherits System.Web.Services.WebService
#Region "TimeSheet"
    <WebMethod()> _
    Public Function TimeSheet_OpenList(UserID As String, FromDate As Date, ToDate As Date) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "select * from OCLG where U_UserID ='" + UserID + "' and CntctDate between '" + FromDate.ToString + "' and '" + ToDate.ToString + "'"
            Return connect.ObjectGetAll_Query_SAP(str)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    <WebMethod()> _
    Public Function TimeSheet_All(UserID As String, FromDate As Date, ToDate As Date) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "select * from OCLG where CntctDate between '" + FromDate.ToString + "' and '" + ToDate.ToString + "'"
            Return connect.ObjectGetAll_Query_SAP(str)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    <WebMethod()> _
    Public Function TimeSheet_Missing(UserID As String, FromDate As Date, ToDate As Date, FilterByUser As Boolean) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String = ""
            str = ";with months (date) AS (SELECT '" + FromDate.ToString + "' UNION ALL SELECT DATEADD(day,1,date) from months where DATEADD(day,1,date)<='" + ToDate.ToString + "')"
            If FilterByUser = True Then
                str = str + " select T0.date from months T0 left join OCLG T1 on DATEDIFF(dd,T0.date,T1.Recontact)=0 and T1.U_UserID=" + UserID + " where T1.Recontact is null"
            Else
                str = str + " select T0.date from months T0 left join OCLG T1 on DATEDIFF(dd,T0.date,T1.Recontact)=0 where T1.Recontact is null"
            End If

            Return connect.ObjectGetAll_Query_SAP(str)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
#Region "Financial Reports"
    <WebMethod()> _
    Public Function VAS_GeneralJournalReport(UserID As String, FromDate As Date, ToDate As Date) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "exec USP_RPT_FI_GENERALJOURNAL '" + FromDate.ToString("MM/dd/yyyy") + "' , '" + ToDate.ToString("MM/dd/yyyy") + "'"
            Dim ds As New DataSet("GeneralJournal")
            ds = connect.ObjectGetAll_Query_SAP(str)
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
#Region "Sales History"
    <WebMethod()> _
    Public Function BP_SalesHistory_Detail(UserID As String, CardCode As String) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "Select ROW_NUMBER() Over(Order By T0.DocEntry) No,T0.ItemCode,T0.Dscription,T0.quantity,T0.DocDate from RDR1 T0 join ORDR T1 on T0.docEntry=T1.DocEntry where T1.cardcode= '" + CardCode + "'"
            Dim ds As New DataSet("BP_SalesHistory_Detail")
            ds = connect.ObjectGetAll_Query_SAP(str)
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
#Region "Activity Pending"
    <WebMethod()> _
    Public Function BP_Activity_List(UserID As String) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "select ClgCode,T1.CardName,T1.LicTradNum, T0.CntctTime,T2.lastName + ' ' + T2.firstName AttendEmpl,T0.Details from OCLG T0 join OCRD T1 on T0.CardCode=T1.CardCode left join OHEM T2 on t2.empID=T0.AttendEmpl"
            Dim ds As New DataSet("BP_Activity_List")
            ds = connect.ObjectGetAll_Query_SAP(str)
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
#Region "Load Attachment"
    <WebMethod()> _
    Public Function BP_Attachment_List(UserID As String, CardCode As String) As DataSet
        Try
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "select ROW_NUMBER() Over(Order By T0.AtcEntry) No,T2.FileName,T2.trgtPath,T2.Date from OCRD T0 join OATC T1 on T0.AtcEntry=T1.AbsEntry join ATC1 T2 on T1.AbsEntry=T1.AbsEntry where T0.CardCode='" + CardCode + "'"
            Dim ds As New DataSet("BP_Attachment_List")
            ds = connect.ObjectGetAll_Query_SAP(str)
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
End Class