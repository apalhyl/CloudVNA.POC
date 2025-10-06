using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acuo.IHEAudit.DAL.Original.SqlServer
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_ACUO_AUDIT",
                columns: table => new
                {
                    AL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AL_AUDIT_DATE = table.Column<DateTime>(type: "datetime", nullable: false),
                    AL_EVENT_TYPE = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    AL_USER = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_PT_ID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_PT_NAME = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_ST_DICOM_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_SE_MODALITY = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: true),
                    AL_SE_DICOM_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_IM_SOPINSTANCE_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_FILE_NAME = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    AL_SOURCE = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_MACHINE = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_DATABASE = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_COMMENT = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    AL_IHE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AL_ACCESSION_NUMBER = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    AL_ACUOSTORE_SERVER_NAME = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    AL_ACUOSTORE_APPLICATION_NAME = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    AL_ACUOSTORE_USER_NAME = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    AL_ACUOSTORE_USER_PASSWORD = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    AL_RECYCLE_BIN_FOLDER = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: true),
                    AL_IMAGE_GUID = table.Column<byte[]>(type: "binary(16)", fixedLength: true, maxLength: 16, nullable: true),
                    AL_DELETION_SET_ID = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    AL_VERIFIED_FOR_DELETION = table.Column<bool>(type: "bit", nullable: false),
                    AL_RESTORED_LAST_DATE = table.Column<DateTime>(type: "datetime", nullable: true),
                    AL_RESTORED_SERVER = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    AL_RESTORED_PORT = table.Column<int>(type: "int", nullable: true),
                    AL_RESTORED_AE_NAME = table.Column<string>(type: "varchar(17)", unicode: false, maxLength: 17, nullable: true),
                    AL_RESTORE = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ACUO_AUDIT__Id", x => x.AL_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_CONFIG",
                columns: table => new
                {
                    CONFIG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CONFIG_DAYS_TO_KEEP_AUDITS = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    CONFIG_SECONDS_TO_CHECK_FOR_SYSLOG_CHANGES = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    CONFIG_SECONDS_TO_CHECK_FOR_SYSLOG_AUDITS = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    CONFIG_DAYS_TO_KEEP_ACUO_AUDITS = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    CONFIG_MINUTES_TO_KEEP_SYSLOG_TASKS = table.Column<int>(type: "int", nullable: false, defaultValue: 1440),
                    CONFIG_DAYS_TO_KEEP_DELETED_IMAGES = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    CONFIG_DELETION_VERIFICATION_REQUIRED = table.Column<bool>(type: "bit", nullable: false),
                    CONFIG_MANUAL_DELETION_ONLY = table.Column<bool>(type: "bit", nullable: false),
                    CONFIG_DAYS_TO_KEEP_USER_AUDITS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CONFIG__Id", x => x.CONFIG_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_EVENTS",
                columns: table => new
                {
                    EV_ID = table.Column<int>(type: "int", nullable: false),
                    EV_TYPE = table.Column<int>(type: "int", nullable: false),
                    EV_CODE_SYSTEM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EV_ID_DESCRIPTION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EV_TYPE_DESCRIPTION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EV_LOG_CREATE_SUPPORT = table.Column<int>(type: "int", nullable: false),
                    EV_LOG_READ_SUPPORT = table.Column<int>(type: "int", nullable: false),
                    EV_LOG_UPDATE_SUPPORT = table.Column<int>(type: "int", nullable: false),
                    EV_LOG_DELETE_SUPPORT = table.Column<int>(type: "int", nullable: false),
                    EV_LOG_EXECUTE_SUPPORT = table.Column<int>(type: "int", nullable: false),
                    EV_LOG_CREATE = table.Column<bool>(type: "bit", nullable: false),
                    EV_LOG_READ = table.Column<bool>(type: "bit", nullable: false),
                    EV_LOG_UPDATE = table.Column<bool>(type: "bit", nullable: false),
                    EV_LOG_DELETE = table.Column<bool>(type: "bit", nullable: false),
                    EV_LOG_EXECUTE = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_EVENTS__Id_Type", x => new { x.EV_ID, x.EV_TYPE });
                });

            migrationBuilder.CreateTable(
                name: "T_IHE_AUDIT",
                columns: table => new
                {
                    IHE_AUDIT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IHE_LOCAL_DATE = table.Column<DateTime>(type: "datetime", nullable: false),
                    IHE_SYSLOG_SEVERITY = table.Column<int>(type: "int", nullable: false),
                    IHE_EVENT_ID = table.Column<int>(type: "int", nullable: false),
                    IHE_EVENT_TYPE = table.Column<int>(type: "int", nullable: true),
                    IHE_EVENT_ACTION = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    IHE_XML_MESSAGE = table.Column<byte[]>(type: "image", nullable: false),
                    IHE_VERIFIED_FOR_DELETION = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_IHE_AUDIT__AuditId", x => x.IHE_AUDIT_ID)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "T_SYSLOG",
                columns: table => new
                {
                    RS_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RS_TYPE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RS_HOSTID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RS_PORT = table.Column<int>(type: "int", nullable: false),
                    RS_PAUSE = table.Column<bool>(type: "bit", nullable: false),
                    RS_TLS_ENABLED = table.Column<bool>(type: "bit", nullable: true),
                    RS_CLIENT_CERT = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RS_SERVER_CERT = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SYSLOG__Id", x => x.RS_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_TASKS",
                columns: table => new
                {
                    TASK_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TASK_TARGET_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TASK_IHE_AUDIT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TASK_STATUS = table.Column<int>(type: "int", nullable: false),
                    TASK_LAST_ERROR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TASK_QUEUED_TIME = table.Column<DateTime>(type: "datetime", nullable: false),
                    TASK_XML_MESSAGE = table.Column<byte[]>(type: "image", nullable: false, defaultValueSql: "(0x00)"),
                    TASK_RUN_COUNT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TASKS__Id", x => x.TASK_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_UPGRADE_BITMASK",
                columns: table => new
                {
                    MSK_KeyWord = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    MSK_Description = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    MSK_KeyType = table.Column<int>(type: "int", nullable: false),
                    MSK_MaskInteger = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UPGRADE_BITMASK__KeyWord", x => x.MSK_KeyWord);
                });

            migrationBuilder.CreateTable(
                name: "T_UPGRADE_DETAIL",
                columns: table => new
                {
                    DTL_Identity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DTL_HST_Identity = table.Column<int>(type: "int", nullable: false),
                    DTL_UtcStartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DTL_UtcEndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DTL_Task = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DTL_Retry = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UPGRADE_DETAIL__Identity", x => x.DTL_Identity);
                });

            migrationBuilder.CreateTable(
                name: "T_UPGRADE_HISTORY",
                columns: table => new
                {
                    HST_Identity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HST_SQLEdition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HST_SQLVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HST_SQLLevel = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    HST_SQLMachine = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HST_SQLInstance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HST_UtcStartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    HST_UtcEndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    HST_Old_Status = table.Column<int>(type: "int", nullable: false),
                    HST_Old_Major = table.Column<int>(type: "int", nullable: false),
                    HST_Old_Minor = table.Column<int>(type: "int", nullable: false),
                    HST_Old_SP = table.Column<int>(type: "int", nullable: false),
                    HST_Old_Build = table.Column<int>(type: "int", nullable: false),
                    HST_Old_DbSize = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    HST_New_Status = table.Column<int>(type: "int", nullable: false),
                    HST_New_Major = table.Column<int>(type: "int", nullable: false),
                    HST_New_Minor = table.Column<int>(type: "int", nullable: false),
                    HST_New_SP = table.Column<int>(type: "int", nullable: false),
                    HST_New_Build = table.Column<int>(type: "int", nullable: false),
                    HST_New_DbSize = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    HST_By_User = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HST_Comment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HST_State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UPGRADE_HISTORY__Identity", x => x.HST_Identity);
                });

            migrationBuilder.CreateTable(
                name: "T_USER_AUDIT",
                columns: table => new
                {
                    UAA_AUDIT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    UAA_AUDIT_DATE = table.Column<DateTime>(type: "datetime", nullable: false),
                    UAA_AUDIT_LEVEL = table.Column<short>(type: "smallint", nullable: true),
                    UAA_AUDIT_ACTION = table.Column<short>(type: "smallint", nullable: true),
                    UAA_USER_DOMAIN = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_USER = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_PT_ID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: false),
                    UAA_PT_NAME = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: false),
                    UAA_ST_DICOM_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_ST_ACCESSION_NUMBER = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_SE_DICOM_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_IM_DICOM_UID = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    UAA_QUERY_STRING = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    UAA_IHE_AUDIT = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UAA_VERIFIED_FOR_DELETION = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USER_AUDIT", x => x.UAA_AUDIT_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_VERSION_VER",
                columns: table => new
                {
                    VER_MAJOR = table.Column<int>(type: "int", nullable: false),
                    VER_MINOR = table.Column<int>(type: "int", nullable: false),
                    VER_REVISION = table.Column<int>(type: "int", nullable: false),
                    VER_BUILDNUMBER = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_ACUO_AUDIT__AuditDate_EventType_ServerName",
                table: "T_ACUO_AUDIT",
                columns: new[] { "AL_AUDIT_DATE", "AL_EVENT_TYPE", "AL_ACUOSTORE_SERVER_NAME" })
                .Annotation("SqlServer:FillFactor", 100);

            migrationBuilder.CreateIndex(
                name: "IX_T_IHE_AUDIT__LocalDate",
                table: "T_IHE_AUDIT",
                column: "IHE_LOCAL_DATE")
                .Annotation("SqlServer:Clustered", true)
                .Annotation("SqlServer:FillFactor", 100);

            migrationBuilder.CreateIndex(
                name: "IX_T_TASKS__TargetId_IheAuditId",
                table: "T_TASKS",
                columns: new[] { "TASK_TARGET_ID", "TASK_IHE_AUDIT_ID" })
                .Annotation("SqlServer:FillFactor", 90);

            migrationBuilder.CreateIndex(
                name: "IX_T_UPGRADE_DETAIL__Task_HSTIdentity",
                table: "T_UPGRADE_DETAIL",
                columns: new[] { "DTL_Task", "DTL_HST_Identity" })
                .Annotation("SqlServer:FillFactor", 100);

            migrationBuilder.CreateIndex(
                name: "IX_T_USER_AUDIT__AuditDate_Verified",
                table: "T_USER_AUDIT",
                columns: new[] { "UAA_AUDIT_DATE", "UAA_VERIFIED_FOR_DELETION" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_T_USER_AUDIT__PatientId",
                table: "T_USER_AUDIT",
                column: "UAA_PT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_USER_AUDIT__PatientName",
                table: "T_USER_AUDIT",
                column: "UAA_PT_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_USER_AUDIT__User",
                table: "T_USER_AUDIT",
                column: "UAA_USER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_ACUO_AUDIT");

            migrationBuilder.DropTable(
                name: "T_CONFIG");

            migrationBuilder.DropTable(
                name: "T_EVENTS");

            migrationBuilder.DropTable(
                name: "T_IHE_AUDIT");

            migrationBuilder.DropTable(
                name: "T_SYSLOG");

            migrationBuilder.DropTable(
                name: "T_TASKS");

            migrationBuilder.DropTable(
                name: "T_UPGRADE_BITMASK");

            migrationBuilder.DropTable(
                name: "T_UPGRADE_DETAIL");

            migrationBuilder.DropTable(
                name: "T_UPGRADE_HISTORY");

            migrationBuilder.DropTable(
                name: "T_USER_AUDIT");

            migrationBuilder.DropTable(
                name: "T_VERSION_VER");
        }
    }
}
