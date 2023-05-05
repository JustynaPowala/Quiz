USE [Quiz]
GO


CREATE TABLE [dbo].[Questions](
	[ID] [uniqueidentifier] NOT NULL,
	[QuestionContent] [nvarchar](max) NOT NULL,
	[Points] [int] NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[SelectionMultiplicity] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


CREATE TABLE [dbo].[Answers](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[AnswerContent] [nvarchar](max) NOT NULL,
	[IsCorrect] [bit] NOT NULL,
	[CreationTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Answers]  WITH CHECK ADD  CONSTRAINT [FK_Answers_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([ID])
GO

ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_Answers_Questions]
GO
