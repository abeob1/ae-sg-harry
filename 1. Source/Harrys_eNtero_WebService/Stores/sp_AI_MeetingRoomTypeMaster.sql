alter proc sp_AI_MeetingRoomTypeMaster
as
select tm.ItemCode, tm.ItemName, t1.Price as BasePrice, '' as Items, U_Hotel from OITM tm	
			join ITM1 t1 on t1.ItemCode = tm.ItemCode
		and t1.PriceList = 1 and tm.U_POSFlag = 'Y' and tm.U_ItemSource = 'MEETING'