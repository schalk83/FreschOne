USE [dbJumpman_Dev]
GO
/****** Object:  Table [dbo].[_old_foApproval]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foApprovalHist]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foApprovalSteps]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foDecisions]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foProcess]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foProcessSteps]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foProcessUser]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foStepHist]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foTaskControls]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[_old_foUsers]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foAdminTables]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalAttachments]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalEvents]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalEventsArchive]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalEventsDetail]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalEventsDetailArchive]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovals]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foApprovalSteps]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foEmailNotifications]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foEmailNotificationsDetail]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foEmailSettings]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foEmailTemplate]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foGroups]    Script Date: 2025/05/23 17:32:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[foGroups](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[foProcess]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessCancellations]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessDetail]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessEvents]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessEventsArchive]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessEventsDetail]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessEventsDetailArchive]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foProcessSteps]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foReports]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foReportTable]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foReportTableQuery]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTable]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTableAttachments]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTableColumns]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTableColumnsToIgnore]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTableGroups]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foTablePrefixes]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserGroups]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserLogin]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserPasswordReset]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserProcess]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserReports]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUsers]    Script Date: 2025/05/23 17:32:54 ******/
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
/****** Object:  Table [dbo].[foUserTable]    Script Date: 2025/05/23 17:32:54 ******/
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
SET IDENTITY_INSERT [dbo].[_old_foApproval] ON 

INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (1, 1, N'adsasd', CAST(N'2024-11-18T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (2, 1, N'sfddf', CAST(N'2024-11-18T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (3, 1, N'2', CAST(N'2024-11-18T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (4, 1, N'sfsdfsd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (6, 2, N'dsas', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (7, 2, N'sdsdf', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (8, 1, N'adssad', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (9, 1, N'dsd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (10, 2, N'dd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (11, 2, N'sddsf', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (12, 1, N'DAS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (13, 1, N'DAS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (14, 1, N'DAS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (15, 1, N'213SDASDF', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (16, 1, N'DASDS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (17, 1, N'SDFFD', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (18, 1, N'3122FDS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (19, 1, N'213DFS', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (20, 2, N'FDSFSFD ', CAST(N'2024-11-26T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (21, 1, N'cc', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (22, 1, N'sadsd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (23, 2, N'dsasdads', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (24, 1, N'ds', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (25, 1, N'sdf', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (26, 1, N'sdf', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (27, 1, N' dfsd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (28, 1, N'sfdfd', CAST(N'2024-11-19T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (30, 1, N'sdf', CAST(N'1900-01-01T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (31, 1, N'SFDFD', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (32, 2, N'SDF', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (33, 1, N'SAD', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (34, 1, N'dg', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (35, 2, N'FDDFG', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (36, 2, N'FDDFG', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (37, 1, N'SDFSDF', CAST(N'2024-11-20T00:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (38, 1, N'sdfsdf', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 11, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (39, 1, N'dff', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 43, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (40, 3, N'assda', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 43, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (41, 1, N'jggdf', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 44, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (42, 1, N'sdf', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 45, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (43, 2, N'asdasd', CAST(N'2024-11-20T00:00:00.000' AS DateTime), 45, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (44, 2, N'sdfdf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 45, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (45, 3, N'sdffsdsfd', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 46, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (46, 3, N'sdfsdf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 46, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (47, 3, N'sdf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 46, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (48, 1, N'jhgf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 47, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (49, 1, N'df', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 47, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (50, 1, N'dzsx 1235555', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 47, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (51, 1, N'dasasd', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 48, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (52, 1, N'sdf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 49, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (53, 1, N'sfddf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 49, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (54, 1, N'dffg', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 50, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (55, 1, N'sdf', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 50, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (56, 1, N'ABCEDERSDF', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 50, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (57, 1, N'SDF', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 50, 4, 1)
INSERT [dbo].[_old_foApproval] ([ID], [DecisionID], [Comment], [ApprovalDate], [ProcessInstanceID], [ProcessID], [UserID]) VALUES (58, 1, N'SDF', CAST(N'2024-11-21T00:00:00.000' AS DateTime), 50, 4, 1)
SET IDENTITY_INSERT [dbo].[_old_foApproval] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foApprovalHist] ON 

INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (10, 11, 4, N'17', 1, 1, CAST(N'2024-11-19T13:03:32.240' AS DateTime), CAST(N'2024-11-19T13:11:05.803' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (11, 11, 4, N'18', 1, 1, CAST(N'2024-11-19T13:11:05.817' AS DateTime), CAST(N'2024-11-19T13:11:47.300' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (12, 11, 4, N'19', 1, 1, CAST(N'2024-11-19T13:11:47.647' AS DateTime), CAST(N'2024-11-19T13:28:07.200' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (13, 25, 4, N'17', 1, 1, CAST(N'2024-11-19T16:33:23.440' AS DateTime), CAST(N'2024-11-19T16:38:18.213' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (14, 25, 4, N'18', 1, 1, CAST(N'2024-11-19T16:38:18.243' AS DateTime), CAST(N'2024-11-19T16:39:09.783' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (15, 25, 4, N'19', 1, 1, CAST(N'2024-11-19T16:39:09.817' AS DateTime), CAST(N'2024-11-19T16:39:41.017' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (16, 36, 4, N'17', 1, 1, CAST(N'2024-11-20T09:07:58.770' AS DateTime), CAST(N'2024-11-20T09:08:26.473' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (17, 36, 4, N'18', 1, 1, CAST(N'2024-11-20T09:08:26.477' AS DateTime), CAST(N'2024-11-20T09:09:08.313' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (18, 36, 4, N'19', 1, 1, CAST(N'2024-11-20T09:09:08.317' AS DateTime), CAST(N'2024-11-20T09:09:17.153' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (19, 41, 4, N'17', 1, 1, CAST(N'2024-11-20T11:53:19.640' AS DateTime), CAST(N'2024-11-20T11:57:15.757' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (20, 41, 4, N'18', 1, 1, CAST(N'2024-11-20T11:56:16.277' AS DateTime), NULL, N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (21, 41, 4, N'19', 1, 1, CAST(N'2024-11-20T11:57:15.770' AS DateTime), CAST(N'2024-11-20T11:58:04.810' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (22, 42, 4, N'17', 1, 1, CAST(N'2024-11-20T12:20:46.107' AS DateTime), NULL, N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (23, 43, 4, N'17', 1, 1, CAST(N'2024-11-20T14:35:21.720' AS DateTime), CAST(N'2024-11-20T14:35:35.213' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (24, 43, 4, N'18', 1, 1, CAST(N'2024-11-20T14:35:35.217' AS DateTime), CAST(N'2024-11-20T14:36:05.963' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (25, 43, 4, N'19', 1, 1, CAST(N'2024-11-20T14:36:05.973' AS DateTime), NULL, N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (26, 44, 4, N'17', 1, 1, CAST(N'2024-11-20T15:07:48.600' AS DateTime), CAST(N'2024-11-20T15:09:45.553' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (27, 44, 4, N'18', 1, 1, CAST(N'2024-11-20T15:09:45.557' AS DateTime), NULL, N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (28, 45, 4, N'17', 1, 1, CAST(N'2024-11-20T15:33:16.140' AS DateTime), CAST(N'2024-11-20T15:52:24.207' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (29, 45, 4, N'18', 1, 1, CAST(N'2024-11-20T15:43:05.233' AS DateTime), NULL, N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (30, 45, 4, N'19', 1, 1, CAST(N'2024-11-20T15:52:24.210' AS DateTime), CAST(N'2024-11-21T08:31:07.600' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (31, 46, 4, N'17', 1, 1, CAST(N'2024-11-21T08:41:53.177' AS DateTime), CAST(N'2024-11-21T08:42:45.360' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (32, 46, 4, N'18', 1, 1, CAST(N'2024-11-21T08:42:45.363' AS DateTime), CAST(N'2024-11-21T09:23:42.393' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (33, 46, 4, N'19', 1, 1, CAST(N'2024-11-21T08:42:57.157' AS DateTime), NULL, N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (34, 47, 4, N'17', 1, 1, CAST(N'2024-11-21T09:26:56.890' AS DateTime), CAST(N'2024-11-21T09:30:02.570' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (35, 47, 4, N'17', 1, 1, CAST(N'2024-11-21T09:41:11.017' AS DateTime), CAST(N'2024-11-21T09:42:02.223' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (36, 47, 4, N'18', 1, 1, CAST(N'2024-11-21T09:42:05.440' AS DateTime), CAST(N'2024-11-21T09:42:18.823' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (37, 47, 4, N'19', 1, 1, CAST(N'2024-11-21T09:42:18.833' AS DateTime), CAST(N'2024-11-21T09:42:57.120' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (38, 48, 4, N'17', 1, 1, CAST(N'2024-11-21T11:11:01.030' AS DateTime), CAST(N'2024-11-21T11:11:22.313' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (39, 48, 4, N'18', 1, 1, CAST(N'2024-11-21T11:11:22.320' AS DateTime), CAST(N'2024-11-21T11:13:06.957' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (40, 49, 4, N'17', 1, 1, CAST(N'2024-11-21T12:54:27.583' AS DateTime), CAST(N'2024-11-21T13:08:50.873' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (41, 49, 4, N'18', 1, 1, CAST(N'2024-11-21T13:08:50.880' AS DateTime), CAST(N'2024-11-21T13:09:12.163' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (42, 49, 4, N'19', 1, 1, CAST(N'2024-11-21T13:09:12.170' AS DateTime), NULL, N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (43, 50, 4, N'17', 1, 1, CAST(N'2024-11-21T13:12:19.323' AS DateTime), CAST(N'2024-11-21T13:17:14.093' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (44, 50, 4, N'18', 1, 1, CAST(N'2024-11-21T13:17:14.100' AS DateTime), CAST(N'2024-11-21T13:19:03.927' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (45, 50, 4, N'19', 1, 1, CAST(N'2024-11-21T13:19:03.930' AS DateTime), CAST(N'2024-11-21T13:19:26.940' AS DateTime), N'd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (46, 50, 4, N'17', 1, 1, CAST(N'2024-11-21T13:19:36.260' AS DateTime), CAST(N'2024-11-21T13:20:13.357' AS DateTime), N'asd')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (47, 50, 4, N'18', 1, 1, CAST(N'2024-11-21T13:20:13.360' AS DateTime), CAST(N'2024-11-21T13:25:23.757' AS DateTime), N'Registration')
INSERT [dbo].[_old_foApprovalHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (48, 50, 4, N'19', 1, 1, CAST(N'2024-11-21T13:25:23.770' AS DateTime), CAST(N'2024-11-21T13:25:29.777' AS DateTime), N'd')
SET IDENTITY_INSERT [dbo].[_old_foApprovalHist] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foApprovalSteps] ON 

INSERT [dbo].[_old_foApprovalSteps] ([ID], [ProcessID], [StepNo], [UserID], [Description], [Active]) VALUES (18, 4, 4, 1, N'Registration', 1)
INSERT [dbo].[_old_foApprovalSteps] ([ID], [ProcessID], [StepNo], [UserID], [Description], [Active]) VALUES (17, 4, 1, 1, N'asd', 1)
INSERT [dbo].[_old_foApprovalSteps] ([ID], [ProcessID], [StepNo], [UserID], [Description], [Active]) VALUES (19, 4, 3, 3, N'd', 1)
SET IDENTITY_INSERT [dbo].[_old_foApprovalSteps] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foDecisions] ON 

INSERT [dbo].[_old_foDecisions] ([ID], [Description], [Active]) VALUES (1, N'Approve', 1)
INSERT [dbo].[_old_foDecisions] ([ID], [Description], [Active]) VALUES (2, N'Decline', 1)
INSERT [dbo].[_old_foDecisions] ([ID], [Description], [Active]) VALUES (3, N'Rework', 1)
SET IDENTITY_INSERT [dbo].[_old_foDecisions] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foProcess] ON 

INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (1, N'tbl_tran_Home', NULL, NULL)
INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (2, N'tbl_tran_Employee', NULL, NULL)
INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (4, N'tbl_tran_Student', 1, N'StudentID')
INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (5, N'tbl_md_Campus', NULL, NULL)
INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (6, N'tbl_md_Province', NULL, NULL)
INSERT [dbo].[_old_foProcess] ([ID], [Description], [NeedApproval], [PKColumn]) VALUES (11, N'tbl_tran_Person', 1, NULL)
SET IDENTITY_INSERT [dbo].[_old_foProcess] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foProcessSteps] ON 

INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (1, 1, N'tbl_tran_Home', N'Home', 1, N'T')
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (8, 1, N'tbl_tran_Home', N'Registration', 1, NULL)
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (9, 2, N'tbl_tran_EmployeeDetails', N'Details', 1, NULL)
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (10, 2, N'tbl_tran_EmployeeRegistration', N'Registraion', 1, NULL)
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (11, 4, N'tbl_tran_StudentDetails1', N'A STUDENT DETAILS 1', 1, N'T')
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (12, 4, N'tbl_tran_StudentDetails2', N'StudentDatail2', 1, N'T')
INSERT [dbo].[_old_foProcessSteps] ([ID], [ProcessID], [ProcessStep], [Description], [Active], [StepType]) VALUES (13, 4, N'tbl_tran_StudentDetails3', N'StudentDatail3', 1, N'T')
SET IDENTITY_INSERT [dbo].[_old_foProcessSteps] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foProcessUser] ON 

INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (1, 1, 4, NULL)
INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (2, 2, 4, NULL)
INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (3, 2, 5, NULL)
INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (4, 2, 5, NULL)
INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (5, 1, 5, NULL)
INSERT [dbo].[_old_foProcessUser] ([ID], [UserID], [ProcessID], [Active]) VALUES (6, 3, 5, NULL)
SET IDENTITY_INSERT [dbo].[_old_foProcessUser] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foStepHist] ON 

INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (274, 1, 4, N'0', 1, 1, CAST(N'2024-11-18T15:22:14.390' AS DateTime), CAST(N'2024-11-18T15:22:14.390' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (275, 1, 4, N'11', 1, 1, CAST(N'2024-11-18T15:22:14.413' AS DateTime), CAST(N'2024-11-18T15:23:53.000' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (276, 1, 4, N'0', 1, 1, CAST(N'2024-11-18T15:23:53.033' AS DateTime), NULL, N'Back to Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (277, 1, 4, N'11', 1, 1, CAST(N'2024-11-18T15:24:17.390' AS DateTime), CAST(N'2024-11-18T15:24:50.990' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (278, 1, 4, N'12', 1, 1, CAST(N'2024-11-18T15:24:51.003' AS DateTime), CAST(N'2024-11-18T15:25:12.600' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (279, 1, 4, N'13', 1, 1, CAST(N'2024-11-18T15:25:12.610' AS DateTime), CAST(N'2024-11-18T15:25:54.900' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (280, 1, 4, N'12', 1, 1, CAST(N'2024-11-18T15:25:54.910' AS DateTime), CAST(N'2024-11-18T15:25:57.690' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (281, 1, 4, N'11', 1, 1, CAST(N'2024-11-18T15:25:57.700' AS DateTime), CAST(N'2024-11-18T15:25:59.300' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (282, 1, 4, N'12', 1, 1, CAST(N'2024-11-18T15:25:59.310' AS DateTime), CAST(N'2024-11-18T15:26:08.330' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (283, 1, 4, N'13', 1, 1, CAST(N'2024-11-18T15:26:08.343' AS DateTime), CAST(N'2024-11-18T15:26:58.123' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (284, 1, 4, N'0', 1, 1, CAST(N'2024-11-18T15:26:11.947' AS DateTime), NULL, N'')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (285, 1, 4, N'0', 1, 1, CAST(N'2024-11-18T15:26:58.153' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (286, 2, 4, N'0', 1, 1, CAST(N'2024-11-18T15:42:48.120' AS DateTime), CAST(N'2024-11-18T15:42:48.120' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (287, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:42:48.147' AS DateTime), CAST(N'2024-11-18T15:43:09.173' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (289, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:43:12.983' AS DateTime), CAST(N'2024-11-18T15:43:15.347' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (290, 2, 4, N'12', 1, 1, CAST(N'2024-11-18T15:43:15.367' AS DateTime), CAST(N'2024-11-18T15:43:20.950' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (291, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:43:20.967' AS DateTime), CAST(N'2024-11-18T15:43:35.267' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (292, 2, 4, N'0', 1, 1, CAST(N'2024-11-18T15:43:35.290' AS DateTime), CAST(N'2024-11-18T15:43:36.520' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (293, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:43:36.533' AS DateTime), CAST(N'2024-11-18T15:43:53.203' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (294, 2, 4, N'0', 1, 1, CAST(N'2024-11-18T15:43:53.227' AS DateTime), CAST(N'2024-11-18T15:43:57.867' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (295, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:43:57.880' AS DateTime), CAST(N'2024-11-18T15:44:02.787' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (296, 2, 4, N'12', 1, 1, CAST(N'2024-11-18T15:44:02.803' AS DateTime), CAST(N'2024-11-18T15:44:04.680' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (297, 2, 4, N'13', 1, 1, CAST(N'2024-11-18T15:44:04.693' AS DateTime), CAST(N'2024-11-18T15:44:10.733' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (298, 2, 4, N'12', 1, 1, CAST(N'2024-11-18T15:44:10.743' AS DateTime), CAST(N'2024-11-18T15:44:14.510' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (299, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:44:14.520' AS DateTime), CAST(N'2024-11-18T15:44:20.347' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (300, 2, 4, N'0', 1, 1, CAST(N'2024-11-18T15:44:20.383' AS DateTime), CAST(N'2024-11-18T15:44:24.153' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (301, 2, 4, N'11', 1, 1, CAST(N'2024-11-18T15:44:24.170' AS DateTime), CAST(N'2024-11-18T15:44:32.207' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (302, 2, 4, N'0', 1, 1, CAST(N'2024-11-18T15:44:32.230' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (303, 3, 4, N'0', 1, 1, CAST(N'2024-11-18T15:44:44.287' AS DateTime), CAST(N'2024-11-18T15:44:44.287' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (304, 3, 4, N'11', 1, 1, CAST(N'2024-11-18T15:44:44.310' AS DateTime), CAST(N'2024-11-18T15:49:26.260' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (305, 3, 4, N'0', 1, 1, CAST(N'2024-11-18T15:49:26.293' AS DateTime), NULL, N'Back to Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (306, 4, 4, N'0', 1, 1, CAST(N'2024-11-18T15:50:03.567' AS DateTime), CAST(N'2024-11-18T15:50:03.567' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (307, 4, 4, N'11', 1, 1, CAST(N'2024-11-18T15:50:03.603' AS DateTime), CAST(N'2024-11-18T15:50:51.943' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (308, 4, 4, N'0', 1, 1, CAST(N'2024-11-18T15:51:09.327' AS DateTime), NULL, N'Back to Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (309, 4, 4, N'11', 1, 1, CAST(N'2024-11-18T15:53:44.197' AS DateTime), CAST(N'2024-11-18T15:53:48.087' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (310, 4, 4, N'0', 1, 1, CAST(N'2024-11-18T15:53:48.110' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (311, 5, 4, N'0', 1, 1, CAST(N'2024-11-18T15:54:40.470' AS DateTime), CAST(N'2024-11-18T15:54:40.470' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (312, 5, 4, N'11', 1, 1, CAST(N'2024-11-18T15:54:40.497' AS DateTime), CAST(N'2024-11-18T15:55:03.803' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (313, 5, 4, N'0', 1, 1, CAST(N'2024-11-18T15:55:05.183' AS DateTime), NULL, N'Back to Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (314, 6, 4, N'0', 1, 1, CAST(N'2024-11-18T15:56:09.480' AS DateTime), CAST(N'2024-11-18T15:56:09.480' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (315, 6, 4, N'11', 1, 1, CAST(N'2024-11-18T15:56:10.040' AS DateTime), CAST(N'2024-11-18T15:57:15.883' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (316, 7, 4, N'0', 1, 1, CAST(N'2024-11-18T15:58:27.423' AS DateTime), CAST(N'2024-11-18T15:58:27.423' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (317, 7, 4, N'11', 1, 1, CAST(N'2024-11-18T15:58:27.447' AS DateTime), CAST(N'2024-11-18T15:58:49.400' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (318, 7, 4, N'11', 1, 1, CAST(N'2024-11-18T16:05:09.877' AS DateTime), CAST(N'2024-11-18T16:05:26.640' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (319, 7, 4, N'0', 1, 1, CAST(N'2024-11-18T16:05:26.663' AS DateTime), CAST(N'2024-11-18T16:05:31.480' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (320, 7, 4, N'11', 1, 1, CAST(N'2024-11-18T16:05:31.490' AS DateTime), CAST(N'2024-11-18T16:14:51.147' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (321, 7, 4, N'12', 1, 1, CAST(N'2024-11-18T16:14:51.160' AS DateTime), CAST(N'2024-11-18T16:14:57.040' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (322, 7, 4, N'11', 1, 1, CAST(N'2024-11-18T16:14:57.053' AS DateTime), CAST(N'2024-11-18T16:33:09.270' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (323, 7, 4, N'12', 1, 1, CAST(N'2024-11-18T16:33:09.283' AS DateTime), CAST(N'2024-11-18T16:33:11.090' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (324, 7, 4, N'13', 1, 1, CAST(N'2024-11-18T16:33:11.110' AS DateTime), NULL, N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (325, 8, 4, N'0', 1, 1, CAST(N'2024-11-18T16:48:51.050' AS DateTime), CAST(N'2024-11-18T16:48:51.050' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (326, 8, 4, N'11', 1, 1, CAST(N'2024-11-18T16:48:51.073' AS DateTime), CAST(N'2024-11-18T16:48:57.070' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (327, 8, 4, N'0', 1, 1, CAST(N'2024-11-18T16:48:57.093' AS DateTime), CAST(N'2024-11-18T16:48:58.510' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (328, 8, 4, N'11', 1, 1, CAST(N'2024-11-18T16:48:58.523' AS DateTime), CAST(N'2024-11-18T16:49:00.333' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (329, 8, 4, N'12', 1, 1, CAST(N'2024-11-18T16:49:00.350' AS DateTime), CAST(N'2024-11-18T16:49:06.027' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (330, 8, 4, N'13', 1, 1, CAST(N'2024-11-18T16:49:06.047' AS DateTime), CAST(N'2024-11-18T16:52:19.740' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (331, 8, 4, N'0', 1, 1, CAST(N'2024-11-18T16:52:19.760' AS DateTime), NULL, N'')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (332, 9, 4, N'0', 1, 1, CAST(N'2024-11-18T17:34:55.023' AS DateTime), CAST(N'2024-11-18T17:34:55.023' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (333, 9, 4, N'11', 1, 1, CAST(N'2024-11-18T17:34:55.047' AS DateTime), CAST(N'2024-11-18T17:35:22.430' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (334, 9, 4, N'12', 1, 1, CAST(N'2024-11-18T17:35:22.447' AS DateTime), CAST(N'2024-11-18T17:35:44.007' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (335, 9, 4, N'11', 1, 1, CAST(N'2024-11-18T17:35:44.020' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (336, 10, 4, N'0', 1, 1, CAST(N'2024-11-18T17:38:33.870' AS DateTime), CAST(N'2024-11-18T17:38:33.870' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (337, 10, 4, N'11', 1, 1, CAST(N'2024-11-18T17:38:33.897' AS DateTime), CAST(N'2024-11-18T17:38:40.047' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (338, 10, 4, N'12', 1, 1, CAST(N'2024-11-18T17:38:40.060' AS DateTime), CAST(N'2024-11-18T17:38:46.100' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (339, 10, 4, N'11', 1, 1, CAST(N'2024-11-18T17:38:46.117' AS DateTime), CAST(N'2024-11-18T17:38:47.987' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (340, 10, 4, N'12', 1, 1, CAST(N'2024-11-18T17:38:48.000' AS DateTime), CAST(N'2024-11-18T17:38:49.490' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (341, 10, 4, N'13', 1, 1, CAST(N'2024-11-18T17:38:49.500' AS DateTime), CAST(N'2024-11-18T17:39:02.257' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (342, 10, 4, N'0', 1, 1, CAST(N'2024-11-18T17:39:02.283' AS DateTime), NULL, N'')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (343, 11, 4, N'0', 1, 1, CAST(N'2024-11-18T19:27:04.320' AS DateTime), CAST(N'2024-11-18T19:27:04.320' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (344, 11, 4, N'11', 1, 1, CAST(N'2024-11-18T19:27:04.340' AS DateTime), CAST(N'2024-11-18T19:27:10.420' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (345, 11, 4, N'12', 1, 1, CAST(N'2024-11-18T19:27:10.430' AS DateTime), CAST(N'2024-11-18T19:27:20.917' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (346, 11, 4, N'13', 1, 1, CAST(N'2024-11-18T19:27:20.930' AS DateTime), CAST(N'2024-11-19T13:03:32.227' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (347, 12, 4, N'0', 1, 1, CAST(N'2024-11-19T14:39:21.523' AS DateTime), CAST(N'2024-11-19T14:39:21.523' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (348, 12, 4, N'11', 1, 1, CAST(N'2024-11-19T14:39:21.550' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (349, 13, 4, N'0', 1, 1, CAST(N'2024-11-19T14:40:11.343' AS DateTime), CAST(N'2024-11-19T14:40:11.343' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (350, 13, 4, N'11', 1, 1, CAST(N'2024-11-19T14:40:11.370' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (351, 14, 4, N'0', 1, 1, CAST(N'2024-11-19T14:49:08.097' AS DateTime), CAST(N'2024-11-19T14:49:08.097' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (352, 14, 4, N'11', 1, 1, CAST(N'2024-11-19T14:49:08.123' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (353, 15, 4, N'0', 1, 1, CAST(N'2024-11-19T14:49:33.870' AS DateTime), CAST(N'2024-11-19T14:49:33.870' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (354, 15, 4, N'11', 1, 1, CAST(N'2024-11-19T14:49:33.900' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (355, 16, 4, N'0', 1, 1, CAST(N'2024-11-19T14:50:55.280' AS DateTime), CAST(N'2024-11-19T14:50:55.280' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (356, 16, 4, N'11', 1, 1, CAST(N'2024-11-19T14:50:55.300' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (357, 17, 4, N'0', 1, 1, CAST(N'2024-11-19T14:53:28.983' AS DateTime), CAST(N'2024-11-19T14:53:28.983' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (358, 17, 4, N'11', 1, 1, CAST(N'2024-11-19T14:53:29.010' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (359, 18, 4, N'0', 1, 1, CAST(N'2024-11-19T14:53:57.770' AS DateTime), CAST(N'2024-11-19T14:53:57.770' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (360, 18, 4, N'11', 1, 1, CAST(N'2024-11-19T14:53:57.797' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (361, 19, 4, N'0', 1, 1, CAST(N'2024-11-19T15:05:51.253' AS DateTime), CAST(N'2024-11-19T15:05:51.253' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (362, 19, 4, N'11', 1, 1, CAST(N'2024-11-19T15:05:51.280' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (363, 20, 4, N'0', 1, 1, CAST(N'2024-11-19T15:06:17.383' AS DateTime), CAST(N'2024-11-19T15:06:17.383' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (364, 20, 4, N'11', 1, 1, CAST(N'2024-11-19T15:06:17.407' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (365, 21, 4, N'0', 1, 1, CAST(N'2024-11-19T15:08:41.050' AS DateTime), CAST(N'2024-11-19T15:08:41.050' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (366, 21, 4, N'11', 1, 1, CAST(N'2024-11-19T15:08:41.083' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (367, 22, 4, N'0', 1, 1, CAST(N'2024-11-19T15:15:46.727' AS DateTime), CAST(N'2024-11-19T15:15:46.727' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (368, 22, 4, N'11', 1, 1, CAST(N'2024-11-19T15:15:46.750' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (369, 23, 4, N'0', 1, 1, CAST(N'2024-11-19T15:28:52.540' AS DateTime), CAST(N'2024-11-19T15:28:52.540' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (370, 23, 4, N'11', 1, 1, CAST(N'2024-11-19T15:28:52.567' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (371, 24, 4, N'0', 1, 1, CAST(N'2024-11-19T15:34:27.820' AS DateTime), CAST(N'2024-11-19T15:34:27.820' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (372, 24, 4, N'11', 1, 1, CAST(N'2024-11-19T15:34:27.850' AS DateTime), CAST(N'2024-11-19T15:51:25.113' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (373, 24, 4, N'12', 1, 1, CAST(N'2024-11-19T15:51:25.133' AS DateTime), NULL, N'StudentDatail2')
GO
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (374, 25, 4, N'0', 1, 1, CAST(N'2024-11-19T16:24:10.963' AS DateTime), CAST(N'2024-11-19T16:24:10.963' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (375, 25, 4, N'11', 1, 1, CAST(N'2024-11-19T16:24:11.050' AS DateTime), CAST(N'2024-11-19T16:24:20.607' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (376, 25, 4, N'12', 1, 1, CAST(N'2024-11-19T16:24:20.623' AS DateTime), CAST(N'2024-11-19T16:24:29.240' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (377, 25, 4, N'11', 1, 1, CAST(N'2024-11-19T16:24:29.277' AS DateTime), CAST(N'2024-11-19T16:24:32.483' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (378, 25, 4, N'12', 1, 1, CAST(N'2024-11-19T16:24:32.530' AS DateTime), CAST(N'2024-11-19T16:24:33.870' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (379, 25, 4, N'11', 1, 1, CAST(N'2024-11-19T16:24:33.890' AS DateTime), CAST(N'2024-11-19T16:24:35.207' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (380, 25, 4, N'12', 1, 1, CAST(N'2024-11-19T16:24:35.220' AS DateTime), CAST(N'2024-11-19T16:24:36.370' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (381, 25, 4, N'11', 1, 1, CAST(N'2024-11-19T16:24:36.393' AS DateTime), CAST(N'2024-11-19T16:24:37.610' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (382, 25, 4, N'12', 1, 1, CAST(N'2024-11-19T16:24:37.627' AS DateTime), CAST(N'2024-11-19T16:33:15.070' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (383, 25, 4, N'13', 1, 1, CAST(N'2024-11-19T16:33:15.097' AS DateTime), CAST(N'2024-11-19T16:33:23.420' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (384, 26, 4, N'0', 1, 1, CAST(N'2024-11-19T16:48:51.437' AS DateTime), CAST(N'2024-11-19T16:48:51.437' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (385, 26, 4, N'11', 1, 1, CAST(N'2024-11-19T16:48:51.490' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (386, 27, 4, N'0', 1, 1, CAST(N'2024-11-19T17:30:49.830' AS DateTime), CAST(N'2024-11-19T17:30:49.830' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (387, 27, 4, N'11', 1, 1, CAST(N'2024-11-19T17:30:49.860' AS DateTime), CAST(N'2024-11-19T17:31:51.617' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (388, 27, 4, N'0', 1, 1, CAST(N'2024-11-19T17:31:51.643' AS DateTime), CAST(N'2024-11-19T17:31:54.240' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (389, 27, 4, N'11', 1, 1, CAST(N'2024-11-19T17:31:54.253' AS DateTime), CAST(N'2024-11-19T17:31:55.400' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (390, 27, 4, N'0', 1, 1, CAST(N'2024-11-19T17:31:55.427' AS DateTime), CAST(N'2024-11-19T17:31:56.243' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (391, 27, 4, N'11', 1, 1, CAST(N'2024-11-19T17:31:56.260' AS DateTime), CAST(N'2024-11-19T17:31:57.280' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (392, 27, 4, N'0', 1, 1, CAST(N'2024-11-19T17:31:57.310' AS DateTime), CAST(N'2024-11-19T17:32:02.773' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (393, 27, 4, N'11', 1, 1, CAST(N'2024-11-19T17:32:02.787' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (394, 28, 4, N'0', 1, 1, CAST(N'2024-11-19T17:35:18.123' AS DateTime), CAST(N'2024-11-19T17:35:18.123' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (395, 28, 4, N'11', 1, 1, CAST(N'2024-11-19T17:35:18.150' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (396, 29, 4, N'0', 1, 1, CAST(N'2024-11-19T18:06:28.640' AS DateTime), CAST(N'2024-11-19T18:06:28.640' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (397, 29, 4, N'11', 1, 1, CAST(N'2024-11-19T18:06:28.660' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (398, 30, 4, N'0', 1, 1, CAST(N'2024-11-19T18:28:31.237' AS DateTime), CAST(N'2024-11-19T18:28:31.237' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (399, 30, 4, N'11', 1, 1, CAST(N'2024-11-19T18:28:31.260' AS DateTime), CAST(N'2024-11-19T18:28:32.743' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (400, 30, 4, N'0', 1, 1, CAST(N'2024-11-19T18:28:32.773' AS DateTime), CAST(N'2024-11-19T18:31:33.460' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (401, 30, 4, N'11', 1, 1, CAST(N'2024-11-19T18:30:34.427' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (402, 30, 4, N'11', 1, 1, CAST(N'2024-11-19T18:31:33.470' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (403, 31, 4, N'0', 1, 1, CAST(N'2024-11-19T18:43:26.847' AS DateTime), CAST(N'2024-11-19T18:43:26.847' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (404, 31, 4, N'11', 1, 1, CAST(N'2024-11-19T18:43:26.870' AS DateTime), CAST(N'2024-11-19T18:43:36.803' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (405, 31, 4, N'0', 1, 1, CAST(N'2024-11-19T18:43:36.823' AS DateTime), CAST(N'2024-11-19T18:43:38.080' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (406, 31, 4, N'11', 1, 1, CAST(N'2024-11-19T18:43:38.093' AS DateTime), CAST(N'2024-11-19T18:43:41.790' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (407, 31, 4, N'12', 1, 1, CAST(N'2024-11-19T18:43:41.800' AS DateTime), CAST(N'2024-11-19T18:43:49.727' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (408, 31, 4, N'11', 1, 1, CAST(N'2024-11-19T18:43:49.740' AS DateTime), CAST(N'2024-11-19T18:43:51.267' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (409, 31, 4, N'12', 1, 1, CAST(N'2024-11-19T18:43:51.280' AS DateTime), CAST(N'2024-11-19T18:45:32.310' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (410, 31, 4, N'11', 1, 1, CAST(N'2024-11-19T18:45:32.323' AS DateTime), CAST(N'2024-11-19T18:45:56.420' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (411, 31, 4, N'12', 1, 1, CAST(N'2024-11-19T18:45:56.430' AS DateTime), CAST(N'2024-11-19T18:48:45.927' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (412, 31, 4, N'11', 1, 1, CAST(N'2024-11-19T18:48:45.940' AS DateTime), CAST(N'2024-11-19T18:48:47.367' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (413, 31, 4, N'12', 1, 1, CAST(N'2024-11-19T18:48:47.380' AS DateTime), NULL, N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (414, 32, 4, N'0', 1, 1, CAST(N'2024-11-20T08:22:33.840' AS DateTime), CAST(N'2024-11-20T08:22:33.840' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (415, 32, 4, N'11', 1, 1, CAST(N'2024-11-20T08:22:33.847' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (416, 33, 4, N'0', 1, 1, CAST(N'2024-11-20T08:45:03.907' AS DateTime), CAST(N'2024-11-20T08:45:03.907' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (417, 33, 4, N'11', 1, 1, CAST(N'2024-11-20T08:45:03.913' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (418, 34, 4, N'0', 1, 1, CAST(N'2024-11-20T08:51:14.673' AS DateTime), CAST(N'2024-11-20T08:51:14.673' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (419, 34, 4, N'11', 1, 1, CAST(N'2024-11-20T08:51:14.680' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (420, 35, 4, N'0', 1, 1, CAST(N'2024-11-20T08:52:59.167' AS DateTime), CAST(N'2024-11-20T08:52:59.167' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (421, 35, 4, N'11', 1, 1, CAST(N'2024-11-20T08:52:59.180' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (422, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:55:50.147' AS DateTime), CAST(N'2024-11-20T08:55:50.147' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (423, 36, 4, N'11', 1, 1, CAST(N'2024-11-20T08:55:50.150' AS DateTime), CAST(N'2024-11-20T08:57:52.860' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (424, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:56:06.087' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (425, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:58:15.290' AS DateTime), CAST(N'2024-11-20T08:58:57.250' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (426, 36, 4, N'11', 1, 1, CAST(N'2024-11-20T08:58:57.260' AS DateTime), CAST(N'2024-11-20T08:59:06.993' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (427, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:59:06.980' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (428, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:59:07.000' AS DateTime), CAST(N'2024-11-20T08:59:13.007' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (429, 36, 4, N'11', 1, 1, CAST(N'2024-11-20T08:59:13.010' AS DateTime), CAST(N'2024-11-20T08:59:13.803' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (430, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:59:13.807' AS DateTime), CAST(N'2024-11-20T08:59:14.603' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (431, 36, 4, N'11', 1, 1, CAST(N'2024-11-20T08:59:14.607' AS DateTime), CAST(N'2024-11-20T08:59:15.540' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (432, 36, 4, N'0', 1, 1, CAST(N'2024-11-20T08:59:15.543' AS DateTime), CAST(N'2024-11-20T08:59:16.400' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (433, 36, 4, N'11', 1, 1, CAST(N'2024-11-20T08:59:16.403' AS DateTime), CAST(N'2024-11-20T08:59:18.260' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (434, 36, 4, N'12', 1, 1, CAST(N'2024-11-20T08:59:18.263' AS DateTime), CAST(N'2024-11-20T09:07:46.817' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (435, 36, 4, N'13', 1, 1, CAST(N'2024-11-20T09:07:46.820' AS DateTime), CAST(N'2024-11-20T09:07:58.767' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (436, 37, 4, N'0', 1, 1, CAST(N'2024-11-20T09:09:39.060' AS DateTime), CAST(N'2024-11-20T09:09:39.060' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (437, 37, 4, N'11', 1, 1, CAST(N'2024-11-20T09:09:39.073' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (438, 38, 4, N'0', 1, 1, CAST(N'2024-11-20T09:32:55.177' AS DateTime), CAST(N'2024-11-20T09:32:55.177' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (439, 38, 4, N'11', 1, 1, CAST(N'2024-11-20T09:32:55.183' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (440, 39, 4, N'0', 1, 1, CAST(N'2024-11-20T09:36:44.560' AS DateTime), CAST(N'2024-11-20T09:36:44.560' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (441, 39, 4, N'11', 1, 1, CAST(N'2024-11-20T09:36:44.567' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (442, 40, 4, N'0', 1, 1, CAST(N'2024-11-20T10:03:20.567' AS DateTime), CAST(N'2024-11-20T10:03:20.567' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (443, 40, 4, N'11', 1, 1, CAST(N'2024-11-20T10:03:20.573' AS DateTime), CAST(N'2024-11-20T10:03:25.037' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (444, 40, 4, N'0', 1, 1, CAST(N'2024-11-20T10:03:25.043' AS DateTime), CAST(N'2024-11-20T10:03:35.010' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (445, 40, 4, N'11', 1, 1, CAST(N'2024-11-20T10:03:35.017' AS DateTime), CAST(N'2024-11-20T10:04:06.730' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (446, 40, 4, N'0', 1, 1, CAST(N'2024-11-20T10:04:06.740' AS DateTime), CAST(N'2024-11-20T10:04:33.163' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (447, 40, 4, N'11', 1, 1, CAST(N'2024-11-20T10:04:33.167' AS DateTime), CAST(N'2024-11-20T10:04:41.177' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (448, 40, 4, N'0', 1, 1, CAST(N'2024-11-20T10:04:41.183' AS DateTime), CAST(N'2024-11-20T10:04:48.637' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (449, 40, 4, N'11', 1, 1, CAST(N'2024-11-20T10:04:48.640' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (450, 41, 4, N'0', 1, 1, CAST(N'2024-11-20T11:52:52.753' AS DateTime), CAST(N'2024-11-20T11:52:52.753' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (451, 41, 4, N'11', 1, 1, CAST(N'2024-11-20T11:52:52.770' AS DateTime), CAST(N'2024-11-20T11:53:02.107' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (452, 41, 4, N'12', 1, 1, CAST(N'2024-11-20T11:53:02.113' AS DateTime), CAST(N'2024-11-20T11:53:10.537' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (453, 41, 4, N'13', 1, 1, CAST(N'2024-11-20T11:53:10.540' AS DateTime), CAST(N'2024-11-20T11:53:19.637' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (454, 42, 4, N'0', 1, 1, CAST(N'2024-11-20T12:20:08.957' AS DateTime), CAST(N'2024-11-20T12:20:08.957' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (455, 42, 4, N'11', 1, 1, CAST(N'2024-11-20T12:20:08.967' AS DateTime), CAST(N'2024-11-20T12:20:28.460' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (456, 42, 4, N'12', 1, 1, CAST(N'2024-11-20T12:20:28.463' AS DateTime), CAST(N'2024-11-20T12:20:35.297' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (457, 42, 4, N'13', 1, 1, CAST(N'2024-11-20T12:20:35.300' AS DateTime), CAST(N'2024-11-20T12:20:46.103' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (458, 43, 4, N'0', 1, 1, CAST(N'2024-11-20T14:33:42.167' AS DateTime), CAST(N'2024-11-20T14:33:42.167' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (459, 43, 4, N'11', 1, 1, CAST(N'2024-11-20T14:33:42.177' AS DateTime), CAST(N'2024-11-20T14:35:07.703' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (460, 43, 4, N'12', 1, 1, CAST(N'2024-11-20T14:35:07.710' AS DateTime), CAST(N'2024-11-20T14:35:14.907' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (461, 43, 4, N'13', 1, 1, CAST(N'2024-11-20T14:35:14.910' AS DateTime), CAST(N'2024-11-20T14:35:21.713' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (462, 44, 4, N'0', 1, 1, CAST(N'2024-11-20T15:06:21.747' AS DateTime), CAST(N'2024-11-20T15:06:21.747' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (463, 44, 4, N'11', 1, 1, CAST(N'2024-11-20T15:06:21.753' AS DateTime), CAST(N'2024-11-20T15:06:29.837' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (464, 44, 4, N'12', 1, 1, CAST(N'2024-11-20T15:06:29.840' AS DateTime), CAST(N'2024-11-20T15:06:36.710' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (465, 44, 4, N'13', 1, 1, CAST(N'2024-11-20T15:06:36.717' AS DateTime), CAST(N'2024-11-20T15:07:48.587' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (466, 45, 4, N'0', 1, 1, CAST(N'2024-11-20T15:32:53.737' AS DateTime), CAST(N'2024-11-20T15:32:53.737' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (467, 45, 4, N'11', 1, 1, CAST(N'2024-11-20T15:32:53.753' AS DateTime), CAST(N'2024-11-20T15:33:00.067' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (468, 45, 4, N'12', 1, 1, CAST(N'2024-11-20T15:33:00.070' AS DateTime), CAST(N'2024-11-20T15:33:05.383' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (469, 45, 4, N'13', 1, 1, CAST(N'2024-11-20T15:33:05.393' AS DateTime), CAST(N'2024-11-20T15:33:16.137' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (470, 46, 4, N'0', 1, 1, CAST(N'2024-11-21T08:41:14.210' AS DateTime), CAST(N'2024-11-21T08:41:14.210' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (471, 46, 4, N'11', 1, 1, CAST(N'2024-11-21T08:41:14.233' AS DateTime), CAST(N'2024-11-21T08:41:22.080' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (472, 46, 4, N'12', 1, 1, CAST(N'2024-11-21T08:41:22.083' AS DateTime), CAST(N'2024-11-21T08:41:38.103' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (473, 46, 4, N'13', 1, 1, CAST(N'2024-11-21T08:41:38.127' AS DateTime), CAST(N'2024-11-21T08:41:53.163' AS DateTime), N'StudentDatail3')
GO
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (474, 47, 4, N'0', 1, 1, CAST(N'2024-11-21T09:25:57.957' AS DateTime), CAST(N'2024-11-21T09:25:57.957' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (475, 47, 4, N'11', 1, 1, CAST(N'2024-11-21T09:25:57.963' AS DateTime), CAST(N'2024-11-21T09:26:37.240' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (476, 47, 4, N'12', 1, 1, CAST(N'2024-11-21T09:26:37.243' AS DateTime), CAST(N'2024-11-21T09:26:47.123' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (477, 47, 4, N'13', 1, 1, CAST(N'2024-11-21T09:26:47.127' AS DateTime), CAST(N'2024-11-21T09:26:56.880' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (478, 47, 4, N'0', 1, 1, CAST(N'2024-11-21T09:30:18.130' AS DateTime), CAST(N'2024-11-21T09:30:37.360' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (479, 47, 4, N'11', 1, 1, CAST(N'2024-11-21T09:30:37.363' AS DateTime), CAST(N'2024-11-21T09:41:08.800' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (480, 47, 4, N'12', 1, 1, CAST(N'2024-11-21T09:41:08.807' AS DateTime), CAST(N'2024-11-21T09:41:09.887' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (481, 47, 4, N'13', 1, 1, CAST(N'2024-11-21T09:41:09.890' AS DateTime), CAST(N'2024-11-21T09:41:11.013' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (482, 48, 4, N'0', 1, 1, CAST(N'2024-11-21T11:10:00.690' AS DateTime), CAST(N'2024-11-21T11:10:00.690' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (483, 48, 4, N'11', 1, 1, CAST(N'2024-11-21T11:10:00.993' AS DateTime), CAST(N'2024-11-21T11:10:16.080' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (484, 48, 4, N'12', 1, 1, CAST(N'2024-11-21T11:10:16.090' AS DateTime), CAST(N'2024-11-21T11:10:24.527' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (485, 48, 4, N'13', 1, 1, CAST(N'2024-11-21T11:10:24.530' AS DateTime), CAST(N'2024-11-21T11:11:01.027' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (486, 48, 4, N'0', 1, 1, CAST(N'2024-11-21T11:12:16.190' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (487, 48, 4, N'0', 1, 1, CAST(N'2024-11-21T11:13:06.977' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (488, 49, 4, N'0', 1, 1, CAST(N'2024-11-21T12:52:38.417' AS DateTime), CAST(N'2024-11-21T12:52:38.417' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (489, 49, 4, N'11', 1, 1, CAST(N'2024-11-21T12:52:38.430' AS DateTime), CAST(N'2024-11-21T12:53:47.713' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (490, 49, 4, N'12', 1, 1, CAST(N'2024-11-21T12:53:47.717' AS DateTime), CAST(N'2024-11-21T12:53:58.213' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (491, 49, 4, N'13', 1, 1, CAST(N'2024-11-21T12:53:58.220' AS DateTime), CAST(N'2024-11-21T12:54:27.580' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (492, 50, 4, N'0', 1, 1, CAST(N'2024-11-21T13:11:44.213' AS DateTime), CAST(N'2024-11-21T13:11:44.213' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (493, 50, 4, N'11', 1, 1, CAST(N'2024-11-21T13:11:44.220' AS DateTime), CAST(N'2024-11-21T13:12:00.317' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (494, 50, 4, N'12', 1, 1, CAST(N'2024-11-21T13:12:00.327' AS DateTime), CAST(N'2024-11-21T13:12:08.077' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (495, 50, 4, N'13', 1, 1, CAST(N'2024-11-21T13:12:08.080' AS DateTime), CAST(N'2024-11-21T13:12:19.320' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (496, 50, 4, N'0', 1, 1, CAST(N'2024-11-21T13:19:26.947' AS DateTime), CAST(N'2024-11-21T13:19:32.477' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (497, 50, 4, N'11', 1, 1, CAST(N'2024-11-21T13:19:32.480' AS DateTime), CAST(N'2024-11-21T13:19:33.887' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (498, 50, 4, N'12', 1, 1, CAST(N'2024-11-21T13:19:33.893' AS DateTime), CAST(N'2024-11-21T13:19:34.923' AS DateTime), N'StudentDatail2')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (499, 50, 4, N'13', 1, 1, CAST(N'2024-11-21T13:19:34.927' AS DateTime), CAST(N'2024-11-21T13:19:36.253' AS DateTime), N'StudentDatail3')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (500, 51, 4, N'0', 1, 1, CAST(N'2024-11-22T15:43:09.570' AS DateTime), CAST(N'2024-11-22T15:43:09.570' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (501, 51, 4, N'11', 1, 1, CAST(N'2024-11-22T15:43:09.600' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (502, 52, 4, N'0', 1, 1, CAST(N'2024-11-25T15:54:38.073' AS DateTime), CAST(N'2024-11-25T15:54:38.073' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (503, 52, 4, N'11', 1, 1, CAST(N'2024-11-25T15:54:38.083' AS DateTime), CAST(N'2024-11-25T17:20:32.370' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (504, 52, 4, N'0', 1, 1, CAST(N'2024-11-25T17:20:32.400' AS DateTime), CAST(N'2024-11-25T17:20:34.017' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (505, 52, 4, N'11', 1, 1, CAST(N'2024-11-25T17:20:34.037' AS DateTime), CAST(N'2024-11-25T17:20:39.750' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (506, 52, 4, N'0', 1, 1, CAST(N'2024-11-25T17:20:39.777' AS DateTime), CAST(N'2024-11-25T17:20:56.623' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (507, 52, 4, N'11', 1, 1, CAST(N'2024-11-25T17:20:56.637' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (508, 53, 4, N'0', 1, 1, CAST(N'2024-11-25T17:21:50.040' AS DateTime), CAST(N'2024-11-25T17:21:50.040' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (509, 53, 4, N'11', 1, 1, CAST(N'2024-11-25T17:21:50.060' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (510, 54, 4, N'0', 1, 1, CAST(N'2024-11-25T17:37:22.657' AS DateTime), CAST(N'2024-11-25T17:37:22.657' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (511, 54, 4, N'11', 1, 1, CAST(N'2024-11-25T17:37:22.683' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (512, 55, 4, N'0', 1, 1, CAST(N'2024-11-25T17:46:20.803' AS DateTime), CAST(N'2024-11-25T17:46:20.803' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (513, 55, 4, N'11', 1, 1, CAST(N'2024-11-25T17:46:20.827' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (514, 56, 4, N'0', 1, 1, CAST(N'2024-11-25T17:55:40.163' AS DateTime), CAST(N'2024-11-25T17:55:40.163' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (515, 56, 4, N'11', 1, 1, CAST(N'2024-11-25T17:55:40.183' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (516, 57, 4, N'0', 1, 1, CAST(N'2024-11-25T18:10:51.250' AS DateTime), CAST(N'2024-11-25T18:10:51.250' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (517, 57, 4, N'11', 1, 1, CAST(N'2024-11-25T18:10:51.277' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (518, 58, 4, N'0', 1, 1, CAST(N'2024-11-25T18:48:07.040' AS DateTime), CAST(N'2024-11-25T18:48:07.040' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (519, 58, 4, N'11', 1, 1, CAST(N'2024-11-25T18:48:07.067' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (520, 59, 4, N'0', 1, 1, CAST(N'2024-11-25T18:51:26.857' AS DateTime), CAST(N'2024-11-25T18:51:26.857' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (521, 59, 4, N'11', 1, 1, CAST(N'2024-11-25T18:51:26.880' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (522, 60, 4, N'0', 1, 1, CAST(N'2024-11-25T19:38:09.730' AS DateTime), CAST(N'2024-11-25T19:38:09.730' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (523, 60, 4, N'11', 1, 1, CAST(N'2024-11-25T19:38:09.750' AS DateTime), CAST(N'2024-11-25T19:39:10.900' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (524, 60, 4, N'0', 1, 1, CAST(N'2024-11-25T19:39:10.927' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (525, 61, 4, N'0', 1, 1, CAST(N'2024-11-25T19:41:45.533' AS DateTime), CAST(N'2024-11-25T19:41:45.533' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (526, 61, 4, N'11', 1, 1, CAST(N'2024-11-25T19:41:45.557' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (527, 62, 4, N'0', 1, 1, CAST(N'2024-11-25T20:01:41.667' AS DateTime), CAST(N'2024-11-25T20:01:41.667' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (528, 62, 4, N'11', 1, 1, CAST(N'2024-11-25T20:01:41.697' AS DateTime), CAST(N'2024-11-25T20:02:21.957' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (529, 62, 4, N'0', 1, 1, CAST(N'2024-11-25T20:02:21.983' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (530, 63, 4, N'0', 1, 1, CAST(N'2024-11-25T20:34:29.307' AS DateTime), CAST(N'2024-11-25T20:34:29.307' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (531, 63, 4, N'11', 1, 1, CAST(N'2024-11-25T20:34:29.337' AS DateTime), CAST(N'2024-11-25T20:35:07.060' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (532, 63, 4, N'0', 1, 1, CAST(N'2024-11-25T20:35:07.090' AS DateTime), CAST(N'2024-11-25T20:35:11.733' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (533, 63, 4, N'11', 1, 1, CAST(N'2024-11-25T20:35:11.747' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (534, 64, 4, N'0', 1, 1, CAST(N'2024-11-25T20:55:29.623' AS DateTime), CAST(N'2024-11-25T20:55:29.623' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (535, 64, 4, N'11', 1, 1, CAST(N'2024-11-25T20:55:29.657' AS DateTime), CAST(N'2024-11-25T20:55:39.380' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (536, 64, 4, N'0', 1, 1, CAST(N'2024-11-25T20:55:39.417' AS DateTime), CAST(N'2024-11-25T20:55:40.847' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (537, 64, 4, N'11', 1, 1, CAST(N'2024-11-25T20:55:40.860' AS DateTime), CAST(N'2024-11-25T22:18:05.023' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (538, 65, 4, N'0', 1, 1, CAST(N'2024-11-26T10:39:33.980' AS DateTime), CAST(N'2024-11-26T10:39:33.980' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (539, 65, 4, N'11', 1, 1, CAST(N'2024-11-26T10:39:33.997' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (540, 66, 4, N'0', 1, 1, CAST(N'2024-11-26T12:55:46.033' AS DateTime), CAST(N'2024-11-26T12:55:46.033' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (541, 66, 4, N'11', 1, 1, CAST(N'2024-11-26T12:55:46.043' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (542, 67, 4, N'0', 1, 1, CAST(N'2024-11-26T13:28:23.037' AS DateTime), CAST(N'2024-11-26T13:28:23.037' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (543, 67, 4, N'11', 1, 1, CAST(N'2024-11-26T13:28:23.047' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (544, 68, 4, N'0', 1, 1, CAST(N'2024-11-26T14:07:29.730' AS DateTime), CAST(N'2024-11-26T14:07:29.730' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (545, 68, 4, N'11', 1, 1, CAST(N'2024-11-26T14:07:29.737' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (546, 69, 4, N'0', 1, 1, CAST(N'2024-11-26T14:18:07.500' AS DateTime), CAST(N'2024-11-26T14:18:07.500' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (547, 69, 4, N'11', 1, 1, CAST(N'2024-11-26T14:18:07.510' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (548, 70, 4, N'0', 1, 1, CAST(N'2024-11-26T14:23:26.710' AS DateTime), CAST(N'2024-11-26T14:23:26.710' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (549, 70, 4, N'11', 1, 1, CAST(N'2024-11-26T14:23:26.720' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (550, 71, 4, N'0', 1, 1, CAST(N'2024-11-26T14:27:46.013' AS DateTime), CAST(N'2024-11-26T14:27:46.013' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (551, 71, 4, N'11', 1, 1, CAST(N'2024-11-26T14:27:46.020' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (552, 72, 4, N'0', 1, 1, CAST(N'2024-11-26T14:29:12.720' AS DateTime), CAST(N'2024-11-26T14:29:12.720' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (553, 72, 4, N'11', 1, 1, CAST(N'2024-11-26T14:29:12.727' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (554, 73, 4, N'0', 1, 1, CAST(N'2024-11-26T14:32:55.523' AS DateTime), CAST(N'2024-11-26T14:32:55.523' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (555, 73, 4, N'11', 1, 1, CAST(N'2024-11-26T14:32:55.530' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (556, 74, 4, N'0', 1, 1, CAST(N'2024-11-26T14:42:52.473' AS DateTime), CAST(N'2024-11-26T14:42:52.473' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (557, 74, 4, N'11', 1, 1, CAST(N'2024-11-26T14:42:52.480' AS DateTime), CAST(N'2024-11-26T14:46:10.337' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (558, 74, 4, N'0', 1, 1, CAST(N'2024-11-26T14:46:10.343' AS DateTime), CAST(N'2024-11-26T14:47:44.127' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (559, 74, 4, N'11', 1, 1, CAST(N'2024-11-26T14:47:44.130' AS DateTime), CAST(N'2024-11-26T14:48:43.407' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (560, 74, 4, N'0', 1, 1, CAST(N'2024-11-26T14:48:43.413' AS DateTime), CAST(N'2024-11-26T14:49:09.850' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (561, 74, 4, N'11', 1, 1, CAST(N'2024-11-26T14:49:09.857' AS DateTime), CAST(N'2024-11-26T15:11:01.497' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (562, 74, 4, N'0', 1, 1, CAST(N'2024-11-26T15:11:01.503' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (563, 75, 4, N'0', 1, 1, CAST(N'2024-11-26T15:15:20.570' AS DateTime), CAST(N'2024-11-26T15:15:20.570' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (564, 75, 4, N'11', 1, 1, CAST(N'2024-11-26T15:15:20.577' AS DateTime), CAST(N'2024-11-26T15:15:50.000' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (565, 75, 4, N'0', 1, 1, CAST(N'2024-11-26T15:15:50.013' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (566, 76, 4, N'0', 1, 1, CAST(N'2024-11-26T15:37:34.003' AS DateTime), CAST(N'2024-11-26T15:37:34.003' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (567, 76, 4, N'11', 1, 1, CAST(N'2024-11-26T15:37:34.010' AS DateTime), CAST(N'2024-11-26T15:37:45.777' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (568, 76, 4, N'0', 1, 1, CAST(N'2024-11-26T15:37:45.780' AS DateTime), CAST(N'2024-11-26T15:38:04.793' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (569, 76, 4, N'11', 1, 1, CAST(N'2024-11-26T15:38:04.797' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (570, 77, 4, N'0', 1, 1, CAST(N'2024-11-26T15:44:07.140' AS DateTime), CAST(N'2024-11-26T15:44:07.140' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (571, 77, 4, N'11', 1, 1, CAST(N'2024-11-26T15:44:07.147' AS DateTime), CAST(N'2024-11-26T15:47:26.890' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (572, 77, 4, N'0', 1, 1, CAST(N'2024-11-26T15:47:26.907' AS DateTime), CAST(N'2024-11-26T15:47:32.560' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (573, 77, 4, N'11', 1, 1, CAST(N'2024-11-26T15:47:32.567' AS DateTime), NULL, N'A STUDENT DETAILS 1')
GO
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (574, 78, 4, N'0', 1, 1, CAST(N'2024-11-27T13:01:38.857' AS DateTime), CAST(N'2024-11-27T13:01:38.857' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (575, 78, 4, N'11', 1, 1, CAST(N'2024-11-27T13:01:38.877' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (576, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T09:27:44.083' AS DateTime), CAST(N'2024-11-28T09:27:44.083' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (577, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T09:27:44.097' AS DateTime), CAST(N'2024-11-28T09:28:02.347' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (578, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T09:28:02.357' AS DateTime), CAST(N'2024-11-28T09:28:03.583' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (579, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T09:28:03.590' AS DateTime), CAST(N'2024-11-28T10:34:37.013' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (580, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T09:31:20.670' AS DateTime), CAST(N'2024-11-28T09:31:52.740' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (581, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T09:31:40.160' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (582, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T09:31:52.747' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (583, 80, 4, N'0', 1, 1, CAST(N'2024-11-28T10:08:49.610' AS DateTime), CAST(N'2024-11-28T10:08:49.610' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (584, 80, 4, N'11', 1, 1, CAST(N'2024-11-28T10:08:49.617' AS DateTime), CAST(N'2024-11-28T10:09:16.517' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (585, 80, 4, N'0', 1, 1, CAST(N'2024-11-28T10:09:16.520' AS DateTime), CAST(N'2024-11-28T10:09:17.467' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (586, 80, 4, N'11', 1, 1, CAST(N'2024-11-28T10:09:17.470' AS DateTime), CAST(N'2024-11-28T10:10:53.797' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (587, 80, 4, N'0', 1, 1, CAST(N'2024-11-28T10:10:53.800' AS DateTime), NULL, N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (588, 81, 4, N'0', 1, 1, CAST(N'2024-11-28T10:23:49.583' AS DateTime), CAST(N'2024-11-28T10:23:49.583' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (589, 81, 4, N'11', 1, 1, CAST(N'2024-11-28T10:23:49.590' AS DateTime), CAST(N'2024-11-28T10:23:52.297' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (590, 81, 4, N'0', 1, 1, CAST(N'2024-11-28T10:23:52.300' AS DateTime), CAST(N'2024-11-28T10:23:55.230' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (591, 81, 4, N'11', 1, 1, CAST(N'2024-11-28T10:23:55.233' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (592, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:34:37.030' AS DateTime), CAST(N'2024-11-28T10:34:39.310' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (593, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:34:39.320' AS DateTime), CAST(N'2024-11-28T10:34:56.540' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (594, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:34:56.547' AS DateTime), CAST(N'2024-11-28T10:34:57.587' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (595, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:34:57.590' AS DateTime), CAST(N'2024-11-28T10:34:59.870' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (596, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:34:59.877' AS DateTime), CAST(N'2024-11-28T10:35:01.510' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (597, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:35:01.513' AS DateTime), CAST(N'2024-11-28T10:35:03.987' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (598, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:35:03.993' AS DateTime), CAST(N'2024-11-28T10:35:04.783' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (599, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:35:04.787' AS DateTime), CAST(N'2024-11-28T10:35:05.843' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (600, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:35:05.850' AS DateTime), CAST(N'2024-11-28T10:35:08.160' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (601, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:35:08.167' AS DateTime), CAST(N'2024-11-28T10:35:09.120' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (602, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:35:09.130' AS DateTime), CAST(N'2024-11-28T10:45:22.527' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (603, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:45:22.530' AS DateTime), CAST(N'2024-11-28T10:45:23.943' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (604, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:45:23.953' AS DateTime), CAST(N'2024-11-28T10:45:24.840' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (605, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:45:24.847' AS DateTime), CAST(N'2024-11-28T10:45:36.500' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (606, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T10:45:36.510' AS DateTime), CAST(N'2024-11-28T10:45:37.577' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (607, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T10:45:37.580' AS DateTime), CAST(N'2024-11-28T12:14:33.590' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (608, 79, 4, N'0', 1, 1, CAST(N'2024-11-28T12:14:33.600' AS DateTime), CAST(N'2024-11-28T12:18:06.353' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (609, 79, 4, N'11', 1, 1, CAST(N'2024-11-28T12:18:06.353' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (610, 82, 4, N'0', 1, 1, CAST(N'2024-12-05T09:32:10.210' AS DateTime), CAST(N'2024-12-05T09:32:10.210' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (611, 82, 4, N'11', 1, 1, CAST(N'2024-12-05T09:32:15.770' AS DateTime), CAST(N'2024-12-05T09:33:13.437' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (612, 82, 4, N'0', 1, 1, CAST(N'2024-12-05T09:33:13.447' AS DateTime), CAST(N'2024-12-05T09:38:17.290' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (613, 82, 4, N'11', 1, 1, CAST(N'2024-12-05T09:38:17.297' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (614, 83, 4, N'0', 1, 1, CAST(N'2024-12-05T10:54:42.277' AS DateTime), CAST(N'2024-12-05T10:54:42.277' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (615, 83, 4, N'11', 1, 1, CAST(N'2024-12-05T10:54:42.283' AS DateTime), CAST(N'2024-12-05T10:54:59.707' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (616, 83, 4, N'0', 1, 1, CAST(N'2024-12-05T10:54:59.720' AS DateTime), CAST(N'2024-12-05T10:55:01.390' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (617, 83, 4, N'11', 1, 1, CAST(N'2024-12-05T10:55:01.397' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (618, 84, 4, N'0', 1, 1, CAST(N'2024-12-05T11:12:20.590' AS DateTime), CAST(N'2024-12-05T11:12:20.590' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (619, 84, 4, N'11', 1, 1, CAST(N'2024-12-05T11:12:20.597' AS DateTime), CAST(N'2024-12-05T11:12:44.900' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (620, 84, 4, N'0', 1, 1, CAST(N'2024-12-05T11:12:44.910' AS DateTime), CAST(N'2024-12-05T11:12:45.940' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (621, 84, 4, N'11', 1, 1, CAST(N'2024-12-05T11:12:45.943' AS DateTime), NULL, N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (622, 85, 4, N'0', 1, 1, CAST(N'2024-12-05T11:27:44.980' AS DateTime), CAST(N'2024-12-05T11:27:44.980' AS DateTime), N'Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (623, 85, 4, N'11', 1, 1, CAST(N'2024-12-05T11:27:44.987' AS DateTime), CAST(N'2024-12-05T11:27:59.733' AS DateTime), N'A STUDENT DETAILS 1')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (624, 85, 4, N'0', 1, 1, CAST(N'2024-12-05T11:27:59.743' AS DateTime), CAST(N'2024-12-05T11:28:01.497' AS DateTime), N'Back Initial Step')
INSERT [dbo].[_old_foStepHist] ([ID], [ProcessInstanceID], [ProcessID], [ProcessStepID], [FromUserID], [ToUserID], [DateStart], [DateEnd], [Description]) VALUES (625, 85, 4, N'11', 1, 1, CAST(N'2024-12-05T11:28:01.500' AS DateTime), NULL, N'A STUDENT DETAILS 1')
SET IDENTITY_INSERT [dbo].[_old_foStepHist] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foTaskControls] ON 

INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (91, 11, N'ID', 0, NULL, NULL, 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (92, 11, N'StudentID', 0, NULL, NULL, 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (93, 11, N'FirstName', 1, N'SELECT   [TableName],[ReconDate],[RecordCount],[RecordSum],[InsertDate] FROM [dbJumpman_Dev].[dbo].[Recon]', N'FirstName=TableName;LastName=InsertDate', 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (94, 11, N'LastName', 1, NULL, NULL, 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (95, 11, N'DOB', 1, NULL, NULL, 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (96, 11, N'CampusID', 1, N'SELECT  1''Test'', [TableName],[ReconDate],[RecordCount],[RecordSum],[InsertDate] FROM [dbJumpman_Dev].[dbo].[Recon]', N'CampusID=[TableName]', 1)
INSERT [dbo].[_old_foTaskControls] ([ID], [ProcessStepsID], [ColumnName], [Selected], [BrowsePage_Query], [BrowsePage_Mapping], [Require]) VALUES (97, 11, N'Age', 1, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[_old_foTaskControls] OFF
GO
SET IDENTITY_INSERT [dbo].[_old_foUsers] ON 

INSERT [dbo].[_old_foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Active]) VALUES (1, N'schalkvdm', N'1234', N'Schalk', N'vd Merwe', N'schalk83.vandermerwe@gmail.com', 1)
INSERT [dbo].[_old_foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Active]) VALUES (2, N'freedomn', N'123', N'Freedom', N'Nxumalo', N'', 1)
INSERT [dbo].[_old_foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Active]) VALUES (3, N'moniquea', N'123', N'Monique', N'Abrahams', N'monique@gmail.com', 1)
SET IDENTITY_INSERT [dbo].[_old_foUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[foAdminTables] ON 

INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (12, N'foUserReports', 1, N'Reports', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (2, N'foReports', 1, N'Reports', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (3, N'foReportTable', NULL, N'Reports', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (13, N'foProcessSteps', NULL, N'Process', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (5, N'foTableColumnsToIgnore', NULL, N'Table', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (6, N'foTablePrefixes', NULL, N'Table', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (14, N'foProcess', NULL, N'Process', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (19, N'foProcessDetail', NULL, N'Process', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (9, N'foUserReports', NULL, N'Users', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (10, N'foUsers', 1, N'Users', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (11, N'foUserTable', NULL, N'Users', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (20, N'foEmailTemplate', NULL, N'Notifications', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (21, N'foTable', NULL, N'Table', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (22, N'foReportTableQuery', NULL, N'Reports', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (23, N'foTableGroups', NULL, N'Table', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (15, N'foUserProcess', NULL, N'Process', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (16, N'foGroups', NULL, N'Users', 1)
INSERT [dbo].[foAdminTables] ([ID], [TableName], [Parent], [TableGroup], [Active]) VALUES (17, N'foUserGroups', NULL, N'Users', 1)
SET IDENTITY_INSERT [dbo].[foAdminTables] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalAttachments] ON 

INSERT [dbo].[foApprovalAttachments] ([ID], [ApprovalID], [AttachmentDescription], [AttachmentPath], [CreatedDate], [CreatedbyID], [Active]) VALUES (8, 63, N'sas', N'Attachments\Approvals\GL COMPARE - NNBNBNB JUMPMAN.sql', CAST(N'2025-05-04T16:15:51.667' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[foApprovalAttachments] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalEvents] ON 

INSERT [dbo].[foApprovalEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (428, 16, 10, -681, 2, NULL, CAST(N'2025-05-23T13:10:45.440' AS DateTime), NULL, 1)
INSERT [dbo].[foApprovalEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (429, 18, 10, -686, NULL, 1, CAST(N'2025-05-23T13:49:45.730' AS DateTime), CAST(N'2025-05-23T13:52:12.997' AS DateTime), 1)
INSERT [dbo].[foApprovalEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (430, 21, 11, -698, NULL, 1, CAST(N'2025-05-23T16:34:43.077' AS DateTime), CAST(N'2025-05-23T16:38:27.610' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[foApprovalEvents] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalEventsArchive] ON 

INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (384, 1, 10, -612, NULL, 1, CAST(N'2025-05-04T14:57:07.467' AS DateTime), CAST(N'2025-05-04T15:00:06.470' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (385, 1, 10, -617, NULL, 1, CAST(N'2025-05-04T15:33:11.853' AS DateTime), CAST(N'2025-05-04T16:15:51.603' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (386, 3, 0, NULL, NULL, 1, CAST(N'2025-05-04T16:34:09.287' AS DateTime), CAST(N'2025-05-04T16:34:23.847' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (387, 2, 10, NULL, NULL, 1, CAST(N'2025-05-04T15:07:00.353' AS DateTime), CAST(N'2025-05-04T15:09:07.430' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (388, 4, 0, NULL, NULL, 1, CAST(N'2025-05-04T16:34:45.953' AS DateTime), CAST(N'2025-05-04T17:07:56.067' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (389, 7, 0, NULL, NULL, 1, CAST(N'2025-05-04T17:27:53.017' AS DateTime), CAST(N'2025-05-04T17:28:01.267' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (390, 8, 0, NULL, NULL, 2, CAST(N'2025-05-04T21:11:40.753' AS DateTime), NULL, 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (391, 8, 0, NULL, NULL, 1, CAST(N'2025-05-04T21:11:40.760' AS DateTime), CAST(N'2025-05-05T09:53:36.187' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (392, 10, 0, NULL, NULL, 2, CAST(N'2025-05-05T09:54:57.540' AS DateTime), CAST(N'2025-05-05T09:55:18.287' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (393, 10, 0, NULL, NULL, 1, CAST(N'2025-05-05T09:54:57.543' AS DateTime), CAST(N'2025-05-05T09:55:13.927' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (394, 12, 0, NULL, NULL, 2, CAST(N'2025-05-08T22:14:25.463' AS DateTime), CAST(N'2025-05-08T22:17:19.697' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (395, 12, 0, NULL, NULL, 1, CAST(N'2025-05-08T22:14:25.830' AS DateTime), CAST(N'2025-05-08T22:17:18.213' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (396, 6, 0, NULL, NULL, 1, CAST(N'2025-05-04T17:18:54.320' AS DateTime), CAST(N'2025-05-04T17:19:13.580' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (397, 11, 10, NULL, 2, NULL, CAST(N'2025-05-05T12:46:01.120' AS DateTime), NULL, 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (398, 10, 0, -632, NULL, 2, CAST(N'2025-05-05T09:54:57.540' AS DateTime), CAST(N'2025-05-05T09:55:18.287' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (399, 10, 0, -632, NULL, 1, CAST(N'2025-05-05T09:54:57.543' AS DateTime), CAST(N'2025-05-05T09:55:13.927' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (400, 10, 0, -633, NULL, 1, CAST(N'2025-05-09T12:54:52.520' AS DateTime), CAST(N'2025-05-09T12:59:35.643' AS DateTime), 1)
INSERT [dbo].[foApprovalEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active]) VALUES (401, 10, 0, -633, NULL, 2, CAST(N'2025-05-09T12:54:53.247' AS DateTime), CAST(N'2025-05-09T13:02:04.420' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[foApprovalEventsArchive] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalEventsDetail] ON 

INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (87, 408, 2, 10, 62, N'{"StepDescription":"Approve Student Details","Decision":"Rework","Comment":"d","CreatedUserID":1,"CreatedDate":"2025-05-04T15:09:07.446182+02:00"}', CAST(N'2025-05-04T15:09:07.467' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (92, 412, 6, 0, 67, N'{"StepDescription":null,"Decision":"Rework","Comment":"D","CreatedUserID":1,"CreatedDate":"2025-05-04T17:19:13.6056964+02:00"}', CAST(N'2025-05-04T17:19:13.597' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (94, 415, 8, 0, 69, N'{"StepDescription":null,"Decision":"Approve","Comment":"asdasd","CreatedUserID":1,"CreatedDate":"2025-05-04T21:11:52.9770355+02:00"}', CAST(N'2025-05-04T21:11:52.963' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (101, 429, 18, 10, 76, N'{"StepDescription":"Approve Student Details","Decision":"Rework","Comment":"A","CreatedUserID":1,"CreatedDate":"2025-05-23T13:52:13.0818953+02:00"}', CAST(N'2025-05-23T13:52:13.030' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (103, 430, 21, 11, 78, N'{"StepDescription":"Asset Approval","Decision":"Rework","Comment":"test","CreatedUserID":1,"CreatedDate":"2025-05-23T16:38:27.9105221+02:00"}', CAST(N'2025-05-23T16:38:27.950' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (93, 413, 7, 0, 68, N'{"StepDescription":null,"Decision":"Rework","Comment":"dd","CreatedUserID":1,"CreatedDate":"2025-05-04T17:28:01.2861203+02:00"}', CAST(N'2025-05-04T17:28:01.280' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (89, 410, 3, 0, 64, N'{"StepDescription":null,"Decision":"Rework","Comment":"sss","CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:23.8716068+02:00"}', CAST(N'2025-05-04T16:34:23.870' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (96, 415, 8, 0, 71, N'{"StepDescription":null,"Decision":"Approve","Comment":"DSD","CreatedUserID":1,"CreatedDate":"2025-05-05T09:53:36.1946958+02:00"}', CAST(N'2025-05-05T09:53:36.197' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (98, 421, 12, 0, 73, N'{"StepDescription":null,"Decision":"Rework","Comment":"sss","CreatedUserID":1,"CreatedDate":"2025-05-08T22:17:18.8769014+02:00"}', CAST(N'2025-05-08T22:17:18.953' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetail] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (91, 411, 4, 0, 66, N'{"StepDescription":null,"Decision":"Rework","Comment":"fdsdf","CreatedUserID":1,"CreatedDate":"2025-05-04T17:07:56.0996396+02:00"}', CAST(N'2025-05-04T17:07:56.107' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[foApprovalEventsDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalEventsDetailArchive] ON 

INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (55, 407, 1, 10, 61, N'{"StepDescription":"Approve Student Details","Decision":"Rework","Comment":"FFF","CreatedUserID":1,"CreatedDate":"2025-05-04T15:00:06.4891439+02:00"}', CAST(N'2025-05-04T15:00:06.483' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (56, 409, 1, 10, 63, N'{"StepDescription":"Approve Student Details","Decision":"Approve","Comment":"sss","CreatedUserID":1,"CreatedDate":"2025-05-04T16:15:51.625773+02:00"}', CAST(N'2025-05-04T16:15:51.653' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (57, 408, 2, 10, 62, N'{"StepDescription":"Approve Student Details","Decision":"Rework","Comment":"d","CreatedUserID":1,"CreatedDate":"2025-05-04T15:09:07.446182+02:00"}', CAST(N'2025-05-04T15:09:07.467' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (58, 412, 6, 0, 67, N'{"StepDescription":null,"Decision":"Rework","Comment":"D","CreatedUserID":1,"CreatedDate":"2025-05-04T17:19:13.6056964+02:00"}', CAST(N'2025-05-04T17:19:13.597' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (59, 415, 8, 0, 69, N'{"StepDescription":null,"Decision":"Approve","Comment":"asdasd","CreatedUserID":1,"CreatedDate":"2025-05-04T21:11:52.9770355+02:00"}', CAST(N'2025-05-04T21:11:52.963' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (60, 413, 7, 0, 68, N'{"StepDescription":null,"Decision":"Rework","Comment":"dd","CreatedUserID":1,"CreatedDate":"2025-05-04T17:28:01.2861203+02:00"}', CAST(N'2025-05-04T17:28:01.280' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (61, 410, 3, 0, 64, N'{"StepDescription":null,"Decision":"Rework","Comment":"sss","CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:23.8716068+02:00"}', CAST(N'2025-05-04T16:34:23.870' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (62, 415, 8, 0, 71, N'{"StepDescription":null,"Decision":"Approve","Comment":"DSD","CreatedUserID":1,"CreatedDate":"2025-05-05T09:53:36.1946958+02:00"}', CAST(N'2025-05-05T09:53:36.197' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (63, 417, 10, 0, 72, N'{"StepDescription":null,"Decision":"Rework","Comment":"ASDASD","CreatedUserID":1,"CreatedDate":"2025-05-05T09:55:13.9325882+02:00"}', CAST(N'2025-05-05T09:55:13.933' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (64, 421, 12, 0, 73, N'{"StepDescription":null,"Decision":"Rework","Comment":"sss","CreatedUserID":1,"CreatedDate":"2025-05-08T22:17:18.8769014+02:00"}', CAST(N'2025-05-08T22:17:18.953' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (65, 411, 4, 0, 66, N'{"StepDescription":null,"Decision":"Rework","Comment":"fdsdf","CreatedUserID":1,"CreatedDate":"2025-05-04T17:07:56.0996396+02:00"}', CAST(N'2025-05-04T17:07:56.107' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (66, 426, 10, 0, 74, N'{"StepDescription":null,"Decision":"Approve","Comment":"","CreatedUserID":1,"CreatedDate":"2025-05-09T12:59:36.3396744+02:00"}', CAST(N'2025-05-09T12:59:36.383' AS DateTime), 1, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (67, 427, 10, 0, 75, N'{"StepDescription":null,"Decision":"Approve","Comment":"","CreatedUserID":2,"CreatedDate":"2025-05-09T13:02:05.0993694+02:00"}', CAST(N'2025-05-09T13:02:05.140' AS DateTime), 2, 1)
INSERT [dbo].[foApprovalEventsDetailArchive] ([ID], [ApprovalEventID], [ProcessInstanceID], [StepID], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (68, 417, 10, 0, 72, N'{"StepDescription":null,"Decision":"Rework","Comment":"ASDASD","CreatedUserID":1,"CreatedDate":"2025-05-05T09:55:13.9325882+02:00"}', CAST(N'2025-05-05T09:55:13.933' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[foApprovalEventsDetailArchive] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovals] ON 

INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (61, 1, 407, 10, N'Rework', N'FFF', CAST(N'2025-05-04T15:00:06.477' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (62, 2, 408, 10, N'Rework', N'd', CAST(N'2025-05-04T15:09:07.437' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (63, 1, 409, 10, N'Approve', N'sss', CAST(N'2025-05-04T16:15:51.613' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (64, 3, 410, 0, N'Rework', N'sss', CAST(N'2025-05-04T16:34:23.857' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (66, 4, 411, 0, N'Rework', N'fdsdf', CAST(N'2025-05-04T17:07:56.077' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (67, 6, 412, 0, N'Rework', N'D', CAST(N'2025-05-04T17:19:13.590' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (68, 7, 413, 0, N'Rework', N'dd', CAST(N'2025-05-04T17:28:01.273' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (69, 8, 415, 0, N'Approve', N'asdasd', CAST(N'2025-05-04T21:11:52.957' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (71, 8, 415, 0, N'Approve', N'DSD', CAST(N'2025-05-05T09:53:36.190' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (72, 10, 417, 0, N'Rework', N'ASDASD', CAST(N'2025-05-05T09:55:13.930' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (73, 12, 421, 0, N'Rework', N'sss', CAST(N'2025-05-08T22:17:18.583' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (74, 10, 426, 0, N'Approve', NULL, CAST(N'2025-05-09T12:59:36.017' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (75, 10, 427, 0, N'Approve', NULL, CAST(N'2025-05-09T13:02:04.780' AS DateTime), 2, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (76, 18, 429, 10, N'Rework', N'A', CAST(N'2025-05-23T13:52:13.013' AS DateTime), 1, 1)
INSERT [dbo].[foApprovals] ([ID], [ProcessInstanceID], [ApprovalEventID], [StepID], [Decision], [Comment], [CreatedDate], [CreatedUserID], [Active]) VALUES (78, 21, 430, 11, N'Rework', N'test', CAST(N'2025-05-23T16:38:27.887' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[foApprovals] OFF
GO
SET IDENTITY_INSERT [dbo].[foApprovalSteps] ON 

INSERT [dbo].[foApprovalSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (10, 4, CAST(1.00 AS Decimal(18, 2)), N'Approve Student Details', 2, NULL, 1)
INSERT [dbo].[foApprovalSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (11, 8, CAST(1.00 AS Decimal(18, 2)), N'Asset Approval', NULL, 1, 1)
INSERT [dbo].[foApprovalSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (3, 1, CAST(1.00 AS Decimal(18, 2)), N'Approval 1 Finance', 1, NULL, 1)
INSERT [dbo].[foApprovalSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (4, 1, CAST(2.00 AS Decimal(18, 2)), N'Approval Step 2 Finance', NULL, 2, 1)
SET IDENTITY_INSERT [dbo].[foApprovalSteps] OFF
GO
SET IDENTITY_INSERT [dbo].[foEmailNotifications] ON 

INSERT [dbo].[foEmailNotifications] ([ID], [ProcessInstanceID], [StepID], [GroupID], [UserID], [EmailTemplateID], [DateSent], [Active]) VALUES (1, 11, 635, 2, 0, 1, NULL, 1)
SET IDENTITY_INSERT [dbo].[foEmailNotifications] OFF
GO
SET IDENTITY_INSERT [dbo].[foEmailTemplate] ON 

INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (1, N'New', N'New Process {{ProcessID}} and {{Description}}.', N'Hello, a new process with ID {{ProcessID}} has been created: {{Description}}.', CAST(N'2025-05-05T10:07:22.510' AS DateTime), 0)
INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (2, N'Pending', N'Pending Process {{ProcessID}} and {{Description}}', N'<table width="100%" cellpadding="0" cellspacing="0" style="font-family: ''Segoe UI'', sans-serif; background-color: #f4f4f4; padding: 30px 0;">
  <tr>
    <td align="center">
      <table width="600" cellpadding="0" cellspacing="0" style="background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.08);">
        
        <!-- Logo -->
        <tr>
          <td style="padding: 20px 0 0; text-align: center;">
            <span style="font-size: 24px; font-weight: bold; color: #4CAF50;">fresch<span style="color: #000;">One</span></span>
          </td>
        </tr>

        <!-- Content -->
        <tr>
          <td style="padding: 20px 30px;">
            <p style="margin: 0; font-size: 15px; color: #333;">Hello,</p>
            <p style="margin: 8px 0 16px; font-size: 14px; color: #555;">
              A new process has been created in the system. Here are the details:
            </p>

            <table cellpadding="10" cellspacing="0" width="100%" style="background-color: #f9f9f9; border-radius: 6px;">
              <tr>
                <td style="font-weight: bold; width: 120px;">Process ID:</td>
                <td>{{ProcessID}}</td>
              </tr>
              <tr>
                <td style="font-weight: bold;">Description:</td>
                <td>{{Description}}</td>
              </tr>
            </table>

            <div style="text-align: center; margin-top: 20px;">
              <a href="#" style="display: inline-block; padding: 10px 22px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 14px; border-radius: 6px;">
                View Process
              </a>
            </div>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style="background-color: #f0f0f0; text-align: center; font-size: 12px; color: #777; padding: 12px;">
            This is an automated message. Do not reply.
          </td>
        </tr>

      </table>
    </td>
  </tr>
</table>
', CAST(N'2025-05-14T15:04:20.757' AS DateTime), 1)
INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (3, N'New', N'New Process {{ProcessID}} and {{Description}}.', N'Hello, a new process with ID {{ProcessID}} has been created: {{Description}}.', CAST(N'2025-05-05T11:56:31.350' AS DateTime), 1)
INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (4, N'Late', N'Late Process {{ProcessID}} and {{Description}}', N'Hello, a Lateprocess with ID {{ProcessID}} has been created: {{Description}}.', CAST(N'2025-05-14T11:53:30.150' AS DateTime), 1)
INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (5, N'Generic', N'New Process {{ProcessID}} - {{Description}}', N'<table width="100%" cellpadding="0" cellspacing="0" style="font-family: ''Segoe UI'', sans-serif; background-color: #f4f4f4; padding: 30px 0;">
          <tr>
            <td align="center">
              <table width="600" cellpadding="0" cellspacing="0" style="background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.08);">
                <tr>
                  <td style="padding: 20px 0 0; text-align: center;">
                    <span style="font-size: 24px; font-weight: bold; color: #4CAF50;">fresch<span style="color: #000;">One</span></span>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 20px 30px;">
                    <p style="margin: 0; font-size: 15px; color: #333;">Hello,</p>
                    <p style="margin: 8px 0 16px; font-size: 14px; color: #555;">
                      A new process has been created in the system. Here are the details:
                    </p>
                    <table cellpadding="10" cellspacing="0" width="100%" style="background-color: #f9f9f9; border-radius: 6px;">
                      <tr>
                        <td style="font-weight: bold; width: 120px;">Process ID:</td>
                        <td>{{ProcessID}}</td>
                      </tr>
                      <tr>
                        <td style="font-weight: bold;">Description:</td>
                        <td>{{Description}}</td>
                      </tr>
                    </table>
                    <div style="text-align: center; margin-top: 20px;">
                      <a href="#" style="display: inline-block; padding: 10px 22px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 14px; border-radius: 6px;">
                        View Process
                      </a>
                    </div>
                  </td>
                </tr>
                <tr>
                  <td style="background-color: #f0f0f0; text-align: center; font-size: 12px; color: #777; padding: 12px;">
                    This is an automated message. Do not reply.
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>', CAST(N'2025-05-14T15:14:09.090' AS DateTime), 1)
INSERT [dbo].[foEmailTemplate] ([ID], [TemplateType], [EmailSubject], [EmailBody], [ActiveDate], [Active]) VALUES (6, N'Generic Test', N'New Process {{ProcessID}} - {{Description}}', N'<table width="100%" cellpadding="0" cellspacing="0" style="font-family: ''Segoe UI'', sans-serif; background-color: #f4f4f4; padding: 30px 0;">
          <tr>
            <td align="center">
              <table width="600" cellpadding="0" cellspacing="0" style="background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.08);">
                <tr>
                  <td style="padding: 20px 0 0; text-align: center;">
                    <span style="font-size: 24px; font-weight: bold; color: #4CAF50;">fresch<span style="color: #000;">One</span></span>
                  </td>
                </tr>
                <tr>
                  <td style="padding: 20px 30px;">
                    <p style="margin: 0; font-size: 15px; color: #333;">Hello,</p>
                    <p style="margin: 8px 0 16px; font-size: 14px; color: #555;">
                      A new process has been created in the system. Here are the details:
                    </p>
                    <table cellpadding="10" cellspacing="0" width="100%" style="background-color: #f9f9f9; border-radius: 6px;">
                      <tr>
                        <td style="font-weight: bold; width: 120px;">Process ID:</td>
                        <td>{{ProcessID}}</td>
                      </tr>
                      <tr>
                        <td style="font-weight: bold;">Description:</td>
                        <td>{{Description}}</td>
                      </tr>
                    </table>
                    <div style="text-align: center; margin-top: 20px;">
                      <a href="#" style="display: inline-block; padding: 10px 22px; background-color: #4CAF50; color: white; text-decoration: none; font-size: 14px; border-radius: 6px;">
                        View Process
                      </a>
                    </div>
                  </td>
                </tr>
                <tr>
                  <td style="background-color: #f0f0f0; text-align: center; font-size: 12px; color: #777; padding: 12px;">
                    This is an automated message. Do not reply.
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>', CAST(N'2025-05-16T11:54:16.743' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[foEmailTemplate] OFF
GO
SET IDENTITY_INSERT [dbo].[foGroups] ON 

INSERT [dbo].[foGroups] ([ID], [Description]) VALUES (1, N'Finance')
INSERT [dbo].[foGroups] ([ID], [Description]) VALUES (2, N'HR')
INSERT [dbo].[foGroups] ([ID], [Description]) VALUES (3, N'Security')
SET IDENTITY_INSERT [dbo].[foGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcess] ON 

INSERT [dbo].[foProcess] ([ID], [ProcessName], [ProcessDescription], [Active]) VALUES (4, N'Capture New Student', N'BLABLA', 1)
INSERT [dbo].[foProcess] ([ID], [ProcessName], [ProcessDescription], [Active]) VALUES (5, N'No Approval', N'as', 1)
INSERT [dbo].[foProcess] ([ID], [ProcessName], [ProcessDescription], [Active]) VALUES (7, N'eXPENSES', N'A', 1)
INSERT [dbo].[foProcess] ([ID], [ProcessName], [ProcessDescription], [Active]) VALUES (6, N'Asset', N'x', 0)
INSERT [dbo].[foProcess] ([ID], [ProcessName], [ProcessDescription], [Active]) VALUES (8, N'Asset Management', N'Asset Management', 1)
SET IDENTITY_INSERT [dbo].[foProcess] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessDetail] ON 

INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (28, 17, N'tbl_Tran_Student', N'*', N'F', 1, 1, NULL, N'Add Student Details1', 1, NULL, NULL)
INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (33, 18, N'tbl_tran_Country', N'*', N'T', 1, 0, NULL, N'Countries Visited', 1, NULL, NULL)
INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (34, 17, N'tbl_tran_StudentDetails1', N'*', N'T', 0, 0, N'StudentID', N'StudentDetails11', 1, NULL, NULL)
INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (32, 19, N'tbl_Tran_Student', N'*', N'F', 2, 1, NULL, N'sss', 1, NULL, NULL)
INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (36, 22, N'tbl_tran_Asset', N'*', N'F', 1, 1, NULL, N'Capture Step', 1, NULL, NULL)
INSERT [dbo].[foProcessDetail] ([ID], [StepID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active], [ColumnCalcs], [ListTable]) VALUES (35, 20, N'tbl_tran_Country', N'*', N'F', 2, 1, NULL, N'Country 1', 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[foProcessDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessEvents] ON 

INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (671, 13, 17, 0, NULL, 1, CAST(N'2025-05-19T14:27:06.263' AS DateTime), CAST(N'2025-05-19T14:27:06.377' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (672, 13, 17, 671, NULL, 1, CAST(N'2025-05-19T14:27:06.390' AS DateTime), CAST(N'2025-05-19T14:27:16.370' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (673, 13, 17, 672, NULL, 1, CAST(N'2025-05-19T14:27:16.370' AS DateTime), CAST(N'2025-05-19T14:50:44.210' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (674, 13, 17, 673, NULL, 1, CAST(N'2025-05-19T14:50:44.210' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (675, 14, 17, 0, NULL, 1, CAST(N'2025-05-21T21:38:04.940' AS DateTime), CAST(N'2025-05-21T21:38:05.347' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (676, 14, 17, 675, NULL, 1, CAST(N'2025-05-21T21:38:05.350' AS DateTime), CAST(N'2025-05-22T13:05:57.353' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (677, 14, 17, 676, NULL, 1, CAST(N'2025-05-22T13:05:57.353' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (678, 15, 17, 0, NULL, 1, CAST(N'2025-05-22T20:03:29.743' AS DateTime), CAST(N'2025-05-22T20:03:29.930' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (679, 15, 18, 678, NULL, 2, CAST(N'2025-05-22T20:03:29.947' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (680, 16, 17, 0, NULL, 2, CAST(N'2025-05-23T12:44:14.797' AS DateTime), CAST(N'2025-05-23T12:44:15.040' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (681, 16, 18, 680, NULL, 2, CAST(N'2025-05-23T12:44:15.073' AS DateTime), CAST(N'2025-05-23T13:10:45.360' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (682, 17, 17, 0, NULL, 2, CAST(N'2025-05-23T12:44:34.547' AS DateTime), CAST(N'2025-05-23T12:44:34.623' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (683, 17, 17, 682, NULL, 2, CAST(N'2025-05-23T12:44:34.640' AS DateTime), CAST(N'2025-05-23T13:05:36.137' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (684, 17, 18, 683, NULL, 2, CAST(N'2025-05-23T13:05:36.170' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (685, 18, 17, 0, NULL, 2, CAST(N'2025-05-23T13:49:04.247' AS DateTime), CAST(N'2025-05-23T13:49:04.577' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (686, 18, 18, 685, NULL, 2, CAST(N'2025-05-23T13:49:04.590' AS DateTime), CAST(N'2025-05-23T13:49:45.700' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (687, 18, 17, -429, NULL, 2, CAST(N'2025-05-23T13:52:13.077' AS DateTime), CAST(N'2025-05-23T13:52:22.100' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (688, 19, 17, 0, NULL, 2, CAST(N'2025-05-23T13:52:45.527' AS DateTime), CAST(N'2025-05-23T13:52:45.603' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (689, 19, 18, 688, NULL, 2, CAST(N'2025-05-23T13:52:45.620' AS DateTime), CAST(N'2025-05-23T13:52:51.067' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (690, 19, 18, 689, NULL, 2, CAST(N'2025-05-23T13:52:51.067' AS DateTime), CAST(N'2025-05-23T13:53:02.467' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (691, 19, 18, 690, NULL, 2, CAST(N'2025-05-23T13:53:02.480' AS DateTime), CAST(N'2025-05-23T13:53:22.123' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (692, 19, 18, 691, NULL, 2, CAST(N'2025-05-23T13:53:22.123' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (693, 20, 17, 0, NULL, 2, CAST(N'2025-05-23T13:58:05.013' AS DateTime), CAST(N'2025-05-23T13:58:05.500' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (694, 20, 18, 693, NULL, 2, CAST(N'2025-05-23T13:58:05.530' AS DateTime), CAST(N'2025-05-23T14:14:14.107' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (695, 20, 18, 694, NULL, 2, CAST(N'2025-05-23T14:14:14.123' AS DateTime), CAST(N'2025-05-23T14:14:20.040' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (696, 20, 18, 695, NULL, 2, CAST(N'2025-05-23T14:14:20.040' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (697, 21, 22, 0, NULL, 2, CAST(N'2025-05-23T16:34:31.323' AS DateTime), CAST(N'2025-05-23T16:34:31.433' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (698, 21, 22, 697, NULL, 2, CAST(N'2025-05-23T16:34:31.463' AS DateTime), CAST(N'2025-05-23T16:34:42.933' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEvents] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (699, 21, 22, -430, NULL, 2, CAST(N'2025-05-23T16:38:28.057' AS DateTime), CAST(N'2025-05-23T16:43:35.510' AS DateTime), 1, 0)
SET IDENTITY_INSERT [dbo].[foProcessEvents] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessEventsArchive] ON 

INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (1, 12, 19, 0, NULL, 1, CAST(N'2025-05-08T22:14:23.110' AS DateTime), CAST(N'2025-05-08T22:14:24.723' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (2, 12, 19, -421, NULL, 1, CAST(N'2025-05-08T22:17:20.430' AS DateTime), CAST(N'2025-05-08T22:22:47.940' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (3, 1, 17, 0, NULL, 2, CAST(N'2025-05-04T14:56:59.633' AS DateTime), CAST(N'2025-05-04T14:56:59.720' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (4, 1, 18, 611, NULL, 2, CAST(N'2025-05-04T14:56:59.737' AS DateTime), CAST(N'2025-05-04T14:57:07.430' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (5, 1, 17, -407, NULL, 2, CAST(N'2025-05-04T15:00:06.523' AS DateTime), CAST(N'2025-05-04T15:32:49.580' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (6, 2, 17, 0, NULL, 2, CAST(N'2025-05-04T15:00:49.177' AS DateTime), CAST(N'2025-05-04T15:00:49.260' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (7, 2, 18, 614, NULL, 2, CAST(N'2025-05-04T15:00:49.273' AS DateTime), CAST(N'2025-05-04T15:07:00.320' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (8, 2, 17, -408, NULL, 2, CAST(N'2025-05-04T15:09:07.507' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (9, 1, 18, 613, NULL, 2, CAST(N'2025-05-04T15:32:49.593' AS DateTime), CAST(N'2025-05-04T15:33:11.820' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (10, 3, 19, 0, NULL, 1, CAST(N'2025-05-04T16:34:09.120' AS DateTime), CAST(N'2025-05-04T16:34:09.263' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (11, 3, 0, -410, NULL, 0, CAST(N'2025-05-04T16:34:23.940' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (12, 4, 19, 0, NULL, 1, CAST(N'2025-05-04T16:34:45.890' AS DateTime), CAST(N'2025-05-04T16:34:45.930' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (13, 4, 0, -411, NULL, 1, CAST(N'2025-05-04T17:07:56.130' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (14, 5, 19, 0, NULL, 1, CAST(N'2025-05-04T17:18:31.747' AS DateTime), CAST(N'2025-05-04T17:18:31.797' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (15, 5, 19, 622, NULL, 1, CAST(N'2025-05-04T17:18:31.803' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (16, 6, 19, 0, NULL, 1, CAST(N'2025-05-04T17:18:54.277' AS DateTime), CAST(N'2025-05-04T17:18:54.307' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (17, 6, 0, -412, NULL, 1, CAST(N'2025-05-04T17:19:13.617' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (18, 8, 19, 0, NULL, 1, CAST(N'2025-05-04T21:11:40.570' AS DateTime), CAST(N'2025-05-04T21:11:40.737' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (19, 9, 17, 0, NULL, 1, CAST(N'2025-05-05T09:50:18.700' AS DateTime), CAST(N'2025-05-05T09:50:18.950' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (20, 9, 17, 630, NULL, 1, CAST(N'2025-05-05T09:50:18.957' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (21, 10, 19, 0, NULL, 1, CAST(N'2025-05-05T09:54:57.513' AS DateTime), CAST(N'2025-05-05T09:54:57.530' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (22, 10, 19, -417, NULL, 1, CAST(N'2025-05-05T09:55:18.290' AS DateTime), NULL, 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (23, 11, 17, 0, NULL, 2, CAST(N'2025-05-05T12:29:40.823' AS DateTime), CAST(N'2025-05-05T12:29:56.597' AS DateTime), 1, 0)
INSERT [dbo].[foProcessEventsArchive] ([ID], [ProcessInstanceID], [StepID], [PreviousEventID], [GroupID], [UserID], [DateAssigned], [DateCompleted], [Active], [Cancelled]) VALUES (24, 11, 18, 634, NULL, 2, CAST(N'2025-05-05T12:30:16.103' AS DateTime), CAST(N'2025-05-05T12:45:59.570' AS DateTime), 1, 0)
SET IDENTITY_INSERT [dbo].[foProcessEventsArchive] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessEventsDetail] ON 

INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (750, 671, 13, 17, N'tbl_Tran_Student', 525, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:06.3814698+02:00","RecordID":525,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-19T14:27:06.360' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (751, 671, 13, 17, N'tbl_tran_StudentDetails1', 667, N'{"FirstName":"x","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":525,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:06.4673238+02:00","RecordID":667,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-19T14:27:06.360' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (752, 671, 13, 17, N'tbl_tran_StudentDetails2', 555, N'{"FirstName":"y","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":525,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:06.4782653+02:00","RecordID":555,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-19T14:27:06.377' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (753, 672, 13, 17, N'tbl_Tran_Student', 525, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:27:16.4418083+02:00","RecordID":525,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:16.444577+02:00"}', CAST(N'2025-05-19T14:27:16.340' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (754, 672, 13, 17, N'tbl_tran_StudentDetails1', 667, N'{"FirstName":"x","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":525,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:27:16.4518792+02:00","RecordID":667,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:16.4578499+02:00"}', CAST(N'2025-05-19T14:27:16.353' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (755, 672, 13, 17, N'tbl_tran_StudentDetails2', 555, N'{"FirstName":"y","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":525,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:27:16.4625777+02:00","RecordID":555,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:27:16.468262+02:00"}', CAST(N'2025-05-19T14:27:16.370' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (756, 673, 13, 17, N'tbl_Tran_Student', 525, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:50:44.2464836+02:00","RecordID":525,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:50:44.253516+02:00"}', CAST(N'2025-05-19T14:50:44.180' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (757, 673, 13, 17, N'tbl_tran_StudentDetails1', 667, N'{"FirstName":"x","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":525,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:50:44.2836486+02:00","RecordID":667,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:50:44.2859994+02:00"}', CAST(N'2025-05-19T14:50:44.180' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (758, 673, 13, 17, N'tbl_tran_StudentDetails2', 555, N'{"FirstName":"y1","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":525,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-19T14:50:44.2925117+02:00","RecordID":555,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-19T14:50:44.307278+02:00"}', CAST(N'2025-05-19T14:50:44.210' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (759, 675, 14, 17, N'tbl_Tran_Student', 526, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_Location":"-33.723227, 18.451842","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-21T21:38:05.0198503+02:00","RecordID":526,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-21T21:38:05.177' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (760, 675, 14, 17, N'tbl_tran_StudentDetails1', 668, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":526,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-21T21:38:05.2678392+02:00","RecordID":668,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-21T21:38:05.270' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (761, 675, 14, 17, N'tbl_tran_StudentDetails2', 556, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":526,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-21T21:38:05.3206288+02:00","RecordID":556,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-21T21:38:05.317' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (762, 676, 14, 17, N'tbl_Tran_Student', 526, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-22T13:05:57.2850626+02:00","RecordID":526,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-22T13:05:57.2969114+02:00"}', CAST(N'2025-05-22T13:05:57.260' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (763, 676, 14, 17, N'tbl_tran_StudentDetails1', 668, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":526,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-22T13:05:57.3301181+02:00","RecordID":668,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-22T13:05:57.3434864+02:00"}', CAST(N'2025-05-22T13:05:57.307' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (764, 676, 14, 17, N'tbl_tran_StudentDetails2', 556, N'{"FirstName":"a","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":526,"Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-22T13:05:57.3523521+02:00","RecordID":556,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-22T13:05:57.3634537+02:00"}', CAST(N'2025-05-22T13:05:57.323' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (765, 676, 14, 17, N'tbl_tran_StudentDetails2', 557, N'{"FirstName":"b","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":526,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-22T13:05:57.3769087+02:00","RecordID":557,"StepDescription":"Student Main Details","interactionType":"Update"}', CAST(N'2025-05-22T13:05:57.340' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (766, 678, 15, 17, N'tbl_Tran_Student', 532, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":"-33.723227, 18.451842","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-22T20:03:29.8410348+02:00","RecordID":532,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-22T20:03:29.913' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (767, 680, 16, 17, N'tbl_Tran_Student', 533, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":"-33.723243, 18.451890","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T12:44:14.9620641+02:00","RecordID":533,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-23T12:44:14.953' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (768, 680, 16, 17, N'tbl_tran_StudentDetails1', 670, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":533,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T12:44:15.0928321+02:00","RecordID":670,"StepDescription":"Student Main Details","TableDescription":"StudentDetails11","interactionType":"Insert"}', CAST(N'2025-05-23T12:44:15.033' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (769, 682, 17, 17, N'tbl_Tran_Student', 534, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T12:44:34.6536883+02:00","RecordID":534,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-23T12:44:34.593' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (770, 682, 17, 17, N'tbl_tran_StudentDetails1', 671, N'{"FirstName":"s","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":534,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T12:44:34.6890498+02:00","RecordID":671,"StepDescription":"Student Main Details","TableDescription":"StudentDetails11","interactionType":"Insert"}', CAST(N'2025-05-23T12:44:34.623' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (771, 683, 17, 17, N'tbl_Tran_Student', 534, N'{"FirstName":"d","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:05:36.0201732+02:00","RecordID":534,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:05:36.0615238+02:00"}', CAST(N'2025-05-23T13:05:36.060' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (772, 683, 17, 17, N'tbl_tran_StudentDetails1', 671, N'{"FirstName":"s","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":534,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:05:36.1611907+02:00","RecordID":671,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:05:36.1756133+02:00"}', CAST(N'2025-05-23T13:05:36.107' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (773, 681, 16, 18, N'tbl_tran_Country', 1, N'{"CountryID":"1","Datevisited":"2025-05-23","Wouldrecommend":null,"StudentID":533,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:10:45.3885015+02:00","RecordID":1,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-23T13:10:45.330' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (774, 685, 18, 17, N'tbl_Tran_Student', 535, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:49:04.3847391+02:00","RecordID":535,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-23T13:49:04.500' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (775, 685, 18, 17, N'tbl_tran_StudentDetails1', 672, N'{"FirstName":"sdf","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":535,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:49:04.6164922+02:00","RecordID":672,"StepDescription":"Student Main Details","TableDescription":"StudentDetails11","interactionType":"Insert"}', CAST(N'2025-05-23T13:49:04.560' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (776, 686, 18, 18, N'tbl_tran_Country', 2, N'{"CountryID":"1","Datevisited":null,"Wouldrecommend":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:49:45.7328048+02:00","RecordID":2,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-23T13:49:45.683' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (777, 687, 18, 17, N'tbl_Tran_Student', 535, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:52:22.0720924+02:00","RecordID":535,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:52:22.0831926+02:00"}', CAST(N'2025-05-23T13:52:22.020' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (778, 687, 18, 17, N'tbl_tran_StudentDetails1', 672, N'{"FirstName":"sdf","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":535,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:52:22.1058262+02:00","RecordID":672,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:52:22.1191496+02:00"}', CAST(N'2025-05-23T13:52:22.067' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (779, 688, 19, 17, N'tbl_Tran_Student', 536, N'{"FirstName":"DDFS","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:52:45.6146092+02:00","RecordID":536,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-23T13:52:45.557' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (780, 688, 19, 17, N'tbl_tran_StudentDetails1', 673, N'{"FirstName":"GGG","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":536,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:52:45.6495225+02:00","RecordID":673,"StepDescription":"Student Main Details","TableDescription":"StudentDetails11","interactionType":"Insert"}', CAST(N'2025-05-23T13:52:45.587' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (781, 689, 19, 18, N'tbl_tran_Country', 3, N'{"CountryID":"2","Datevisited":null,"Wouldrecommend":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:52:51.1097803+02:00","RecordID":3,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-23T13:52:51.050' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (782, 690, 19, 18, N'tbl_tran_Country', 3, N'{"CountryID":"2","Datevisited":null,"Wouldrecommend":"True","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:53:02.4850457+02:00","RecordID":3,"StepDescription":"Capture Details 1","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:53:02.5046577+02:00"}', CAST(N'2025-05-23T13:53:02.450' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (783, 691, 19, 18, N'tbl_tran_Country', 3, N'{"CountryID":"7","Datevisited":"2025-05-27","Wouldrecommend":"True","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T13:53:22.1442523+02:00","RecordID":3,"StepDescription":"Capture Details 1","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T13:53:22.1614537+02:00"}', CAST(N'2025-05-23T13:53:22.107' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (784, 693, 20, 17, N'tbl_Tran_Student', 537, N'{"FirstName":"f","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","geo_SchoolLocation":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:58:05.158245+02:00","RecordID":537,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-23T13:58:05.420' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (785, 693, 20, 17, N'tbl_tran_StudentDetails1', 674, N'{"FirstName":"s","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":537,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T13:58:05.5455326+02:00","RecordID":674,"StepDescription":"Student Main Details","TableDescription":"StudentDetails11","interactionType":"Insert"}', CAST(N'2025-05-23T13:58:05.483' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (786, 694, 20, 18, N'tbl_tran_Country', 4, N'{"CountryID":null,"Datevisited":null,"Wouldrecommend":null,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T14:14:14.1255216+02:00","RecordID":4,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-23T14:14:14.090' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (787, 695, 20, 18, N'tbl_tran_Country', 4, N'{"CountryID":null,"Datevisited":null,"Wouldrecommend":"xxx","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T14:14:20.0733781+02:00","RecordID":4,"StepDescription":"Capture Details 1","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T14:14:20.0846813+02:00"}', CAST(N'2025-05-23T14:14:20.027' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (788, 697, 21, 22, N'tbl_tran_Asset', 1, N'{"Assetname":"Lenovo","Assetnumber":"434633","AssettypeID":"1","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-23T16:34:31.3990764+02:00","RecordID":1,"StepDescription":"Capture Asset","TableDescription":"Capture Step","interactionType":"Insert"}', CAST(N'2025-05-23T16:34:31.417' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (789, 698, 21, 22, N'tbl_tran_Asset', 1, N'{"Assetname":"Lenovo","Assetnumber":"434633","AssettypeID":"1","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T16:34:42.8511499+02:00","RecordID":1,"StepDescription":"Capture Asset","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T16:34:42.8903312+02:00"}', CAST(N'2025-05-23T16:34:42.903' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetail] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (790, 699, 21, 22, N'tbl_tran_Asset', 1, N'{"Assetname":"Lenovo","Assetnumber":"434633 test","AssettypeID":"2","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-23T16:43:35.240695+02:00","RecordID":1,"StepDescription":"Capture Asset","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-23T16:43:35.2896734+02:00"}', CAST(N'2025-05-23T16:43:35.353' AS DateTime), 2, 1)
SET IDENTITY_INSERT [dbo].[foProcessEventsDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessEventsDetailArchive] ON 

INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (1, 636, 12, 19, N'tbl_Tran_Student', 511, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-08T22:14:24.1847214+02:00","RecordID":511,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-08T22:14:24.353' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (2, 637, 12, 19, N'tbl_Tran_Student', 511, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"ModifiedUserID":1,"ModifiedDate":"2025-05-08T22:22:46.728722+02:00","RecordID":511,"StepDescription":"Student No Approval","interactionType":"Update","CreatedUserID":1,"CreatedDate":"2025-05-08T22:22:47.0984072+02:00"}', CAST(N'2025-05-08T22:22:47.210' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (3, 611, 1, 17, N'tbl_Tran_Student', 500, N'{"FirstName":"FD","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.6596691+02:00","RecordID":500,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.657' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (4, 611, 1, 17, N'tbl_tran_StudentDetails1', 641, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.6810909+02:00","RecordID":641,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.677' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (5, 611, 1, 17, N'tbl_tran_StudentDetails2', 538, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.7178458+02:00","RecordID":538,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.717' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (6, 612, 1, 18, N'tbl_tran_StudentDetails1', 642, N'{"FirstName":"F","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:57:07.4290958+02:00","RecordID":642,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-04T14:57:07.423' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (7, 614, 2, 17, N'tbl_Tran_Student', 501, N'{"FirstName":"GG","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.2098619+02:00","RecordID":501,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.207' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (8, 614, 2, 17, N'tbl_tran_StudentDetails1', 643, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.2323545+02:00","RecordID":643,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.227' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (9, 614, 2, 17, N'tbl_tran_StudentDetails2', 539, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.255645+02:00","RecordID":539,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.250' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (10, 615, 2, 18, N'tbl_tran_StudentDetails1', 644, N'{"FirstName":"F","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:07:00.3100102+02:00","RecordID":644,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-04T15:07:00.310' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (11, 613, 1, 17, N'tbl_Tran_Student', 500, N'{"FirstName":"FD","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.4776457+02:00","RecordID":500,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.4944399+02:00"}', CAST(N'2025-05-04T15:32:49.517' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (12, 613, 1, 17, N'tbl_tran_StudentDetails1', 641, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.5411921+02:00","RecordID":641,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.5508251+02:00"}', CAST(N'2025-05-04T15:32:49.547' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (13, 613, 1, 17, N'tbl_tran_StudentDetails2', 538, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.5649935+02:00","RecordID":538,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.5730263+02:00"}', CAST(N'2025-05-04T15:32:49.570' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (14, 617, 1, 18, N'tbl_tran_StudentDetails1', 642, N'{"FirstName":"F","LastName":"f","DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:33:11.8062817+02:00","RecordID":642,"StepDescription":"Capture Details 1","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:33:11.8174004+02:00"}', CAST(N'2025-05-04T15:33:11.813' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (15, 618, 3, 19, N'tbl_Tran_Student', 502, N'{"FirstName":"asas","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:09.1726279+02:00","RecordID":502,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T16:34:09.253' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (16, 620, 4, 19, N'tbl_Tran_Student', 503, N'{"FirstName":"ddsfdds","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:45.9285559+02:00","RecordID":503,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T16:34:45.923' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (17, 622, 5, 19, N'tbl_Tran_Student', 504, N'{"FirstName":"SDADS","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T17:18:31.7936916+02:00","RecordID":504,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T17:18:31.787' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (18, 624, 6, 19, N'tbl_Tran_Student', 505, N'{"FirstName":"D","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T17:18:54.3095911+02:00","RecordID":505,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T17:18:54.300' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (19, 628, 8, 19, N'tbl_Tran_Student', 507, N'{"FirstName":"sd","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T21:11:40.6283604+02:00","RecordID":507,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T21:11:40.730' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (20, 630, 9, 17, N'tbl_Tran_Student', 508, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.7626872+02:00","attachment_StudentPass":"SSS;Attachments/tbl_Tran_Student/508/GL COMPARE - NNBNBNB JUMPMAN.sql","attachment_ProofOfAddress":"DDD;Attachments/tbl_Tran_Student/508/JUMPMAN AND SIX - GL QUERIES AND REFRESH.sql","RecordID":508,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.887' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (21, 630, 9, 17, N'tbl_tran_StudentDetails1', 645, N'{"FirstName":"ZDF","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":508,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.9173918+02:00","RecordID":645,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.920' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (22, 630, 9, 17, N'tbl_tran_StudentDetails2', 540, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":508,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.9391059+02:00","RecordID":540,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.940' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (23, 632, 10, 19, N'tbl_Tran_Student', 509, N'{"FirstName":"SDFSDF","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:54:57.5250857+02:00","RecordID":509,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-05T09:54:57.527' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (24, 634, 11, 17, N'tbl_Tran_Student', 510, N'{"FirstName":"nhj","LastName":"hgjhg","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":"gg;","attachment_ProofOfAddress":"gg;","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.3888921+02:00","RecordID":510,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.490' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (25, 634, 11, 17, N'tbl_tran_StudentDetails1', 646, N'{"FirstName":"nhj","LastName":"hgjhg","DOB":null,"CampusID":null,"Age":"2","attachment_ProofOfAddress":"gg;","attachment_ProofOfIncome":"gg;","StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.5314559+02:00","RecordID":646,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.543' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (26, 634, 11, 17, N'tbl_tran_StudentDetails2', 541, N'{"FirstName":"2","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":"ff;","Amount":null,"StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.5690984+02:00","RecordID":541,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.580' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (27, 635, 11, 18, N'tbl_tran_StudentDetails1', 653, N'{"FirstName":"t","LastName":"ret","DOB":"2025-05-06","CampusID":"2","Age":"1","attachment_ProofOfAddress":"ret;","attachment_ProofOfIncome":"tret;","StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:45:57.2580568+02:00","RecordID":653,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-05T12:45:57.287' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (28, 611, 1, 17, N'tbl_Tran_Student', 500, N'{"FirstName":"FD","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.6596691+02:00","RecordID":500,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.657' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (29, 611, 1, 17, N'tbl_tran_StudentDetails1', 641, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.6810909+02:00","RecordID":641,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.677' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (30, 611, 1, 17, N'tbl_tran_StudentDetails2', 538, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:56:59.7178458+02:00","RecordID":538,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-04T14:56:59.717' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (31, 612, 1, 18, N'tbl_tran_StudentDetails1', 642, N'{"FirstName":"F","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T14:57:07.4290958+02:00","RecordID":642,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-04T14:57:07.423' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (32, 614, 2, 17, N'tbl_Tran_Student', 501, N'{"FirstName":"GG","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.2098619+02:00","RecordID":501,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.207' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (33, 614, 2, 17, N'tbl_tran_StudentDetails1', 643, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.2323545+02:00","RecordID":643,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.227' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (34, 614, 2, 17, N'tbl_tran_StudentDetails2', 539, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:00:49.255645+02:00","RecordID":539,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-04T15:00:49.250' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (35, 615, 2, 18, N'tbl_tran_StudentDetails1', 644, N'{"FirstName":"F","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":501,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-04T15:07:00.3100102+02:00","RecordID":644,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-04T15:07:00.310' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (36, 613, 1, 17, N'tbl_Tran_Student', 500, N'{"FirstName":"FD","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.4776457+02:00","RecordID":500,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.4944399+02:00"}', CAST(N'2025-05-04T15:32:49.517' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (37, 613, 1, 17, N'tbl_tran_StudentDetails1', 641, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.5411921+02:00","RecordID":641,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.5508251+02:00"}', CAST(N'2025-05-04T15:32:49.547' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (38, 613, 1, 17, N'tbl_tran_StudentDetails2', 538, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:32:49.5649935+02:00","RecordID":538,"StepDescription":"Student Main Details","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:32:49.5730263+02:00"}', CAST(N'2025-05-04T15:32:49.570' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (39, 617, 1, 18, N'tbl_tran_StudentDetails1', 642, N'{"FirstName":"F","LastName":"f","DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":500,"Active":1,"ModifiedUserID":2,"ModifiedDate":"2025-05-04T15:33:11.8062817+02:00","RecordID":642,"StepDescription":"Capture Details 1","interactionType":"Update","CreatedUserID":2,"CreatedDate":"2025-05-04T15:33:11.8174004+02:00"}', CAST(N'2025-05-04T15:33:11.813' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (40, 618, 3, 19, N'tbl_Tran_Student', 502, N'{"FirstName":"asas","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:09.1726279+02:00","RecordID":502,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T16:34:09.253' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (41, 620, 4, 19, N'tbl_Tran_Student', 503, N'{"FirstName":"ddsfdds","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T16:34:45.9285559+02:00","RecordID":503,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T16:34:45.923' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (42, 622, 5, 19, N'tbl_Tran_Student', 504, N'{"FirstName":"SDADS","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T17:18:31.7936916+02:00","RecordID":504,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T17:18:31.787' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (43, 624, 6, 19, N'tbl_Tran_Student', 505, N'{"FirstName":"D","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T17:18:54.3095911+02:00","RecordID":505,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T17:18:54.300' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (44, 628, 8, 19, N'tbl_Tran_Student', 507, N'{"FirstName":"sd","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-04T21:11:40.6283604+02:00","RecordID":507,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-04T21:11:40.730' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (45, 630, 9, 17, N'tbl_Tran_Student', 508, N'{"FirstName":"Schalk","LastName":"Van Der Merwe","DOB":null,"CampusID":null,"ProvinceID":null,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.7626872+02:00","attachment_StudentPass":"SSS;Attachments/tbl_Tran_Student/508/GL COMPARE - NNBNBNB JUMPMAN.sql","attachment_ProofOfAddress":"DDD;Attachments/tbl_Tran_Student/508/JUMPMAN AND SIX - GL QUERIES AND REFRESH.sql","RecordID":508,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.887' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (46, 630, 9, 17, N'tbl_tran_StudentDetails1', 645, N'{"FirstName":"ZDF","LastName":null,"DOB":null,"CampusID":null,"Age":null,"attachment_ProofOfAddress":";","attachment_ProofOfIncome":";","StudentID":508,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.9173918+02:00","RecordID":645,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.920' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (47, 630, 9, 17, N'tbl_tran_StudentDetails2', 540, N'{"FirstName":null,"LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":";","Amount":null,"StudentID":508,"Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:50:18.9391059+02:00","RecordID":540,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-05T09:50:18.940' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (48, 632, 10, 19, N'tbl_Tran_Student', 509, N'{"FirstName":"SDFSDF","LastName":null,"DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":";","attachment_ProofOfAddress":";","Active":1,"CreatedUserID":1,"CreatedDate":"2025-05-05T09:54:57.5250857+02:00","RecordID":509,"StepDescription":"Student No Approval","TableDescription":"sss","interactionType":"Insert"}', CAST(N'2025-05-05T09:54:57.527' AS DateTime), 1, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (49, 634, 11, 17, N'tbl_Tran_Student', 510, N'{"FirstName":"nhj","LastName":"hgjhg","DOB":null,"CampusID":null,"ProvinceID":null,"attachment_StudentPass":"gg;","attachment_ProofOfAddress":"gg;","Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.3888921+02:00","RecordID":510,"StepDescription":"Student Main Details","TableDescription":"Add Student Details1","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.490' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (50, 634, 11, 17, N'tbl_tran_StudentDetails1', 646, N'{"FirstName":"nhj","LastName":"hgjhg","DOB":null,"CampusID":null,"Age":"2","attachment_ProofOfAddress":"gg;","attachment_ProofOfIncome":"gg;","StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.5314559+02:00","RecordID":646,"StepDescription":"Student Main Details","TableDescription":"1","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.543' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (51, 634, 11, 17, N'tbl_tran_StudentDetails2', 541, N'{"FirstName":"2","LastName":null,"DOB":null,"CampusID":null,"attachment_ProofofAddress":"ff;","Amount":null,"StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:29:47.5690984+02:00","RecordID":541,"StepDescription":"Student Main Details","TableDescription":"2","interactionType":"Insert"}', CAST(N'2025-05-05T12:29:47.580' AS DateTime), 2, 1)
INSERT [dbo].[foProcessEventsDetailArchive] ([ID], [ProcessEventID], [ProcessInstanceID], [StepID], [TableName], [RecordID], [DataSetUpdate], [CreatedDate], [CreatedUserID], [Active]) VALUES (52, 635, 11, 18, N'tbl_tran_StudentDetails1', 653, N'{"FirstName":"t","LastName":"ret","DOB":"2025-05-06","CampusID":"2","Age":"1","attachment_ProofOfAddress":"ret;","attachment_ProofOfIncome":"tret;","StudentID":510,"Active":1,"CreatedUserID":2,"CreatedDate":"2025-05-05T12:45:57.2580568+02:00","RecordID":653,"StepDescription":"Capture Details 1","interactionType":"Update"}', CAST(N'2025-05-05T12:45:57.287' AS DateTime), 2, 1)
SET IDENTITY_INSERT [dbo].[foProcessEventsDetailArchive] OFF
GO
SET IDENTITY_INSERT [dbo].[foProcessSteps] ON 

INSERT [dbo].[foProcessSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (17, 4, CAST(1.00 AS Decimal(18, 2)), N'Student Main Details', 1, NULL, 1)
INSERT [dbo].[foProcessSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (18, 4, CAST(2.00 AS Decimal(18, 2)), N'Capture Details 1', NULL, 2, 1)
INSERT [dbo].[foProcessSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (19, 5, CAST(1.00 AS Decimal(18, 2)), N'Student No Approval', NULL, 1, 1)
INSERT [dbo].[foProcessSteps] ([ID], [ProcessID], [StepNo], [StepDescription], [GroupID], [UserID], [Active]) VALUES (22, 8, CAST(1.00 AS Decimal(18, 2)), N'Capture Asset', NULL, 2, 1)
SET IDENTITY_INSERT [dbo].[foProcessSteps] OFF
GO
SET IDENTITY_INSERT [dbo].[foReports] ON 

INSERT [dbo].[foReports] ([ID], [ReportName], [ReportDescription], [Active]) VALUES (1, N'Student Report', N'This is a report that will take you to the student details', 1)
INSERT [dbo].[foReports] ([ID], [ReportName], [ReportDescription], [Active]) VALUES (2, N'Report ABC', N'This is just a copy of the report to show', 1)
INSERT [dbo].[foReports] ([ID], [ReportName], [ReportDescription], [Active]) VALUES (3, N'Student Report Description', N'AFF', 1)
INSERT [dbo].[foReports] ([ID], [ReportName], [ReportDescription], [Active]) VALUES (4, N'Asset Expire', N'Show us Expired or close expired', 0)
INSERT [dbo].[foReports] ([ID], [ReportName], [ReportDescription], [Active]) VALUES (5, N'Asset Management', N'Asset Management', 1)
SET IDENTITY_INSERT [dbo].[foReports] OFF
GO
SET IDENTITY_INSERT [dbo].[foReportTable] ON 

INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (1, 1, N'tbl_Tran_Student', N'*', N'F', 2, 1, NULL, N'Student', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (2, 1, N'tbl_tran_StudentDetails1', N'*', N'T', 1, 0, N'StudentID', N'StudentDetails1', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (3, 1, N'tbl_tran_StudentDetails1', N'*', N'T', 1, 0, N'StudentID', N'StudentDetails1', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (4, 1, N'tbl_tran_StudentDetails2', N'FirstName,LastName,CampusID', N'T', 1, 0, N'StudentID', N'StudentDetails2', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (5, 2, N'tbl_Tran_Student', N'*', N'F', 2, 1, NULL, N'Student', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (6, 2, N'tbl_tran_StudentDetails1', N'*', N'T', 1, 0, N'StudentID', N'StudentDetails1', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (7, 2, N'tbl_tran_StudentDetails1', N'*', N'F', 1, 0, N'StudentID', N'StudentDetails1', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (8, 2, N'tbl_tran_StudentDetails2', N'FirstName,LastName,CampusID', N'F', 1, 0, N'StudentID', N'StudentDetails2', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (9, 3, N'tbl_Tran_Student', N'FirstName,LastName,CampusID', N'F', 2, 1, NULL, N'Student', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (10, 3, N'tbl_tran_StudentDetails1', N'FirstName,LastName,CampusID', N'F', 1, 0, N'StudentID', N'Capture Student ', 1)
INSERT [dbo].[foReportTable] ([ID], [ReportsID], [TableName], [ColumnQuery], [FormType], [ColumnCount], [Parent], [FKColumn], [TableDescription], [Active]) VALUES (12, 5, N'tbl_tran_Asset', N'*', N'T', 1, 1, N'AssettypeID', N'Asset Report', 1)
SET IDENTITY_INSERT [dbo].[foReportTable] OFF
GO
SET IDENTITY_INSERT [dbo].[foReportTableQuery] ON 

INSERT [dbo].[foReportTableQuery] ([ID], [ReportsID], [Query], [FormType], [ColumnCount], [TableDescription], [Active]) VALUES (16, 5, N'select * from tbl_tran_Asset', N'T', 1, N'Asset Report 2', 1)
SET IDENTITY_INSERT [dbo].[foReportTableQuery] OFF
GO
SET IDENTITY_INSERT [dbo].[foTableAttachments] ON 

INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (4, N'tbl_tran_Student', 1, N'DEF', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_5.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (5, N'tbl_tran_Student', 1, N'ZZZZ', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_4.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (6, N'tbl_tran_Student', 1, N'Description Freedom demo', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_2.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (7, N'tbl_tran_Student', 1, N'Description Freedom demo 2', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_5.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (8, N'tbl_tran_Student', 1, N'123', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_4.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (10, N'tbl_tran_Student', 1, N'Hierdie en daardie', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_1.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (11, N'tbl_tran_Student', 1, N'abcder', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_5.txt', NULL, NULL)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (12, N'tbl_tran_Student', 1, N'dddd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_5.txt', CAST(N'2025-02-16T11:47:26.753' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (13, N'tbl_tran_Student', 1, N'asd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_3.txt', CAST(N'2025-02-17T09:51:00.520' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (14, N'tbl_tran_Student', 1, N'sss', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_4.txt', CAST(N'2025-02-17T10:23:24.490' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (15, N'tbl_tran_Student', 1, N'ddd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_1.txt', CAST(N'2025-02-17T10:23:24.497' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (16, N'tbl_tran_Student', 1, N'ddd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_1.txt', CAST(N'2025-02-17T10:23:24.513' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (17, N'tbl_tran_Student', 184, N'ZZZZ 444', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\f38aaa44-4dcf-4ee8-b11b-9dc7056558da.txt', CAST(N'2025-02-17T14:08:19.313' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (18, N'tbl_tran_Student', 185, N'fREEDOM TEST 123 ATTACH', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\fd0b2ae6-b211-4b35-bed1-3e939ff504ff.txt', CAST(N'2025-02-17T14:13:34.387' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (19, N'tbl_tran_Student', 186, N'freedom test 2', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\040f8d5e-a6b1-43e9-9c38-86c5b7a19739.txt', CAST(N'2025-02-17T14:14:42.207' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (20, N'tbl_tran_Student', 186, N'freedom test 2', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\860b932b-f5f7-4180-8e37-f65816898647.txt', CAST(N'2025-02-17T14:14:42.213' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (21, N'tbl_md_Campus', 4, N'mpus', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\d7ee7088-c824-4fae-afdd-7b033c723af4.txt', CAST(N'2025-02-17T14:16:00.803' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (22, N'tbl_tran_StudentDetails1', 196, N'zzz 900', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\837b950e-42ae-4c6a-88bd-28e0811d1d5c.txt', CAST(N'2025-02-17T15:30:02.323' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (23, N'tbl_tran_StudentDetails1', 197, N'zzz 900', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\caec24aa-ceec-4f29-96d4-aea526817591.txt', CAST(N'2025-02-17T15:30:02.360' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (24, N'tbl_tran_StudentDetails1', 1, N'BCD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_4.txt', CAST(N'2025-02-17T18:43:10.583' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (25, N'tbl_tran_StudentDetails1', 1, N'BCD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_4.txt', CAST(N'2025-02-17T18:43:37.080' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (26, N'tbl_tran_StudentDetails1', 1, N'shhh123', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_2.txt', CAST(N'2025-02-17T18:56:41.753' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (27, N'tbl_tran_Student', 8, N'ZZZZ 111', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\steps_for_edit_logic.txt', CAST(N'2025-02-17T19:13:12.540' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (28, N'tbl_tran_Student', 8, N'sssssss', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\steps_for_edit_logic.txt', CAST(N'2025-02-17T19:13:33.277' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (29, N'tbl_tran_StudentDetails1', 198, N'www', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\936c47d8-a8a5-41b3-8c7e-419727c83bde.txt', CAST(N'2025-02-17T19:14:08.923' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (30, N'tbl_tran_StudentDetails1', 0, N'cccc', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\steps_for_edit_logic.txt', CAST(N'2025-02-17T19:15:09.567' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (31, N'tbl_tran_StudentDetails1', 0, N'cccc', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\steps_for_edit_logic.txt', CAST(N'2025-02-17T19:15:25.773' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (32, N'tbl_tran_StudentDetails1', 198, N'cccc', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\steps_for_edit_logic.txt', CAST(N'2025-02-17T19:26:01.823' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (33, N'tbl_tran_StudentDetails1', 8, N'DD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\step_3.txt', CAST(N'2025-02-17T19:27:28.643' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (34, N'tbl_tran_studentdetails1_ExtraInformation', 3, N'FSDF', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\43c67e2e-ca1f-43d7-a946-ecdcdcfa2bab.txt', CAST(N'2025-02-17T19:28:30.887' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (35, N'tbl_tran_Student', 187, N'ABC', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\3cc88de7-8fa3-4a69-a507-7015d0887c9c.txt', CAST(N'2025-02-18T11:40:06.713' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (36, N'tbl_tran_Student', 188, N'ss', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\aa5ed092-dd36-4fa3-b9ab-3794f6761104.txt', CAST(N'2025-02-18T11:58:04.073' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (37, N'tbl_tran_Student', 188, N'asd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_Student\TableXController.cs', CAST(N'2025-02-18T11:59:31.393' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (38, N'tbl_tran_Student', 196, N'ABC', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\0ee7f115-0329-40b1-8e6c-4b18c9c9440c.txt', CAST(N'2025-02-19T14:05:03.053' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (39, N'tbl_tran_StudentDetails1', 299, N'asd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\9ff370e1-60e4-4f3d-8123-6aa16b0d9c76.txt', CAST(N'2025-02-21T12:39:50.990' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (40, N'tbl_tran_Student', 1, N'asd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_Student\PAR CASH.xlsx', CAST(N'2025-02-25T16:40:15.113' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (41, N'tbl_tran_StudentDetails1', 1, N'SASD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\step_5.txt', CAST(N'2025-02-28T18:11:25.753' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (42, N'tbl_tran_StudentDetails1', 1, N'SSS', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\step_1.txt', CAST(N'2025-02-28T18:11:59.887' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (43, N'tbl_tran_Student', 1, N'friday', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_Student\step_1.txt', CAST(N'2025-02-28T18:43:27.623' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (44, N'tbl_tran_Student', 14, N'Friday1', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_Student\step_1.txt', CAST(N'2025-02-28T18:44:19.473' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (45, N'tbl_tran_StudentDetails1', 1, N'dddFrida', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\step_1.txt', CAST(N'2025-02-28T19:09:05.923' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (46, N'tbl_tran_StudentDetails1', 1, N'Friday123', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\step_1.txt', CAST(N'2025-02-28T19:14:34.223' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (47, N'tbl_tran_StudentDetails1', 199, N'DDDD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_Deployment2025\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\step_3.txt', CAST(N'2025-02-28T19:16:26.793' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (48, N'tbl_tran_StudentDetails1', 667, N'BCD', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\tbl_tran_StudentDetails1\full bc.sql', CAST(N'2025-05-19T22:41:47.797' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (49, N'tbl_tran_Student', 526, N'Description  1', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\tbl_tran_Student\GL COMPARE - NNBNBNB JUMPMAN.sql', CAST(N'2025-05-22T11:42:22.773' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (50, N'tbl_tran_Student', 526, N'Description  2', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\tbl_tran_Student\jmp source data.sql', CAST(N'2025-05-22T11:42:22.783' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (51, N'tbl_tran_Student', 526, N'xxx', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\tbl_tran_Student\full bc.sql', CAST(N'2025-05-22T12:51:50.217' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (52, N'tbl_tran_Student', 526, N'd', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\tbl_tran_Student\uv_GroupReports_Entity.sql', CAST(N'2025-05-22T12:52:31.080' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (53, N'tbl_tran_Student', 527, N'sss', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\38f8491a-7157-4612-b285-b1cc26375200.sql', CAST(N'2025-05-22T18:15:55.507' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (54, N'tbl_tran_Student', 531, N'dddsdfsd 32423 ', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\688d7e7f-b9b5-4035-96c2-4822ce565e96.sql', CAST(N'2025-05-22T18:32:37.123' AS DateTime), 1)
INSERT [dbo].[foTableAttachments] ([ID], [tablename], [PKID], [AttachmentDescription], [Attachment], [DateAdded], [UserID]) VALUES (55, N'tbl_tran_StudentDetails1', 669, N'dddsf', N'C:\Users\Schalk.vandermerwe\source\repos\FreschOne_V1UAT\FreschOne\FreschOne\Attachments\3b928ac4-bde9-4301-848c-cc888d92ab0c.sql', CAST(N'2025-05-22T18:40:42.727' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[foTableAttachments] OFF
GO
SET IDENTITY_INSERT [dbo].[foTableColumnsToIgnore] ON 

INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (1, N'Active')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (2, N'CreatedUserID')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (3, N'CreatedDate')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (4, N'ModifiedUserID')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (5, N'ModifiedDate')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (6, N'DeletedUserID')
INSERT [dbo].[foTableColumnsToIgnore] ([ID], [ColumnName]) VALUES (7, N'DeletedDate')
SET IDENTITY_INSERT [dbo].[foTableColumnsToIgnore] OFF
GO
SET IDENTITY_INSERT [dbo].[foTableGroups] ON 

INSERT [dbo].[foTableGroups] ([ID], [Description], [Active]) VALUES (1, N'Asset Management', 1)
SET IDENTITY_INSERT [dbo].[foTableGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[foTablePrefixes] ON 

INSERT [dbo].[foTablePrefixes] ([ID], [Prefix], [Description], [Active]) VALUES (1, N'tbl_md_', N'Maintenance', 1)
INSERT [dbo].[foTablePrefixes] ([ID], [Prefix], [Description], [Active]) VALUES (2, N'tbl_tran_', N'Transactional', 1)
INSERT [dbo].[foTablePrefixes] ([ID], [Prefix], [Description], [Active]) VALUES (6, N'tbl_audit_', N'Auditing', 1)
SET IDENTITY_INSERT [dbo].[foTablePrefixes] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserGroups] ON 

INSERT [dbo].[foUserGroups] ([ID], [UserID], [GroupID]) VALUES (1, 2, 1)
INSERT [dbo].[foUserGroups] ([ID], [UserID], [GroupID]) VALUES (2, 1, 2)
INSERT [dbo].[foUserGroups] ([ID], [UserID], [GroupID]) VALUES (3, 1, 1)
SET IDENTITY_INSERT [dbo].[foUserGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserLogin] ON 

INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1, 1, CAST(N'2025-01-28T14:13:56.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (2, 1, CAST(N'2025-01-28T14:40:24.300' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (3, 1, CAST(N'2025-01-28T15:16:07.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (4, 1, CAST(N'2025-01-29T12:00:03.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (5, 1, CAST(N'2025-01-29T12:04:12.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (6, 1, CAST(N'2025-01-29T12:06:16.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (7, 1, CAST(N'2025-01-29T12:12:04.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (8, 1, CAST(N'2025-01-29T12:15:15.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (9, 1, CAST(N'2025-01-29T12:16:47.133' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (10, 1, CAST(N'2025-01-29T12:30:22.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (11, 1, CAST(N'2025-01-29T12:31:02.497' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (12, 1, CAST(N'2025-01-29T12:38:38.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (13, 1, CAST(N'2025-01-29T13:12:22.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (14, 1, CAST(N'2025-01-29T13:17:33.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (15, 1, CAST(N'2025-01-29T13:44:24.330' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (16, 1, CAST(N'2025-01-29T13:46:35.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (17, 1, CAST(N'2025-01-29T13:48:58.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (18, 1, CAST(N'2025-01-29T13:49:55.563' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (19, 1, CAST(N'2025-01-29T14:13:38.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (20, 1, CAST(N'2025-01-29T14:21:01.793' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (21, 1, CAST(N'2025-01-29T14:22:00.037' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (22, 1, CAST(N'2025-01-29T14:33:57.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (23, 1, CAST(N'2025-01-29T14:36:49.760' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (24, 1, CAST(N'2025-01-29T14:48:45.460' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (25, 1, CAST(N'2025-01-29T14:49:29.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (26, 1, CAST(N'2025-01-29T14:54:40.233' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (27, 1, CAST(N'2025-01-29T14:55:21.410' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (28, 1, CAST(N'2025-01-29T14:57:15.150' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (29, 1, CAST(N'2025-01-29T14:58:51.370' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (30, 1, CAST(N'2025-01-29T15:03:04.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (31, 1, CAST(N'2025-01-29T15:04:57.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (32, 1, CAST(N'2025-01-29T15:05:51.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (33, 1, CAST(N'2025-01-29T15:17:39.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (34, 1, CAST(N'2025-01-29T15:19:56.993' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (35, 1, CAST(N'2025-01-29T15:23:40.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (36, 1, CAST(N'2025-01-30T09:55:57.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (37, 1, CAST(N'2025-01-30T10:55:10.970' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (38, 1, CAST(N'2025-01-30T10:56:43.577' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (39, 1, CAST(N'2025-01-30T10:59:44.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (40, 1, CAST(N'2025-01-30T11:00:28.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (41, 1, CAST(N'2025-01-30T11:08:42.753' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (42, 1, CAST(N'2025-01-30T11:16:14.647' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (43, 1, CAST(N'2025-01-30T11:18:51.140' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (44, 1, CAST(N'2025-01-30T11:21:17.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (45, 1, CAST(N'2025-01-30T11:22:17.410' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (46, 1, CAST(N'2025-01-30T11:26:26.647' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (47, 1, CAST(N'2025-01-30T11:28:22.143' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (48, 1, CAST(N'2025-01-30T11:49:07.527' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (49, 1, CAST(N'2025-01-30T11:51:44.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (50, 1, CAST(N'2025-01-30T11:59:07.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (51, 1, CAST(N'2025-01-30T11:59:53.817' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (52, 1, CAST(N'2025-01-30T12:00:38.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (53, 1, CAST(N'2025-01-30T12:02:00.500' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (54, 1, CAST(N'2025-01-30T12:03:23.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (55, 1, CAST(N'2025-01-30T13:15:02.753' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (56, 1, CAST(N'2025-01-30T13:17:37.613' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (57, 1, CAST(N'2025-01-30T13:19:17.397' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (58, 1, CAST(N'2025-01-30T13:20:52.703' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (59, 1, CAST(N'2025-01-30T13:33:08.523' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (60, 1, CAST(N'2025-01-30T13:34:12.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (61, 1, CAST(N'2025-01-30T13:35:19.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (62, 1, CAST(N'2025-01-30T13:36:49.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (63, 1, CAST(N'2025-01-30T13:45:27.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (64, 1, CAST(N'2025-01-30T13:54:47.503' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (65, 1, CAST(N'2025-01-30T14:28:03.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (66, 1, CAST(N'2025-01-30T14:50:17.600' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (67, 1, CAST(N'2025-01-30T14:51:14.887' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (68, 1, CAST(N'2025-01-30T14:53:23.130' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (69, 1, CAST(N'2025-01-30T14:55:09.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (70, 1, CAST(N'2025-01-30T14:59:50.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (71, 1, CAST(N'2025-01-30T15:02:21.533' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (72, 1, CAST(N'2025-01-30T15:04:42.280' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (73, 1, CAST(N'2025-01-30T15:09:44.840' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (74, 1, CAST(N'2025-01-30T15:12:42.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (75, 1, CAST(N'2025-01-30T15:15:33.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (76, 1, CAST(N'2025-01-30T15:23:16.280' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (77, 1, CAST(N'2025-01-30T15:23:56.590' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (78, 1, CAST(N'2025-01-30T15:53:36.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (79, 1, CAST(N'2025-01-30T15:54:19.950' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (80, 1, CAST(N'2025-01-30T15:58:55.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (81, 1, CAST(N'2025-01-30T16:00:43.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (82, 1, CAST(N'2025-01-30T16:01:35.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (83, 1, CAST(N'2025-01-30T16:02:39.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (84, 1, CAST(N'2025-01-30T16:06:03.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (85, 1, CAST(N'2025-01-30T16:07:54.827' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (86, 1, CAST(N'2025-01-30T16:09:22.603' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (87, 1, CAST(N'2025-01-30T16:18:52.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (88, 1, CAST(N'2025-01-30T16:23:50.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (89, 1, CAST(N'2025-01-30T16:29:08.620' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (90, 1, CAST(N'2025-01-31T09:58:48.883' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (91, 1, CAST(N'2025-01-31T10:03:06.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (92, 1, CAST(N'2025-01-31T10:08:04.590' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (93, 1, CAST(N'2025-01-31T10:31:12.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (94, 1, CAST(N'2025-01-31T10:58:51.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (95, 1, CAST(N'2025-01-31T11:02:22.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (96, 1, CAST(N'2025-01-31T11:10:57.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (97, 1, CAST(N'2025-01-31T11:12:34.653' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (98, 1, CAST(N'2025-01-31T11:26:39.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (99, 1, CAST(N'2025-01-31T11:35:49.580' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (100, 1, CAST(N'2025-01-31T11:41:35.930' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (101, 1, CAST(N'2025-01-31T11:53:04.357' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (102, 1, CAST(N'2025-01-31T11:58:23.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (103, 1, CAST(N'2025-01-31T12:02:38.177' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (104, 1, CAST(N'2025-01-31T12:04:40.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (105, 1, CAST(N'2025-01-31T12:22:30.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (106, 1, CAST(N'2025-01-31T12:27:36.070' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (107, 1, CAST(N'2025-01-31T12:31:00.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (108, 1, CAST(N'2025-01-31T12:49:02.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (109, 1, CAST(N'2025-01-31T12:50:49.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (110, 1, CAST(N'2025-01-31T12:57:50.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (111, 1, CAST(N'2025-01-31T12:58:52.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (112, 1, CAST(N'2025-01-31T13:01:29.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (113, 1, CAST(N'2025-01-31T13:06:30.593' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (114, 1, CAST(N'2025-01-31T13:28:23.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (115, 1, CAST(N'2025-01-31T13:41:17.693' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (116, 1, CAST(N'2025-01-31T13:43:42.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (117, 1, CAST(N'2025-01-31T13:49:03.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (118, 1, CAST(N'2025-01-31T13:50:42.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (119, 1, CAST(N'2025-01-31T13:53:48.630' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (120, 1, CAST(N'2025-01-31T14:08:25.267' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (121, 1, CAST(N'2025-01-31T14:13:58.487' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (122, 1, CAST(N'2025-01-31T14:15:31.990' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (123, 1, CAST(N'2025-01-31T14:17:36.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (124, 1, CAST(N'2025-01-31T14:18:56.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (125, 1, CAST(N'2025-01-31T14:39:02.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (126, 1, CAST(N'2025-01-31T14:41:09.993' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (127, 1, CAST(N'2025-01-31T14:45:28.217' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (128, 1, CAST(N'2025-01-31T15:16:51.243' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (129, 1, CAST(N'2025-01-31T15:20:50.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (130, 1, CAST(N'2025-01-31T15:22:41.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (131, 1, CAST(N'2025-01-31T15:24:10.143' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (132, 1, CAST(N'2025-01-31T15:40:31.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (133, 1, CAST(N'2025-01-31T15:49:39.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (134, 1, CAST(N'2025-01-31T15:52:05.880' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (135, 1, CAST(N'2025-02-03T11:48:17.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (136, 1, CAST(N'2025-02-03T12:10:38.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (137, 1, CAST(N'2025-02-03T12:25:06.303' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (138, 1, CAST(N'2025-02-03T12:58:39.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (139, 1, CAST(N'2025-02-03T13:07:46.993' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (140, 1, CAST(N'2025-02-03T13:31:35.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (141, 1, CAST(N'2025-02-03T13:46:55.080' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (142, 1, CAST(N'2025-02-03T14:16:46.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (143, 1, CAST(N'2025-02-03T14:41:42.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (144, 1, CAST(N'2025-02-03T14:44:12.647' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (145, 1, CAST(N'2025-02-03T15:23:16.820' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (146, 1, CAST(N'2025-02-03T15:28:14.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (147, 1, CAST(N'2025-02-03T15:39:08.063' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (148, 1, CAST(N'2025-02-03T15:46:30.633' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (149, 1, CAST(N'2025-02-03T16:00:34.650' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (150, 1, CAST(N'2025-02-04T14:41:43.257' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (151, 1, CAST(N'2025-02-04T15:37:42.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (152, 1, CAST(N'2025-02-04T15:43:14.903' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (153, 1, CAST(N'2025-02-04T16:03:53.090' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (154, 1, CAST(N'2025-02-04T16:27:15.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (155, 1, CAST(N'2025-02-04T16:28:00.300' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (156, 1, CAST(N'2025-02-04T16:29:28.590' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (157, 1, CAST(N'2025-02-04T16:30:33.210' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (158, 1, CAST(N'2025-02-05T10:28:10.750' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (159, 1, CAST(N'2025-02-05T10:32:05.813' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (160, 1, CAST(N'2025-02-05T10:36:36.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (161, 1, CAST(N'2025-02-05T10:40:24.573' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (162, 1, CAST(N'2025-02-05T11:18:45.750' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (163, 1, CAST(N'2025-02-05T11:27:32.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (164, 1, CAST(N'2025-02-05T11:38:25.760' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (165, 1, CAST(N'2025-02-05T11:42:29.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (166, 1, CAST(N'2025-02-05T11:51:01.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (167, 1, CAST(N'2025-02-05T11:53:06.100' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (168, 1, CAST(N'2025-02-05T12:19:47.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (169, 1, CAST(N'2025-02-05T12:29:48.333' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (170, 1, CAST(N'2025-02-05T12:38:02.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (171, 1, CAST(N'2025-02-05T12:56:31.993' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (172, 1, CAST(N'2025-02-05T12:57:25.023' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (173, 1, CAST(N'2025-02-05T12:58:25.843' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (174, 1, CAST(N'2025-02-05T13:07:41.067' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (175, 1, CAST(N'2025-02-05T13:26:25.117' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (176, 1, CAST(N'2025-02-05T13:31:54.877' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (177, 1, CAST(N'2025-02-05T13:36:16.603' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (178, 1, CAST(N'2025-02-05T13:41:17.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (179, 1, CAST(N'2025-02-05T13:54:35.537' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (180, 1, CAST(N'2025-02-05T14:24:08.507' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (181, 1, CAST(N'2025-02-05T15:26:04.920' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (182, 1, CAST(N'2025-02-06T09:31:51.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (183, 1, CAST(N'2025-02-06T10:34:04.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (184, 1, CAST(N'2025-02-06T11:17:07.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (185, 1, CAST(N'2025-02-06T11:40:49.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (186, 1, CAST(N'2025-02-06T11:43:27.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (187, 1, CAST(N'2025-02-06T12:01:18.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (188, 1, CAST(N'2025-02-06T12:24:53.197' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (189, 1, CAST(N'2025-02-06T12:59:35.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (190, 1, CAST(N'2025-02-06T15:23:14.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (191, 1, CAST(N'2025-02-06T15:25:15.507' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (192, 1, CAST(N'2025-02-06T15:26:36.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (193, 1, CAST(N'2025-02-06T15:29:05.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (194, 1, CAST(N'2025-02-07T12:01:37.243' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (195, 1, CAST(N'2025-02-07T12:14:40.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (196, 1, CAST(N'2025-02-07T12:18:39.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (197, 1, CAST(N'2025-02-07T13:17:29.460' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (198, 1, CAST(N'2025-02-07T13:19:21.983' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (199, 1, CAST(N'2025-02-07T13:33:59.087' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (200, 1, CAST(N'2025-02-07T13:41:24.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (201, 1, CAST(N'2025-02-07T13:47:51.717' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (202, 1, CAST(N'2025-02-07T14:09:14.043' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (203, 1, CAST(N'2025-02-07T14:11:58.303' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (204, 1, CAST(N'2025-02-07T14:14:51.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (205, 1, CAST(N'2025-02-07T14:17:23.883' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (206, 1, CAST(N'2025-02-07T14:20:59.043' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (207, 1, CAST(N'2025-02-10T09:58:40.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (208, 1, CAST(N'2025-02-10T10:37:12.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (209, 1, CAST(N'2025-02-10T10:43:22.813' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (210, 1, CAST(N'2025-02-10T10:46:27.140' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (211, 1, CAST(N'2025-02-10T10:46:59.233' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (212, 1, CAST(N'2025-02-10T10:50:16.930' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (213, 1, CAST(N'2025-02-10T10:53:15.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (214, 1, CAST(N'2025-02-10T11:04:53.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (215, 1, CAST(N'2025-02-10T11:21:07.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (216, 1, CAST(N'2025-02-10T11:48:07.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (217, 1, CAST(N'2025-02-10T11:51:59.093' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (218, 1, CAST(N'2025-02-10T12:00:03.497' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (219, 1, CAST(N'2025-02-10T12:08:23.927' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (220, 1, CAST(N'2025-02-10T12:09:55.863' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (221, 1, CAST(N'2025-02-10T12:18:15.910' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (222, 1, CAST(N'2025-02-10T12:26:28.407' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (223, 1, CAST(N'2025-02-10T12:40:31.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (224, 1, CAST(N'2025-02-10T13:01:22.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (225, 1, CAST(N'2025-02-10T13:03:51.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (226, 1, CAST(N'2025-02-10T13:16:59.110' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (227, 1, CAST(N'2025-02-10T14:10:34.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (228, 1, CAST(N'2025-02-10T14:23:02.717' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (229, 1, CAST(N'2025-02-10T15:01:20.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (230, 1, CAST(N'2025-02-10T15:02:49.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (231, 1, CAST(N'2025-02-10T15:06:31.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (232, 1, CAST(N'2025-02-10T15:10:24.460' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (233, 1, CAST(N'2025-02-10T15:11:21.470' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (234, 1, CAST(N'2025-02-10T15:14:01.443' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (235, 1, CAST(N'2025-02-10T15:16:45.990' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (236, 1, CAST(N'2025-02-10T15:17:53.793' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (237, 1, CAST(N'2025-02-10T15:20:24.370' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (238, 1, CAST(N'2025-02-10T15:26:21.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (239, 1, CAST(N'2025-02-10T15:40:00.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (240, 1, CAST(N'2025-02-10T15:43:13.863' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (241, 1, CAST(N'2025-02-10T15:54:48.880' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (242, 1, CAST(N'2025-02-10T16:03:27.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (243, 1, CAST(N'2025-02-10T16:06:50.383' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (244, 1, CAST(N'2025-02-11T10:35:02.287' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (245, 1, CAST(N'2025-02-11T10:47:53.137' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (246, 1, CAST(N'2025-02-11T11:09:33.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (247, 1, CAST(N'2025-02-11T14:24:08.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (248, 1, CAST(N'2025-02-11T14:34:51.067' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (249, 1, CAST(N'2025-02-11T14:35:11.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (250, 1, CAST(N'2025-02-11T14:37:07.287' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (251, 1, CAST(N'2025-02-11T14:51:39.010' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (252, 1, CAST(N'2025-02-11T14:58:34.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (253, 1, CAST(N'2025-02-11T15:04:40.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (254, 1, CAST(N'2025-02-11T15:07:16.363' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (255, 1, CAST(N'2025-02-11T15:09:39.650' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (256, 1, CAST(N'2025-02-11T15:47:18.010' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (257, 1, CAST(N'2025-02-11T15:54:40.673' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (258, 1, CAST(N'2025-02-12T10:08:18.487' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (259, 1, CAST(N'2025-02-12T10:14:16.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (260, 1, CAST(N'2025-02-12T10:32:54.813' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (261, 1, CAST(N'2025-02-12T10:36:27.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (262, 1, CAST(N'2025-02-12T10:38:13.143' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (263, 1, CAST(N'2025-02-12T11:00:45.117' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (264, 1, CAST(N'2025-02-12T11:36:23.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (265, 1, CAST(N'2025-02-12T11:42:55.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (266, 1, CAST(N'2025-02-12T14:05:34.497' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (267, 1, CAST(N'2025-02-12T14:11:37.530' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (268, 1, CAST(N'2025-02-12T14:20:35.820' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (269, 1, CAST(N'2025-02-12T14:26:59.383' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (270, 1, CAST(N'2025-02-12T14:42:30.750' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (271, 1, CAST(N'2025-02-12T15:19:24.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (272, 1, CAST(N'2025-02-12T15:40:56.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (273, 1, CAST(N'2025-02-12T15:45:18.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (274, 1, CAST(N'2025-02-12T15:48:42.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (275, 1, CAST(N'2025-02-12T15:48:42.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (276, 1, CAST(N'2025-02-12T15:51:16.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (277, 1, CAST(N'2025-02-12T15:54:06.107' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (278, 1, CAST(N'2025-02-12T15:58:25.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (279, 1, CAST(N'2025-02-12T16:03:58.300' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (280, 1, CAST(N'2025-02-12T16:09:16.957' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (281, 1, CAST(N'2025-02-12T16:14:09.580' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (282, 1, CAST(N'2025-02-12T16:16:08.330' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (283, 1, CAST(N'2025-02-13T10:21:46.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (284, 1, CAST(N'2025-02-13T12:00:25.280' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (285, 1, CAST(N'2025-02-13T12:02:58.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (286, 1, CAST(N'2025-02-13T12:14:44.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (287, 1, CAST(N'2025-02-13T12:20:46.963' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (288, 1, CAST(N'2025-02-13T12:45:34.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (289, 1, CAST(N'2025-02-13T12:52:14.113' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (290, 1, CAST(N'2025-02-13T13:00:48.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (291, 1, CAST(N'2025-02-13T13:13:25.963' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (292, 1, CAST(N'2025-02-13T13:21:01.227' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (293, 1, CAST(N'2025-02-13T13:22:32.657' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (294, 1, CAST(N'2025-02-13T13:24:36.573' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (295, 1, CAST(N'2025-02-13T13:46:36.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (296, 1, CAST(N'2025-02-13T13:51:52.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (297, 1, CAST(N'2025-02-13T13:52:57.020' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (298, 1, CAST(N'2025-02-13T13:54:22.827' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (299, 1, CAST(N'2025-02-13T14:22:17.767' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (300, 1, CAST(N'2025-02-13T14:23:20.623' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (301, 1, CAST(N'2025-02-13T14:30:39.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (302, 1, CAST(N'2025-02-13T14:38:26.253' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (303, 1, CAST(N'2025-02-13T14:39:29.630' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (304, 1, CAST(N'2025-02-13T14:40:03.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (305, 1, CAST(N'2025-02-13T14:41:48.403' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (306, 1, CAST(N'2025-02-13T15:35:26.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (307, 1, CAST(N'2025-02-13T15:43:06.860' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (308, 1, CAST(N'2025-02-13T15:45:43.223' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (309, 1, CAST(N'2025-02-14T10:55:47.763' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (310, 1, CAST(N'2025-02-14T11:08:38.080' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (311, 1, CAST(N'2025-02-14T11:16:00.650' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (312, 1, CAST(N'2025-02-14T11:24:37.083' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (313, 1, CAST(N'2025-02-14T11:26:42.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (314, 1, CAST(N'2025-02-14T11:28:47.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (315, 1, CAST(N'2025-02-14T11:34:24.393' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (316, 1, CAST(N'2025-02-14T11:38:50.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (317, 1, CAST(N'2025-02-14T11:42:16.467' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (318, 1, CAST(N'2025-02-14T12:17:21.823' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (319, 1, CAST(N'2025-02-14T12:19:07.683' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (320, 1, CAST(N'2025-02-14T12:21:56.960' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (321, 1, CAST(N'2025-02-14T13:10:33.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (322, 1, CAST(N'2025-02-14T13:25:07.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (323, 1, CAST(N'2025-02-14T14:10:40.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (324, 1, CAST(N'2025-02-14T14:16:07.663' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (325, 1, CAST(N'2025-02-14T14:40:13.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (326, 1, CAST(N'2025-02-14T14:41:40.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (327, 1, CAST(N'2025-02-14T14:45:01.227' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (328, 1, CAST(N'2025-02-14T15:56:12.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (329, 1, CAST(N'2025-02-16T11:44:00.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (330, 1, CAST(N'2025-02-16T11:47:04.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (331, 1, CAST(N'2025-02-16T11:54:16.373' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (332, 1, CAST(N'2025-02-16T11:55:09.263' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (333, 1, CAST(N'2025-02-17T09:50:15.880' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (334, 1, CAST(N'2025-02-17T10:22:28.950' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (335, 1, CAST(N'2025-02-17T12:52:30.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (336, 1, CAST(N'2025-02-17T13:23:40.580' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (337, 1, CAST(N'2025-02-17T13:32:48.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (338, 1, CAST(N'2025-02-17T13:39:00.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (339, 1, CAST(N'2025-02-17T13:54:24.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (340, 1, CAST(N'2025-02-17T14:07:49.860' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (341, 1, CAST(N'2025-02-17T14:36:37.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (342, 1, CAST(N'2025-02-17T14:40:54.400' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (343, 1, CAST(N'2025-02-17T14:42:25.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (344, 1, CAST(N'2025-02-17T14:44:33.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (345, 1, CAST(N'2025-02-17T14:54:47.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (346, 1, CAST(N'2025-02-17T15:02:40.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (347, 1, CAST(N'2025-02-17T15:21:15.447' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (348, 1, CAST(N'2025-02-17T15:22:53.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (349, 1, CAST(N'2025-02-17T15:25:25.190' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (350, 1, CAST(N'2025-02-17T15:29:14.700' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (351, 1, CAST(N'2025-02-17T15:47:42.737' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (352, 1, CAST(N'2025-02-17T15:57:34.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (353, 1, CAST(N'2025-02-17T16:04:08.440' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (354, 1, CAST(N'2025-02-17T16:08:42.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (355, 1, CAST(N'2025-02-17T16:16:36.693' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (356, 1, CAST(N'2025-02-17T18:45:31.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (357, 1, CAST(N'2025-02-17T18:55:37.530' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (358, 1, CAST(N'2025-02-17T19:11:15.380' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (359, 1, CAST(N'2025-02-17T19:26:41.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (360, 1, CAST(N'2025-02-17T20:30:28.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (361, 1, CAST(N'2025-02-17T21:19:06.450' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (362, 1, CAST(N'2025-02-17T21:44:25.690' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (363, 1, CAST(N'2025-02-17T21:53:10.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (364, 1, CAST(N'2025-02-17T22:09:37.330' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (365, 1, CAST(N'2025-02-17T22:14:27.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (366, 1, CAST(N'2025-02-17T22:18:26.713' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (367, 1, CAST(N'2025-02-17T22:19:13.703' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (368, 1, CAST(N'2025-02-18T11:06:18.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (369, 1, CAST(N'2025-02-18T11:10:04.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (370, 1, CAST(N'2025-02-18T11:16:16.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (371, 1, CAST(N'2025-02-18T11:21:28.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (372, 1, CAST(N'2025-02-18T11:39:35.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (373, 1, CAST(N'2025-02-18T11:53:41.330' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (374, 1, CAST(N'2025-02-18T13:08:54.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (375, 1, CAST(N'2025-02-18T13:57:28.823' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (376, 1, CAST(N'2025-02-18T14:03:34.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (377, 1, CAST(N'2025-02-18T14:11:03.863' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (378, 1, CAST(N'2025-02-18T15:01:41.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (379, 1, CAST(N'2025-02-18T15:12:02.277' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (380, 1, CAST(N'2025-02-18T15:18:07.827' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (381, 1, CAST(N'2025-02-18T15:22:43.567' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (382, 1, CAST(N'2025-02-18T15:38:37.823' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (383, 1, CAST(N'2025-02-18T15:41:49.010' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (384, 1, CAST(N'2025-02-18T15:46:50.873' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (385, 1, CAST(N'2025-02-18T16:06:13.937' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (386, 1, CAST(N'2025-02-18T16:07:17.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (387, 1, CAST(N'2025-02-18T16:12:22.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (388, 1, CAST(N'2025-02-19T10:00:04.930' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (389, 1, CAST(N'2025-02-19T10:02:48.020' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (390, 1, CAST(N'2025-02-19T10:16:11.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (391, 1, CAST(N'2025-02-19T10:22:05.590' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (392, 1, CAST(N'2025-02-19T11:24:17.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (393, 1, CAST(N'2025-02-19T12:01:03.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (394, 1, CAST(N'2025-02-19T12:24:28.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (395, 1, CAST(N'2025-02-19T12:44:09.443' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (396, 1, CAST(N'2025-02-19T12:45:05.720' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (397, 1, CAST(N'2025-02-19T12:54:19.950' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (398, 1, CAST(N'2025-02-19T13:14:51.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (399, 1, CAST(N'2025-02-19T13:17:33.047' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (400, 1, CAST(N'2025-02-19T13:30:53.177' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (401, 1, CAST(N'2025-02-19T13:34:02.393' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (402, 1, CAST(N'2025-02-19T13:37:04.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (403, 1, CAST(N'2025-02-19T13:42:48.070' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (404, 1, CAST(N'2025-02-19T13:45:29.233' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (405, 1, CAST(N'2025-02-19T13:57:07.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (406, 1, CAST(N'2025-02-19T13:59:26.440' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (407, 1, CAST(N'2025-02-19T14:04:24.730' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (408, 1, CAST(N'2025-02-19T14:07:47.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (409, 1, CAST(N'2025-02-19T14:35:06.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (410, 1, CAST(N'2025-02-20T09:28:06.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (411, 1, CAST(N'2025-02-20T09:28:39.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (412, 1, CAST(N'2025-02-20T11:24:23.590' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (413, 1, CAST(N'2025-02-20T13:22:23.873' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (414, 1, CAST(N'2025-02-20T13:26:02.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (415, 1, CAST(N'2025-02-20T13:46:01.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (416, 1, CAST(N'2025-02-20T13:58:27.750' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (417, 1, CAST(N'2025-02-20T14:23:58.087' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (418, 1, CAST(N'2025-02-20T14:31:41.113' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (419, 1, CAST(N'2025-02-20T15:33:03.440' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (420, 1, CAST(N'2025-02-20T15:34:24.393' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (421, 1, CAST(N'2025-02-20T15:35:43.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (422, 1, CAST(N'2025-02-20T15:37:42.397' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (423, 1, CAST(N'2025-02-20T15:38:07.693' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (424, 1, CAST(N'2025-02-20T15:41:02.087' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (425, 1, CAST(N'2025-02-20T16:58:08.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (426, 1, CAST(N'2025-02-20T17:04:59.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (427, 1, CAST(N'2025-02-20T17:17:25.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (428, 1, CAST(N'2025-02-20T17:18:58.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (429, 1, CAST(N'2025-02-20T17:19:29.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (430, 1, CAST(N'2025-02-20T17:19:52.847' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (431, 1, CAST(N'2025-02-20T17:31:03.887' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (432, 1, CAST(N'2025-02-20T17:36:15.470' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (433, 1, CAST(N'2025-02-20T17:38:27.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (434, 1, CAST(N'2025-02-20T17:46:54.567' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (435, 1, CAST(N'2025-02-20T17:48:41.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (436, 1, CAST(N'2025-02-20T17:51:36.753' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (437, 1, CAST(N'2025-02-20T17:52:59.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (438, 1, CAST(N'2025-02-20T17:53:53.150' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (439, 1, CAST(N'2025-02-20T17:55:26.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (440, 1, CAST(N'2025-02-20T18:52:00.347' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (441, 1, CAST(N'2025-02-20T18:54:45.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (442, 1, CAST(N'2025-02-20T18:56:49.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (443, 1, CAST(N'2025-02-20T19:17:29.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (444, 1, CAST(N'2025-02-21T10:25:08.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (445, 1, CAST(N'2025-02-21T10:27:20.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (446, 1, CAST(N'2025-02-21T10:37:18.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (447, 2, CAST(N'2025-02-21T13:27:54.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (448, 2, CAST(N'2025-02-21T13:31:02.447' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (449, 2, CAST(N'2025-02-21T13:36:27.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (450, 2, CAST(N'2025-02-21T13:42:32.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (451, 2, CAST(N'2025-02-21T13:46:14.890' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (452, 1, CAST(N'2025-02-21T13:57:18.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (453, 2, CAST(N'2025-02-21T14:05:09.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (454, 1, CAST(N'2025-02-22T12:37:56.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (455, 1, CAST(N'2025-02-22T13:34:22.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (456, 1, CAST(N'2025-02-22T13:52:11.087' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (457, 1, CAST(N'2025-02-22T14:01:06.373' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (458, 1, CAST(N'2025-02-22T14:01:51.303' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (459, 1, CAST(N'2025-02-22T14:11:47.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (460, 1, CAST(N'2025-02-22T14:29:33.970' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (461, 1, CAST(N'2025-02-22T15:16:21.400' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (462, 1, CAST(N'2025-02-22T15:28:14.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (463, 1, CAST(N'2025-02-22T15:33:52.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (464, 1, CAST(N'2025-02-22T15:43:25.933' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (465, 1, CAST(N'2025-02-22T15:46:02.453' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (466, 1, CAST(N'2025-02-22T15:53:04.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (467, 1, CAST(N'2025-02-22T16:12:49.747' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (468, 1, CAST(N'2025-02-22T16:15:20.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (469, 1, CAST(N'2025-02-22T16:18:59.633' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (470, 1, CAST(N'2025-02-22T16:23:32.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (471, 1, CAST(N'2025-02-22T17:15:38.437' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (472, 1, CAST(N'2025-02-22T17:18:33.743' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (473, 1, CAST(N'2025-02-22T17:20:18.863' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (474, 1, CAST(N'2025-02-24T09:22:59.990' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (475, 1, CAST(N'2025-02-24T10:01:12.987' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (476, 1, CAST(N'2025-02-24T10:09:54.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (477, 1, CAST(N'2025-02-24T12:05:32.577' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (478, 1, CAST(N'2025-02-24T12:07:02.833' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (479, 1, CAST(N'2025-02-24T12:07:52.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (480, 1, CAST(N'2025-02-24T12:11:55.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (481, 1, CAST(N'2025-02-24T13:09:11.980' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (482, 1, CAST(N'2025-02-24T13:14:18.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (483, 1, CAST(N'2025-02-24T13:17:53.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (484, 1, CAST(N'2025-02-24T13:32:37.507' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (485, 1, CAST(N'2025-02-24T13:47:13.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (486, 1, CAST(N'2025-02-24T13:52:33.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (487, 1, CAST(N'2025-02-24T13:59:36.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (488, 1, CAST(N'2025-02-24T14:18:54.417' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (489, 1, CAST(N'2025-02-24T14:37:58.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (490, 1, CAST(N'2025-02-24T14:54:57.843' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (491, 1, CAST(N'2025-02-24T15:07:28.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (492, 1, CAST(N'2025-02-24T15:35:52.467' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (493, 1, CAST(N'2025-02-24T15:38:30.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (494, 1, CAST(N'2025-02-24T15:43:07.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (495, 1, CAST(N'2025-02-24T15:49:45.860' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (496, 1, CAST(N'2025-02-24T15:55:55.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (497, 1, CAST(N'2025-02-24T16:02:52.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (498, 1, CAST(N'2025-02-24T16:10:56.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (499, 1, CAST(N'2025-02-25T13:51:17.297' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (500, 1, CAST(N'2025-02-25T13:53:40.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (501, 1, CAST(N'2025-02-25T14:05:33.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (502, 1, CAST(N'2025-02-25T14:11:47.497' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (503, 1, CAST(N'2025-02-25T14:42:32.470' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (504, 1, CAST(N'2025-02-25T14:45:53.740' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (505, 1, CAST(N'2025-02-25T14:49:04.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (506, 1, CAST(N'2025-02-25T14:55:52.260' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (507, 1, CAST(N'2025-02-25T14:57:20.143' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (508, 1, CAST(N'2025-02-25T14:58:32.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (509, 1, CAST(N'2025-02-25T15:05:12.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (510, 1, CAST(N'2025-02-25T15:07:25.037' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (511, 1, CAST(N'2025-02-25T15:18:52.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (512, 1, CAST(N'2025-02-25T15:22:40.517' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (513, 1, CAST(N'2025-02-25T15:35:08.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (514, 1, CAST(N'2025-02-25T15:41:48.487' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (515, 1, CAST(N'2025-02-25T16:15:35.783' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (516, 1, CAST(N'2025-02-25T16:21:13.123' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (517, 1, CAST(N'2025-02-25T16:27:54.957' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (518, 1, CAST(N'2025-02-25T17:58:18.037' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (519, 1, CAST(N'2025-02-25T18:00:28.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (520, 1, CAST(N'2025-02-25T20:45:52.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (521, 1, CAST(N'2025-02-25T21:40:30.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (522, 1, CAST(N'2025-02-25T21:45:41.937' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (523, 1, CAST(N'2025-02-25T21:57:12.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (524, 1, CAST(N'2025-02-25T22:06:47.883' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (525, 1, CAST(N'2025-02-25T22:09:32.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (526, 1, CAST(N'2025-02-25T22:10:19.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (527, 1, CAST(N'2025-02-26T09:48:00.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (528, 1, CAST(N'2025-02-27T12:08:20.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (529, 1, CAST(N'2025-02-27T12:48:43.833' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (530, 1, CAST(N'2025-02-28T17:33:53.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (531, 1, CAST(N'2025-02-28T17:46:40.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (532, 1, CAST(N'2025-02-28T17:49:56.757' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (533, 1, CAST(N'2025-02-28T18:01:30.330' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (534, 1, CAST(N'2025-02-28T18:09:49.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (535, 1, CAST(N'2025-02-28T19:06:41.687' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (536, 1, CAST(N'2025-02-28T19:08:27.627' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (537, 1, CAST(N'2025-02-28T19:12:57.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (538, 1, CAST(N'2025-02-28T19:21:35.877' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (539, 1, CAST(N'2025-02-28T19:37:04.467' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (540, 1, CAST(N'2025-02-28T19:48:54.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (541, 1, CAST(N'2025-02-28T20:02:49.760' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (542, 1, CAST(N'2025-03-01T13:35:17.150' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (543, 1, CAST(N'2025-03-01T13:38:53.477' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (544, 1, CAST(N'2025-03-01T13:43:41.527' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (545, 1, CAST(N'2025-03-01T13:45:07.943' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (546, 1, CAST(N'2025-03-01T14:10:15.020' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (547, 1, CAST(N'2025-03-01T14:12:57.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (548, 1, CAST(N'2025-03-01T14:19:51.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (549, 1, CAST(N'2025-03-01T14:21:03.600' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (550, 1, CAST(N'2025-03-01T14:25:51.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (551, 1, CAST(N'2025-03-01T14:34:56.357' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (552, 1, CAST(N'2025-03-02T11:54:20.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (553, 1, CAST(N'2025-03-02T11:56:26.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (554, 1, CAST(N'2025-03-02T12:14:16.930' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (555, 1, CAST(N'2025-03-02T12:16:02.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (556, 1, CAST(N'2025-03-02T12:53:05.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (557, 1, CAST(N'2025-03-02T13:07:37.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (558, 1, CAST(N'2025-03-02T13:54:42.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (559, 1, CAST(N'2025-03-03T09:46:13.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (560, 1, CAST(N'2025-03-03T09:48:04.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (561, 1, CAST(N'2025-03-03T10:14:14.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (562, 1, CAST(N'2025-03-03T10:15:28.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (563, 1, CAST(N'2025-03-03T10:49:36.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (564, 1, CAST(N'2025-03-03T11:23:59.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (565, 1, CAST(N'2025-03-03T11:26:46.317' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (566, 1, CAST(N'2025-03-03T11:31:38.937' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (567, 1, CAST(N'2025-03-03T11:53:22.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (568, 1, CAST(N'2025-03-03T11:54:41.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (569, 1, CAST(N'2025-03-03T12:34:49.033' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (570, 1, CAST(N'2025-03-03T13:12:53.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (571, 1, CAST(N'2025-03-03T14:57:13.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (572, 1, CAST(N'2025-03-03T15:23:01.703' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (573, 1, CAST(N'2025-03-03T15:33:39.620' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (574, 1, CAST(N'2025-03-03T15:44:32.907' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (575, 1, CAST(N'2025-03-03T15:51:24.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (576, 1, CAST(N'2025-03-03T16:13:21.033' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (577, 1, CAST(N'2025-03-03T16:15:55.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (578, 1, CAST(N'2025-03-04T12:28:08.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (579, 1, CAST(N'2025-03-04T12:47:07.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (580, 1, CAST(N'2025-03-04T13:28:28.343' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (581, 1, CAST(N'2025-03-04T13:58:36.960' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (582, 1, CAST(N'2025-03-04T14:58:29.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (583, 1, CAST(N'2025-03-04T15:36:22.700' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (584, 1, CAST(N'2025-03-04T15:40:13.587' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (585, 1, CAST(N'2025-03-04T20:10:50.417' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (586, 1, CAST(N'2025-03-04T20:44:57.037' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (587, 1, CAST(N'2025-03-04T20:50:08.993' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (588, 1, CAST(N'2025-03-05T20:18:32.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (589, 1, CAST(N'2025-03-05T20:20:06.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (590, 1, CAST(N'2025-03-05T20:21:10.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (591, 1, CAST(N'2025-03-05T20:24:34.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (592, 1, CAST(N'2025-03-05T20:26:34.907' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (593, 1, CAST(N'2025-03-05T20:33:47.713' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (594, 1, CAST(N'2025-03-05T20:38:17.980' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (595, 1, CAST(N'2025-03-05T20:44:30.277' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (596, 1, CAST(N'2025-03-05T20:52:16.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (597, 1, CAST(N'2025-03-05T21:01:37.970' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (598, 1, CAST(N'2025-03-05T21:03:28.223' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (599, 1, CAST(N'2025-03-05T21:13:07.833' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (600, 1, CAST(N'2025-03-06T09:22:42.593' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (601, 1, CAST(N'2025-03-06T09:36:36.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (602, 1, CAST(N'2025-03-06T11:32:08.253' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (603, 1, CAST(N'2025-03-06T12:47:12.460' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (604, 1, CAST(N'2025-03-06T13:08:12.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (605, 1, CAST(N'2025-03-06T13:48:37.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (606, 1, CAST(N'2025-03-06T14:44:47.783' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (607, 1, CAST(N'2025-03-06T15:00:54.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (608, 1, CAST(N'2025-03-06T15:04:26.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (609, 1, CAST(N'2025-03-06T15:08:28.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (610, 1, CAST(N'2025-03-06T15:18:07.883' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (611, 1, CAST(N'2025-03-07T11:13:39.853' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (612, 1, CAST(N'2025-03-07T11:20:36.553' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (613, 1, CAST(N'2025-03-07T13:16:19.747' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (614, 1, CAST(N'2025-03-07T13:31:35.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (615, 1, CAST(N'2025-03-07T13:38:39.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (616, 1, CAST(N'2025-03-07T13:54:22.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (617, 1, CAST(N'2025-03-07T14:04:27.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (618, 1, CAST(N'2025-03-07T17:32:45.020' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (619, 1, CAST(N'2025-03-10T09:29:56.860' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (620, 1, CAST(N'2025-03-10T09:46:40.287' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (621, 1, CAST(N'2025-03-10T10:00:42.850' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (622, 1, CAST(N'2025-03-10T16:10:55.833' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (623, 1, CAST(N'2025-03-10T16:12:13.437' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (624, 1, CAST(N'2025-03-10T22:06:46.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (625, 1, CAST(N'2025-03-10T22:10:52.217' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (626, 1, CAST(N'2025-03-10T22:11:51.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (627, 1, CAST(N'2025-03-10T22:15:24.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (628, 1, CAST(N'2025-03-10T22:16:15.627' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (629, 1, CAST(N'2025-03-10T22:17:27.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (630, 1, CAST(N'2025-03-10T22:26:10.533' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (631, 1, CAST(N'2025-03-11T10:56:15.247' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (632, 1, CAST(N'2025-03-11T11:06:11.493' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (633, 1, CAST(N'2025-03-11T12:21:21.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (634, 1, CAST(N'2025-03-11T12:36:12.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (635, 1, CAST(N'2025-03-11T12:38:34.820' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (636, 1, CAST(N'2025-03-11T12:41:14.603' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (637, 1, CAST(N'2025-03-11T12:43:23.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (638, 1, CAST(N'2025-03-11T12:50:33.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (639, 1, CAST(N'2025-03-11T12:54:33.237' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (640, 1, CAST(N'2025-03-11T12:55:41.743' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (641, 1, CAST(N'2025-03-11T12:58:58.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (642, 1, CAST(N'2025-03-11T12:59:23.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (643, 1, CAST(N'2025-03-11T13:00:15.040' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (644, 1, CAST(N'2025-03-11T13:05:53.870' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (645, 1, CAST(N'2025-03-11T13:15:46.253' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (646, 1, CAST(N'2025-03-11T13:19:31.437' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (647, 1, CAST(N'2025-03-11T13:25:58.630' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (648, 1, CAST(N'2025-03-11T13:39:12.240' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (649, 1, CAST(N'2025-03-11T13:40:27.070' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (650, 1, CAST(N'2025-03-11T13:43:38.263' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (651, 1, CAST(N'2025-03-11T13:47:59.703' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (652, 1, CAST(N'2025-03-11T14:40:25.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (653, 1, CAST(N'2025-03-11T14:45:00.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (654, 1, CAST(N'2025-03-11T14:56:22.277' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (655, 1, CAST(N'2025-03-11T15:07:41.057' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (656, 1, CAST(N'2025-03-11T15:20:06.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (657, 1, CAST(N'2025-03-11T15:44:38.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (658, 1, CAST(N'2025-03-11T15:59:00.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (659, 1, CAST(N'2025-03-11T16:13:18.137' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (660, 1, CAST(N'2025-03-11T16:14:44.347' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (661, 1, CAST(N'2025-03-11T16:28:13.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (662, 1, CAST(N'2025-03-11T16:29:01.783' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (663, 1, CAST(N'2025-03-11T16:31:03.370' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (664, 1, CAST(N'2025-03-11T16:42:25.260' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (665, 1, CAST(N'2025-03-12T15:42:14.467' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (666, 1, CAST(N'2025-03-13T10:44:11.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (667, 2, CAST(N'2025-03-13T11:30:14.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (668, 2, CAST(N'2025-03-13T13:49:27.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (669, 2, CAST(N'2025-03-13T14:02:08.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (670, 2, CAST(N'2025-03-13T14:13:47.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (671, 2, CAST(N'2025-03-13T14:15:02.613' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (672, 2, CAST(N'2025-03-13T14:43:08.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (673, 2, CAST(N'2025-03-13T14:46:16.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (674, 2, CAST(N'2025-03-13T14:47:28.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (675, 2, CAST(N'2025-03-13T14:55:00.873' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (676, 2, CAST(N'2025-03-13T15:02:41.683' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (677, 2, CAST(N'2025-03-13T15:08:44.593' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (678, 2, CAST(N'2025-03-13T15:10:59.827' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (679, 2, CAST(N'2025-03-13T15:15:14.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (680, 2, CAST(N'2025-03-14T11:07:46.587' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (681, 2, CAST(N'2025-03-14T11:10:19.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (682, 1, CAST(N'2025-03-17T12:50:45.113' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (683, 1, CAST(N'2025-03-17T13:02:24.817' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (684, 1, CAST(N'2025-03-17T13:03:11.640' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (685, 1, CAST(N'2025-03-17T13:06:31.343' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (686, 1, CAST(N'2025-03-17T13:27:55.640' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (687, 1, CAST(N'2025-03-17T13:41:41.637' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (688, 1, CAST(N'2025-03-17T13:56:11.107' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (689, 1, CAST(N'2025-03-17T13:58:53.753' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (690, 1, CAST(N'2025-03-17T13:59:52.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (691, 1, CAST(N'2025-03-17T14:02:05.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (692, 1, CAST(N'2025-03-17T14:07:13.623' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (693, 1, CAST(N'2025-03-17T14:16:52.783' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (694, 1, CAST(N'2025-03-17T14:18:49.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (695, 1, CAST(N'2025-03-17T14:43:58.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (696, 1, CAST(N'2025-03-17T15:11:31.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (697, 1, CAST(N'2025-03-17T15:13:04.237' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (698, 1, CAST(N'2025-03-17T15:22:20.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (699, 1, CAST(N'2025-03-17T15:24:57.947' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (700, 1, CAST(N'2025-03-17T15:27:38.780' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (701, 1, CAST(N'2025-03-17T15:48:24.167' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (702, 1, CAST(N'2025-03-17T16:00:36.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (703, 1, CAST(N'2025-03-17T16:06:36.093' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (704, 1, CAST(N'2025-03-17T16:08:37.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (705, 1, CAST(N'2025-03-17T16:11:03.720' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (706, 1, CAST(N'2025-03-20T14:31:38.933' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (707, 1, CAST(N'2025-03-21T11:55:27.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (708, 1, CAST(N'2025-03-21T12:04:43.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (709, 1, CAST(N'2025-03-21T12:06:41.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (710, 1, CAST(N'2025-03-21T12:51:51.927' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (711, 1, CAST(N'2025-03-21T13:13:29.357' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (712, 1, CAST(N'2025-03-21T13:19:24.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (713, 1, CAST(N'2025-03-21T13:23:42.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (714, 1, CAST(N'2025-03-21T13:59:53.910' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (715, 1, CAST(N'2025-03-21T14:20:54.400' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (716, 1, CAST(N'2025-03-21T14:30:31.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (717, 1, CAST(N'2025-03-21T14:34:49.287' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (718, 1, CAST(N'2025-03-21T15:10:34.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (719, 1, CAST(N'2025-03-21T15:37:33.817' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (720, 1, CAST(N'2025-03-21T16:50:58.897' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (721, 1, CAST(N'2025-03-21T16:58:13.803' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (722, 1, CAST(N'2025-03-21T17:06:11.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (723, 1, CAST(N'2025-03-21T18:33:32.440' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (724, 1, CAST(N'2025-03-21T18:33:55.243' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (725, 1, CAST(N'2025-03-21T18:49:41.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (726, 1, CAST(N'2025-03-21T18:51:21.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (727, 1, CAST(N'2025-03-21T19:21:27.173' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (728, 1, CAST(N'2025-03-21T19:27:35.700' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (729, 1, CAST(N'2025-03-21T19:34:30.500' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (730, 1, CAST(N'2025-03-21T19:37:17.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (731, 1, CAST(N'2025-03-21T19:43:11.057' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (732, 1, CAST(N'2025-03-21T19:44:01.063' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (733, 1, CAST(N'2025-03-21T19:51:58.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (734, 1, CAST(N'2025-03-21T19:53:50.527' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (735, 1, CAST(N'2025-03-21T19:56:06.363' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (736, 1, CAST(N'2025-03-21T19:58:39.417' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (737, 1, CAST(N'2025-03-23T14:20:43.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (738, 1, CAST(N'2025-03-24T09:39:57.060' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (739, 1, CAST(N'2025-03-24T09:45:28.690' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (740, 1, CAST(N'2025-03-24T11:33:29.657' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (741, 1, CAST(N'2025-03-24T11:37:39.567' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (742, 1, CAST(N'2025-03-24T12:20:18.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (743, 1, CAST(N'2025-03-24T12:22:22.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (744, 1, CAST(N'2025-03-24T12:40:31.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (745, 1, CAST(N'2025-03-24T12:47:10.317' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (746, 1, CAST(N'2025-03-24T12:50:43.663' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (747, 1, CAST(N'2025-03-24T13:00:10.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (748, 1, CAST(N'2025-03-24T13:02:22.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (749, 1, CAST(N'2025-03-24T13:02:59.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (750, 1, CAST(N'2025-03-24T13:59:46.933' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (751, 1, CAST(N'2025-03-24T14:27:05.560' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (752, 1, CAST(N'2025-03-24T14:28:23.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (753, 1, CAST(N'2025-03-24T14:29:45.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (754, 1, CAST(N'2025-03-24T14:48:12.340' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (755, 1, CAST(N'2025-03-24T14:58:57.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (756, 1, CAST(N'2025-03-24T15:46:19.563' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (757, 1, CAST(N'2025-03-24T15:47:01.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (758, 1, CAST(N'2025-03-24T15:59:21.897' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (759, 1, CAST(N'2025-03-24T16:00:13.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (760, 1, CAST(N'2025-03-24T16:12:16.453' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (761, 1, CAST(N'2025-03-24T16:13:22.087' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (762, 1, CAST(N'2025-03-24T16:14:13.267' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (763, 1, CAST(N'2025-03-24T16:24:55.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (764, 1, CAST(N'2025-03-24T16:31:02.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (765, 1, CAST(N'2025-03-24T16:33:34.257' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (766, 1, CAST(N'2025-03-24T16:47:17.257' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (767, 1, CAST(N'2025-03-24T16:48:54.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (768, 1, CAST(N'2025-03-24T16:49:51.817' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (769, 1, CAST(N'2025-03-25T09:48:53.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (770, 1, CAST(N'2025-03-25T10:01:59.683' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (771, 1, CAST(N'2025-03-25T10:39:56.777' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (772, 1, CAST(N'2025-03-25T10:43:49.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (773, 1, CAST(N'2025-03-25T10:44:27.963' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (774, 1, CAST(N'2025-03-25T10:59:05.780' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (775, 1, CAST(N'2025-03-25T11:30:28.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (776, 1, CAST(N'2025-03-25T11:41:31.023' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (777, 1, CAST(N'2025-03-25T11:45:00.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (778, 1, CAST(N'2025-03-25T12:51:37.517' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (779, 1, CAST(N'2025-03-25T12:57:02.117' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (780, 1, CAST(N'2025-03-25T12:59:36.833' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (781, 1, CAST(N'2025-03-25T13:23:02.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (782, 1, CAST(N'2025-03-25T13:25:02.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (783, 1, CAST(N'2025-03-25T13:43:52.140' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (784, 1, CAST(N'2025-03-25T13:52:23.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (785, 1, CAST(N'2025-03-25T13:53:16.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (786, 1, CAST(N'2025-03-25T14:06:40.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (787, 1, CAST(N'2025-03-25T15:28:45.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (788, 1, CAST(N'2025-03-25T15:45:08.637' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (789, 1, CAST(N'2025-03-25T16:37:10.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (790, 1, CAST(N'2025-03-26T09:58:47.657' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (791, 1, CAST(N'2025-03-26T11:24:43.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (792, 1, CAST(N'2025-03-26T11:26:37.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (793, 1, CAST(N'2025-03-26T11:31:59.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (794, 1, CAST(N'2025-03-26T11:36:59.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (795, 1, CAST(N'2025-03-26T12:28:56.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (796, 1, CAST(N'2025-03-26T13:15:34.650' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (797, 1, CAST(N'2025-03-26T16:22:33.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (798, 1, CAST(N'2025-03-26T16:25:44.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (799, 1, CAST(N'2025-03-26T16:27:41.390' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (800, 1, CAST(N'2025-03-27T10:04:32.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (801, 2, CAST(N'2025-03-27T13:21:56.920' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (802, 2, CAST(N'2025-03-27T13:22:22.057' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (803, 2, CAST(N'2025-03-27T14:26:19.923' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (804, 2, CAST(N'2025-03-27T14:28:25.223' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (805, 2, CAST(N'2025-03-27T14:33:59.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (806, 2, CAST(N'2025-03-27T14:35:20.107' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (807, 2, CAST(N'2025-03-27T14:51:51.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (808, 2, CAST(N'2025-03-27T14:55:47.803' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (809, 1, CAST(N'2025-03-28T13:59:47.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (810, 1, CAST(N'2025-03-28T14:03:36.737' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (811, 1, CAST(N'2025-03-28T14:21:04.427' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (812, 1, CAST(N'2025-03-28T14:24:22.983' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (813, 1, CAST(N'2025-03-28T14:30:51.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (814, 1, CAST(N'2025-03-28T16:42:11.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (815, 1, CAST(N'2025-03-28T16:47:05.687' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (816, 1, CAST(N'2025-03-28T17:04:57.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (817, 1, CAST(N'2025-03-28T17:35:31.600' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (818, 1, CAST(N'2025-03-28T17:47:17.897' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (819, 1, CAST(N'2025-03-28T18:15:29.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (820, 1, CAST(N'2025-03-28T18:18:36.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (821, 1, CAST(N'2025-03-28T18:22:04.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (822, 1, CAST(N'2025-03-28T18:24:26.890' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (823, 1, CAST(N'2025-03-28T18:26:42.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (824, 1, CAST(N'2025-03-28T18:38:27.987' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (825, 1, CAST(N'2025-03-28T19:16:09.913' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (826, 1, CAST(N'2025-03-28T19:28:26.527' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (827, 1, CAST(N'2025-03-28T19:30:45.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (828, 1, CAST(N'2025-03-28T19:36:48.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (829, 1, CAST(N'2025-03-28T19:48:58.663' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (830, 1, CAST(N'2025-03-28T19:56:47.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (831, 1, CAST(N'2025-03-28T20:06:56.067' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (832, 1, CAST(N'2025-03-28T20:07:44.693' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (833, 1, CAST(N'2025-03-28T20:08:52.407' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (834, 1, CAST(N'2025-03-28T20:10:17.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (835, 1, CAST(N'2025-03-28T20:12:18.887' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (836, 1, CAST(N'2025-03-28T20:16:58.040' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (837, 1, CAST(N'2025-03-28T20:22:24.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (838, 1, CAST(N'2025-03-28T20:25:34.647' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (839, 1, CAST(N'2025-03-28T20:28:41.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (840, 1, CAST(N'2025-03-28T20:32:23.390' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (841, 1, CAST(N'2025-03-28T20:34:35.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (842, 1, CAST(N'2025-03-28T20:37:01.510' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (843, 1, CAST(N'2025-03-28T20:39:06.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (844, 1, CAST(N'2025-03-28T20:43:12.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (845, 1, CAST(N'2025-03-28T20:49:15.407' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (846, 1, CAST(N'2025-03-28T20:51:14.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (847, 1, CAST(N'2025-03-28T20:54:36.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (848, 1, CAST(N'2025-03-28T20:57:48.130' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (849, 1, CAST(N'2025-03-28T22:22:10.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (850, 1, CAST(N'2025-03-28T22:25:51.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (851, 1, CAST(N'2025-03-28T22:29:04.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (852, 1, CAST(N'2025-03-28T22:35:18.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (853, 1, CAST(N'2025-03-28T22:45:29.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (854, 1, CAST(N'2025-03-28T22:50:48.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (855, 1, CAST(N'2025-03-28T22:53:17.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (856, 1, CAST(N'2025-03-28T22:57:39.407' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (857, 1, CAST(N'2025-03-28T23:03:02.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (858, 1, CAST(N'2025-03-28T23:04:04.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (859, 1, CAST(N'2025-03-28T23:05:57.070' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (860, 1, CAST(N'2025-03-28T23:07:39.090' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (861, 1, CAST(N'2025-03-28T23:13:12.897' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (862, 1, CAST(N'2025-03-28T23:15:12.840' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (863, 1, CAST(N'2025-03-28T23:17:46.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (864, 1, CAST(N'2025-03-28T23:19:25.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (865, 1, CAST(N'2025-03-28T23:24:47.907' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (866, 1, CAST(N'2025-03-28T23:28:51.537' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (867, 1, CAST(N'2025-03-28T23:34:48.753' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (868, 1, CAST(N'2025-03-28T23:37:27.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (869, 1, CAST(N'2025-03-28T23:40:03.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (870, 1, CAST(N'2025-03-28T23:41:57.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (871, 1, CAST(N'2025-03-28T23:48:19.890' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (872, 1, CAST(N'2025-03-28T23:52:32.920' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (873, 1, CAST(N'2025-03-28T23:57:28.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (874, 1, CAST(N'2025-03-29T19:18:35.380' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (875, 1, CAST(N'2025-03-29T19:27:41.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (876, 1, CAST(N'2025-03-29T20:07:43.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (877, 1, CAST(N'2025-03-29T20:19:49.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (878, 1, CAST(N'2025-03-29T20:44:34.850' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (879, 1, CAST(N'2025-03-29T20:51:24.657' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (880, 1, CAST(N'2025-03-29T21:06:16.580' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (881, 1, CAST(N'2025-03-29T21:08:15.553' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (882, 1, CAST(N'2025-03-29T21:29:17.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (883, 1, CAST(N'2025-03-29T21:39:19.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (884, 1, CAST(N'2025-03-29T22:04:51.693' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (885, 1, CAST(N'2025-03-29T22:29:07.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (886, 1, CAST(N'2025-03-29T22:40:52.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (887, 1, CAST(N'2025-03-29T23:02:25.913' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (888, 1, CAST(N'2025-03-29T23:08:09.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (889, 1, CAST(N'2025-03-29T23:15:30.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (890, 1, CAST(N'2025-03-29T23:23:32.427' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (891, 1, CAST(N'2025-03-30T12:43:04.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (892, 1, CAST(N'2025-03-30T12:48:18.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (893, 1, CAST(N'2025-03-30T12:51:07.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (894, 1, CAST(N'2025-03-30T12:54:09.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (895, 1, CAST(N'2025-03-30T12:58:43.040' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (896, 1, CAST(N'2025-03-30T13:02:10.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (897, 1, CAST(N'2025-03-30T13:06:21.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (898, 1, CAST(N'2025-03-30T13:29:27.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (899, 1, CAST(N'2025-03-30T13:59:21.167' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (900, 1, CAST(N'2025-03-30T20:11:53.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (901, 1, CAST(N'2025-03-30T20:41:26.817' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (902, 1, CAST(N'2025-03-30T20:54:14.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (903, 1, CAST(N'2025-03-30T20:56:22.517' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (904, 1, CAST(N'2025-03-30T21:06:33.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (905, 1, CAST(N'2025-03-30T21:11:06.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (906, 1, CAST(N'2025-03-30T21:36:07.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (907, 1, CAST(N'2025-03-30T22:04:10.870' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (908, 1, CAST(N'2025-03-30T22:11:25.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (909, 1, CAST(N'2025-03-30T22:24:02.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (910, 1, CAST(N'2025-03-30T22:36:11.210' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (911, 1, CAST(N'2025-03-30T22:38:11.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (912, 1, CAST(N'2025-03-30T22:40:55.963' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (913, 1, CAST(N'2025-03-30T22:44:08.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (914, 1, CAST(N'2025-03-30T22:49:10.603' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (915, 1, CAST(N'2025-03-30T23:21:34.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (916, 1, CAST(N'2025-03-30T23:22:21.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (917, 1, CAST(N'2025-03-30T23:31:31.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (918, 1, CAST(N'2025-03-30T23:33:14.523' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (919, 1, CAST(N'2025-03-30T23:38:22.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (920, 1, CAST(N'2025-03-30T23:40:32.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (921, 1, CAST(N'2025-03-30T23:53:41.397' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (922, 1, CAST(N'2025-03-31T00:04:32.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (923, 1, CAST(N'2025-03-31T11:52:46.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (924, 1, CAST(N'2025-03-31T12:23:06.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (925, 1, CAST(N'2025-03-31T12:30:14.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (926, 1, CAST(N'2025-03-31T12:37:45.493' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (927, 1, CAST(N'2025-03-31T13:36:09.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (928, 1, CAST(N'2025-03-31T13:39:10.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (929, 1, CAST(N'2025-03-31T13:44:48.943' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (930, 1, CAST(N'2025-03-31T14:02:16.913' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (931, 1, CAST(N'2025-03-31T14:03:42.033' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (932, 1, CAST(N'2025-03-31T14:04:27.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (933, 1, CAST(N'2025-03-31T14:23:10.417' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (934, 1, CAST(N'2025-03-31T14:41:39.903' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (935, 1, CAST(N'2025-04-01T10:10:02.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (936, 1, CAST(N'2025-04-01T10:25:59.653' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (937, 1, CAST(N'2025-04-01T12:38:11.343' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (938, 1, CAST(N'2025-04-01T12:44:02.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (939, 1, CAST(N'2025-04-01T12:59:01.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (940, 1, CAST(N'2025-04-01T13:00:55.503' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (941, 1, CAST(N'2025-04-01T14:10:05.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (942, 1, CAST(N'2025-04-02T15:25:44.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (943, 1, CAST(N'2025-04-02T15:35:18.743' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (944, 1, CAST(N'2025-04-02T15:38:44.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (945, 1, CAST(N'2025-04-02T15:46:11.813' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (946, 1, CAST(N'2025-04-02T15:50:44.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (947, 1, CAST(N'2025-04-02T16:02:32.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (948, 1, CAST(N'2025-04-02T16:05:59.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (949, 1, CAST(N'2025-04-02T16:18:54.477' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (950, 1, CAST(N'2025-04-02T16:21:39.247' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (951, 1, CAST(N'2025-04-02T16:29:47.297' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (952, 1, CAST(N'2025-04-02T16:37:22.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (953, 1, CAST(N'2025-04-02T18:46:35.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (954, 1, CAST(N'2025-04-02T18:57:58.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (955, 1, CAST(N'2025-04-02T19:23:14.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (956, 1, CAST(N'2025-04-02T20:01:33.557' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (957, 1, CAST(N'2025-04-02T20:20:02.453' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (958, 1, CAST(N'2025-04-02T20:29:55.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (959, 1, CAST(N'2025-04-02T20:47:34.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (960, 1, CAST(N'2025-04-02T21:16:18.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (961, 1, CAST(N'2025-04-02T21:37:23.037' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (962, 1, CAST(N'2025-04-02T21:43:01.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (963, 1, CAST(N'2025-04-02T21:45:44.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (964, 1, CAST(N'2025-04-02T21:49:12.010' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (965, 1, CAST(N'2025-04-02T23:15:59.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (966, 1, CAST(N'2025-04-02T23:22:35.780' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (967, 1, CAST(N'2025-04-02T23:48:27.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (968, 1, CAST(N'2025-04-02T23:54:52.410' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (969, 1, CAST(N'2025-04-02T23:56:43.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (970, 1, CAST(N'2025-04-02T23:58:42.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (971, 1, CAST(N'2025-04-03T21:51:15.193' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (972, 1, CAST(N'2025-04-03T21:52:17.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (973, 1, CAST(N'2025-04-03T21:55:16.107' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (974, 1, CAST(N'2025-04-03T22:11:38.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (975, 1, CAST(N'2025-04-03T22:17:46.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (976, 1, CAST(N'2025-04-03T22:23:43.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (977, 1, CAST(N'2025-04-03T22:31:12.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (978, 1, CAST(N'2025-04-03T22:36:02.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (979, 1, CAST(N'2025-04-03T23:30:06.133' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (980, 1, CAST(N'2025-04-03T23:40:46.713' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (981, 1, CAST(N'2025-04-03T23:44:42.333' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (982, 1, CAST(N'2025-04-03T23:52:41.113' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (983, 1, CAST(N'2025-04-04T00:00:49.190' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (984, 1, CAST(N'2025-04-04T00:06:38.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (985, 1, CAST(N'2025-04-04T00:11:15.180' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (986, 1, CAST(N'2025-04-04T00:21:55.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (987, 1, CAST(N'2025-04-04T00:24:15.280' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (988, 1, CAST(N'2025-04-04T00:28:05.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (989, 1, CAST(N'2025-04-04T00:30:22.223' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (990, 1, CAST(N'2025-04-04T00:36:21.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (991, 1, CAST(N'2025-04-04T00:38:58.283' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (992, 1, CAST(N'2025-04-04T13:31:03.927' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (993, 1, CAST(N'2025-04-04T13:36:33.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (994, 1, CAST(N'2025-04-04T13:43:34.517' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (995, 1, CAST(N'2025-04-04T13:51:26.913' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (996, 1, CAST(N'2025-04-04T13:57:39.240' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (997, 1, CAST(N'2025-04-04T14:08:03.270' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (998, 1, CAST(N'2025-04-04T14:18:25.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (999, 1, CAST(N'2025-04-04T14:36:26.647' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1000, 1, CAST(N'2025-04-04T14:41:18.207' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1001, 1, CAST(N'2025-04-04T14:45:52.920' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1002, 1, CAST(N'2025-04-04T15:05:24.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1003, 1, CAST(N'2025-04-04T15:12:15.167' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1004, 1, CAST(N'2025-04-04T15:15:26.847' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1005, 1, CAST(N'2025-04-04T15:20:39.793' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1006, 1, CAST(N'2025-04-04T15:24:44.370' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1007, 1, CAST(N'2025-04-04T15:35:42.130' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1008, 1, CAST(N'2025-04-04T16:34:32.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1009, 1, CAST(N'2025-04-04T16:45:55.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1010, 1, CAST(N'2025-04-04T16:57:09.430' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1011, 1, CAST(N'2025-04-04T16:59:24.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1012, 1, CAST(N'2025-04-04T17:02:20.107' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1013, 1, CAST(N'2025-04-04T17:20:21.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1014, 1, CAST(N'2025-04-04T17:24:35.507' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1015, 1, CAST(N'2025-04-04T17:32:46.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1016, 1, CAST(N'2025-04-04T17:34:00.343' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1017, 1, CAST(N'2025-04-04T17:35:16.240' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1018, 1, CAST(N'2025-04-04T18:13:08.063' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1019, 1, CAST(N'2025-04-04T18:25:20.177' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1020, 1, CAST(N'2025-04-04T18:40:00.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1021, 1, CAST(N'2025-04-05T14:46:08.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1022, 1, CAST(N'2025-04-05T15:00:03.833' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1023, 1, CAST(N'2025-04-05T15:01:50.247' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1024, 1, CAST(N'2025-04-05T15:18:08.690' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1025, 1, CAST(N'2025-04-05T15:26:11.987' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1026, 1, CAST(N'2025-04-05T15:48:49.163' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1027, 1, CAST(N'2025-04-05T15:54:25.980' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1028, 1, CAST(N'2025-04-05T15:57:56.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1029, 1, CAST(N'2025-04-05T16:11:51.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1030, 1, CAST(N'2025-04-05T16:18:15.263' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1031, 1, CAST(N'2025-04-05T16:19:47.233' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1032, 1, CAST(N'2025-04-05T16:30:45.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1033, 1, CAST(N'2025-04-05T17:04:50.477' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1034, 1, CAST(N'2025-04-05T17:28:22.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1035, 1, CAST(N'2025-04-05T17:36:53.970' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1036, 1, CAST(N'2025-04-05T17:46:35.730' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1037, 1, CAST(N'2025-04-05T17:51:06.417' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1038, 1, CAST(N'2025-04-05T17:52:53.737' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1039, 1, CAST(N'2025-04-05T17:57:41.390' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1040, 1, CAST(N'2025-04-07T08:57:01.273' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1041, 2, CAST(N'2025-04-07T14:58:22.290' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1042, 2, CAST(N'2025-04-07T15:03:06.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1043, 2, CAST(N'2025-04-07T15:20:17.780' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1044, 2, CAST(N'2025-04-07T15:51:54.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1045, 2, CAST(N'2025-04-07T15:55:06.603' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1046, 2, CAST(N'2025-04-08T10:07:31.297' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1047, 2, CAST(N'2025-04-08T10:09:53.503' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1048, 2, CAST(N'2025-04-08T10:10:30.567' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1049, 2, CAST(N'2025-04-08T10:14:22.217' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1050, 2, CAST(N'2025-04-08T10:16:44.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1051, 2, CAST(N'2025-04-08T10:18:37.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1052, 2, CAST(N'2025-04-08T10:22:05.787' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1053, 2, CAST(N'2025-04-08T10:23:48.933' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1054, 2, CAST(N'2025-04-08T10:54:57.237' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1055, 2, CAST(N'2025-04-08T11:02:19.450' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1056, 2, CAST(N'2025-04-08T11:06:25.373' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1057, 2, CAST(N'2025-04-08T11:17:05.730' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1058, 2, CAST(N'2025-04-08T11:17:55.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1059, 2, CAST(N'2025-04-08T11:29:28.210' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1060, 2, CAST(N'2025-04-08T11:38:52.090' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1061, 2, CAST(N'2025-04-08T11:41:38.500' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1062, 1, CAST(N'2025-04-08T13:44:33.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1063, 1, CAST(N'2025-04-08T13:50:29.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1064, 1, CAST(N'2025-04-08T13:56:56.000' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1065, 1, CAST(N'2025-04-08T14:18:54.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1066, 1, CAST(N'2025-04-08T14:19:39.240' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1067, 1, CAST(N'2025-04-08T14:24:24.293' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1068, 2, CAST(N'2025-04-08T15:09:47.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1069, 2, CAST(N'2025-04-08T15:18:36.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1070, 1, CAST(N'2025-04-08T15:20:22.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1071, 2, CAST(N'2025-04-08T15:23:07.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1072, 2, CAST(N'2025-04-08T15:25:25.580' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1073, 2, CAST(N'2025-04-08T15:29:19.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1074, 2, CAST(N'2025-04-08T15:32:38.890' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1075, 2, CAST(N'2025-04-08T15:34:58.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1076, 2, CAST(N'2025-04-08T15:38:12.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1077, 2, CAST(N'2025-04-08T15:39:57.443' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1078, 2, CAST(N'2025-04-08T15:40:43.960' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1079, 2, CAST(N'2025-04-08T15:44:53.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1080, 1, CAST(N'2025-04-08T19:52:33.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1081, 1, CAST(N'2025-04-08T19:53:31.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1082, 1, CAST(N'2025-04-08T19:55:00.380' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1083, 1, CAST(N'2025-04-08T19:57:17.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1084, 1, CAST(N'2025-04-08T21:12:29.610' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1085, 1, CAST(N'2025-04-09T00:18:14.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1086, 1, CAST(N'2025-04-09T00:23:42.730' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1087, 1, CAST(N'2025-04-09T08:49:58.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1088, 1, CAST(N'2025-04-09T12:27:09.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1089, 1, CAST(N'2025-04-09T12:31:07.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1090, 1, CAST(N'2025-04-09T12:37:34.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1091, 1, CAST(N'2025-04-09T12:43:40.870' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1092, 1, CAST(N'2025-04-09T12:48:05.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1093, 1, CAST(N'2025-04-09T13:10:28.217' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1094, 1, CAST(N'2025-04-09T13:15:03.130' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1095, 1, CAST(N'2025-04-09T13:24:55.513' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1096, 1, CAST(N'2025-04-09T13:48:20.490' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1097, 1, CAST(N'2025-04-09T14:08:11.970' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1098, 1, CAST(N'2025-04-09T14:12:20.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1099, 1, CAST(N'2025-04-09T14:23:11.600' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1100, 1, CAST(N'2025-04-09T14:23:28.990' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1101, 1, CAST(N'2025-04-09T19:50:25.443' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1102, 1, CAST(N'2025-04-09T20:09:14.530' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1103, 1, CAST(N'2025-04-09T20:10:53.940' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1104, 1, CAST(N'2025-04-09T20:29:37.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1105, 1, CAST(N'2025-04-09T20:42:46.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1106, 1, CAST(N'2025-04-09T20:46:53.260' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1107, 1, CAST(N'2025-04-09T20:47:58.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1108, 1, CAST(N'2025-04-09T20:51:48.983' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1109, 1, CAST(N'2025-04-09T21:03:48.183' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1110, 1, CAST(N'2025-04-09T21:08:28.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1111, 1, CAST(N'2025-04-09T21:21:04.577' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1112, 1, CAST(N'2025-04-09T21:24:18.027' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1113, 1, CAST(N'2025-04-09T21:50:23.877' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1114, 1, CAST(N'2025-04-09T21:54:36.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1115, 1, CAST(N'2025-04-09T22:30:56.287' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1116, 1, CAST(N'2025-04-09T22:45:35.683' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1117, 1, CAST(N'2025-04-09T22:51:00.340' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1118, 1, CAST(N'2025-04-12T11:48:01.710' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1119, 1, CAST(N'2025-04-12T12:17:03.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1120, 1, CAST(N'2025-04-12T12:36:02.153' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1121, 1, CAST(N'2025-04-12T12:37:19.120' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1122, 1, CAST(N'2025-04-12T12:50:25.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1123, 1, CAST(N'2025-04-12T13:04:35.140' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1124, 1, CAST(N'2025-04-12T13:17:01.763' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1125, 1, CAST(N'2025-04-12T13:22:04.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1126, 1, CAST(N'2025-04-12T13:24:27.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1127, 1, CAST(N'2025-04-12T13:31:49.570' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1128, 1, CAST(N'2025-04-12T13:53:59.260' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1129, 1, CAST(N'2025-04-12T13:56:48.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1130, 1, CAST(N'2025-04-12T13:57:32.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1131, 1, CAST(N'2025-04-12T13:58:40.093' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1132, 1, CAST(N'2025-04-12T14:04:08.173' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1133, 1, CAST(N'2025-04-12T14:04:47.347' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1134, 1, CAST(N'2025-04-12T14:15:02.870' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1135, 1, CAST(N'2025-04-12T14:26:52.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1136, 1, CAST(N'2025-04-12T14:39:36.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1137, 1, CAST(N'2025-04-12T14:54:12.697' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1138, 1, CAST(N'2025-04-12T15:00:24.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1139, 1, CAST(N'2025-04-12T15:08:04.447' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1140, 1, CAST(N'2025-04-12T15:10:50.280' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1141, 1, CAST(N'2025-04-12T15:12:02.463' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1142, 1, CAST(N'2025-04-12T15:12:52.983' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1143, 1, CAST(N'2025-04-12T15:16:41.780' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1144, 1, CAST(N'2025-04-12T15:19:07.390' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1145, 1, CAST(N'2025-04-12T15:24:01.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1146, 1, CAST(N'2025-04-12T15:27:13.897' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1147, 1, CAST(N'2025-04-12T15:30:47.523' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1148, 1, CAST(N'2025-04-12T15:50:14.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1149, 1, CAST(N'2025-04-12T16:08:04.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1150, 1, CAST(N'2025-04-12T16:31:16.747' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1151, 1, CAST(N'2025-04-12T17:06:02.407' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1152, 1, CAST(N'2025-04-12T17:08:25.537' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1153, 1, CAST(N'2025-04-12T17:11:23.760' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1154, 1, CAST(N'2025-04-12T17:16:48.350' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1155, 1, CAST(N'2025-04-12T17:21:52.630' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1156, 1, CAST(N'2025-04-12T17:33:32.340' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1157, 1, CAST(N'2025-04-12T17:36:51.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1158, 1, CAST(N'2025-04-12T17:48:36.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1159, 1, CAST(N'2025-04-12T17:55:47.023' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1160, 1, CAST(N'2025-04-12T18:14:52.800' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1161, 1, CAST(N'2025-04-12T18:54:33.150' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1162, 1, CAST(N'2025-04-12T19:51:29.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1163, 1, CAST(N'2025-04-12T19:56:38.447' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1164, 1, CAST(N'2025-04-12T20:09:25.093' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1165, 1, CAST(N'2025-04-12T21:06:19.010' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1166, 1, CAST(N'2025-04-13T11:39:49.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1167, 1, CAST(N'2025-04-13T12:22:23.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1168, 1, CAST(N'2025-04-13T12:32:27.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1169, 1, CAST(N'2025-04-13T12:38:47.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1170, 1, CAST(N'2025-04-13T12:43:38.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1171, 1, CAST(N'2025-04-13T12:43:38.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1172, 1, CAST(N'2025-04-13T12:55:02.820' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1173, 1, CAST(N'2025-04-13T13:11:06.127' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1174, 1, CAST(N'2025-04-13T13:18:33.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1175, 1, CAST(N'2025-04-13T14:53:07.920' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1176, 1, CAST(N'2025-04-13T15:53:07.937' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1177, 1, CAST(N'2025-04-13T16:18:33.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1178, 1, CAST(N'2025-04-13T16:19:57.333' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1179, 1, CAST(N'2025-04-13T16:36:05.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1180, 1, CAST(N'2025-04-13T16:46:24.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1181, 1, CAST(N'2025-04-13T17:16:43.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1182, 1, CAST(N'2025-04-13T17:27:46.563' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1183, 1, CAST(N'2025-04-13T17:31:20.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1184, 1, CAST(N'2025-04-13T17:44:24.577' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1185, 1, CAST(N'2025-04-13T18:02:45.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1186, 1, CAST(N'2025-04-13T18:11:53.093' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1187, 1, CAST(N'2025-04-13T18:16:28.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1188, 1, CAST(N'2025-04-13T18:28:56.197' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1189, 1, CAST(N'2025-04-13T18:32:44.873' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1190, 1, CAST(N'2025-04-13T18:37:08.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1191, 1, CAST(N'2025-04-13T18:47:05.410' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1192, 1, CAST(N'2025-04-13T18:57:39.400' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1193, 1, CAST(N'2025-04-13T19:11:31.887' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1194, 1, CAST(N'2025-04-13T19:19:55.797' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1195, 1, CAST(N'2025-04-13T19:24:52.357' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1196, 1, CAST(N'2025-04-13T19:40:06.230' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1197, 1, CAST(N'2025-04-13T19:58:49.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1198, 1, CAST(N'2025-04-13T20:39:47.453' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1199, 1, CAST(N'2025-04-13T21:05:09.417' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1200, 1, CAST(N'2025-04-13T21:12:11.360' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1201, 1, CAST(N'2025-04-13T21:25:53.250' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1202, 1, CAST(N'2025-04-13T22:20:34.850' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1203, 1, CAST(N'2025-04-13T22:26:50.740' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1204, 1, CAST(N'2025-04-13T22:55:20.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1205, 1, CAST(N'2025-04-13T23:14:48.100' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1206, 1, CAST(N'2025-04-13T23:52:05.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1207, 1, CAST(N'2025-04-13T23:54:02.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1208, 1, CAST(N'2025-04-14T00:10:23.060' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1209, 1, CAST(N'2025-04-14T00:13:40.390' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1210, 1, CAST(N'2025-04-14T00:26:42.363' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1211, 1, CAST(N'2025-04-14T00:30:03.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1212, 1, CAST(N'2025-04-14T00:37:05.073' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1213, 1, CAST(N'2025-04-14T00:40:08.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1214, 1, CAST(N'2025-04-14T00:47:25.457' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1215, 1, CAST(N'2025-04-14T01:06:12.470' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1216, 1, CAST(N'2025-04-14T01:21:12.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1217, 1, CAST(N'2025-04-14T09:21:38.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1218, 1, CAST(N'2025-04-15T15:01:19.743' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1219, 1, CAST(N'2025-04-15T15:12:51.217' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1220, 1, CAST(N'2025-04-15T15:34:47.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1221, 1, CAST(N'2025-04-15T15:43:49.487' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1222, 1, CAST(N'2025-04-15T15:49:18.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1223, 1, CAST(N'2025-04-15T20:57:21.387' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1224, 1, CAST(N'2025-04-15T21:24:50.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1225, 1, CAST(N'2025-04-15T21:30:18.403' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1226, 1, CAST(N'2025-04-15T21:32:46.660' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1227, 1, CAST(N'2025-04-15T21:41:25.720' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1228, 1, CAST(N'2025-04-15T21:51:55.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1229, 1, CAST(N'2025-04-15T22:03:45.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1230, 1, CAST(N'2025-04-15T22:41:47.687' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1231, 1, CAST(N'2025-04-16T19:18:34.113' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1232, 1, CAST(N'2025-04-25T11:41:33.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1233, 1, CAST(N'2025-04-25T12:00:53.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1234, 1, CAST(N'2025-04-25T14:41:36.200' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1235, 1, CAST(N'2025-04-29T21:56:10.133' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1236, 1, CAST(N'2025-04-29T21:59:04.150' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1237, 1, CAST(N'2025-04-29T22:36:13.843' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1238, 1, CAST(N'2025-05-01T20:26:11.583' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1239, 1, CAST(N'2025-05-01T21:14:01.597' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1240, 2, CAST(N'2025-05-04T11:37:51.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1241, 2, CAST(N'2025-05-04T11:55:15.480' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1242, 2, CAST(N'2025-05-04T12:08:57.393' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1243, 2, CAST(N'2025-05-05T09:40:53.187' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1244, 2, CAST(N'2025-05-05T09:42:53.870' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1245, 2, CAST(N'2025-05-05T09:44:30.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1246, 2, CAST(N'2025-05-05T10:04:23.323' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1247, 2, CAST(N'2025-05-05T10:13:14.743' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1248, 2, CAST(N'2025-05-05T10:17:30.100' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1249, 2, CAST(N'2025-05-05T10:20:48.450' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1250, 2, CAST(N'2025-05-05T10:23:25.537' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1251, 2, CAST(N'2025-05-05T10:24:27.587' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1252, 2, CAST(N'2025-05-05T10:54:17.677' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1253, 2, CAST(N'2025-05-05T10:55:45.620' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1254, 2, CAST(N'2025-05-05T10:56:59.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1255, 2, CAST(N'2025-05-05T11:00:58.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1256, 2, CAST(N'2025-05-05T11:02:20.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1257, 2, CAST(N'2025-05-05T11:45:54.257' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1258, 2, CAST(N'2025-05-05T11:50:50.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1259, 2, CAST(N'2025-05-05T11:51:33.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1260, 2, CAST(N'2025-05-05T11:52:41.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1261, 2, CAST(N'2025-05-05T11:54:06.687' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1262, 2, CAST(N'2025-05-05T11:54:39.003' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1263, 2, CAST(N'2025-05-05T11:55:44.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1264, 2, CAST(N'2025-05-05T11:57:24.493' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1265, 2, CAST(N'2025-05-05T12:15:17.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1266, 2, CAST(N'2025-05-05T12:24:14.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1267, 2, CAST(N'2025-05-05T12:28:40.857' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1268, 2, CAST(N'2025-05-05T12:38:25.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1269, 2, CAST(N'2025-05-05T12:45:33.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1270, 2, CAST(N'2025-05-05T13:00:52.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1271, 2, CAST(N'2025-05-05T13:03:55.840' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1272, 2, CAST(N'2025-05-05T13:10:02.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1273, 2, CAST(N'2025-05-14T10:54:36.253' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1274, 2, CAST(N'2025-05-14T11:34:58.253' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1275, 2, CAST(N'2025-05-14T11:37:44.670' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1276, 2, CAST(N'2025-05-14T11:39:47.233' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1277, 2, CAST(N'2025-05-14T11:41:59.973' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1278, 2, CAST(N'2025-05-14T11:43:53.460' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1279, 2, CAST(N'2025-05-14T11:52:44.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1280, 2, CAST(N'2025-05-14T11:56:20.960' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1281, 2, CAST(N'2025-05-14T11:58:24.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1282, 2, CAST(N'2025-05-14T12:35:29.313' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1283, 2, CAST(N'2025-05-14T12:37:32.340' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1284, 2, CAST(N'2025-05-14T12:40:02.057' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1285, 2, CAST(N'2025-05-14T12:46:28.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1286, 2, CAST(N'2025-05-14T12:54:28.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1287, 2, CAST(N'2025-05-14T12:56:01.160' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1288, 2, CAST(N'2025-05-14T13:00:18.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1289, 2, CAST(N'2025-05-14T13:04:35.503' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1290, 2, CAST(N'2025-05-14T13:14:59.227' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1291, 2, CAST(N'2025-05-14T13:29:29.147' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1292, 2, CAST(N'2025-05-14T14:45:38.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1293, 2, CAST(N'2025-05-14T14:46:58.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1294, 2, CAST(N'2025-05-14T14:52:27.947' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1295, 2, CAST(N'2025-05-14T15:13:53.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1296, 2, CAST(N'2025-05-16T11:52:18.647' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1297, 1, CAST(N'2025-05-19T15:27:24.203' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1298, 1, CAST(N'2025-05-19T15:29:43.923' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1299, 1, CAST(N'2025-05-19T15:39:56.767' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1300, 1, CAST(N'2025-05-19T15:40:12.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1301, 1, CAST(N'2025-05-19T19:34:17.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1302, 1, CAST(N'2025-05-19T19:45:15.747' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1303, 1, CAST(N'2025-05-19T19:50:53.380' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1304, 1, CAST(N'2025-05-19T19:55:50.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1305, 1, CAST(N'2025-05-19T20:26:08.033' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1306, 1, CAST(N'2025-05-19T20:27:19.477' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1307, 1, CAST(N'2025-05-19T20:49:26.060' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1308, 1, CAST(N'2025-05-19T20:52:49.403' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1309, 1, CAST(N'2025-05-19T20:54:44.680' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1310, 1, CAST(N'2025-05-19T21:34:42.867' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1311, 1, CAST(N'2025-05-19T21:48:57.327' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1312, 1, CAST(N'2025-05-19T21:53:50.810' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1313, 1, CAST(N'2025-05-19T22:40:39.067' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1314, 1, CAST(N'2025-05-20T11:54:40.917' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1315, 1, CAST(N'2025-05-20T13:38:27.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1316, 1, CAST(N'2025-05-20T13:44:07.130' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1317, 1, CAST(N'2025-05-20T14:00:38.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1318, 1, CAST(N'2025-05-20T14:11:04.667' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1319, 1, CAST(N'2025-05-20T14:16:10.047' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1320, 1, CAST(N'2025-05-20T14:20:50.607' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1321, 1, CAST(N'2025-05-20T14:36:48.767' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1322, 1, CAST(N'2025-05-20T14:55:59.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1323, 1, CAST(N'2025-05-20T15:00:17.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1324, 1, CAST(N'2025-05-20T17:51:57.450' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1325, 1, CAST(N'2025-05-20T18:00:10.467' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1326, 1, CAST(N'2025-05-20T18:01:51.893' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1327, 1, CAST(N'2025-05-20T18:13:23.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1328, 1, CAST(N'2025-05-20T18:22:29.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1329, 1, CAST(N'2025-05-20T18:25:39.213' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1330, 1, CAST(N'2025-05-20T18:49:44.340' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1331, 1, CAST(N'2025-05-20T18:56:15.967' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1332, 1, CAST(N'2025-05-20T18:59:45.043' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1333, 1, CAST(N'2025-05-20T19:03:00.433' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1334, 1, CAST(N'2025-05-20T19:25:33.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1335, 1, CAST(N'2025-05-20T19:32:11.523' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1336, 1, CAST(N'2025-05-20T19:41:55.823' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1337, 1, CAST(N'2025-05-20T19:45:35.413' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1338, 1, CAST(N'2025-05-20T19:45:38.170' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1339, 1, CAST(N'2025-05-20T19:58:49.563' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1340, 1, CAST(N'2025-05-20T20:29:53.297' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1341, 1, CAST(N'2025-05-20T20:53:37.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1342, 1, CAST(N'2025-05-20T20:53:40.900' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1343, 1, CAST(N'2025-05-20T21:00:59.723' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1344, 1, CAST(N'2025-05-20T21:07:50.050' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1345, 1, CAST(N'2025-05-20T21:14:11.643' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1346, 1, CAST(N'2025-05-20T21:23:37.053' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1347, 1, CAST(N'2025-05-20T21:26:20.383' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1348, 1, CAST(N'2025-05-20T21:35:20.347' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1349, 1, CAST(N'2025-05-20T21:51:32.617' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1350, 1, CAST(N'2025-05-20T21:59:11.067' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1351, 1, CAST(N'2025-05-21T09:43:32.790' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1352, 1, CAST(N'2025-05-21T10:49:22.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1353, 1, CAST(N'2025-05-21T11:47:49.380' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1354, 1, CAST(N'2025-05-21T11:54:20.997' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1355, 1, CAST(N'2025-05-21T11:56:10.843' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1356, 1, CAST(N'2025-05-21T11:58:00.600' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1357, 1, CAST(N'2025-05-21T11:59:04.703' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1358, 1, CAST(N'2025-05-21T12:02:08.770' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1359, 1, CAST(N'2025-05-21T12:05:23.377' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1360, 1, CAST(N'2025-05-21T12:19:28.423' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1361, 1, CAST(N'2025-05-21T12:40:50.307' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1362, 1, CAST(N'2025-05-21T13:04:29.957' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1363, 1, CAST(N'2025-05-21T13:23:51.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1364, 1, CAST(N'2025-05-21T13:27:59.543' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1365, 1, CAST(N'2025-05-21T13:31:57.727' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1366, 1, CAST(N'2025-05-21T13:37:25.447' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1367, 1, CAST(N'2025-05-21T13:38:44.537' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1368, 1, CAST(N'2025-05-21T13:44:58.650' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1369, 1, CAST(N'2025-05-21T13:46:03.220' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1370, 1, CAST(N'2025-05-21T13:46:55.560' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1371, 1, CAST(N'2025-05-21T13:59:51.807' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1372, 1, CAST(N'2025-05-21T14:10:45.013' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1373, 1, CAST(N'2025-05-21T14:14:05.707' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1374, 1, CAST(N'2025-05-21T14:19:19.353' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1375, 1, CAST(N'2025-05-21T14:21:43.483' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1376, 1, CAST(N'2025-05-21T14:23:15.673' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1377, 1, CAST(N'2025-05-21T14:35:34.567' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1378, 1, CAST(N'2025-05-21T15:03:45.837' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1379, 1, CAST(N'2025-05-21T15:04:14.990' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1380, 1, CAST(N'2025-05-21T15:12:29.367' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1381, 1, CAST(N'2025-05-21T15:18:42.257' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1382, 1, CAST(N'2025-05-21T15:23:57.320' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1383, 1, CAST(N'2025-05-21T15:26:49.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1384, 1, CAST(N'2025-05-21T16:50:24.720' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1385, 1, CAST(N'2025-05-21T17:24:55.637' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1386, 1, CAST(N'2025-05-21T17:25:32.887' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1387, 1, CAST(N'2025-05-21T17:25:51.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1388, 1, CAST(N'2025-05-21T17:33:08.733' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1389, 1, CAST(N'2025-05-21T17:59:26.310' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1390, 1, CAST(N'2025-05-21T18:03:40.000' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1391, 1, CAST(N'2025-05-21T18:04:16.963' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1392, 1, CAST(N'2025-05-21T18:05:10.613' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1393, 1, CAST(N'2025-05-21T18:11:01.773' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1394, 1, CAST(N'2025-05-21T18:17:17.237' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1395, 1, CAST(N'2025-05-21T18:29:50.157' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1396, 1, CAST(N'2025-05-21T18:31:11.473' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1397, 1, CAST(N'2025-05-21T18:35:33.077' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1398, 1, CAST(N'2025-05-21T18:37:09.830' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1399, 1, CAST(N'2025-05-21T18:38:10.077' AS DateTime))
GO
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1400, 1, CAST(N'2025-05-21T18:40:55.017' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1401, 1, CAST(N'2025-05-21T18:42:47.953' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1402, 1, CAST(N'2025-05-21T18:43:47.103' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1403, 1, CAST(N'2025-05-21T18:46:16.713' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1404, 1, CAST(N'2025-05-21T19:52:24.977' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1405, 1, CAST(N'2025-05-21T20:10:46.913' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1406, 1, CAST(N'2025-05-21T21:09:41.520' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1407, 1, CAST(N'2025-05-21T22:18:49.937' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1408, 1, CAST(N'2025-05-21T22:38:35.923' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1409, 1, CAST(N'2025-05-21T23:28:41.370' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1410, 1, CAST(N'2025-05-21T23:33:58.060' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1411, 1, CAST(N'2025-05-21T23:35:25.470' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1412, 1, CAST(N'2025-05-21T23:39:50.030' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1413, 1, CAST(N'2025-05-22T15:12:33.690' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1414, 2, CAST(N'2025-05-23T10:30:50.540' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1415, 2, CAST(N'2025-05-23T12:04:07.587' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1416, 2, CAST(N'2025-05-23T12:09:15.337' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1417, 2, CAST(N'2025-05-23T14:48:44.803' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1418, 2, CAST(N'2025-05-23T14:54:33.177' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1419, 2, CAST(N'2025-05-23T15:11:17.357' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1420, 2, CAST(N'2025-05-23T16:18:56.140' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1421, 2, CAST(N'2025-05-23T16:37:45.637' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1422, 1, CAST(N'2025-05-23T16:39:57.420' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1423, 2, CAST(N'2025-05-23T16:40:07.117' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1424, 2, CAST(N'2025-05-23T16:42:56.097' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1425, 1, CAST(N'2025-05-23T16:43:47.550' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1426, 2, CAST(N'2025-05-23T16:44:02.007' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1427, 1, CAST(N'2025-05-23T16:44:24.547' AS DateTime))
INSERT [dbo].[foUserLogin] ([ID], [UserID], [LoginDateTime]) VALUES (1428, 2, CAST(N'2025-05-23T16:44:47.520' AS DateTime))
SET IDENTITY_INSERT [dbo].[foUserLogin] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserPasswordReset] ON 

INSERT [dbo].[foUserPasswordReset] ([ID], [UserID], [ResetwithNextLogin]) VALUES (1, 1, 0)
SET IDENTITY_INSERT [dbo].[foUserPasswordReset] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserProcess] ON 

INSERT [dbo].[foUserProcess] ([ID], [UserID], [ProcessID]) VALUES (2, 2, 1)
INSERT [dbo].[foUserProcess] ([ID], [UserID], [ProcessID]) VALUES (3, 1, 1)
INSERT [dbo].[foUserProcess] ([ID], [UserID], [ProcessID]) VALUES (4, 1, 3)
SET IDENTITY_INSERT [dbo].[foUserProcess] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserReports] ON 

INSERT [dbo].[foUserReports] ([ID], [UserID], [ReportID], [Active]) VALUES (15, 1, 1, 1)
INSERT [dbo].[foUserReports] ([ID], [UserID], [ReportID], [Active]) VALUES (16, 2, 2, 1)
INSERT [dbo].[foUserReports] ([ID], [UserID], [ReportID], [Active]) VALUES (17, 2, 1, 1)
INSERT [dbo].[foUserReports] ([ID], [UserID], [ReportID], [Active]) VALUES (18, 1, 2, 1)
INSERT [dbo].[foUserReports] ([ID], [UserID], [ReportID], [Active]) VALUES (20, 2, 5, 1)
SET IDENTITY_INSERT [dbo].[foUserReports] OFF
GO
SET IDENTITY_INSERT [dbo].[foUsers] ON 

INSERT [dbo].[foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Admin], [Active]) VALUES (1, N'schalkvdm', N'111', N'Schalk', N'van der Merwe', N'schalk83.vandermerwe@gmail.com', 1, 1)
INSERT [dbo].[foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Admin], [Active]) VALUES (2, N'freedomn', N'freedomn', N'Freedom', N'Nxumalo', N'', 1, 1)
INSERT [dbo].[foUsers] ([ID], [UserName], [Password], [FirstName], [LastName], [Email], [Admin], [Active]) VALUES (3, N'moniquea', N'123', N'Monique', N'Abrahams', N'schalk.vandermerwe@digioutsource.com', 1, 0)
SET IDENTITY_INSERT [dbo].[foUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[foUserTable] ON 

INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (1, 1, N'tbl_md_Campus', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (2, 1, N'tbl_md_Province', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (3, 1, N'tbl_md_HomeType', N'R', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (11, 2, N'tbl_md_Campus', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (12, 2, N'tbl_md_HomeType', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (13, 2, N'tbl_md_Province', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (14, 2, N'tbl_tran_Device', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (15, 2, N'tbl_tran_Home', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (16, 2, N'tbl_tran_Home_Person', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (17, 2, N'tbl_tran_Home_Registration', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (18, 2, N'tbl_tran_Person', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (19, 2, N'tbl_tran_Student', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (20, 2, N'tbl_tran_StudentDetails1', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (21, 2, N'tbl_tran_studentdetails1_ExtraInformation', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (22, 2, N'tbl_tran_studentdetails1_ExtraInformation_no2', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (23, 2, N'tbl_tran_StudentDetails2', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (24, 2, N'tbl_tran_StudentDetails3', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (25, 1, N'tbl_tran_StudentDetails1', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (26, 1, N'tbl_tran_StudentDetails2', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (27, 1, N'tbl_tran_studentdetails1_ExtraInformation', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (29, 1, N'tbl_tran_Student', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (32, 1, N'tbl_tran_Asset', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (33, 1, N'tbl_tran_AssetExpiry', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (34, 2, N'tbl_tran_Country', N'R', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (35, 2, N'tbl_md_AssetType', N'RW', 1)
INSERT [dbo].[foUserTable] ([ID], [UserID], [TableName], [ReadWriteAccess], [Active]) VALUES (36, 2, N'tbl_tran_Asset', N'R', 1)
SET IDENTITY_INSERT [dbo].[foUserTable] OFF
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
