
/*
	return the package list from OITT with BOM type is sales BOM
	include standard room type (@ItemCode)
	Column return: ItemCode,ItemName,Price,Discount,PriceAfter,Flag,U_IsTax,ComponentList
	usp_getPriceFull
*/
--select * from [usf_getPrice]('QUEEN',100,'',1,'2014-12-9')
--[sp_AI_PackageSelection] 'QUEEN',100,'',1,'2014-12-9'
alter procedure [dbo].[sp_AI_PackageSelection]
@ItemCode varchar(20),
@GroupCode int,
@CardCode varchar(15),
@Qty int,
@Date date
as
begin
	-- Pseudo Data
	declare @tab table(
		ItemCode nvarchar(20) ,
		ItemName nvarchar(100),
		Price numeric(19,2),
		Discount numeric(19, 2),
		PriceAfter numeric(19, 2),
		Flag char(3),
		IsTax nvarchar(1),
		VATPercent numeric(10,2),
		SCPercent numeric(10,2),
		HotelCode nvarchar(30)
	)
	
	if isnull(@CardCode,'') = '' -- choose any cuscode that belongs to this group code
	begin
		insert into @tab(ItemCode, ItemName, Flag,IsTax,HotelCode)
		--select oi.ItemCode, oi.ItemName, 'STD' ,oi.U_IsTax
		--from OITM oi			
		--join ITM1 itm  on oi.ItemCode = itm.ItemCode
		--join OVTG T2 on oi.VatGourpSa=T2.Code
		--where itm.ItemCode = @ItemCode 
		--	and itm.PriceList = (select rg.PriceList from OCRG rg where rg.GroupCode = @GroupCode)
											
		--union all
		select distinct T.Father, oi1.ItemName, 'BOM' ,T.U_IsTax, oi.U_Hotel
		from 
		(
			select t1.Code, t1.Father,tt.U_IsTax
			from OITT tt 
			join ITT1 t1 on tt.Code = t1.Father
			where TreeType='S' --only get sales BOM
		) T 
		join ITM1 itm on T.Code = itm.ItemCode
		join OITM oi on oi.ItemCode = itm.ItemCode
		join OITM oi1 on oi1.ItemCode = T.Father
		where itm.ItemCode = @ItemCode 
			and itm.PriceList = (select rg.PriceList from OCRG rg where rg.GroupCode = @GroupCode)
	end
	else
	begin	
		
		insert into @tab(ItemCode, ItemName, Flag,IsTax,HotelCode)
		--select oi.ItemCode, oi.ItemName, 'STD' ,oi.U_IsTax
		--from OITM oi
		--join ITM1 itm  on oi.ItemCode = itm.ItemCode
		--join OCRD cus on itm.PriceList = cus.ListNum 
		--where itm.ItemCode = @ItemCode and cus.CardCode = @CardCode 
											
		--union all
		select distinct T.Father, oi1.ItemName, 'BOM' ,T.U_IsTax,oi.U_Hotel
		from 
		(
			select t1.Code, t1.Father ,tt.U_IsTax
			from OITT tt join ITT1 t1 on tt.Code = t1.Father
			where TreeType='S' --only get sales BOM
		) T 
		join ITM1 itm on T.Code = itm.ItemCode
		join OCRD cus on itm.PriceList = cus.ListNum 
		join OITM oi on oi.ItemCode = itm.ItemCode
		join OITM oi1 on oi1.ItemCode = T.Father
		where itm.ItemCode = @ItemCode and cus.CardCode = @CardCode
	end	
	
	select ta.ItemCode, ta.ItemName, T.Price, T.Discount, 
				T.PriceAfter, dbo.usf_getChildList(ta.ItemCode) as Items, isnull(ta.IsTax,'N') as IsTaxIncluded,
				10 SCPercent, 6 VATPercent, 
				case when isnull(ta.IsTax,'N')='N' then T.PriceAfter*1.16 
					else T.PriceAfter
				end PriceAfterTaxes, ta.HotelCode
		 from @tab ta 
		cross apply dbo.usf_getPrice (ta.ItemCode, @GroupCode, @CardCode, @Qty, @Date) T
		--left join OITT tt on tt.Code COLLATE DATABASE_DEFAULT = ta.ItemCode	COLLATE DATABASE_DEFAULT
end


