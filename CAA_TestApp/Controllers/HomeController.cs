using CAA_TestApp.Data;
using CAA_TestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CAA_TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CaaContext _context;
   
        public HomeController(ILogger<HomeController> logger, CaaContext context)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            CheckDateForTakeInvBasedOnEvent();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //this is for the Help Centre page 
        public ActionResult NavigateHome()
        {
            return View("HelpCentre");
        }

        public async void CheckDateForTakeInvBasedOnEvent()
        {
            List<Event> events = await _context.Events
                                    .Include(e => e.EventInventories)
                                    .ThenInclude(ei => ei.Inventory)
                                    .ToListAsync();

            if (events == null)
            {
                return;
            }

            List<Inventory> eventInfo = new List<Inventory> ();

            List<Inventory> invsInDb = await _context.Inventories
                                        .Include(i => i.Product)
                                        .Include(i => i.Location)
                                        .ToListAsync();

            List<Inventory> invToModifyList = new List<Inventory> ();
            List<Inventory> invToModify = new List<Inventory>();

            foreach (Event @event in events)
            {

                if(@event.Date <= DateTime.Now)
                {
                    int index = 0;

                    foreach (var eventInvInfo in @event.EventInventories)
                    {
                        Inventory inv = await _context.Inventories.Include(i => i.Product).Include(i => i.Location).FirstOrDefaultAsync(i => i.ID == eventInvInfo.InventoryID);
                        eventInfo.Add(inv);
                    }

                    foreach (var eventInv in eventInfo)
                    {
                        Inventory aux = invsInDb.FirstOrDefault(i => i.ProductID == eventInv.ProductID && i.LocationID == eventInv.LocationID);
                        invToModifyList.Add(aux);
                    }

                    foreach(Inventory inv in invToModifyList)
                    {
                        if(inv.statusID != 1)
                        {
                            continue;
                        }
                        invToModify.Add(inv);
                    }

                    foreach(var eventInv in eventInfo)
                    {
                        Inventory evntInvInfo = await _context.Inventories.FirstOrDefaultAsync(i => i.ID == eventInv.ID);
                        if (evntInvInfo.statusID == _context.statuses.FirstOrDefault(s => s.status == "In use").ID)
                        {
                            continue;
                        }
                        
                        evntInvInfo.statusID = _context.statuses.FirstOrDefault(s => s.status == "In use").ID;
                        evntInvInfo.ShelfOn = "In use";
                        invToModify[index].Quantity -= evntInvInfo.Quantity;
                        _context.Update(evntInvInfo);
                        _context.Update(invToModify[index]);
                        index++;
                    }                    
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}