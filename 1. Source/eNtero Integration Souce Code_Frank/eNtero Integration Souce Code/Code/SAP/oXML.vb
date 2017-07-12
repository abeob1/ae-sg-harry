Imports System.Text
Imports System.Xml

Public Class oXML
    Public Function ToXMLStringFromDS(ObjType As String, ds As DataSet) As String
        Try
            'Dim gf As New GeneralFunctions()
            Dim XmlString As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(XmlString)
            writer.WriteStartDocument()
            If True Then
                writer.WriteStartElement("BOM")
                If True Then
                    writer.WriteStartElement("BO")
                    If True Then
                        '#Region "write ADMINFO_ELEMENT"
                        writer.WriteStartElement("AdmInfo")
                        If True Then
                            writer.WriteStartElement("Object")
                            If True Then
                                writer.WriteString(ObjType)
                            End If
                            writer.WriteEndElement()
                        End If
                        writer.WriteEndElement()
                        '#End Region

                        '#Region "Header&Line XML"
                        For Each dt As DataTable In ds.Tables
                            If dt.Rows.Count > 0 Then
                                writer.WriteStartElement(dt.TableName.ToString())
                                If True Then
                                    For Each row As DataRow In dt.Rows
                                        writer.WriteStartElement("row")
                                        If True Then
                                            For Each column As DataColumn In dt.Columns
                                                If column.DefaultValue.ToString() <> "xx_remove_xx" Then
                                                    If row(column).ToString() <> "" Then
                                                        writer.WriteStartElement(column.ColumnName)
                                                        'Write Tag
                                                        If True Then
                                                            writer.WriteString(row(column).ToString())
                                                        End If
                                                        writer.WriteEndElement()
                                                    End If
                                                End If
                                            Next
                                        End If
                                        writer.WriteEndElement()
                                    Next
                                End If
                                writer.WriteEndElement()
                            End If
                        Next
                        '#End Region
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndDocument()

            writer.Flush()

            Return XmlString.ToString()
        Catch ex As Exception
            Return ex.ToString()
        End Try
    End Function
    
    Public Function CreateMarketingDocument(ByVal Key As String, ByVal strXml As String, ByVal DocType As String) As String
        Try
            Dim sStr As String = ""
            Dim lErrCode As Integer
            Dim sErrMsg As String
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
                Case Else
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
            End Select

            sErrMsg = Functions.SystemInitial
            If sErrMsg <> "" Then
                Return sErrMsg
            End If

            PublicVariable.oCompanyInfo.XMLAsString = True
            oDocment = PublicVariable.oCompanyInfo.GetBusinessObjectFromXML(strXml, 0)
            If Key <> "" Then
                If oDocment.getbyKey(Key) = True Then
                    lErrCode = oDocment.Update()
                Else
                    lErrCode = oDocment.add()
                End If
            Else
                lErrCode = oDocment.add()
            End If


            If lErrCode <> 0 Then
                PublicVariable.oCompanyInfo.GetLastError(lErrCode, sErrMsg)
                Return sErrMsg
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function
    'Public Sub SetDB()
    '    Try
    '        Dim MyArr As Array

    '        MyArr = PublicVariable.SAPConnectionString.Split(";")

    '        If IsNothing(PublicVariable.oCompanyInfo) Then
    '            PublicVariable.oCompanyInfo = New SAPbobsCOM.Company
    '        End If
    '        PublicVariable.oCompanyInfo.CompanyDB = MyArr(0).ToString()
    '        PublicVariable.oCompanyInfo.UserName = MyArr(1).ToString()
    '        PublicVariable.oCompanyInfo.Password = MyArr(2).ToString()
    '        PublicVariable.oCompanyInfo.Server = MyArr(3).ToString()
    '        PublicVariable.oCompanyInfo.DbUserName = MyArr(4).ToString()
    '        PublicVariable.oCompanyInfo.DbPassword = MyArr(5).ToString()
    '        PublicVariable.oCompanyInfo.LicenseServer = MyArr(6)
    '        Dim SQLType As String = MyArr(7)
    '        If SQLType = 2008 Then
    '            PublicVariable.oCompanyInfo.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
    '        Else
    '            PublicVariable.oCompanyInfo.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005
    '        End If

    '    Catch ex As Exception
    '        Functions.WriteLog(ex.ToString)
    '    End Try
    'End Sub
    'Public Function ConnectSAPDB() As String
    '    Dim lRetCode As Integer
    '    Dim lErrCode As Integer
    '    Dim sErrMsg As String
    '    Try
    '        SetDB()
    '        lRetCode = PublicVariable.oCompanyInfo.Connect
    '        If lRetCode <> 0 Then
    '            PublicVariable.oCompanyInfo.GetLastError(lErrCode, sErrMsg)
    '            Functions.WriteLog(sErrMsg)
    '            Return sErrMsg
    '        Else
    '            Return ""
    '        End If
    '    Catch ex As Exception
    '        Functions.WriteLog(sErrMsg)
    '        Return sErrMsg
    '    End Try
    'End Function

    Public Function Create_Update_BPInfo(dr As DataRow) As String
        Try
            Dim sStr As String = ""
            Dim lErrCode As Integer
            Dim sErrMsg As String
            Dim oDocment As SAPbobsCOM.BusinessPartners

            sErrMsg = Functions.SystemInitial
            If sErrMsg <> "" Then
                Return sErrMsg
            End If
            oDocment = PublicVariable.oCompanyInfo.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners)

            Dim isUpdate As Boolean
            isUpdate = oDocment.GetByKey(dr("CardCode"))

            oDocment.CardCode = dr("CardCode")
            oDocment.CardName = dr("CardName")
            oDocment.GroupCode = dr("GroupCode")
            oDocment.Phone1 = dr("Phone1")
            oDocment.Phone2 = dr("Phone2")
            oDocment.Cellular = dr("Mobile")
            oDocment.Fax = dr("Fax")
            oDocment.EmailAddress = dr("E_Mail")
            oDocment.Address = dr("Address")
            oDocment.Block = dr("Block")
            oDocment.City = dr("City")
            oDocment.ZipCode = dr("ZipCode")
            oDocment.Country = dr("Country")
            oDocment.AdditionalID = dr("IDNo")
            oDocment.VatIDNum = dr("IDType")

            If oDocment.Addresses.Count = 0 Then
                oDocment.Addresses.Add()
            End If

            oDocment.Addresses.SetCurrentLine(0)
            oDocment.Addresses.AddressName = dr("Address")
            oDocment.Addresses.Block = dr("Block")
            oDocment.Addresses.City = dr("City")
            oDocment.Addresses.ZipCode = dr("ZipCode")
            oDocment.Addresses.Country = dr("Country")

            If oDocment.ContactEmployees.Count = 0 Then
                oDocment.ContactEmployees.Add()
            End If
            oDocment.ContactEmployees.SetCurrentLine(0)
            oDocment.ContactEmployees.Title = dr("Title")
            oDocment.ContactEmployees.FirstName = dr("FirstName")
            oDocment.ContactEmployees.MiddleName = dr("MiddleName")
            oDocment.ContactEmployees.LastName = dr("LastName")
            oDocment.ContactEmployees.DateOfBirth = CDate(dr("DOB"))
            oDocment.ContactEmployees.Gender = ConvertGender(dr("Gender"))
            oDocment.ContactEmployees.PlaceOfBirth = dr("Nationality")
            oDocment.ContactEmployees.E_Mail = dr("E_Mail")
            oDocment.ContactEmployees.MobilePhone = dr("Mobile")
            oDocment.ContactEmployees.Fax = dr("Fax")
            oDocment.ContactEmployees.Address = dr("Address")
            oDocment.ContactEmployees.Name = dr("CardName")
            oDocment.ContactEmployees.Phone1 = dr("Phone1")
            oDocment.ContactEmployees.Phone2 = dr("Phone2")

            If isUpdate Then
                lErrCode = oDocment.Update()
            Else
                lErrCode = oDocment.Add()
            End If

            If lErrCode <> 0 Then
                PublicVariable.oCompanyInfo.GetLastError(lErrCode, sErrMsg)
                Return sErrMsg
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ex.ToString
        End Try
    End Function

    Private Function ConvertGender(s As String) As SAPbobsCOM.BoGenderTypes
        Select Case s
            Case "F"
                Return SAPbobsCOM.BoGenderTypes.gt_Female
            Case "M"
                Return SAPbobsCOM.BoGenderTypes.gt_Male
            Case Else
                Return SAPbobsCOM.BoGenderTypes.gt_Undefined
        End Select
    End Function
End Class
