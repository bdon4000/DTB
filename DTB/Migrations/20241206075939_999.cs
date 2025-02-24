using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTB.Migrations
{
    /// <inheritdoc />
    public partial class _999 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppearanceInspectionDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppearanceInspectionDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatteryRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JellyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FilmCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeadingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeadingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiInsertingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JellyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiInsertingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BottomWelding1Datas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottomWelding1Datas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BottomWelding2Datas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BottomWelding2Datas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapWeldingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapWeldingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilmShrinkingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmShrinkingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InjectingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjectingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InkjetPrintingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FilmCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InkjetPrintingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JellyFedingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JellyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JellyFedingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NeckingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeckingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlasticFilmingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlasticFilmingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreChargeDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilmCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreChargeDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SealingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SealingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShellInsertingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JellyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShellInsertingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortCircuitTestDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortCircuitTestDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiInsertingDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiInsertingDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XRAYDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
                    ngReason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    uploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Param1 = table.Column<float>(type: "real", nullable: true),
                    Param2 = table.Column<float>(type: "real", nullable: true),
                    Param3 = table.Column<float>(type: "real", nullable: true),
                    Param4 = table.Column<float>(type: "real", nullable: true),
                    Param5 = table.Column<float>(type: "real", nullable: true),
                    Param6 = table.Column<float>(type: "real", nullable: true),
                    Param7 = table.Column<float>(type: "real", nullable: true),
                    Param8 = table.Column<float>(type: "real", nullable: true),
                    Param9 = table.Column<float>(type: "real", nullable: true),
                    StringParam1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringParam2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShellCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XRAYDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmCode",
                table: "AppearanceInspectionDatas",
                column: "FilmCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatteryRelations_FilmCode",
                table: "BatteryRelations",
                column: "FilmCode");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryRelations_JellyCode",
                table: "BatteryRelations",
                column: "JellyCode");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryRelations_ShellCode",
                table: "BatteryRelations",
                column: "ShellCode");

            migrationBuilder.CreateIndex(
                name: "IX_BeadingDatas_ShellCode",
                table: "BeadingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiInsertingDatas_JellyCode",
                table: "BiInsertingDatas",
                column: "JellyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BottomWelding1Datas_ShellCode",
                table: "BottomWelding1Datas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BottomWelding2Datas_ShellCode",
                table: "BottomWelding2Datas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapWeldingDatas_ShellCode",
                table: "CapWeldingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilmShrinkingDatas_ShellCode",
                table: "FilmShrinkingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InjectingDatas_ShellCode",
                table: "InjectingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InkjetPrintingDatas_ShellCode",
                table: "InkjetPrintingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JellyFedingDatas_JellyCode",
                table: "JellyFedingDatas",
                column: "JellyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NeckingDatas_ShellCode",
                table: "NeckingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlasticFilmingDatas_ShellCode",
                table: "PlasticFilmingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilmCode",
                table: "PreChargeDatas",
                column: "FilmCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SealingDatas_ShellCode",
                table: "SealingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShellInsertingDatas_JellyCode",
                table: "ShellInsertingDatas",
                column: "JellyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortCircuitTestDatas_ShellCode",
                table: "ShortCircuitTestDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiInsertingDatas_ShellCode",
                table: "TiInsertingDatas",
                column: "ShellCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XRAYDatas_ShellCode",
                table: "XRAYDatas",
                column: "ShellCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppearanceInspectionDatas");

            migrationBuilder.DropTable(
                name: "BatteryRelations");

            migrationBuilder.DropTable(
                name: "BeadingDatas");

            migrationBuilder.DropTable(
                name: "BiInsertingDatas");

            migrationBuilder.DropTable(
                name: "BottomWelding1Datas");

            migrationBuilder.DropTable(
                name: "BottomWelding2Datas");

            migrationBuilder.DropTable(
                name: "CapWeldingDatas");

            migrationBuilder.DropTable(
                name: "FilmShrinkingDatas");

            migrationBuilder.DropTable(
                name: "InjectingDatas");

            migrationBuilder.DropTable(
                name: "InkjetPrintingDatas");

            migrationBuilder.DropTable(
                name: "JellyFedingDatas");

            migrationBuilder.DropTable(
                name: "NeckingDatas");

            migrationBuilder.DropTable(
                name: "PlasticFilmingDatas");

            migrationBuilder.DropTable(
                name: "PreChargeDatas");

            migrationBuilder.DropTable(
                name: "SealingDatas");

            migrationBuilder.DropTable(
                name: "ShellInsertingDatas");

            migrationBuilder.DropTable(
                name: "ShortCircuitTestDatas");

            migrationBuilder.DropTable(
                name: "TiInsertingDatas");

            migrationBuilder.DropTable(
                name: "XRAYDatas");
        }
    }
}
