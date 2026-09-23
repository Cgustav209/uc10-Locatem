using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "CategoriaPaiId", "EhPadrao", "nome" },
                values: new object[,]
                {
                    { 1, null, true, "Ferramentas Elétricas • Parafusadeira/Furadeira" },
                    { 2, null, true, "Ferramentas Elétricas • Corte e Desgaste" },
                    { 3, null, true, "Ferramentas Elétricas • Pintura" },
                    { 4, null, true, "Ferramentas Manuais" },
                    { 5, null, true, "Jardinagem e Paisagismo" },
                    { 6, null, true, "Construção e Alvenaria" },
                    { 7, null, true, "Elevação e Transporte" },
                    { 8, null, true, "Limpeza e Lavagem" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
