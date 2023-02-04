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
    public class OrganizesController : Controller
    {
        private readonly CaaContext _context;

        public OrganizesController(CaaContext context)
        {
            _context = context;
        }

        // GET: Organizes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Organizes.ToListAsync());
        }

        // GET: Organizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Organizes == null)
            {
                return NotFound();
            }

            var organize = await _context.Organizes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (organize == null)
            {
                return NotFound();
            }

            return View(organize);
        }

        // GET: Organizes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Organizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrganizedBy")] Organize organize)
        {
            if (ModelState.IsValid)
            {
                _context.Add(organize);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { organize.ID });
            }
            return View(organize);
        }

        // GET: Organizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Organizes == null)
            {
                return NotFound();
            }

            var organize = await _context.Organizes.FindAsync(id);
            if (organize == null)
            {
                return NotFound();
            }
            return View(organize);
        }

        // POST: Organizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrganizedBy")] Organize organize)
        {
            if (id != organize.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizeExists(organize.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { organize.ID });
            }
            return View(organize);
        }

        // GET: Organizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Organizes == null)
            {
                return NotFound();
            }

            var organize = await _context.Organizes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (organize == null)
            {
                return NotFound();
            }

            return View(organize);
        }

        // POST: Organizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Organizes == null)
            {
                return Problem("Entity set 'CaaContext.Organizes'  is null.");
            }
            var organize = await _context.Organizes.FindAsync(id);
            if (organize != null)
            {
                _context.Organizes.Remove(organize);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizeExists(int id)
        {
          return _context.Organizes.Any(e => e.ID == id);
        }
    }
}
