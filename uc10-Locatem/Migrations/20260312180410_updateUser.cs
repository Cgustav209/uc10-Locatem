using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uc10_Locatem.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Usuario",
                newName: "Nome");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuario",
                newName: "Name");

            migrationBuilder.AlterColumn<bool>(
                name: "Tipo",
                table: "Usuario",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
