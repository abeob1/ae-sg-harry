
--usp_getItemList add on 
create proc [dbo].[sp_AI_AddonSelection](
@GroupCode varchar(20),
@CardCode varchar(15),
@Date date)
as
begin

	select oi.ItemCode, oi.ItemName, T.Price, T.Discount, T.PriceAfter, 
	dbo.usf_getChildList(oi.ItemCode) as Items, isnull(oi.U_IsTax,'N') as IsTaxIncluded,
	10 SCPercent, 6 VATPercent, 
	case when isnull(oi.U_IsTax,'N')='N' then T.PriceAfter*1.16 
		else T.PriceAfter
	end PriceAfterTaxes
	from OITM oi cross apply dbo.usf_getPrice (oi.ItemCode, @GroupCode, @CardCode, 1, @Date) T
	where isnull(oi.U_ItemSource,'') = 'SVC' -- don't get ROOM
		and ISNULL(oi.U_POSFlag,'N')= 'Y' --get item for POS only
end

--  exec sp_AI_AddonSelection 100, '', '2013-04-08'
