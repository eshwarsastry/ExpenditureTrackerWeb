using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenditureTrackerWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    TT_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TT_Name = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.TT_Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    U_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    U_FirstName = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    U_Name = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    U_Email = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    U_Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.U_Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCategories",
                columns: table => new
                {
                    TC_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC_Name = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    TC_Description = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    TC_UserU_Id = table.Column<int>(type: "int", nullable: false),
                    TC_TransactionTypeTT_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategories", x => x.TC_Id);
                    table.ForeignKey(
                        name: "FK_TransactionCategories_TransactionTypes_TC_TransactionTypeTT_Id",
                        column: x => x.TC_TransactionTypeTT_Id,
                        principalTable: "TransactionTypes",
                        principalColumn: "TT_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionCategories_Users_TC_UserU_Id",
                        column: x => x.TC_UserU_Id,
                        principalTable: "Users",
                        principalColumn: "U_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    EX_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EX_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EX_DateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EX_Note = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    EX_UserU_Id = table.Column<int>(type: "int", nullable: false),
                    EX_TransactionCategoryTC_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.EX_Id);
                    table.ForeignKey(
                        name: "FK_Expenses_TransactionCategories_EX_TransactionCategoryTC_Id",
                        column: x => x.EX_TransactionCategoryTC_Id,
                        principalTable: "TransactionCategories",
                        principalColumn: "TC_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_EX_UserU_Id",
                        column: x => x.EX_UserU_Id,
                        principalTable: "Users",
                        principalColumn: "U_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "TT_Id", "TT_Name" },
                values: new object[,]
                {
                    { 1, "Income" },
                    { 2, "Expenditure" },
                    { 3, "Transfer" },
                    { 4, "Investment" },
                    { 5, "Loan" },
                    { 6, "Refund" },
                    { 7, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EX_TransactionCategoryTC_Id",
                table: "Expenses",
                column: "EX_TransactionCategoryTC_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_EX_UserU_Id",
                table: "Expenses",
                column: "EX_UserU_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategories_TC_TransactionTypeTT_Id",
                table: "TransactionCategories",
                column: "TC_TransactionTypeTT_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategories_TC_UserU_Id",
                table: "TransactionCategories",
                column: "TC_UserU_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "TransactionCategories");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
