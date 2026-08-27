using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class projetoIntegrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Usuario_UsuarioId1",
                table: "Endereco");

            migrationBuilder.DropIndex(
                name: "IX_Endereco_UsuarioId1",
                table: "Endereco");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Endereco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId1",
                table: "Endereco",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_UsuarioId1",
                table: "Endereco",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Usuario_UsuarioId1",
                table: "Endereco",
                column: "UsuarioId1",
                principalTable: "Usuario",
                principalColumn: "Id");
        }
    }
}
