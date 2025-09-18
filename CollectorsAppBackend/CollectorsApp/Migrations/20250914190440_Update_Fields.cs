using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectorsApp.Migrations
{
    /// <inheritdoc />
    public partial class Update_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "UserPreferences",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Dark",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Pagination",
                table: "UserPreferences",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Layout",
                table: "UserPreferences",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Classic",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ItemsPerPage",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                defaultValue: 20,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Theme",
                table: "UserPreferences",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldDefaultValue: "Dark");

            migrationBuilder.AlterColumn<bool>(
                name: "Pagination",
                table: "UserPreferences",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Layout",
                table: "UserPreferences",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldDefaultValue: "Classic");

            migrationBuilder.AlterColumn<int>(
                name: "ItemsPerPage",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 20);
        }
    }
}
