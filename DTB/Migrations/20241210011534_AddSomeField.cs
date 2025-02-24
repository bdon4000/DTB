using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTB.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Param10",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param11",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param12",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param13",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param14",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param15",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param16",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param17",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param18",
                table: "PlasticFilmingDatas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stringParam3",
                table: "JellyFedingDatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param10",
                table: "BottomWelding2Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param11",
                table: "BottomWelding2Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param12",
                table: "BottomWelding2Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param13",
                table: "BottomWelding2Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param14",
                table: "BottomWelding2Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param10",
                table: "BottomWelding1Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param11",
                table: "BottomWelding1Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param12",
                table: "BottomWelding1Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param13",
                table: "BottomWelding1Datas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Param14",
                table: "BottomWelding1Datas",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param10",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param11",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param12",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param13",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param14",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param15",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param16",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param17",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "Param18",
                table: "PlasticFilmingDatas");

            migrationBuilder.DropColumn(
                name: "stringParam3",
                table: "JellyFedingDatas");

            migrationBuilder.DropColumn(
                name: "Param10",
                table: "BottomWelding2Datas");

            migrationBuilder.DropColumn(
                name: "Param11",
                table: "BottomWelding2Datas");

            migrationBuilder.DropColumn(
                name: "Param12",
                table: "BottomWelding2Datas");

            migrationBuilder.DropColumn(
                name: "Param13",
                table: "BottomWelding2Datas");

            migrationBuilder.DropColumn(
                name: "Param14",
                table: "BottomWelding2Datas");

            migrationBuilder.DropColumn(
                name: "Param10",
                table: "BottomWelding1Datas");

            migrationBuilder.DropColumn(
                name: "Param11",
                table: "BottomWelding1Datas");

            migrationBuilder.DropColumn(
                name: "Param12",
                table: "BottomWelding1Datas");

            migrationBuilder.DropColumn(
                name: "Param13",
                table: "BottomWelding1Datas");

            migrationBuilder.DropColumn(
                name: "Param14",
                table: "BottomWelding1Datas");
        }
    }
}
