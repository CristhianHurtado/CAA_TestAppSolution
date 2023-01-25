namespace CAA_TestApp.Models
{
    public class Location
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
