﻿// <auto-generated />
using System;
using CAA_TestApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CAA_TestApp.Data.CaaMigrations
{
    [DbContext(typeof(CaaContext))]
    [Migration("20230314081147_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.14");

            modelBuilder.Entity("CAA_TestApp.Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Classification")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Classification")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Event", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
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

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("CAA_TestApp.Models.EventInventory", b =>
                {
                    b.Property<int>("EventID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InventoryID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("ID")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("EventID", "InventoryID");

                    b.HasIndex("InventoryID");

                    b.ToTable("EventInventories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
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

                    b.Property<string>("ISBN")
                        .HasColumnType("TEXT");

                    b.Property<int>("LocationID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
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

                    b.Property<int>("statusID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("LocationID");

                    b.HasIndex("ProductID");

                    b.HasIndex("statusID");

                    b.ToTable("Inventories");
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

                    b.ToTable("ItemsPhotos");
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

                    b.ToTable("ItemsThumbnails");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Location", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(6)
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

                    b.HasIndex("City", "Phone", "Address", "PostalCode")
                        .IsUnique();

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Organize", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("OrganizedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("OrganizedBy")
                        .IsUnique();

                    b.ToTable("Organizes");
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

                    b.Property<int?>("InventoryID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("OrganizeID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParLevel")
                        .HasColumnType("INTEGER");

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

                    b.HasIndex("InventoryID");

                    b.HasIndex("OrganizeID");

                    b.HasIndex("Name", "CategoryID")
                        .IsUnique();

                    b.ToTable("Products");
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

                    b.ToTable("QrImage");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Status", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("status")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("statuses");
                });

            modelBuilder.Entity("CAA_TestApp.Models.EventInventory", b =>
                {
                    b.HasOne("CAA_TestApp.Models.Event", "Event")
                        .WithMany("EventInventories")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CAA_TestApp.Models.Inventory", "Inventory")
                        .WithMany("EventInventories")
                        .HasForeignKey("InventoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
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

                    b.HasOne("CAA_TestApp.Models.Status", "Status")
                        .WithMany("Inventories")
                        .HasForeignKey("statusID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Product");

                    b.Navigation("Status");
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

                    b.HasOne("CAA_TestApp.Models.Inventory", null)
                        .WithMany("Products")
                        .HasForeignKey("InventoryID");

                    b.HasOne("CAA_TestApp.Models.Organize", "Organize")
                        .WithMany("Products")
                        .HasForeignKey("OrganizeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Organize");
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
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Event", b =>
                {
                    b.Navigation("EventInventories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Inventory", b =>
                {
                    b.Navigation("EventInventories");

                    b.Navigation("ItemPhoto");

                    b.Navigation("ItemThumbnail");

                    b.Navigation("Products");

                    b.Navigation("QRImage");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Location", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Organize", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Product", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("CAA_TestApp.Models.Status", b =>
                {
                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}