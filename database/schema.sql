USE [Quiz]
GO


CREATE TABLE [dbo].[Questions](
	[ID] [uniqueidentifier] NOT NULL,
	[QuestionContent] [nvarchar](max) NOT NULL,
	[Points] [int] NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[SelectionMultiplicity] [varchar](10) NOT NULL,
	[Status] [varchar](50) NOT NULL DEFAULT ('InPreparation'),
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

CREATE TABLE [dbo].[Tests](
	[ID] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](200) NOT NULL,
	[Started] [datetime], 
	[Completed] [datetime],
	[MaxPoints] [float] NULL,
	[GainedPoints] [float] NULL, 
 CONSTRAINT [PK_Tests] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
))

CREATE TABLE [dbo].[TestQuestions](
	[ID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[TestID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_TestQuestions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
))

ALTER TABLE [dbo].[TestQuestions]  WITH CHECK ADD  CONSTRAINT [FK_TestQuestions_Tests] FOREIGN KEY([TestID])
REFERENCES [dbo].[Tests] ([ID])
GO

ALTER TABLE [dbo].[TestQuestions]  WITH CHECK ADD  CONSTRAINT [FK_TestQuestions_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([ID])
GO


CREATE TABLE [dbo].[TestAnswers](
	[ID] [uniqueidentifier] NOT NULL,
	[AnswerID] [uniqueidentifier] NOT NULL,
	[TestQuestionsID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_TestAnswers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
))

ALTER TABLE [dbo].[TestAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TestAnswers_TestQuestions] FOREIGN KEY([TestQuestionsID])
REFERENCES [dbo].[TestQuestions] ([ID])
GO

ALTER TABLE [dbo].[TestAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TestAnswers_Answers] FOREIGN KEY([AnswerID])
REFERENCES [dbo].[Answers] ([ID])
GO
