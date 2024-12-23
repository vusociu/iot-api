using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iot_project.Migrations
{
    /// <inheritdoc />
    public partial class addCollumIntoHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fullName",
                table: "CheckCardHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fullName",
                table: "CheckCardHistories");
        }
    }
}
