using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    IdAvaliacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeAvaliador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nota = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.IdAvaliacao);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");
        }
    }
}
