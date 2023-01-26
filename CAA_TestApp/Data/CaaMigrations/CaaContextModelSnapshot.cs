﻿// <auto-generated />
using System;
using CAA_TestApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CAA_TestApp.Data.CaaMigrations
{
    [DbContext(typeof(CaaContext))]
    partial class CaaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("CAA")
                .HasAnnotation("ProductVersion", "6.0.13");

            modelBuilder.Entity("CAA_TestApp.Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ProductID");

                    b.ToTable("Categories", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Event", b =>
                {
                    b.Property<int>("InventoryID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventLocation")
                        .HasColumnType("TEXT");

                    b.Property<int>("InventoryQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastLocation")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("LocationChangedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LocationchangedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("OrderedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReturnedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ReturnedOn")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("ShelfMoveBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ShelfMoveOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("TookBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TookOn")
                        .HasColumnType("TEXT");

                    b.HasKey("InventoryID", "ID");

                    b.ToTable("Events", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Cost")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("DateReceived")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EventID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EventInventoryID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ISBN")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastLocation")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("LocationChangedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<int>("LocationID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LocationchangedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("OrderedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReturnedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ReturnedOn")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("ShelfMoveBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ShelfMoveOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShelfOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("TookBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TookOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("LocationID");

                    b.HasIndex("ProductID");

                    b.HasIndex("EventInventoryID", "EventID");

                    b.ToTable("Inventories", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.ItemPhoto", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("invID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("invID")
                        .IsUnique();

                    b.ToTable("ItemsPhotos", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.ItemThumbnail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("invID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("invID")
                        .IsUnique();

                    b.ToTable("ItemsThumbnails", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Location", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Locations", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Products", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Category", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Product", null)
                        .WithMany("Categories")
                        .HasForeignKey("ProductID");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Event", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Category", null)
                        .WithMany("Inventories")
                        .HasForeignKey("CategoryID");

                    b.HasOne("CAA_TestApp.Models.Location", "Location")
                        .WithMany("Inventories")
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CAA_TestApp.Models.Product", "Product")
                        .WithMany("Inventories")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAA_TestApp.Models.Event", null)
                        .WithMany("ItemsInEvent")
                        .HasForeignKey("EventInventoryID", "EventID");

                    b.Navigation("Location");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CAA_TestApp.Models.ItemPhoto", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Inventory", "inventory")
                        .WithOne("ItemPhoto")
                        .HasForeignKey("CAA_TestApp.Models.ItemPhoto", "invID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inventory");
                });

            modelBuilder.Entity("CAA_TestApp.Models.ItemThumbnail", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Inventory", "inventory")
                        .WithOne("ItemThumbnail")
                        .HasForeignKey("CAA_TestApp.Models.ItemThumbnail", "invID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inventory");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Product", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Category", b =>
                {
                    b.Navigation("Inventories");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Event", b =>
                {
                    b.Navigation("ItemsInEvent");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
                    b.Navigation("ItemPhoto");

                    b.Navigation("ItemThumbnail");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Location", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Product", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}
