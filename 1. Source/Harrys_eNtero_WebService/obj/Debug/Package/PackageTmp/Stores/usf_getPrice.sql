alter function [dbo].[usf_getPrice](
@ItemCode varchar(100),
@GroupCode varchar(100),
@CardCode varchar(100),
@Qty int,
@Date date)
returns @tab table(
	ItemCode nvarchar(100),
	ItemName nvarchar(100),
	Price numeric(19, 2),
	Discount numeric(19, 2),
	PriceAfter numeric(19, 2),
	Statute nvarchar(500)
)
as
begin

	declare @Price as numeric(19,6)
	declare @status as nvarchar(50)
	declare @discount as numeric(19,6)
	declare @basePrice as numeric(19,6)
	declare @itemName as nvarchar(100)
	
	set @discount = 0
	--base price
	
	if isnull(@CardCode,'') = '' or @CardCode ='' -- choose any cuscode that belongs to this group code
		begin
			select @basePrice = itm.Price, @itemName = oi.ItemName 
			from ITM1 itm join OITM oi on oi.ItemCode = itm.ItemCode
			where itm.ItemCode = @ItemCode and itm.PriceList = (select rg.PriceList from OCRG rg where rg.GroupCode = @GroupCode)
			 
			set @CardCode = (select top 1 cus.CardCode from OCRD cus where cus.GroupCode = @GroupCode)

		end
	else
		select @basePrice = itm.Price, @itemName = oi.ItemName 
		from ITM1 itm join OCRD cus on itm.PriceList = cus.ListNum 
		join OITM oi on oi.ItemCode = itm.ItemCode
		where itm.ItemCode = @ItemCode and cus.CardCode = @CardCode
	
												
	--case 1.: Special price for BP	
	--case 1.1: By volume (qty)
	select @Price = s2.Price, @status = '1.1 SPP2. dics by qty', @discount = s2.Discount 
			from SPP2 s2 where s2.ItemCode = @ItemCode 
					and s2.CardCode = @CardCode 
					and s2.Amount = (select max(is2.Amount) from SPP2 is2 
											where is2.ItemCode = @ItemCode 
												and is2.Amount <= @Qty) 
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end		

	--case 1.2: By period
	select @Price = s1.Price, @status = '1.2 SPP1. dics by period', @discount = s1.Discount 
		from SPP1 s1 where s1.ItemCode = @ItemCode 
						and s1.CardCode = @CardCode 
						and (@Date between isnull(s1.FromDate, '1990-01-01') and isnull(s1.ToDate, '2999-01-01'))

	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end		

	-- case 1.3: For BP
	select @Price = os.Price, @status = '1.3 OSPP. disc for special BP', @discount = os.Discount 
		from OSPP os where os.ItemCode = @ItemCode 
						and os.CardCode = @CardCode 
										
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end
	

	--Objtype:
	--	-1: all BPs
	--	2: Specific BP
	--	10: BP Group
		
	--ObjKey:
	--	?:Item Group
	--	?:Properties
	--	?: Item
	--	?: Manufacture

	--case 2: Discount Group
	  --case 2.1: Special BP
		--case 2.1.1: By item
	select @Price = it1.Price*(1-g1.Discount/100), @status = '2.1.1 Specific BP by Item', @discount = g1.Discount 
		from OEDG g0 join EDG1 g1 on g0.AbsEntry = g1.AbsEntry
					join ITM1 it1 on it1.ItemCode = g1.ObjKey
					join OCRD bp on bp.CardCode = g0.ObjCode
	where g0.ObjCode = @CardCode 
	and it1.ItemCode = @ItemCode and it1.PriceList = bp.ListNum
	and g0.ObjType = 2
	and g0.ValidFor = 'Y' 
	and (@Date between isnull(g0.ValidForm, '1990-01-01') and isnull(g0.ValidTo, '2999-01-01'))
														
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end							
	  --case 2.2: Customer group & vendor
		--case 2.2.1: By item
		
		
		-----------------------------lay theo @baseprice --------------------
		--: sua lai-----------------------
	select @Price = @basePrice*(1-g1.Discount/100), @status = '2.2.1 Customer Group and vendor by Item', @discount = g1.Discount 
	from OEDG g0 join EDG1 g1 on g0.AbsEntry = g1.AbsEntry
				join ITM1 it1 on it1.ItemCode = g1.ObjKey
				join OCRG gr on gr.GroupCode = g0.ObjCode
	where it1.ItemCode = @ItemCode
				and it1.PriceList = (select rg.PriceList from OCRG rg where rg.GroupCode = @GroupCode)--get price list of group
				--and it1.PriceList = (select bp.ListNum from OCRD bp where bp.CardCode = @CardCode)
				and g0.ObjType = 10   
				and g0.ValidFor = 'Y' 
				and (@Date between isnull(g0.ValidForm, '1990-01-01') and isnull(g0.ValidTo, '2999-01-01'))							
								 
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end
	  --case 2.3: All BP
		--case 2.3.1: By item  
	select @Price = it1.Price*(1-g1.Discount/100), @status = '2.3.1 All BP by Item', @discount = g1.Discount 
	from OEDG g0 join EDG1 g1 on g0.AbsEntry = g1.AbsEntry
			join ITM1 it1 on it1.ItemCode = g1.ObjKey
	where  it1.ItemCode = @ItemCode 
			and 
			it1.PriceList = isnull((select rg.PriceList from OCRG rg where rg.GroupCode = @GroupCode),(select bp.ListNum from OCRD bp where bp.CardCode = @CardCode)) --get price list of group
			and g0.ObjType = -1
			and g0.ValidFor = 'Y'
			and (@Date between isnull(g0.ValidForm, '1990-01-01') and isnull(g0.ValidTo, '2999-01-01'))
								
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end
	--case 3: Special price for all BP
	--case 3.1: By volume (qty)
	select @Price = s2.Price, @status = '3.1 SPP2. dics by qty', @discount = s2.Discount from SPP2 s2 
	where s2.ItemCode = @ItemCode 
			and s2.Amount = (select max(is2.Amount) from SPP2 is2 
									where is2.ItemCode = @ItemCode 
										and is2.Amount <= @Qty) 
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end		

	--case 3.2: By period
	select @Price = s1.Price, @status = '3.2 SPP1. dics by period', @discount = s1.Discount 
	from SPP1 s1 where s1.ItemCode = @ItemCode 
			and (@Date between isnull(s1.FromDate, '1990-01-01') and isnull(s1.ToDate, '2999-01-01'))
			
	if (@Price is not null) begin
		insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, case when @Price IS null then @basePrice else @Price end, @status)
		return
	end		

	--case 4: base price
			
	insert into @tab values(@ItemCode, @itemName, @basePrice, @discount, 
	case when @Price IS null then @basePrice else @Price end, '4. Base Price')
	return
end
-- select * from usf_getPrice ('STD-DLX', '', 'C99999', 1, '2013-04-13')
-- select * from OCRD
--select * from OITM
/*
select itm.ItemCode, itm.Price, oi.ItemName 
			from ITM1 itm join OITM oi on oi.ItemCode = itm.ItemCode
			where itm.ItemCode = 'STD-DLX' and itm.PriceList = (select rg.PriceList from OCRG rg where rg.GroupCode = 100)
			
			select * from ITM1		 
			*/