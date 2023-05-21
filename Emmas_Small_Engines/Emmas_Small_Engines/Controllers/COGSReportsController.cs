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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Emmas_Small_Engines.Controllers
{
    /*[Authorize(Roles = "Owner, Admin, Sales")]*/
    public class COGSReportsController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public ICollection<Inventory> invNames { get; set; } = new HashSet<Inventory>();
        static string err = "";

        double mpp;
        double mpe;

        public COGSReportsController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: 
        public async Task<IActionResult> Index(string SearchString, int? page,
            string actionButton, string sortDirection = "asc", string sortField = "Report Name")
        {
            PopulateDropDownLists();
            ViewDataReturnURL();

            ViewBag.err = err;
            ViewBag.invNames = invNames;

            

            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //List of sort options
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Report Name", "Date Start", "Date End", "Criteria", "Date of creation" };

            var cogsReports = _context.COGSReports
                .Include(i => i.Invoice).ThenInclude(i => i.InvoiceLines)
                .Include(j => j.Inventory).ThenInclude(j => j.Prices)
                .AsNoTracking();

            // Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchString))
            {
                cogsReports = cogsReports.Where(p => p.cogName.ToUpper().Contains(SearchString.ToUpper())
                                                    //                                                    || p.srepCriteria.ToUpper().Contains(SearchString.ToUpper())
                                                    );
            }
            // Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) // Form Submitted!
            {
                page = 1;   // Reset page to start

                if (sortOptions.Contains(actionButton)) // Change of sort is requested
                {
                    if (actionButton == sortField)      // Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;           // Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Date Start")
            {
                if (sortDirection == "asc")
                {
                    cogsReports = cogsReports
                        .OrderByDescending(p => p.StartDate)
                        .ThenBy(p => p.cogName.ToUpper());
                }
                else
                {
                    cogsReports = cogsReports
                        .OrderBy(p => p.StartDate)
                        .ThenBy(p => p.cogName.ToUpper());
                }
            }
            else if (sortField == "Date End")
            {
                if (sortDirection == "asc")
                {
                    cogsReports = cogsReports
                        .OrderByDescending(p => p.EndDate)
                        .ThenBy(p => p.cogName.ToUpper());
                }
                else
                {
                    cogsReports = cogsReports
                        .OrderBy(p => p.EndDate)
                        .ThenBy(p => p.cogName.ToUpper());
                }
            }
            else if (sortField == "Criteria")
            {
                if (sortDirection == "asc")
                {
                    cogsReports = cogsReports
                        .OrderBy(p => p.crepCriteria.ToUpper())
                        .ThenBy(p => p.cogName.ToUpper());
                }
                else
                {
                    cogsReports = cogsReports
                        .OrderByDescending(p => p.crepCriteria.ToUpper())
                        .ThenBy(p => p.cogName.ToUpper());
                }
            }
            else if (sortField == "Date of creation")
            {
                if (sortDirection == "asc")
                {
                    cogsReports = cogsReports
                        .OrderByDescending(p => p.TimeStamp)
                        .ThenBy(p => p.cogName.ToUpper());
                }
                else
                {
                    cogsReports = cogsReports
                        .OrderBy(p => p.TimeStamp)
                        .ThenBy(p => p.cogName.ToUpper());
                }
            }
            else    // Sorting by Report Name
            {
                if (sortDirection == "asc")
                {
                    cogsReports = cogsReports
                        .OrderBy(p => p.cogName.ToUpper())
                        .ThenBy(p => p.crepCriteria.ToUpper());
                }
                else
                {
                    cogsReports = cogsReports
                        .OrderByDescending(p => p.cogName.ToUpper())
                        .ThenBy(p => p.crepCriteria.ToUpper());
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            var pagedData = await PaginatedList<COGSReport>.CreateAsync(cogsReports.AsNoTracking(), page ?? 1, 5);

            return View(pagedData);
        }

        // GET: HourlyReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.COGSReports == null)
            {
                return NotFound();
            }

            var cogsReport = await _context.COGSReports
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cogsReport == null)
            {
                return NotFound();
            }

            return View(cogsReport);
        }

        // HourlyReport
        public async Task<IActionResult> COGSReport(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.COGSReports == null)
            {
                return NotFound();
            }

            var cogsReport = await _context.COGSReports
                .Include(i => i.Invoice).ThenInclude(i => i.InvoiceLines)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cogsReport == null)
            {
                return NotFound();
            }

            return View(cogsReport);
        }

        // AddReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReport(string actionButton, string purchases, string endTotal,
                        [Bind("cogName,StartDate,EndDate,crepCriteria")] COGSReport cogsReport)
        {
            ViewDataReturnURL();
            //get Inventories
            var invTotal = await _context.Inventories
                .ToListAsync();
            if (cogsReport.crepCriteria != null)
            {
                invTotal = await _context.Inventories
                .Where(i => cogsReport.crepCriteria != null ? i.Name == cogsReport.crepCriteria : true)
                .ToListAsync();
            }
            else
            {
                cogsReport.crepCriteria = "All Inventory";
            }
           

           //get invoicelines
           var periodLines = await _context.InvoiceLines
                .ToListAsync();

            //get invoices
            var periodInvoice = await _context.Invoices
                .ToListAsync();

            double itemCostTotal = 0;
            if (cogsReport.crepCriteria != "All Inventory")
            {
                foreach (var invLine in periodLines)
                {

                    if (invLine.Inventory.Name == cogsReport.crepCriteria)
                    {

                        itemCostTotal += invLine.Inventory.MarkupPrice;
                    }
                }
            }
            else
            {
                foreach (var invLine in periodLines)
                {

                   

                        itemCostTotal += invLine.Inventory.MarkupPrice;
                    
                }
            }
            Console.WriteLine(itemCostTotal);
            //Total cost of all inventory
            double invTotalCost = 0;
            //set materialstart, materialpurchases, and period ned
            foreach(var item in invTotal)
            {
                invTotalCost += item.AdjustPrice;
                cogsReport.InventoryID = item.UPC;
            }
            try
            {
                 mpp = Double.Parse(purchases);
                 mpe = Double.Parse(endTotal);
            }
            catch
            {
                err = "Purchases and End of Period amount must both be in valid currency format.";
                return RedirectToAction("Index");

            }
            // double mpp = 500;
            // double mpe = 1000;


            //total Sales revenue
            double sr = itemCostTotal;
            foreach(var item in periodInvoice)
            {
                cogsReport.InvoiceID = item.ID;
            }

            double COGS = invTotalCost + mpp - mpe;

            double grossProfit = sr - COGS;

            double margin = grossProfit / COGS;

            //add above to report
            cogsReport.MaterialPeriodStart = invTotalCost;
            cogsReport.MaterialPeriodPurchase = mpp;
            cogsReport.MaterialPeriodEnd = mpe;

            cogsReport.COGS = COGS;

            cogsReport.SaleRevenuePeriod = sr;

            cogsReport.GrossProfit = grossProfit;

            cogsReport.ProfitMargin = margin;

            _context.Add(cogsReport);
            await _context.SaveChangesAsync();  // Saves the new records
            err = "";
            if (actionButton == "Save")
                return RedirectToAction("Index");
            else
                return RedirectToAction("COGSReport", new { cogsReport.ID }); // "Prints" the Receipt



            PopulateDropDownLists();
            return RedirectToAction("Index");
        }

        // GET: HourlyReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HourlyReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("cogName, StartDate, EndDate, crepCriteria, TimeStamp")] COGSReport cogsReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cogsReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cogsReport);
        }

        // GET: HourlyReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.COGSReports == null)
            {
                return NotFound();
            }

            var cogsReport = await _context.COGSReports.FindAsync(id);
            if (cogsReport == null)
            {
                return NotFound();
            }
            return View(cogsReport);
        }

        // POST: HourlyReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID, hourName, StartDate, EndDate, crepCriteria, TimeStamp")] COGSReport cogsReport)
        {
            if (id != cogsReport.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cogsReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!COGSReportExists(cogsReport.ID))
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
            return View(cogsReport);
        }

        // GET: HourlyReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.COGSReports == null)
            {
                return NotFound();
            }

            var cogsReport = await _context.COGSReports
                .Include(i => i.Invoice).ThenInclude(i => i.InvoiceLines)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cogsReport == null)
            {
                return NotFound();
            }

            return View(cogsReport);
        }

        // POST: HourlyReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewDataReturnURL();

            if (_context.COGSReports == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.COGSReports'  is null.");
            }
            var cogsReport = await _context.COGSReports.FindAsync(id);


            if (cogsReport != null)
            {
                _context.COGSReports.Remove(cogsReport);
            }
            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());

            return View(cogsReport);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private SelectList CriteriaList(string selectedId)
        {
            return new SelectList(_context.Inventories
                .OrderBy(e => e.Name).ThenBy(e => e.Stock), "Name", "Name", selectedId);
        }

        private void PopulateDropDownLists(COGSReport cogsReport = null)
        {
            ViewData["crepCriteria"] = CriteriaList(cogsReport?.crepCriteria);
        }

        private bool COGSReportExists(int id)
        {
            return _context.COGSReports.Any(e => e.ID == id);
        }
    }
}
