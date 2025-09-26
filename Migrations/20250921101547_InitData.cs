using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WinFormApp.Migrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    PassWord = table.Column<string>(maxLength: 1000, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableFoods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IdCategory = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Foods_FoodCategories_IdCategory",
                        column: x => x.IdCategory,
                        principalTable: "FoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTable = table.Column<int>(nullable: false),
                    DateCheckIn = table.Column<DateTime>(nullable: false),
                    DateCheckOut = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_TableFoods_IdTable",
                        column: x => x.IdTable,
                        principalTable: "TableFoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBill = table.Column<int>(nullable: false),
                    IdFood = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillInfos_Bills_IdBill",
                        column: x => x.IdBill,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillInfos_Foods_IdFood",
                        column: x => x.IdFood,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "UserName", "DisplayName", "PassWord", "Type" },
                values: new object[,]
                {
                    { "admin", "Administrator", "123456", 1 },
                    { "john_doe", "John Doe", "123456", 0 },
                    { "jane_smith", "Jane Smith", "123456", 0 },
                    { "michael", "Michael Johnson", "123456", 0 },
                    { "emily", "Emily Davis", "123456", 0 },
                    { "david", "David Wilson", "123456", 0 },
                    { "sarah", "Sarah Brown", "123456", 0 },
                    { "chris", "Chris Lee", "123456", 0 },
                    { "amanda", "Amanda Miller", "123456", 0 },
                    { "matthew", "Matthew Garcia", "123456", 0 }
                });

            migrationBuilder.InsertData(
                table: "FoodCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 8, "Specials" },
                    { 7, "Ice Cream" },
                    { 6, "Juices" },
                    { 5, "Sandwiches" },
                    { 4, "Pastries" },
                    { 3, "Smoothies" },
                    { 2, "Tea" },
                    { 1, "Coffee" }
                });

            migrationBuilder.InsertData(
                table: "TableFoods",
                columns: new[] { "Id", "Name", "Status" },
                values: new object[,]
                {
                    { 18, "Table 18", "Empty" },
                    { 17, "Table 17", "Empty" },
                    { 16, "Table 16", "Empty" },
                    { 15, "Table 15", "Empty" },
                    { 14, "Table 14", "Empty" },
                    { 13, "Table 13", "Empty" },
                    { 12, "Table 12", "Empty" },
                    { 11, "Table 11", "Empty" },
                    { 10, "Table 10", "Empty" },
                    { 1, "Table 1", "Empty" },
                    { 8, "Table 8", "Empty" },
                    { 7, "Table 7", "Empty" },
                    { 6, "Table 6", "Empty" },
                    { 5, "Table 5", "Empty" },
                    { 4, "Table 4", "Empty" },
                    { 3, "Table 3", "Empty" },
                    { 2, "Table 2", "Empty" },
                    { 19, "Table 19", "Empty" },
                    { 9, "Table 9", "Empty" },
                    { 20, "Table 20", "Empty" }
                });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "IdCategory", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Espresso", 2.5 },
                    { 25, 7, "Strawberry Ice Cream", 2.5 },
                    { 24, 7, "Chocolate Ice Cream", 2.5 },
                    { 23, 7, "Vanilla Ice Cream", 2.5 },
                    { 22, 6, "Carrot Juice", 3.2000000000000002 },
                    { 21, 6, "Apple Juice", 3.0 },
                    { 20, 6, "Orange Juice", 3.0 },
                    { 19, 5, "Veggie Sandwich", 3.7999999999999998 },
                    { 18, 5, "Chicken Sandwich", 4.5 },
                    { 17, 5, "Ham Sandwich", 4.0 },
                    { 16, 4, "Chocolate Cake", 3.7999999999999998 },
                    { 15, 4, "Cheesecake", 3.5 },
                    { 26, 8, "Affogato", 5.5 },
                    { 14, 4, "Muffin", 2.0 },
                    { 12, 3, "Avocado Smoothie", 5.0 },
                    { 11, 3, "Banana Smoothie", 4.0 },
                    { 10, 3, "Strawberry Smoothie", 4.5 },
                    { 9, 3, "Mango Smoothie", 4.5 },
                    { 8, 2, "Milk Tea", 3.0 },
                    { 7, 2, "Herbal Tea", 2.5 },
                    { 6, 2, "Black Tea", 2.0 },
                    { 5, 2, "Green Tea", 2.0 },
                    { 4, 1, "Americano", 2.7999999999999998 },
                    { 3, 1, "Cappuccino", 3.7999999999999998 },
                    { 2, 1, "Latte", 3.5 },
                    { 13, 4, "Croissant", 2.5 },
                    { 27, 8, "Irish Coffee", 6.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillInfos_IdBill",
                table: "BillInfos",
                column: "IdBill");

            migrationBuilder.CreateIndex(
                name: "IX_BillInfos_IdFood",
                table: "BillInfos",
                column: "IdFood");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_IdTable",
                table: "Bills",
                column: "IdTable");

            migrationBuilder.CreateIndex(
                name: "IX_Foods_IdCategory",
                table: "Foods",
                column: "IdCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BillInfos");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "TableFoods");

            migrationBuilder.DropTable(
                name: "FoodCategories");
        }
    }
}
