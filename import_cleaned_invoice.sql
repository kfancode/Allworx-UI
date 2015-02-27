USE [AllworxCallActivity]
GO

/****** Object:  Table [dbo].[call_import]    Script Date: 2/16/2015 1:48:28 PM ******/
DROP TABLE [dbo].[chargedcalls]
GO

/****** Object:  Table [dbo].[incomingcalls]    Script Date: 2/16/2015 1:48:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[chargedcalls](
	[calldate] [datetime] NOT NULL,
	[calltime] [datetime] NOT NULL,
	[calltype] [nvarchar] (50) NOT NULL,
	[placedto] [nvarchar] (50) NOT NULL,
	[placedfrom] [nvarchar] (50) NOT NULL,
	[duration] [float]  NOT NULL,
	[rate] [nvarchar] (50) NOT NULL,
	[charge] [nvarchar] (50) NOT NULL,
	[acctcode] [nvarchar] (50) NOT NULL
	)
	
	

GO
bulk insert [dbo].[chargedcalls]
from 'c:\imports\charged.csv'
with
(
	fieldterminator = ',',
	rowterminator = '\n'
)

GO


