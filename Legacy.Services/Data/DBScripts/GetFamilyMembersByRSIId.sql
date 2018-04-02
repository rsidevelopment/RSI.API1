USE [ReservationServicesIntl]
GO

/****** Object: SqlProcedure [dbo].[GetFamilyMembersByRSIId] Script Date: 4/2/2018 3:29:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Dennis
-- Create date: 2018-02-24
-- Description:	
-- =============================================
alter PROCEDURE GetFamilyMembersByRSIId 
	-- Add the parameters for the stored procedure here
	@RSIId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @QUERY AS NVARCHAR(1000);
	DECLARE @FinalQuery AS NVARCHAR(2000);

	SET @QUERY = 'SELECT * FROM `tb_members_authorized_users` where `is_active` > 0 and `id_rsi` = ' + CAST(@RSIId AS VARCHAR(100));
	SET @FinalQuery = 'SELECT * FROM OPENQUERY(CRM, ' + '''' + @QUERY + '''' + ')';
	--PRINT @FinalQuery;
	EXEC(@FinalQuery);
END
