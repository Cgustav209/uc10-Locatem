using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosCompletosFerramenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantidadeDisponivel",
                table: "Ferramenta",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "EstadoConservacao",
                table: "Ferramenta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FonteAlimentacao",
                table: "Ferramenta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EspecificacoesTecnicasJson",
                table: "Ferramenta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "TipoAprovacao",
                table: "Ferramenta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "manual");

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "Ferramenta",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ferramenta_EnderecoId",
                table: "Ferramenta",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ferramenta_Endereco_EnderecoId",
                table: "Ferramenta",
                column: "EnderecoId",
                principalTable: "Endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.CreateIndex(
                name: "IX_BloqueioDisponibilidade_FerramentaId",
                table: "BloqueioDisponibilidade",
                column: "FerramentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloqueioDisponibilidade_Ferramenta_FerramentaId",
                table: "BloqueioDisponibilidade",
                column: "FerramentaId",
                principalTable: "Ferramenta",
                principalColumn: "FerramentaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloqueioDisponibilidade_Ferramenta_FerramentaId",
                table: "BloqueioDisponibilidade");

            migrationBuilder.DropForeignKey(
                name: "FK_Ferramenta_Endereco_EnderecoId",
                table: "Ferramenta");

            migrationBuilder.DropIndex(
                name: "IX_BloqueioDisponibilidade_FerramentaId",
                table: "BloqueioDisponibilidade");

            migrationBuilder.DropIndex(
                name: "IX_Ferramenta_EnderecoId",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "QuantidadeDisponivel",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "EstadoConservacao",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "FonteAlimentacao",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "EspecificacoesTecnicasJson",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "TipoAprovacao",
                table: "Ferramenta");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Ferramenta");
        }
    }
}
