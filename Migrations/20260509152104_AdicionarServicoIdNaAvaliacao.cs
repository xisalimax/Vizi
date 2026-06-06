using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarServicoIdNaAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicoId",
                table: "Avaliacao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "Avaliacao");
        }
    }
}
