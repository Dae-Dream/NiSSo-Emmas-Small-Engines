

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
    [Authorize(Roles = "Owner, Admin, Sales")]
    public class CustomersController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public CustomersController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int? page, string SearchString, string actionButton, string sortDirection="asc", string sortField ="First Name")
        {
            
            string[] sortOptions = new[] { "First Name", "Last Name", "Phone", "Address", "City", "Province", "Postal"};

            var customers = _context.Customers
                            .AsNoTracking();

           
            if (!String.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.Trim(' ');
                customers = customers.Where(customer => customer.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || customer.FirstName.ToUpper().Contains(SearchString.ToUpper()) || customer.Phone.ToUpper().Contains(SearchString.ToUpper()) || customer.Province.ToUpper().Contains(SearchString.ToUpper()) || customer.Address.ToUpper().Contains(SearchString.ToUpper()) || customer.City.ToUpper().Contains(SearchString.ToUpper()) || customer.Postal.ToUpper().Contains(SearchString.ToUpper()));
                
                
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked

                    
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            if (sortField == "First Name")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderBy(c => c.FirstName)
                        .ThenBy(c => c.LastName);
                }
                else
                {
                    customers = customers
                        .OrderByDescending(c => c.FirstName)
                        .ThenByDescending(c => c.LastName);
                }
            }
            else if (sortField == "Last Name")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.LastName);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.LastName);
                }
            }
            else if (sortField == "Phone")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.Phone);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.Phone);
                }
            }
            else if (sortField == "Address")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.Address);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.Address);
                }
            }
            else if (sortField == "City")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.City);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.City);
                }
            }
            else if (sortField == "Province")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.Province);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.Province);
                }
            }
            else if (sortField == "Postal")
            {
                if (sortDirection == "asc")
                {
                    customers = customers
                        .OrderByDescending(c => c.Postal);
                }
                else
                {
                    customers = customers
                        .OrderBy(c => c.Postal);
                }
            }





            //Set sort for next time


            int pageSize = 5;
            var pagedData = await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Address,City,Province,Postal")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.Phone != null)
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "POS");

                }
            }
                return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Phone,Address,City,Province,Postal")] Customer customer)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.ID))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return _context.Customers.Any(e => e.ID == id);
        }
        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
    }
}
