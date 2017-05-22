
/****** Object:  UserDefinedFunction [dbo].[Web_getAvgReq]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION  [dbo].[Web_getAvgReq]     
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5), @days as int )

RETURNS numeric(19,6)
AS    
begin
RETURN (
select isnull(
(select 
sum(Quantity) / @days

from OINV t0 inner join INV1 t1 on t0.DocEntry = t1.DocEntry
inner join OITM t2 on t1.ItemCode = t2.ItemCode
where t0.isIns = 'Y' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet
and DATEDIFF(d, t0.docdate, convert(date,getdate())) <= @days


 ),0)*-1)
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getARRINVComitmt]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION  [dbo].[Web_getARRINVComitmt]     
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5) )

RETURNS numeric(19,6)
AS    
begin
RETURN (
isnull (
(select 
sum(Quantity)

from OINV t0 inner join INV1 t1 on t0.DocEntry = t1.DocEntry
inner join OITM t2 on t1.ItemCode = t2.ItemCode
where t0.isIns = 'Y' and t0.InvntSttus = 'O' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet


),0))
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getARResINVComitmt]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION  [dbo].[Web_getARResINVComitmt]     
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5) )

RETURNS numeric(19,6)
AS    
begin
RETURN (
isnull (
(select 
sum(Quantity)

from OINV t0 inner join INV1 t1 on t0.DocEntry = t1.DocEntry
inner join OITM t2 on t1.ItemCode = t2.ItemCode
where t0.isIns = 'Y' and t0.InvntSttus = 'O' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet


),0))
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getToleranceLevel]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[Web_getToleranceLevel]
( @itemcode as nvarchar(20)
--, @dailyAvg as numeric(19,6)
, @recommendedQty AS numeric(19,6) 
, @orderedQty as numeric(19,6)  )
RETURNS int
AS    
begin

RETURN (

select 

case 
when @recommendedQty <= 0 then 2
when (@orderedQty - @recommendedQty) / @recommendedQty > T1.U_ToleranceLvl2 then 2
when (@orderedQty - @recommendedQty) / @recommendedQty > T1.U_ToleranceLvl1 then 1
else 0
end

from OITM T0 INNER JOIN OITB T1 ON T0.ItmsGrpCod = T1.ItmsGrpCod 
where T0.ItemCode = @itemcode
 )
 
end
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getSOComitmt]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[Web_getSOComitmt]
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5), @daysdue as int )
RETURNS numeric(19,6)
AS    
begin
RETURN (
select isnull(
(select 


sum(openqty)
from ORDR t0 inner join RDR1 t1 on t0.DocEntry = t1.DocEntry
where t0.DocStatus= 'O' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet
and DATEDIFF(d, t0.DocDueDate, convert(date,getdate())) <= @daysdue



 ),0)*-1)
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getPOQty]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[Web_getPOQty]
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5) )
RETURNS numeric(19,6)
AS    
begin
RETURN (
select isnull(
(select 
sum(openqty)
from OPOR t0 inner join POR1 t1 on t0.DocEntry = t1.DocEntry
where t0.DocStatus= 'O' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet
 ),0) +

ISNULL(
(select

SUM(openqty)
from owtq t0 inner join WTQ1 t1 on t0.DocEntry = t1.DocEntry
where t0.DocStatus= 'O' and t0.CANCELED = 'N' and
t1.ItemCode = @itemcode and t1.WhsCode = @outlet

 ),0))
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getMinimumStock]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[Web_getMinimumStock]
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5) )
RETURNS numeric(19,6)
AS    
begin
RETURN (
select isnull(
(select 
oitw.MinStock
from OITW where ItemCode = @itemcode and WhsCode = @outlet
 ),0))
end;
GO
/****** Object:  UserDefinedFunction [dbo].[Web_getInStock]    Script Date: 01/09/2015 18:00:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION  [dbo].[Web_getInStock]     
( @itemcode AS nvarchar(20) , @outlet as nvarchar(5) )

RETURNS numeric(19,6)
AS    
begin
RETURN (
isnull(
(select 
(t1.OnHand/t2.numinbuy - ISNULL(dbo.Web_getARResINVComitmt ( @itemcode, @outlet),0))

from OITW t1 inner join OITM t2 on t1.ItemCode=t2.ItemCode
where t1.ItemCode = @itemcode and t1.WhsCode = @outlet
),0))
end;
GO
