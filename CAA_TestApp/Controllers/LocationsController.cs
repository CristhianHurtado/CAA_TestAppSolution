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

namespace CAA_TestApp.Controllers
{
    public class LocationsController : Controller
    {
        private readonly CaaContext _context;

        public LocationsController(CaaContext context)
        {
            _context = context;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
              return View(await _context.Locations.ToListAsync());
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Locations == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,City,Phone,Address,PostalCode")] Location location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(location);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { location.ID });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Locations.Name"))
                {
                    ModelState.AddModelError("", "Unable to save changes. You cannot have duplicate records of locations.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Locations.Phone"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Locations cannot have the same phone number.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Locations.Address"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Locations cannot have the same address.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Locations.PostalCode"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Locations cannot have the same postal code.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Please try again. " +
                    "If the problem persists, contact your systems administrator.");
                }
            }

            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Locations == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion)
        {
            var locationToUpdate = await _context.Locations
                .FirstOrDefaultAsync(i => i.ID == id);

            if (locationToUpdate == null)
            {
                return NotFound();
            }

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(locationToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            //Try updating with the values posted
            if (await TryUpdateModelAsync<Location>(locationToUpdate, "", i => i.City,
                i => i.Phone, i => i.Address, i => i.PostalCode))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { locationToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Location)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Location was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Location)databaseEntry.ToObject();
                        if (databaseValues.City != clientValues.City)
                            ModelState.AddModelError("City", "Current value: "
                                + databaseValues.City);
                        if (databaseValues.Address != clientValues.Address)
                            ModelState.AddModelError("Address", "Current value: "
                                + databaseValues.Address);
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + databaseValues.Phone);
                        if (databaseValues.PostalCode != clientValues.PostalCode)
                            ModelState.AddModelError("PostalCode", "Current value: "
                                + databaseValues.Phone);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Location List' hyperlink.");
                        locationToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(locationToUpdate);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Locations == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Locations == null)
            {
                return Problem("Entity set 'CaaContext.Locations'  is null.");
            }
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
          return _context.Locations.Any(e => e.ID == id);
        }
    }
}
