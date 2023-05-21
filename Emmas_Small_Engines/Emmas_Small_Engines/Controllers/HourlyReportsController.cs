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

namespace Emmas_Small_Engines.Controllers
{
    /*[Authorize(Roles = "Owner, Admin, Sales")]*/
    public class HourlyReportsController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public HourlyReportsController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: HourlyReports
        public async Task<IActionResult> Index(string SearchString, int? page,
            string actionButton, string sortDirection = "asc", string sortField = "Report Name")
        {
            PopulateDropDownLists();
            ViewDataReturnURL();

            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //List of sort options
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Report Name", "Date Start", "Date End", "Criteria", "Date of creation" };

            var hourlyReports = _context.HourlyReports
                .Include(i => i.EmpLogins).ThenInclude(i => i.Employee)
                .AsNoTracking();

            // Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchString))
            {
                hourlyReports = hourlyReports.Where(p => p.hourName.ToUpper().Contains(SearchString.ToUpper())
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
                    hourlyReports = hourlyReports
                        .OrderByDescending(p => p.StartDate)
                        .ThenBy(p => p.hourName.ToUpper());
                }
                else
                {
                    hourlyReports = hourlyReports
                        .OrderBy(p => p.StartDate)
                        .ThenBy(p => p.hourName.ToUpper());
                }
            }
            else if (sortField == "Date End")
            {
                if (sortDirection == "asc")
                {
                    hourlyReports = hourlyReports
                        .OrderByDescending(p => p.EndDate)
                        .ThenBy(p => p.hourName.ToUpper());
                }
                else
                {
                    hourlyReports = hourlyReports
                        .OrderBy(p => p.EndDate)
                        .ThenBy(p => p.hourName.ToUpper());
                }
            }
            else if (sortField == "Criteria")
            {
                if (sortDirection == "asc")
                {
                    hourlyReports = hourlyReports
                        .OrderBy(p => p.hrepCriteria.ToUpper())
                        .ThenBy(p => p.hourName.ToUpper());
                }
                else
                {
                    hourlyReports = hourlyReports
                        .OrderByDescending(p => p.hrepCriteria.ToUpper())
                        .ThenBy(p => p.hourName.ToUpper());
                }
            }
            else if (sortField == "Date of creation")
            {
                if (sortDirection == "asc")
                {
                    hourlyReports = hourlyReports
                        .OrderByDescending(p => p.TimeStamp)
                        .ThenBy(p => p.hourName.ToUpper());
                }
                else
                {
                    hourlyReports = hourlyReports
                        .OrderBy(p => p.TimeStamp)
                        .ThenBy(p => p.hourName.ToUpper());
                }
            }
            else    // Sorting by Report Name
            {
                if (sortDirection == "asc")
                {
                    hourlyReports = hourlyReports
                        .OrderBy(p => p.hourName.ToUpper())
                        .ThenBy(p => p.hrepCriteria.ToUpper());
                }
                else
                {
                    hourlyReports = hourlyReports
                        .OrderByDescending(p => p.hourName.ToUpper())
                        .ThenBy(p => p.hrepCriteria.ToUpper());
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            var pagedData = await PaginatedList<HourlyReport>.CreateAsync(hourlyReports.AsNoTracking(), page ?? 1, 5);

            return View(pagedData);
        }

        // GET: HourlyReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HourlyReports == null)
            {
                return NotFound();
            }

            var salesReport = await _context.HourlyReports
                .FirstOrDefaultAsync(m => m.ID == id);
            if (salesReport == null)
            {
                return NotFound();
            }

            return View(salesReport);
        }

        // HourlyReport
        public async Task<IActionResult> HourlyReport(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.HourlyReports == null)
            {
                return NotFound();
            }

            var hourlyReport = await _context.HourlyReports
                .Include(i => i.EmpLogins).ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (hourlyReport == null)
            {
                return NotFound();
            }

            return View(hourlyReport);
        }

        // AddReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReport(string actionButton,
                        [Bind("hourName,StartDate,EndDate,hrepCriteria")] HourlyReport hourlyReport)
        {
            ViewDataReturnURL();

            // Get all the empLogins filtered by date and Employee
            var empLogins = await _context.EmpLogins
                .Where(i => i.SignIn >= hourlyReport.StartDate && i.SignOut <= hourlyReport.EndDate)
                .Where(i => hourlyReport.hrepCriteria != null ? (i.Employee.FirstName + " " + i.Employee.LastName) == hourlyReport.hrepCriteria : true)
                .ToListAsync();


                    hourlyReport.EmpLogins = empLogins;
                    hourlyReport.hour = empLogins.Select(e => e.Hours).Sum();


                    _context.Add(hourlyReport);
                    await _context.SaveChangesAsync();  // Saves the new records

                    if (actionButton == "Save")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("HourlyReport", new { hourlyReport.ID }); // "Prints" the Receipt



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
        public async Task<IActionResult> Create([Bind("hourName, StartDate, EndDate, hrepCriteria, TimeStamp")] HourlyReport hourlyReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hourlyReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hourlyReport);
        }

        // GET: HourlyReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HourlyReports == null)
            {
                return NotFound();
            }

            var hourlyReport = await _context.HourlyReports.FindAsync(id);
            if (hourlyReport == null)
            {
                return NotFound();
            }
            return View(hourlyReport);
        }

        // POST: HourlyReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID, hourName, StartDate, EndDate, hrepCriteria, TimeStamp")] HourlyReport hourlyReport)
        {
            if (id != hourlyReport.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hourlyReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HourlyReportExists(hourlyReport.ID))
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
            return View(hourlyReport);
        }

        // GET: HourlyReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.HourlyReports == null)
            {
                return NotFound();
            }

            var hourlyReport = await _context.HourlyReports
                .Include(i => i.EmpLogins).ThenInclude(i => i.Employee)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (hourlyReport == null)
            {
                return NotFound();
            }

            return View(hourlyReport);
        }

        // POST: HourlyReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewDataReturnURL();

            if (_context.HourlyReports == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.HourlyReports'  is null.");
            }
            var hourlyReport = await _context.HourlyReports.FindAsync(id);


                if (hourlyReport != null)
                {
                    _context.HourlyReports.Remove(hourlyReport);
                }
                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());

            return View(hourlyReport);
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

        private void PopulateDropDownLists(SalesReport hourlyReport = null)
        {
            ViewData["hrepCriteria"] = CriteriaList(hourlyReport?.srepCriteria);
        }

        private bool HourlyReportExists(int id)
        {
            return _context.HourlyReports.Any(e => e.ID == id);
        }
    }
}
