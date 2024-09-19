using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActTrans_Customers_CustomerID",
                table: "ActTrans");

            migrationBuilder.DropForeignKey(
                name: "FK_ActTrans_Invoices_InvoiceID",
                table: "ActTrans");

            migrationBuilder.DropForeignKey(
                name: "FK_Balances_Companies_CompanyID",
                table: "Balances");

            migrationBuilder.DropForeignKey(
                name: "FK_Balances_Customers_CustomerID",
                table: "Balances");

            migrationBuilder.DropForeignKey(
                name: "FK_Balances_Stocks_StockID",
                table: "Balances");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Companies_CompanyID",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Stocks_StockID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Companies_CompanyID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Customers_CustomerID",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Companies_CompanyID",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTrans_InvoiceDetails_InvoiceDetailsID",
                table: "StockTrans");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTrans_Stocks_StockID",
                table: "StockTrans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTrans",
                table: "StockTrans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Balances",
                table: "Balances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActTrans",
                table: "ActTrans");

            migrationBuilder.RenameTable(
                name: "StockTrans",
                newName: "StockTran");

            migrationBuilder.RenameTable(
                name: "Stocks",
                newName: "Stock");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.RenameTable(
                name: "Balances",
                newName: "Balance");

            migrationBuilder.RenameTable(
                name: "ActTrans",
                newName: "ActTran");

            migrationBuilder.RenameIndex(
                name: "IX_StockTrans_StockID",
                table: "StockTran",
                newName: "IX_StockTran_StockID");

            migrationBuilder.RenameIndex(
                name: "IX_StockTrans_InvoiceDetailsID",
                table: "StockTran",
                newName: "IX_StockTran_InvoiceDetailsID");

            migrationBuilder.RenameIndex(
                name: "IX_Stocks_CompanyID",
                table: "Stock",
                newName: "IX_Stock_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CustomerID",
                table: "Invoice",
                newName: "IX_Invoice_CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CompanyID",
                table: "Invoice",
                newName: "IX_Invoice_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_CompanyID",
                table: "Customer",
                newName: "IX_Customer_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Balances_StockID",
                table: "Balance",
                newName: "IX_Balance_StockID");

            migrationBuilder.RenameIndex(
                name: "IX_Balances_CustomerID",
                table: "Balance",
                newName: "IX_Balance_CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Balances_CompanyID",
                table: "Balance",
                newName: "IX_Balance_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_ActTrans_InvoiceID",
                table: "ActTran",
                newName: "IX_ActTran_InvoiceID");

            migrationBuilder.RenameIndex(
                name: "IX_ActTrans_CustomerID",
                table: "ActTran",
                newName: "IX_ActTran_CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTran",
                table: "StockTran",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stock",
                table: "Stock",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Balance",
                table: "Balance",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActTran",
                table: "ActTran",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ActTran_Customer_CustomerID",
                table: "ActTran",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActTran_Invoice_InvoiceID",
                table: "ActTran",
                column: "InvoiceID",
                principalTable: "Invoice",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balance_Company_CompanyID",
                table: "Balance",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balance_Customer_CustomerID",
                table: "Balance",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balance_Stock_StockID",
                table: "Balance",
                column: "StockID",
                principalTable: "Stock",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Company_CompanyID",
                table: "Customer",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Customer_CustomerID",
                table: "Invoice",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoice_InvoiceID",
                table: "InvoiceDetails",
                column: "InvoiceID",
                principalTable: "Invoice",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Stock_StockID",
                table: "InvoiceDetails",
                column: "StockID",
                principalTable: "Stock",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Company_CompanyID",
                table: "Stock",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTran_InvoiceDetails_InvoiceDetailsID",
                table: "StockTran",
                column: "InvoiceDetailsID",
                principalTable: "InvoiceDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTran_Stock_StockID",
                table: "StockTran",
                column: "StockID",
                principalTable: "Stock",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActTran_Customer_CustomerID",
                table: "ActTran");

            migrationBuilder.DropForeignKey(
                name: "FK_ActTran_Invoice_InvoiceID",
                table: "ActTran");

            migrationBuilder.DropForeignKey(
                name: "FK_Balance_Company_CompanyID",
                table: "Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_Balance_Customer_CustomerID",
                table: "Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_Balance_Stock_StockID",
                table: "Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Company_CompanyID",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Company_CompanyID",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Customer_CustomerID",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoice_InvoiceID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Stock_StockID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Company_CompanyID",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTran_InvoiceDetails_InvoiceDetailsID",
                table: "StockTran");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTran_Stock_StockID",
                table: "StockTran");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTran",
                table: "StockTran");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stock",
                table: "Stock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Balance",
                table: "Balance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActTran",
                table: "ActTran");

            migrationBuilder.RenameTable(
                name: "StockTran",
                newName: "StockTrans");

            migrationBuilder.RenameTable(
                name: "Stock",
                newName: "Stocks");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.RenameTable(
                name: "Balance",
                newName: "Balances");

            migrationBuilder.RenameTable(
                name: "ActTran",
                newName: "ActTrans");

            migrationBuilder.RenameIndex(
                name: "IX_StockTran_StockID",
                table: "StockTrans",
                newName: "IX_StockTrans_StockID");

            migrationBuilder.RenameIndex(
                name: "IX_StockTran_InvoiceDetailsID",
                table: "StockTrans",
                newName: "IX_StockTrans_InvoiceDetailsID");

            migrationBuilder.RenameIndex(
                name: "IX_Stock_CompanyID",
                table: "Stocks",
                newName: "IX_Stocks_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CustomerID",
                table: "Invoices",
                newName: "IX_Invoices_CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CompanyID",
                table: "Invoices",
                newName: "IX_Invoices_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_CompanyID",
                table: "Customers",
                newName: "IX_Customers_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_Balance_StockID",
                table: "Balances",
                newName: "IX_Balances_StockID");

            migrationBuilder.RenameIndex(
                name: "IX_Balance_CustomerID",
                table: "Balances",
                newName: "IX_Balances_CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Balance_CompanyID",
                table: "Balances",
                newName: "IX_Balances_CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_ActTran_InvoiceID",
                table: "ActTrans",
                newName: "IX_ActTrans_InvoiceID");

            migrationBuilder.RenameIndex(
                name: "IX_ActTran_CustomerID",
                table: "ActTrans",
                newName: "IX_ActTrans_CustomerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTrans",
                table: "StockTrans",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Balances",
                table: "Balances",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActTrans",
                table: "ActTrans",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ActTrans_Customers_CustomerID",
                table: "ActTrans",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActTrans_Invoices_InvoiceID",
                table: "ActTrans",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balances_Companies_CompanyID",
                table: "Balances",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balances_Customers_CustomerID",
                table: "Balances",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Balances_Stocks_StockID",
                table: "Balances",
                column: "StockID",
                principalTable: "Stocks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Companies_CompanyID",
                table: "Customers",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceID",
                table: "InvoiceDetails",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Stocks_StockID",
                table: "InvoiceDetails",
                column: "StockID",
                principalTable: "Stocks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Companies_CompanyID",
                table: "Invoices",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Customers_CustomerID",
                table: "Invoices",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Companies_CompanyID",
                table: "Stocks",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTrans_InvoiceDetails_InvoiceDetailsID",
                table: "StockTrans",
                column: "InvoiceDetailsID",
                principalTable: "InvoiceDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTrans_Stocks_StockID",
                table: "StockTrans",
                column: "StockID",
                principalTable: "Stocks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
