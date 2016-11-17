USE [VIPPARKING]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 11/16/2016 9:17:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Category_ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Category_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Department]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[Dept_ID] [int] IDENTITY(1,1) NOT NULL,
	[Dept_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[Dept_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Events]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[Event_ID] [int] IDENTITY(1,1) NOT NULL,
	[Event_Name] [varchar](255) NOT NULL,
	[Event_Start_Time] [datetime] NOT NULL,
	[Event_End_Time] [datetime] NOT NULL,
	[Event_Spots_Needed] [int] NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[Event_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GateCode]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GateCode](
	[GateCode] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK_GateCode] PRIMARY KEY CLUSTERED 
(
	[GateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Lot]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lot](
	[Lot_ID] [int] IDENTITY(1,1) NOT NULL,
	[Lot_Name] [varchar](50) NOT NULL,
	[Lot_Spaces_Available] [int] NOT NULL,
 CONSTRAINT [PK_Lot] PRIMARY KEY CLUSTERED 
(
	[Lot_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ParkingSpot]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpot](
	[ParkingSpot_ID] [int] IDENTITY(1,1) NOT NULL,
	[ParkingLot_ID] [int] NOT NULL,
	[Location] [varchar](50) NOT NULL,
	[Status] [char](1) NOT NULL,
 CONSTRAINT [PK_ParkingSpot] PRIMARY KEY CLUSTERED 
(
	[ParkingSpot_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Permit]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permit](
	[PermitCode] [int] NOT NULL,
	[Reserv_ID] [int] NOT NULL,
 CONSTRAINT [PK_Permit] PRIMARY KEY CLUSTERED 
(
	[PermitCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Requester]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Requester](
	[Requester_ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[Firstname] [varchar](50) NOT NULL,
	[Lastname] [varchar](50) NOT NULL,
	[Dept_ID] [int] NULL,
	[Email] [varchar](50) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Requester_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Reservation]    Script Date: 11/16/2016 9:17:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservation](
	[Reserv_ID] [int] IDENTITY(1,1) NOT NULL,
	[Requester_ID] [int] NOT NULL,
	[RecipientName] [varchar](50) NOT NULL,
	[NumOfSlots] [int] NOT NULL,
	[RecipientEmail] [varchar](50) NULL,
	[Category_ID] [int] NULL,
	[ParkingSpotID] [int] NULL,
	[Event_ID] [int] NULL,
	[GateCode] [int] NULL,
	[Start_Time] [datetime2](7) NOT NULL,
	[End_Time] [datetime2](7) NOT NULL,
	[Dept_ID] [int] NULL,
	[Approved] [bit] NOT NULL,
	[isWaitingList] [bit] NOT NULL,
 CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED 
(
	[Reserv_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ParkingSpot]  WITH CHECK ADD  CONSTRAINT [FK_ParkingSpot_Lot] FOREIGN KEY([ParkingLot_ID])
REFERENCES [dbo].[Lot] ([Lot_ID])
GO
ALTER TABLE [dbo].[ParkingSpot] CHECK CONSTRAINT [FK_ParkingSpot_Lot]
GO
ALTER TABLE [dbo].[Permit]  WITH CHECK ADD  CONSTRAINT [FK_Permit_Reservation] FOREIGN KEY([Reserv_ID])
REFERENCES [dbo].[Reservation] ([Reserv_ID])
GO
ALTER TABLE [dbo].[Permit] CHECK CONSTRAINT [FK_Permit_Reservation]
GO
ALTER TABLE [dbo].[Requester]  WITH CHECK ADD  CONSTRAINT [FK_Requester_Department] FOREIGN KEY([Dept_ID])
REFERENCES [dbo].[Department] ([Dept_ID])
GO
ALTER TABLE [dbo].[Requester] CHECK CONSTRAINT [FK_Requester_Department]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Category] FOREIGN KEY([Category_ID])
REFERENCES [dbo].[Category] ([Category_ID])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Category]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Department] FOREIGN KEY([Dept_ID])
REFERENCES [dbo].[Department] ([Dept_ID])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Department]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Events] FOREIGN KEY([Event_ID])
REFERENCES [dbo].[Events] ([Event_ID])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Events]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_GateCode] FOREIGN KEY([GateCode])
REFERENCES [dbo].[GateCode] ([GateCode])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_GateCode]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_ParkingSpot] FOREIGN KEY([ParkingSpotID])
REFERENCES [dbo].[ParkingSpot] ([ParkingSpot_ID])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_ParkingSpot]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Requester] FOREIGN KEY([Requester_ID])
REFERENCES [dbo].[Requester] ([Requester_ID])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Requester]
GO
