using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace venteenligne.Migrations
{
    /// <inheritdoc />
    public partial class _3lallah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produits_Categories_IDC",
                table: "Produits");

            migrationBuilder.DropForeignKey(
                name: "FK_Produits_Proprietaires_IDP",
                table: "Produits");

            migrationBuilder.DropIndex(
                name: "IX_Produits_IDC",
                table: "Produits");

            migrationBuilder.DropIndex(
                name: "IX_Produits_IDP",
                table: "Produits");

            migrationBuilder.AlterColumn<string>(
                name: "IDP",
                table: "Produits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IDP",
                table: "Produits",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_IDC",
                table: "Produits",
                column: "IDC");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_IDP",
                table: "Produits",
                column: "IDP");

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_Categories_IDC",
                table: "Produits",
                column: "IDC",
                principalTable: "Categories",
                principalColumn: "CategorieID");

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_Proprietaires_IDP",
                table: "Produits",
                column: "IDP",
                principalTable: "Proprietaires",
                principalColumn: "INterID");
        }
    }
}
