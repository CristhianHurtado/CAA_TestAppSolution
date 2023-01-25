﻿using CAA_TestApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CAA_TestApp.Data
{
    public class CaaContext : DbContext 
    {
        public CaaContext(DbContextOptions<CaaContext> options)
            : base(options)
        {
        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ItemPhoto> ItemsPhotos { get; set; }
        public DbSet<ItemThumbnail> ItemsThumbnails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CAA");


            //add other foreign key to product table
            modelBuilder.Entity<Category>()
                .HasMany<Inventory>(i => i.Inventories)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Location>()
                .HasMany<Inventory>(i => i.Inventories)
                .WithOne(i => i.Location)
                .HasForeignKey(i => i.LocationID)
                .OnDelete(DeleteBehavior.Restrict);

            //many to many
            modelBuilder.Entity<Event>()
                .HasKey(i => new { i.InventoryID, i.ID });

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ItemPhoto)
                .WithOne(v => v.inventory)
                .HasForeignKey<ItemPhoto>(i => i.invID);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ItemThumbnail)
                .WithOne(v => v.inventory)
                .HasForeignKey<ItemThumbnail>(i => i.invID);

            //add foreign key to inventory table
            //modelBuilder.Entity<Product>()
            //    .HasMany<Inventory>(i => i.Inventories)
            //    .WithOne(i => i.Product)
            //    .HasForeignKey(i => i.ProductID);

            //unique index for location
            modelBuilder.Entity<Location>()
                .HasIndex(i => i.Name)
                .IsUnique();

            //unique index for category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

        }

        //Code for for Tracking on logged user

        /*public override int SaveChanges(bool acceptAllChangesOnSuccess)
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
        }*/
    }
}
