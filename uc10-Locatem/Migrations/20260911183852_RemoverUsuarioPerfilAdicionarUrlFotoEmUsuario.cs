using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class RemoverUsuarioPerfilAdicionarUrlFotoEmUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioPerfis");

            migrationBuilder.AddColumn<string>(
                name: "UrlFoto",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlFoto",
                table: "Usuario");

            migrationBuilder.CreateTable(
                name: "UsuarioPerfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId1 = table.Column<int>(type: "int", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlFoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPerfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioPerfis_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioPerfis_Usuario_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfis_UsuarioId",
                table: "UsuarioPerfis",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfis_UsuarioId1",
                table: "UsuarioPerfis",
                column: "UsuarioId1");
        }
    }
}
