USE [RSI]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[apiBookInventory] 
	@InventoryID INT,
	@BillingID INT = NULL,
FirstName
MiddleName
LastName
Address
	@CountryCode VARCHAR(10) = NULL,
	@StateCode VARCHAR(10) = NULL,
	@City VARCHAR(100) = NULL,
Zip
Phone1
Ext1
Phone2
Ext2
Email
Price
Comments

	WITH RECOMPILE
AS
BEGIN
	INSERT INTO reservations (
		keyid, billingkeyid, unitkeyid, creatorid, creationdate, , 
		--purchaser
		fname, mi, lname, address, city, state, zip, country, phone1, ext1, phone2, ext2, email, 
		--inventory
		ownerid, startdate, enddate, los, size, sleeps, cost, price, origionalid, hw, 
		specialinventory, discount, inventorytype,

		--unknown mapping
		rewardsamt, overage, org, transactionnumber, refnum, inventorytbl, program, checked, 
		error, comments, 
		--seem unused
		tickets, realtime, resell, customercostpernight, retail, dt1, dt2, dt3, InventoryTable, ContactInfoKeyID, agentID, xmlSend, xmlReceive
	)
	VALUES (
		--inventory
		ownerid, fdate, tdate, los, unitsize, maxguests, cost, price, origionalid, hw, 
		specialinventory, discount, inventorytype, 
	)
END
