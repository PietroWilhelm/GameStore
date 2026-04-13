using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PG_Content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR2(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR2(1000)", maxLength: 1000, nullable: false),
                    LaunchDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ContentType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Content", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PG_Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR2(255)", maxLength: 255, nullable: false),
                    Cpf = table.Column<string>(type: "VARCHAR2(11)", maxLength: 11, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Password = table.Column<string>(type: "VARCHAR2(500)", maxLength: 500, nullable: false),
                    Salt = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PG_Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "VARCHAR2(500)", maxLength: 500, nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PG_Studios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR2(100)", maxLength: 100, nullable: false),
                    FoundationDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PG_Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PG_Orders_PG_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "PG_Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PG_UserConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Theme = table.Column<string>(type: "VARCHAR2(50)", maxLength: 50, nullable: false, defaultValue: "Dark"),
                    EnableNotifications = table.Column<bool>(type: "CHAR(1)", nullable: false, defaultValue: "0"),
                    CustomerId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Active = table.Column<bool>(type: "CHAR(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_UserConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PG_UserConfigurations_PG_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "PG_Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentGenres",
                columns: table => new
                {
                    ContentsId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GenresId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGenres", x => new { x.ContentsId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_ContentGenres_PG_Content_ContentsId",
                        column: x => x.ContentsId,
                        principalTable: "PG_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentGenres_PG_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "PG_Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PG_Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    StudioId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PG_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PG_Games_PG_Content_Id",
                        column: x => x.Id,
                        principalTable: "PG_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PG_Games_PG_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "PG_Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentGenres_GenresId",
                table: "ContentGenres",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_PG_Customers_Email",
                table: "PG_Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PG_Games_StudioId",
                table: "PG_Games",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_PG_Orders_CustomerId",
                table: "PG_Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PG_UserConfigurations_CustomerId",
                table: "PG_UserConfigurations",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentGenres");

            migrationBuilder.DropTable(
                name: "PG_Games");

            migrationBuilder.DropTable(
                name: "PG_Orders");

            migrationBuilder.DropTable(
                name: "PG_UserConfigurations");

            migrationBuilder.DropTable(
                name: "PG_Genres");

            migrationBuilder.DropTable(
                name: "PG_Content");

            migrationBuilder.DropTable(
                name: "PG_Studios");

            migrationBuilder.DropTable(
                name: "PG_Customers");
        }
    }
}
