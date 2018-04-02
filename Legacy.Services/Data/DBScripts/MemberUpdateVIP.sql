USE [ReservationServicesIntl]
GO

/****** Object: SqlProcedure [dbo].[MemberUpdateVIP] Script Date: 4/2/2018 3:32:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Dennis
-- Create date: 05/19/2016
-- Description:	
-- =============================================
alter PROCEDURE [dbo].[MemberUpdateVIP]
	-- Add the parameters for the stored procedure here
	@RSIId INT, 
	@RSIOrganizationId INT = NULL,
	@DistributorId INT = NULL,
	@PackageId INT = NULL,
	@AffiliateId INT = NULL,
	@RSICreatorId INT = NULL,
	@ClubReference VARCHAR(100) = NULL,
	@CreatorIP VARCHAR(50) = NULL,
	@Username VARCHAR(32) = NULL,
	@Password VARCHAR(32) = NULL,
	@FirstName VARCHAR(100) = NULL,
	@MiddleName VARCHAR(100) = NULL,
	@LastName VARCHAR(255) = NULL,
	@FirstName2 VARCHAR(100) = NULL,
	@MiddleName2 VARCHAR(100) = NULL,
	@LastName2 VARCHAR(255) = NULL,
	@Family VARCHAR(2000) = NULL,
	@Phone1 VARCHAR(32) = NULL,
	@Phone2 VARCHAR(32) = NULL,
	@Email1 VARCHAR(100) = NULL,
	@Email2 VARCHAR(100) = NULL,
	@Address1 VARCHAR(255) = NULL,
	@Address2 VARCHAR(255) = NULL,
	@City VARCHAR(100) = NULL,
	@StateCode VARCHAR(50) = NULL,
	@PostalCode VARCHAR(50) = NULL,
	@CountryCode VARCHAR(50) = NULL,
	@CondoRewards MONEY = NULL,
	@CruiseRewards MONEY = NULL,
	@HotelRewards MONEY = NULL,
	@SalesAmount MONEY = NULL,
	@Note VARCHAR(2000) = NULL,
	@BlockedReason VARCHAR(100) = NULL,
	@SalesDate DateTime = NULL,
	@DateOfBirth DateTime = NULL,
	@BlockedDate DateTime = NULL,
	@ExpirationDate DateTime = NULL,
	@IsEmailOptOut BIT = NULL,
	@IsActive BIT = NULL,
	@IsGuest BIT = NULL,
	@IsComped BIT = NULL

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @BlockedFlag AS BIT;
	DECLARE @CurrentValue AS VARCHAR(100);
	DECLARE @UserNameTMP AS VARCHAR(100);
	DECLARE @TSQLCRM AS NVARCHAR(4000);
	DECLARE @CondoRewardsTMP AS VARCHAR(50);

	if @IsActive IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueVIP] @Id = @RSIId,
		@CurrentFieldName = N'blockedFlag',
		@Table = N'members',
		@TablePrimaryKeyName = N'RSIID',
		@CurrentValue = @CurrentValue OUTPUT
		IF @CurrentValue = 1
		BEGIN
			SET @IsActive = 0;
		END
		ELSE
		BEGIN
			SET @IsActive = 1;
		END;
	END;

	IF @SalesDate IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueVIP] @Id = @RSIId,
		@CurrentFieldName = N'salesDate',
		@Table = N'members',
		@TablePrimaryKeyName = N'RSIID',
		@CurrentValue = @CurrentValue OUTPUT
		SET @SalesDate = @CurrentValue;
	END;

	IF @PackageId IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueVIP] @Id = @RSIId,
		@CurrentFieldName = N'packagesIDFK',
		@Table = N'members',
		@TablePrimaryKeyName = N'RSIID',
		@CurrentValue = @CurrentValue OUTPUT
		SET @PackageId = @CurrentValue;
	END;

	IF @ExpirationDate IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueVIP] @Id = @RSIId,
		@CurrentFieldName = N'expiryDate',
		@Table = N'members',
		@TablePrimaryKeyName = N'RSIID',
		@CurrentValue = @CurrentValue OUTPUT
		SET @ExpirationDate = @CurrentValue;
	END;

	IF @Username IS NOT NULL AND LEN(@Username) > 0
	BEGIN
		SET @UsernameTMP = @Username;
	END
	ELSE
	BEGIN
		SET @UsernameTMP = @RSIId;
	END;

	SET @TSQLCRM = 'CALL add_update_member(''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSIId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSIOrganizationId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@DistributorId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@PackageId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@AffiliateId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSICreatorId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@ClubReference));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@CreatorIP));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@UsernameTMP);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Password);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@FirstName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@MiddleName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@LastName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@FirstName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@MiddleName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@LastName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Family);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Phone1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Phone2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Email1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Email2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Address1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Address2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@City);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@StateCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@PostalCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@CountryCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@CondoRewardsTMP);
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@CruiseRewards));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@HotelRewards));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@SalesAmount));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Note);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@BlockedReason);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@SalesDate));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@DateOfBirth));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@BlockedDate));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@ExpirationDate));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsEmailOptOut));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsActive));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsGuest));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsComped));
	SET @TSQLCRM = @TSQLCRM + ''');';

	EXEC(@TSQLCRM) AT VIP;
END
