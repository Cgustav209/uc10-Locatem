using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class AtualizandoFerramenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Ferramenta");

            migrationBuilder.AddColumn<int>(
                name: "Disponibilidade",
                table: "Ferramenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Ferramenta",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponibilidade",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Ferramenta");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Ferramenta",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
