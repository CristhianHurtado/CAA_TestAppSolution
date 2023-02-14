using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CAA_TestApp.Data;
using CAA_TestApp.Models;

namespace CAA_TestApp.Controllers
{
    public class StatusController : Controller
    {
        private readonly CaaContext _context;

        public StatusController(CaaContext context)
        {
            _context = context;
        }

        // GET: Status
        public async Task<IActionResult> Index()
        {
              return View(await _context.statuses.ToListAsync());
        }

        // GET: Status/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.statuses == null)
            {
                return NotFound();
            }

            var status = await _context.statuses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // GET: Status/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,status")] Status status)
        {
            if (ModelState.IsValid)
            {
                _context.Add(status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }

        // GET: Status/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.statuses == null)
            {
                return NotFound();
            }

            var status = await _context.statuses.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }

        // POST: Status/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,status")] Status status)
        {
            if (id != status.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusExists(status.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }

        // GET: Status/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.statuses == null)
            {
                return NotFound();
            }

            var status = await _context.statuses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.statuses == null)
            {
                return Problem("Entity set 'CaaContext.statuses'  is null.");
            }
            var status = await _context.statuses.FindAsync(id);
            if (status != null)
            {
                _context.statuses.Remove(status);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatusExists(int id)
        {
          return _context.statuses.Any(e => e.ID == id);
        }
    }
}
