Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class GetDefault
    Inherits System.Web.Services.WebService
    Dim connect As New Connection()
#Region "Default Infor"
    '- Goi ws de lay default vendor/customer
    '- default date = today
    '- Sau khi chon Item: hien thi ItemCode, Description, Quantity=1,  goi ws de lay gia tri default
    '- Doi so luong: goi ws de lay gia tri default
#End Region
    <WebMethod()> _
    Public Function GetDefaultLineInfo(UserID As String, ByVal cardCode As String, ByVal itemCode As String, ByVal Quantity As Double, _
                                    ByVal refDate As Date) As DataSet
        If PublicVariable.Simulate Then
            Dim a As New Simulation
            Return a.Simulate_GetDefaultInfo(UserID, cardCode, itemCode, Quantity, refDate)
        Else
            Dim a As New SAP_Functions
            Return a.GetDefaultLineInfo(UserID, cardCode, itemCode, Quantity, refDate)
        End If
    End Function

    <WebMethod()> _
    Public Function GetDefaultBP(UserID As String, CardType As String) As DataSet
        If PublicVariable.Simulate Then
            Dim a As New Simulation
            Return a.Simulate_GetDeafaultBP(UserID, CardType)
        Else
            Dim a As New SAP_Functions
            Return a.GetDefaultBP(UserID, CardType)
        End If
    End Function
#Region "Apply Promotion"
    '- Neu WS GetPromotion tra ve nhieu hon 1 record thi show page "Promotion Selection" de chon 1, sau do theo logic duoi.

    '- Neu WS GetPromotion Tra ve 1 record thi theo logic duoi.

    'Logic Apply Promotion:
    '1. Discount By Promotion = HeadDscAmt + HeadDscPer*UnitPrice/100 + ProValue
    '2. Unit Price = Unit Price - Discount By Promotion
    '3. So le = field Sole
    '4. ProCode = field ProCode
    '5. Neu ProQty>0: them 1 dong vao grid
    '    Item Code= Item Code cua dong apply
    '    Description = Description cua dong apply
    '    Quantity=ProQty
    '    Unit Price,Discount,Discount By Promotion  = 0
    '    Warehouse = WS GetPromotionWarehouse
    '    U_ProLine = Y
    '    ProCode = field ProCode
    '    So le = field Sole
#End Region
    <WebMethod()> _
    Public Function GetPromotion(UserID As String, ItemCode As String, CardCode As String, _
                                 Quantity As Double, DocDate As Date, Amount As Double) As DataSet
        If PublicVariable.Simulate Then
            Dim a As New Simulation
            Return a.Simulate_Promotion(UserID, ItemCode, CardCode, Quantity, DocDate, Amount)
        Else
            Dim a As New SAP_Functions
            Return a.GetPromotion(UserID, ItemCode, CardCode, Quantity, DocDate, Amount)
        End If
    End Function
    <WebMethod()> _
    Public Function GetCopyFromTo(Type As Integer, ObjType As String) As DataSet
        Dim a As New SAP_Functions
        'Type=1: Copy To
        'Type=2: Copy From
        'ObjType=22: Purchase Order
        Return a.GetGopyFromTo(Type, ObjType)
    End Function
    <WebMethod()> _
    Public Function GetConnection(UserID As String) As String
        Dim connect As New Connection()
        Dim str As String
        str = connect.connectDB(UserID)
        Return str
    End Function
    <WebMethod()> _
    Public Function LogOut(UserID As String) As Integer
        Try
            Dim connect As New Connection()
            Connection.bConnect = False
            PublicVariable.oCompany.Disconnect()
        Catch ex As Exception

        End Try
    End Function

    <WebMethod()> _
    Public Function GetLoginInfo(UserID As String, EmpID As Integer) As DataSet
        connect.setDB(UserID)
        Return connect.ObjectGetAll_Query_SAP("exec sp_AI_LoadEmpID '" + UserID + "'")
    End Function
    <WebMethod()> _
    Public Function GetNextPreviousID(ObjType As String, UserID As String, TableName As String, KeyName As String, CurrentKey As String) As DataSet
        Try
            connect.setDB(UserID)
            Dim strFilter As String = ""
            Select Case ObjType
                Case "60-202"
                    strFilter = " And exists(select top(1) * from IGE1 T0 where T0.DocEntry=T.DocEntry and BaseType='202')"
                Case "59-202"
                    strFilter = " And exists(select top(1) * from IGN1 T0 where T0.DocEntry=T.DocEntry and BaseType='202')"
                Case "60"
                    strFilter = " And exists(select top(1) * from IGE1 T0 where T0.DocEntry=T.DocEntry and BaseType='-1')"
                Case "59"
                    strFilter = " And exists(select top(1) * from IGN1 T0 where T0.DocEntry=T.DocEntry and BaseType='-1')"
            End Select
            Dim str As String = "select Isnull(Nex,Fir) Nex,isnull(Pre,Las) Pre,Fir,Las from ( select  "
            'next'
            str = str + "(select top(1) " + KeyName + " from " + TableName + " T where U_UserID='" + UserID + "' " + strFilter + " and " + KeyName + ">'" + CurrentKey + "' order by " + KeyName + ") Nex,"
            'previous
            str = str + "(select top(1) " + KeyName + " from " + TableName + " T where U_UserID='" + UserID + "' " + strFilter + " and " + KeyName + "<'" + CurrentKey + "' order by " + KeyName + " Desc) Pre,"
            'first
            str = str + "(select top(1) " + KeyName + " from " + TableName + " T where U_UserID='" + UserID + "' " + strFilter + " and " + KeyName + ">'' order by " + KeyName + ") Fir,"
            'last
            str = str + "(select top(1) " + KeyName + " from " + TableName + " T where U_UserID='" + UserID + "' " + strFilter + " and " + KeyName + ">'' order by " + KeyName + " Desc) Las"
            str = str + " ) T0"
            Return connect.ObjectGetAll_Query_SAP(str)
        Catch
            Return Nothing
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOpenDocument(UserID As String, CardCode As String, DocType As String) As DataSet
        If PublicVariable.Simulate Then
            Dim a As New Simulation
            Return a.Simulate_GetDeafaultBP(UserID, "")
        Else
            Dim dt As DataSet
            Dim connect As New Connection()
            connect.setDB(UserID)
            Dim str As String
            str = "select DocEntry,DocDate,DocDueDate,Comments,DocTotal from "
            Select Case DocType
                Case "19"
                    str = str + "ORPC"
                Case "20"
                    str = str + "OPDN"
                Case "21"
                    str = str + "ORPD"
                Case "22"
                    str = str + "OPOR"

                Case "13"
                    str = str + "OINV"
                Case "14"
                    str = str + "ORIN"
                Case "15"
                    str = str + "ODLN"

                Case "59"
                    str = str + "OIGN"
                Case "60"
                    str = str + "OIGE"

                Case "97"
                    str = str + "OOPR"

            End Select
            str = str + " where DocStatus='O' and CardCode='" + CardCode + "'"
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        End If
    End Function

    <WebMethod()> _
    Public Function CreateUDF(UserID As String) As String
        If PublicVariable.Simulate Then
            Dim a As New Simulation
            Return ""
        Else
            Dim a As New SAP_Functions
            a.CreateUDF("ORDR", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Marketing Document
            a.CreateUDF("OCRD", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) ' BP
            a.CreateUDF("OITM", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Item
            a.CreateUDF("OVPM", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Banking
            a.CreateUDF("OJDT", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'JE
            a.CreateUDF("OITT", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'BOM
            a.CreateUDF("OWOR", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Production
            a.CreateUDF("OSCL", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Service Call
            a.CreateUDF("OCLG", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Activity
            a.CreateUDF("OOPR", "UserID", "UserID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, UserID) 'Sales Opportunity
            Return ""
        End If
    End Function

    <WebMethod()> _
    Public Function TestConnection(ConnectionString As String) As String
        Dim MyArr As Array
        Dim sErrMsg As String = ""
        Dim connectOk As Integer = 0
        MyArr = ConnectionString.Split(";")
        Dim sCon As String = "server= " + MyArr(3).ToString() + ";database=" + MyArr(0).ToString() + " ;uid=" + MyArr(4).ToString() + "; pwd=" + MyArr(5).ToString() + ";"
        Dim sConnSAP As SqlConnection = New SqlConnection(sCon)

        Try
            sConnSAP.Open()

            Dim newcompayne As New SAPbobsCOM.Company
            newcompayne.CompanyDB = MyArr(0).ToString()
            newcompayne.UserName = MyArr(1).ToString()
            newcompayne.Password = MyArr(2).ToString()
            newcompayne.Server = MyArr(3).ToString()
            newcompayne.DbUserName = MyArr(4).ToString()
            newcompayne.DbPassword = MyArr(5).ToString()
            newcompayne.LicenseServer = MyArr(6)
            If MyArr(7) = 2008 Then
                newcompayne.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
            Else
                newcompayne.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005
            End If

            If newcompayne.Connect() <> 0 Then
                newcompayne.GetLastError(connectOk, sErrMsg)
                Return sErrMsg
            Else
                Return "OK"
            End If

        Catch ex As Exception
            Return ex.ToString
        End Try

        
    End Function

    <WebMethod()> _
    Public Function GetBOMChild(UserID As String, ItemCode As String) As DataSet
        Try
            Dim dt As New DataSet("ITT1")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                Dim str As String = ""
                str = "select ROW_NUMBER() Over(Order By ItemCode) No,T1.ItemCode,T1.ItemName Dscription,T0.Quantity PlannedQty,T0.IssueMthd IssueType,T0.Warehouse wareHouse, "
                str = str + " 0 IssuedQty,T0.Quantity BaseQty"
                str = str + " from ITT1 T0 join OITM T1 on T1.ItemCode=T0.Code where T0.Father='" + ItemCode + "'"
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    <WebMethod()> _
    Public Function GetUDF(UserID As String, TableID As String) As DataSet
        connect.setDB(UserID)
        Return connect.ObjectGetAll_Query_SAP("select AliasID,Descr,RTable from cufd where tableid='" + TableID + "'")
    End Function

    <WebMethod()> _
    Public Function GetDefaultPayment(UserID As String, Year As Integer) As DataSet
        connect.setDB(UserID)
        Dim str As String = ""
        str = str + " select LinkAct_3 CashAcctCode, (select AcctName from OACT where AcctCode=LinkAct_3) CashAcctName,"
        str = str + " LinkAct_12 TransferAcctCode, (select AcctName from OACT where AcctCode=LinkAct_12) TransferAcctName"
        str = str + " from OACP T0 where YEAR=" + CStr(Year)
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function
    <WebMethod()> _
    Public Function GetIssueForProduction(UserID As String) As DataSet
        connect.setDB(UserID)
        Dim str As String = ""
        str = str + " select T0.DocEntry BaseEntry,T1.LineNum BaseLine,T1.ItemCode,T2.ItemName Dscription, "
        str = str + " T1.PlannedQty-t1.IssuedQty Quantity,T1.wareHouse WhsCode, "
        str = str + " (select CASE when  A.GLMethod='C' then B.WipAcct when  A.GLMethod='W' then C.WipAcct else D.WipAcct end from OITM A"
        str = str + " join OITB B on A.ItmsGrpCod=B.ItmsGrpCod join OWHS C on C.WhsCode=T1.wareHouse"
        str = str + " join OITW D on D.ItemCode=A.ItemCode and D.WhsCode=T1.wareHouse WHERE A.ItemCode=T1.ItemCode) AcctCode"
        str = str + " from OWOR T0 join WOR1 T1 on T0.DocEntry=T1.DocEntry"
        str = str + " join OITM T2 on t2.ItemCode=T1.ItemCode"
        str = str + " where T0.Status='R' and T1.IssueType='M' and T1.PlannedQty-t1.IssuedQty>0"
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function
    <WebMethod()> _
    Public Function GetReceiptProduction(UserID As String) As DataSet
        connect.setDB(UserID)
        Dim str As String = ""
        str = str + " select T0.DocEntry BaseEntry,T0.ItemCode,T2.ItemName Dscription,  "
        str = str + " T0.PlannedQty-T0.CmpltQty Quantity,T0.wareHouse WhsCode,'C' TranType, "
        str = str + " (select CASE when  A.GLMethod='C' then B.WipAcct when  A.GLMethod='W' then C.WipAcct else D.WipAcct end from OITM A"
        str = str + " join OITB B on A.ItmsGrpCod=B.ItmsGrpCod join OWHS C on C.WhsCode=T0.wareHouse"
        str = str + " join OITW D on D.ItemCode=A.ItemCode and D.WhsCode=T0.wareHouse WHERE A.ItemCode=T0.ItemCode) AcctCode"
        str = str + " from OWOR T0"
        str = str + " join OITM T2 on T2.ItemCode=T0.ItemCode where T0.Status='R' and T0.PlannedQty-T0.CmpltQty>0 "
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function

    <WebMethod()> _
    Public Function GetLastVendorPrice_PR(UserID As String, ItemCode As String) As DataSet
        Try
            Dim dt As New DataSet("OITM")
            Dim str As String
            str = "select top(1) T0.DocDate,T0.DocEntry,T0.LineNum,T1.CardName,T0.PriceAfVAT "
            str = str + " from por1 T0 join OPOR T1 on T0.DocEntry=T1.DocEntry "
            str = str + " where T1.CANCELED='N' and ItemCode='" + ItemCode + "'"
            str = str + " order by DocDate desc , DocEntry desc,LineNum  desc"

            connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
End Class