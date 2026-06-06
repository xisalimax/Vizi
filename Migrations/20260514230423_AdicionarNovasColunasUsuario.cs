using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNovasColunasUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Inclusao",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sobre",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonePessoal",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneServico",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_UsuarioId",
                table: "Servicos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_ServicoId",
                table: "Avaliacao",
                column: "ServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Servicos_ServicoId",
                table: "Avaliacao",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Usuario_UsuarioId",
                table: "Servicos",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Servicos_ServicoId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Usuario_UsuarioId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_UsuarioId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacao_ServicoId",
                table: "Avaliacao");

            migrationBuilder.DropColumn(
                name: "Inclusao",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Sobre",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TelefonePessoal",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TelefoneServico",
                table: "Usuario");
        }
    }
}
