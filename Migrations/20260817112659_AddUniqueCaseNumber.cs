using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caseManageMentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCaseNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CaseNumber",
                table: "Cases",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases",
                column: "CaseNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases");

            migrationBuilder.AlterColumn<string>(
                name: "CaseNumber",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
