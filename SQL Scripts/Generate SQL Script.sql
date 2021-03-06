USE [master]
GO
/****** Object:  Database [SampleApplication]    Script Date: 03/17/2014 09:43:58 ******/
CREATE DATABASE [SampleApplication] ON  PRIMARY 
( NAME = N'SampleApplication', FILENAME = N'F:\Database MS SQL Server\SampleApplication.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SampleApplication_log', FILENAME = N'F:\Database MS SQL Server\SampleApplication_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SampleApplication] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SampleApplication].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SampleApplication] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [SampleApplication] SET ANSI_NULLS OFF
GO
ALTER DATABASE [SampleApplication] SET ANSI_PADDING OFF
GO
ALTER DATABASE [SampleApplication] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [SampleApplication] SET ARITHABORT OFF
GO
ALTER DATABASE [SampleApplication] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [SampleApplication] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [SampleApplication] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [SampleApplication] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [SampleApplication] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [SampleApplication] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [SampleApplication] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [SampleApplication] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [SampleApplication] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [SampleApplication] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [SampleApplication] SET  DISABLE_BROKER
GO
ALTER DATABASE [SampleApplication] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [SampleApplication] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [SampleApplication] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [SampleApplication] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [SampleApplication] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [SampleApplication] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [SampleApplication] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [SampleApplication] SET  READ_WRITE
GO
ALTER DATABASE [SampleApplication] SET RECOVERY FULL
GO
ALTER DATABASE [SampleApplication] SET  MULTI_USER
GO
ALTER DATABASE [SampleApplication] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [SampleApplication] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'SampleApplication', N'ON'
GO
USE [SampleApplication]
GO
/****** Object:  Table [dbo].[Reimburse]    Script Date: 03/17/2014 09:43:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Reimburse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NULL,
	[Total] [decimal](18, 2) NULL,
	[IsSubmitted] [bit] NULL,
	[IsConfirmed] [bit] NULL,
	[IsCleared] [bit] NULL,
	[SubmittedDate] [datetime] NULL,
	[ConfirmedDate] [datetime] NULL,
	[ClearanceDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ActualPaid] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Reimburse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserType]    Script Date: 03/17/2014 09:43:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 03/17/2014 09:43:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[UserTypeId] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReimburseDetail]    Script Date: 03/17/2014 09:43:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReimburseDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReimburseId] [int] NULL,
	[Description] [varchar](500) NULL,
	[Amount] [decimal](18, 2) NULL,
	[IsRejected] [bit] NULL,
	[ExpenseDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_ReimburseDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_Users_UserType]    Script Date: 03/17/2014 09:43:59 ******/
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserType] FOREIGN KEY([UserTypeId])
REFERENCES [dbo].[UserType] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserType]
GO
/****** Object:  ForeignKey [FK_ReimburseDetail_ReimburseDetail]    Script Date: 03/17/2014 09:43:59 ******/
ALTER TABLE [dbo].[ReimburseDetail]  WITH CHECK ADD  CONSTRAINT [FK_ReimburseDetail_ReimburseDetail] FOREIGN KEY([ReimburseId])
REFERENCES [dbo].[Reimburse] ([Id])
GO
ALTER TABLE [dbo].[ReimburseDetail] CHECK CONSTRAINT [FK_ReimburseDetail_ReimburseDetail]
GO
