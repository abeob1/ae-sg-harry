Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Transaction
    Inherits System.Web.Services.WebService

    Dim lRetCode As Integer
    Dim lErrCode As Integer
    Dim sErrMsg As String
    Dim connect As New Connection()
    <WebMethod()> _
    Public Function CreateMarketingDocument(ByVal strXml As String, UserID As String, DocType As String, Key As String, IsUpdate As Boolean) As DataSet
        Dim b As New SAP_Functions
        Try
            Dim sStr As String = "Operation Completed Successfully!"
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                Return a.Simulate_CreateTransaction()
            Else

                Dim oDocment
                Select Case DocType
                    Case "30"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                    Case "97"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                    Case "191"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                    Case "33"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                    Case "221"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                    Case "2"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                    Case "28"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                    Case Else
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
                End Select

                Dim constr As String
                constr = connect.connectDB(UserID)
                If constr <> "" Then
                    Return b.ReturnMessage(-1, constr)
                End If

                PublicVariable.oCompany.XMLAsString = True
                oDocment = PublicVariable.oCompany.GetBusinessObjectFromXML(strXml, 0)
                If IsUpdate Then
                    If oDocment.GetByKey(Key) Then
                        oDocment.Browser.ReadXML(strXml, 0)
                        lErrCode = oDocment.Update()
                    Else
                        Return b.ReturnMessage(-1, "Record not found!")
                    End If
                Else
                    lErrCode = oDocment.Add()
                End If

                If lErrCode <> 0 Then
                    PublicVariable.oCompany.GetLastError(lErrCode, sErrMsg)
                    Return b.ReturnMessage(lErrCode, sErrMsg)
                Else
                    Return b.ReturnMessage(lErrCode, "Operation Sucessful!")
                End If
            End If

        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString)
        End Try
    End Function
    <WebMethod()> _
    Public Function GetMarketingDocument(DocType As String, DocEntry As String, UserID As String) As String
        Try
            Dim sStr As String = ""
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                Return a.Simulate_OPOR
            Else
                Dim oDocment
                Select Case DocType
                    Case "30"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                    Case "97"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                    Case "191"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                    Case "33"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                    Case "221"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                    Case "2"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                    Case "171" 'Employee
                        oDocment = DirectCast(oDocment, SAPbobsCOM.ContactEmployees)
                    Case "206" '
                        oDocment = DirectCast(oDocment, SAPbobsCOM.UserObjectsMD)
                    Case "28"
                        oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                    Case Else
                        oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)

                End Select
                '-----------------TEST--------------------
                'oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)


                Dim constr As String = connect.connectDB(UserID)
                If constr <> "" Then
                    Return constr
                End If

                PublicVariable.oCompany.XMLAsString = True
                PublicVariable.oCompany.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ValidNodesOnly

                oDocment = PublicVariable.oCompany.GetBusinessObject(DocType)
                If DocType = "206" Then
                    oDocment.ObjectType = SAPbobsCOM.BoUDOObjType.boud_Document
                End If
                '-----------------TEST--------------------
                'oDocment = PublicVariable.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oContacts)
                If oDocment.GetByKey(DocEntry) Then
                    oDocment.SaveXML(sStr)
                    Return sStr
                Else
                    Return "Error: docentry not found"
                End If
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function
    <WebMethod()> _
    Public Function GetMarketingDocument_ReturnDS(DocType As String, DocEntry As String, UserID As String) As DataSet
        Try
            Dim sStr As String = ""
            If PublicVariable.Simulate Then
                Return Nothing
            Else
                Dim connect As New Connection()


                Dim HeaderTableName As String = ""
                Dim LineTableName1 As String = ""
                Dim LineTableName2 As String = ""
                Dim KeyName As String = "DocEntry"
                Dim strFilter As String = ""
                Select Case DocType
                   

                        '----------------------PURCHASE------------------------
                    Case "18" 'AP INVOICE
                        HeaderTableName = "OPCH"
                        LineTableName1 = "PCH1"
                    Case "19" 'AP CREDIT
                        HeaderTableName = "ORPC"
                        LineTableName1 = "RPC1"
                    Case "20" 'GOODS RECEIPT PO
                        HeaderTableName = "OPDN"
                        LineTableName1 = "PDN1"
                    Case "21" 'GOODS RETURN
                        HeaderTableName = "ORPD"
                        LineTableName1 = "RPD1"
                    Case "22" 'PURCHASE ORDER
                        HeaderTableName = "OPOR"
                        LineTableName1 = "POR1"
                    Case "540000006" 'PURCHASE QUOTATION"
                        HeaderTableName = "OPQT"
                        LineTableName1 = "PQT1"
                    Case "1470000113" 'PURCHASE REQUEST"
                        HeaderTableName = "OPRQ"
                        LineTableName1 = "PRQ1"
                        '-------------------SALES--------------------------
                    Case "13" 'AR INVOICE
                        HeaderTableName = "OINV"
                        LineTableName1 = "INV1"
                    Case "14" 'AR CREDIT
                        HeaderTableName = "ORIN"
                        LineTableName1 = "RIN1"
                    Case "15" 'DELIVERY
                        HeaderTableName = "ODLN"
                        LineTableName1 = "DLN1"
                    Case "16" 'RETURN
                        HeaderTableName = "ORDN"
                        LineTableName1 = "RDN1"
                    Case "17" 'SALES ORDER
                        HeaderTableName = "ORDR"
                        LineTableName1 = "RDR1"
                    Case "23" 'SALES QUOTATION
                        HeaderTableName = "OQUT"
                        LineTableName1 = "QUT1"

                        '---------------------INVENTORY----------------------
                    Case "59" 'GOODS RECEIPT
                        HeaderTableName = "OIGN"
                        LineTableName1 = "IGN1"
                    Case "60" 'GOODS ISSUE
                        HeaderTableName = "OIGE"
                        LineTableName1 = "IGE1"
                    Case "67" 'TRANSFER
                        HeaderTableName = "OWTR"
                        LineTableName1 = "WTR1"
                    Case "1250000001" 'TRANSFER REQUEST
                        HeaderTableName = "OWTQ"
                        LineTableName1 = "WTQ1"

                        '-------------------MASTER DATA------------------
                    Case "171" 'EMPLOYEE
                        HeaderTableName = "OHEM"
                        LineTableName1 = "HEM1"
                        KeyName = "empID"
                    Case "2" 'BUSINESS PARTNER
                        HeaderTableName = "OCRD"
                        LineTableName1 = "CRD1"
                        LineTableName2 = "OCPR"
                        KeyName = "cardcode"
                    Case "4" 'ITEM
                        HeaderTableName = "OITM"
                        LineTableName1 = ""
                        KeyName = "itemcode"
                    Case "11" 'CONTACT PERSON
                        HeaderTableName = "OCPR"
                        LineTableName1 = ""
                        KeyName = "cardcode"
                    Case "12" 'USER
                        HeaderTableName = "OUSR"
                        LineTableName1 = "OUSR"
                        KeyName = "USERID"

                        '----------------------OTHERS--------------------
                    Case "221" 'Attachment
                        HeaderTableName = "OATC"
                        LineTableName1 = "ATC1"
                        KeyName = "AbsEntry"
                    Case "97" 'Sales opportunity
                        HeaderTableName = "OOPR"
                        LineTableName1 = "OPR1"
                    Case "33" 'Activity
                        HeaderTableName = "OCLG"
                        LineTableName1 = ""
                        KeyName = "ClgCode"
                    Case "30" 'Journal Entry
                        HeaderTableName = "OJDT"
                        LineTableName1 = "JDT1"
                        KeyName = "TransID"

                        '-------------------PRODUCTION--------------------
                    Case "202" 'Production Order
                        HeaderTableName = "OWOR"
                        LineTableName1 = "WOR1"
                        'Case "60-202" 'ISSUE for production
                        '    HeaderTableName = "OIGE"
                        '    LineTableName1 = "IGE1"
                        '    strFilter = " And BaseType='202'"
                        'Case "59-202" 'RECEIPT from production
                        '    HeaderTableName = "OIGN"
                        '    LineTableName1 = "IGN1"
                        '    strFilter = " And BaseType='202"
                        '--------------------PAYMENT----------------------
                    Case "24" 'Incoming
                        HeaderTableName = "ORCT"
                        LineTableName1 = "RCT2"
                    Case "46" 'Outgoing
                        HeaderTableName = "OVPM"
                        LineTableName1 = "VPM2"
                End Select
                'If DocEntry <= "0" Then
                '    Dim b As New SAP_Functions
                '    connect.setDB(UserID)
                '    DocEntry = b.GetMaxDocEntry(DocType, UserID, HeaderTableName, KeyName)
                'End If

                Dim ds As New DataSet("Document")
                Dim dt1 As New DataTable
                connect.setDB(UserID)
                Dim str As String
                str = "Select ROW_NUMBER() Over(Order By " + KeyName + ") No,* from " + HeaderTableName + " where " + KeyName + "='" + DocEntry + "' " + strFilter

                dt1 = connect.ObjectGetAll_Query_SAP(str).Tables(0)
                dt1.TableName = HeaderTableName
                ds.Tables.Add(dt1.Copy)

                If LineTableName1 <> "" Then
                    Dim dt2 As New DataTable
                    connect.setDB(UserID)
                    Dim str1 As String = ""

                    Select Case DocType
                        Case "24", "46"
                            KeyName = "DocNum"
                            str1 = str1 + " select ROW_NUMBER() Over(Order By T2.Transid) No,T0.DocEntry,T0.InvType, T1.RefDate,T1.DueDate,'' BalDueDeb, T0.SumApplied,DATEDIFF(DD,T1.DueDate,T2.DocDate) Overdue,T0.DocLine "
                            str1 = str1 + " from " + LineTableName1 + " T0"
                            str1 = str1 + " join JDT1 T1 on T0.InvType=case when TransType in ('24','46') then T1.ObjType else TransType end "
                            str1 = str1 + " and T0.DocEntry=case when TransType in ('24','46') then TransId else BaseRef end"
                            str1 = str1 + " join " + HeaderTableName + " T2 on T2.CardCode=T1.ShortName and T2.DocEntry=T0.DocNum"
                            str1 = str1 + " where T0.DocNum = " + DocEntry
                        Case "30"
                            str1 = "Select ROW_NUMBER() Over(Order By " + KeyName + ") No,isnull(T1.AcctName,T2.CardName) Dscription, T0.* from " + LineTableName1 + " T0 "
                            str1 = str1 + " left join OACT T1 on T0.ShortName=T1.AcctCode"
                            str1 = str1 + " left join OCRD T2 on T0.ShortName= T2.CardCode"
                            str1 = str1 + " where " + KeyName + "='" + DocEntry + "'"
                        Case "202"
                            str1 = "Select ROW_NUMBER() Over(Order By T0." + KeyName + ") No,T1.ItemName Dscription,T0.* from " + LineTableName1
                            str1 = str1 + " T0 join OITM T1 on T0.ItemCode=T1.ItemCode where T0." + KeyName + "='" + DocEntry + "'"
                        Case Else
                            str1 = "Select ROW_NUMBER() Over(Order By " + KeyName + ") No,* from " + LineTableName1 + " where " + KeyName + "='" + DocEntry + "'"
                    End Select

                    dt2 = connect.ObjectGetAll_Query_SAP(str1).Tables(0)
                    dt2.TableName = LineTableName1
                    ds.Tables.Add(dt2.Copy)
                End If

                If (LineTableName2 <> "") Then
                    Dim str2 As String = ""
                    Dim dt3 As New DataTable
                    connect.setDB(UserID)
                    str2 = "Select ROW_NUMBER() Over(Order By " + KeyName + ") No,* from " + LineTableName2 + " where " + KeyName + "='" + DocEntry + "'"
                    dt3 = connect.ObjectGetAll_Query_SAP(str2).Tables(0)
                    dt3.TableName = LineTableName2
                    ds.Tables.Add(dt3.Copy)
                End If
                Return ds
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    <WebMethod()> _
    Public Function GetLastKey(DocType As String, UserID As String) As String
        Try
            Dim sStr As String = ""
            If PublicVariable.Simulate Then
                Return ""
            Else
                Dim connect As New Connection()

                Dim HeaderTableName As String = ""
                Dim LineTableName1 As String = ""
                Dim KeyName As String = "DocEntry"
                Select Case DocType
                    Case "22"
                        HeaderTableName = "OPOR"
                        LineTableName1 = "POR1"
                    Case "19"
                        HeaderTableName = "ORPC"
                        LineTableName1 = "RPC1"
                    Case "20"
                        HeaderTableName = "OPDN"
                        LineTableName1 = "PDN1"
                    Case "21"
                        HeaderTableName = "ORPD"
                        LineTableName1 = "RPD1"
                    Case "22"
                        HeaderTableName = "OPOR"
                        LineTableName1 = "POR1"
                    Case "13"
                        HeaderTableName = "OINV"
                        LineTableName1 = "INV1"
                    Case "14"
                        HeaderTableName = "ORIN"
                        LineTableName1 = "RIN1"
                    Case "15"
                        HeaderTableName = "ODLN"
                        LineTableName1 = "DLN1"
                    Case "97" 'Sales opportunity
                        HeaderTableName = "OOPR"
                        LineTableName1 = "OPR1"
                    Case "33" 'Activity
                        HeaderTableName = "OCLG"
                        LineTableName1 = "OCLG"
                        KeyName = "ClgCode"
                    Case "221" 'Attachment
                        HeaderTableName = "OATC"
                        LineTableName1 = "ATC1"
                        KeyName = "AbsEntry"
                    Case "2" 'BP
                        HeaderTableName = "OCRD"
                        LineTableName1 = "OCRD"
                        KeyName = "CardCode"
                End Select
                Dim b As New SAP_Functions
                connect.setDB(UserID)
                Return b.GetMaxDocEntry(DocType, UserID, HeaderTableName, KeyName)

            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Function DeleteActivity(UserID As String, Key As String) As DataSet
        Dim b As New SAP_Functions
        Try
            Dim sStr As String = "Operation Completed Successfully!"
            If PublicVariable.Simulate Then
                Dim a As New Simulation
                Return a.Simulate_CreateTransaction()
            Else

                Dim oActSrv As SAPbobsCOM.ActivitiesService
                Dim oCompanyService As SAPbobsCOM.CompanyService
                Dim oAct As SAPbobsCOM.Activity
                Dim actpara As SAPbobsCOM.ActivitiesParams

                If Connection.bConnect = False Then
                    If Not connect.connectDB(UserID) Then
                        Return b.ReturnMessage(-1, "Connect SAP failed")
                    End If
                End If

                oCompanyService = PublicVariable.oCompany.GetCompanyService
                oActSrv = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ActivitiesService)
                oAct = oActSrv.GetDataInterface(SAPbobsCOM.ActivitiesServiceDataInterfaces.asActivity)
                actpara = oActSrv.GetDataInterface(SAPbobsCOM.ActivitiesServiceDataInterfaces.asActivitiesParams)

                actpara.Add()
                actpara.Item(0).ActivityCode = Key
                oAct = oActSrv.GetActivity(actpara)
                'oActSrv.DeleteActivity(actpara)

                Return b.ReturnMessage(oAct.ActivityCode, "Operation Sucessful!")
                'If lErrCode <> 0 Then
                '    PublicVariable.oCompany.GetLastError(lErrCode, sErrMsg)
                '    Return b.ReturnMessage(lErrCode, sErrMsg)
                'Else
                '    Return b.ReturnMessage(lErrCode, "Operation Sucessful!")
                'End If
            End If

        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString)
        End Try
    End Function

    <WebMethod()> _
    Public Function GetDocumentForPayment(CardCode As String, DocDate As Date, UserID As String) As DataSet
        Try
            Dim dt As New DataSet("OCRD")
            Dim str As String
            str = "select ROW_NUMBER() Over(Order By Transid) No, "
            str = str + " case when TransType in ('24','46') then TransId else BaseRef end DocEntry,"
            str = str + " case when TransType in ('24','46') then T0.ObjType else TransType end InvType,RefDate,DueDate,"
            str = str + " RefDate,DueDate,DATEDIFF(dd,duedate,'" + DocDate.ToString("MM/dd/yyyy") + "') Overdue ,Line_ID DocLine,"
            str = str + " case when T1.CardType='S' then T0.BalDueCred-T0.BalDueDeb else T0.BalDueDeb-T0.BalDueCred  End BalDueDeb, "
            str = str + " case when T1.CardType='S' then T0.BalDueCred-T0.BalDueDeb else T0.BalDueDeb-T0.BalDueCred  End SumApplied"
            str = str + " from jdt1 T0 "
            str = str + " join ocrd T1 on cardcode=shortname"
            str = str + " where shortname='" + CardCode + "' "
            str = str + " and DATEDIFF(dd, DueDate,'" + DocDate.ToString("MM/dd/yyyy") + "')>=0 and (BalDueDeb<>0 or BalDueCred<>0)"
            connect.setDB(UserID)
            dt = connect.ObjectGetAll_Query_SAP(str)
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CreateJVTimeEntry(ByVal strXml As String, UserID As String, DocType As String, Key As String, IsUpdate As Boolean) As DataSet
        Dim b As New SAP_Functions
        Try
            Dim sStr As String = ""

            Dim oDocment As SAPbobsCOM.IJournalVouchers


            Dim constr As String = connect.connectDB(UserID)

            If constr <> "" Then
                Return b.ReturnMessage(-1, "Connect SAP failed")
            End If

            PublicVariable.oCompany.XMLAsString = True
            oDocment = PublicVariable.oCompany.GetBusinessObjectFromXML(strXml, 0)
            
            lErrCode = oDocment.Add()

            If lErrCode <> 0 Then
                PublicVariable.oCompany.GetLastError(lErrCode, sErrMsg)
                Return b.ReturnMessage(lErrCode, sErrMsg)
            Else
                Return b.ReturnMessage(lErrCode, "Operation Sucessful!")
            End If
        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString)
        End Try
    End Function
End Class