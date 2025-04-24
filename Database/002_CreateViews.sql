USE [pmsdb];

DROP VIEW IF EXISTS [UsersView];

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