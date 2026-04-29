using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacionamentoCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaPaiId",
                table: "Categorias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EhPadrao",
                table: "Categorias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_CategoriaPaiId",
                table: "Categorias",
                column: "CategoriaPaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPaiId",
                table: "Categorias",
                column: "CategoriaPaiId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Categorias_CategoriaPaiId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_CategoriaPaiId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "CategoriaPaiId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "EhPadrao",
                table: "Categorias");
        }
    }
}
