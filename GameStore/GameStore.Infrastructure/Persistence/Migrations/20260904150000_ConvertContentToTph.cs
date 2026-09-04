using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertContentToTph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Cria as novas colunas em PG_Content (a tabela compartilhada do TPH) ainda como
            //    opcionais, para podermos popular os dados existentes antes de torná-las obrigatórias.
            migrationBuilder.AddColumn<Guid>(
                name: "StudioId",
                table: "PG_Content",
                type: "RAW(16)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "PG_Content",
                type: "NVARCHAR2(32)",
                maxLength: 32,
                nullable: true);

            // 2) Preserva os dados que hoje estão em PG_Games antes de a tabela ser descartada.
            migrationBuilder.Sql(@"
                UPDATE ""PG_Content"" c
                SET ""StudioId"" = (SELECT g.""StudioId"" FROM ""PG_Games"" g WHERE g.""Id"" = c.""Id""),
                    ""Type"" = 'Game'
                WHERE EXISTS (SELECT 1 FROM ""PG_Games"" g WHERE g.""Id"" = c.""Id"")
            ");

            // 3) Hoje Game é o único subtipo de Content, então qualquer linha que não tenha
            //    correspondência em PG_Games (não deveria existir, mas por segurança) também
            //    recebe o discriminador padrão para não ficar nula.
            migrationBuilder.Sql(@"UPDATE ""PG_Content"" SET ""Type"" = 'Game' WHERE ""Type"" IS NULL");

            // 4) Agora que todas as linhas têm um valor, a coluna discriminadora pode virar obrigatória.
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PG_Content",
                type: "NVARCHAR2(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(32)",
                oldMaxLength: 32,
                oldNullable: true);

            // 5) Remove a tabela separada de Game (mapeamento TPT antigo).
            migrationBuilder.DropForeignKey(
                name: "FK_PG_Games_PG_Content_Id",
                table: "PG_Games");

            migrationBuilder.DropForeignKey(
                name: "FK_PG_Games_PG_Studios_StudioId",
                table: "PG_Games");

            migrationBuilder.DropTable(
                name: "PG_Games");

            // 6) Recria o índice e a FK de StudioId agora na própria PG_Content.
            migrationBuilder.CreateIndex(
                name: "IX_PG_Content_StudioId",
                table: "PG_Content",
                column: "StudioId");

            migrationBuilder.AddForeignKey(
                name: "FK_PG_Content_PG_Studios_StudioId",
                table: "PG_Content",
                column: "StudioId",
                principalTable: "PG_Studios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PG_Content_PG_Studios_StudioId",
                table: "PG_Content");

            migrationBuilder.DropIndex(
                name: "IX_PG_Content_StudioId",
                table: "PG_Content");

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

            migrationBuilder.Sql(@"
                INSERT INTO ""PG_Games"" (""Id"", ""StudioId"")
                SELECT ""Id"", ""StudioId"" FROM ""PG_Content"" WHERE ""Type"" = 'Game'
            ");

            migrationBuilder.CreateIndex(
                name: "IX_PG_Games_StudioId",
                table: "PG_Games",
                column: "StudioId");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PG_Content");

            migrationBuilder.DropColumn(
                name: "StudioId",
                table: "PG_Content");
        }
    }
}
