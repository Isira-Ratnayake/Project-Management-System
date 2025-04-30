CREATE OR ALTER FUNCTION GenerateNextBatchNo
(
	@CompanyId CHAR(3)
)
RETURNS VARCHAR(20)
BEGIN
	DECLARE @Batches INT;
	DECLARE @NextBatchNo VARCHAR(20);
	SELECT @Batches = ([Batches]+1) FROM [CompanySequence] WHERE [CompanyId] = @CompanyId;
	SET @NextBatchNo = CONCAT_WS('-', @CompanyId, CAST(@Batches AS VARCHAR));
	RETURN @NextBatchNo;
END;

GO

CREATE OR ALTER FUNCTION GenerateNextUserId
(
	@CompanyId CHAR(3)
)
RETURNS CHAR(8)
BEGIN
	DECLARE @Users INT;
	DECLARE @NextUserId CHAR(8);
	SELECT @Users = ([Users]+1) FROM [CompanySequence] WHERE [CompanyId] = @CompanyId;
	SET @NextUserId = CONCAT_WS('-', @CompanyId, RIGHT(REPLICATE('0', 4) + CAST(@Users AS VARCHAR), 4));
	RETURN @NextUserId;
END;

GO

CREATE OR ALTER PROCEDURE CreateUser
	@UserEmail VARCHAR(100),
	@UserPassword VARCHAR(300),
	@UserFullname VARCHAR(100),
	@UserGroupId CHAR(5),
	@ActionUserId CHAR(10)
AS
BEGIN
	DECLARE @UsersCount INT;
	DECLARE @NextUserId CHAR(10);
	DECLARE @NextBatchNo VARCHAR(20);

	SELECT @UsersCount = COUNT([UserId]) FROM [User] WHERE [UserEmail] = @UserEmail;

	IF @UsersCount > 0
		THROW 50000, 'This user email is already registered.', 1; 
	ELSE
		BEGIN
			BEGIN TRY
				SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
				BEGIN TRANSACTION
					SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
					SET @NextUserId = dbo.GenerateNextUserId('CCL');
					INSERT INTO [UserShadow] VALUES (@NextUserId, @UserEmail, @UserPassword, @UserFullname, @UserGroupId, @NextBatchNo, @NextBatchNo, 'INSERT', GETDATE(), @ActionUserId);
					INSERT INTO [User] VALUES (@NextUserId, @UserEmail, @UserPassword, @UserFullname, @UserGroupId, @NextBatchNo, @NextBatchNo);
					UPDATE [CompanySequence] SET [Batches] = [Batches] + 1, [Users] = [Users] + 1 WHERE [CompanyId] = 'CCL';
				COMMIT;
			END TRY
			BEGIN CATCH
				ROLLBACK;
				THROW 50001, 'User creation failed. Please try again.', 1; 
			END CATCH;
		END;
END;

GO

CREATE OR ALTER PROCEDURE UpdateUser
	@UserEmail VARCHAR(100),
	@UserFullname VARCHAR(100),
	@UserGroupId CHAR(5),
	@UserId CHAR(8),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(10)
AS
BEGIN
	DECLARE @UsersCount INT;
	DECLARE @UserPassword VARCHAR(300);
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);

	SELECT @UsersCount = COUNT([UserId]) FROM [User] WHERE [UserEmail] = @UserEmail AND [UserId] <> @UserId;

	IF @UsersCount > 0
		THROW 50000, 'This user email is already registered.', 2; 
	ELSE
		BEGIN
			BEGIN TRY
				SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
				BEGIN TRANSACTION
					SELECT @UserPassword = [UserPassword], @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [User] WHERE [UserId] = @UserId;
					IF @CurrentBatchNo <> @ExistingBatchNo
						THROW 50000, 'Record version conflict.', 1;
					SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
					INSERT INTO [UserShadow] VALUES (@UserId, @UserEmail, @UserPassword, @UserFullname, @UserGroupId, @OriginalBatchNo, @NextBatchNo, 'UPDATE', GETDATE(), @ActionUserId);
					UPDATE [User] SET [UserEmail] = @UserEmail, [UserFullname] = @UserFullname, [UserGroupId] = @UserGroupId, [CurrentBatchNo] = @NextBatchNo WHERE [UserId] = @UserId;
					UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
				COMMIT;
			END TRY
			BEGIN CATCH
				ROLLBACK;
				THROW 50003, 'User updating failed. Please try again.', 1;
			END CATCH;
		END;
END;

GO

CREATE OR ALTER PROCEDURE DeleteUser
	@UserId CHAR(8),
	@CurrentBatchNo VARCHAR(20),
	@ActionUserId CHAR(10)
AS
BEGIN
	DECLARE @UserPassword VARCHAR(300);
	DECLARE @OriginalBatchNo VARCHAR(20);
	DECLARE @ExistingBatchNo VARCHAR(20);
	DECLARE @NextBatchNo VARCHAR(20);
	DECLARE	@UserEmail VARCHAR(100);
	DECLARE @UserFullname VARCHAR(100);
	DECLARE @UserGroupId CHAR(5);

	BEGIN
		BEGIN TRY
			SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
			BEGIN TRANSACTION
				SELECT @UserEmail = [UserEmail], @UserPassword = [UserPassword], @UserFullname = [UserFullname], @UserGroupId = [UserGroupId], @OriginalBatchNo = [OriginalBatchNo], @ExistingBatchNo = [CurrentBatchNo] FROM [User] WHERE [UserId] = @UserId;
				IF @CurrentBatchNo <> @ExistingBatchNo
					THROW 50000, 'Record version conflict.', 1;
				SET @NextBatchNo = dbo.GenerateNextBatchNo('CCL');
				INSERT INTO [UserShadow] VALUES (@UserId, @UserEmail, @UserPassword, @UserFullname, @UserGroupId, @OriginalBatchNo, @NextBatchNo, 'DELETE', GETDATE(), @ActionUserId);
				DELETE FROM [User] WHERE [UserId] = @UserId;
				UPDATE [CompanySequence] SET [Batches] = [Batches] + 1 WHERE [CompanyId] = 'CCL';
			COMMIT;
		END TRY
		BEGIN CATCH
			ROLLBACK;
			THROW 50003, 'User deleting failed. Please try again.', 1; 
		END CATCH;
	END;
END;

GO

CREATE OR ALTER PROCEDURE ReadAllUsers
AS
BEGIN
	SELECT * FROM [UsersView];
END;

GO

CREATE OR ALTER PROCEDURE ReadUserById
	@UserId CHAR(8)
AS
BEGIN
	SELECT * FROM [UsersView] WHERE [UserId] = @UserId;
END;

GO

CREATE OR ALTER PROCEDURE ReadUserByEmail
	@UserEmail VARCHAR(100)
AS
BEGIN
	SELECT * FROM [UsersView] WHERE [UserEmail] = @UserEmail;
END;

GO

CREATE OR ALTER PROCEDURE ReadUserShadowByCurrentBatchNo
	@CurrentBatchNo VARCHAR(20)
AS
BEGIN
	SELECT * FROM [UserShadow] WHERE [CurrentBatchNo] = @CurrentBatchNo;
END;