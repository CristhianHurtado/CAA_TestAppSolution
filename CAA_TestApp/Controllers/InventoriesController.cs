using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAA_TestApp.Data;
using CAA_TestApp.Models;
using System.Drawing.Imaging;
using System.Drawing;
using CAA_TestApp.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using CAA_TestApp.ViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using QRCoder;
using ZXing;
using Microsoft.AspNetCore.Authorization;

namespace CAA_TestApp.Controllers
{
    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly CaaContext _context;

        private Random r = new Random();

        private int SendToken;

        public InventoriesController(CaaContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int[] LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Context"] = _context;

            ViewData["Filtering"] = "btn-outline-secondary ";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Status)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .Include(i => i.ItemThumbnail).OrderBy(i => i.Product.Name).ThenBy(i => i.Location.City)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (LocationID.Length > 0)
            {
                inventories = inventories.Where(p => LocationID.Contains(p.LocationID));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()) || p.ISBN.Contains(SearchName));
                ViewData["Filtering"] = " btn-danger";
            }

            //See if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sort by the button clicked
                }
                else
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "desc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                    .OrderBy(i => i.Product.Name)
                    .ThenBy(i => i.Location.City);

                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
            }
            if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Classification);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Classification);
                    }
                }
            }
            else if (sortField == "Status") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Status.status);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Status.status);
                    }
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "inventories");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public async Task<IActionResult> Archived(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int? LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Context"] = _context;

            ViewData["Filtering"] = "btn-outline-secondary ";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Status)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .Where(i => i.statusID == _context.statuses.FirstOrDefault(i => i.status == "Archived").ID)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (LocationID.HasValue)
            {
                inventories = inventories.Where(p => p.LocationID == LocationID);
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = " btn-danger";
            }

            //See if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sort by the button clicked
                }
                else
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Product.Name)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.City);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Classification);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Classification);
                    }
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "inventories");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public async Task<IActionResult> ReceivedInv(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int? LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Context"] = _context;

            ViewData["Filtering"] = "btn-outline-secondary ";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Status)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .Where(i => i.statusID == _context.statuses.FirstOrDefault(i => i.status == "On transit").ID)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (LocationID.HasValue)
            {
                inventories = inventories.Where(p => p.LocationID == LocationID);
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = " btn-danger";
            }

            //See if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sort by the button clicked
                }
                else
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Product.Name)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.City);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Classification);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Classification);
                    }
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "inventories");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public async Task<IActionResult> InUseInv(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int? LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Context"] = _context;

            ViewData["Filtering"] = "btn-outline-secondary ";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Status)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .Where(i => i.statusID == _context.statuses.FirstOrDefault(i => i.status == "In use").ID)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (LocationID.HasValue)
            {
                inventories = inventories.Where(p => p.LocationID == LocationID);
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = " btn-danger";
            }

            //See if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sort by the button clicked
                }
                else
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Product.Name)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.City);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Classification);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Classification);
                    }
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "inventories");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public async Task<IActionResult> ReservedInv(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int? LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Context"] = _context;

            ViewData["Filtering"] = "btn-outline-secondary ";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Status)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .Where(i => i.statusID == _context.statuses.FirstOrDefault(i => i.status == "Reserved").ID)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (LocationID.HasValue)
            {
                inventories = inventories.Where(p => p.LocationID == LocationID);
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = " btn-danger";
            }

            //See if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start

                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sort by the button clicked
                }
                else
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Location")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.City)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Product.Name)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.City);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Cost)
                        .ThenBy(i => i.Location.City);
                }
            }
            else if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Classification);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Classification);
                    }
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "inventories");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.Location);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.Product);
            return View(inventory);
        }

        public async Task<IActionResult> NoActionDetails(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.Location);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.Product);
            return View(inventory);
        }

        // GET: Inventories/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var excludedItem = _context.Locations.FirstOrDefault(i => i.City == "On transit");
            var locations = _context.Locations.Where(i => i != excludedItem);

            ViewData["LocationID"] = new SelectList(locations, "ID", "City");
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification");
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy");
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name");
            //Redirect("/Inventories/Index");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,ISBN,Quantity,Notes,ShelfOn,Cost,DateReceived," +
            "LocationID,ProductID, statusID")] Inventory inventory, IFormFile thePicture)
        {
            try
            {
                //checks for duplicate ISBN
                string newISBN = GenerateISBN();

                //checks for duplicate in DB

                bool isDuplicate = false;

                do
                {
                    foreach (var item in _context.Inventories)
                    {
                        if (item.LocationID == inventory.LocationID && item.ProductID == inventory.ProductID)
                        {
                            isDuplicate = true;
                            throw new Exception();
                        }
                    }
                }
                while (isDuplicate);

                if (ModelState.IsValid)
                {
                    await AddPicture(inventory, thePicture);
                    inventory.ISBN = newISBN;
                    inventory.statusID = _context.statuses.FirstOrDefault(s => s.status == "In stock").ID;
                    _context.Add(inventory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { inventory.ID });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Inventories.LocationID, Inventories.ProductID"))
                {
                    ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and location.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and location.");
            }


            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.ProductID);
            return View(inventory);
        }

        public IActionResult CreateProduct()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification");
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([Bind("ID,Name,ParLevel,CategoryID,OrganizeID")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Create");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Products.Name, Products.CategoryID"))
                {
                    ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and category.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
                }
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", product.OrganizeID);
            RedirectToPage("/Inventories/Create");
            return View();
        }


        // GET: Inventories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            //var inventory = await _context.Inventories
            //    .Include(i => i.ItemPhoto)
            //    .Include(i => i.Location)
            //    .Include(i => i.Product)
            //    .ThenInclude(i => i.Organize)

            var inventory = await _context.Inventories
                .Include(i => i.ItemPhoto)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventory == null)
            {
                return NotFound();
            }



            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.Location);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.Product);
            return View(inventory);
        }
        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Inventory inventory,
            string chkRemoveImage, IFormFile thePicture, Byte[] RowVersion)
        {
            var inventoryToUpdate = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventoryToUpdate == null)
            {
                return NotFound();
            }

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(inventoryToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Inventory>(inventoryToUpdate, "",
                i => i.Quantity, i => i.Notes, i => i.Cost,
                i => i.DateReceived, i => i.LocationID, i => i.ProductID))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        inventoryToUpdate.ItemThumbnail = _context.ItemsThumbnails.Where(p => p.invID == inventoryToUpdate.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        inventoryToUpdate.ItemPhoto = null;
                        inventoryToUpdate.ItemThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(inventoryToUpdate, thePicture);
                    }

                    //_context.Update(inventory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { inventoryToUpdate.ID });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Inventory)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Inventory record was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Inventory)databaseEntry.ToObject();
                        if (databaseValues.Quantity != clientValues.Quantity)
                            ModelState.AddModelError("Quantity", "Current value: "
                                + databaseValues.Quantity);
                        if (databaseValues.Notes != clientValues.Notes)
                            ModelState.AddModelError("Notes", "Current value: "
                                + databaseValues.Notes);
                        if (databaseValues.ShelfOn != clientValues.ShelfOn)
                            ModelState.AddModelError("ShelfOn", "Current value: "
                                + databaseValues.ShelfOn);
                        if (databaseValues.Cost != clientValues.Cost)
                            ModelState.AddModelError("Cost", "Current value: "
                                + databaseValues.Cost);
                        if (databaseValues.DateReceived != clientValues.DateReceived)
                            ModelState.AddModelError("DateReceived", "Current value: "
                                + databaseValues.DateReceived);
                        if (databaseValues.LocationID != clientValues.LocationID)
                        {
                            Models.Location databaseLocation = await _context.Locations.FirstOrDefaultAsync(i => i.ID == databaseValues.LocationID);
                            ModelState.AddModelError("LocationID", $"Current value: {databaseLocation?.City}");
                        }
                        if (databaseValues.ProductID != clientValues.ProductID)
                        {
                            Product databaseProduct = await _context.Products.FirstOrDefaultAsync(i => i.ID == databaseValues.ProductID);
                            ModelState.AddModelError("ProductID", $"Current value: {databaseProduct?.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Inventory List' hyperlink.");
                        inventoryToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Inventories.LocationID, Inventories.ProductID"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and location.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Please try again. " +
                        "If the problem persists, contact your systems administrator.");
                    }
                }
            }
            //return RedirectToAction(nameof(Index));

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.ProductID);
            return View(inventoryToUpdate);
        }

        /*
        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ISBN,Quantity,Notes,ShelfOn,Cost,DateReceived," +
            "LocationID,ProductID")] Inventory inventory, string chkRemoveImage, IFormFile thePicture, Byte[] RowVersion)
        {
            if (id != inventory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        inventory.ItemThumbnail = _context.ItemsThumbnails.Where(p => p.invID == inventory.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        inventory.ItemPhoto = null;
                        inventory.ItemThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(inventory, thePicture);
                    }

                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /* for match values with IDs on try/catch update 
                     * 
                    if (databaseValues.GenreID != clientValues.GenreID)
                    {
                        Genre databaseGenre = await _context.Genres.FirstOrDefaultAsync(i => i.ID == databaseValues.GenreID);
                        ModelState.AddModelError("GenreID", $"Current value: {databaseGenre?.Name}");
                    }
                    if (databaseValues.AlbumID != clientValues.AlbumID)
                    {
                        Album databaseAlbum = await _context.Albums.FirstOrDefaultAsync(i => i.ID == databaseValues.AlbumID);
                        ModelState.AddModelError("AlbumID", $"Current value: {databaseAlbum?.Name}");
                    }
                    if (!InventoryExists(inventory.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user. Please go back and refresh.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _context.Entry(inventory).Property("RowVersion").OriginalValue = RowVersion;
            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID", inventory.ProductID);
            return View(inventory);

          

        }*/
        public async Task<IActionResult> SendInv(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            //var inventory = await _context.Inventories
            //    .Include(i => i.ItemPhoto)
            //    .Include(i => i.Location)
            //    .Include(i => i.Product)
            //    .ThenInclude(i => i.Organize)

            var inventory = await _context.Inventories
                .Include(i => i.ItemPhoto)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventory == null)
            {
                return NotFound();
            }

            var excludedItem = _context.Locations.FirstOrDefault(i => i.City == "On transit");
            var locations = _context.Locations.Where(i => i != excludedItem);

            ViewData["LocFrom"] = _context.Locations.FirstOrDefault(i => i.ID == inventory.LocationID).City;
            ViewData["LocationID"] = new SelectList(locations, "ID", "City");
            ViewData["quanValue"] = 0;
            return View(inventory);
        }
        /*
        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ISBN,Quantity,Notes,ShelfOn,Cost,DateReceived," +
            "LocationID,ProductID")] Inventory inventory, string chkRemoveImage, IFormFile thePicture, Byte[] RowVersion)
        {
            if (id != inventory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        inventory.ItemThumbnail = _context.ItemsThumbnails.Where(p => p.invID == inventory.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        inventory.ItemPhoto = null;
                        inventory.ItemThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(inventory, thePicture);
                    }

                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /* for match values with IDs on try/catch update 
                     * 
                    if (databaseValues.GenreID != clientValues.GenreID)
                    {
                        Genre databaseGenre = await _context.Genres.FirstOrDefaultAsync(i => i.ID == databaseValues.GenreID);
                        ModelState.AddModelError("GenreID", $"Current value: {databaseGenre?.Name}");
                    }
                    if (databaseValues.AlbumID != clientValues.AlbumID)
                    {
                        Album databaseAlbum = await _context.Albums.FirstOrDefaultAsync(i => i.ID == databaseValues.AlbumID);
                        ModelState.AddModelError("AlbumID", $"Current value: {databaseAlbum?.Name}");
                    }
                    if (!InventoryExists(inventory.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user. Please go back and refresh.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _context.Entry(inventory).Property("RowVersion").OriginalValue = RowVersion;
            
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "ID", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "ID", inventory.ProductID);
            return View(inventory);

          

        }*/

        //POST : Inventories/SendInv/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SendInv(int id, string locationFrom, string locationTo, int quantity)
        {
            

            SendToken = r.Next(200);

            var inventoryToSend = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventoryToSend == null)
            {
                return NotFound();
            }

            try
            {
                string To = _context.Locations.FirstOrDefault(i => i.ID == Convert.ToInt32(locationTo)).City;

                Inventory send = new Inventory
                {
                    ISBN = $"{inventoryToSend.ISBN} {SendToken}",
                    ProductID = inventoryToSend.ProductID,
                    LocationID = inventoryToSend.LocationID,
                    Notes = $"To {To}",
                    ShelfOn = inventoryToSend.ShelfOn,
                    Cost = inventoryToSend.Cost,
                    DateReceived = inventoryToSend.DateReceived,
                    Quantity = quantity,
                    ItemPhoto = inventoryToSend.ItemPhoto,
                    ItemThumbnail = inventoryToSend.ItemThumbnail,
                    QRImage = inventoryToSend.QRImage,
                    EventInventories = inventoryToSend.EventInventories,
                    statusID = _context.statuses.FirstOrDefault(i => i.status == "On transit").ID,
                };

                int preventNegative = inventoryToSend.Quantity - quantity;

                if (!ValidateLocationFromAndTo(locationFrom, To))
                {
                    ViewData["SameLocs"] = "• You can't send inventory to the same locations";
                    // Return the view with errors if validation fails
                    ViewData["LocFrom"] = _context.Locations.FirstOrDefault(i => i.ID == inventoryToSend.LocationID).City;
                    ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City");
                    ViewData["quanValue"] = quantity;
                    return View(inventoryToSend);
                }

                if (!ValidatePreventBelowZero(inventoryToSend.Quantity, quantity))
                {
                    ViewData["OverQuan"] = "• You can't send more items than existing in inventory";
                    ViewData["LocFrom"] = _context.Locations.FirstOrDefault(i => i.ID == inventoryToSend.LocationID).City;
                    ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City");
                    ViewData["quanValue"] = quantity;
                    return View(inventoryToSend);
                }
                

                ModelState.SetModelValue("locationFrom", new ValueProviderResult(locationFrom));
                ModelState.SetModelValue("locationTo", new ValueProviderResult(To));
                ModelState.SetModelValue("quantity", new ValueProviderResult(quantity.ToString()));

                inventoryToSend.Quantity = preventNegative;
                
                _context.Inventories.Add(send);
                _context.Update(inventoryToSend);
                _context.SaveChanges();
                

            }
            catch (Exception)
            {
            }

            ViewData["LocFrom"] = _context.Locations.FirstOrDefault(i => i.ID == inventoryToSend.LocationID).City;
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City");
            ViewData["done"] = true;

            //if successful - take the user to the ReceivedInv page (which is In Transit)
            return RedirectToAction("ItemTransfered");
        }
        public IActionResult ItemTransfered()
        {
            return View();
        }
        public async Task<IActionResult> RecieveInv(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Status)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "City", inventory.Location);
            return View(inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveInv(int id)
        {
            //string[] qrValidator = codeForISBN.Split(' ');

            var inventoryToReceive = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventoryToReceive == null)
            {
                return NotFound();
            }

            string[] filterLocation = inventoryToReceive.Notes.Split(' ');
            string addToLocation = "";
            for (int i = 1; i < filterLocation.Length; i++)
            {
                addToLocation += $"{filterLocation[i]} ";
            }

            addToLocation = addToLocation.Remove(addToLocation.Length - 1);

            string[] validateISBN = inventoryToReceive.ISBN.Split(' ');

            int To = _context.Locations.FirstOrDefault(i => i.City == addToLocation).ID;

            /*if(To != Convert.ToInt32(location))
            {
                throw new Exception("Location incorrect");
            }**/

            List<Inventory> receive = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Where(i => i.ProductID == inventoryToReceive.ProductID)
                .ToList();

            if (inventoryToReceive == null)
            {
                return NotFound();
            }

            //if (qrValidator[0] != validateISBN[0] || qrValidator[1] != validateISBN[1])
            //{
            //    throw new Exception("Wrong qr, make sure you are scanning the right package");
            //}

            List<int> aux = new List<int>();
            for (int i = 0; i < receive.Count; i++)
            {
                if (receive[i].LocationID == To && receive[i].statusID == _context.statuses.FirstOrDefault(i => i.status == "In stock").ID)
                {
                    aux.Add(i);
                }
            }

            Dictionary<int, Inventory> dic = new Dictionary<int, Inventory>();
            foreach (int i in aux)
            {
                dic.Add(i, receive[i]);
            }

            if (dic.Count <= 0)
            {
                inventoryToReceive.ISBN = GenerateISBN();
                inventoryToReceive.LocationID = To;
                inventoryToReceive.Notes = $"Add new notes";
                inventoryToReceive.statusID = _context.statuses.FirstOrDefault(i => i.status == "In stock").ID;
                inventoryToReceive.DateReceived = DateTime.Now;
                _context.Inventories.Update(inventoryToReceive);
            }
            else
            {
                Inventory updateInv = (Inventory)dic.FirstOrDefault().Value;
                updateInv.Quantity += inventoryToReceive.Quantity;
                updateInv.DateReceived = DateTime.Now;
                _context.Inventories.Remove(inventoryToReceive);
                _context.Inventories.Update(updateInv);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Inventories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Status)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Status)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Status)
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'CaaContext.Inventories'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);

            inventory.statusID = _context.statuses.FirstOrDefault(i => i.status == "Archived").ID;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'CaaContext.Inventories'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);

            inventory.statusID = _context.statuses.FirstOrDefault(i => i.status == "In stock").ID;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'CaaContext.Inventories'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);


            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> QuickScan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickScan(string codeForISBN)
        {
            Inventory inventoryScaned = await _context.Inventories.FirstOrDefaultAsync(i => i.ISBN == codeForISBN);

            return RedirectToAction("Details", new { inventoryScaned.ID });
        }

        public async Task<IActionResult> GenerateQr(int? id)
        {
            //Random r = new Random();
            //int isbn = r.Next(10, 50);

            var inventory = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            //string code = ViewData["ISBN"].ToString();

            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            //QRCodeData qrData = qrCodeGen.CreateQrCode($"{inventory.Product.Name}{inventory.Location}", QRCodeGenerator.ECCLevel.Q);
            QRCodeData qrData = qrCodeGen.CreateQrCode($"{inventory.ISBN}", QRCodeGenerator.ECCLevel.Q);
            QRCode qr = new QRCode(qrData);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = qr.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            
            return View(inventory);
        }

        public async Task<IActionResult> PreMassGen()
        {
            PopulateAssignedInventoryData();

            return View();
        }

        public async Task<IActionResult> MassQrCodeGen(int?[] selectedOptions, string locationAt)
        {
            List<string> pngs = new List<string>();
            List<string> itemsNotFound = new List<string>();
            List<string> found = new List<string>();
            
            for(int index = 0; index < selectedOptions.Length; index++)
            {
                Inventory inv = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == selectedOptions[index] && i.LocationID == Convert.ToInt32(locationAt)); 
                Product item = await _context.Products.FirstOrDefaultAsync(i => i.ID == selectedOptions[index]);
                if(inv == null)
                {
                    string name = item.Name;
                    itemsNotFound.Add(name);
                    continue;
                }

                string nameFound = item.Name;
                found.Add(nameFound);

                QRCodeGenerator qrCodeGen = new QRCodeGenerator();
                //QRCodeData qrData = qrCodeGen.CreateQrCode($"{inventory.Product.Name}{inventory.Location}", QRCodeGenerator.ECCLevel.Q);
                QRCodeData qrData = qrCodeGen.CreateQrCode($"{inv.ISBN}", QRCodeGenerator.ECCLevel.Q);
                QRCode qr = new QRCode(qrData);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap bitmap = qr.GetGraphic(20))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        pngs.Add("data:image/png;base64," + Convert.ToBase64String(ms.ToArray()));
                    }
                }
            }
            
            ViewBag.FoundItems = found;
            ViewBag.ItemsNotFound = itemsNotFound;
            ViewBag.QRCodeImages = pngs;

            return View();
        }

        public async Task<IActionResult> SendInvQr(int? id)
        {
            //Random r = new Random();
            //int isbn = r.Next(10, 50);

            var inventory = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            //string code = ViewData["ISBN"].ToString();

            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            //           QRCodeData qrData = qrCodeGen.CreateQrCode($"{inventory.Product.Name}{inventory.Location}", QRCodeGenerator.ECCLevel.Q);
            QRCodeData qrData = qrCodeGen.CreateQrCode($"{inventory.ISBN}", QRCodeGenerator.ECCLevel.Q);
            QRCode qr = new QRCode(qrData);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bitmap = qr.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View(inventory);
        }
        [HttpGet]
        public JsonResult GetProducts(int? id)
        {
            return Json(ProductSelectList(id));
        }

        private void PopulateAssignedInventoryData()
        {
            var allOptions = _context.Products;
            //var currentOptionsIDs = new HashSet<int>(@event.EventInventories.Select(i => i.InventoryID));
            var checkBoxes = new List<CheckOptionVM>();

            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.Name
                    //Assigned = currentOptionsIDs.Contains(option.ID)
                });
            }
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "City");
            ViewData["InventoryOptions"] = checkBoxes;
        }

        private SelectList LocationSelectList(int? id)
        {
            var dQuery = from d in _context.Locations
                         orderby d.City
                         select d;
            return new SelectList(dQuery, "ID", "City", id);
        }

        private SelectList CategorySelectList(int? id)
        {
            var dQuery = from d in _context.Categories
                         orderby d.Classification
                         select d;
            return new SelectList(dQuery, "ID", "Classification", id);
        }

        private SelectList ProductSelectList(int? id)
        {
            var dQuery = from d in _context.Products
                         orderby d.Name
                         select d;
            return new SelectList(dQuery, "ID", "Name", id);
        }

        private void PopulateDropDownListsLocations(Models.Location location = null)
        {
            ViewData["LocationID"] = LocationSelectList(location?.ID);
        }
        private void PopulateDropDownListsCategories(Category category = null)
        {
            ViewData["CategoryID"] = CategorySelectList(category?.ID);
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.ID == id);
        }

        public IActionResult DownloadInventories()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                         orderby a.Product descending
                         select new
                         {
                             ISBN = a.ISBN,
                             Quantity = a.Quantity,
                             Notes = a.Notes,
                             Shelf = a.ShelfOn,
                             Cost = a.Cost,
                             DateReceived = a.DateReceived.ToShortDateString(),
                             Location = a.Location.City,
                             Product = a.Product.Name
                         };
            int numRows = intory.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Inventory");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);

                    workSheet.Column(4).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 8])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Inventory Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 8])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 8])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Inventory.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }

        private string GenerateISBN()
        {
            Random random = new Random();

            string newISBN;
            bool Exist = false;

            do
            {
                newISBN = random.Next(10000, 99999).ToString();

                foreach (var item in _context.Inventories)
                {
                    if (item.ISBN == newISBN)
                    {
                        Exist = true;
                    }
                }
            }
            while (Exist);

            return newISBN;
        }

        public async Task<IActionResult> InventoryReports(int? page, int? pageSizeID, string actionButton, string SearchName, int[] LocationID)
        {
            PopulateDropDownListsLocations();

            var sumQ = _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)

                .GroupBy(c => new { c.ID, LocID = c.Location.ID ,c.Product.Category.Classification, c.Product.Name, c.Location.City, c.Quantity, c.Cost })
                
                .Select(grp => new InventoryReportsVM
                {
                    ID = grp.Key.ID,
                    LocationID = grp.Key.LocID,
                    Category = grp.Key.Classification,
                    Product = grp.Key.Name,
                    Location = grp.Key.City,
                    Quantity = grp.Key.Quantity,
                    Cost = grp.Key.Cost,




                }).OrderBy(s => s.Product);

            ViewData["Context"] = _context;
            ViewData["Filtering"] = "btn-outline-secondary ";

            if (LocationID.Length > 0)
            {
                sumQ = (IOrderedQueryable<InventoryReportsVM>)sumQ.Where(i => LocationID.Contains(i.LocationID));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                sumQ = (IOrderedQueryable<InventoryReportsVM>)sumQ.Where(i => i.Product.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start
            }

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "InventoryReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<InventoryReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);



        }

        public IActionResult DownloadInventoriesReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                         orderby a.Location descending
                         select new
                         {
                             Product = a.Product.Name,
                             Quantity = a.Quantity,
                             Cost = a.Cost,
                             Location = a.Location.City,
                             Category = a.Product.Category.Classification,

                         };
            int numRows = intory.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Inventory");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);

                    workSheet.Column(3).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 5])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Inventory Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 5])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 5])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "InventoryReport.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }

        public async Task<IActionResult> CostReports(int? page, int? pageSizeID, string SearchName, int[] LocationID, string actionButton)
        {
            PopulateDropDownListsLocations();

            var sumQ = _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)

                .GroupBy(c => new { c.ID, LocID = c.Location.ID, c.Product.Name, c.Quantity, c.Cost })
                .Select(grp => new CostReportsVM
                {
                    ID = grp.Key.ID,
                    LocationID = grp.Key.LocID,
                    Product = grp.Key.Name,
                    Quantity = grp.Key.Quantity,
                    Cost = grp.Key.Cost,
                    CostTotal = grp.Key.Cost * grp.Key.Quantity


                }).OrderBy(s => s.Product);

            ViewData["Context"] = _context;
            ViewData["Filtering"] = "btn-outline-secondary ";

            if (LocationID.Length > 0)
            {
                sumQ = (IOrderedQueryable<CostReportsVM>)sumQ.Where(i => LocationID.Contains(i.LocationID));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                sumQ = (IOrderedQueryable<CostReportsVM>)sumQ.Where(i => i.Product.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start
            }

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "CostReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<CostReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        public IActionResult DownloadCostReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                         orderby a.Product descending
                         select new
                         {
                             Product = a.Product.Name,
                             Quantity = a.Quantity,
                             Cost = a.Cost,
                             CostTotal = a.Cost * a.Quantity

                         };
            int numRows = intory.Count();


            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Inventory");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);

                    workSheet.Column(3).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Column(4).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 4])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Cost Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 4])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 4])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Cost.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }

        public async Task<IActionResult> LowStockReports(int? page, int? pageSizeID, string SearchName, int[] LocationID, string actionButton)
        {
            PopulateDropDownListsLocations();

            var sumQ = _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                .Where(a => a.Quantity < a.Product.ParLevel)
                .Where(a => a.Quantity != 0)

                .GroupBy(c => new { c.ID, LocID = c.Location.ID, c.Product.Category.Classification, c.Product.Name, c.Product.ParLevel, c.Location.City, c.Quantity, c.Cost })
                .Select(grp => new LowStockReportsVM
                {
                    ID = grp.Key.ID,
                    LocationID = grp.Key.LocID,
                    Category = grp.Key.Classification,
                    Product = grp.Key.Name,
                    Quantity = grp.Key.Quantity,
                    ParLevel = grp.Key.ParLevel,
                    Location = grp.Key.City,
                    Cost = grp.Key.Cost,

                }).OrderBy(s => s.Product);

            if (LocationID.Length > 0)
            {
                sumQ = (IOrderedQueryable<LowStockReportsVM>)sumQ.Where(i => LocationID.Contains(i.LocationID));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                sumQ = (IOrderedQueryable<LowStockReportsVM>)sumQ.Where(i => i.Product.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start
            }

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "LowStockReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<LowStockReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        public IActionResult DownloadLowStockReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                .Where(a => a.Quantity < a.Product.ParLevel)
                .Where(a => a.Quantity != 0)
                         orderby a.Product descending
                         select new
                         {
                             Name = a.Product.Name,
                             Category = a.Product.Category.Classification,
                             Quantity = a.Quantity,
                             ParLevel = a.Product.ParLevel,
                             Location = a.Location.City,
                             Cost = a.Cost,



                         };
            int numRows = intory.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("LowStock");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);

                    workSheet.Column(6).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 6])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Low Stock Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 6])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "LowStock.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }

        public async Task<IActionResult> NoStockReports(int? page, int? pageSizeID, string SearchName, int[] LocationID, string actionButton)
        {
            PopulateDropDownListsLocations();

            var sumQ = _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                .Where(a => a.Quantity == 0)

                .GroupBy(c => new { c.ID, LocID = c.Location.ID, c.Product.Category.Classification, c.Product.Name, c.Product.ParLevel, c.Location.City, c.Quantity, c.Cost })
                .Select(grp => new NoStockReportsVM
                {
                    ID = grp.Key.ID,
                    LocationID = grp.Key.LocID,
                    Category = grp.Key.Classification,
                    Product = grp.Key.Name,
                    Quantity = grp.Key.Quantity,
                    ParLevel = grp.Key.ParLevel,
                    Location = grp.Key.City,
                    Cost = grp.Key.Cost,

                }).OrderBy(s => s.Product);

            if (LocationID.Length > 0)
            {
                sumQ = (IOrderedQueryable<NoStockReportsVM>)sumQ.Where(i => LocationID.Contains(i.LocationID));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                sumQ = (IOrderedQueryable<NoStockReportsVM>)sumQ.Where(i => i.Product.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start
            }

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "NoStockReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<NoStockReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        public IActionResult DownloadNoStockReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Inventories
                .Include(a => a.Products)
                .ThenInclude(b => b.Category)
                .Include(a => a.Products)
                .ThenInclude(a => a.Organize)
                .Include(a => a.Location)
                .Where(a => a.Quantity == 0)
                         orderby a.Product descending
                         select new
                         {
                             Name = a.Product.Name,
                             Category = a.Product.Category.Classification,
                             Quantity = a.Quantity,
                             ParLevel = a.Product.ParLevel,
                             Location = a.Location.City,
                             Cost = a.Cost,



                         };
            int numRows = intory.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("NoStock");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);

                    workSheet.Column(6).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 6])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "No Stock Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 6])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "NoStock.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }

        public async Task<IActionResult> EventReports(int? page, int? pageSizeID, string SearchName, string actionButton)
        {


            var sumQ = _context.EventInventories
                .Include(a => a.Event)
                .Include(a => a.Inventory)
                .ThenInclude(a => a.Product)


                .GroupBy(c => new { c.ID, c.Inventory.Product.Name, c.Inventory.Quantity, c.Event.Title,c.Event.Date, c.Event.EventLocation, c.Event.Notes })
                .Select(grp => new EventReportsVM
                {
                    ID = grp.Key.ID,
                    Title = grp.Key.Title,
                    Name = grp.Key.Name,
                    Quantity = grp.Key.Quantity,
                    Date = grp.Key.Date,
                    EventLocation = grp.Key.EventLocation,
                    Notes = grp.Key.Notes


                }).OrderBy(s => s.Name);

            //Handle paging
            if (!String.IsNullOrEmpty(SearchName))
            {
                sumQ = (IOrderedQueryable<EventReportsVM>)sumQ.Where(i => i.Title.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  btn-danger ";
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1; //Reset page to start
            }
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "EventReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<EventReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        public IActionResult DownloadEventReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.EventInventories
                .Include(a => a.Event)
                .Include(a => a.Inventory)
                .ThenInclude(a => a.Product)
                         orderby a.Event.Title descending
                         select new
                         {
                             Title = a.Event.Title,
                             Name = a.Inventory.Product.Name,
                             Quantity = a.Inventory.Quantity,
                             Date = a.Event.Date.ToShortDateString(),
                             EventLocation = a.Event.EventLocation,
                             Notes = a.Event.Notes

                         };
            int numRows = intory.Count();


            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("Events");

                    workSheet.Cells[3, 1].LoadFromCollection(intory, true);


                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 5])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    workSheet.Cells.AutoFitColumns();

                    workSheet.Cells[1, 1].Value = "Event Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 5])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 5])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Event.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data. ");
        }


        private bool ValidateLocationFromAndTo(string locationFrom, string locationTo)
        {
            if (locationFrom == locationTo)
            {
                ModelState.AddModelError("locationToAndFrom", "Locations cannot be the same.");
                return false;
            }

            return true;
        }
        private bool ValidatePreventBelowZero(int realQuan, int sending)
        {
            int diff = realQuan - sending;
            if (diff < 0)
            {
                ModelState.AddModelError("ValidatePreventBelowZero", "Can't send more items than existing.");
                return false;
            }

            return true;
        }

        private async Task AddPicture(Inventory inventory, IFormFile thePicture)
        {
            //Get the picture and save it with the Inventory (2 sizes)
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();//Gives us the Byte[]

                        //Check if we are replacing or creating new
                        if (inventory.ItemPhoto != null)
                        {
                            //We already have pictures so just replace the Byte[]
                            inventory.ItemPhoto.Content = ResizeImage.shrinkImageWebp(pictureArray, 300, 400);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            inventory.ItemThumbnail = _context.ItemsThumbnails.Where(p => p.invID == inventory.ID).FirstOrDefault();
                            inventory.ItemThumbnail.Content = ResizeImage.shrinkImageWebp(pictureArray, 75, 90);
                        }
                        else //No pictures saved so start new
                        {
                            inventory.ItemPhoto = new ItemPhoto
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 300, 400),
                                MimeType = "image/webp"
                            };
                            inventory.ItemThumbnail = new ItemThumbnail
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 75, 90),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }


    }
}
