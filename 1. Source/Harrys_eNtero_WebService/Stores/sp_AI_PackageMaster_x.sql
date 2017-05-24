alter proc sp_AI_PackageMaster
as

select oi.Code, tm.ItemName, t1.Price as BasePrice, dbo.usf_getChildList(oi.Code) as Items ,'N' IsTaxIncluded,
10 SCPercent, 6 VATPercent, case when isnull(oi.U_IsTax,'N')='N' then t1.Price*1.16 else t1.Price end PriceAfterTaxes,
tm.U_Hotel,tm.U_RevenueCode ItemGroup,tm.U_Hotel
from OITT oi join OITM tm on oi.Code = tm.ItemCode
join ITM1 t1 on t1.ItemCode = tm.ItemCode and t1.PriceList = 1
where oi.TreeType = 'S' and oi.U_POSFlag = 'Y' --and isnull(tm.U_Hotel,'')=@HotelCode 