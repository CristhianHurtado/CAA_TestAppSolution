using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAA_TestApp.Data;
using CAA_TestApp.Models;
using Microsoft.EntityFrameworkCore.Storage;
using CAA_TestApp.ViewModels;
using CAA_TestApp.Utilities;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using Location = CAA_TestApp.Models.Location;
using NuGet.Packaging;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CAA_TestApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly CaaContext _context;

        private Random _random = new Random();

        private int InUseToken;
        public EventsController(CaaContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var @event = _context.Events
                .Include(i => i.EventInventories).ThenInclude(i => i.Inventory).ThenInclude(i => i.Product)
                .AsNoTracking();

            return View(await @event.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(i => i.EventInventories)
                .ThenInclude(i => i.Inventory).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var @event = new Event();
            PopulateAssignedInventoryData(@event);

            ViewData["conText"] = _context.Inventories.Include(i => i.Product).Include(i => i.Location).OrderBy(i => i.ProductID).ToList();
            ViewData["caa"] = _context;

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,Title,Date,EventLocation,Notes")] Event @event, string[] selectedOptions, string dataInfo, string locations)
        {
            /*
             * 1). loop through selectedOptions to get the products from inventory
             * 2). Filter them with locations to get the inventory to get the items out
             * 3). use the same code for create a copy of the item as I did in sendInv in invetoriesController to create the In use inv
             * 4). add them to DB, update Existing inv, and saveChanges
             * 5). Filter the context for inventory that its in use and its related to the @event name
             * 6). add totalQuantity to @event looping through the quan array and add its values
             * 7). execute the foreach loop after the comment of @event.quantity... using the IDs filtered in step 5 as iterable
             * 8). add and savechanges
            */
            try
            {
                Dictionary<string, List<List<string>>> Info = JsonConvert.DeserializeObject<Dictionary<string, List<List<string>>>>(dataInfo);

                var myArray = _context.Locations;

                ModelState.SetModelValue("locations", new ValueProviderResult(locations));

                string[] ActualLocations = locations.Split(',');

                var filteredArray = myArray.Where(i => ActualLocations.Contains(i.City)).ToArray();


                if (selectedOptions != null)
                {
                    //info is a dictionary taht the key is the productID and then has an array as valuea that contains [locations, quantityTotake]
                    //structure: {"productID":[["location.City", "quantityTotake"], ["location.city", "quantityTotake"]]} all values are structured as strings so convertion is required
                    /*
                     use the location and productId to modify that item and change its quantity, u can use a loop to use it and the lenght of the vlaue as iterable
                     */

                    //gets and deparate data from dictionary
                    string[] keysFromDic = Info.Keys.ToArray();

                    int[] productsFromDic = Array.ConvertAll(keysFromDic, k => int.Parse(k));

                    //variaqbles for event data
                    int totalQuanInEvent = 0;
                    List<Inventory> newInventoryForEvent = new List<Inventory>();

                    //converts arrays to numbers for filtering
                    int[] selectOptInNumbers = selectedOptions.Select(int.Parse).ToArray();
                    //int[] locationsIDinNumber = ActualLocations.Select(int.Parse).ToArray();
                    int[] locationsIDinNumber = filteredArray.Select(i => i.ID).ToArray();
                    int isInStock = _context.statuses.FirstOrDefault(i => i.status == "In stock").ID;

                    //gets all inv
                    List<Inventory> inv = _context.Inventories.Include(i => i.Location).Include(i => i.Status).Where(i => selectOptInNumbers.Contains(i.ProductID) && locationsIDinNumber.Contains(i.LocationID) && i.statusID == isInStock).ToList();

                    /*filters inventory/*
                    List<Inventory> productFilter = inv.Where(i => selectOptInNumbers.Contains(i.ProductID)).ToList();
                    List<Inventory> locationFilter = productFilter.Where(i => locationsIDinNumber.Contains(i.LocationID)).ToList();
                    List<Inventory> statusFilter = locationFilter.Where(i => i.statusID == isInStock).ToList();*/

                    //checks for no matching values
                    //var unmatchedProduct = selectOptInNumbers.Except(productFilter.Select(i => i.ProductID)).ToArray();
                    //var unmatchedLocation = locationsIDinNumber.Except(locationFilter.Select(i => i.LocationID)).ToArray();

                    if (inv.Count <= 0)
                    {
                        return NotFound();
                    }

                    for (int dicPosition = 0; dicPosition < Info.Count; dicPosition++)
                    {
                        InUseToken = _random.Next(201, 1000);

                        string auxISBN = GenerateISBN();

                        int itemInUse = productsFromDic[dicPosition];
                        List<Inventory> matchingKey = inv.Where(i => i.ProductID == itemInUse).ToList();

                        //int dicAux = dicPosition + 1;
                        List<List<string>> data = Info[keysFromDic[dicPosition].ToString()];


                        for (int i = 0; i < data.Count; i++)
                        {
                            Inventory ANTinv = new Inventory();

                            //[['location', 'takingquan']]
                            string[] eachData = data[i].ToArray();
                            if (eachData[1] == "0")
                            {
                                continue;
                            }

                            int locationTaken = _context.Locations.FirstOrDefault(i => i.City == eachData[0]).ID;
                            Inventory invForMod = matchingKey.FirstOrDefault(i => i.LocationID == locationTaken);

                            ANTinv = inv.FirstOrDefault(i => i.LocationID == invForMod.LocationID);

                            Inventory eventInv = new Inventory
                            {
                                ISBN = $"{ANTinv.ISBN} {auxISBN} {InUseToken}",
                                ProductID = ANTinv.ProductID,
                                LocationID = ANTinv.LocationID,
                                Notes = $"Taken from {ANTinv.Location.City}",
                                ShelfOn = "In use",
                                Cost = ANTinv.Cost,
                                DateReceived = @event.Date,
                                Quantity = Convert.ToInt32(eachData[1]),
                                ItemPhoto = ANTinv.ItemPhoto,
                                ItemThumbnail = ANTinv.ItemThumbnail,
                                QRImage = ANTinv.QRImage,
                                EventInventories = ANTinv.EventInventories,
                                statusID = _context.statuses.FirstOrDefault(i => i.status == "In use").ID,
                            };
                            newInventoryForEvent.Add(eventInv);
                            ANTinv.Quantity -= Convert.ToInt32(eachData[1]);
                            totalQuanInEvent += Convert.ToInt32(eachData[1]);
                            _context.Update(ANTinv);
                            _context.Add(eventInv);
                            await _context.SaveChangesAsync();
                        }



                    }
                    @event.Quantity = totalQuanInEvent;
                    foreach (var item in newInventoryForEvent)
                    {
                        var itemToAdd = new EventInventory { EventID = @event.ID, InventoryID = int.Parse(item.ID.ToString()) };
                        @event.EventInventories.Add(itemToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists contacts an Administrator");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contacts an Administrator");
            }

            PopulateAssignedInventoryData(@event);
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(i => i.EventInventories)
                .ThenInclude(i => i.Inventory)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            var eventToUpdate = await _context.Events
                .Include(e => e.EventInventories)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (id != eventToUpdate.ID)
            {
                return NotFound();
            }

            //try updating with values posted
            if (await TryUpdateModelAsync<Event>(eventToUpdate, "",
                i => i.Title , i => i.Quantity, i => i.Date, i => i.EventLocation, i => i.Notes))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists contacts an Administrator");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contacts an Administrator");
                }
            }
            return View(eventToUpdate);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'CaaContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.ID == id);
        }

        private void PopulateAssignedInventoryData(Event @event)
        {
            var allOptions = _context.Products;
            var currentOptionsIDs = new HashSet<int>(@event.EventInventories.Select(i => i.InventoryID));
            var checkBoxes = new List<CheckOptionVM>();

            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.Name,
                    Assigned = currentOptionsIDs.Contains(option.ID)
                });
            }
            ViewData["Locations"] = new SelectList(_context.Locations, "ID", "City");
            ViewData["InventoryOptions"] = checkBoxes;
        }

        public async Task<IActionResult> EventReports(int? page, int? pageSizeID)
        {


            var sumQ = _context.Events
                .Include(a => a.EventInventories)
                .ThenInclude(a => a.Inventory)
                .ThenInclude(a => a.Product)

                .GroupBy(c => new { c.ID, c.Title, c.Quantity, c.Date, c.EventLocation, c.Notes })
                .Select(grp => new EventReportsVM
                {
                    ID = grp.Key.ID,
                    Name = grp.Key.Title,
                    Quantity = grp.Key.Quantity,
                    Date = grp.Key.Date,
                    EventLocation = grp.Key.EventLocation,
                    Notes = grp.Key.Notes


                }).OrderBy(s => s.Name);

            //Handle paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "EventReports");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<EventReportsVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

        }

        public IActionResult DownloadEventReports()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var intory = from a in _context.Events
                .Include(a => a.EventInventories)
                .ThenInclude(a => a.Inventory)
                         orderby a.Title descending
                         select new
                         {
                             Name = a.Title,
                             Quantity = a.Quantity,
                             Date = a.Date.ToShortDateString(),
                             EventLocation = a.EventLocation,
                             Notes = a.Notes

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
    }
}
