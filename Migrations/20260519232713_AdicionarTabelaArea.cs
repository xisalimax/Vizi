using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ViziLogin.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id_Area = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id_Area);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
