--ALTER DATABASE SAP_BW SET SINGLE_USER WITH ROLLBACK IMMEDIATE
--ALTER DATABASE SAP_BW COLLATE Latin1_General_CI_AS
--ALTER DATABASE SAP_BW SET MULTI_USER
alter PROC [dbo].sp_AI_TransactionNotification_Customize
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
	
	---------------------------BUSINESS PARTNER--------------------
	/*Do not allow to update GUEST information from SAP*/
	if @object_type=N'2'
	begin
		declare @createusercode nvarchar(100)
		select @createusercode=T1.user_code from OCRD T0
			join OUSR T1 on T1.USERID=T0.UserSign
			where T0.CardCode=@list_of_cols_val_tab_del and T0.GroupCode=100 
			
		if upper(ISNULL(@createusercode,'')) <> upper(@IntegrationSAPUser)
		begin
			set @error=-1
			set @error_message= isnull(@createusercode,'-') + 'YOU CANNOT UPDATE GUEST INFORMATION FROM SAP !!!'
		end
	end
	---------------------------HOTEL MASTER------------------------
	/*Do not allow to delete HOTEL from SAP*/
	if @object_type=N'-3'+CHAR(9)+'@HOTEL'
	begin	
		if @transaction_type='D'
		begin
			set @error=-1
			set @error_message='YOU CANNOT DELETE!!'
		end
	end
	---------------------------FLOOR MASTER------------------------
	/*Do not allow to delete FLOOR from SAP*/
	if @object_type=N'-3'+CHAR(9)+'@FLOOR'
	begin
		if @transaction_type='D'
		begin
			set @error=-1
			set @error_message='YOU CANNOT DELETE!!'
		end
	end
	
	---------------------------ROOM MASTER-------------------------
	/*Do not allow to delete ROOM from SAP*/
	if @object_type=N'-3'+CHAR(9)+'@ROOM'
	begin
		if @transaction_type='D'
		begin
			set @error=-1
			set @error_message='YOU CANNOT DELETE!!'
		end
	end
	---------------------------PAYMENT METHOD-------------------------
	/*Do not allow to delete PAYMENT METHOD from SAP*/
	if @object_type=N'-3'+CHAR(9)+'@PAYMENTMETHOD'
	begin
		if @transaction_type='D'
		begin
			set @error=-1
			set @error_message='YOU CANNOT DELETE!!'
		end
	end
	
	---------------------------EMPLOYEE----------------------------
	if @object_type=N'171'
	begin
		if @transaction_type='U'
		begin
			if isnull((select U_UserName from AHEM where empID=@list_of_cols_val_tab_del
													and LogInstanc=(select max(LogInstanc) from AHEM 
																where empID=@list_of_cols_val_tab_del)),'') <>''
			begin
				if isnull((select U_UserName from OHEM where empID=@list_of_cols_val_tab_del),'') <>
					isnull((select U_UserName from AHEM where empID=@list_of_cols_val_tab_del
														and LogInstanc=(select max(LogInstanc) from AHEM 
																	where empID=@list_of_cols_val_tab_del)),'')
				begin
					set @error=-1
					set @error_message='YOU CANNOT UPDATE USER NAME!!'
				end
			end
		end
	end
	
	---------------------------ITEM----------------------------
	if @object_type=N'4'
	begin
		-------------Service & Inventory Item is not allow-------------
		if (
			select COUNT(*) from OITM T0 join OITB T1 on T0.ItmsGrpCod=T1.ItmsGrpCod
			where T0.ItemCode=@list_of_cols_val_tab_del
				and InvntItem='Y' and upper(T1.ItmsGrpNam) like '%SERVICE%'
				) >=1
		begin
				set @error=-1
				set @error_message='YOU CANNOT SET INVENTORY ITEM FOR SERVICE GROUP!!'
		end
	end
	select @error, @error_message
END                                               
                                              
