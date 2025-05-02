USE [pmsdb];

DROP VIEW IF EXISTS [UsersView];
DROP VIEW IF EXISTS [TasksView];
DROP VIEW IF EXISTS [UserTasksView];

GO

CREATE VIEW [UsersView] AS
	SELECT 
		[UserId],
		[UserEmail],
		[UserPassword],
		[UserFullname],
		[User].[UserGroupId],
		[UserGroupName],
		[OriginalBatchNo],
		[CurrentBatchNo]
	FROM [User] INNER JOIN [UserGroup] ON [User].[UserGroupId] = [UserGroup].[UserGroupId];

GO

CREATE VIEW [TasksView] AS
	SELECT 
		[TaskId],
		[TaskTitle],
		[TaskDescription],
		[StartDate],
		[EndDate],
		[Task].[TaskStatusId],
		[TaskStatusDescription],
		[ProjectId],
		[OriginalBatchNo],
		[CurrentBatchNo]
	FROM [Task] INNER JOIN [TaskStatus] ON [Task].[TaskStatusId] = [TaskStatus].[TaskStatusId];

GO

CREATE VIEW [UserTasksView] AS
	SELECT 
		[TaskId],
		[UserTask].[UserId],
		[UserEmail],
		[UserFullname],
		[UserTask].[OriginalBatchNo],
		[UserTask].[CurrentBatchNo]
	FROM [UserTask] INNER JOIN [User] ON [UserTask].[UserId] = [User].[UserId];

GO