CREATE OR ALTER FUNCTION GenerateNextTaskId
(
	@ProjectId CHAR(8)
)
RETURNS CHAR(13)
BEGIN
	DECLARE @Tasks INT;
	DECLARE @NextTaskId CHAR(13);
	SELECT @Tasks = ([Tasks]+1) FROM [ProjectSequence] WHERE [ProjectId] = @ProjectId;
	SET @NextTaskId = CONCAT_WS('-', @ProjectId, RIGHT(REPLICATE('0', 4) + CAST(@Tasks AS VARCHAR), 4));
	RETURN @NextTaskId;
END;

GO

CREATE OR ALTER PROCEDURE CreateTask
	@TaskTitle VARCHAR(50),
	@TaskDescription VARCHAR(MAX),
	@StartDate DATE,
	@EndDate DATE,
	@ProjectId CHAR(8),
	@Users dbo.[UserIdList] READONLY,
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @NextTaskId CHAR(13);
	DECLARE @NextBatchNo VARCHAR(20);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				SET @NextTaskId = dbo.GenerateNextTaskId(@ProjectId);
				INSERT INTO [TaskShadow] VALUES (@NextTaskId, @TaskTitle, @TaskDescription, @StartDate, @EndDate, 'CCL-1', @ProjectId, @NextBatchNo, @NextBatchNo, 'INSERT', GETDATE(), @ActionUserId);
				INSERT INTO [Task] VALUES (@NextTaskId, @TaskTitle, @TaskDescription, @StartDate, @EndDate, 'CCL-1', @ProjectId, @NextBatchNo, @NextBatchNo);
				UPDATE [ProjectSequence] SET [Tasks] = [Tasks] + 1 WHERE [ProjectId] = @ProjectId;
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
				BEGIN
					DECLARE @UserId CHAR(8);
					DECLARE UsersCursor CURSOR LOCAL FOR SELECT [UserId] FROM @Users;
					OPEN UsersCursor;
					FETCH NEXT FROM UsersCursor INTO @UserId;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [UserTaskShadow] VALUES (@UserId, @NextTaskId, @NextBatchNo, @NextBatchNo, 'INSERT', GETDATE(), @ActionUserId);
							INSERT INTO [UserTask] VALUES (@UserId, @NextTaskId, @NextBatchNo, @NextBatchNo);
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM UsersCursor INTO @UserId;
						END;
					CLOSE UsersCursor;
					DEALLOCATE UsersCursor;
				END;
			COMMIT;
		END TRY
		BEGIN CATCH
			IF CURSOR_STATUS('local','UsersCursor') >= 0 
			BEGIN
				CLOSE UsersCursor;
				DEALLOCATE UsersCursor;
			END;
			ROLLBACK;
			THROW 50008, 'Task creation failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE UpdateTask
	@TaskTitle VARCHAR(50),
	@TaskDescription VARCHAR(MAX),
	@StartDate DATE,
	@EndDate DATE,
	@TaskStatusId CHAR(5),
	@InsertedUsers dbo.[UserIdList] READONLY,
	@DeletedUsers dbo.[UserIdList] READONLY,
	@TaskId CHAR(13),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);
	DECLARE @ProjectId CHAR(8);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SELECT @ProjectId = [ProjectId], @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [Task] WHERE [TaskId] = @TaskId;
				IF @CurrentBatchNo <> @ExistingBatchNo
					THROW 50000, 'Record version conflict.', 3;
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				INSERT INTO [TaskShadow] VALUES (@TaskId, @TaskTitle, @TaskDescription, @StartDate, @EndDate, @TaskStatusId, @ProjectId, @OriginalBatchNo, @NextBatchNo, 'UPDATE', GETDATE(), @ActionUserId);
				UPDATE [Task] SET [TaskTitle] = @TaskTitle, [TaskDescription] = @TaskDescription, [StartDate] = @StartDate, [EndDate] = @EndDate, [TaskStatusId] = @TaskStatusId, [CurrentBatchNo] = @NextBatchNo WHERE [TaskId] = @TaskId;
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
				BEGIN
					DECLARE @DeletedUserId CHAR(8);
					DECLARE DeletedUsersCursor CURSOR LOCAL FOR SELECT [UserId] FROM @DeletedUsers;
					OPEN DeletedUsersCursor;
					DECLARE @UserTaskOriginalBatchNo VARCHAR(20);
					FETCH NEXT FROM DeletedUsersCursor INTO @DeletedUserId;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SELECT @UserTaskOriginalBatchNo = [OriginalBatchNo] FROM [UserTask] WHERE [UserId] = @DeletedUserId AND [TaskId] = @TaskId;
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [UserTaskShadow] VALUES (@DeletedUserId, @TaskId, @UserTaskOriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
							DELETE FROM [UserTask] WHERE [UserId] = @DeletedUserId AND [TaskId] = @TaskId;
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM DeletedUsersCursor INTO @DeletedUserId;
						END;
					CLOSE DeletedUsersCursor;
					DEALLOCATE DeletedUsersCursor;
				END;
				BEGIN
					DECLARE @InsertedUserId CHAR(8);
					DECLARE InsertedUsersCursor CURSOR LOCAL FOR SELECT [UserId] FROM @InsertedUsers;
					OPEN InsertedUsersCursor;
					FETCH NEXT FROM InsertedUsersCursor INTO @InsertedUserId;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [UserTaskShadow] VALUES (@InsertedUserId, @TaskId, @NextBatchNo, @NextBatchNo, 'INSERT', GETDATE(), @ActionUserId);
							INSERT INTO [UserTask] VALUES (@InsertedUserId, @TaskId, @NextBatchNo, @NextBatchNo);
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM InsertedUsersCursor INTO @InsertedUserId;
						END;
					CLOSE InsertedUsersCursor;
					DEALLOCATE InsertedUsersCursor;
				END;
			COMMIT;
		END TRY
		BEGIN CATCH
			IF CURSOR_STATUS('local','InsertedUsersCursor') >= 0 
			BEGIN
				CLOSE InsertedUsersCursor;
				DEALLOCATE InsertedUsersCursor;
			END;
			IF CURSOR_STATUS('local','DeletedUsersCursor') >= 0 
			BEGIN
				CLOSE DeletedUsersCursor;
				DEALLOCATE DeletedUsersCursor;
			END;
			ROLLBACK;
			THROW 50009, 'Task updating failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE DeleteTask
	@TaskId CHAR(13),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);
	DECLARE @ProjectId CHAR(8);
	DECLARE	@TaskTitle VARCHAR(50);
	DECLARE @TaskDescription VARCHAR(MAX);
	DECLARE @StartDate DATE;
	DECLARE @EndDate DATE;
	DECLARE @TaskStatusId CHAR(5);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SELECT @TaskTitle = [TaskTitle], @TaskDescription = [TaskDescription], @StartDate = [StartDate], @EndDate = [EndDate], @TaskStatusId = [TaskStatusId], @ProjectId = [ProjectId], @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [Task] WHERE [TaskId] = @TaskId;
				IF @CurrentBatchNo <> @ExistingBatchNo
					THROW 50000, 'Record version conflict.', 3;
				BEGIN
					DECLARE @DeletedUserId CHAR(8);
					DECLARE DeletedUsersCursor CURSOR LOCAL FOR SELECT [UserId], [OriginalBatchNo] FROM [UserTask] WHERE [TaskId] = @TaskId;
					OPEN DeletedUsersCursor;
					DECLARE @UserTaskOriginalBatchNo VARCHAR(20);
					FETCH NEXT FROM DeletedUsersCursor INTO @DeletedUserId, @UserTaskOriginalBatchNo;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [UserTaskShadow] VALUES (@DeletedUserId, @TaskId, @UserTaskOriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
							DELETE FROM [UserTask] WHERE [UserId] = @DeletedUserId AND [TaskId] = @TaskId;
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM DeletedUsersCursor INTO @DeletedUserId, @UserTaskOriginalBatchNo;
						END;
					CLOSE DeletedUsersCursor;
					DEALLOCATE DeletedUsersCursor;
				END;
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				INSERT INTO [TaskShadow] VALUES (@TaskId, @TaskTitle, @TaskDescription, @StartDate, @EndDate, @TaskStatusId, @ProjectId, @OriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
				DELETE FROM [Task] WHERE [TaskId] = @TaskId;
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
			COMMIT;
		END TRY
		BEGIN CATCH
			IF CURSOR_STATUS('local','DeletedUsersCursor') >= 0 
			BEGIN
				CLOSE DeletedUsersCursor;
				DEALLOCATE DeletedUsersCursor;
			END;
			ROLLBACK;
			THROW 50010, 'Task deleting failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE ReadAllTasks
AS
BEGIN
	SELECT * FROM [TasksView];
END;

GO

CREATE OR ALTER PROCEDURE ReadTaskById
	@TaskId CHAR(13)
AS
BEGIN
	SELECT * FROM [TasksView] WHERE [TaskId] = @TaskId;
END;

GO

CREATE OR ALTER PROCEDURE ReadTaskShadowByCurrentBatchNo
	@CurrentBatchNo VARCHAR(20)
AS
BEGIN
	SELECT * FROM [TaskShadow] WHERE [CurrentBatchNo] = @CurrentBatchNo;
END;

GO

CREATE OR ALTER PROCEDURE ReadUserTasksByTaskId
	@TaskId CHAR(13)
AS
BEGIN
	SELECT * FROM [UserTasksView] WHERE [TaskId] = @TaskId;
END;

GO

CREATE OR ALTER PROCEDURE ReadTasksByProjectId
	@ProjectId CHAR(8)
AS
BEGIN
	SELECT * FROM [TasksView] WHERE [ProjectId] = @ProjectId;
END;

GO

CREATE OR ALTER PROCEDURE ReadAllTaskStatuses
AS
BEGIN
	SELECT * FROM [TaskStatus];
END;

GO