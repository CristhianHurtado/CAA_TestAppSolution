using CAA_TestApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CAA_TestApp.Data
{
    public static class CaaInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            Random random = new Random();

            List<int> numbers = Enumerable.Range(10000, 99999).OrderBy(x => random.Next()).Take(7).ToList();

            CaaContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<CaaContext>();

            try
            {
                context.Database.Migrate();

                //seed categories if there arent any
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category
                        {
                            Classification = "Equipment"
                        },
                        new Category
                        {
                            Classification = "SWAG"
                        },
                        new Category
                        {
                            Classification = "Printed"
                        },
                        new Category
                        {
                            Classification = "Event Material"
                        });

                    context.SaveChanges();
                }

                if (!context.statuses.Any())
                {
                    context.statuses.AddRange(
                        new Status
                        {
                            status = "In stock"
                        },
                        new Status
                        {
                            status = "In transit"
                        },
                        new Status
                        {
                            status = "Archived"
                        },
                        new Status
                        {
                            status = "In use"
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
                            City = "St. Catharines",
                            Phone = "9051211212",
                            Address = "3271 Schmon Pkwy",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            City = "Niagara Falls",
                            Phone = "9051211212",
                            Address = "6788 Regional Rd 57",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            City = "Welland",
                            Phone = "9051211212",
                            Address = "800 Niagara St",
                            PostalCode = "L3C5Z4"
                        },
                        new Location
                        {
                            City = "Thorold",
                            Phone = "9059848585",
                            Address = "3271 Schmon Pkwy",
                            PostalCode = "L2V4Y6"
                        },
                        new Location
                        {
                            City = "Grimsby",
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
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Equipment")).ID
                    },
                    new Product
                    {
                        Name = "Garmin Smart Watch",
                        ParLevel = 20,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Equipment")).ID,
                    },
                    new Product
                    {
                        Name = "Bracelets",
                        ParLevel = 15,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("SWAG")).ID
                    }, 
                    new Product
                    {
                        Name = "Cap",
                        ParLevel = 4,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("SWAG")).ID
                    }, 
                    new Product
                    {
                        Name = "Brochure",
                        ParLevel = 500,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Printed")).ID
                    }, 
                    new Product
                    {
                        Name = "Poster",
                        ParLevel = 3,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Boxes")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Printed")).ID
                    }, 
                    new Product
                    {
                        Name = "Foldable table",
                        ParLevel = 9,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Event Material")).ID
                    }, 
                    new Product
                    {
                        Name = "Chair",
                        ParLevel = 30,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Event Material")).ID
                    }, 
                    new Product
                    {
                        Name = "Walkie Talkie",
                        ParLevel = 5,
                        OrganizeID = context.Organizes.FirstOrDefault(c => c.OrganizedBy.Contains("Items")).ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Classification.Contains("Equipment")).ID
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
                        ISBN = numbers[0].ToString(),
                        Cost = 499.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S001-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("St. Catharines")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("iPad Mini")).ID
                    },
                    new Inventory
                    {
                        ISBN = numbers[1].ToString(),
                        Cost = 299.99,
                        DateReceived = DateTime.Now,
                        Quantity = 0,
                        Notes = "",
                        ShelfOn = "S001-02",
                        /*UpdatedBy = "Scott",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("St. Catharines")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Garmin Smart Watch")).ID
                    },
                    new Inventory
                    {
                        Quantity = 14,
                        ISBN = numbers[2].ToString(),
                        Cost = 3.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S002-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("Welland")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Bracelet")).ID
                    },
                    new Inventory
                    {
                        Quantity = 4,
                        ISBN = numbers[3].ToString(),
                        Cost = 15.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S002-02",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("Thorold")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Cap")).ID
                    },
                    new Inventory
                    {
                        Quantity = 500,
                        ISBN = numbers[4].ToString(),
                        Cost = 5.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S003-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("Thorold")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Brochure")).ID
                    },
                    new Inventory
                    {
                        Quantity = 3,
                        ISBN = numbers[5].ToString(),
                        Cost = 7.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S003-02",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("Grimsby")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Poster")).ID
                    },
                    new Inventory
                    {
                        Quantity = 10,
                        ISBN = numbers[6].ToString(),
                        Cost = 54.99,
                        DateReceived = DateTime.Now,
                        Notes = "",
                        ShelfOn = "S004-01",
                        /*UpdatedBy = "Brandon",
                        UpdatedOn = DateTime.Now,*/
                        LocationID = context.Locations.FirstOrDefault(l => l.City.Contains("Niagara Falls")).ID,
                        statusID = context.statuses.FirstOrDefault(l => l.status.Contains("In stock")).ID,
                        ProductID = context.Products.FirstOrDefault(p => p.Name.Contains("Chair")).ID
                    }); ;

                    context.SaveChanges();
                }

                if (!context.Events.Any())
                {
                    context.Events.AddRange(
                        new Event
                        {
                            Title = "Niagara College Career Fair",
                            Quantity= 45,
                            Date = DateTime.Now,
                            EventLocation = "100 Niagara College Blvd",
                            Notes = ""
                        },
                        new Event
                        {
                            Title = "Rankin Cancer Run",
                            Quantity= 100,
                            Date = DateTime.Now,
                            EventLocation = "St. Catharines",
                            Notes = ""
                        });

                    context.SaveChanges();
                }

                if (!context.EventInventories.Any())
                {
                    context.EventInventories.AddRange(
                        new EventInventory
                        {
                            InventoryID = context.Inventories.FirstOrDefault(i => i.Product.Name.Contains("Bracelet")).ID,
                            EventID = context.Events.FirstOrDefault(i => i.Title == "Niagara College Career Fair").ID
                        },
                        new EventInventory
                        {
                            InventoryID = context.Inventories.FirstOrDefault(i => i.Product.Name.Contains("Cap")).ID,
                            EventID = context.Events.FirstOrDefault(i => i.Title == "Niagara College Career Fair").ID
                        },
                        new EventInventory
                        {
                            InventoryID = context.Inventories.FirstOrDefault(i => i.Product.Name.Contains("Cap")).ID,
                            EventID = context.Events.FirstOrDefault(i => i.Title == "Rankin Cancer Run").ID
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
