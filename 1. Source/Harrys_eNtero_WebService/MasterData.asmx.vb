Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://electra-ai.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class MasterData
    Inherits System.Web.Services.WebService
    Dim connect As New Connection()
    <WebMethod()> _
    Public Function GetBPGroup(CardType As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCRG")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OCRD(CardType)
            Else
                Dim str As String = ""
                If CardType = "S" Then
                    str = "select GroupCode,GroupName from OCRG where GroupType='S'"
                Else
                    str = "select GroupCode,GroupName from OCRG where GroupType<>'S'"
                End If

                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If

            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetBusinessPartner(CardType As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCRD")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OCRD(CardType)
            Else
                Dim str As String
                Select Case CardType
                    Case "A" 'ALL BP
                        str = "Select CardCode,CardName from OCRD"
                    Case "CA" 'CUSTOMER + LEAD
                        str = "Select CardCode,CardName from OCRD Where CardType in ('C','L')"
                    Case "CS" 'CUSTOMER + VENDOR
                        str = "Select CardCode,CardName from OCRD Where CardType in ('C','S')"
                    Case Else 'CUSTOMER or VENDOR or LEADD
                        str = "Select CardCode,CardName from OCRD Where CardType='" + CardType + "'"
                End Select
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If

            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetItemMasterData(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OITM")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OITM
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("Select * from OITM order by ItemCode")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetItemMasterDataFilter(UserID As String, Filter As String) As DataSet
        Try
            Dim dt As New DataSet("OITM")
            Dim str As String
            Select Case Filter
                Case 0 'Food - group name like 
                    str = "Select T0.* from OITM T0 with(nolock) join OITB T1 with(nolock) on T0.ItmsGrpCod=T1.ItmsGrpCod where T1.ItmsGrpNam like '%food%' order by ItemCode"
                Case 1 'Food - group name like 
                    str = "Select T0.* from OITM T0 with(nolock) join OITB T1 with(nolock) on T0.ItmsGrpCod=T1.ItmsGrpCod where T1.ItmsGrpNam like '%beverage%' order by ItemCode"
                Case 2 'purchase
                    str = "Select T0.* from OITM T0 with(nolock) where T0.U_ItemSource not in ('ROOM','PACKAGE','SVC') order by ItemCode"
                Case 3 'food transfer request
                    str = "Select T0.* from OITM T0 with(nolock) join OITB T1 with(nolock) on T0.ItmsGrpCod=T1.ItmsGrpCod where T1.ItmsGrpNam like '%food%' order by ItemCode"
                Case 4 'inventory item
                    str = "Select T0.* from OITM T0 with(nolock) where T0.invntItem='Y' order by ItemCode"
                Case Else
                    str = "Select T0.* from OITM T0 with(nolock) order by ItemCode"
            End Select
            If Filter <> "" Then

            Else

            End If
            connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetWarehouse(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OWHS")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OWHS
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("Select WhsCode,WhsName from OWHS order by WhsCode")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetTaxGroup(ByVal Category As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OVTG")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OTVG
            Else
                connect.setDB(UserID)
                Dim str As String
                If Category = "A" Then
                    str = "Select Code,Name,rate from OVTG order by Code"
                Else
                    str = "Select Code,Name,rate from OVTG where Category='" + Category + "' order by Code"
                End If

                dt = connect.ObjectGetAll_Query_SAP(str)
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetEmployeeMasterData(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OHEM")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OHEM
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("Select empID Code,* from ohem")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetSalesBuyerMasterData(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OSLP")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OSLP
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select SlpCode Code,SlpName Name from OSLP order by SlpCode")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetAccountMasterData(Filter As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OACT")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OACT
            Else
                Dim str As String
                str = "select AcctCode,AcctName,FrgnName from OACT where Postable='Y'"
                Select Case Filter
                    Case "Revenue"
                        str = str + " and ActType='I'"
                    Case "AR"
                        str = str + " and LocManTran='Y' and GroupMask=1"
                    Case "AP"
                        str = str + " and LocManTran='Y' and GroupMask=2"
                End Select
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetContactPerson(ByVal CardCode As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCPR")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OCPR(CardCode)
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select CntctCode Code,Name FirstName,'' LastName, 0 IsDefault from OCPR where CardCode='" + CardCode + "'")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetProject(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OPRJ")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OPRJ
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select PrjCode,PrjName from OPRJ ")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetShippingType(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OSHP")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OSHP
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select TrnspCode,TrnspName from OSHP")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetBPCurrency(CardCode As String, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCRD")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_BPCurrency(CardCode)
            Else
                Dim str As String
                If CardCode = "" Then
                    str = "select T0.CurrCode,T0.CurrName from OCRN T0 "
                Else
                    str = "select T1.CurrCode,T1.CurrName from ocrd T0 "
                    str = str + " full join OCRN T1 oN T0.Currency=T1.CurrCode or T0.Currency='##'"
                    str = str + " where T0.cardcode='" + CardCode + "'"
                End If
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If

            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetIndicator(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OIDC")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OIDC
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("Select Code,Name from OIDC")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPaymentTerm(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("octg")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OCTG
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select GroupNum,PymntGroup from octg order by GroupNum")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '----------------------------------------------OTHER MASTER DATA---------------------------------
    <WebMethod()> _
    Public Function GetItemGroup(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OITB")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OITB
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("Select ItmsGrpCod,ItmsGrpNam from OITB")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPriceList(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OPLN")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OPLN
            Else
                connect.setDB(UserID)
                Dim str As String = ""
                str = "select ListNum,ListName from opln "
                str = str + " union all select -1,'Last Purchase Price' "
                str = str + " union all select -2,'Last Evaluated Price' "
                dt = connect.ObjectGetAll_Query_SAP(str)
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetManufacture(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OMRC")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OMRC
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select FirmCode,FirmName from OMRC")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetIndustry(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OOND")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OOND
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select IndCode,IndName from OOND")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetTerritory(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OTER")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OIDC
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select T0.territryID,T0.descript,T1.descript Parent from OTER T0 left join OTER T1 on T1.territryID=T0.parent")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetDisplaySetting(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OADM")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OADM
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select CompnyName,DecSep,ThousSep,SumDec,PriceDec,QtyDec,PercentDec,RateDec,DateFormat,DateSep from OADM")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetCompanySetting(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OADP")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OADM
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select (select AttachPath from OADP) AttachPath, (select compnyName from oadm) CompanyName")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetCostCenter(DimCode As Integer, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OPRC")
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                dt = a.Simulate_OPRC
            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select PrcCode,PrcName from OPRC where DimCode=" + CStr(DimCode))
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetInformationSource(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OOSR")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select Num,Descript from OOSR")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetStage(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OOST")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select Num,Descript,CloPrcnt from OOST")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPartners(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OPRT")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select PrtId,Name from OPRT")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetCompetitor(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCMT")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select CompetId,Name from OCMT")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetLevelOfInterest(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OOIR")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select Num,Descript from OOIR")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetActivityType(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCLT")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select code,name from OCLT")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetActivitySubject(UserID As String, Type As Integer) As DataSet
        Try
            Dim dt As New DataSet("OCLS")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select code,name from OCLS where type=" + CStr(Type))
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetSAPUser(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OUSR")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select UserID,User_Code from OUSR")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetBOM(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OITT")
            If PublicVariable.Simulate Then

            Else
                connect.setDB(UserID)
                dt = connect.ObjectGetAll_Query_SAP("select T1.ItemCode,T1.ItemName from OITT T0 join OITM T1 on T0.code=T1.ItemCode")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetSeries(UserID As String, ObjType As String, SubObjType As String) As DataSet
        connect.setDB(UserID)
        Dim str As String = "select * from nnm1 where ObjectCode='" + ObjType + "'"
        If SubObjType <> "" Then
            str = str + " and DocSubType='" + SubObjType + "'"
        End If
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function
    <WebMethod()> _
    Public Function GetUDTValue(UserID As String, UDT As String) As DataSet
        connect.setDB(UserID)
        Dim str As String = "select * from [@" + UDT + "]"
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function
    <WebMethod()> _
    Public Function GetCashFlowItem(UserID As String) As DataSet
        connect.setDB(UserID)
        Dim str As String = "select CFWId,CFWName from ocfw where Postable='Y'"
        Return connect.ObjectGetAll_Query_SAP(str)
    End Function
    <WebMethod()> _
    Public Function GetCustomGroup(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OARG")
                connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP("Select CstGrpCode,CstGrpName from OARG")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOrderInterval(UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCYC")
            connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP("Select Code,Name from OCYC")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetTransValue(ByVal UserID As String, ByVal ItemCode As String) As DataSet
        Try
            Dim dt As New DataSet("OINM")
            connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP("select top 1 oi.ItemCode, abs(oi.TransValue) TransValue from OINM oi where oi.TransType = 60 and oi.ItemCode = '" + ItemCode + "' order by oi.TransNum desc")

            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function

End Class