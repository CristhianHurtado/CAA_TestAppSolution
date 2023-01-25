namespace CAA_TestApp.Models
{
    public class Category
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Category()
        {
            this.Inventories = new HashSet<Inventory>();
        }

        public ICollection<Inventory> Inventories { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
