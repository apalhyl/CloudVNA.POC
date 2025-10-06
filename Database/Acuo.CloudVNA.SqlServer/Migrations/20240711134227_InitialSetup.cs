using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acuo.CloudVNA.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "UserActivity",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserData = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_UserData",
                schema: "dbo",
                table: "UserActivity",
                column: "UserData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivity",
                schema: "dbo");
        }
    }
}
