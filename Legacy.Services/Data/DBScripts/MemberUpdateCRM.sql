USE [ReservationServicesIntl]
GO

/****** Object: SqlProcedure [dbo].[MemberUpdateCRM] Script Date: 4/2/2018 3:42:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Dennis
-- Create date: 05/13/2016
-- Description:	
-- =============================================
alter PROCEDURE [dbo].[MemberUpdateCRM] 
	-- Add the parameters for the stored procedure here
	@RSIId INT,
	@RSIModifierId INT = NULL,
	@RSIOrganizationId INT = NULL,
	@PackageId INT = NULL,
	@CreatorIP VARCHAR(50) = NULL,
	@Username VARCHAR(100) = NULL,
	@Password VARCHAR(100) = NULL,
	@Title VARCHAR(50) = NULL,
	@FirstName VARCHAR(100) = NULL,
	@MiddleName VARCHAR(50) = NULL,
	@LastName VARCHAR(255) = NULL,
	@FirstName2 VARCHAR(100) = NULL,
	@MiddleName2 VARCHAR(50) = NULL,
	@LastName2 VARCHAR(255) = NULL,
	@Family VARCHAR(2555) = NULL,
	@Address1 VARCHAR(255) = NULL,
	@Address2 VARCHAR(255) = NULL,
	@City VARCHAR(100) = NULL,
	@StateCode VARCHAR(50) = NULL,
	@PostalCode VARCHAR(50) = NULL,
	@CountryCode VARCHAR(50) = NULL,
	@Phone1 VARCHAR(50) = NULL,
	@Phone2 VARCHAR(50) = NULL,
	@Email1 VARCHAR(100) = NULL,
	@BlockedReason VARCHAR(100) = NULL,
	@CondoRewards MONEY = NULL,
	@CruiseRewards MONEY = NULL,
	@HotelRewards MONEY = NULL,
	@UnlimitedRewards bit = NULL,
	@SalesDate DATETIME = NULL,
	@BlockedDate DATETIME = NULL,
	@DateOfBirth DATETIME = NULL,
	@ExpirationDate DATETIME = NULL,
	@IsActive bit = NULL,
	@IsMilitary bit = NULL

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @TSQLCRM AS NVARCHAR(4000);
	DECLARE @CondoRewardsTMP AS VARCHAR(50);
	DECLARE @UsernameTMP AS VARCHAR(100);
	DECLARE @CurrentValue AS VARCHAR(100);
	
	IF @RSIModifierId IS NULL
	BEGIN
		SET @RSIModifierId = 1;
	END;

	IF @RSIOrganizationId IS NULL
	BEGIN
		SET @RSIOrganizationId = 0;
	END;

	if @IsActive IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueCRM] @Id = @RSIId,
		@CurrentFieldName = N'is_active',
		@Table = N'tb_members',
		@TablePrimaryKeyName = N'id_rsi',
		@CurrentValue = @CurrentValue OUTPUT
		SET @IsActive = @CurrentValue;
	END;

	if @IsMilitary IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueCRM] @Id = @RSIId,
		@CurrentFieldName = N'is_military',
		@Table = N'tb_members',
		@TablePrimaryKeyName = N'id_rsi',
		@CurrentValue = @CurrentValue OUTPUT
		SET @IsMilitary = @CurrentValue;
	END;

	IF @SalesDate IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueCRM] @Id = @RSIId,
		@CurrentFieldName = N'sales_date',
		@Table = N'tb_members',
		@TablePrimaryKeyName = N'id_rsi',
		@CurrentValue = @CurrentValue OUTPUT
		SET @SalesDate = @CurrentValue;
	END;

	IF @PackageId IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueCRM] @Id = @RSIId,
		@CurrentFieldName = N'id_package',
		@Table = N'tb_members',
		@TablePrimaryKeyName = N'id_rsi',
		@CurrentValue = @CurrentValue OUTPUT
		SET @PackageId = @CurrentValue;
	END;

	IF @ExpirationDate IS NULL
	BEGIN
		exec [dbo].[GetCurrentFieldValueCRM] @Id = @RSIId,
		@CurrentFieldName = N'expiration_date',
		@Table = N'tb_members',
		@TablePrimaryKeyName = N'id_rsi',
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

	IF @UnlimitedRewards = 1 
	BEGIN
		SET @CondoRewardsTMP = 'U';
	END
	ELSE
	BEGIN
		SET @CondoRewardsTMP = @CondoRewards;
	END;

    -- Insert statements for procedure here
	SET @TSQLCRM = 'CALL add_update_member(''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSIId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSIModifierId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@RSIOrganizationId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@PackageId));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@CreatorIP);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@UsernameTMP);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Password);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Title); 
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@FirstName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@MiddleName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@LastName);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@FirstName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@MiddleName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@LastName2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Family);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Address1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Address2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@City);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@StateCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@PostalCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@CountryCode);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Phone1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Phone2);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@Email1);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@BlockedReason);
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@SalesDate));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@ExpirationDate));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(dbo.DateToMySQLFormat(@DateOfBirth));
	SET @TSQLCRM = @TSQLCRM + ''',''' + dbo.IfNullBlank(@CondoRewardsTMP);
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@CruiseRewards));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@HotelRewards));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@UnlimitedRewards));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsMilitary));
	SET @TSQLCRM = @TSQLCRM + ''',''' + CONVERT(NVARCHAR(100), dbo.IfNullBlank(@IsActive));
	SET @TSQLCRM = @TSQLCRM + ''');';
	--PRINT @TSQLCRM;
	EXEC(@TSQLCRM) AT CRM;
END
