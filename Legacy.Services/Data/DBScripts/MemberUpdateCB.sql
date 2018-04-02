USE [ReservationServicesIntl]
GO

/****** Object: SqlProcedure [dbo].[MemberUpdateCB] Script Date: 4/2/2018 3:43:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Dennis
-- Create date: 05/16/2016
-- Description:	
-- =============================================
alter PROCEDURE [dbo].[MemberUpdateCB] 
	-- Add the parameters for the stored procedure here
	@RSIId INT, 
	@RSIOrganizationId INT,
	@OrganizationName VARCHAR(100),
	@FirstName VARCHAR(30),
	@MiddleName VARCHAR(40),
	@LastName VARCHAR(50),
	@FirstName2 VARCHAR(30),
	@MiddleName2 VARCHAR(40),
	@LastName2 VARCHAR(50),
	@Address1 VARCHAR(80),
	@Address2 VARCHAR(80),
	@City VARCHAR(80),
	@StateCode VARCHAR(3),
	@PostalCode VARCHAR(12),
	@CountryCode VARCHAR(30),
	@NOTES VARCHAR(128),
	@IsActive BIT,
	@Phone1 VARCHAR(25),
	@Phone2 VARCHAR(25),
	@Email1 VARCHAR(100),
	@Email2 VARCHAR(100),
	@Username VARCHAR(100),
	@Password VARCHAR(100),
	@BirthDate1 DateTime,
	@BirthDate2 DateTime,
	@ExpirationDate DateTime,
	@PROFILEID INT OUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @IsActiveChar CHAR(1);
	DECLARE @Statement as NVARCHAR(4000);
	DECLARE @CBOrgProfileId INT;
	DECLARE @ADDRESSNO INT;
	DECLARE @PHONE1NO INT;
	DECLARE @PHONE2NO INT;
	DECLARE @EMAIL1NO INT;
	DECLARE @EMAIL2NO INT;
	DECLARE @ADDRESSMESSAGE AS VARCHAR(5000);
	DECLARE @ISSUCCESSADDRESS AS BIT;
	DECLARE @COMMUNICATIONMESSAGE AS VARCHAR(5000);
	DECLARE @ISSUCCESSCOMMUNICATIONMESSAGE AS BIT;
    -- Insert statements for procedure here
	IF @IsActive = 1
	BEGIN
		SET @IsActiveChar = 'Y';
	END
	ELSE
	BEGIN
		SET @IsActiveChar = 'N';
	END;

	SET @Phone1 = dbo.FormatPhoneCB(@Phone1);
	SET @Phone2 = dbo.FormatPhoneCB(@Phone2);

	IF @Username IS NULL OR LEN(@Username) < 1
	BEGIN
		SET @Username = CONVERT(VARCHAR(200), @RSIId);
	END;

	SET @Statement = 'SELECT @CBOrgProfileId = PROFILENO FROM OPENQUERY(TRAMS, ''SELECT P.PROFILENO as PROFILENO
				FROM PROFILE P 
				WHERE P.INTERFACEID =  ''''CLUB' + CONVERT(NVARCHAR(200),@RSIOrganizationId) + ''''' ROWS 1'')';

	EXEC sp_executesql @Statement, N'@CBOrgProfileId INT OUTPUT', @CBOrgProfileId OUTPUT;

	IF @CBOrgProfileId IS NULL OR @CBOrgProfileId < 1
	BEGIN
		SET @Statement = 'SELECT @CBOrgProfileId = PRIMARYAGENT_LINKNO  FROM OPENQUERY(TRAMS, ''SELECT P.PRIMARYAGENT_LINKNO as PRIMARYAGENT_LINKNO
			FROM PROFILE P 
			WHERE P.INTERFACEID =  ''''' + CONVERT(NVARCHAR(200),@RSIId) + ''''' ROWS 1'')';

		EXEC sp_executesql @Statement, N'@CBOrgProfileId INT OUTPUT', @CBOrgProfileId OUTPUT;
	END;

	IF @OrganizationName IS NULL OR LEN(@OrganizationName) < 1
	BEGIn
		SET @OrganizationName = (SELECT TOP 1 o.organization FROM [205.153.118.116].[RSI].dbo.organizations o WHERE o.keyid = @RSIOrganizationId);
	END;

	SET @Statement = 'SELECT @PROFILEID = PROFILENO  FROM OPENQUERY(TRAMS, ''SELECT P.PROFILENO as PROFILENO
				FROM PROFILE P 
				WHERE P.INTERFACEID =  ''''' + CONVERT(NVARCHAR(200),@RSIId) + ''''' ROWS 1'')';

	exec sp_executesql @Statement, N'@PROFILEID INT OUTPUT', @PROFILEID OUTPUT;
	--PRINT 'BEFORE IF STATEMENT';
	IF @PROFILEID IS NOT NULL AND @PROFILEID > 0
	BEGIN
		--PRINT' PROFILE IS GREATER THAN 0';
		DECLARE @FullName AS NVARCHAR(1000);

		SET @FullName = dbo.IfNullBlank(@LastName) + '/' + dbo.IfNullBlank(@FirstName);

		IF @MiddleName IS NOT NULL AND LEN(@MiddleName) > 0
		BEGIN
			SET @FullName = @FullName + ' ' + @MiddleName;
		END;

		SET @Statement = N'UPDATE OPENQUERY(TRAMS, ''SELECT * 
			FROM PROFILE WHERE PROFILENO = ''''' + CONVERT(NVARCHAR(200),@PROFILEID) + ''''''') 
			SET NAME = ''' + @FullName + ''', NAMEUPPER = ''' + UPPER(@FullName) + ''', FIRSTNAME = ''' + dbo.IfNullBlank(@FirstName) +
			''', MIDDLEINIT = ''' + dbo.IfNullBlank(@MiddleName) + ''', LASTNAME = ''' + dbo.IfNullBlank(@LastName) + ''', ISACTIVE = ''' + dbo.IfNullBlank(@IsActiveChar) +
			''', PRIMARYAGENT_LINKNO = ''' + CONVERT(NVARCHAR(200),@CBOrgProfileId) + ''', WEBID = ''' + dbo.IfNullBlank(@Username) + ''', WEBPASSWORD = ''' + 
			dbo.IfNullBlank(@Password) + '''';
		EXEC (@Statement);

		EXECUTE [dbo].[CB_AddressAdd] 
			@PROFILEID
			,'NORMAL'
			,@Address1
			,@Address2
			,@City
			,@StateCode
			,@PostalCode
			,@CountryCode
			,@ADDRESSNO OUTPUT
			,@ADDRESSMESSAGE OUTPUT
			,@ISSUCCESSADDRESS OUTPUT;

		EXECUTE [dbo].[CB_CommunicationsAdd] 
			@PROFILEID
			,@Phone1
			,@Phone2
			,@Email1
			,@Email2
			,@PHONE1NO OUTPUT
			,@PHONE2NO OUTPUT
			,@EMAIL1NO OUTPUT
			,@EMAIL2NO OUTPUT
			,@COMMUNICATIONMESSAGE OUTPUT
			,@ISSUCCESSCOMMUNICATIONMESSAGE OUTPUT
			
	END
	ELSE
	BEGIN

		EXECUTE [dbo].[MemberAddCB] 
		   @RSIId
		  ,@RSIOrganizationId
		  ,@FirstName
		  ,@MiddleName
		  ,@LastName
		  ,@Address1
		  ,@Address2
		  ,@City
		  ,@StateCode
		  ,@PostalCode
		  ,@CountryCode
		  ,@NOTES
		  ,@OrganizationName
		  ,@IsActive
		  ,@Phone1
		  ,@Phone2
		  ,@Email1
		  ,@Email2
		  ,@ExpirationDate
		  ,@FirstName2
		  ,@MiddleName2
		  ,@LastName2
		  ,@Password
		  ,@BirthDate1
		  ,@BirthDate2
		  ,@PROFILEID OUTPUT;
	END;
END
