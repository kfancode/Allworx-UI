USE [AllworxCallActivity]
GO

/****** Object:  Table [dbo].[call_import]    Script Date: 2/16/2015 1:48:28 PM ******/
DROP TABLE [dbo].[outgoingcalls]
GO

/****** Object:  Table [dbo].[outgoingcalls]    Script Date: 2/16/2015 1:48:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[outgoingcalls](
	[duration] [nvarchar] (50) NOT NULL,
	[calltype] [nvarchar] (50) NOT NULL,
	[placedfrom] [nvarchar] (max) NOT NULL,
	[placedto] [nvarchar] (max) NOT NULL)

GO
bulk insert [dbo].[outgoingcalls]
from 'c:\imports\outgoing.csv'
with
(
	fieldterminator = ',',
	rowterminator = '\n'
)

GO


