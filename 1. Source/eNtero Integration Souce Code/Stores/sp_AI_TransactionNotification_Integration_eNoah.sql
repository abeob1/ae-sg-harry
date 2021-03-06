--ALTER DATABASE BW_AMS SET SINGLE_USER WITH ROLLBACK IMMEDIATE
--ALTER DATABASE BW_AMS COLLATE Latin1_General_CI_AS
--ALTER DATABASE BW_AMS SET MULTI_USER
alter PROC [dbo].sp_AI_TransactionNotification_Integration_eNoah
    @object_type NVARCHAR(20), 				-- SBO Object Type
    @transaction_type NCHAR(1),				-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
    @num_of_cols_in_key INT,
    @list_of_key_cols_tab_del NVARCHAR(255),
    @list_of_cols_val_tab_del NVARCHAR(255),
    @error INT OUTPUT,
    @error_message NVARCHAR(200) OUTPUT
AS 
BEGIN

	declare @IntegrationSAPUser nvarchar(8)
	set @IntegrationSAPUser='B1i'
	---------------------------ITEM + PRICE------------------------
	if @object_type=N'4' 
	begin
		insert into SAP_Integration..ItemMaster  --Hardcode, fix database name: SAP_Integration
		(ItemCode,ItemName,BaselPrice,ItemSource,IsDelete,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		select T0.ItemCode, T0.ItemName, T1.Price,T0.U_ItemSource, 'N' , 
		case when frozenfor='Y' then 'N' else 'Y' end,--inactive
			GETDATE(),null, null,N'SAP'
		from OITM T0  with(nolock) 
		join ITM1 T1  with(nolock) on T0.ItemCode=T1.ItemCode and T1.PriceList=1 --hardcode, fix pricelist 1
		where T0.ItemCode=@list_of_cols_val_tab_del and isnull(T0.U_POSFlag,'N')='Y' --only for POS Item
		and isnull(T0.U_ItemSource,'')<>'RES'
		
		-----------if Item is Room Type : update status of ROOM ---------------
		
		update [@ROOM] set U_IsActive=(select isnull(U_POSFlag,'N') from OITM where ItemCode=@list_of_cols_val_tab_del)
		where U_RoomType = @list_of_cols_val_tab_del
		
		insert into SAP_Integration..[@ROOM]
		(RoomCode,RoomName,RoomType,HotelCode,FloorCode,MaxGuest,NoOfBed,IsSofa,IsTelephone,
		IsHairDryer,IsNonSmoker,IsDesk,IsMiniBar,IsCoffeeMachine,IsMicrowave,IsActive,SendDate,ReceiveDate,ErrMsg,Source)		
		select Code,Name,U_RoomType,isnull(U_HotelCode,'N'),isnull(U_FloorCode,'N')
		,isnull(U_MaxGuest,2),isnull(U_NoOfBed,1),isnull(U_IsSofa,'N'),isnull(U_IsTelephone,'N')
		,isnull(U_IsHairDryer,'N'),isnull(U_IsNonSmoker,'N'),isnull(U_IsDesk,'N'),isnull(U_IsMiniBar,'N')
		,isnull(U_IsCoffeeMachine,'N'),isnull(U_IsMicrowave,'N'),U_IsActive, GETDATE(),null,'','SAP'
		from [@ROOM] where U_RoomType=@list_of_cols_val_tab_del
		
		---------------------------------
		insert into SAP_Integration..POSItemMaster  --Hardcode, fix database name: SAP_Integration
		(ItemCode,ItemName,BasePrice,ItemSource,IsDelete,IsActive,SendDate,ReceiveDate,ErrMsg,Source,IsCook,GSTPer,SCPer,IsTaxIncluded,
		ItemGroup,MCategory,OutletCode,RevenueAccount)
		select T0.ItemCode, T0.ItemName, T1.Price,T0.U_ItemSource, 'N' , 
		case when frozenfor='Y' then 'N' else 'Y' end,--inactive
			GETDATE(),null, null,N'SAP',isnull(U_IsCook,'N'),(select Rate from ovtg where code=T0.VatGourpSa),U_SCPer,U_IsTax,
		T2.ItmsGrpNam,T3.U_Cagetory,T3.U_Outlet,''
		from OITM T0  with(nolock) 
		join ITM1 T1  with(nolock) on T0.ItemCode=T1.ItemCode 
		join OITB T2  with(nolock) on T2.ItmsGrpCod=T0.ItmsGrpCod
		join OPLN T3 on T1.PriceList=T3.ListNum
		where T0.ItemCode=@list_of_cols_val_tab_del 
		and isnull(T0.U_POSFlag,'N')='Y' --only for POS Item
		and ISNULL(T3.U_Cagetory,'') <>''
		and ISNULL(T3.U_Outlet,'')<>''
		and isnull(T0.U_ItemSource,'')='RES'
		
		---------------incase of delete---------------------
		if @transaction_type='D'
		begin
			insert into SAP_Integration..ItemMaster  --Hardcode, fix database name: SAP_Integration
			(ItemCode,ItemName,BaselPrice,ItemSource,IsDelete,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'', 0, '','Y','N',GETDATE(),null, null,N'SAP' 
		end
		
		---------------insert price from all price list-----------------
		insert into SAP_Integration..ITM1
		(PriceList,ItemCode,Price,SendDate,ReceiveDate,ErrMsg,Source)
		select T0.PriceList,T0.ItemCode,isnull(T0.Price,0),GETDATE(),null,'','SAP'
		from ITM1 T0 
		join OITM T1 on T0.ItemCode=T1.ItemCode
		where T0.ItemCode=@list_of_cols_val_tab_del and isnull(T1.U_POSFlag,'N')='Y' --only for POS Item
		
	end
	---------------------------EMPLOYEE----------------------------
	if @object_type=N'171'
	begin
		insert into SAP_Integration..EmployeeMaster
		(empID,lastName,firstName,middleName,IsActive,IsDelete, SendDate,ReceiveDate,ErrMsg,Source,UserName,eMail,HotelCode,
		ENTFB,ENTFO,OCFB,OCFO)
		select empID,lastName,firstName,middleName,Active,'N',GETDATE(),null,'','SAP' ,
		U_UserName,email,U_Hotel,U_ENTFB,U_ENTFO,U_OCFB,U_OCFO
		from OHEM
		where empID=@list_of_cols_val_tab_del
		---------------incase of delete---------------------
		if @transaction_type='D'
		begin
			insert into SAP_Integration..EmployeeMaster
			(empID,lastName,firstName,middleName,IsActive,IsDelete, SendDate,ReceiveDate,ErrMsg,Source,UserName,eMail,HotelCode)
			select @list_of_cols_val_tab_del,'','','','N','Y',GETDATE(),null,'','SAP','','',''
		end
		
		--if @transaction_type='A'
		--begin
		--	declare @LoginID nvarchar(100)
		--	declare @EmailID nvarchar(100)
		--	declare @EmpID int
		--	declare @Manager int
		--	declare @DefaultWhs nvarchar(8)
		--	select @LoginID=U_UserName,@EmailID=isnull(email,'a@a.com'),@EmpID=empID,
		--		@Manager=isnull(manager,empID),@DefaultWhs='01' from OHEM where empID=@list_of_cols_val_tab_del
		--	exec sboweb_bw..sp_CreateNewUser @LoginID, @EmailID,@EmpID,@Manager,@DefaultWhs
		--end
	end
	---------------------------BUSINESS PARTNER--------------------
	if @object_type=N'2'
	begin
		insert into SAP_Integration..CustomerMaster --Hardcode, fix database name: SAP_Integration
		(CardCode,Title,FirstName,LastName,MiddleName,CardName,Gender,GroupCode,IDType,IDNo,
		DOB,Nationality,Address,Block,City,ZipCode,Country,Phone1,Phone2,Mobile,Fax,E_Mail,
		IsDelete,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		select top(1) -- incase bill-to or ship-to return more than 2 records
			T0.CardCode,'','','','',T0.CardName,'',T1.GroupCode,'',isnull(T0.VatIdUnCmp,''),null,null,
			T2.Address,T2.Block,T2.City,T2.ZipCode,T2.Country,
			T0.Phone1,T0.Phone2,T0.Cellular, T0.Fax,T0.E_Mail,
			CASE when @transaction_type='D' then 'Y' else 'N' end, frozenfor,--inactive
			GETDATE(),null, null,N'SAP'
		from OCRD T0  with(nolock) 
		join OCRG T1 with(nolock)  on T0.GroupCode=T1.GroupCode
		left join CRD1 T2  with(nolock) on T2.CardCode=T0.CardCode and T2.AdresType='B' -- Bill-To
		where T0.CardCode=@list_of_cols_val_tab_del and isnull(T0.U_POSFlag,'N')='Y' --only for POS BP
		
		if @transaction_type='D'
		begin
			insert into SAP_Integration..CustomerMaster --Hardcode, fix database name: SAP_Integration
			(CardCode,Title,FirstName,LastName,MiddleName,CardName,Gender,GroupCode,IDType,IDNo,
			DOB,Nationality,Address,Block,City,ZipCode,Country,Phone1,Phone2,Mobile,Fax,E_Mail,
			IsDelete,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'title','firstname','lastname','middlename','cardname','M', 0,'','',null,null,'address',
			'block','city','zipcode','VN','phone1','phone2','mobile','fax','email', 
			'Y', 'N',	GETDATE(),null, null,N'SAP'
		end
	end
	---------------------------CURRENCY----------------------------
	if @object_type=N'37'
	begin
		insert into SAP_Integration..CurrencyMaster
		(CurrCode,CurrName,Decimal,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
		select CurrCode,CurrName,Decimals,'N',GETDATE(),null,'','SAP' 
		from OCRN 
		where CurrCode=@list_of_cols_val_tab_del
		---------------incase of delete---------------------
		if @transaction_type='D'
		begin
			insert into SAP_Integration..CurrencyMaster
			(CurrCode,CurrName,Decimal,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'',0,'Y',GETDATE(),null,'','SAP' 
		end
	end		
	---------------------------HOTEL MASTER------------------------
	if @object_type=N'-3'+CHAR(9)+'@HOTEL'
	begin
		insert into SAP_Integration..[@HOTEL]
		(HotelCode,HotelName,IsActive,SendDate,ReceiveDate,ErrMsg,Source,Address1,Address2,City,Country,PinCode,ContactNumber,
			Fax,WebSite,Email,LogoURL,LoginLogoURL)
		select Code,Name,isnull(U_IsActive,'N'), GETDATE(),null,'','SAP',
		U_Address1,U_Address2,U_City,U_Country,U_PinCode,U_ContactNumber,U_Fax,U_WebSite, 
		U_Email,isnull(convert(nvarchar(100),T1.BitmapPath),'') + U_LogoUrl,isnull(convert(nvarchar(100),T1.BitmapPath),'') + U_LoginLogoUrl
		from [@HOTEL] T0 
		join OADP T1 on 1=1
		where Code=@list_of_cols_val_tab_del
	end
	---------------------------TCODE MASTER------------------------
	if @object_type=N'-3'+CHAR(9)+'@TCODE'
	begin
		insert into SAP_Integration..TCodeMaster
		(TCODE,NAME,AUTO,ISACTIVE, SENDDATE, RECEIVEDATE,ERRMSG,SOURCE)
		select Code,Name,isnull(U_AUTO,'N'),isnull(U_IsActive,'N'), GETDATE(),null,'','SAP'
		from [@TCODE] T0 
		where Code=@list_of_cols_val_tab_del
	end
	---------------------------FLOOR MASTER------------------------
	if @object_type=N'-3'+CHAR(9)+'@FLOOR'
	begin
		insert into SAP_Integration..[@FLOOR]
		(FloorLevel, FloorCode,FloorName,HotelCode,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		select U_FloorLevel, Code,Name,U_Hotel,U_IsActive, GETDATE(),null,'','SAP'
		from [@FLOOR] where Code=@list_of_cols_val_tab_del
	end
	---------------------------ROOM MASTER-------------------------
	if @object_type=N'-3'+CHAR(9)+'@ROOM'
	begin
		
		insert into SAP_Integration..[@ROOM]
		(RoomCode,RoomName,RoomType,HotelCode,FloorCode,MaxGuest,NoOfBed,IsSofa,IsTelephone,
		IsHairDryer,IsNonSmoker,IsDesk,IsMiniBar,IsCoffeeMachine,IsMicrowave,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		
		select Code,Name,U_RoomType,isnull(U_HotelCode,'N'),isnull(U_FloorCode,'N')
		,isnull(U_MaxGuest,2),isnull(U_NoOfBed,1),isnull(U_IsSofa,'N'),isnull(U_IsTelephone,'N')
		,isnull(U_IsHairDryer,'N'),isnull(U_IsNonSmoker,'N'),isnull(U_IsDesk,'N'),isnull(U_IsMiniBar,'N')
		,isnull(U_IsCoffeeMachine,'N'),isnull(U_IsMicrowave,'N'),U_IsActive, GETDATE(),null,'','SAP'
		from [@ROOM] where Code=@list_of_cols_val_tab_del
	end
	---------------------------MEETING ROOM MASTER-------------------------
	if @object_type=N'-3'+CHAR(9)+'@MEETINGROOM'
	begin
		
		insert into SAP_Integration..[@MEETINGROOMMASTER]
		(RoomCode,RoomName,RoomType,HotelCode,FloorCode,MaxGuest,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		
		select Code,Name,U_RoomType,isnull(U_HotelCode,'N'),isnull(U_FloorCode,'N')
		,isnull(U_MaxGuest,2),U_IsActive, GETDATE(),null,'','SAP'
		from [@MEETINGROOM] where Code=@list_of_cols_val_tab_del
	end
	---------------------------RESTAURANT TABLE MASTER-------------------------
	if @object_type=N'-3'+CHAR(9)+'@RESTABLES'
	begin
		
		insert into SAP_Integration..[@POSTABLEMASTER]
		(TableCode,TableName,HotelCode,FloorCode,Outlet, MaxGuest,IsActive,SendDate,ReceiveDate,ErrMsg,Source)
		
		select Code,Name,isnull(U_HotelCode,'N'),isnull(U_FloorCode,'N'),U_Outlet
		,isnull(U_MaxGuest,2),U_IsActive, GETDATE(),null,'','SAP'
		from [@RESTABLES] where Code=@list_of_cols_val_tab_del
	end
	---------------------------MAPPING-----------------------------
	if @object_type=N'-3'+CHAR(9)+'@MAPPING'
	begin
		if (select COUNT(*) from SAP_Integration..[@MAPPING] where Code=@list_of_cols_val_tab_del)=1
		begin
			update T0 set Code=T1.Code,Value=T1.Name, Description=T1.U_Description
			from SAP_Integration..[@MAPPING] T0 
			join [@MAPPING] T1 on T0.Code COLLATE DATABASE_DEFAULT =T1.Code	 COLLATE DATABASE_DEFAULT		
			where T1.Code=@list_of_cols_val_tab_del
		end			
		else
		begin
			insert into SAP_Integration..[@MAPPING]
			select Code,Name,U_Description
			from [@MAPPING] where Code=@list_of_cols_val_tab_del
		end
		
		if @transaction_type='D'
		begin
			set @error=-1
			set @error_message='YOU CANNOT DELETE!!'
		end
	end
	---------------------------CUSTOMER GROUP----------------------
	if @object_type=N'10'
	begin
		insert into SAP_Integration..CustomerGroupMaster
		(GroupCode,GroupName,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
		select GroupCode,GroupName,'N',GETDATE(),null,'','SAP'
		from OCRG where GroupCode=@list_of_cols_val_tab_del and GroupType='C'
		
		if @transaction_type='D'
		begin
			insert into SAP_Integration..CustomerGroupMaster
			(GroupCode,GroupName,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'','Y',GETDATE(),null,'','SAP'
		end
	end
	----------------------------SPA COMMISSION---------------------
	if @object_type=N'-3'+CHAR(9)+'@SPACOMMISSION'
	begin
		insert into SAP_Integration..[@SPACOMMISSION]
		select U_ItemCode,U_ItemName,U_Hour,U_Commission,GETDATE(),null,'','SAP'
		from [@SPACOMMISSION]
	end
	--------------------------Marketing SOURCE----------------------
	if @object_type=N'100'
	begin
		insert into SAP_Integration..MarketingSourceMaster
		select num,descript,GETDATE(),null,'','SAP'
		from OOSR T0 where Num=@list_of_cols_val_tab_del
	end
	--------------------------CostCenter----------------------
	if @object_type=N'61'
	begin
		insert into SAP_Integration..CostCenter
		(CostCenterCode,CostCenterName,Dimension,IsActive,IsDelete, SendDate,ReceiveDate,ErrMsg,Source,FromTime,ToTime,
		RevenueAct,MealPlan,Department,Division,MarketSegment,Auto)
		select PrcCode,PrcName,DimCode,Active,'N',GETDATE(),null,'','SAP',U_FromTime,U_ToTime,
		U_RevenueAct,U_MealPlan,U_Department,U_Division,U_MarketSegment,U_Auto, U_HotelCode
		from OPRC T0 where PrcCode=@list_of_cols_val_tab_del
		
		if @transaction_type='D'
		begin
			insert into SAP_Integration..CostCenter
			(CostCenterCode,CostCenterName,Dimension,IsActive,IsDelete, SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'',0,'Y','Y',GETDATE(),null,'','SAP'
		end
	end
	---------------------------PAYMENT METHOD-------------------------
	if @object_type=N'-3'+CHAR(9)+'@PAYMENTMETHOD'
	begin
		insert into SAP_Integration..PaymentMethod
		(PaymentMethodCode,PaymentMethodName,IsActive,PaymentType,SendDate,ReceiveDate,ErrMsg,Source,Division,TCode,IsSC, IsTAX)
		select Code,Name,U_IsActive,U_PaymentType, GETDATE(),null,'','SAP',U_Division, U_TCode,U_IsTax,U_IsSC
		from [@PAYMENTMETHOD] where Code=@list_of_cols_val_tab_del
	end
	----------------------------SHIFT MASTER---------------------
	if @object_type=N'-3'+CHAR(9)+'@SHIFTMASTER'
	begin
		insert into SAP_Integration..[@SHIFTMASTER]
		select Code,Name,U_IsActive,GETDATE(),null,'','SAP',U_ModuleName
		from [@SHIFTMASTER] where Code=@list_of_cols_val_tab_del
	end
	----------------------------OUTLET MASTER---------------------
	if @object_type=N'64'
	begin
		insert into SAP_Integration..OutletMaster
		select WhsCode,WhsName,U_IsOutlet,GETDATE(),null,'','SAP',U_HotelCode
		from OWHS where WhsCode=@list_of_cols_val_tab_del		
	end
	---------------------------PRICING----------------------
	Exec sp_AI_TransactionNotification_Integration_eNoah_Pricing
							                   @object_type =  @object_type, 				
                                               @transaction_type = @transaction_type ,			
                                               @num_of_cols_in_key = @num_of_cols_in_key,
                                               @list_of_key_cols_tab_del = @list_of_key_cols_tab_del,
                                               @list_of_cols_val_tab_del =@list_of_cols_val_tab_del,
							                   @error = @error OUTPUT,
							                   @error_message = @error_message OUTPUT
	
	
	select @error, @error_message
END                                               
                                              
