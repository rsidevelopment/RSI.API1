﻿USE [HangFire]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RSIJobData]
(
	[jobId] INT NOT NULL IDENTITY (1,1) PRIMARY KEY,
    [data] VARCHAR(MAX) NOT NULL
)