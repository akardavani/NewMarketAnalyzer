/*
  Copy existing Tools data into a freshly migrated database.

  1. Select the NEW database in SSMS before running this script.
  2. Set @SourceDatabase to the database that currently contains your Tools data.
  3. Run once with @ExecuteTransfer = 0 to review row counts.
  4. Set @ExecuteTransfer = 1 and rerun only if every destination Tools table is empty.

  This script copies all rows, including stored files and password-vault records.
  It does not run automatically as part of EF migrations.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @SourceDatabase sysname = N'db_MarketAnalyzer_GM';
DECLARE @TargetDatabase sysname = DB_NAME();
DECLARE @ExecuteTransfer bit = 0;

IF DB_ID(@SourceDatabase) IS NULL
    THROW 51000, 'The source database does not exist.', 1;

IF DB_ID(@TargetDatabase) IS NULL OR @TargetDatabase = @SourceDatabase
    THROW 51001, 'Select a different, freshly migrated target database before running this script.', 1;

DECLARE @ToolsTables TABLE (TableName sysname NOT NULL PRIMARY KEY);
INSERT INTO @ToolsTables (TableName)
VALUES
    (N'BookmarkTagLinks'),
    (N'Bookmarks'),
    (N'BookmarkCollections'),
    (N'BookmarkTags'),
    (N'Notes'),
    (N'PasswordEntries'),
    (N'PasswordVaults'),
    (N'PublishTargets'),
    (N'PublishTargetFolders'),
    (N'Resources'),
    (N'ResourceFiles'),
    (N'ResourceTags'),
    (N'ResourceTagLinks');

DECLARE @TableName sysname;
DECLARE @Sql nvarchar(max);
DECLARE @SourceCount bigint;
DECLARE @TargetCount bigint;

DECLARE PreviewCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT TableName FROM @ToolsTables ORDER BY TableName;
OPEN PreviewCursor;
FETCH NEXT FROM PreviewCursor INTO @TableName;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(QUOTENAME(@TargetDatabase) + N'.[Tools].' + QUOTENAME(@TableName), N'U') IS NULL
       OR OBJECT_ID(QUOTENAME(@SourceDatabase) + N'.[Tools].' + QUOTENAME(@TableName), N'U') IS NULL
        THROW 51002, 'A required Tools table is missing from the source or target database.', 1;

    SET @Sql = N'SELECT @Count = COUNT_BIG(*) FROM ' + QUOTENAME(@SourceDatabase) + N'.[Tools].' + QUOTENAME(@TableName) + N';';
    EXEC sys.sp_executesql @Sql, N'@Count bigint OUTPUT', @Count = @SourceCount OUTPUT;
    SET @Sql = N'SELECT @Count = COUNT_BIG(*) FROM ' + QUOTENAME(@TargetDatabase) + N'.[Tools].' + QUOTENAME(@TableName) + N';';
    EXEC sys.sp_executesql @Sql, N'@Count bigint OUTPUT', @Count = @TargetCount OUTPUT;

    SELECT @TableName AS TableName, @SourceCount AS SourceRows, @TargetCount AS TargetRows;
    IF @TargetCount <> 0
        THROW 51003, 'Target Tools tables must be empty; refusing to merge or overwrite data.', 1;

    FETCH NEXT FROM PreviewCursor INTO @TableName;
END;
CLOSE PreviewCursor;
DEALLOCATE PreviewCursor;

IF @ExecuteTransfer = 0
BEGIN
    PRINT 'Preview only. Set @ExecuteTransfer = 1 after reviewing the counts to copy the rows.';
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE ConstraintCursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName FROM @ToolsTables ORDER BY TableName;
    OPEN ConstraintCursor;
    FETCH NEXT FROM ConstraintCursor INTO @TableName;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Sql = N'ALTER TABLE [Tools].' + QUOTENAME(@TableName) + N' NOCHECK CONSTRAINT ALL;';
        EXEC sys.sp_executesql @Sql;
        FETCH NEXT FROM ConstraintCursor INTO @TableName;
    END;
    CLOSE ConstraintCursor;
    DEALLOCATE ConstraintCursor;

    DECLARE CopyCursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName FROM @ToolsTables ORDER BY TableName;
    OPEN CopyCursor;
    FETCH NEXT FROM CopyCursor INTO @TableName;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @Columns nvarchar(max);
        DECLARE @HasIdentity bit;

        SET @Sql = N'SELECT @ColumnList = STUFF((SELECT N'','' + QUOTENAME(c.name) FROM '
            + QUOTENAME(@TargetDatabase) + N'.sys.columns c JOIN '
            + QUOTENAME(@TargetDatabase) + N'.sys.tables t ON t.object_id = c.object_id JOIN '
            + QUOTENAME(@TargetDatabase) + N'.sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = N''Tools'' AND t.name = @Name AND c.is_computed = 0 AND c.generated_always_type = 0 ORDER BY c.column_id FOR XML PATH(N''''), TYPE).value(N''.'', N''nvarchar(max)''), 1, 1, N''''), @Identity = CASE WHEN EXISTS (SELECT 1 FROM '
            + QUOTENAME(@TargetDatabase) + N'.sys.columns c JOIN '
            + QUOTENAME(@TargetDatabase) + N'.sys.tables t ON t.object_id = c.object_id JOIN '
            + QUOTENAME(@TargetDatabase) + N'.sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = N''Tools'' AND t.name = @Name AND c.is_identity = 1) THEN 1 ELSE 0 END;';
        EXEC sys.sp_executesql @Sql, N'@Name sysname, @ColumnList nvarchar(max) OUTPUT, @Identity bit OUTPUT', @Name = @TableName, @ColumnList = @Columns OUTPUT, @Identity = @HasIdentity OUTPUT;

        SET @Sql = CASE WHEN @HasIdentity = 1
            THEN N'SET IDENTITY_INSERT [Tools].' + QUOTENAME(@TableName) + N' ON; ' ELSE N'' END
            + N'INSERT INTO [Tools].' + QUOTENAME(@TableName) + N' (' + @Columns + N') SELECT ' + @Columns + N' FROM '
            + QUOTENAME(@SourceDatabase) + N'.[Tools].' + QUOTENAME(@TableName) + N'; '
            + CASE WHEN @HasIdentity = 1 THEN N'SET IDENTITY_INSERT [Tools].' + QUOTENAME(@TableName) + N' OFF;' ELSE N'' END;
        EXEC sys.sp_executesql @Sql;

        FETCH NEXT FROM CopyCursor INTO @TableName;
    END;
    CLOSE CopyCursor;
    DEALLOCATE CopyCursor;

    DECLARE ValidateCursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName FROM @ToolsTables ORDER BY TableName;
    OPEN ValidateCursor;
    FETCH NEXT FROM ValidateCursor INTO @TableName;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Sql = N'ALTER TABLE [Tools].' + QUOTENAME(@TableName) + N' WITH CHECK CHECK CONSTRAINT ALL;';
        EXEC sys.sp_executesql @Sql;
        FETCH NEXT FROM ValidateCursor INTO @TableName;
    END;
    CLOSE ValidateCursor;
    DEALLOCATE ValidateCursor;

    COMMIT TRANSACTION;
    PRINT 'Tools data copy completed. Review row counts before using the new database.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
