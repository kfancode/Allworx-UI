USE [AllworxCallActivity]
GO

/****** Object:  Table [dbo].[call_import]    Script Date: 2/16/2015 1:48:28 PM ******/
DROP TABLE [dbo].[incomingcalls]
GO

/****** Object:  Table [dbo].[incomingcalls]    Script Date: 2/16/2015 1:48:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[incomingcalls](
	[duration] [nvarchar] (50) NOT NULL,
	[calltype] [nvarchar] (50) NOT NULL,
	[placedfrom] [nvarchar] (50) NOT NULL,
	[placedto] [nvarchar] (50) NOT NULL)

GO
bulk insert [dbo].[incomingcalls]
from 'c:\imports\incoming.csv'
with
(
	fieldterminator = '|',
	rowterminator = '\n'
)

GO


