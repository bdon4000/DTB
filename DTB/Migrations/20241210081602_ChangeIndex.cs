using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTB.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InkjetPrintingDatas_ShellCode",
                table: "InkjetPrintingDatas");

            migrationBuilder.CreateIndex(
                name: "IX_InkjetPrintingDatas_FilmCode",
                table: "InkjetPrintingDatas",
                column: "FilmCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InkjetPrintingDatas_FilmCode",
                table: "InkjetPrintingDatas");

            migrationBuilder.CreateIndex(
                name: "IX_InkjetPrintingDatas_ShellCode",
                table: "InkjetPrintingDatas",
                column: "ShellCode",
                unique: true);
        }
    }
}
