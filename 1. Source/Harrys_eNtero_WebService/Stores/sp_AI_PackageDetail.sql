--sp_AI_PackageDetail 'Q001' 
alter proc sp_AI_PackageDetail
@ItemCode nvarchar(30)
as
select T0.Quantity PAX, T1.ItemCode,T1.ItemName, T0.Price, 
CASE when T1.U_ItemSource='ROOM' then '1' else '0' end IsRoom, 
T1.U_RevenueCode ItemGroup,T0.Warehouse OutletCode ,
CASE when ISNULL(T1.U_RevenueCode,'')='B' then '1' else '0' end IsBreakfast
from ITT1 T0 
join OITM T1 on T0.Code=T1.Itemcode  
where T0.Father=@ItemCode
 order by Quantity
