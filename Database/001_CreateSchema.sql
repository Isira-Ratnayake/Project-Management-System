USE [pmsdb];

DROP TABLE IF EXISTS [UserTask];
DROP TABLE IF EXISTS [TaskShadow];
DROP TABLE IF EXISTS [Task];
DROP TABLE IF EXISTS [TaskStatus];
DROP TABLE IF EXISTS [ProjectSequence];
DROP TABLE IF EXISTS [Project];
DROP TABLE IF EXISTS [UserShadow];
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS [UserGroup];
DROP TABLE IF EXISTS [CompanySequence];
DROP TABLE IF EXISTS [Company];

CREATE TABLE [Company] (
	[CompanyId] CHAR(3) NOT NULL PRIMARY KEY,
	[CompanyName] VARCHAR(100) NOT NULL 
);

CREATE TABLE [CompanySequence] (
	[CompanyId] CHAR(3) NOT NULL PRIMARY KEY,
	[Batches] INT NOT NULL DEFAULT 0,
	[Users] INT NOT NULL DEFAULT 0,
	FOREIGN KEY ([CompanyId]) REFERENCES [Company]([CompanyId])
);

CREATE TABLE [Project] (
	[ProjectId] CHAR(5) NOT NULL PRIMARY KEY,
	[ProjectName] VARCHAR(100) NOT NULL,
	[CompanyId] CHAR(3) NOT NULL,
	FOREIGN KEY ([CompanyId]) REFERENCES [Company]([CompanyId])
);

CREATE TABLE [ProjectSequence] (
	[ProjectId] CHAR(5) NOT NULL PRIMARY KEY,
	[Tasks] INT NOT NULL DEFAULT 0,
	FOREIGN KEY ([ProjectId]) REFERENCES [Project]([ProjectId])
);

CREATE TABLE [UserGroup] (
	[UserGroupId] CHAR(5) NOT NULL PRIMARY KEY,
	[UserGroupName] VARCHAR(100) NOT NULL,
	[CompanyId] CHAR(3) NOT NULL,
	FOREIGN KEY ([CompanyId]) REFERENCES [Company]([CompanyId])
);

CREATE TABLE [User] (
	[UserId] CHAR(8) NOT NULL PRIMARY KEY,
	[UserEmail] VARCHAR(100) NOT NULL,
	[UserPassword] VARCHAR(300) NOT NULL,
	[UserFullname] VARCHAR(100) NOT NULL,
	[UserGroupId] CHAR(5) NOT NULL,
	[OriginalBatchNo] VARCHAR(20) NOT NULL,
	[CurrentBatchNo] VARCHAR(20) NOT NULL,
	FOREIGN KEY ([UserGroupId]) REFERENCES [UserGroup]([UserGroupId])
);

CREATE TABLE [UserShadow] (
	[UserId] CHAR(8) NOT NULL,
	[UserEmail] VARCHAR(100) NOT NULL,
	[UserPassword] VARCHAR(300) NOT NULL,
	[UserFullname] VARCHAR(100) NOT NULL,
	[UserGroupId] CHAR(5) NOT NULL,
	[OriginalBatchNo] VARCHAR(20) NOT NULL,
	[CurrentBatchNo] VARCHAR(20) NOT NULL PRIMARY KEY,
	[Action] VARCHAR(30) NOT NULL,
	[ActionDateTime] DATETIME NOT NULL,
	[ActionUserId] CHAR(8) NOT NULL
);

CREATE TABLE [TaskStatus] (
	[TaskStatusId] CHAR(5) NOT NULL PRIMARY KEY,
	[TaskStatusDescription] VARCHAR(50) NOT NULL,
	[CompanyId] CHAR(3) NOT NULL,
	FOREIGN KEY ([CompanyId]) REFERENCES [Company]([CompanyId])
);

CREATE TABLE [Task] (
	[TaskId] CHAR(10) NOT NULL PRIMARY KEY,
	[TaskTitle] VARCHAR(50) NOT NULL,
	[TaskDescription] TEXT NOT NULL,
	[StartDate] DATE NOT NULL,
	[EndDate] DATE NOT NULL,
	[TaskStatusId] CHAR(5) NOT NULL,
	[ProjectId] CHAR(5) NOT NULL,
	[OriginalBatchNo] VARCHAR(20) NOT NULL,
	[CurrentBatchNo] VARCHAR(20) NOT NULL,
	FOREIGN KEY ([TaskStatusId]) REFERENCES [TaskStatus]([TaskStatusId]),
	FOREIGN KEY ([ProjectId]) REFERENCES [Project]([ProjectId])
);

CREATE TABLE [TaskShadow] (
	[TaskId] CHAR(10) NOT NULL,
	[TaskTitle] VARCHAR(50) NOT NULL,
	[TaskDescription] TEXT NOT NULL,
	[StartDate] DATE NOT NULL,
	[EndDate] DATE NOT NULL,
	[TaskStatusId] CHAR(5) NOT NULL,
	[ProjectId] CHAR(5) NOT NULL,
	[OriginalBatchNo] VARCHAR(20) NOT NULL,
	[CurrentBatchNo] VARCHAR(20) NOT NULL PRIMARY KEY,
	[Action] VARCHAR(30) NOT NULL,
	[ActionDateTime] DATETIME NOT NULL,
	[ActionUserId] CHAR(8) NOT NULL
);

CREATE TABLE [UserTask] (
	[UserId] CHAR(8) NOT NULL,
	[TaskId] CHAR(10) NOT NULL,
	PRIMARY KEY([UserId], [TaskId]),
	FOREIGN KEY([UserId]) REFERENCES [User]([UserId]),
	FOREIGN KEY([TaskId]) REFERENCES [Task]([TaskId])
);