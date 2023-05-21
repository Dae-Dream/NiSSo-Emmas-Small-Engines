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
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Sales")]
    public class SalesReportsController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public SalesReportsController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: SalesReports
        public async Task<IActionResult> Index(string SearchString, int? page,
            string actionButton, string sortDirection = "asc", string sortField = "Date of creation")
        {
            PopulateDropDownLists();
            ViewDataReturnURL();

            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //List of sort options
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Report Name", "Date Start", "Date End", "Criteria", "Date of creation" };

            var salesReports = _context.SalesReports
                .Include(i => i.SalesReportEmps)
                .Include(i => i.SalesReportItems)
                .AsNoTracking();

            // Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchString))
            {
                salesReports = salesReports.Where(p => p.srepName.ToUpper().Contains(SearchString.ToUpper())
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
                    salesReports = salesReports
                        .OrderByDescending(p => p.StartDate)
                        .ThenBy(p => p.srepName.ToUpper());
                }
                else
                {
                    salesReports = salesReports
                        .OrderBy(p => p.StartDate)
                        .ThenBy(p => p.srepName.ToUpper());
                }
            }
            else if (sortField == "Date End")
            {
                if (sortDirection == "asc")
                {
                    salesReports = salesReports
                        .OrderByDescending(p => p.EndDate)
                        .ThenBy(p => p.srepName.ToUpper());
                }
                else
                {
                    salesReports = salesReports
                        .OrderBy(p => p.EndDate)
                        .ThenBy(p => p.srepName.ToUpper());
                }
            }
            else if (sortField == "Criteria")
            {
                if (sortDirection == "asc")
                {
                    salesReports = salesReports
                        .OrderBy(p => p.srepCriteria.ToUpper())
                        .ThenBy(p => p.srepName.ToUpper());
                }
                else
                {
                    salesReports = salesReports
                        .OrderByDescending(p => p.srepCriteria.ToUpper())
                        .ThenBy(p => p.srepName.ToUpper());
                }
            }
            else if (sortField == "Date of creation")
            {
                if (sortDirection == "asc")
                {
                    salesReports = salesReports
                        .OrderByDescending(p => p.TimeStamp)
                        .ThenBy(p => p.srepName.ToUpper());
                }
                else
                {
                    salesReports = salesReports
                        .OrderBy(p => p.TimeStamp)
                        .ThenBy(p => p.srepName.ToUpper());
                }
            }
            else    // Sorting by Report Name
            {
                if (sortDirection == "asc")
                {
                    salesReports = salesReports
                        .OrderBy(p => p.srepName.ToUpper())
                        .ThenBy(p => p.srepCriteria.ToUpper());
                }
                else
                {
                    salesReports = salesReports
                        .OrderByDescending(p => p.srepName.ToUpper())
                        .ThenBy(p => p.srepCriteria.ToUpper());
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            var pagedData = await PaginatedList<SalesReport>.CreateAsync(salesReports.AsNoTracking(), page ?? 1, 5);

            return View(pagedData);
        }

        // GET: SalesReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports
                .FirstOrDefaultAsync(m => m.ID == id);
            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }

        // SalesReport
        public async Task<IActionResult> SalesReport(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports
                .Include(i => i.SalesReportEmps)
                .Include(i => i.SalesReportItems)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }




        // AddReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReport(string actionButton,
                        [Bind("srepName,StartDate,EndDate,srepCriteria")] SalesReport salesReport)
        {
            ViewDataReturnURL();

            // Get all the invoices filtered by date and Employee
            var invoices = await _context.Invoices
                .Include(i => i.Employee)
                .Include(i => i.InvoicePayments).ThenInclude(i => i.Payment)
                .Include(i => i.InvoiceLines).ThenInclude(i => i.Inventory)
                .Where(i => i.Date >= salesReport.StartDate && i.Date <= salesReport.EndDate)
                .Where(i => salesReport.srepCriteria != null ? (i.Employee.FirstName + " " + i.Employee.LastName) == salesReport.srepCriteria : true)
                .ToListAsync();

            try
            {
                if (ModelState.IsValid)
                {
                    // 1st part
                    // Updates all the Subtotals on the selected Invoices 
                    foreach (var inv in invoices)
                        inv.Subtotal = invoices.SelectMany(i => i.InvoiceLines).Where(i => i.Invoice.ID == inv.ID).Sum(i => i.Quantity * i.SalePrice);

                    // Calculates the totals by Payment
                    salesReport.Cash = invoices.Where(i => i.InvoicePayments.FirstOrDefault().Payment.Type == "Cash").Sum(i => i.Subtotal);
                    salesReport.Debit = invoices.Where(i => i.InvoicePayments.FirstOrDefault().Payment.Type == "Debit").Sum(i => i.Subtotal);
                    salesReport.Credit = invoices.Where(i => i.InvoicePayments.FirstOrDefault().Payment.Type == "Credit").Sum(i => i.Subtotal);
                    salesReport.Cheque = invoices.Where(i => i.InvoicePayments.FirstOrDefault().Payment.Type == "Cheque").Sum(i => i.Subtotal);
                    salesReport.paymentTotal = salesReport.Cash + salesReport.Debit + salesReport.Credit + salesReport.Cheque;

                    // Calculates Appreciation
                    salesReport.AppreciationTotal = invoices.Sum(i => i.Subtotal * i.Appreciation);

                    _context.Add(salesReport);          // Adds the SalesReport
                    await _context.SaveChangesAsync();  // Saves changes to get the ID of the SalesReport

                    // 2nd part / Create child tables
                    // SalesReportEmp table
                    // Gets all the Employees IDs (distinct) present in the Invoices 
                    int[] empIDs = invoices
                        .GroupBy(e => e.EmployeeID)
                        .Select(group => group.First().EmployeeID).ToArray();

                    // Loops through all the Employees, gets their sales and creates their record
                    foreach (int emp in empIDs)
                    {
                        SalesReportEmp se = new SalesReportEmp()
                        {
                            EmpName = _context.Employees.FirstOrDefault(i => i.ID == emp).FullName,
                            EmpSales = invoices.Where(i => i.EmployeeID == emp).Sum(i => i.Subtotal),
                            SaleReportsID = salesReport.ID  // You won't have this ID until the SalesReport is created
                        };
                        _context.SalesReportEmps.Add(se);
                    }

                    // SalesReportItem table
                    // Gets all the Inventory IDs (distinct)
                    string[] prodIDs = invoices.SelectMany(i => i.InvoiceLines)
                                        .Select(il => il.InventoryID)
                                        .Distinct()
                                        .ToArray();

                    // Loops through all the Products, gets their sales and creates their record
                    foreach (string it in prodIDs)
                    {
                        SalesReportItem si = new SalesReportItem()
                        {
                            ItemName = _context.Inventories.FirstOrDefault(e => e.UPC == it).Name,
                            ItemQuantity = invoices.SelectMany(i => i.InvoiceLines).Where(i => i.InventoryID == it).Sum(i => i.Quantity),
                            ItemTotal = invoices.SelectMany(i => i.InvoiceLines).Where(i => i.InventoryID == it).Sum(i => i.Quantity * i.SalePrice),
                            SalesReportID = salesReport.ID  // You won't have this ID until the SalesReport is created
                        };
                        _context.SalesReportItems.Add(si);
                    }

                    await _context.SaveChangesAsync();  // Saves the new records

                    if (actionButton == "Save")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("SalesReport", new { salesReport.ID }); // "Prints" the Receipt
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator");
            }

            PopulateDropDownLists();
            return RedirectToAction("Index");
        }

        // GET: SalesReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SalesReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,srepName,StartDate,EndDate,srepCriteria,Cash,Debit,Credit,Cheque,paymentTotal,SaleTax,OtherTax,TotalTax,AppreciationTotal,TimeStamp")] SalesReport salesReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesReport);
        }

        // GET: SalesReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports.FindAsync(id);
            if (salesReport == null)
            {
                return NotFound();
            }
            return View(salesReport);
        }

        // POST: SalesReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,srepName,StartDate,EndDate,srepCriteria," +
            "Cash,Debit,Credit,Cheque,paymentTotal,SaleTax,OtherTax,TotalTax,TimeStamp")] SalesReport salesReport)
        {
            if (id != salesReport.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesReportExists(salesReport.ID))
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
            return View(salesReport);
        }

        // GET: SalesReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.SalesReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.SalesReports
                .Include(i => i.SalesReportEmps)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }

        // POST: SalesReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewDataReturnURL();

            if (_context.SalesReports == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.SalesReports'  is null.");
            }
            var salesReport = await _context.SalesReports.FindAsync(id);

            try
            {
                if (salesReport != null)
                {
                    _context.SalesReports.Remove(salesReport);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator");
            }
            return View(salesReport);
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
            return new SelectList(_context.Employees
                .OrderBy(e => e.FirstName).ThenBy(e => e.LastName), "FullName", "FullName", selectedId);
        }

        private void PopulateDropDownLists(SalesReport salesReport = null)
        {
            ViewData["srepCriteria"] = CriteriaList(salesReport?.srepCriteria);
        }

        private bool SalesReportExists(int id)
        {
            return _context.SalesReports.Any(e => e.ID == id);
        }
    }
}
