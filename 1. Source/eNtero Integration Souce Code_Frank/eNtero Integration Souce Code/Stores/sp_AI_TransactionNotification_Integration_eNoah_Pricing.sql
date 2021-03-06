--ALTER DATABASE SAP_BW SET SINGLE_USER WITH ROLLBACK IMMEDIATE
--ALTER DATABASE SAP_BW COLLATE Latin1_General_CI_AS
--ALTER DATABASE SAP_BW SET MULTI_USER
create PROC [dbo].sp_AI_TransactionNotification_Integration_eNoah_Pricing
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
	
	----------------------------------NOT USING NOW--------------------------------
	--------------------------BOM----------------------------------
	if @object_type=N'66'
	begin
		insert into SAP_Integration..OITT
		(ItemCode,SendDate,ReceiveDate,ErrMsg,Source)
		select Code,GETDATE(),null,'','SAP'
		from OITT T0 where Code=@list_of_cols_val_tab_del
			and isnull(T0.U_POSFlag,'N')='Y'
		
		insert into SAP_Integration..ITT1
		(ItemCode,FatherCode,Quantity)
		select Code,Father,Quantity from ITT1
		where Father=@list_of_cols_val_tab_del
	end
	---------------------------PRICE LIST--------------------------
	if @object_type=N'6'
	begin
		insert into SAP_Integration..OPLN
		(ListNum,ListName,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
		select ListNum,ListName,'N',getdate(),null,'','SAP' from OPLN where ListNum=@list_of_cols_val_tab_del
		---------------incase of delete---------------------
		if @transaction_type='D'
		begin
			insert into SAP_Integration..OPLN
			(ListNum,ListName,IsDelete,SendDate,ReceiveDate,ErrMsg,Source)
			select @list_of_cols_val_tab_del,'','Y',getdate(),null,'','SAP'
		end
	end
	---------------------------SPECIAL PRICE-----------------------	
	if @object_type='7'
	begin
		declare @ItemCode nvarchar(40)
		declare @CardCode nvarchar(35)
		declare @spacePos decimal
		select @spacePos=CHARINDEX(CHAR(9),@list_of_cols_val_tab_del)
		
		Select @CardCode=rtrim(SUBSTRING(@list_of_cols_val_tab_del,0,@spacePos))
		select @ItemCode=ltrim(SUBSTRING(@list_of_cols_val_tab_del,@spacePos+1,LEN(@list_of_cols_val_tab_del)-len(@CardCode)))
		
		
		insert into SAP_Integration..OSPP
		select @ItemCode,@CardCode,Price,Discount,@transaction_type,GETDATE(),null,'','SAP' from OSPP
		where ItemCode=@ItemCode and CardCode=@CardCode
		
		insert into SAP_Integration..SPP1
		select ItemCode,CardCode,LINENUM,Price,Discount,FromDate,ToDate,GETDATE(),null,'','SAP' from SPP1
		where ItemCode=@ItemCode and CardCode=@CardCode
	
		insert into SAP_Integration..SPP2
		select ItemCode,CardCode,SPP1LNum,SPP2LNum,Amount,Price,Discount,GETDATE(),null,'','SAP' from SPP2
		where ItemCode=@ItemCode and CardCode=@CardCode	
		
		--if @transaction_type='D'
		--begin
		--end
	end
	---------------------------DISCOUNT GROUP----------------------
	if @object_type=N'1470000077'
	begin
		insert into SAP_Integration..OEDG
		select AbsEntry,Type,ObjType,ObjCode,DiscRel,ValidFor,ValidForm,ValidTo,'N',
		GETDATE(),null,'','SAP'
		from OEDG where AbsEntry=@list_of_cols_val_tab_del
		
		insert into SAP_Integration..EDG1
		select AbsEntry,ObjType,ObjKey,DiscType,Discount,PayFor,ForFree,UpTo
		from EDG1 where AbsEntry=@list_of_cols_val_tab_del
		
		if @transaction_type='D'
		begin
			insert into SAP_Integration..OEDG
			select @list_of_cols_val_tab_del,'A','A','A','A','N',null,null,'Y',
			GETDATE(),null,'','SAP'
		end
	end
	
	
	--------------------------EXCHANGE RATE------------------------
	
	
	select @error, @error_message
END                                               
                                              
