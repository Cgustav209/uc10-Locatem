using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class TornarReservaIdOpcionalEmAluguel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alugueis_Reserva_ReservaId",
                table: "Alugueis");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Alugueis",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Alugueis_Reserva_ReservaId",
                table: "Alugueis",
                column: "ReservaId",
                principalTable: "Reserva",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alugueis_Reserva_ReservaId",
                table: "Alugueis");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Alugueis",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alugueis_Reserva_ReservaId",
                table: "Alugueis",
                column: "ReservaId",
                principalTable: "Reserva",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
