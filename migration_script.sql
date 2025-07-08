
IF NOT EXISTS (
    SELECT * FROM [TransactionTypes]
    WHERE [TT_Name] = 'Income'
)
BEGIN
    INSERT INTO [TransactionTypes] VALUES('Income');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [TransactionTypes]
    WHERE [TT_Name] = 'Expenditure'
)
BEGIN
    INSERT INTO [TransactionTypes] VALUES('Expenditure');
END;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE TABLE [TransactionTypes] (
        [TT_Id] int NOT NULL IDENTITY,
        [TT_Name] nvarchar(150) NOT NULL,
        CONSTRAINT [PK_TransactionTypes] PRIMARY KEY ([TT_Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE TABLE [Users] (
        [U_Id] int NOT NULL IDENTITY,
        [U_FirstName] nvarchar(150) NOT NULL,
        [U_Name] nvarchar(150) NOT NULL,
        [U_Email] nvarchar(150) NOT NULL,
        [U_Password] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([U_Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE TABLE [TransactionCategories] (
        [TC_Id] int NOT NULL IDENTITY,
        [TC_Name] nvarchar(150) NOT NULL,
        [TC_Description] nvarchar(150) NOT NULL,
        [TC_UserU_Id] int NOT NULL,
        [TC_TransactionTypeTT_Id] int NOT NULL,
        CONSTRAINT [PK_TransactionCategories] PRIMARY KEY ([TC_Id]),
        CONSTRAINT [FK_TransactionCategories_TransactionTypes_TC_TransactionTypeTT_Id] FOREIGN KEY ([TC_TransactionTypeTT_Id]) REFERENCES [TransactionTypes] ([TT_Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TransactionCategories_Users_TC_UserU_Id] FOREIGN KEY ([TC_UserU_Id]) REFERENCES [Users] ([U_Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE TABLE [Expenses] (
        [EX_Id] int NOT NULL IDENTITY,
        [EX_Amount] decimal(18,2) NOT NULL,
        [EX_DateTime] datetime NOT NULL,
        [EX_Note] nvarchar(150) NOT NULL,
        [EX_UserU_Id] int NOT NULL,
        [EX_TransactionCategoryTC_Id] int NOT NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY ([EX_Id]),
        CONSTRAINT [FK_Expenses_TransactionCategories_EX_TransactionCategoryTC_Id] FOREIGN KEY ([EX_TransactionCategoryTC_Id]) REFERENCES [TransactionCategories] ([TC_Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Expenses_Users_EX_UserU_Id] FOREIGN KEY ([EX_UserU_Id]) REFERENCES [Users] ([U_Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE INDEX [IX_Expenses_EX_TransactionCategoryTC_Id] ON [Expenses] ([EX_TransactionCategoryTC_Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE INDEX [IX_Expenses_EX_UserU_Id] ON [Expenses] ([EX_UserU_Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE INDEX [IX_TransactionCategories_TC_TransactionTypeTT_Id] ON [TransactionCategories] ([TC_TransactionTypeTT_Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    CREATE INDEX [IX_TransactionCategories_TC_UserU_Id] ON [TransactionCategories] ([TC_UserU_Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241024204030_mssql.onprem_migration_919'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241024204030_mssql.onprem_migration_919', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241025155354_mssql.onprem_migration_955'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'U_Password');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Users] ALTER COLUMN [U_Password] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241025155354_mssql.onprem_migration_955'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241025155354_mssql.onprem_migration_955', N'8.0.10');
END;
GO

COMMIT;
GO

