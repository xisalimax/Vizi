using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class TornarUsuarioOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Usuario_IdUsuario",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_IdUsuario",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Servicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_IdUsuario",
                table: "Servicos",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Usuario_IdUsuario",
                table: "Servicos",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
