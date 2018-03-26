USE [RSI]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[apiUnitSearch] 
	@OwnerType INT = NULL,
	@InventoryID INT = NULL,
	@ResortID INT = NULL,
	@StartDate DATETIME = NULL, 
	@EndDate DATETIME = NULL,
	@CountryCode VARCHAR(10) = NULL,
	@StateCode VARCHAR(10) = NULL,
	@City VARCHAR(100) = NULL,
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
	
	if @InventoryID IS NOT NULL AND @InventoryID > 0
	begin
		set @ResortID = null
		set @CountryCode = null
		set @StateCode = null
		set @City = null
	end
	else if @ResortID IS NOT NULL AND @ResortID > 0
	begin
		set @CountryCode = null
		set @StateCode = null
		set @City = null
	end
	
	if @MaximumRSICost = 0
	begin
		set @MaximumRSICost = null
	end

    -- Insert statements for procedure here
	;WITH ListEntries AS
		(SELECT ROW_NUMBER() OVER ( ORDER BY 
			CASE @OrderDirection WHEN 'asc' 
				THEN CAST(dbo.udfMinRSICost(U.keyid, @StartDate, @EndDate, @BedroomSize, @InventoryType) as money) END,
			CASE @OrderDirection WHEN 'desc' 
				THEN CAST(dbo.udfMinRSICost(U.keyid, @StartDate, @EndDate, @BedroomSize, @InventoryType) as money) END DESC,
			CASE @OrderDirection WHEN 'asc' THEN  CASE @OrderBy WHEN 'name' THEN U.name END END,
			CASE @OrderDirection WHEN 'desc' THEN CASE @OrderBy WHEN 'name' THEN U.name END END DESC )
		as row,

		U.keyid as unitID,
		U.ownerid as ownerID,
		U.thumbId as imageID,
		U.name as unitName,
		U.address, 
		U.city, 
		U.state AS stateCode,
		CASE WHEN S.ref IS NOT NULL THEN CAST(S.v AS VARCHAR(255)) ELSE U.state END AS stateFullName,
		U.zip AS postalCode, 
		U.country AS countryCode, 
		CASE WHEN C.ref IS NOT NULL THEN CAST(C.v AS VARCHAR(255)) ELSE U.country END AS countryFullName, 
		CAST(U.info AS VARCHAR(MAX)) AS description,
		dbo.udfMinRSICost(U.keyid, @StartDate, @EndDate, @BedroomSize, @InventoryType) as lowest

	FROM units AS U INNER JOIN
		 translator C ON U.country = C.ref AND C.language = 'EN' AND C.type = 'COUNTRY' LEFT OUTER JOIN
		 translator S ON U.state = S.ref AND S.language = 'EN' AND S.type = '_S_' + U.country

	WHERE EXISTS (
		SELECT * 
		  FROM inventory AS I INNER JOIN
			   translator RS ON I.unitsize = RS.ref AND RS.language = 'EN' AND RS.type = 'RS' 
		 WHERE I.unitkeyid = U.keyid
		   AND I.keyid = ISNULL(@InventoryID, I.keyid)
		   AND (GETDATE() < I.finish) 
           AND (I.quantity - I.hold > 0)
		   AND I.fdate BETWEEN ISNULL(dbo.MinDate(@StartDate), I.fdate) 
					   AND ISNULL(dbo.MaxDate(@EndDate), I.fdate)
		   AND I.unitsize = ISNULL(@BedroomSize, I.unitsize)
		   AND [dbo].[CondoCatagory](I.specialinventory) = ISNULL(@InventoryType, [dbo].[CondoCatagory](I.specialinventory))
		   AND I.rsicost <= ISNULL(@MaximumRSICost, I.rsicost)
		)
		AND U.keyid = ISNULL(@ResortID, U.keyid)
		AND ((@OwnerType IS NULL) OR 
			 (@OwnerType=1 AND U.ownerid = 100) OR
			 (@OwnerType=2 AND U.ownerid <> 100))
		AND U.country = ISNULL(@CountryCode, U.country)
		AND U.state = ISNULL(@StateCode, U.state)
		AND LTRIM(RTRIM(U.city)) = ISNULL(@City, LTRIM(RTRIM(U.city)))
	), RecordCount AS ( SELECT COUNT(*) AS MaxRows FROM ListEntries )

	SELECT * from ListEntries, RecordCount
	 WHERE row  BETWEEN @StartRowIndex AND (@StartRowIndex + @NumberOfRows - 1)
end
