using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAA_TestApp.Data.CaaMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CAA");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ISBN = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ShelfOn = table.Column<string>(type: "TEXT", nullable: true),
                    Cost = table.Column<double>(type: "REAL", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CategoryID = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationID = table.Column<int>(type: "INTEGER", nullable: false),
                    EventID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Inventories_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalSchema: "CAA",
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventories_Locations_LocationID",
                        column: x => x.LocationID,
                        principalSchema: "CAA",
                        principalTable: "Locations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false),
                    InventoryID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventLocation = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    InventoryName = table.Column<string>(type: "TEXT", nullable: true),
                    InventoryQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    OrderedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TookBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TookOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReturnedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ReturnedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => new { x.InventoryID, x.ID });
                    table.ForeignKey(
                        name: "FK_Events_Inventories_InventoryID",
                        column: x => x.InventoryID,
                        principalSchema: "CAA",
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsPhotos",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    invID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsPhotos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemsPhotos_Inventories_invID",
                        column: x => x.invID,
                        principalSchema: "CAA",
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsThumbnails",
                schema: "CAA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    invID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsThumbnails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemsThumbnails_Inventories_invID",
                        column: x => x.invID,
                        principalSchema: "CAA",
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "CAA",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_InventoryID",
                schema: "CAA",
                table: "Events",
                column: "InventoryID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CategoryID",
                schema: "CAA",
                table: "Inventories",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_LocationID",
                schema: "CAA",
                table: "Inventories",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsPhotos_invID",
                schema: "CAA",
                table: "ItemsPhotos",
                column: "invID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemsThumbnails_invID",
                schema: "CAA",
                table: "ItemsThumbnails",
                column: "invID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Name",
                schema: "CAA",
                table: "Locations",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events",
                schema: "CAA");

            migrationBuilder.DropTable(
                name: "ItemsPhotos",
                schema: "CAA");

            migrationBuilder.DropTable(
                name: "ItemsThumbnails",
                schema: "CAA");

            migrationBuilder.DropTable(
                name: "Inventories",
                schema: "CAA");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "CAA");

            migrationBuilder.DropTable(
                name: "Locations",
                schema: "CAA");
        }
    }
}
