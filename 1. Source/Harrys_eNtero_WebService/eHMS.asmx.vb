Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class eHMS
    Inherits System.Web.Services.WebService
    Dim connect As New Connection()
#Region "Banquet"
    <WebMethod()> _
    Public Function PackageMaster_banquet(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select oi.Code, tm.ItemName, tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode,t1.Price as BasePrice, dbo.usf_getChildList(oi.Code) as Items ,'N' IsTaxIncluded,"
        str = str + " tm.U_SCPer SCPercent, T2.Rate VATPercent, case when isnull(oi.U_IsTax,'N')='N' then t1.Price*(100+tm.U_SCPer+T2.Rate)/100 else t1.Price end PriceAfterTaxes,"
        str = str + " tm.U_Hotel,tm.U_RevenueCode ItemGroup,tm.U_Hotel"
        str = str + " from OITT oi join OITM tm on oi.Code = tm.ItemCode"
        str = str + " join ITM1 t1 on t1.ItemCode = tm.ItemCode and t1.PriceList = 1"
        str = str + " join OVTG T2 on T2.Code=VatGourpSa"
        str = str + " where oi.TreeType = 'S' and oi.U_BPOSFlag = 'Y' and isnull(tm.U_Hotel,'')='" + HotelCode + "'"

        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function AddonMaster_Banquet(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select tm.ItemCode, tm.ItemName,tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode, t1.Price as BasePrice, '' as Items ,tm.U_IsTax IsTaxIncluded,tm.U_SCPer SCPercent, T2.Rate VATPercent,"
        str = str + " case when isnull(tm.U_IsTax,'N')='N' then t1.Price*(100+tm.U_SCPer+T2.Rate)/100 else t1.Price end PriceAfterTaxes,tm.U_RevenueCode ItemGroup,tm.U_Hotel"
        str = str + " from OITM tm join ITM1 t1 on t1.ItemCode = tm.ItemCode"
        str = str + " join OVTG T2 on T2.Code=VatGourpSa"
        str = str + " where t1.PriceList = 1 and tm.U_BPOSFlag = 'Y' and tm.U_ItemSource = 'SVC' and ISNULL(tm.U_Hotel,'')='" + HotelCode + "'"
        Return RunQuery(str)
    End Function


#End Region
    '<WebMethod()> _
    'Public Function PackageSelection(RoomType As String) As DataSet
    '    Try
    '        Dim str As String
    '        str = "exec sp_AI_PackageDetail '" & RoomType & "'"
    '        Dim dt As DataSet
    '        Dim connect As New Connection()
    '        connect.setDB("")
    '        dt = connect.ObjectGetAll_Query_SAP(str)
    '        Return dt
    '    Catch ex As Exception
    '        Return Nothing
    '    End Try
    'End Function
    <WebMethod()> _
    Public Function PackageSelection(MarkSeg As String, RoomType As String, CardCode As String, _
                                 Quantity As Double, DocDate As Date, GroupCode As Integer) As DataSet
        Return GetPackageList1(RoomType, CardCode, Quantity, DocDate, GroupCode)
    End Function

    '<WebMethod()> _
    'Public Function PackageSelection(MarkSeg As String, ItemCode As String, CardCode As String, _
    '                             Quantity As Integer, DocDate As Date, CustGroupCode As Integer) As DataSet
    '    Return GetPackageList(MarkSeg, ItemCode, CardCode, Quantity, DocDate, CustGroupCode)
    'End Function

    <WebMethod()> _
    Public Function PackageDetail(MarkSeg As String, PackageCode As String, CardCode As String, _
                                 Quantity As Integer, DocDate As Date, CustGroupCode As Integer) As DataSet
        Return GetPackageList(MarkSeg, PackageCode, CardCode, Quantity, DocDate, CustGroupCode)
    End Function
    Public Function GetPackageList(MarkSeg As String, ItemCode As String, CardCode As String, _
                                 Quantity As Integer, DocDate As Date, CustGroupCode As Integer) As DataSet

        Try
            Dim str As String
            str = "exec AI_SP_Package_Price '" & MarkSeg & "', '" & ItemCode & "'," & CStr(CustGroupCode) & ",'" & CStr(CardCode) & "'," & CStr(Quantity) & ",'" & DocDate.ToString("yyyy-MM-dd") & "'"
            Dim dt As DataSet
            Dim connect As New Connection()
            connect.setDB("")
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetPackageList1(RoomType As String, CardCode As String, _
                                 Quantity As Double, DocDate As Date, GroupCode As Integer) As DataSet
        Try
            Dim str As String
            str = "exec sp_AI_PackageSelection '" & RoomType & "'," & CStr(GroupCode) & ",'" & CStr(CardCode) & "'," & CStr(Quantity) & ",'" & DocDate.ToString("yyyy-MM-dd") & "'"
            Dim dt As DataSet
            Dim connect As New Connection()
            connect.setDB("")
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    <WebMethod()> _
    Public Function AddonSelection(CardCode As String, DocDate As Date, GroupCode As Integer) As DataSet
        Return GetAddonList(CardCode, DocDate, GroupCode)
    End Function
    <WebMethod()> _
    Private Function GetSAPItemPrice(ByVal cardCode As String, ByVal itemCode As String, ByVal amount As Single, ByVal refDate As Date) As Double
        Try
            Dim vObj As SAPbobsCOM.SBObob
            Dim rs As SAPbobsCOM.Recordset
            vObj = PublicVariable.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge)
            rs = PublicVariable.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            rs = vObj.GetItemPrice(cardCode, itemCode, amount, refDate)
            If rs.RecordCount > 0 Then
                Return rs.Fields.Item(0).Value
            Else
                Return 0
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Function GetAddonList(CardCode As String, DocDate As Date, GroupCode As Integer) As DataSet
        Try
            Dim str As String
            str = "exec sp_AI_AddonSelection " & CStr(GroupCode) & ",'" & CStr(CardCode) + "','" & DocDate.ToString("yyyy-MM-dd") & "'"
            Dim dt As DataSet
            Dim connect As New Connection()
            connect.setDB("")
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function PackageMaster(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select oi.Code, tm.ItemName, tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode,t1.Price as BasePrice, dbo.usf_getChildList(oi.Code) as Items ,'N' IsTaxIncluded,"
        str = str + " tm.U_SCPer SCPercent, T2.Rate VATPercent, case when isnull(oi.U_IsTax,'N')='N' then t1.Price*(100+tm.U_SCPer+T2.Rate)/100 else t1.Price end PriceAfterTaxes,"
        str = str + " tm.U_Hotel,tm.U_RevenueCode ItemGroup,tm.U_Hotel"
        str = str + " from OITT oi join OITM tm on oi.Code = tm.ItemCode"
        str = str + " join ITM1 t1 on t1.ItemCode = tm.ItemCode and t1.PriceList = 1"
        str = str + " join OVTG T2 on T2.Code=VatGourpSa"
        str = str + " where oi.TreeType = 'S' and oi.U_POSFlag = 'Y' and isnull(tm.U_Hotel,'')='" + HotelCode + "'"

        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function AddonMaster(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select tm.ItemCode, tm.ItemName,tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode, t1.Price as BasePrice, '' as Items ,tm.U_IsTax IsTaxIncluded,tm.U_SCPer SCPercent, T2.Rate VATPercent,"
        str = str + " case when isnull(tm.U_IsTax,'N')='N' then t1.Price*(100+tm.U_SCPer+T2.Rate)/100 else t1.Price end PriceAfterTaxes,tm.U_RevenueCode ItemGroup,tm.U_Hotel"
        str = str + " from OITM tm join ITM1 t1 on t1.ItemCode = tm.ItemCode"
        str = str + " join OVTG T2 on T2.Code=VatGourpSa"
        str = str + " where t1.PriceList = 8 and tm.U_POSFlag = 'Y' and tm.U_ItemSource = 'SVC' and ISNULL(tm.U_Hotel,'')='" + HotelCode + "'"
        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function OtherCharges(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select tm.ItemCode, tm.ItemName,tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode, t1.Price as BasePrice, '' as Items ,tm.U_IsTax IsTaxIncluded,tm.U_SCPer SCPercent, T2.Rate VATPercent,"
        str = str + " case when isnull(tm.U_IsTax,'N')='N' then t1.Price*(100+tm.U_SCPer+T2.Rate)/100 else t1.Price end PriceAfterTaxes,tm.U_RevenueCode ItemGroup,tm.U_Hotel"
        str = str + " from OITM tm join ITM1 t1 on t1.ItemCode = tm.ItemCode"
        str = str + " join OVTG T2 on T2.Code=VatGourpSa"
        str = str + " where t1.PriceList = 8 and tm.U_POSFlag = 'Y' and tm.U_ItemSource = 'OTH' and ISNULL(tm.U_Hotel,'')='" + HotelCode + "'"
        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function RoomTypeMaster(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select tm.ItemCode, tm.ItemName,tm.ItmsGrpCod,Isnull(tm.U_TCode,'') U_TCode, t1.Price as BasePrice, '' as Items, tm.U_Hotel"
        str = str + " from OITM tm join ITM1 t1 on t1.ItemCode = tm.ItemCode and t1.PriceList = 1 and tm.U_POSFlag = 'Y' and tm.U_ItemSource = 'ROOM'"
        str = str + " where isnull(tm.U_Hotel,'')= '" + HotelCode + "'"
        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function MeetingRoomTypeMaster(ByVal HotelCode As String) As DataSet
        Dim str As String = ""
        str = str + " select tm.ItemCode, tm.ItemName, t1.Price as BasePrice, '' as Items, U_Hotel from OITM tm"
        str = str + " join ITM1 t1 on t1.ItemCode = tm.ItemCode and t1.PriceList = 1 and tm.U_POSFlag = 'Y' and tm.U_ItemSource = 'MEETING'"
        str = str + " where isnull(tm.U_Hotel,'')= '" + HotelCode + "'"
        Return RunQuery(str)
    End Function
    <WebMethod()> _
    Public Function CorporateMaster(ByVal HotelCode As String) As DataSet
        Return RunQuery("Select CardCode, CardName,GroupName from OCRD T0 join OCRG T1 on T0.GroupCode=T1.GroupCode where T0.CardType='C' and T1.GroupName like '%Corporate%' and isnull(T0.U_HotelCode,'')='" + HotelCode + "'")
    End Function
    <WebMethod()> _
    Public Function TravelAgentMaster(ByVal HotelCode As String) As DataSet
        Return RunQuery("Select CardCode, CardName,GroupName from OCRD T0 join OCRG T1 on T0.GroupCode=T1.GroupCode where T0.CardType='C' and T1.GroupName like '%Travel%' and isnull(T0.U_HotelCode,'')='" + HotelCode + "'")
    End Function

    '<WebMethod()> _
    'Public Function AgencyMaster() As DataSet
    '    Return RunQuery("Select CardCode, CardName,GroupName from OCRD T0 join OCRG T1 on T0.GroupCode=T1.GroupCode where T0.CardType='C' and T1.GroupName like '%Agency%'")
    'End Function
    Public Function RunQuery(str As String) As DataSet
        Try
            Dim dt As DataSet
            Dim connect As New Connection()
            connect.setDB("")
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    
    
End Class