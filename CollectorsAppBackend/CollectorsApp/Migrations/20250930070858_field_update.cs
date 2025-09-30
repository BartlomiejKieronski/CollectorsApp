using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectorsApp.Migrations
{
    /// <inheritdoc />
    public partial class field_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestPathIV",
                table: "APILogs",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserIdHash",
                table: "APILogs",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestPathIV",
                table: "APILogs");

            migrationBuilder.DropColumn(
                name: "UserIdHash",
                table: "APILogs");
        }
    }
}
