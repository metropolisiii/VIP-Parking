
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/05/2016 19:04:46
-- Generated from EDMX file: C:\Users\midtown\Source\Repos\RU-CCIS-DB-Practicum-VIP-Parking-Project-Fall-2016\VIP_Parking\VIP_Parking\Models\Database\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [VIPPARKINGEntities1];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[VIPPARKINGModelStoreContainer].[FK_AllowedLots_Lot]', 'F') IS NOT NULL
    ALTER TABLE [VIPPARKINGModelStoreContainer].[AllowedLots] DROP CONSTRAINT [FK_AllowedLots_Lot];
GO
IF OBJECT_ID(N'[VIPPARKINGModelStoreContainer].[FK_AllowedLots_Reservation]', 'F') IS NOT NULL
    ALTER TABLE [VIPPARKINGModelStoreContainer].[AllowedLots] DROP CONSTRAINT [FK_AllowedLots_Reservation];
GO
IF OBJECT_ID(N'[dbo].[FK_History_Reservation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[History] DROP CONSTRAINT [FK_History_Reservation];
GO
IF OBJECT_ID(N'[dbo].[FK_ParkingSpot_Lot]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParkingSpot] DROP CONSTRAINT [FK_ParkingSpot_Lot];
GO
IF OBJECT_ID(N'[dbo].[FK_Permit_Reservation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Permit] DROP CONSTRAINT [FK_Permit_Reservation];
GO
IF OBJECT_ID(N'[dbo].[FK_Requester_Department]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Requester] DROP CONSTRAINT [FK_Requester_Department];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_Department]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_Department];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_Events]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_Events];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_GateCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_GateCode];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_ParkingSpot]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_ParkingSpot];
GO
IF OBJECT_ID(N'[dbo].[FK_Reservation_Requester]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reservation] DROP CONSTRAINT [FK_Reservation_Requester];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[Department]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Department];
GO
IF OBJECT_ID(N'[dbo].[Events]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Events];
GO
IF OBJECT_ID(N'[dbo].[GateCode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GateCode];
GO
IF OBJECT_ID(N'[dbo].[History]', 'U') IS NOT NULL
    DROP TABLE [dbo].[History];
GO
IF OBJECT_ID(N'[dbo].[Lot]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Lot];
GO
IF OBJECT_ID(N'[dbo].[ParkingSpot]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParkingSpot];
GO
IF OBJECT_ID(N'[dbo].[Permit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Permit];
GO
IF OBJECT_ID(N'[dbo].[Requester]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Requester];
GO
IF OBJECT_ID(N'[dbo].[Reservation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reservation];
GO
IF OBJECT_ID(N'[VIPPARKINGModelStoreContainer].[AllowedLots]', 'U') IS NOT NULL
    DROP TABLE [VIPPARKINGModelStoreContainer].[AllowedLots];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [Category_ID] int IDENTITY(1,1) NOT NULL,
    [Title] varchar(50)  NOT NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [Dept_ID] int IDENTITY(1,1) NOT NULL,
    [Dept_name] varchar(50)  NOT NULL
);
GO

-- Creating table 'Events'
CREATE TABLE [dbo].[Events] (
    [Event_ID] int IDENTITY(1,1) NOT NULL,
    [Event_Name] varchar(255)  NOT NULL,
    [Event_Start_Time] datetime  NOT NULL,
    [Event_End_Time] datetime  NOT NULL,
    [Event_Spots_Needed] int  NOT NULL
);
GO

-- Creating table 'GateCodes'
CREATE TABLE [dbo].[GateCodes] (
    [GateCode1] int  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL
);
GO

-- Creating table 'Histories'
CREATE TABLE [dbo].[Histories] (
    [ItemID] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [Action] varchar(50)  NOT NULL,
    [Reserv_ID] int  NULL
);
GO

-- Creating table 'Lots'
CREATE TABLE [dbo].[Lots] (
    [Lot_ID] int IDENTITY(1,1) NOT NULL,
    [Lot_Name] varchar(50)  NOT NULL,
    [Lot_Spaces_Available] int  NOT NULL
);
GO

-- Creating table 'ParkingSpots'
CREATE TABLE [dbo].[ParkingSpots] (
    [ParkingSpot_ID] int IDENTITY(1,1) NOT NULL,
    [ParkingLot_ID] int  NOT NULL,
    [Location] varchar(50)  NOT NULL,
    [Status] char(1)  NOT NULL
);
GO

-- Creating table 'Permits'
CREATE TABLE [dbo].[Permits] (
    [PermitCode] bigint  NOT NULL,
    [Reserv_ID] int  NOT NULL
);
GO

-- Creating table 'Requesters'
CREATE TABLE [dbo].[Requesters] (
    [Requester_ID] int IDENTITY(1,1) NOT NULL,
    [Username] varchar(50)  NULL,
    [Password] varchar(50)  NULL,
    [Firstname] varchar(50)  NOT NULL,
    [Lastname] varchar(50)  NOT NULL,
    [Dept_ID] int  NULL,
    [Email] varchar(50)  NOT NULL,
    [IsAdmin] bit  NOT NULL,
    [Fullname] varchar(100)  NULL,
    [IsLocked] bit  NOT NULL
);
GO

-- Creating table 'Reservations'
CREATE TABLE [dbo].[Reservations] (
    [Reserv_ID] int IDENTITY(1,1) NOT NULL,
    [Requester_ID] int  NOT NULL,
    [RecipientName] varchar(50)  NOT NULL,
    [NumOfSlots] int  NOT NULL,
    [RecipientEmail] varchar(50)  NULL,
    [Category_ID] int  NULL,
    [ParkingSpotID] int  NULL,
    [Event_ID] int  NULL,
    [GateCode] int  NULL,
    [Start_Time] datetime  NOT NULL,
    [End_Time] datetime  NOT NULL,
    [Dept_ID] int  NULL,
    [Approved] tinyint  NOT NULL,
    [isWaitingList] bit  NOT NULL,
    [RequesterEmail] varchar(50)  NOT NULL,
    [CreationDate] datetime  NOT NULL
);
GO

-- Creating table 'AllowedLots'
CREATE TABLE [dbo].[AllowedLots] (
    [Lots_Lot_ID] int  NOT NULL,
    [Reservations_Reserv_ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Category_ID] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([Category_ID] ASC);
GO

-- Creating primary key on [Dept_ID] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([Dept_ID] ASC);
GO

-- Creating primary key on [Event_ID] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [PK_Events]
    PRIMARY KEY CLUSTERED ([Event_ID] ASC);
GO

-- Creating primary key on [GateCode1] in table 'GateCodes'
ALTER TABLE [dbo].[GateCodes]
ADD CONSTRAINT [PK_GateCodes]
    PRIMARY KEY CLUSTERED ([GateCode1] ASC);
GO

-- Creating primary key on [ItemID] in table 'Histories'
ALTER TABLE [dbo].[Histories]
ADD CONSTRAINT [PK_Histories]
    PRIMARY KEY CLUSTERED ([ItemID] ASC);
GO

-- Creating primary key on [Lot_ID] in table 'Lots'
ALTER TABLE [dbo].[Lots]
ADD CONSTRAINT [PK_Lots]
    PRIMARY KEY CLUSTERED ([Lot_ID] ASC);
GO

-- Creating primary key on [ParkingSpot_ID] in table 'ParkingSpots'
ALTER TABLE [dbo].[ParkingSpots]
ADD CONSTRAINT [PK_ParkingSpots]
    PRIMARY KEY CLUSTERED ([ParkingSpot_ID] ASC);
GO

-- Creating primary key on [PermitCode] in table 'Permits'
ALTER TABLE [dbo].[Permits]
ADD CONSTRAINT [PK_Permits]
    PRIMARY KEY CLUSTERED ([PermitCode] ASC);
GO

-- Creating primary key on [Requester_ID] in table 'Requesters'
ALTER TABLE [dbo].[Requesters]
ADD CONSTRAINT [PK_Requesters]
    PRIMARY KEY CLUSTERED ([Requester_ID] ASC);
GO

-- Creating primary key on [Reserv_ID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [PK_Reservations]
    PRIMARY KEY CLUSTERED ([Reserv_ID] ASC);
GO

-- Creating primary key on [Lots_Lot_ID], [Reservations_Reserv_ID] in table 'AllowedLots'
ALTER TABLE [dbo].[AllowedLots]
ADD CONSTRAINT [PK_AllowedLots]
    PRIMARY KEY CLUSTERED ([Lots_Lot_ID], [Reservations_Reserv_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Category_ID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_Category]
    FOREIGN KEY ([Category_ID])
    REFERENCES [dbo].[Categories]
        ([Category_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_Category'
CREATE INDEX [IX_FK_Reservation_Category]
ON [dbo].[Reservations]
    ([Category_ID]);
GO

-- Creating foreign key on [Dept_ID] in table 'Requesters'
ALTER TABLE [dbo].[Requesters]
ADD CONSTRAINT [FK_Requester_Department]
    FOREIGN KEY ([Dept_ID])
    REFERENCES [dbo].[Departments]
        ([Dept_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Requester_Department'
CREATE INDEX [IX_FK_Requester_Department]
ON [dbo].[Requesters]
    ([Dept_ID]);
GO

-- Creating foreign key on [Dept_ID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_Department]
    FOREIGN KEY ([Dept_ID])
    REFERENCES [dbo].[Departments]
        ([Dept_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_Department'
CREATE INDEX [IX_FK_Reservation_Department]
ON [dbo].[Reservations]
    ([Dept_ID]);
GO

-- Creating foreign key on [Event_ID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_Events]
    FOREIGN KEY ([Event_ID])
    REFERENCES [dbo].[Events]
        ([Event_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_Events'
CREATE INDEX [IX_FK_Reservation_Events]
ON [dbo].[Reservations]
    ([Event_ID]);
GO

-- Creating foreign key on [GateCode] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_GateCode]
    FOREIGN KEY ([GateCode])
    REFERENCES [dbo].[GateCodes]
        ([GateCode1])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_GateCode'
CREATE INDEX [IX_FK_Reservation_GateCode]
ON [dbo].[Reservations]
    ([GateCode]);
GO

-- Creating foreign key on [Reserv_ID] in table 'Histories'
ALTER TABLE [dbo].[Histories]
ADD CONSTRAINT [FK_History_Reservation]
    FOREIGN KEY ([Reserv_ID])
    REFERENCES [dbo].[Reservations]
        ([Reserv_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_History_Reservation'
CREATE INDEX [IX_FK_History_Reservation]
ON [dbo].[Histories]
    ([Reserv_ID]);
GO

-- Creating foreign key on [ParkingLot_ID] in table 'ParkingSpots'
ALTER TABLE [dbo].[ParkingSpots]
ADD CONSTRAINT [FK_ParkingSpot_Lot]
    FOREIGN KEY ([ParkingLot_ID])
    REFERENCES [dbo].[Lots]
        ([Lot_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParkingSpot_Lot'
CREATE INDEX [IX_FK_ParkingSpot_Lot]
ON [dbo].[ParkingSpots]
    ([ParkingLot_ID]);
GO

-- Creating foreign key on [ParkingSpotID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_ParkingSpot]
    FOREIGN KEY ([ParkingSpotID])
    REFERENCES [dbo].[ParkingSpots]
        ([ParkingSpot_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_ParkingSpot'
CREATE INDEX [IX_FK_Reservation_ParkingSpot]
ON [dbo].[Reservations]
    ([ParkingSpotID]);
GO

-- Creating foreign key on [Reserv_ID] in table 'Permits'
ALTER TABLE [dbo].[Permits]
ADD CONSTRAINT [FK_Permit_Reservation]
    FOREIGN KEY ([Reserv_ID])
    REFERENCES [dbo].[Reservations]
        ([Reserv_ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Permit_Reservation'
CREATE INDEX [IX_FK_Permit_Reservation]
ON [dbo].[Permits]
    ([Reserv_ID]);
GO

-- Creating foreign key on [Requester_ID] in table 'Reservations'
ALTER TABLE [dbo].[Reservations]
ADD CONSTRAINT [FK_Reservation_Requester]
    FOREIGN KEY ([Requester_ID])
    REFERENCES [dbo].[Requesters]
        ([Requester_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reservation_Requester'
CREATE INDEX [IX_FK_Reservation_Requester]
ON [dbo].[Reservations]
    ([Requester_ID]);
GO

-- Creating foreign key on [Lots_Lot_ID] in table 'AllowedLots'
ALTER TABLE [dbo].[AllowedLots]
ADD CONSTRAINT [FK_AllowedLots_Lot]
    FOREIGN KEY ([Lots_Lot_ID])
    REFERENCES [dbo].[Lots]
        ([Lot_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Reservations_Reserv_ID] in table 'AllowedLots'
ALTER TABLE [dbo].[AllowedLots]
ADD CONSTRAINT [FK_AllowedLots_Reservation]
    FOREIGN KEY ([Reservations_Reserv_ID])
    REFERENCES [dbo].[Reservations]
        ([Reserv_ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AllowedLots_Reservation'
CREATE INDEX [IX_FK_AllowedLots_Reservation]
ON [dbo].[AllowedLots]
    ([Reservations_Reserv_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------