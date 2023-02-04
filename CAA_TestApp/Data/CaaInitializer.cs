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
                            Name = "Equipment"
                        },
                        new Category
                        {
                            Name = "SWAG"
                        },
                        new Category
                        {
                            Name = "Printed"
                        },
                        new Category
                        {
                            Name = "Event Material"
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
                            Name = "St. Catharines",
                            Phone = "9051211212",
                            Address = "3271 Schmon Pkwy",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            Name = "Niagara Falls",
                            Phone = "9051211212",
                            Address = "6788 Regional Rd 57",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            Name = "Welland",
                            Phone = "9051211212",
                            Address = "800 Niagara St",
                            PostalCode = "L3C5Z4"
                        },
                        new Location
                        {
                            Name = "Thorold",
                            Phone = "9059848585",
                            Address = "3271 Schmon Pkwy",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            Name = "Grimsby",
                            Phone = "9051211212",
                            Address = "Orchardview Village Square, 155 Main St E",
                            PostalCode = "L3M0A3"
                        });
                    context.SaveChanges();
                }

                //seed organization
                if (!context.Organizes.Any())
                {
                    context.Organizes.AddRange(
                        new Organize
                        {
                            OrganizedBy = "Boxes"
                        },
                        new Organize
                        {
                            OrganizedBy = "Items"
                        });
                    context.SaveChanges();
                }

                if (!context.Products.Any())
                {
                    context.Products.AddRange(
                    new Product
                    {
                        Name = "iPad Mini",
                        ParLevel = 20,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Equipment")).ID
                    },
                    new Product
                    {
                        Name = "Garmin Smart Watch",
                        ParLevel = 20,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Equipment")).ID,
                    },
                    new Product
                    {
                        Name = "Bracelets",
                        ParLevel = 15,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("SWAG")).ID
                    }, 
                    new Product
                    {
                        Name = "Cap",
                        ParLevel = 4,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("SWAG")).ID
                    }, 
                    new Product
                    {
                        Name = "Brochure",
                        ParLevel = 500,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Printed")).ID
                    }, 
                    new Product
                    {
                        Name = "Poster",
                        ParLevel = 3,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Printed")).ID
                    }, 
                    new Product
                    {
                        Name = "Foldable table",
                        ParLevel = 9,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Event Material")).ID
                    }, 
                    new Product
                    {
                        Name = "Chair",
                        ParLevel = 30,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Event Material")).ID
                    }, 
                    new Product
                    {
                        Name = "Walkie Talkie",
                        ParLevel = 5,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name.Contains("Equipment")).ID
                    });
                    context.SaveChanges();
                }

                //seed inventory if there aren't any
                if (!context.Inventories.Any())
                {
                    context.Inventories.AddRange(
                    new Inventory
                    {
                        Quantity = 30,
                        ISBN = "978006154236",
                        Cost = 499.99,
                        DateReceived = DateTime.Now,
                        Notes ="Sit itur Ad astra",
                        ShelfOn = "S001-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("St. Catharines")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("iPad Mini")).ID
                    },
                    new Inventory
                    {
                        ISBN = "97800557894654",
                        Cost = 299.99,
                        DateReceived = DateTime.Now,
                        Quantity = 0,
                        Notes = "Memoento mory",
                        ShelfOn = "S001-02",
                        /*UpdatedBy = "Scott",
                        UpdatedOn = DateTime.Now,*/                        
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("St. Catharines")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Garmin Smart Watch")).ID
                    },
                    new Inventory
                    {
                        Quantity = 14,
                        ISBN = "9780068984236",
                        Cost = 100,
                        DateReceived = DateTime.Now,
                        Notes = "Love pascem",
                        ShelfOn = "S002-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("Welland")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Bracelet")).ID
                    }, 
                    new Inventory
                    {
                        Quantity = 4,
                        ISBN = "9784568795462",
                        Cost = 250,
                        DateReceived = DateTime.Now,
                        Notes = "Si vis pacem para Belum",
                        ShelfOn = "S002-02",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("Thorold")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Cap")).ID
                    }, 
                    new Inventory
                    {
                        Quantity = 500,
                        ISBN = "978006598957894",
                        Cost = 120,
                        DateReceived = DateTime.Now,
                        Notes = "Non perdidit",
                        ShelfOn = "S003-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("Thorold")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Brochure")).ID
                    }, 
                    new Inventory
                    {
                        Quantity = 3,
                        ISBN = "978006544121456",
                        Cost = 110.99,
                        DateReceived = DateTime.Now,
                        Notes = "Vivandum, moriendum est",
                        ShelfOn = "S003-02",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("Grimsby")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Poster")).ID
                    }, 
                    new Inventory
                    {
                        Quantity = 0,
                        ISBN = "9784561238552",
                        Cost = 99.99,
                        DateReceived = DateTime.Now,
                        Notes = "Quid pro quo",
                        ShelfOn = "S004-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.Name.Contains("Niagara Falls")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Chair")).ID
                    });
                    context.SaveChanges();
                }

                if (!context.Events.Any())
                {
                    context.Events.AddRange(
                        new Event
                        {
                            Name = "Charity",
                            Date = DateTime.Now,
                            EventLocation = "Welland",
                            Notes = ""
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
