using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class InitialSQLite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id_Area = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id_Area);
                });

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioDestino = table.Column<string>(type: "TEXT", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: false),
                    DataNotificacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Lida = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServicoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Senha = table.Column<string>(type: "TEXT", nullable: true),
                    Tipo_Perfil = table.Column<string>(type: "TEXT", nullable: true),
                    Id_Area = table.Column<int>(type: "INTEGER", nullable: true),
                    TelefonePessoal = table.Column<string>(type: "TEXT", nullable: true),
                    TelefoneServico = table.Column<string>(type: "TEXT", nullable: true),
                    Sobre = table.Column<string>(type: "TEXT", nullable: true),
                    Inclusao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Areas_Id_Area",
                        column: x => x.Id_Area,
                        principalTable: "Areas",
                        principalColumn: "Id_Area");
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeServico = table.Column<string>(type: "TEXT", nullable: false),
                    Profissional = table.Column<string>(type: "TEXT", nullable: false),
                    TipoServico = table.Column<string>(type: "TEXT", nullable: false),
                    Contato = table.Column<string>(type: "TEXT", nullable: false),
                    Preco = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicos_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    IdAvaliacao = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeAvaliador = table.Column<string>(type: "TEXT", nullable: false),
                    Nota = table.Column<decimal>(type: "decimal(3, 1)", nullable: false),
                    Comentario = table.Column<string>(type: "TEXT", nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ServicoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.IdAvaliacao);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Denuncias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", nullable: false),
                    NomeUsuario = table.Column<string>(type: "TEXT", nullable: false),
                    DataDenuncia = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denuncias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Denuncias_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id_Area", "Nome" },
                values: new object[,]
                {
                    { 1, "Barreiro" },
                    { 2, "Centro-Sul" },
                    { 3, "Leste" },
                    { 4, "Nordeste" },
                    { 5, "Noroeste" },
                    { 6, "Norte" },
                    { 7, "Oeste" },
                    { 8, "Pampulha" },
                    { 9, "Venda Nova" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_ServicoId",
                table: "Avaliacao",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_ServicoId",
                table: "Denuncias",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_UsuarioId",
                table: "Servicos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Id_Area",
                table: "Usuario",
                column: "Id_Area");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "Denuncias");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
