CREATE OR ALTER FUNCTION GenerateNextProjectId
(
	@CompanyId CHAR(3)
)
RETURNS CHAR(8)
BEGIN
	DECLARE @Projects INT;
	DECLARE @NextProjectId CHAR(8);
	SELECT @Projects = ([Projects]+1) FROM [CompanySequence] WHERE [CompanyId] = @CompanyId;
	SET @NextProjectId = CONCAT_WS('-', @CompanyId, RIGHT(REPLICATE('0', 4) + CAST(@Projects AS VARCHAR), 4));
	RETURN @NextProjectId;
END;

GO

CREATE OR ALTER PROCEDURE CreateProject
	@ProjectName VARCHAR(100),
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @NextProjectId CHAR(8);
	DECLARE @NextBatchNo VARCHAR(20);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				SET @NextProjectId = dbo.GenerateNextProjectId('CCL');
				INSERT INTO [ProjectShadow] VALUES (@NextProjectId, @ProjectName,'CCL', @NextBatchNo, @NextBatchNo, 'INSERT', GETDATE(), @ActionUserId);
				INSERT INTO [Project] VALUES (@NextProjectId, @ProjectName, 'CCL', @NextBatchNo, @NextBatchNo);
				INSERT INTO [ProjectSequence] VALUES (@NextProjectId, 0);
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1, [Projects] = [Projects] + 1 WHERE [CompanyId] = 'CCL';
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK;
			THROW 50005, 'Project creation failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE UpdateProject
	@ProjectName VARCHAR(100),
	@ProjectId CHAR(8),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);
	DECLARE @CompanyId CHAR(3);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SELECT @CompanyId = CompanyId, @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [Project] WHERE [ProjectId] = @ProjectId;
				IF @CurrentBatchNo <> @ExistingBatchNo
					THROW 50000, 'Record version conflict.', 3;
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');

				INSERT INTO [ProjectShadow] VALUES (@ProjectId, @ProjectName, @CompanyId, @OriginalBatchNo, @NextBatchNo, 'UPDATE', GETDATE(), @ActionUserId);
				UPDATE [Project] SET [ProjectName] = @ProjectName, [CurrentBatchNo] = @NextBatchNo WHERE [ProjectId] = @ProjectId;

				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK;
			THROW 50006, 'Project updating failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE DeleteProject
	@ProjectId CHAR(8),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(8)
AS
BEGIN
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);
	DECLARE @CompanyId CHAR(3);
	DECLARE	@ProjectName VARCHAR(100);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SELECT @ProjectName = [ProjectName], @CompanyId = CompanyId, @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [Project] WHERE [ProjectId] = @ProjectId;
				IF @CurrentBatchNo <> @ExistingBatchNo
					THROW 50000, 'Record version conflict.', 3;

				BEGIN
					DECLARE @TaskId_1 CHAR(13);
					DECLARE @UserId CHAR(8);
					DECLARE @UserTaskOriginalBatchNo VARCHAR(20);
					DECLARE UserTaskCursor CURSOR LOCAL FOR SELECT [UserTask].[TaskId], [UserTask].[UserId], [UserTask].[OriginalBatchNo] FROM [Task] INNER JOIN [UserTask] ON [Task].[TaskId] = [UserTask].[TaskId] WHERE [ProjectId] = @ProjectId;
					OPEN UserTaskCursor;
					FETCH NEXT FROM UserTaskCursor INTO @TaskId_1, @UserId, @UserTaskOriginalBatchNo;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [UserTaskShadow] VALUES (@UserId, @TaskId_1, @UserTaskOriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
							DELETE FROM [UserTask] WHERE [UserId] = @UserId AND [TaskId] = @TaskId_1;
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM UserTaskCursor INTO @TaskId_1, @UserId, @UserTaskOriginalBatchNo;
						END;
					CLOSE UserTaskCursor;
					DEALLOCATE UserTaskCursor;
				END;

				BEGIN
					DECLARE @TaskId_2 CHAR(13);
					DECLARE @TaskTitle VARCHAR(50);
					DECLARE @TaskDescription VARCHAR(MAX);
					DECLARE @StartDate DATE;
					DECLARE @EndDate DATE;
					DECLARE @TaskStatusId CHAR(8);
					DECLARE @TaskOriginalBatchNo VARCHAR(20);
					DECLARE TaskCursor CURSOR LOCAL FOR SELECT [TaskId], [TaskTitle], [TaskDescription], [StartDate], [EndDate], [TaskStatusId], [OriginalBatchNo] FROM [Task] WHERE [ProjectId] = @ProjectId;
					OPEN TaskCursor;
					FETCH NEXT FROM TaskCursor INTO @TaskId_2, @TaskTitle, @TaskDescription, @StartDate, @EndDate, @TaskStatusId, @TaskOriginalBatchNo;
					WHILE @@FETCH_STATUS = 0
						BEGIN
							SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
							INSERT INTO [TaskShadow] VALUES (@TaskId_2, @TaskTitle, @TaskDescription, @StartDate, @EndDate, @TaskStatusId, @ProjectId, @TaskOriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
							DELETE FROM [Task] WHERE [TaskId] = @TaskId_2;
							UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
							FETCH NEXT FROM TaskCursor INTO @TaskId_2, @TaskTitle, @TaskDescription, @StartDate, @EndDate, @TaskStatusId, @TaskOriginalBatchNo;
						END;
					CLOSE TaskCursor;
					DEALLOCATE TaskCursor;
				END;

				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				INSERT INTO [ProjectShadow] VALUES (@ProjectId, @ProjectName, @CompanyId, @OriginalBatchNo, @NextBatchNo, 'UPDATE', GETDATE(), @ActionUserId);
				DELETE FROM [Project] WHERE [ProjectId] = @ProjectId;
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
			COMMIT;
		END TRY
		BEGIN CATCH
			IF CURSOR_STATUS('local','UserTaskCursor') >= 0 
				BEGIN
					CLOSE UserTaskCursor;
					DEALLOCATE UserTaskCursor;
				END;
			IF CURSOR_STATUS('local','TaskCursor') >= 0 
				BEGIN
					CLOSE TaskCursor;
					DEALLOCATE TaskCursor;
				END;
			ROLLBACK;
			THROW 50007, 'Project deleting failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE ReadAllProjects
AS
BEGIN
	SELECT * FROM [Project];
END;

GO

CREATE OR ALTER PROCEDURE ReadProjectById
	@ProjectId CHAR(8)
AS
BEGIN
	SELECT * FROM [Project] WHERE [ProjectId] = @ProjectId;
END;

GO

CREATE OR ALTER PROCEDURE ReadProjectShadowByCurrentBatchNo
	@CurrentBatchNo VARCHAR(20)
AS
BEGIN
	SELECT * FROM [ProjectShadow] WHERE [CurrentBatchNo] = @CurrentBatchNo;
END;