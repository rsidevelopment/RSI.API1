USE [RSI]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[apiInventorySearch] 
	@ResortID INT,
	@StartDate DATETIME = NULL, 
	@EndDate DATETIME = NULL,
	@BedroomSize INT = NULL,
	@InventoryType VARCHAR(50) = NULL,
	@MaximumRSICost money = NULL,
	@StartRowIndex INT = 0,
	@NumberOfRows INT = 10,
	@OrderBy VARCHAR(50) = 'price',
	@OrderDirection varchar(50) = 'asc'

	WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON;
	
	if @MaximumRSICost = 0
	begin
		set @MaximumRSICost = null
	end

    -- Insert statements for procedure here
	;WITH ListEntries AS
		(SELECT ROW_NUMBER() OVER ( ORDER BY 
			CASE @OrderDirection WHEN 'asc' 
				THEN CAST(I.rsicost as money) END,
			CASE @OrderDirection WHEN 'desc' 
				THEN CAST(I.rsicost as money) END DESC,
			CASE @OrderDirection WHEN 'asc' THEN  CASE @OrderBy WHEN 'date' THEN I.fdate END END,
			CASE @OrderDirection WHEN 'desc' THEN CASE @OrderBy WHEN 'date' THEN I.fdate END END DESC )
		as row,
			@ResortID as UnitId,
			I.keyid as InventoryId,
			I.rsicost as NetRate,
			I.fdate as CheckInDate,
			I.tdate as CheckOutDate,
			I.quantity - I.hold as Quantity,
			I.unitsize as UnitSize,
			I.maxguests as MaxGuests,
			I.kitchentype as KitchenType,
			I.adults as Adults,
			[dbo].[CondoCatagory](I.specialinventory) as InventoryType
		  FROM inventory AS I 
		 WHERE I.unitkeyid = @ResortID
		   AND (GETDATE() < I.finish) 
           AND (I.quantity - I.hold > 0)
		   AND I.fdate BETWEEN ISNULL(dbo.MinDate(@StartDate), I.fdate) 
					   AND ISNULL(dbo.MaxDate(@EndDate), I.fdate)
		   AND I.unitsize = ISNULL(@BedroomSize, I.unitsize)
		   AND [dbo].[CondoCatagory](I.specialinventory) = ISNULL(@InventoryType, [dbo].[CondoCatagory](I.specialinventory))
		   AND I.rsicost <= ISNULL(@MaximumRSICost, I.rsicost)
	), RecordCount AS ( SELECT COUNT(*) AS MaxRows FROM ListEntries )

	SELECT * from ListEntries, RecordCount
	 WHERE row  BETWEEN @StartRowIndex AND (@StartRowIndex + @NumberOfRows - 1)
end
