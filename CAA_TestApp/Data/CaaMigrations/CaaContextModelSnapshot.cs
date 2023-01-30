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
                .HasAnnotation("ProductVersion", "6.0.11");

            modelBuilder.Entity("CAA_TestApp.Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
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

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventLocation")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<int>("InventoryQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
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

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateReceived")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EventID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EventInventoryID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ISBN")
                        .HasColumnType("TEXT");

                    b.Property<int>("LocationID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("ShelfOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("ProductID");

                    b.HasIndex("EventInventoryID", "EventID");

                    b.HasIndex("LocationID", "ProductID")
                        .IsUnique();

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
                        .IsRequired()
                        .HasMaxLength(40)
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

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("Name", "CategoryID")
                        .IsUnique();

                    b.ToTable("Products", "CAA");
                });

            modelBuilder.Entity("CAA_TestApp.Models.QrImage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("invID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("invID")
                        .IsUnique();

                    b.ToTable("QrImage", "CAA");
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
                        .OnDelete(DeleteBehavior.Restrict)
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

            modelBuilder.Entity("CAA_TestApp.Models.QrImage", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Inventory", "inventory")
                        .WithOne("QRImage")
                        .HasForeignKey("CAA_TestApp.Models.QrImage", "invID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inventory");
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

                    b.Navigation("QRImage");
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
