USE [RSI]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[udfMinRSICost] 
(
	-- Add the parameters for the function here
	@ResortID int,
	@StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
	@BedroomSize INT = NULL,
    @InventoryType VARCHAR(50) = NULL
)
RETURNS money
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result money

	SET @Result = (
		SELECT MIN(rsicost)
		  FROM inventory AS I INNER JOIN
			   translator RS ON I.unitsize = RS.ref AND RS.language = 'EN' AND RS.type = 'RS' 
		 WHERE I.unitkeyid = @ResortID
		   AND (GETDATE() < I.finish) 
           AND (I.quantity - I.hold > 0)
		   AND I.fdate BETWEEN ISNULL(dbo.MinDate(@StartDate), I.fdate) 
					   AND ISNULL(dbo.MaxDate(@EndDate), I.fdate)
		   AND I.unitsize = ISNULL(@BedroomSize, I.unitsize)
		   AND [dbo].[CondoCatagory](I.specialinventory) = ISNULL(@InventoryType, [dbo].[CondoCatagory](I.specialinventory))
		)
	RETURN @Result
END