namespace CAA_TestApp.Models
{
    public class Event : Auditable
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string EventLocation { get; set; }

        public string Notes { get; set; }

        public int InventoryID { get; set; }

        public int InventoryQuantity { get; set; }

        public Inventory Inventory { get; set; }

        public ICollection<Inventory> ItemsInEvent { get; set; }

    }
}
