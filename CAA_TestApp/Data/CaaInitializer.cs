using CAA_TestApp.Models;
using System.Diagnostics;

namespace CAA_TestApp.Data
{
    public static class CaaInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            CaaContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<CaaContext>();

            try
            {
                //seed categories if there arent any
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category
                        {
                            Name = "Tech"
                        },
                        new Category
                        {
                            Name = "Clothes"
                        });
                    context.SaveChanges();
                }

                //int[] InventoryIDs = context.Inventories.Select(a => a.ID).ToArray();
                //int InventoryIDCount = InventoryIDs.Length;

                //seed locations if there arent any
                if (!context.Locations.Any())
                {
                    context.Locations.AddRange(
                        new Location
                        {
                            Name = "St. Catharines"
                        },
                        new Location
                        {
                            Name = "Niagara Falls"
                        });
                    context.SaveChanges();
                }

                //seed products if there aren't any
                if (!context.Inventories.Any())
                {
                    context.Inventories.AddRange(
                    new Inventory
                    {
                        Name = "iPad Mini",
                        Quantity = 1,
                        Description = "64GB, Midnight Blue",
                        Cost = 499.99,
                        DateReceived = DateTime.Now,
                        ShelfOn = "S001-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Tech")).ID,
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("St. Catharines")).ID
                    },
                    new Inventory
                    {
                        Name = "Garmin Smart Watch",
                        Description = "6th Generation, Black",
                        Cost = 299.99,
                        DateReceived = DateTime.Now,
                        Quantity = 1,
                        ShelfOn = "S001-02",
                        /*UpdatedBy = "Scott",
                        UpdatedOn = DateTime.Now,*/
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Tech")).ID,
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("St. Catharines")).ID
                    });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
