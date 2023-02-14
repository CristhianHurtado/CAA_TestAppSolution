namespace CAA_TestApp.Models
{
    public class Status
    {
        public int ID { get; set; }

        public string status { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
