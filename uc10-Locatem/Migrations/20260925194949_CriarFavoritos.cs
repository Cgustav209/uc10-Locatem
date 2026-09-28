using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class CriarFavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FerramentaId = table.Column<int>(type: "int", nullable: false),
                    DataFavorito = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoritos_Ferramenta_FerramentaId",
                        column: x => x.FerramentaId,
                        principalTable: "Ferramenta",
                        principalColumn: "FerramentaId");
                    table.ForeignKey(
                        name: "FK_Favoritos_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_FerramentaId",
                table: "Favoritos",
                column: "FerramentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioId_FerramentaId",
                table: "Favoritos",
                columns: new[] { "UsuarioId", "FerramentaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favoritos");
        }
    }
}
