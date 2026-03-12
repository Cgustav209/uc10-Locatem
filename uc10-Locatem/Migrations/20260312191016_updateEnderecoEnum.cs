using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class updateEnderecoEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoEndereco",
                table: "Endereco",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoEndereco",
                table: "Endereco");
        }
    }
}
