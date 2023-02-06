using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAA_TestApp.Data;
using CAA_TestApp.Models;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using CAA_TestApp.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore.Storage;

namespace CAA_TestApp.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly CaaContext _context;

        public InventoriesController(CaaContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(string sortDirectionCheck, string sortFieldID, string SearchName, int? CategoryID, int? LocationID,
            int? page, string actionButton, int? pageSizeID, string sortDirection = "asc", string sortField = "Inventory")
        {
            PopulateDropDownListsCategories();
            PopulateDropDownListsLocations();

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Product", "Quantity", "Cost", "Location" };

            var inventories = _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .ThenInclude(c => c.Category)
                .AsNoTracking();

            if (CategoryID.HasValue)
            {
                inventories = inventories.Where(p => p.Product.CategoryID == CategoryID);
                ViewData["Filtering"] = "  show";
            }
            if (LocationID.HasValue)
            {
                inventories = inventories.Where(p => p.LocationID == LocationID);
                ViewData["Filtering"] = "  show";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                inventories = inventories.Where(p => p.Product.Name.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "  show";
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
                        .OrderBy(i => i.Location.Name)
                        .ThenBy(i => i.Product.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Location.Name)
                        .ThenBy(i => i.Product.Name);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Product.Name)
                        .ThenBy(i => i.Location.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Product.Name)
                        .ThenByDescending(i => i.Location.Name);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Quantity)
                        .ThenBy(i => i.Location.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Location.Name);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Cost)
                        .ThenBy(i => i.Location.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderBy(i => i.Cost)
                        .ThenBy(i => i.Location.Name);
                }
            }
            else if (sortField == "Category") //sort by category
            {
                if (sortDirection == "asc")
                {
                    if (sortDirection == "asc")
                    {
                        inventories = inventories
                            .OrderBy(i => i.Product.Category.Name);
                    }
                    else
                    {
                        inventories = inventories
                            .OrderByDescending(i => i.Product.Category.Name);
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

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "Name", inventory.Location);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.Product);
            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "Name");
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name");
            //Redirect("/Inventories/Index");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ISBN,Quantity,Notes,ShelfOn,Cost,DateReceived," +
            "LocationID,ProductID")] Inventory inventory, IFormFile thePicture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddPicture(inventory, thePicture);
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

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "Name", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.ProductID);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
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



            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "Name", inventory.Location);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.Product);
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

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventory inventory, string chkRemoveImage, IFormFile thePicture, Byte[] RowVersion)
        {
            var inventoryToUpdate = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .Include(i => i.ItemPhoto)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventoryToUpdate == null)
            {
                return NotFound();
            }

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(inventoryToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Inventory>(inventoryToUpdate, "",
                i => i.Quantity, i => i.Notes, i => i.ShelfOn, i => i.Cost,
                i => i.DateReceived, i => i.ISBN, i => i.LocationID, i => i.ProductID))
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
                            Location databaseLocation = await _context.Locations.FirstOrDefaultAsync(i => i.ID == databaseValues.LocationID);
                            ModelState.AddModelError("LocationID", $"Current value: {databaseLocation?.Name}");
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

            ViewData["LocationID"] = new SelectList(_context.Locations, "ID", "Name", inventory.LocationID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Name", inventory.ProductID);
            return View(inventoryToUpdate);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Location)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
            QRCodeData qrData = qrCodeGen.CreateQrCode(inventory.ISBN, QRCodeGenerator.ECCLevel.Q);
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

        private SelectList LocationSelectList(int? id)
        {
            var dQuery = from d in _context.Locations
                         orderby d.Name
                         select d;
            return new SelectList(dQuery, "ID", "Name", id);
        }

        private SelectList CategorySelectList(int? id)
        {
            var dQuery = from d in _context.Categories
                         orderby d.Name
                         select d;
            return new SelectList(dQuery, "ID", "Name", id);
        }

        private void PopulateDropDownListsLocations(Location location = null)
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
                             Location = a.Location.Name,
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

        private async Task AddPicture(Inventory inventory, IFormFile thePicture)
        {
            //Get the picture and save it with the Patient (2 sizes)
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
                            inventory.ItemPhoto.Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            inventory.ItemThumbnail = _context.ItemsThumbnails.Where(p => p.invID == inventory.ID).FirstOrDefault();
                            inventory.ItemThumbnail.Content = ResizeImage.shrinkImageWebp(pictureArray, 100, 120);
                        }
                        else //No pictures saved so start new
                        {
                            inventory.ItemPhoto = new ItemPhoto
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                            inventory.ItemThumbnail = new ItemThumbnail
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 100, 120),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }
    }
}
