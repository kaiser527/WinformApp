using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WinFormApp.Migrations
{
    public partial class SeedAllData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Module = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
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
                name: "Accounts",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    PassWord = table.Column<string>(maxLength: 1000, nullable: false),
                    Image = table.Column<string>(nullable: false),
                    IdRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
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
                    Status = table.Column<int>(nullable: false),
                    Discount = table.Column<float>(nullable: false)
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
                table: "FoodCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Coffee" },
                    { 2, "Tea" },
                    { 3, "Smoothies" },
                    { 4, "Pastries" },
                    { 5, "Sandwiches" },
                    { 6, "Juices" },
                    { 7, "Ice Cream" },
                    { 8, "Specials" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Module", "Name" },
                values: new object[,]
                {
                    { 15, "Table", "Delete Table" },
                    { 16, "Table", "View Table" },
                    { 17, "Role", "Create Role" },
                    { 18, "Role", "Update Role" },
                    { 19, "Role", "Delete Role" },
                    { 24, "Permission", "View Permission" },
                    { 22, "Permission", "Update Permission" },
                    { 23, "Permission", "Delete Permission" },
                    { 14, "Table", "Update Table" },
                    { 25, "Bill", "View Bill" },
                    { 21, "Permission", "Create Permission" },
                    { 13, "Table", "Create Table" },
                    { 20, "Role", "View Role" },
                    { 11, "Category", "Delete Category" },
                    { 12, "Category", "View Category" },
                    { 2, "Account", "Update Account" },
                    { 3, "Account", "Delete Account" },
                    { 4, "Account", "View Account" },
                    { 5, "Food", "Create Food" },
                    { 1, "Account", "Create Account" },
                    { 6, "Food", "Update Food" },
                    { 7, "Food", "Delete Food" },
                    { 8, "Food", "View Food" },
                    { 9, "Category", "Create Category" },
                    { 10, "Category", "Update Category" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Admin" },
                    { 2, true, "Staff" },
                    { 3, true, "Tester" }
                });

            migrationBuilder.InsertData(
                table: "TableFoods",
                columns: new[] { "Id", "Name", "Status" },
                values: new object[,]
                {
                    { 12, "Table 12", "Empty" },
                    { 13, "Table 13", "Empty" },
                    { 14, "Table 14", "Empty" },
                    { 17, "Table 17", "Empty" },
                    { 16, "Table 16", "Empty" },
                    { 18, "Table 18", "Empty" },
                    { 11, "Table 11", "Empty" },
                    { 15, "Table 15", "Empty" },
                    { 10, "Table 10", "Empty" },
                    { 3, "Table 3", "Empty" },
                    { 8, "Table 8", "Empty" },
                    { 7, "Table 7", "Empty" },
                    { 6, "Table 6", "Empty" },
                    { 5, "Table 5", "Empty" },
                    { 4, "Table 4", "Empty" },
                    { 2, "Table 2", "Empty" },
                    { 1, "Table 1", "Empty" },
                    { 19, "Table 19", "Empty" },
                    { 9, "Table 9", "Empty" },
                    { 20, "Table 20", "Empty" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "UserName", "DisplayName", "IdRole", "Image", "PassWord" },
                values: new object[,]
                {
                    { "john_doe", "John Doe", 2, "default.png", "$2a$11$4eVxgQjWinRYtFEX3jOCueVLxitmyIJac.ugRs41Pd73kZJZ0yjyu" },
                    { "jane_smith", "Jane Smith", 2, "default.png", "$2a$11$HsCjq0Ab4fEMAckVp0z1mefmnchEYQr7qANwnEsL1GXxCPNfijA9K" },
                    { "matthew", "Matthew Garcia", 3, "default.png", "$2a$11$BRZ6W5/zCZVrkg/xDO0qCew9shHaJCMILRhybpSZVTj65Krwz/UO2" },
                    { "sarah", "Sarah Brown", 3, "default.png", "$2a$11$FLykWMDQO9B45TFiAWv3SedhhHeacvvw3bF/XVaDFALCqXvgAe.S." },
                    { "emily", "Emily Davis", 3, "default.png", "$2a$11$5w/QCn8sKEcfZetuOFUuoubtapNY6cd1h7NFhFNv4bI3JPjc2lY2." },
                    { "admin", "Administrator", 1, "default.png", "$2a$11$xBVSHCBOsjOpq02M/WxobO9KBNpvn8mzopvb9qWCH2qcWjwlhZPM." },
                    { "chris", "Chris Lee", 2, "default.png", "$2a$11$A/DvfNP1jmJ/5wcRczACAezvRnWYAgZHkmDPBChUlYSxItpnAsU8i" },
                    { "david", "David Wilson", 2, "default.png", "$2a$11$jgvFx2FMGPrG.VaH/DW81.1IY4i7KMpYC1MIuMZcXfeLM9qtX8.x6" },
                    { "michael", "Michael Johnson", 2, "default.png", "$2a$11$72/9A/FV9ncpkBOelC147eg0qd1DipdhQRJe6KeVK03kRmsAzxHA6" },
                    { "amanda", "Amanda Miller", 2, "default.png", "$2a$11$ytZCOayl8AZCpW7inmNYGurpCvONM2i2q5ML4c9Nmk4Olim7KEAyC" }
                });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "IdCategory", "Name", "Price" },
                values: new object[,]
                {
                    { 20, 6, "Orange Juice", 3.0 },
                    { 26, 8, "Affogato", 5.5 },
                    { 25, 7, "Strawberry Ice Cream", 2.5 },
                    { 24, 7, "Chocolate Ice Cream", 2.5 },
                    { 23, 7, "Vanilla Ice Cream", 2.5 },
                    { 22, 6, "Carrot Juice", 3.2000000000000002 },
                    { 21, 6, "Apple Juice", 3.0 },
                    { 19, 5, "Veggie Sandwich", 3.7999999999999998 },
                    { 27, 8, "Irish Coffee", 6.0 },
                    { 17, 5, "Ham Sandwich", 4.0 },
                    { 2, 1, "Latte", 3.5 },
                    { 3, 1, "Cappuccino", 3.7999999999999998 },
                    { 4, 1, "Americano", 2.7999999999999998 },
                    { 5, 2, "Green Tea", 2.0 },
                    { 6, 2, "Black Tea", 2.0 },
                    { 7, 2, "Herbal Tea", 2.5 },
                    { 8, 2, "Milk Tea", 3.0 },
                    { 18, 5, "Chicken Sandwich", 4.5 },
                    { 1, 1, "Espresso", 2.5 },
                    { 16, 4, "Chocolate Cake", 3.7999999999999998 },
                    { 15, 4, "Cheesecake", 3.5 },
                    { 14, 4, "Muffin", 2.0 },
                    { 13, 4, "Croissant", 2.5 },
                    { 9, 3, "Mango Smoothie", 4.5 },
                    { 12, 3, "Avocado Smoothie", 5.0 },
                    { 11, 3, "Banana Smoothie", 4.0 },
                    { 10, 3, "Strawberry Smoothie", 4.5 }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { 3, 14 },
                    { 2, 16 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 3 },
                    { 3, 4 },
                    { 3, 5 },
                    { 3, 6 },
                    { 3, 7 },
                    { 3, 9 },
                    { 3, 10 },
                    { 3, 11 },
                    { 3, 12 },
                    { 3, 13 },
                    { 3, 15 },
                    { 3, 8 },
                    { 1, 12 },
                    { 1, 24 },
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 8 },
                    { 1, 9 },
                    { 1, 10 },
                    { 1, 11 },
                    { 3, 16 },
                    { 1, 13 },
                    { 1, 14 },
                    { 1, 15 },
                    { 1, 16 },
                    { 1, 17 },
                    { 1, 18 },
                    { 1, 19 },
                    { 1, 20 },
                    { 1, 21 },
                    { 1, 22 },
                    { 1, 23 },
                    { 1, 25 },
                    { 3, 25 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IdRole",
                table: "Accounts",
                column: "IdRole");

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

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BillInfos");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TableFoods");

            migrationBuilder.DropTable(
                name: "FoodCategories");
        }
    }
}
