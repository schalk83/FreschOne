USE [dbJumpman_Dev]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foApproval]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foApproval](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DecisionID] [bigint] NULL,
	[Comment] [varchar](max) NULL,
	[ApprovalDate] [datetime] NULL,
	[ProcessInstanceID] [bigint] NULL,
	[ProcessID] [bigint] NULL,
	[UserID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foApprovalHist]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foApprovalHist](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[ProcessID] [bigint] NULL,
	[ProcessStepID] [varchar](500) NULL,
	[FromUserID] [bigint] NULL,
	[ToUserID] [bigint] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [PK__foApprovalHi__3214EC2739F69C7F] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foApprovalSteps]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foApprovalSteps](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessID] [bigint] NULL,
	[StepNo] [int] NULL,
	[UserID] [bigint] NULL,
	[Description] [varchar](1000) NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foDecisions]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foDecisions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foProcess]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foProcess](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](1000) NULL,
	[NeedApproval] [bit] NULL,
	[PKColumn] [varchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foProcessSteps]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foProcessSteps](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessID] [bigint] NULL,
	[ProcessStep] [varchar](500) NULL,
	[Description] [varchar](1000) NULL,
	[Active] [bit] NULL,
	[StepType] [varchar](10) NULL,
 CONSTRAINT [PK__tbl_Proc__3214EC27D9C2B7D0] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foProcessUser]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foProcessUser](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[ProcessID] [bigint] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foStepHist]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foStepHist](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[ProcessID] [bigint] NULL,
	[ProcessStepID] [varchar](500) NULL,
	[FromUserID] [bigint] NULL,
	[ToUserID] [bigint] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[Description] [varchar](1000) NULL,
 CONSTRAINT [PK__foStepHi__3214EC2739F69C7F] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foTaskControls]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foTaskControls](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessStepsID] [bigint] NULL,
	[ColumnName] [varchar](500) NULL,
	[Selected] [bit] NULL,
	[BrowsePage_Query] [varchar](max) NULL,
	[BrowsePage_Mapping] [varchar](max) NULL,
	[Require] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[_old_foUsers]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_old_foUsers](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[FirstName] [varchar](max) NULL,
	[LastName] [varchar](max) NULL,
	[Email] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foAdminTables]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foAdminTables](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [varchar](max) NULL,
	[Parent] [bit] NULL,
	[TableGroup] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalAttachments]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalAttachments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalID] [int] NOT NULL,
	[AttachmentDescription] [nvarchar](255) NULL,
	[AttachmentPath] [nvarchar](500) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedbyID] [int] NOT NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalEvents]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalEvents](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[StepID] [bigint] NULL,
	[PreviousEventID] [bigint] NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[DateAssigned] [datetime] NULL,
	[DateCompleted] [datetime] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalEventsArchive]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalEventsArchive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[StepID] [bigint] NULL,
	[PreviousEventID] [bigint] NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[DateAssigned] [datetime] NULL,
	[DateCompleted] [datetime] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalEventsDetail]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalEventsDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalEventID] [bigint] NOT NULL,
	[ProcessInstanceID] [bigint] NOT NULL,
	[StepID] [bigint] NOT NULL,
	[RecordID] [bigint] NOT NULL,
	[DataSetUpdate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalEventsDetailArchive]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalEventsDetailArchive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalEventID] [bigint] NOT NULL,
	[ProcessInstanceID] [bigint] NOT NULL,
	[StepID] [bigint] NOT NULL,
	[RecordID] [bigint] NOT NULL,
	[DataSetUpdate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovals]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovals](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [int] NOT NULL,
	[ApprovalEventID] [int] NOT NULL,
	[StepID] [int] NOT NULL,
	[Decision] [varchar](20) NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK__foApprov__3214EC27B1FFFF82] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foApprovalSteps]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foApprovalSteps](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessID] [bigint] NULL,
	[StepNo] [decimal](18, 2) NULL,
	[StepDescription] [varchar](1000) NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foEmailNotifications]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foEmailNotifications](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[StepID] [bigint] NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[EmailTemplateID] [bigint] NULL,
	[DateSent] [datetime] NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foEmailNotificationsDetail]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foEmailNotificationsDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmailNotificationID] [bigint] NOT NULL,
	[DataSetUpdate] [nvarchar](max) NULL,
	[AttachmentDataSetUpdate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foEmailSettings]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foEmailSettings](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailSMTP] [varchar](500) NULL,
	[EmailUserName] [varchar](500) NULL,
	[EmailPassword] [varchar](500) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foEmailTemplate]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foEmailTemplate](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[TemplateType] [varchar](50) NULL,
	[EmailSubject] [varchar](max) NULL,
	[EmailBody] [varchar](max) NULL,
	[ActiveDate] [datetime] NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foGroups]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foGroups](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcess]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcess](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessName] [varchar](max) NULL,
	[ProcessDescription] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessCancellations]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessCancellations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [int] NOT NULL,
	[CancelledUserID] [int] NOT NULL,
	[CancellationReason] [nvarchar](max) NOT NULL,
	[CancelledDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessDetail]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessDetail](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StepID] [bigint] NULL,
	[TableName] [varchar](max) NULL,
	[ColumnQuery] [varchar](max) NULL,
	[FormType] [varchar](2) NULL,
	[ColumnCount] [int] NULL,
	[Parent] [bit] NULL,
	[FKColumn] [varchar](500) NULL,
	[TableDescription] [varchar](max) NULL,
	[Active] [bit] NULL,
	[ColumnCalcs] [nvarchar](max) NULL,
	[ListTable] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessEvents]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessEvents](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[StepID] [bigint] NULL,
	[PreviousEventID] [bigint] NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[DateAssigned] [datetime] NULL,
	[DateCompleted] [datetime] NULL,
	[Active] [bit] NULL,
	[Cancelled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessEventsArchive]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessEventsArchive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [bigint] NULL,
	[StepID] [bigint] NULL,
	[PreviousEventID] [bigint] NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[DateAssigned] [datetime] NULL,
	[DateCompleted] [datetime] NULL,
	[Active] [bit] NULL,
	[Cancelled] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessEventsDetail]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessEventsDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessEventID] [bigint] NOT NULL,
	[ProcessInstanceID] [bigint] NOT NULL,
	[StepID] [bigint] NOT NULL,
	[TableName] [nvarchar](255) NOT NULL,
	[RecordID] [bigint] NOT NULL,
	[DataSetUpdate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessEventsDetailArchive]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessEventsDetailArchive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessEventID] [bigint] NOT NULL,
	[ProcessInstanceID] [bigint] NOT NULL,
	[StepID] [bigint] NOT NULL,
	[TableName] [nvarchar](255) NOT NULL,
	[RecordID] [bigint] NOT NULL,
	[DataSetUpdate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedUserID] [int] NOT NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcessSteps]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foProcessSteps](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessID] [bigint] NULL,
	[StepNo] [decimal](18, 2) NULL,
	[StepDescription] [varchar](1000) NULL,
	[GroupID] [bigint] NULL,
	[UserID] [bigint] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foReports]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foReports](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportName] [varchar](max) NULL,
	[ReportDescription] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foReportTable]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foReportTable](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportsID] [bigint] NULL,
	[TableName] [varchar](max) NULL,
	[ColumnQuery] [varchar](max) NULL,
	[FormType] [varchar](2) NULL,
	[ColumnCount] [int] NULL,
	[Parent] [bit] NULL,
	[FKColumn] [varchar](500) NULL,
	[TableDescription] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foReportTableQuery]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foReportTableQuery](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportsID] [bigint] NULL,
	[Query] [varchar](max) NULL,
	[FormType] [varchar](2) NULL,
	[ColumnCount] [int] NULL,
	[TableDescription] [varchar](max) NULL,
	[Active] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTable]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTable](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SchemaName] [varchar](120) NULL,
	[TableName] [varchar](1500) NULL,
	[Active] [bit] NULL,
	[ColumnNames] [varchar](max) NULL,
	[Script] [varchar](max) NULL,
	[TableGroup] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTableAttachments]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foTableColumns]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTableColumns](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[TableID] [bigint] NOT NULL,
	[ColumnName] [varchar](200) NOT NULL,
	[ColumnDataType] [varchar](50) NOT NULL,
	[ColumnLength_Precision] [varchar](50) NULL,
	[IsNullable] [bit] NOT NULL,
	[ForeignKeyTable] [varchar](200) NULL,
	[Active] [bit] NOT NULL,
	[Attachment] [bit] NULL,
	[Geo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTableColumnsToIgnore]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foTableGroups]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foTableGroups](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](255) NULL,
	[Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foTablePrefixes]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foUserGroups]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserGroups](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[GroupID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUserLogin]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foUserPasswordReset]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foUserProcess]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserProcess](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[ProcessID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUserReports]    Script Date: 2025/05/23 17:16:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foUserReports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NULL,
	[ReportID] [bigint] NULL,
	[Active] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foUsers]    Script Date: 2025/05/23 17:16:34 ******/
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
/****** Object:  Table [dbo].[foUserTable]    Script Date: 2025/05/23 17:16:34 ******/
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
ALTER TABLE [dbo].[_old_foTaskControls] ADD  DEFAULT ((1)) FOR [Require]
GO
ALTER TABLE [dbo].[foApprovalAttachments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[foApprovals] ADD  CONSTRAINT [DF__foApprova__Creat__09602436]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[foProcessCancellations] ADD  DEFAULT (getdate()) FOR [CancelledDate]
GO
ALTER TABLE [dbo].[foProcessEvents] ADD  DEFAULT ((0)) FOR [Cancelled]
GO
ALTER TABLE [dbo].[foProcessEventsDetail] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[foTableColumns] ADD  DEFAULT ((1)) FOR [IsNullable]
GO
ALTER TABLE [dbo].[foTableColumns] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[_old_foApproval]  WITH CHECK ADD FOREIGN KEY([DecisionID])
REFERENCES [dbo].[_old_foDecisions] ([ID])
GO
ALTER TABLE [dbo].[_old_foProcessSteps]  WITH CHECK ADD  CONSTRAINT [FK__tbl_Proce__Proce__50C6C558] FOREIGN KEY([ProcessID])
REFERENCES [dbo].[_old_foProcess] ([ID])
GO
ALTER TABLE [dbo].[_old_foProcessSteps] CHECK CONSTRAINT [FK__tbl_Proce__Proce__50C6C558]
GO
ALTER TABLE [dbo].[_old_foTaskControls]  WITH CHECK ADD FOREIGN KEY([ProcessStepsID])
REFERENCES [dbo].[_old_foProcessSteps] ([ID])
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
