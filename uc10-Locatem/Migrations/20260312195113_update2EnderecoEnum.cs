using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class update2EnderecoEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Complemente",
                table: "Endereco",
                newName: "Complemento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Complemento",
                table: "Endereco",
                newName: "Complemente");
        }
    }
}
