using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAA_TestApp.Data;
using CAA_TestApp.Models;
using CAA_TestApp.Utilities;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAA_TestApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CaaContext _context;

        public ProductsController(CaaContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string SearchName, int? CategoryID, int? OrganizeID, int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Product")
        {
            PopulateDropDownListCategories();
            PopulateDropDownListOrganizes();

            ViewData["Filtering"] = "btn-outline-secondary";

            string[] sortOptions = new[] { "Items", "Quantity Limit", "Category", "Organize" };


            var caaContext = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Organize)
                .AsNoTracking();
         
            if (CategoryID.HasValue)
            {
                caaContext = caaContext.Where(p => p.CategoryID == CategoryID);
                ViewData["Filtering"] = " btn-danger";
            }
            if (OrganizeID.HasValue)
            {
                caaContext = caaContext.Where(p => p.OrganizeID == OrganizeID);
                ViewData["Filtering"] = " btn-danger";
            }

            if (!String.IsNullOrEmpty(SearchName))
            {
                caaContext = caaContext.Where(p => p.Name.ToUpper().Contains(SearchName.ToUpper()));
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

            }
            //Now we know which field and direction to sort by
            if (sortField == "Organize")
            {
                if (sortDirection == "asc")
                {
                    caaContext = caaContext
                        .OrderBy(i => i.Organize.OrganizedBy);
                        
                }
                else
                {
                    caaContext = caaContext
                        .OrderByDescending(i => i.Organize.OrganizedBy);
                        
                }
            }
            else if (sortField == "Category")
            {
                if (sortDirection == "asc")
                {
                    caaContext = caaContext
                        
                        .OrderBy(i => i.Category.Classification);
                }
                else
                {
                    caaContext = caaContext
                        .OrderByDescending(i => i.Category.Classification);
                }
            }
            else if (sortField == "Quantity Limit")
            {
                if (sortDirection == "asc")
                {
                    caaContext = caaContext
                        .OrderByDescending(i => i.ParLevel);
                        
                }
                else
                {
                    caaContext = caaContext
                        .OrderBy(i => i.ParLevel);
                        
                }
            }
            else if (sortField == "Items")
            {
                if (sortDirection == "asc")
                {
                    caaContext = caaContext
                        .OrderByDescending(i => i.Name);
                      
                }
                else
                {
                    caaContext = caaContext
                        .OrderBy(i => i.Name);
                    
                }
            }
       
            
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for sorting options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "caaContext");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Product>.CreateAsync(caaContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Organize)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification", product.CategoryID);
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", product.OrganizeID);
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
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
        public async Task<IActionResult> Create([Bind("ID,Name,ParLevel,CategoryID,OrganizeID")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { product.ID });
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

            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification", product.CategoryID);
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", product.OrganizeID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(i => i.Category)
                .Include(i => i.Organize)
                .FirstOrDefaultAsync(i => i.ID == id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification", product.CategoryID);
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", product.OrganizeID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, Byte[] RowVersion)
        {
            var productToUpdate = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Organize)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(productToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            //try updating it with the values posted
            if (await TryUpdateModelAsync<Product>(productToUpdate, "", p => p.Name, p => p.ParLevel,
                p => p.CategoryID, p => p.OrganizeID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { productToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Product)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Product was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Product)databaseEntry.ToObject();
                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Classification", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.CategoryID != clientValues.CategoryID)
                        {
                            Product databaseProduct = await _context.Products.FirstOrDefaultAsync(i => i.ID == databaseValues.CategoryID);
                            ModelState.AddModelError("CategoryID", $"Current value: {databaseProduct?.Name}");
                        }
                        if (databaseValues.OrganizeID != clientValues.OrganizeID)
                        {
                            Product databaseProduct = await _context.Products.FirstOrDefaultAsync(i => i.ID == databaseValues.CategoryID);
                            ModelState.AddModelError("OrganizeID", $"Current value: {databaseProduct?.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Product List' hyperlink.");
                        productToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Products.CategoryID"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and category.");
                    }
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Products.OrganizeID"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records with the same name and organization method.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Please try again. " +
                        "If the problem persists, contact your systems administrator.");
                    }
                }
            }
            //return RedirectToAction(nameof(Index));

            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Classification", productToUpdate.CategoryID);
            ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", productToUpdate.OrganizeID);
            return View(productToUpdate);
        }


        //public async Task<IActionResult> Edit(int id, [Bind("ID,Name,ParLevel,CategoryID,OrganizeID")] Product product, Byte[] RowVersion)
        //{
        //    if (id != product.ID)
        //    {
        //        return NotFound();
        //    }

        //    //Put the original RowVersion value in the OriginalValues collection for the entity
        //    _context.Entry(product).Property("RowVersion").OriginalValue = RowVersion;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
        //                    + "was modified by another user. Please go back and refresh.");
        //            }
        //        }
        //        catch(DbUpdateException) 
        //        {
        //            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //        }
        //        return RedirectToAction("Details", new { product.ID });
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
        //    ViewData["OrganizeID"] = new SelectList(_context.Organizes, "ID", "OrganizedBy", product.OrganizeID);
        //    return View(product);
        //}

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Organize)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'CaaContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            try
            {
                if (product != null)
                {
                    _context.Products.Remove(product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Product. Remember, you cannot delete a Product that has Inventory assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(product);
        }

        private SelectList OrganizeSelectList(int? id)
        {
            var dQuery = from d in _context.Organizes
                         orderby d.OrganizedBy
                         select d;
            return new SelectList(dQuery, "ID", "OrganizedBy", id);
        }

        private SelectList CategorySelectList(int? id)
        {
            var dQuery = from d in _context.Categories
                         orderby d.Classification
                         select d;
            return new SelectList(dQuery, "ID", "Classification", id);
        }

        private void PopulateDropDownListCategories(Category category = null)
        {
            ViewData["CategoryID"] = CategorySelectList(category?.ID);
        }

        private void PopulateDropDownListOrganizes(Organize organize = null)
        {
            ViewData["OrganizeID"] = OrganizeSelectList(organize?.ID);
        }
       

        public IActionResult DownloadProducts()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var product = from a in _context.Products
                .Include(p => p.Category)
                .Include(p => p.Organize)
                          orderby a.Name descending
                          select new
                          {

                              Item = a.Name,
                              Category = a.Category.Classification
                          };

            int numRows = product.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Product");

                    workSheet.Cells[3, 1].LoadFromCollection(product, true);

                    workSheet.Cells[4, 1, numRows + 4, 1].Style.Font.Bold = true;

                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 2])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }
                    workSheet.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Product Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 2])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 2])
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
                        string filename = "Product.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data.");
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ID == id);
        }
    }
}
