
/****** Object:  Table [dbo].[Mentores]    Script Date: 2017-11-14 09:29:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF Exists [dbo].[MentoresDump]

GO

DROP TABLE IF Exists [dbo].[MentorsDump]

GO

DROP TABLE IF Exists [dbo].[InterventionsDump]

GO

CREATE TABLE [dbo].[MentoresDump](
	[No_Mentore] int NOT NULL,
	[Prenom_Mentore] [nvarchar](30) NOT NULL,
	[Nom_Mentore] [nvarchar](30) NOT NULL,
	[Organisme_Mentore] [nvarchar](100) NOT NULL,
	[Courriel_Mentore] [nvarchar](50) NOT NULL,
	[Telephone_Mentore] [nvarchar](15) NOT NULL,
	[Cellulaire_Mentore] [nvarchar](15) NULL,
	[No_Expert_Mentore] [int] NULL,
	[No_Mentor_Mentore] [int] NOT NULL,
	[Objectifs_Mentore] [ntext] NULL,
	[Paye_Mentore] [bit] NOT NULL,
	[DateInscription_Mentore] [datetime] NULL,
	[MotPasse_Mentore] [nvarchar](255) NULL,
	[upsize_ts] [timestamp] NOT NULL)

GO

CREATE TABLE [dbo].[MentorsDump](
	[No_Mentor] int NOT NULL,
	[Prenom_Mentor] [nvarchar](255) NULL,
	[Nom_Mentor] [nvarchar](255) NULL,
	[Taxe_Mentor] [bit] NOT NULL,
	[NoTPS_Mentor] [nvarchar](255) NULL,
	[NoTVQ_Mentor] [nvarchar](255) NULL,
	[DateConnexion_Mentor] [datetime] NULL,
	[Courriel_Mentor] [nvarchar](50) NOT NULL DEFAULT (''))

GO

CREATE TABLE [dbo].[InterventionsDump](
	[No_Intervention] int NOT NULL,
	[Date_Intervention] [datetime] NULL,
	[No_Mentor_Intervention] [int] NULL,
	[No_Mentore_Intervention] [int] NULL,
	[Duree_Intervention] [int] NULL,
	[Description_Intervention] [nvarchar](500) NULL)

Go

