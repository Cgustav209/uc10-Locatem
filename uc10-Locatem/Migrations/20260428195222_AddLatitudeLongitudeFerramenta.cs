using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeFerramenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoUsuario",
                table: "UsuarioPerfis");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Ferramenta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Ferramenta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Ferramenta");

            migrationBuilder.AddColumn<string>(
                name: "TipoUsuario",
                table: "UsuarioPerfis",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
