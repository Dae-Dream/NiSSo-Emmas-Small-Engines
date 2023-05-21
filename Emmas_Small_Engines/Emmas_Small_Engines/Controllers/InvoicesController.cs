using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Emmas_Small_Engines.Data;
using Emmas_Small_Engines.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Emmas_Small_Engines.Utilities;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Sales")]
    public class InvoicesController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public InvoicesController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index(string SearchString, int? page,
            string actionButton, string sortDirection = "asc", string sortField = "Date")
        {
            
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //List of sort options
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Customer", "Date", "Employee", "Description" };

            var invoices = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .Include(i => i.InvoiceLines).ThenInclude(i => i.Inventory)
                .AsNoTracking();

            // Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchString))
            {
                invoices = invoices.Where(p => p.Customer.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Customer.FirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Employee.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Employee.FirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Description.ToUpper().Contains(SearchString.ToUpper())
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
            if (sortField == "Date")
            {
                if (sortDirection == "asc")
                {
                    invoices = invoices
                        .OrderByDescending(p => p.Date)
                        .ThenBy(p => p.Customer.FirstName)
                        .ThenBy(p => p.Customer.LastName);
                }
                else
                {
                    invoices = invoices
                        .OrderBy(p => p.Date)
                        .ThenBy(p => p.Customer.FirstName)
                        .ThenBy(p => p.Customer.LastName);
                }
            }
            else if (sortField == "Employee")
            {
                if (sortDirection == "asc")
                {
                    invoices = invoices
                        .OrderBy(p => p.Employee.FirstName.ToUpper())   // Without ToUpper() 'lower case a' goes after 'Z capital' because their ASCII values
                        .ThenBy(p => p.Employee.LastName.ToUpper())
                        .ThenBy(p => p.Customer.FirstName.ToUpper());
                }
                else
                {
                    invoices = invoices
                        .OrderByDescending(p => p.Employee.FirstName.ToUpper())
                        .ThenByDescending(p => p.Employee.LastName.ToUpper())
                        .ThenBy(p => p.Customer.FirstName.ToUpper());
                }
            }
            else if (sortField == "Description")
            {
                if (sortDirection == "asc")
                {
                    invoices = invoices
                        .OrderBy(p => p.Description.ToUpper())
                        .ThenBy(p => p.Customer.FirstName.ToUpper())
                        .ThenBy(p => p.Customer.LastName.ToUpper());
                }
                else
                {
                    invoices = invoices
                        .OrderByDescending(p => p.Description.ToUpper())
                        .ThenBy(p => p.Customer.FirstName.ToUpper())
                        .ThenBy(p => p.Customer.LastName.ToUpper());
                }
            }
            else    // Sorting by Customer
            {
                if (sortDirection == "asc")
                {
                    invoices = invoices
                        .OrderBy(p => p.Customer.FirstName.ToUpper())
                        .ThenBy(p => p.Customer.LastName.ToUpper())
                        .ThenBy(p => p.Employee.FirstName.ToUpper());
                }
                else
                {
                    invoices = invoices
                        .OrderByDescending(p => p.Customer.FirstName.ToUpper())
                        .ThenByDescending(p => p.Customer.LastName.ToUpper())
                        .ThenBy(p => p.Employee.FirstName.ToUpper());
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            var pagedData = await PaginatedList<Invoice>.CreateAsync(invoices.AsNoTracking(), page ?? 1, 5);

            return View(pagedData);
        }

        // GET: Invoices/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .Include(i => i.InvoicePayments).ThenInclude(i => i.Payment)
                .Include(i => i.InvoiceLines).ThenInclude(i => i.Inventory)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address");
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Date,Description,Appreciation,Subtotal,CustomerID,EmployeeID")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", invoice.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName", invoice.EmployeeID);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName", invoice.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FullName", invoice.EmployeeID);
            return View(invoice);
        }

        /*
        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Description,Appreciation,Subtotal,CustomerID,EmployeeID")] Invoice invoice)
        {
            if (id != invoice.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.ID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", invoice.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName", invoice.EmployeeID);
            return View(invoice);
        }
        */

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Invoices == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.Invoices'  is null.");
            }
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var invoiceToUpdate = await _context.Invoices
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.InvoiceLines).ThenInclude(a => a.Inventory)
                .Include(a => a.InvoicePayments).ThenInclude(a => a.Payment)
                .FirstOrDefaultAsync(b => b.ID == id);

            if (invoiceToUpdate == null) { return NotFound(); }

            if (await TryUpdateModelAsync<Invoice>(invoiceToUpdate, "",
                a => a.Date, a => a.Description, a => a.Appreciation, a => a.Subtotal,
                a => a.CustomerID, a => a.EmployeeID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoiceToUpdate.ID))
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
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateDropDownLists(invoiceToUpdate);
            return View(invoiceToUpdate);
        }

        public async Task<IActionResult> CreateInvoice([Bind("ID,Date,Description,Appreciation,Subtotal,CustomerID,EmployeeID")] Invoice invoice)
        {
            invoice.Date = DateTime.Today;
            invoice.Description = "Order on: " + DateTime.Now.ToString();

            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "POS", new {invoiceID = invoice.ID});
            }

            PopulateDropDownLists(invoice);

            return View(invoice);
        }

        public SelectList CustomerList(int? selectedID)
        {
            return new SelectList(_context.Customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName), "ID", "FullName", selectedID);
        }

        public SelectList EmployeeList(int? selectedID)
        {
            return new SelectList(_context.Employees.OrderBy(c => c.FirstName).ThenBy(c => c.LastName), "ID", "FullName", selectedID);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private void PopulateDropDownLists(Invoice invoice = null)
        {
            ViewData["CustomerID"] = CustomerList(invoice?.CustomerID);
            ViewData["EmployeeID"] = EmployeeList(invoice?.EmployeeID);
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.ID == id);
        }
    }
}
