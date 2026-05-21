using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ong.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCpfIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Cpf",
                table: "Users",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Cpf",
                table: "Users");
        }
    }
}
