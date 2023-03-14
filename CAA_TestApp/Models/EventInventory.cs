namespace CAA_TestApp.Models
{
    public class EventInventory : Auditable
    {
        public int ID { get; set; }

        public int InventoryID { get; set; }
        public Inventory Inventory { get; set; }

        public int EventID { get; set; }
        public Event Event { get; set; }
    }
}
