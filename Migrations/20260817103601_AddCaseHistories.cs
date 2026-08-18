using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caseManageMentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseHistory_AspNetUsers_UserId",
                table: "CaseHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseHistory_Cases_CaseId",
                table: "CaseHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseHistory",
                table: "CaseHistory");

            migrationBuilder.RenameTable(
                name: "CaseHistory",
                newName: "CaseHistories");

            migrationBuilder.RenameIndex(
                name: "IX_CaseHistory_UserId",
                table: "CaseHistories",
                newName: "IX_CaseHistories_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CaseHistory_CaseId",
                table: "CaseHistories",
                newName: "IX_CaseHistories_CaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseHistories",
                table: "CaseHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseHistories_AspNetUsers_UserId",
                table: "CaseHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseHistories_Cases_CaseId",
                table: "CaseHistories",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseHistories_AspNetUsers_UserId",
                table: "CaseHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseHistories_Cases_CaseId",
                table: "CaseHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CaseHistories",
                table: "CaseHistories");

            migrationBuilder.RenameTable(
                name: "CaseHistories",
                newName: "CaseHistory");

            migrationBuilder.RenameIndex(
                name: "IX_CaseHistories_UserId",
                table: "CaseHistory",
                newName: "IX_CaseHistory_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CaseHistories_CaseId",
                table: "CaseHistory",
                newName: "IX_CaseHistory_CaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CaseHistory",
                table: "CaseHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseHistory_AspNetUsers_UserId",
                table: "CaseHistory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseHistory_Cases_CaseId",
                table: "CaseHistory",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
