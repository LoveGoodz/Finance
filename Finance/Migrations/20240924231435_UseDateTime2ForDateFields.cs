using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Migrations
{
    /// <inheritdoc />
    public partial class UseDateTime2ForDateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
