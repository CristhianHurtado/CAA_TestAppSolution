using Microsoft.EntityFrameworkCore.Migrations;

namespace CAA_TestApp.Data
{
    public class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            //Triggers for Items
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetItemTimestampOnUpdate
                    AFTER UPDATE ON Inventories
                    BEGIN
                        UPDATE Inventory
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetInventoryTimestampOnInsert
                    AFTER INSERT ON Inventories
                    BEGIN
                        UPDATE Inventories
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
                migrationBuilder.Sql(
    @"
                    CREATE TRIGGER SetItemTimestampOnUpdate
                    AFTER UPDATE ON Product
                    BEGIN
                        UPDATE Product
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
             migrationBuilder.Sql(
    @"
                    CREATE TRIGGER SetItemTimestampOnUpdate
                    AFTER UPDATE ON Products
                    BEGIN
                        UPDATE Products
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }

    }
}
