
ALTER proc [dbo].[SBO_SP_TransactionNotification] 

@object_type nvarchar(20), 				-- SBO Object Type
@transaction_type nchar(1),			-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
@num_of_cols_in_key int,
@list_of_key_cols_tab_del nvarchar(255),
@list_of_cols_val_tab_del nvarchar(255)

AS

begin

-- Return values
declare @error  int				-- Result (0 for no error)
declare @error_message nvarchar (200) 		-- Error string to be displayed
select @error = 0
select @error_message = N'Ok'

--------------------------------------------------------------------------------------------------------------------------------

/*Start: Customize for SAP*/
declare @IntegrationDBName nvarchar(100)
Exec sp_AI_TransactionNotification_Customize
							                   @object_type =  @object_type, 				
                                               @transaction_type = @transaction_type ,			
                                               @num_of_cols_in_key = @num_of_cols_in_key,
                                               @list_of_key_cols_tab_del = @list_of_key_cols_tab_del,
                                               @list_of_cols_val_tab_del =@list_of_cols_val_tab_del,
							                   @error = @error OUTPUT,
							                   @error_message = @error_message OUTPUT
/*End: Customize for SAP*/

--------------------------------------------------------------------------------------------------------------------------------

--------------------------------------------------------------------------------------------------------------------------------

/*Start: Integration SAP-POS*/
Exec sp_AI_TransactionNotification_Integration_eNoah
							                   @object_type =  @object_type, 				
                                               @transaction_type = @transaction_type ,			
                                               @num_of_cols_in_key = @num_of_cols_in_key,
                                               @list_of_key_cols_tab_del = @list_of_key_cols_tab_del,
                                               @list_of_cols_val_tab_del =@list_of_cols_val_tab_del,
							                   @error = @error OUTPUT,
							                   @error_message = @error_message OUTPUT
/*End: Integration SAP-POS*/

--------------------------------------------------------------------------------------------------------------------------------
--set @error=-1
--	set @error_message=@object_type
-- Select the return values
select @error, @error_message

end
