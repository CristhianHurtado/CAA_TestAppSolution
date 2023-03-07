using CAA_TestApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CAA_TestApp.Data
{
    public class CaaContext : DbContext 
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public CaaContext(DbContextOptions<CaaContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Organize> Organizes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ItemPhoto> ItemsPhotos { get; set; }
        public DbSet<ItemThumbnail> ItemsThumbnails { get; set; }
        public DbSet<Status> statuses { get; set; }
        public DbSet<EventInventory> EventInventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("CAA");

            //add foreign key to product table
            modelBuilder.Entity<Category>()
                .HasMany<Product>(i => i.Products)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organize>()
                .HasMany<Product>(i => i.Products)
                .WithOne(i => i.Organize)
                .HasForeignKey(i => i.OrganizeID)
                .OnDelete(DeleteBehavior.Restrict);

            //many to many

            modelBuilder.Entity<EventInventory>()
                .HasKey(i => new { i.EventID, i.InventoryID });

            modelBuilder.Entity<EventInventory>()
                .HasOne(i => i.Event)
                .WithMany(i => i.EventInventories)
                .HasForeignKey(i => i.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ItemPhoto)
                .WithOne(v => v.inventory)
                .HasForeignKey<ItemPhoto>(i => i.invID)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ItemThumbnail)
                .WithOne(v => v.inventory)
                .HasForeignKey<ItemThumbnail>(i => i.invID)
                .OnDelete(DeleteBehavior.Cascade);

            //for archive and send inv purposes
            modelBuilder.Entity<Status>()
                .HasMany<Inventory>(i => i.Inventories)
                .WithOne(i => i.Status)
                .HasForeignKey(i => i.statusID)
                .OnDelete(DeleteBehavior.Restrict);
            

            //add foreign key to inventory table
            modelBuilder.Entity<Product>()
                .HasMany<Inventory>(i => i.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasMany<Inventory>(i => i.Inventories)
                .WithOne(i => i.Location)
                .OnDelete(DeleteBehavior.Restrict);

            //unique index for location
            modelBuilder.Entity<Location>()
                .HasIndex(i => new { i.City, i.Phone, i.Address, i.PostalCode })
                .IsUnique();

            //unique index for category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Classification)
                .IsUnique();

            //unique index for product name
            modelBuilder.Entity<Product>()
                .HasIndex(i => new { i.Name, i.CategoryID })
                .IsUnique();

            //unique index for organize
            modelBuilder.Entity<Organize>()
                .HasIndex(i => i.OrganizedBy)
                .IsUnique();

            modelBuilder.Entity<Inventory>()
               .HasOne(i => i.QRImage)
               .WithOne(v => v.inventory)
               .HasForeignKey<QrImage>(i => i.invID);

        }

        //Code for for Tracking on logged user
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
