USE [dbJumpman_Dev]
GO
/****** Object:  Table [dbo].[foTableAttachments]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTableAttachments](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[tablename] [varchar](100) NOT NULL,
	[PKID] [bigint] NOT NULL,
	[AttachmentDescription] [varchar](max) NULL,
	[Attachment] [varchar](max) NULL,
	[DateAdded] [datetime] NULL,
	[UserID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTableColumnsToIgnore]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTableColumnsToIgnore](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ColumnName] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTablePrefixes]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTablePrefixes](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Prefix] [varchar](100) NULL,
	[Description] [varchar](255) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUserLogin]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserLogin](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[LoginDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUserPasswordReset]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserPasswordReset](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[ResetwithNextLogin] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUsers]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUsers](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[FirstName] [varchar](max) NULL,
	[LastName] [varchar](max) NULL,
	[Email] [varchar](max) NULL,
	[Admin] [bit] NOT NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUserTable]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserTable](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[TableName] [varchar](max) NULL,
	[ReadWriteAccess] [varchar](20) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_md_Campus]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_md_Campus](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_md_HomeType]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_md_HomeType](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_md_Province]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_md_Province](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Device]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Device](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](500) NULL,
	[Wattage] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Home]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Home](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Street1] [varchar](500) NULL,
	[Street2] [varchar](500) NULL,
	[Street3] [varchar](500) NULL,
	[Postal] [int] NULL,
	[HomeTypeID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Home_Person]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Home_Person](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[HomeID] [bigint] NULL,
	[Owner_FirstName] [varchar](50) NULL,
	[Owner_LastName] [varchar](50) NULL,
	[PurchasePrice] [decimal](18, 2) NULL,
	[HomeTypeID] [bigint] NULL,
	[ProvinceID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Home_Registration]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Home_Registration](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[HomeID] [bigint] NULL,
	[StartDate] [datetime] NULL,
	[Fee] [decimal](18, 2) NULL,
	[RegistrationOfficer] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Person]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Person](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[DOB] [datetime] NULL,
	[IDNumber] [varchar](13) NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_Student]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_Student](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[DOB] [datetime] NULL,
	[CampusID] [bigint] NULL,
	[ProvinceID] [bigint] NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
	[attachment_StudentPass] [varchar](max) NULL,
	[attachment_ProofOfAddress] [varchar](max) NULL,
 CONSTRAINT [PK__tbl_Stud__3214EC2765C5CC04] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_StudentDetails1]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_StudentDetails1](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentID] [bigint] NULL,
	[FirstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[DOB] [datetime] NULL,
	[CampusID] [bigint] NULL,
	[Age] [int] NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
	[attachment_ProofOfAddress] [varchar](max) NULL,
	[attachment_ProofOfIncome] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_studentdetails1_ExtraInformation]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentDetails1ID] [bigint] NULL,
	[HomeTypeID] [bigint] NULL,
	[ExtraDetails1] [varchar](250) NULL,
	[ExtraDetails] [varchar](max) NULL,
	[ExtraInformationDateTime] [datetime] NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_studentdetails1_ExtraInformation_no2]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation_no2](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentDetails1ID] [bigint] NULL,
	[HomeTypeID] [bigint] NULL,
	[ExtraDetails1] [varchar](250) NULL,
	[ExtraDetails] [varchar](max) NULL,
	[ExtraInformationDateTime] [datetime] NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_StudentDetails2]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_StudentDetails2](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentID] [bigint] NULL,
	[FirstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[DOB] [datetime] NULL,
	[CampusID] [bigint] NULL,
	[Active] [bit] NULL,
	[CreateUserID] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedUserID] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedUserID] [bigint] NULL,
	[DeletedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_tran_StudentDetails3]    Script Date: 2025/03/20 10:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tran_StudentDetails3](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentID] [bigint] NULL,
	[FirstName] [varchar](500) NULL,
	[LastName] [varchar](500) NULL,
	[DOB] [datetime] NULL,
	[CampusID] [bigint] NULL,
	[Grade] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_tran_Person] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails1] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation_no2] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails2] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[foUserLogin]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[foUsers] ([ID])
GO
ALTER TABLE [dbo].[foUserPasswordReset]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[foUsers] ([ID])
GO
ALTER TABLE [dbo].[foUserTable]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[foUsers] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home]  WITH CHECK ADD FOREIGN KEY([HomeTypeID])
REFERENCES [dbo].[tbl_md_HomeType] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home]  WITH CHECK ADD FOREIGN KEY([HomeTypeID])
REFERENCES [dbo].[tbl_md_HomeType] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home_Person]  WITH CHECK ADD FOREIGN KEY([HomeID])
REFERENCES [dbo].[tbl_tran_Home] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home_Person]  WITH CHECK ADD FOREIGN KEY([HomeTypeID])
REFERENCES [dbo].[tbl_md_HomeType] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home_Person]  WITH CHECK ADD FOREIGN KEY([ProvinceID])
REFERENCES [dbo].[tbl_md_Province] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Home_Registration]  WITH CHECK ADD FOREIGN KEY([HomeID])
REFERENCES [dbo].[tbl_tran_Home] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Student]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Stude__Campu__21D6CC45] FOREIGN KEY([CampusID])
REFERENCES [dbo].[tbl_md_Campus] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Student] CHECK CONSTRAINT [FK__tbl_Stude__Campu__21D6CC45]
GO
ALTER TABLE [dbo].[tbl_tran_Student]  WITH CHECK ADD  CONSTRAINT [FK__tbl_tran___Provi__59E61B3E] FOREIGN KEY([ProvinceID])
REFERENCES [dbo].[tbl_md_Province] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_Student] CHECK CONSTRAINT [FK__tbl_tran___Provi__59E61B3E]
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails1]  WITH CHECK ADD FOREIGN KEY([CampusID])
REFERENCES [dbo].[tbl_md_Campus] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails1]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Stude__Stude__2E3CA32A] FOREIGN KEY([StudentID])
REFERENCES [dbo].[tbl_tran_Student] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails1] CHECK CONSTRAINT [FK__tbl_Stude__Stude__2E3CA32A]
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation]  WITH CHECK ADD FOREIGN KEY([HomeTypeID])
REFERENCES [dbo].[tbl_md_HomeType] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation]  WITH CHECK ADD FOREIGN KEY([StudentDetails1ID])
REFERENCES [dbo].[tbl_tran_StudentDetails1] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation_no2]  WITH CHECK ADD FOREIGN KEY([HomeTypeID])
REFERENCES [dbo].[tbl_md_HomeType] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_studentdetails1_ExtraInformation_no2]  WITH CHECK ADD FOREIGN KEY([StudentDetails1ID])
REFERENCES [dbo].[tbl_tran_StudentDetails1] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails2]  WITH CHECK ADD FOREIGN KEY([CampusID])
REFERENCES [dbo].[tbl_md_Campus] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails2]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Stude__Stude__320D340E] FOREIGN KEY([StudentID])
REFERENCES [dbo].[tbl_tran_Student] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails2] CHECK CONSTRAINT [FK__tbl_Stude__Stude__320D340E]
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails3]  WITH CHECK ADD FOREIGN KEY([CampusID])
REFERENCES [dbo].[tbl_md_Campus] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails3]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Stude__Stude__35DDC4F2] FOREIGN KEY([StudentID])
REFERENCES [dbo].[tbl_tran_Student] ([ID])
GO
ALTER TABLE [dbo].[tbl_tran_StudentDetails3] CHECK CONSTRAINT [FK__tbl_Stude__Stude__35DDC4F2]
GO
