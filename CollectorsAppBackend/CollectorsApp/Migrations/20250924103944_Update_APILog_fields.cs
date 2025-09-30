using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectorsApp.Migrations
{
    /// <inheritdoc />
    public partial class Update_APILog_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "APILogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserIDIV",
                table: "APILogs",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "consentId",
                table: "APILogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIDIV",
                table: "APILogs");

            migrationBuilder.DropColumn(
                name: "consentId",
                table: "APILogs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "APILogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);
        }
    }
}
