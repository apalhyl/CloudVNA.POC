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
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_ACUO_AUDIT] (
        [AL_ID] int NOT NULL IDENTITY,
        [AL_AUDIT_DATE] datetime NOT NULL,
        [AL_EVENT_TYPE] varchar(255) NOT NULL,
        [AL_USER] varchar(65) NULL,
        [AL_PT_ID] varchar(65) NULL,
        [AL_PT_NAME] varchar(65) NULL,
        [AL_ST_DICOM_UID] varchar(65) NULL,
        [AL_SE_MODALITY] varchar(17) NULL,
        [AL_SE_DICOM_UID] varchar(65) NULL,
        [AL_IM_SOPINSTANCE_UID] varchar(65) NULL,
        [AL_FILE_NAME] varchar(256) NULL,
        [AL_SOURCE] varchar(65) NULL,
        [AL_MACHINE] varchar(65) NULL,
        [AL_DATABASE] varchar(65) NULL,
        [AL_COMMENT] varchar(256) NULL,
        [AL_IHE_ID] uniqueidentifier NULL,
        [AL_ACCESSION_NUMBER] varchar(65) NULL,
        [AL_ACUOSTORE_SERVER_NAME] varchar(256) NULL,
        [AL_ACUOSTORE_APPLICATION_NAME] varchar(64) NULL,
        [AL_ACUOSTORE_USER_NAME] varchar(64) NULL,
        [AL_ACUOSTORE_USER_PASSWORD] varchar(64) NULL,
        [AL_RECYCLE_BIN_FOLDER] binary(16) NULL,
        [AL_IMAGE_GUID] binary(16) NULL,
        [AL_DELETION_SET_ID] varchar(256) NULL,
        [AL_VERIFIED_FOR_DELETION] bit NOT NULL,
        [AL_RESTORED_LAST_DATE] datetime NULL,
        [AL_RESTORED_SERVER] varchar(256) NULL,
        [AL_RESTORED_PORT] int NULL,
        [AL_RESTORED_AE_NAME] varchar(17) NULL,
        [AL_RESTORE] bit NOT NULL,
        CONSTRAINT [PK_T_ACUO_AUDIT__Id] PRIMARY KEY ([AL_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_CONFIG] (
        [CONFIG_ID] int NOT NULL IDENTITY,
        [CONFIG_DAYS_TO_KEEP_AUDITS] int NOT NULL DEFAULT 30,
        [CONFIG_SECONDS_TO_CHECK_FOR_SYSLOG_CHANGES] int NOT NULL DEFAULT 10,
        [CONFIG_SECONDS_TO_CHECK_FOR_SYSLOG_AUDITS] int NOT NULL DEFAULT 5,
        [CONFIG_DAYS_TO_KEEP_ACUO_AUDITS] int NOT NULL DEFAULT 30,
        [CONFIG_MINUTES_TO_KEEP_SYSLOG_TASKS] int NOT NULL DEFAULT 1440,
        [CONFIG_DAYS_TO_KEEP_DELETED_IMAGES] int NOT NULL DEFAULT 30,
        [CONFIG_DELETION_VERIFICATION_REQUIRED] bit NOT NULL,
        [CONFIG_MANUAL_DELETION_ONLY] bit NOT NULL,
        [CONFIG_DAYS_TO_KEEP_USER_AUDITS] int NOT NULL,
        CONSTRAINT [PK_T_CONFIG__Id] PRIMARY KEY ([CONFIG_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_EVENTS] (
        [EV_ID] int NOT NULL,
        [EV_TYPE] int NOT NULL,
        [EV_CODE_SYSTEM] nvarchar(50) NOT NULL,
        [EV_ID_DESCRIPTION] nvarchar(50) NOT NULL,
        [EV_TYPE_DESCRIPTION] nvarchar(50) NULL,
        [EV_LOG_CREATE_SUPPORT] int NOT NULL,
        [EV_LOG_READ_SUPPORT] int NOT NULL,
        [EV_LOG_UPDATE_SUPPORT] int NOT NULL,
        [EV_LOG_DELETE_SUPPORT] int NOT NULL,
        [EV_LOG_EXECUTE_SUPPORT] int NOT NULL,
        [EV_LOG_CREATE] bit NOT NULL,
        [EV_LOG_READ] bit NOT NULL,
        [EV_LOG_UPDATE] bit NOT NULL,
        [EV_LOG_DELETE] bit NOT NULL,
        [EV_LOG_EXECUTE] bit NOT NULL,
        CONSTRAINT [PK_T_EVENTS__Id_Type] PRIMARY KEY ([EV_ID], [EV_TYPE])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_IHE_AUDIT] (
        [IHE_AUDIT_ID] uniqueidentifier NOT NULL,
        [IHE_LOCAL_DATE] datetime NOT NULL,
        [IHE_SYSLOG_SEVERITY] int NOT NULL,
        [IHE_EVENT_ID] int NOT NULL,
        [IHE_EVENT_TYPE] int NULL,
        [IHE_EVENT_ACTION] char(1) NOT NULL,
        [IHE_XML_MESSAGE] image NOT NULL,
        [IHE_VERIFIED_FOR_DELETION] bit NOT NULL,
        CONSTRAINT [PK_T_IHE_AUDIT__AuditId] PRIMARY KEY NONCLUSTERED ([IHE_AUDIT_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_SYSLOG] (
        [RS_ID] uniqueidentifier NOT NULL DEFAULT ((newid())),
        [RS_TYPE] nvarchar(10) NOT NULL,
        [RS_HOSTID] nvarchar(255) NOT NULL,
        [RS_PORT] int NOT NULL,
        [RS_PAUSE] bit NOT NULL,
        [RS_TLS_ENABLED] bit NULL,
        [RS_CLIENT_CERT] nvarchar(255) NULL,
        [RS_SERVER_CERT] nvarchar(255) NULL,
        CONSTRAINT [PK_T_SYSLOG__Id] PRIMARY KEY ([RS_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_TASKS] (
        [TASK_ID] int NOT NULL IDENTITY,
        [TASK_TARGET_ID] uniqueidentifier NOT NULL,
        [TASK_IHE_AUDIT_ID] uniqueidentifier NOT NULL,
        [TASK_STATUS] int NOT NULL,
        [TASK_LAST_ERROR] nvarchar(255) NULL,
        [TASK_QUEUED_TIME] datetime NOT NULL,
        [TASK_XML_MESSAGE] image NOT NULL DEFAULT ((0x00)),
        [TASK_RUN_COUNT] int NOT NULL,
        CONSTRAINT [PK_T_TASKS__Id] PRIMARY KEY ([TASK_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_UPGRADE_BITMASK] (
        [MSK_KeyWord] nvarchar(40) NOT NULL,
        [MSK_Description] nvarchar(40) NOT NULL,
        [MSK_KeyType] int NOT NULL,
        [MSK_MaskInteger] int NOT NULL,
        CONSTRAINT [PK_T_UPGRADE_BITMASK__KeyWord] PRIMARY KEY ([MSK_KeyWord])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_UPGRADE_DETAIL] (
        [DTL_Identity] int NOT NULL IDENTITY,
        [DTL_HST_Identity] int NOT NULL,
        [DTL_UtcStartDate] datetime NOT NULL,
        [DTL_UtcEndDate] datetime NOT NULL,
        [DTL_Task] nvarchar(255) NULL,
        [DTL_Retry] smallint NOT NULL,
        CONSTRAINT [PK_T_UPGRADE_DETAIL__Identity] PRIMARY KEY ([DTL_Identity])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_UPGRADE_HISTORY] (
        [HST_Identity] int NOT NULL IDENTITY,
        [HST_SQLEdition] nvarchar(20) NOT NULL,
        [HST_SQLVersion] nvarchar(20) NOT NULL,
        [HST_SQLLevel] nvarchar(5) NOT NULL,
        [HST_SQLMachine] nvarchar(50) NOT NULL,
        [HST_SQLInstance] nvarchar(50) NOT NULL,
        [HST_UtcStartDate] datetime NOT NULL,
        [HST_UtcEndDate] datetime NOT NULL,
        [HST_Old_Status] int NOT NULL,
        [HST_Old_Major] int NOT NULL,
        [HST_Old_Minor] int NOT NULL,
        [HST_Old_SP] int NOT NULL,
        [HST_Old_Build] int NOT NULL,
        [HST_Old_DbSize] bigint NOT NULL DEFAULT CAST(-1 AS bigint),
        [HST_New_Status] int NOT NULL,
        [HST_New_Major] int NOT NULL,
        [HST_New_Minor] int NOT NULL,
        [HST_New_SP] int NOT NULL,
        [HST_New_Build] int NOT NULL,
        [HST_New_DbSize] bigint NOT NULL DEFAULT CAST(-1 AS bigint),
        [HST_By_User] nvarchar(100) NOT NULL,
        [HST_Comment] nvarchar(100) NULL,
        [HST_State] int NOT NULL,
        CONSTRAINT [PK_T_UPGRADE_HISTORY__Identity] PRIMARY KEY ([HST_Identity])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_USER_AUDIT] (
        [UAA_AUDIT_ID] uniqueidentifier NOT NULL DEFAULT ((newsequentialid())),
        [UAA_AUDIT_DATE] datetime NOT NULL,
        [UAA_AUDIT_LEVEL] smallint NULL,
        [UAA_AUDIT_ACTION] smallint NULL,
        [UAA_USER_DOMAIN] varchar(65) NULL,
        [UAA_USER] varchar(65) NULL,
        [UAA_PT_ID] varchar(65) NOT NULL,
        [UAA_PT_NAME] varchar(65) NOT NULL,
        [UAA_ST_DICOM_UID] varchar(65) NULL,
        [UAA_ST_ACCESSION_NUMBER] varchar(65) NULL,
        [UAA_SE_DICOM_UID] varchar(65) NULL,
        [UAA_IM_DICOM_UID] varchar(65) NULL,
        [UAA_QUERY_STRING] varchar(max) NULL,
        [UAA_IHE_AUDIT] uniqueidentifier NULL,
        [UAA_VERIFIED_FOR_DELETION] bit NOT NULL,
        CONSTRAINT [PK_T_USER_AUDIT] PRIMARY KEY ([UAA_AUDIT_ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE TABLE [T_VERSION_VER] (
        [VER_MAJOR] int NOT NULL,
        [VER_MINOR] int NOT NULL,
        [VER_REVISION] int NOT NULL,
        [VER_BUILDNUMBER] int NOT NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_ACUO_AUDIT__AuditDate_EventType_ServerName] ON [T_ACUO_AUDIT] ([AL_AUDIT_DATE], [AL_EVENT_TYPE], [AL_ACUOSTORE_SERVER_NAME]) WITH (FILLFACTOR = 100);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE CLUSTERED INDEX [IX_T_IHE_AUDIT__LocalDate] ON [T_IHE_AUDIT] ([IHE_LOCAL_DATE]) WITH (FILLFACTOR = 100);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_TASKS__TargetId_IheAuditId] ON [T_TASKS] ([TASK_TARGET_ID], [TASK_IHE_AUDIT_ID]) WITH (FILLFACTOR = 90);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_UPGRADE_DETAIL__Task_HSTIdentity] ON [T_UPGRADE_DETAIL] ([DTL_Task], [DTL_HST_Identity]) WITH (FILLFACTOR = 100);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_USER_AUDIT__AuditDate_Verified] ON [T_USER_AUDIT] ([UAA_AUDIT_DATE], [UAA_VERIFIED_FOR_DELETION] DESC);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_USER_AUDIT__PatientId] ON [T_USER_AUDIT] ([UAA_PT_ID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_USER_AUDIT__PatientName] ON [T_USER_AUDIT] ([UAA_PT_NAME]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    CREATE INDEX [IX_T_USER_AUDIT__User] ON [T_USER_AUDIT] ([UAA_USER]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240828095112_InitialSetup'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240828095112_InitialSetup', N'8.0.8');
END;
GO

COMMIT;
GO

