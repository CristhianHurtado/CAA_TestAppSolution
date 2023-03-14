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

namespace CAA_TestApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly CaaContext _context;

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
        public IActionResult Create()
        {
            var @event = new Event();
            PopulateAssignedInventoryData(@event);

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Date,EventLocation,Notes")] Event @event, string[] selectedOptions, int[] quan, string[] locations)
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
                if(selectedOptions != null)
                {
                        //@event.Quantity = the sum of all values inside quan;
                    foreach(var item in selectedOptions)
                    {
                        var itemToAdd = new EventInventory { EventID = @event.ID, InventoryID = int.Parse(item)};
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(i=>i.EventInventories)
                .ThenInclude(i=>i.Inventory)
                .ThenInclude(i=>i.Product)
                .FirstOrDefaultAsync(i=>i.ID == id);
    
            if (@event == null)
            {
                return NotFound();
            }
            PopulateAssignedInventoryData(@event);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            var eventToUpdate = await _context.Events
                .Include(i =>i.EventInventories)
                .ThenInclude(i =>i.Inventory)
                .ThenInclude(i =>i.Product)
                .FirstOrDefaultAsync(i => i.ID== id);

            if (id != eventToUpdate.ID) 
            {
                return NotFound();
            }

            //Update inventory items
            UpdateEventInventory(selectedOptions, eventToUpdate);

            //try updating with values posted
            if(await TryUpdateModelAsync<Event>(eventToUpdate, "",
                i => i.Title, i => i.Quantity, i => i.Date, i => i.EventLocation, i => i.Notes)) 
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
           
            PopulateAssignedInventoryData(eventToUpdate);
            return View(eventToUpdate);
        }

        // GET: Events/Delete/5
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
            var currentOptionsIDs = new HashSet<int>(@event.EventInventories.Select(i=> i.InventoryID));
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

        private void UpdateEventInventory(string[] selectedOptions, Event eventToUpdate)
        {
            if(selectedOptions == null)
            {
                eventToUpdate.EventInventories = new List<EventInventory>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var eventOptionsHS = new HashSet<int>
                (eventToUpdate.EventInventories.Select(i => i.InventoryID)); //id if currently selected inventory item

            foreach (var option in _context.Inventories) 
            {
                if (selectedOptionsHS.Contains(option.ID.ToString())) //it is checked
                {
                    if(!eventOptionsHS.Contains(option.ID))
                    {
                        eventToUpdate.EventInventories.Add(new EventInventory {EventID = eventToUpdate.ID, InventoryID = option.ID});
                    }
                }
                else
                {
                    //not checked
                    if (eventOptionsHS.Contains(option.ID))
                    {
                        EventInventory inventoryToRemove = eventToUpdate.EventInventories.SingleOrDefault(i => i.InventoryID == option.ID);
                    }
                }
            }
        }

      
    }
}
