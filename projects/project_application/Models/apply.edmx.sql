
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/15/2012 13:40:17
-- Generated from EDMX file: G:\贵州项目\Models\apply.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ApplyData];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ApplyProjectSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ApplyProjectSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ApplyProjectSet'
CREATE TABLE [dbo].[ApplyProjectSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectNameTitle] nvarchar(100)  NOT NULL,
    [ApplySchool] nvarchar(100)  NOT NULL,
    [ProjectLeader] nvarchar(100)  NOT NULL,
    [ApplyDate] datetime  NOT NULL,
    [ProjectType] nvarchar(100)  NOT NULL,
    [ProjectName] nvarchar(max)  NOT NULL,
    [ProjectLeaderDetail] nvarchar(max)  NOT NULL,
    [ProjectAbstract] nvarchar(max)  NOT NULL,
    [ProjectMeaning] nvarchar(max)  NOT NULL,
    [BulidTarget] nvarchar(max)  NOT NULL,
    [BulidContent] nvarchar(max)  NOT NULL,
    [BulidTask] nvarchar(max)  NOT NULL,
    [ExpectEffect] nvarchar(max)  NOT NULL,
    [ProjectSchedule] nvarchar(max)  NOT NULL,
    [ProjectPay] nvarchar(max)  NOT NULL,
    [ProjectAttach] nvarchar(100)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ApplyProjectSet'
ALTER TABLE [dbo].[ApplyProjectSet]
ADD CONSTRAINT [PK_ApplyProjectSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------