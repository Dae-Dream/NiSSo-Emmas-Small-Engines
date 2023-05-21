using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Emmas_Small_Engines.Data;
using Emmas_Small_Engines.Models;
using Emmas_Small_Engines.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Technician, Ordering, Sales")]
    public class InventoriesController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public InventoriesController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(int? page, string searchString, string actionButton, string sortDirection = "asc", string sortField = "Name")
        {
            string[] sortOptions = new[] { "UPC", "Adjust Price", "Markup Price", "Current", "Name", "Stock"};
            
            var inventories = _context.Inventories
                            .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim(' ');
                inventories = inventories.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) || s.UPC.ToUpper().Contains(searchString.ToUpper())
                || s.Size.ToUpper().Contains(searchString.ToUpper()) || s.Quantity.ToUpper().Contains(searchString.ToUpper())
                || s.Stock.ToString().ToUpper().Contains(searchString.ToUpper()));

            }


            if (!String.IsNullOrEmpty(actionButton)) 
            {
                page = 1;

                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) 
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;


                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            if (sortField == "UPC")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.UPC)
                        .ThenBy(i => i.Name);

                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.UPC)
                        .ThenBy(i => i.Name);
                }
            }
            else if (sortField == "Current")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Current)
                        .ThenBy(i => i.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Current)
                        .ThenBy(i => i.Name);
                }
            }
            else if (sortField == "Adjust Price")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.AdjustPrice)
                        .ThenBy(i => i.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.AdjustPrice)
                        .ThenBy(i => i.Name);
                }
            }
            else if (sortField == "Markup Price")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.MarkupPrice)
                        .ThenBy(i => i.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.MarkupPrice)
                        .ThenBy(i => i.Name);
                }
            }
            else if (sortField == "Stock")
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Stock)
                        .ThenBy(i => i.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.MarkupPrice)
                        .ThenBy(i => i.Name);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    inventories = inventories
                        .OrderBy(i => i.Name);
                }
                else
                {
                    inventories = inventories
                        .OrderByDescending(i => i.Name);
                }
            }

            int pageSize = 5;
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);

        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.UPC == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UPC,Name,Size,Quantity,AdjustPrice,MarkupPrice,Current, Stock")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
               
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UPC,Name,Size,Quantity,AdjustPrice,MarkupPrice,Current, Stock")] Inventory inventory)
        {
            if (id != inventory.UPC)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.UPC))
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
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.UPC == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Inventory'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(string id)
        {
          return _context.Inventories.Any(e => e.UPC == id);
        }
        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
    }
}
