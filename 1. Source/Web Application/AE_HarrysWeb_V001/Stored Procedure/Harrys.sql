
/****** Object:  StoredProcedure [dbo].[AE_SP023_TransferRequest_ItemList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec AE_SP023_TransferRequest_ItemList '100000001'

CREATE PROCEDURE [dbo].[AE_SP023_TransferRequest_ItemList]

@ReqNum int
As
Begin

select T0.DocNum [RequestNo], ItemCode,Dscription [ItemName]  ,T1.OpenQty ,T1.LineNum ,T1.WhsCode 
from OWTQ T0 with (nolock)
Left join WTQ1 T1 with (nolock) on T0.DocEntry =T1.DocEntry 
where T0.DocNum =@ReqNum and T1.LineStatus ='O'
End




--
GO
/****** Object:  StoredProcedure [dbo].[AE_SP022_OpenTRList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC AE_SP022_OpenTRList '11APK'

CREATE procedure [dbo].[AE_SP022_OpenTRList]

@Whscode varchar(40)
As
Begin

select distinct T0.DocNum [RequestNo],T0.DocDate [Date] ,T2.WhsCode [ToOutlet],T1.U_ITR2 [SYS2RequestNo] 

from OWTQ T0 with (nolock)
left join OPRQ T1 with (nolock) on  T0.U_PR_No =T1.DocNum 
left join PRQ1 T2 with (nolock)on T1.DocEntry =T2.DocEntry 
left join NNM1 T3 with (nolock)on T0.Series =T3.Series 
where T0.DocStatus ='O' and 
T3.SeriesName ='SYS1' and T3.ObjectCode ='1250000001' and T2.WhsCode =@Whscode 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP021_CheckingPendingApproval]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP021_CheckingPendingApproval]
 
 @DraftKey Varchar(30)
 
 As
 
 Begin
 
  select (CASE when T0.U_ApprovalLevel =1 and T0.U_L1ApprovalStatus ='Approved' then 0 
 when T0.U_ApprovalLevel =2 and(  U_L1ApprovalStatus ='Approved' and U_L2ApprovalStatus ='Approved' )  then 0 
 when T0.U_ApprovalLevel =0 then 0 else 1 end)  [LineStatus]
 INTO #TEMP from DRF1 T0 with (nolock)
 where T0.ObjType in('1470000113','540000006') and T0.DocEntry =@DraftKey 
  
 select COUNT(*) from #TEMP  where LineStatus =1
 drop table #TEMP 
 
 End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP020_GetSalesTakeCountList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procEDURE [dbo].[AE_SP020_GetSalesTakeCountList]  
@Outlet Varchar(40)  
  
As  
  
Begin  
  
select Distinct T1.WhsCode , T0.DocDate,U_StockTakeStatus,U_CreatedBy  As CreatedBy, T2.WhsName , T0.U_ApprovedDate,T0.U_ApprovedBy  
from ODRF T0  with (nolock)
INNER JOIN DRF1  T1 with (nolock) on T0.DocEntry =T1.DocEntry   
INNER JOIN OWHS T2 with (noLOCK) ON T2.WhsCode = T1.WhsCode

  
where T0.ObjType ='15' and ISNULL (CANCELED ,'N')<>'Y' and DocStatus ='O' and T0.CardCode ='STOCKTAKE' 
and T1.WhsCode =@Outlet   
  
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP019_GetItemsToDODraft]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP019_GetItemsToDODraft] 

@sUserRole as Varchar(40)

As
Begin

select T0.ItemCode from OITM T0 with (nolock)
INNER JOIN OITB T1 with (nolock)on T0.ItmsGrpCod =T1.ItmsGrpCod 
where T0.TreeType IN('S','N') and isnull(T0.InvntItem,'N') ='Y' and frozenFor='N' and T1.U_GroupType =@sUserRole

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP018_StockTakeCounting]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP018_StockTakeCounting] '11BQY','Kitchen','Draft'

CREATE PROCEDURE [dbo].[AE_SP018_StockTakeCounting]

@WhsCode Varchar(30)
,@UserRole varchar(30)
,@Status varchar(50)


As
Begin

if @Status ='' set @Status ='%'

select ItemCode,SUM(Quantity) [Quantity] INTO #TEMP2 from RDR1 with (nolock) where WhsCode =@WhsCode and LineStatus ='O' group by ItemCode ;

select T0.DocEntry ,T0.ItemCode ,T1.DocDate ,T0.Dscription,T0.LineNum ,IsNull(T3.OnHand,0) [In Stock] ,T3.IsCommited ,isnull(T2.U_StockTakeConv ,0)[StockTakeConv]
,CASE when T0.TranType ='S' then T2.SalUnitMsr when T0.TreeType ='N' then T2.U_StockTakeUoM End [Stocktake UoM]
,IsNull(T4.Quantity,0) [Event Order Commitment]
,T0.Quantity [Counted],T0.U_Variance [Variance],T0.U_AdjustInvUOM [Adjust In InventoryUOM],T5.WhsName  

INTO #TEMP

from DRF1 T0 with (nolock)
INNER JOIN ODRF T1 with (nolock)on T0.DocEntry =T1.DocEntry 
INNER JOIN OITM T2 with (nolock)on T0.ItemCode =T2.ItemCode 
INNER JOIN OITW T3 with (nolock)on T3.ItemCode =T0.ItemCode and T3.WhsCode =T0.WhsCode 
LEFT JOIN #TEMP2 T4 WITH (NOLOCK) ON T4.ItemCode =T0.ItemCode 
INNER JOIN OWHS T5 with (noLOCK) ON T5.WhsCode = T0.WhsCode
where T0.ObjType ='15' and T1.DocStatus ='O' and ISNULL (T1.CANCELED ,'N')<>'Y' and T1.CardCode ='STOCKTAKE'
 and T0.TreeType <>'I' and T1.DocType ='I' and T1.U_UserRole=@UserRole  and T0.WhsCode =@WhsCode  and T1.U_StockTakeStatus in(@Status )
 order by ISNULL( T2.QryGroup1 ,'N'),T2.InvntItem 

select T0.DocEntry ,T0.ItemCode ,T0.Dscription ,T0.LineNum,T0.Counted ,T0.Variance ,T0.[Adjust In InventoryUOM] 
,IsNull(CASE when T0.StockTakeConv>0 then ((T0.[In Stock] -(T0.IsCommited + T0.[Event Order Commitment] ))/T0.StockTakeConv) 
else (T0.[In Stock] -(T0.IsCommited + T0.[Event Order Commitment] )) End,0) [In Stock]
,T0.[Stocktake UoM] ,IsNull(T0.[Event Order Commitment],0) [Event Order Commitment] ,T0.StockTakeConv,T0.DocDate,T0.WhsName
 from #TEMP T0 with (nolock)
 
 DROP TABLE #TEMP 
 DROP TABLE #TEMP2 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP017_GetReasonDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP017_GetReasonDetails]

As
Begin

SELECT T0.Code, T0.Name FROM [dbo].[@REASONCODE]  T0 with (nolock)

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP016_GetItemList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP016_GetItemList]

As
Begin

select ItemCode,ItemName,frozenFor  from OITM with (nolock) where ISNULL( frozenFor,'N') ='N'


End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP015_GetInventoryRequestListDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP015_GetInventoryRequestListDetails]

@WhsCode varchar(20),
@AccessType varchar(20),
@VendorCode Varchar(40)


AS
Begin
select T0.DocEntry ,T0.ItemCode,T0.Dscription,T0.Quantity ,T2.PicturName [ImageURL],T0.BaseType ,T0.BaseEntry ,T0.BaseLine   
from WTQ1 T0 with (nolock)
INNER JOIN OWTQ T1 with (nolock)ON T1.DocEntry =T0.DocEntry 
INNER JOIN OITM T2 with (nolock)on T2.ItemCode =T0.ItemCode 
INNER JOIN OITB T3 with (nolock)on T3.ItmsGrpCod =T2.ItmsGrpCod 
INNER JOIN NNM1 T4 with (nolock)ON T4.Series =T1.Series 
where LineStatus  ='O' and T4.SeriesName ='SYS1' and T1.CardCode =@VendorCode and T0.WhsCode =@WhsCode and T3.U_GroupType =@AccessType 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP014_Get_POListDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP014_Get_POListDetails]

@WhsCode varchar(20),
@AccessType varchar(20),
@VendorName Varchar(40)
As

Declare @ImagePath as varchar(200)
Declare @VendorCode as varchar(40)

Begin

select @ImagePath = TT.BitmapPath  from OADP TT with (nolock)
select @VendorCode  = TT.CardCode  from OCRD TT with (nolock) where CardName =@VendorName  

if UPPER (@VendorCode) <>'VA-HARCT' 

select T1.DocNum ,T0.DocEntry,T0.VisOrder [LineNum] ,T0.ItemCode,T0.Dscription--,T0.Quantity
,T0.OpenQty [Quantity],@ImagePath + T2.PicturName [ImageURL],T0.BaseType ,T0.BaseEntry ,T0.BaseLine 
from POR1 T0 with (nolock)
INNER JOIN OPOR T1 with (nolock) ON T1.DocEntry =T0.DocEntry 
INNER JOIN OITM T2 with (nolock)on T2.ItemCode =T0.ItemCode 
INNER JOIN OITB T3 with (nolock)on T3.ItmsGrpCod =T2.ItmsGrpCod 
where LineStatus  ='O' and T1.CardCode  =@VendorCode and T0.WhsCode =@WhsCode and T3.U_GroupType =@AccessType 

union all

select 0[DocNum],0[DocEntry],0 [LineNum],T0.ItemCode ,T0.ItemName ,0 [Quantity],@ImagePath + PicturName [ImageURL],0 [BaseType], 0 [BaseEntry] ,0 [BaseLine]  
from OITM T0 with (nolock)
inner join OCRD T1 with (nolock) on T0.CardCode =T1.CardCode 
inner join OITW T2 with (nolock)on T2.ItemCode =T0.ItemCode 
where T0.QryGroup16 ='Y' and T0.CardCode =@VendorCode and T2.WhsCode =@WhsCode 

else

select T1.DocNum , T0.DocEntry,T0.VisOrder [LineNum] ,T0.ItemCode,T0.Dscription--,T0.Quantity 
,T0.OpenQty [Quantity] ,@ImagePath + T2.PicturName [ImageURL] ,T0.BaseType ,T0.BaseEntry ,T0.BaseLine  
from WTQ1 T0 with (nolock)
INNER JOIN OWTQ T1 with (nolock)ON T1.DocEntry =T0.DocEntry 
INNER JOIN OITM T2 with (nolock)on T2.ItemCode =T0.ItemCode 
INNER JOIN OITB T3 with (nolock)on T3.ItmsGrpCod =T2.ItmsGrpCod 
INNER JOIN NNM1 T4 with (nolock)ON T4.Series =T1.Series 
where LineStatus  ='O' and T4.SeriesName  ='SYS2' and T1.CardName =@VendorName and T0.WhsCode =@WhsCode and T3.U_GroupType =@AccessType 


End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP014_Get_Open_POListDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP014_Get_Open_POListDetails]

@WhsCode varchar(20),
@AccessType varchar(20),
@VendorCode Varchar(40)


AS
Begin
select T0.DocEntry ,T0.ItemCode,T0.Dscription,T0.Quantity ,''[ImageURL],T0.BaseType ,T0.BaseEntry ,T0.BaseLine  INTO #GetPODet
from POR1 T0 with (nolock)
INNER JOIN OPOR T1 with (nolock)ON T1.DocEntry =T0.DocEntry 
INNER JOIN OITM T2 with (nolock)on T2.ItemCode =T0.ItemCode 
INNER JOIN OITB T3 with (nolock)on T3.ItmsGrpCod =T2.ItmsGrpCod 
where LineStatus  ='O' and T1.CardCode =@VendorCode and T0.WhsCode =@WhsCode and T3.U_GroupType =@AccessType 

select * from #GetPODet

drop table #GetPODet

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP013_GetPOList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP013_GetPOList] '','I','Kitchen','11TPG'

CREATE PROCEDURE [dbo].[AE_SP013_GetPOList]  
  
@VendorName varchar(40),  
@SupplierType char(10),
@AccessType varchar(40),
@WhsCode varchar(40)

As  
Begin  
  
if UPPER ( @SupplierType) <>'I'  
begin
select T0.CardCode ,T0.CardName,COUNT(T0.DocEntry)[NoOfOpenPO]   
from OPOR T0 with (nolock)
INNER JOIN POR1 T1 with (nolock)ON T0.DocEntry =T1.DocEntry 
INNER JOIN OITM T2 with (nolock)ON T1.ItemCode =T2.ItemCode
INNER JOIN OITB T3 with (nolock)ON T3.ItmsGrpCod =T2.ItmsGrpCod 
where DocStatus ='O' and CardName like @VendorName   AND T1.WhsCode =@WhsCode AND T3.U_GroupType =@AccessType
  
group by T0.CardCode ,T0.CardName  

union all

select T1.CardCode ,T1.CardName ,0 [No of PO's] from OITM T0 with (nolock)
inner join OCRD T1 with (nolock)on T0.CardCode =T1.CardCode 
inner join OITB T2 with (nolock) on T2.ItmsGrpCod =T0.ItmsGrpCod 
where T0.QryGroup16 ='Y' and CardName like @VendorName AND T2.U_GroupType =@AccessType
end
    
else  
  begin
  select T0.CardCode ,T0.CardName,COUNT(T0.DocEntry)[NoOfOpenPO]   

into #TEMP

from OWTQ T0 with (nolock)

INNER JOIN WTQ1 T1 with (nolock)ON T0.DocEntry =T1.DocEntry 
INNER JOIN OITM T2 with (nolock)ON T1.ItemCode =T2.ItemCode
INNER JOIN OITB T3 with (nolock)ON T3.ItmsGrpCod =T2.ItmsGrpCod 
INNER JOIN NNM1 T4 with (nolock)ON T4.Series =T0.Series 
where DocStatus ='O' and T4.SeriesName ='SYS2' and T0.CardCode = 'VA-HARCT' AND T1.WhsCode =@WhsCode AND T3.U_GroupType =@AccessType
  
group by T0.CardCode ,T0.CardName,T0.DocEntry 

select T0.CardCode ,T0.CardName,COUNT(T0.CardCode)[NoOfOpenPO] from #TEMP T0
group by T0.CardCode ,T0.CardName 

drop table #TEMP 
  end
  

 
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP013_GetInventoryRequestList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP013_GetInventoryRequestList]

@WhsCode varchar(10)

As
Begin

select CardCode ,CardName,COUNT(DocEntry)[NoOfOpenPO] INTO #GetInvReqList from OWTQ with (nolock)
INNER JOIN NNM1 T1 with (nolock)ON T1.Series =OWTQ.Series 
where DocStatus ='O' and T1.SeriesName  ='SYS1' and CardCode = 'VA-HARCT'

group by CardCode ,CardName

select * from #GetInvReqList 

Drop Table #GetInvReqList 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP012_Get_Open_POList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec AE_SP012_Get_Open_POList 'VT%'

CREATE PROCEDURE [dbo].[AE_SP012_Get_Open_POList]

@VendorName varchar(40), @WhsCode varchar(10)

As
Begin

select CardCode ,CardName,COUNT(DocEntry)[NoOfOpenPO] from OPOR with (nolock)
where DocStatus ='O' and CardName like @VendorName

group by CardCode,CardName 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP011_GetInventoryReqDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--ExEC AE_SP011_GetInventoryReqDetails '3'

CREATE PROCEDURE [dbo].[AE_SP011_GetInventoryReqDetails]

@DocEntry varchar(20)
AS
Begin
select T0.DocEntry ,T0.ItemCode,T0.Dscription,T0.OpenQty,''[BatchNum] ,T0.BaseEntry ,T0.BaseLine   
from WTQ1 T0 with (nolock)

where LineStatus  ='O' and DocEntry  =@DocEntry 
 
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP010_GetInventoryRequest]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP010_GetInventoryRequest]

@WhsCode varchar(40)

As
Begin

select DocEntry ,DocDate ,ToWhsCode ,DocNum,DocStatus  
from OWTQ with (nolock)

INNER JOIN NNM1 T1 with (nolock)ON T1.Series =OWTQ.Series 
where DocStatus ='O' and T1.SeriesName  ='SYS1' and ToWhsCode =@WhsCode 
 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP001_UserInformation]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP001_UserInformation] 'manager'

CREATE PROCEDURE [dbo].[AE_SP001_UserInformation]

@UserCode varchar(40)

As
Begin 
 

select T1.EmpID ,ISNULL (T1.firstName ,'')+' '+ISNULL (T1.middleName,'')+' '+ ISNULL (T1.lastName ,'') [EmployeeName]
,T0.U_Outlet WhsCode ,T2.WhsName , case when T0.SUPERUSER = 'Y' then 'ALL' else T0.U_UserRole  end as [U_AE_Access]
,T0.SUPERUSER,T0.U_UserRole,T0.U_ApprovalLevel 
,(select CASE when ISNULL ( PrintHeadr,'')='' then CompnyName else PrintHeadr end   from OADM with (nolock)) [CompanyName]

 

from OUSR T0 with (nolock)
join OHEM T1 with (nolock)on T0.USERID =T1.userId 
join OWHS T2 with (nolock)on  (CASE when ISNULL( T0.U_Outlet,'') <>'' then  T0.U_Outlet end) =T2 .WhsCode or
(CASE when ISNULL( T0.U_Outlet,'') ='' then  1 end ) =1
where T0.USER_CODE =@UserCode


End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP001_GetOutlets]    Script Date: 01/09/2015 17:59:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP001_GetOutlets]
As
Begin
select ' --- Select Outlet --- '[WhsCode],' --- Select Outlet --- '[WhsName]  

union all

select WhsCode ,WhsName    from OWHS with  (nolock)
 
END
GO
/****** Object:  StoredProcedure [dbo].[AE_SP004_MaterialReqByBar]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Exec AE_SP004_MaterialReqByBar 'VA-HARCB','01CKT'

CREATE PROCEDURE [dbo].[AE_SP004_MaterialReqByBar]

@VendorCode Varchar(40)
,@WhsCode varchar (40)

As
Begin 

SELECT T0.[ItemCode], T0.[ItemName], T0.[OnHand], T0.[MinLevel], T0.[OnOrder], T0.[INUoMEntry] 
INTO #MatReqByBar
FROM OITM T0  with (nolock)
INNER JOIN ITM2 T1 with (nolock)ON T0.[ItemCode] = T1.[ItemCode] 
INNER JOIN OITW T2 with (nolock)ON T0.[ItemCode] = T2.[ItemCode] 
WHERE T1.[VendorCode] =@VendorCode and  T2.[WhsCode] =@WhsCode

select * from #MatReqByBar with (nolock)

delete from #MatReqByBar 

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP003_GetSuppliers]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP003_GetSuppliers]

@UserRole Varchar(20)


As
Begin

select ' --- Select Supplier --- '[CardCode],' --- Select Supplier --- '[CardName]

union all

select  T2.CardCode , T2.CardName
from OITM T0 with (nolock)
left join OITB T1 with (nolock)on T0.ItmsGrpCod=T1.ItmsGrpCod
left join OCRD T2 with (nolock)on T0.cardcode = T2.CardCode
left join ITM1 T3 with (nolock)on T0.ItemCode = T3.ItemCode and T3.PriceList=13
INNER JOIN ITM2 T4 with (nolock)ON T0.CardCode =T4.VendorCode AND T0.ItemCode =T4.ItemCode 
where T0.CardCode is not null and T1.U_GroupType is not null and T2.CardType ='S' 
and T1.U_GroupType=@UserRole AND ISNULL( T2.frozenFor ,'N')='N' AND IsNull(T0.frozenFor,'N') = 'N'
group by T2.CardCode ,T2.CardName 
--and (isnull(T3.Price,0) > 0 or (isnull(T3.Price,0) = 0 and isnull(T2.U_OutletMinSpend,0) = 0))
order by CardName

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP008_PurchaseRequest]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--AE_SP008_PurchaseRequest '01CKT','Bar'  
  
CREATE procedure [dbo].[AE_SP008_PurchaseRequest]       
        
@WhsCode as varchar(30)      
,@AccessType as varchar(30) 
,@Status as int
,@FromDate as varchar(30)
,@ToDate as varchar(30)   
        
AS        
Begin        
    
  
SELECT T0.[DocEntry],'PR' + Cast(T0.[DocNum] AS Varchar) [PRNo], T1.WhsCode [Outlet], T0.[DocDate] [OrderDate]        
,isnull((CASE WHEN T0.[U_Urgent] = 'Y' THEN 'YES' WHEN isnull(T0.[U_Urgent],'N') = 'N' THEN 'NO' END),'NO') as [Urgent] ,    
 MAX(T0.[DocTotal]) [TotalSpend],T0.[DocStatus]  
 ,CASE when T1.U_ApprovalLevel =0 then 'Approval Not Required' else 'Approved' END [Status]  
   
,ISNULL (T3.[firstName] ,'')+' '+ISNULL (T3.[middleName] ,'')+' '+ISNULL (T3.[lastName] ,'') [UserName]   
 into #TEMPPR  
FROM OPRQ T0   with (nolock)     
INNER JOIN PRQ1 T1 WITH (NOLOCK) ON T1.DocEntry =T0.DocEntry   
INNER JOIN OUSR T2 with (nolock)ON T0.[UserSign] = T2.[USERID]   
INNER JOIN OHEM T3 with (nolock)ON T0.[OwnerCode] = T3.[empID]   
INNER JOIN OITM T4 WITH (NOLOCK) ON T4.ItemCode =T1.ItemCode   
INNER JOIN OITB T5 WITH (NOLOCK) ON T5.ItmsGrpCod =T4.ItmsGrpCod         
WHERE ISNULL(T0.[CANCELED],'N') <>'Y' AND T1.WhsCode =@WhsCode  AND T5.U_GroupType =@AccessType   
  
Group By T0.DocEntry ,T0.DocNum ,T1.WhsCode ,T0.DocDate ,T0.U_Urgent ,T0.DocStatus ,T1.U_ApprovalLevel  
,T3.[firstName],T3.[middleName],T3.[lastName]  
  
  
SELECT T0.[DocEntry],'PQ' +  Cast(T0.[DocNum] AS Varchar) [PRNo], @WhsCode  [Outlet], T0.[DocDate] [OrderDate]        
,(CASE WHEN T0.[U_Urgent] = 'Y' THEN 'YES' WHEN T0.[U_Urgent] = 'N' THEN 'NO' END) as [Urgent] ,    
 T0.[DocTotal] [TotalSpend],T0.[DocStatus],  
   
 'Approved'[Status]       
          
,ISNULL (T3.[firstName] ,'')+' '+ISNULL (T3.[middleName] ,'')+' '+ISNULL (T3.[lastName] ,'') [UserName]   
INTO #TEMPPQ   
FROM OPQT T0  with (nolock)       
    
INNER JOIN OUSR T2 with (nolock)ON T0.[UserSign] = T2.[USERID]         
INNER JOIN OHEM T3 with (nolock)ON T0.[OwnerCode] = T3.[empID]         
WHERE ISNULL(T0.[CANCELED],'N') <>'Y' and T0.DocEntry in (select TT0.DocEntry   
from PQT1 TT0 with (nolock) INNER JOIN OITM TT1 with (nolock)ON TT0.ItemCode =TT1.ItemCode   
INNER JOIN OITB TT2 with (nolock)ON TT2.ItmsGrpCod =TT1.ItmsGrpCod   
where TT0.WhsCode  =@WhsCode AND TT2.U_GroupType =@AccessType and BaseType = '1470000113' )  
  
If(@Status = -1 AND (@FromDate = '' AND @ToDate = ''))
BEGIN
	SELECT * FROM #TEMPPR with (nolock)   
	union all  
	SELECT * FROM #TEMPPQ with (nolock)  ORDER BY OrderDate DESC  
END
ELSE IF(@Status = -1 AND (@FromDate != '' AND @ToDate != ''))
BEGIN
	SELECT * FROM #TEMPPR with (nolock) WHERE OrderDate BETWEEN @FromDate AND @ToDate 
	UNION ALL
	SELECT * FROM #TEMPPQ  with (nolock) WHERE OrderDate BETWEEN @FromDate AND @ToDate ORDER BY OrderDate DESC 
END
ELSE IF(@Status = 0 AND (@FromDate = '' AND @ToDate = ''))
BEGIN 
	SELECT * FROM #TEMPPR with (nolock) WHERE [Status] = 'Approved'
	UNION ALL
	SELECT * FROM #TEMPPQ with (nolock) WHERE [Status] = 'Approved' ORDER BY OrderDate DESC 
END
ELSE IF(@Status = 1 AND (@FromDate = '' AND @ToDate = ''))
BEGIN 
	SELECT * FROM #TEMPPR with (nolock) WHERE [Status] = 'Approval Not Required' 
	UNION ALL
	SELECT * FROM #TEMPPQ with (nolock) WHERE [Status] = 'Approval Not Required' ORDER BY OrderDate DESC 
END
ELSE IF(@Status = 0 AND (@FromDate != '' AND @ToDate != ''))
BEGIN 
	SELECT * FROM #TEMPPR with (nolock) WHERE [Status] = 'Approved' AND OrderDate BETWEEN @FromDate AND @ToDate 
	UNION ALL
	SELECT * FROM #TEMPPQ with (nolock) WHERE [Status] = 'Approved' AND OrderDate BETWEEN @FromDate AND @ToDate ORDER BY OrderDate DESC 
END
ELSE IF(@Status = 1 AND (@FromDate != '' AND @ToDate != ''))
BEGIN 
	SELECT * FROM #TEMPPR with (nolock) WHERE [Status] = 'Approval Not Required' AND OrderDate BETWEEN @FromDate AND @ToDate 
	UNION ALL
	SELECT * FROM #TEMPPQ with (nolock) WHERE [Status] = 'Approval Not Required' AND OrderDate BETWEEN @FromDate AND @ToDate ORDER BY OrderDate DESC 
END
          
DROP TABLE #TEMPPR  
DROP TABLE #TEMPPQ  
        
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP006_PurchaseRequestDraft]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP006_PurchaseRequestDraft]        
        
@WhsCode as varchar(30)  
--,@UserAccess as Varchar(30)        
        
AS        
Begin        
        
SELECT  T0.[DocEntry],T0.[DocNum] [DRFNo], @WhsCode [Outlet], T0.[DocDate] [OrderDate]        
,(CASE WHEN ISNULL(T0.[U_Urgent],'N') = 'Y' THEN 'YES'     
  WHEN ISNULL(T0.[U_Urgent],'N')= 'N' THEN 'NO' END) as [Urgent] ,    
 MAX(T0.[DocTotal]) AS [TotalSpend],T0.[DocStatus], 'Draft' [Status]      
,ISNULL (T3.[firstName] ,'')+' '+ISNULL (T3.[middleName] ,'')+' '+ISNULL (T3.[lastName] ,'') [UserName]     
FROM ODRF T0   with (nolock)       
INNER JOIN DRF1 T1 with (nolock)  ON T0.[DocEntry] = T1.[DocEntry]  
INNER JOIN OITM TT4 with (nolock)  ON T1.ItemCode = TT4.ItemCode   
INNER JOIN OITB TT5 with (nolock)  ON TT5.ItmsGrpCod = TT4.ItmsGrpCod   
INNER JOIN OUSR T2 with (nolock)  ON T0.[UserSign] = T2.[USERID]         
INNER JOIN OHEM T3 with (nolock)  ON T0.[OwnerCode] = T3.[empID]         
WHERE T0.ObjType in('1470000113','540000006') and T0.DocStatus ='O'  
and T1.U_ApprovalLevel IS NULL and ISNULL(T0.[CANCELED],'N') <>'Y'  AND T1.WhsCode=@WhsCode
group by T0.DocEntry,T0.DocNum,T0.DocDate,T0.U_Urgent,T0.DocStatus,t3.firstName,t3.middleName,t3.lastName
 
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP005_MaterialReqBySupplierItemList]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec [AE_SP005_MaterialReqBySupplierItemList] '01CKT'
 
CREATE PROCEDURE [dbo].[AE_SP005_MaterialReqBySupplierItemList]        
        
        
@WhsCode varchar (40)   
        
As        
Begin         
        
SELECT T1.VendorCode [SupplierCode],T3.CardName [SupplierName]        
, T0.[ItemCode], T0.[ItemName] [Description], T2.OnHand,T2.OnOrder  [AlreadyOrdered],isnull(T3.U_OutletMinSpend,0) MinSpend,t4.Price         
,-(SELECT dbo.Web_getSOComitmt (T1.ItemCode ,T2.WhsCode ,3)) [Eventorder],      
       
 --NULLIF(Convert(decimal(18,6),0),0) as [OrderQuantity],      
 convert(decimal(18,6),0) [Total], 
 -(SELECT dbo.Web_getAvgReq (T2.ItemCode ,T2.WhsCode  ,7)) as [Last7Days]   
        
,T0.[OnOrder], T0.[MinLevel],T0.BuyUnitMsr  as UOM       
    
,T3.QryGroup1 [Mon],T3.QryGroup2 [Tue],T3.QryGroup3 [Wed],T3.QryGroup4 [Thu],T3.QryGroup5 [Fri],T3.QryGroup6 [Sat],T3.QryGroup7 [Sun]        
  
        
INTO #MatReqByItem        
FROM OITM T0  with (nolock)        
INNER JOIN ITM2 T1 with (nolock)ON T0.[ItemCode] = T1.[ItemCode]    
INNER JOIN ITM1 T4 with (nolock)ON T4.[ItemCode] = T0.[ItemCode]         
INNER JOIN OITW T2 with (nolock)ON T0.[ItemCode] = T2.[ItemCode]        
INNER JOIN OCRD T3 with (nolock)ON T0.CardCode = T3.[CardCode]         
inner join OPLN T5 with (nolock)on T5.ListNum =T4.PriceList         
         
WHERE  T2.[WhsCode] =@WhsCode and isnull(T0.frozenFor,'N') ='N' --and t4.Price >0   
 
and t5.ListName='PurchasePrice'         
         
        
select (T0.OnHand  -(T0.AlreadyOrdered +T0.Eventorder )) [In stock] ,*   
INTO #MatFinal  
from #MatReqByItem T0 with (nolock) Order By SupplierName,[Description]           
        
select (T0.[In stock] +T0.Eventorder +(T0.Last7Days -T0.MinLevel ))[OrderQuantity],*   
from #MatFinal T0  with (nolock)
        
DROP TABLE #MatReqByItem          
DROP TABLE #MatFinal   
        
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP009_GetPurchaseRequestDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC [AE_SP009_GetPurchaseRequestDetails] '3'

CREATE PROCEDURE [dbo].[AE_SP009_GetPurchaseRequestDetails]    
    
@DocEntry varchar(20)    
    
AS    
Begin    
    
select T0.DocDate, T0.DocDueDate,T0.DocEntry,'Approved' as 'Status', isnull(T0.[U_Urgent],'N') as 'Urgent'
, T4.CardCode ,T4.CardName,T1.ItemCode ,T1.Dscription as ItemName,T1.WhsCode
, T5.[WhsName] , ISNull(T1.Price,0) Price 
,IsNull(T1.Quantity,0) as OrderQuantity

,(SELECT dbo.Web_getInStock (T1.ItemCode,T1.WhsCode))[OnHand] 
,(SELECT dbo.Web_getSOComitmt (T1.ItemCode ,T1.WhsCode ,3))[Eventorder]     
,(select IsCommited from OITW where ItemCode =T1.ItemCode and WhsCode =T1.WhsCode )[IsCommited]    
,(SELECT dbo.Web_getAvgReq (T1.ItemCode ,T1.WhsCode  ,7)) as Last7DaysAvg
,(SELECT dbo.Web_getPOQty (T1.ItemCode ,T1.WhsCode  )) AS AlreadyOrdered
,(SELECT dbo.Web_getMinimumStock (T1.ItemCode ,T1.WhsCode  ))[MinLevel] 

, IsNull(CONVERT(DECIMAL(10,6),(T1.Price * T1.Quantity )) ,0) as Total   
,T2.[OnOrder],T2.[BuyUnitMsr] as UOM 
,isnull(T4.U_OutletMinSpend,0) [MinSpend]
,IsNull(T0.DocTotal  ,0)DocTotal  

,T4.QryGroup1 [Mon],T4.QryGroup2 [Tue],T4.QryGroup3 [Wed],T4.QryGroup4 [Thu],T4.QryGroup5 [Fri],T4.QryGroup6 [Sat],T4.QryGroup7 [Sun]        
   
INTO #TEMP   
    
from OPRQ T0 with (nolock)   
INNER JOIN PRQ1 T1 with (nolock)on T0.DocEntry =T1.DocEntry     
INNER JOIN OITM T2 with (nolock)ON T2.ItemCode =T1.ItemCode      
INNER JOIN OCRD T4 with (nolock)on T4.CardCode =T1.LineVendor 
inner join OWHS T5 with (nolock) on T5.WhsCode =T1.WhsCode    
where  T0.DocEntry =@DocEntry       
   
select (T0.MinLevel -T0.[Eventorder] -(2*T0.Last7DaysAvg)-T0.AlreadyOrdered -T0.OnHand  )[RecommendedQuantity] ,*
from #TEMP T0 with (nolock)
      
  
drop table #TEMP    

End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP009_GetPurchaseQuotationDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP009_GetPurchaseQuotationDetails]    
   
@DocEntry varchar(20)    
    
AS    
Begin    
 select T0.DocDate, T0.DocDueDate,T0.DocEntry,'Approved' as 'Status', isnull(T0.[U_Urgent],'N') as 'Urgent'
 , T4.CardCode ,T4.CardName,T1.ItemCode , T1.Dscription as ItemName,T1.WhsCode
 , T5.WhsName  
,IsNull(T1.Price,0) Price
,IsNull(T1.Quantity,0) as OrderQuantity

,(SELECT dbo.Web_getInStock (T1.ItemCode,T1.WhsCode))[OnHand]        
,(SELECT dbo.Web_getSOComitmt (T1.ItemCode ,T1.WhsCode ,3))[OnOrder]       
,(select IsCommited from OITW where ItemCode =T1.ItemCode and WhsCode =T1.WhsCode )[IsCommited] 
,(SELECT dbo.Web_getPOQty (T1.ItemCode ,T1.WhsCode  )) AS AlreadyOrdered      
,(SELECT dbo.Web_getMinimumStock (T1.ItemCode ,T1.WhsCode  ))[MinLevel]  

, IsNull(CONVERT(DECIMAL(10,6),(T1.Price * T1.Quantity )),0)  as Total   
,T2.InvntryUom [UOM] ,isnull(T4.U_OutletMinSpend,0) [MinSpend],IsNull(T0.DocTotal,0) DocTotal
,T4.QryGroup1 [Mon],T4.QryGroup2 [Tue],T4.QryGroup3 [Wed],T4.QryGroup4 [Thu],T4.QryGroup5 [Fri],T4.QryGroup6 [Sat],T4.QryGroup7 [Sun]        
   
    
from OPQT T0 with (nolock)    
INNER JOIN PQT1 T1 with (nolock) on T0.DocEntry =T1.DocEntry 
INNER JOIN OITM T2 with (nolock)ON T2.ItemCode =T1.ItemCode      
INNER JOIN OCRD T4 with (nolock)on T4.CardCode =T1.LineVendor 
INNER JOIN OWHS T5 with (Nolock) on T5.WhsCode =T1.WhsCode   
where  T0.DocEntry =@DocEntry       
   
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP006_GetDraftDetails]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AE_SP006_GetDraftDetails]        
        
@DocEntry varchar(20)        
        
AS        
Begin        
        
select T0.DocDate, Isnull(T1.PQTReqDate,T0.DocDueDate)  as DocDueDate,T0.DocEntry,T5.U_GroupType [GroupType] ,  
      
 'Draft' [Status]    
, isnull(T0.[U_Urgent],'N') as 'Urgent', T4.CardCode 'SupplierCode' ,T4.CardName 'SupplierName',T1.ItemCode ,      
T1.Dscription as ItemName,T1.WhsCode,(select Distinct WhsName from OWHS where WhsCode =T1.WhsCode)[WhsName]    
       
,T1.Price , (case when T1.Quantity=0 then T1.PQTReqQty else T1.Quantity end ) as OrderQuantity  
  
,(SELECT dbo.Web_getInStock (T1.ItemCode,T1.WhsCode))[OnHand]        
,(SELECT dbo.Web_getSOComitmt (T1.ItemCode ,T1.WhsCode ,3))[Eventorder]       
,(select IsCommited from OITW where ItemCode =T1.ItemCode and WhsCode =T1.WhsCode )[IsCommited]      
,(SELECT dbo.Web_getAvgReq (T1.ItemCode ,T1.WhsCode  ,7)) as Last7DaysAvg  
,(SELECT dbo.Web_getPOQty (T1.ItemCode ,T1.WhsCode  )) AS AlreadyOrdered  
,(SELECT dbo.Web_getMinimumStock (T1.ItemCode ,T1.WhsCode  ))[MinLevel]   
  
       
,T2.[OnOrder],T2.[BuyUnitMsr] as UOM  
,isnull(T4.U_OutletMinSpend,0) [MinSpend]    
,ISNULL (T1.U_DeliveryCharge,'N') AS 'DelChargeUDF' 
      
,T4.QryGroup1 [Mon],T4.QryGroup2 [Tue],T4.QryGroup3 [Wed],T4.QryGroup4 [Thu],T4.QryGroup5 [Fri],T4.QryGroup6 [Sat],T4.QryGroup7 [Sun]            
  
INTO #TEMP       
      
from ODRF T0 with (nolock)   
INNER JOIN DRF1 T1 with (nolock)on T0.DocEntry =T1.DocEntry  
INNER JOIN OITM T2 with (nolock)ON T2.ItemCode =T1.ItemCode        
INNER JOIN OCRD T4 with (nolock)on T4.CardCode =Isnull(T1.LineVendor,T0.CardCode)     
INNER JOIN OITB T5 with (nolock)on T5.ItmsGrpCod =T2.ItmsGrpCod         
where T0.ObjType in('1470000113','540000006') and T0.DocEntry =@DocEntry           
and T0.DocStatus ='O' and ISNULL (T0.CANCELED ,'N' )<>'Y'        
        
        
select (T0.MinLevel -T0.[Eventorder] -(2*T0.Last7DaysAvg)-T0.AlreadyOrdered -T0.OnHand  )[RecommendedQuantity]
,(T0.Price *T0.OrderQuantity )[Total] ,* from #TEMP T0  with (nolock)
  
drop table #TEMP   
  
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP005_MaterialReqByItem]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP005_MaterialReqByItem] 'Bar','02CTS'
 
CREATE PROCEDURE [dbo].[AE_SP005_MaterialReqByItem]    
    
@GroupType Varchar(30)  
,@WhsCode varchar (40)   
    
As    
Begin     
    
SELECT T0.CardCode [SupplierCode],T3.CardName [SupplierName]      
, T0.[ItemCode], T0.[ItemName] [Description], T2.OnHand 
,isnull(T3.U_OutletMinSpend,0) MinSpend,t4.Price,Cast(0 as decimal(19,6)) [OrderQuantity],   
 
 convert(decimal(18,6),0) [Total] 
  
,(SELECT dbo.Web_getInStock (T0.ItemCode,@WhsCode))[In stock]
,(SELECT dbo.Web_getSOComitmt (T0.ItemCode ,@WhsCode,3))[Eventorder]
,(SELECT dbo.Web_getAvgReq (T0.ItemCode ,@WhsCode ,7))[Last7Days]  
,(SELECT dbo.Web_getPOQty (T0.ItemCode ,@WhsCode ))[AlreadyOrdered]
,(SELECT dbo.Web_getMinimumStock (T0.ItemCode ,@WhsCode ))[MinLevel]
    
 ,T0.[OnOrder],  T0.[BuyUnitMsr] as UOM ,'' as DeliveryDate       
,T3.QryGroup1 [Mon],T3.QryGroup2 [Tue],T3.QryGroup3 [Wed],T3.QryGroup4 [Thu],T3.QryGroup5 [Fri],T3.QryGroup6 [Sat],T3.QryGroup7 [Sun]      
      
INTO #MatReqByItem     
FROM OITM T0 with (nolock)  
   
INNER JOIN ITM1 T4 with (nolock)ON T4.[ItemCode] = T0.[ItemCode]     
INNER JOIN OITW T2 with (nolock)ON T0.[ItemCode] = T2.[ItemCode]    
 
INNER JOIN OCRD T3 with (nolock)ON T0.[CardCode] = T3.[CardCode]     
INNER JOIN OITB T6 with (nolock)ON T6.ItmsGrpCod =T0.ItmsGrpCod     
inner join OPLN T5 with (nolock)on T5.ListNum =T4.PriceList     
     
WHERE    
T5.ListName ='PurchasePrice' and isnull(T0.frozenFor,'N') ='N' --and t4.Price >0  
and isnull(T3.frozenFor,'N') = 'N' and T3.frozenFor is not null
and T0.frozenFor is not null 
AND T6.U_GroupType=@GroupType and T2.WhsCode =@WhsCode   
    
    
select  *
INTO #MatFinal
from #MatReqByItem T0  with (nolock) Order By SupplierName,[Description]         
      
select (T0.MinLevel -T0.[Eventorder] -(2*T0.[Last7Days] )-T0.AlreadyOrdered -T0.[In stock] )[RecommendedQuantity] ,*  
from #MatFinal T0 with (nolock)
      
DROP TABLE #MatReqByItem        
DROP TABLE #MatFinal 
       
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP004_MaterialReqBySupplier]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC [AE_SP004_MaterialReqBySupplier] 'VT-6DMPL','11APK','Bar'


CREATE PROCEDURE [dbo].[AE_SP004_MaterialReqBySupplier]    
    
@VendorCode Varchar(40)    
,@WhsCode varchar (40)    
,@GroupType Varchar(30)    
As    
Begin     
    
SELECT T3.CardCode 'SupplierCode',T3.CardName 'SupplierName',T0.[ItemCode],T6.U_GroupType [GroupType] ,T0.[ItemName], T2.OnHand ,T2.IsCommited 
,isnull(T3.U_OutletMinSpend,0) [MinSpend],t4.Price ,Cast(0 as decimal(19,6)) [OrderQuantity]

,(SELECT dbo.Web_getInStock (T0.ItemCode,@WhsCode))[In stock]
,(SELECT dbo.Web_getSOComitmt (T0.ItemCode ,@WhsCode,3))[Event order]
,(SELECT dbo.Web_getAvgReq (T0.ItemCode ,@WhsCode ,7))[Last 7 Days]
,(SELECT dbo.Web_getPOQty (T0.ItemCode ,@WhsCode ))[AlreadyOrdered]
,(SELECT dbo.Web_getMinimumStock (T0.ItemCode ,@WhsCode ))[MinLevel]
,T0.[OnOrder], T0.[BuyUnitMsr] as UOM      
,T3.QryGroup1 [Mon],T3.QryGroup2 [Tue],T3.QryGroup3 [Wed],T3.QryGroup4 [Thu],T3.QryGroup5 [Fri],T3.QryGroup6 [Sat],T3.QryGroup7 [Sun]    
    
INTO #MatReqByBar    
FROM OITM T0  with (nolock)    
--INNER JOIN ITM2 T1 ON T0.[ItemCode] = T1.[ItemCode]     
INNER JOIN ITM1 T4 with (nolock)ON T4.[ItemCode] = T0.[ItemCode]     
INNER JOIN OITW T2 with (nolock)ON T0.[ItemCode] = T2.[ItemCode]    
INNER JOIN OCRD T3 with (nolock)ON T0.CardCode  = T3.[CardCode]     
INNER JOIN OITB T6 with (nolock)ON T6.ItmsGrpCod =T0.ItmsGrpCod     
inner join OPLN T5 with (nolock)on T5.ListNum =T4.PriceList     
     
WHERE T3.CardCode =@VendorCode and  T2.[WhsCode] =@WhsCode and T5.ListName ='PurchasePrice' 
and isnull(T0.frozenFor,'N') ='N'  and T0.frozenFor is not null
and isnull(T3.frozenFor,'N') ='N' and T3.frozenFor is not null
--and t4.Price >0 
AND T6.U_GroupType=@GroupType     
    
          
select  *   
INTO #MatFinal
from #MatReqByBar T0 with (nolock) Order By SupplierName,ItemName        
      
select (T0.MinLevel -T0.[Event order] -(2*T0.[Last 7 Days] )-T0.AlreadyOrdered -T0.[In stock] )[RecommendedQuantity] ,*  
from #MatFinal T0 with (nolock)
      
DROP TABLE #MatReqByBar        
DROP TABLE #MatFinal 
    
    
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP0018_GetPendingDrafts]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP0018_GetPendingDrafts] '01CKT','Kitchen','1'

CREATE PROCEDURE [dbo].[AE_SP0018_GetPendingDrafts]        
        
@whsCode as varchar(40)    
,@UserRole as Varchar(40)  
,@UserLevel Varchar(5)  
        
AS        
Begin        
        
select T0.DocDate, T0.DocDueDate,T0.DocEntry,T5.U_GroupType [GroupType],U_ApprovalLevel ,U_L1ApprovalStatus ,U_L2ApprovalStatus ,VisOrder [LineNum]  
 , isnull(T0.[U_Urgent],'N') as 'Urgent', T4.CardCode AS 'SupplierCode' ,T4.CardName AS 'SupplierName',T1.ItemCode ,      
T1.Dscription as ItemName,T1.WhsCode  
, T6.WhsName          
,T1.Price   
,T1.Quantity as OrderQuantity  
  
,(SELECT dbo.Web_getInStock (T1.ItemCode,T1.WhsCode))[OnHand]   
,(SELECT dbo.Web_getSOComitmt (T1.ItemCode ,T1.WhsCode ,3))[Eventorder]       
,(select IsCommited from OITW where ItemCode =T1.ItemCode and WhsCode =T1.WhsCode )[IsCommited]      
,(SELECT dbo.Web_getAvgReq (T1.ItemCode ,T1.WhsCode  ,7)) as Last7DaysAvg  
,(SELECT dbo.Web_getPOQty (T1.ItemCode ,T1.WhsCode  )) AS AlreadyOrdered  
,(SELECT dbo.Web_getMinimumStock (T1.ItemCode ,T1.WhsCode  ))[MinLevel]   
  
, cast((IsNull(T1.Price,0.00) * IsNull(T1.Quantity,0.00) ) AS Decimal(19,6)) as Total       
,T2.[OnOrder],T2.[BuyUnitMsr] as UOM  
,isnull(T4.U_OutletMinSpend,0) [MinSpend]    
--,case when (select Count(*) from DRF1 TT0 with (nolock) where TT0.DocEntry =T0.DocEntry and isnull(TT0.LineVendor,TT0.U_LineVendor)= isnull(T1.LineVendor,T1.U_LineVendor)  
-- and TT0.ItemCode ='SDELIVERY')  >0  
--then 'Y'else 'N' End  
,CASE when @UserLevel =1 and U_L1ApprovalStatus ='Pending' then 1    
 when @UserLevel =2 and (U_L1ApprovalStatus ='Approved' and U_L2ApprovalStatus ='Pending') then 1 else 0 End [Approval],  
ISNULL (T1.U_DeliveryCharge,'N') AS 'DelChargeUDF'   
  
,T4.QryGroup1 [Mon],T4.QryGroup2 [Tue],T4.QryGroup3 [Wed],T4.QryGroup4 [Thu],T4.QryGroup5 [Fri],T4.QryGroup6 [Sat],T4.QryGroup7 [Sun]            
       
INTO #TEMP        
from ODRF T0  with (nolock)      
INNER JOIN DRF1 T1 with (nolock)on T0.DocEntry =T1.DocEntry         
INNER JOIN OITM T2 with (nolock)ON T2.ItemCode =T1.ItemCode        
INNER JOIN OCRD T4 with (nolock)on T4.CardCode = case when T0.CardCode = 'VA-HARHO' then t0.CardCode else isnull(T1.LineVendor,T1.U_LineVendor) end    
INNER JOIN OITB T5 with (nolock)on T5.ItmsGrpCod =T2.ItmsGrpCod  
LEFT JOIN OWHS T6 WITH (NOLOCK) ON T6.WhsCode =T1.WhsCode        
where T0.ObjType in('1470000113','540000006') and T1.WhsCode =@whsCode and T5.U_GroupType =@UserRole   
and T0.DocStatus ='O' and ISNULL (T0.CANCELED ,'N' )<>'Y'      
        
select (T0.MinLevel -T0.[Eventorder] -(2*T0.Last7DaysAvg)-T0.AlreadyOrdered -T0.OnHand  )[RecommendedQuantity] ,*  
 from #TEMP T0  with (nolock)
  where (CASE when @UserLevel =1 then U_L1ApprovalStatus  
    when @UserLevel =2 then U_L2ApprovalStatus end)='Pending'  Order by SupplierCode Asc    
                      
    
drop table #TEMP         
        
End
GO
/****** Object:  StoredProcedure [dbo].[AE_SP010_Popup_ItemSearch]    Script Date: 01/09/2015 17:59:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[AE_SP010_Popup_ItemSearch] '02CTS','Bar'

CREATE PROCEDURE [dbo].[AE_SP010_Popup_ItemSearch]      
         
@WhsCode varchar (40)      
,@GroupType Varchar(30)      
As      
Begin 

SELECT T3.CardCode 'SupplierCode',T3.CardName 'SupplierName',T0.[ItemCode],T6.U_GroupType [GroupType] 
,T0.[ItemName], T2.OnHand ,T2.IsCommited,isnull(T3.U_OutletMinSpend,0) [MinSpend],t4.Price,convert(decimal(18,6),0) [Total]   
  
,(SELECT dbo.Web_getInStock (T0.ItemCode,@WhsCode))[In stock]
,(SELECT dbo.Web_getSOComitmt (T0.ItemCode ,@WhsCode,3))[Event order]
,(SELECT dbo.Web_getAvgReq (T0.ItemCode ,@WhsCode ,7))[Last 7 Days] 
,(SELECT dbo.Web_getPOQty (T0.ItemCode ,@WhsCode ))[AlreadyOrdered]
,(SELECT dbo.Web_getMinimumStock (T0.ItemCode ,@WhsCode ))[MinLevel]    
   
,T0.[OnOrder],T0.[BuyUnitMsr] as UOM      
,T3.QryGroup1 [Mon],T3.QryGroup2 [Tue],T3.QryGroup3 [Wed],T3.QryGroup4 [Thu],T3.QryGroup5 [Fri],T3.QryGroup6 [Sat],T3.QryGroup7 [Sun]      
      
INTO #MatReqByBar      
FROM OITM T0 with (nolock) 
--INNER JOIN ITM2 T1 ON T0.[ItemCode] = T1.[ItemCode]       
INNER JOIN ITM1 T4 with (nolock)ON T4.[ItemCode] = T0.[ItemCode]       
INNER JOIN OITW T2 with (nolock)ON T0.[ItemCode] = T2.[ItemCode]      
INNER JOIN OCRD T3 with (nolock)ON T0.[CardCode] = T3.[CardCode]       
INNER JOIN OITB T6 with (nolock)ON T6.ItmsGrpCod =T0.ItmsGrpCod       
inner join OPLN T5 with (nolock)on T5.ListNum =T4.PriceList       
       
WHERE T2.[WhsCode] =@WhsCode and T5.ListName ='PurchasePrice'   
and isnull(T0.frozenFor,'N') ='N'  and isnull(T3.frozenFor,'N') ='N' and T3.frozenFor is not null
and T0.frozenFor is not null
--and t4.Price >0   
AND T6.U_GroupType=@GroupType       
      
            
select *     
INTO #MatFinal  
from #MatReqByBar T0  with (nolock)Order By SupplierName,ItemName          
        
select (T0.MinLevel -T0.[Event order] -(2*T0.[Last 7 Days] )-T0.AlreadyOrdered -T0.[In stock] )[RecommendedQuantity] ,*  
from #MatFinal T0  with (nolock)
        
DROP TABLE #MatReqByBar          
DROP TABLE #MatFinal   
      
End
GO
