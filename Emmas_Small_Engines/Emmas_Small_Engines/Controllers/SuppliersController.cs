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
	[Authorize(Roles = "Owner, Admin, Sales, Technician, Ordering")]
	public class SuppliersController : Controller
	{
		private readonly EmmaSmallEngineContext _context;

		public SuppliersController(EmmaSmallEngineContext context)
		{
			_context = context;
		}

		// GET: Suppliers
		public async Task<IActionResult> Index(int? page, string searchString, string actionButton, string sortDirection = "asc", string sortField = "Name")
		{
			string[] sortOptions = new[] { "Name", "City", "Province" };
			CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            var suppliers = _context.Suppliers
				.AsNoTracking();

			if (!String.IsNullOrEmpty(searchString))
			{
				searchString = searchString.Trim(' ');
				suppliers = suppliers.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) || s.Email.ToUpper().Contains(searchString.ToUpper())
				|| s.Address.ToUpper().Contains(searchString.ToUpper()) || s.City.ToUpper().Contains(searchString.ToUpper()) || s.Province.ToUpper().Contains(searchString.ToUpper())
				|| s.Postal.ToUpper().Contains(searchString.ToUpper()));
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

			if (sortField == "Name")
			{
				if (sortDirection == "asc")
				{
					suppliers = suppliers.OrderBy(s => s.Name);
				}
				else
				{
					suppliers = suppliers.OrderByDescending(s => s.Name);
				}
			}
			else if (sortField == "City")
			{
				if (sortDirection == "asc")
				{
					suppliers = suppliers.OrderBy(s => s.City);
				}
				else
				{
					suppliers = suppliers.OrderByDescending(s => s.City);
				}
			}
			else if (sortField == "Province")
			{
				if (sortDirection == "asc")
				{
					suppliers = suppliers.OrderBy(s => s.Province);
				}
				else
				{
					suppliers = suppliers.OrderByDescending(s => s.Province);
				}
			}

			var pagedData = await PaginatedList<Supplier>.CreateAsync(suppliers.AsNoTracking(), page ?? 1, 5);

			return View(pagedData);
		}

		// GET: Suppliers/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Suppliers == null)
			{
				return NotFound();
			}

			var supplier = await _context.Suppliers
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.ID == id);
			if (supplier == null)
			{
				return NotFound();
			}

			return View(supplier);
		}

		// GET: Suppliers/Create
		public IActionResult Create()
		{
            ViewDataReturnURL();
            return View();
		}

		// POST: Suppliers/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,Name,Phone,Email,Address,City,Province,Postal")] Supplier supplier)
		{
            //ViewDataReturnURL();
            /*
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
            */
            try
			{
				if (ModelState.IsValid)
				{
					_context.Add(supplier);
					await _context.SaveChangesAsync();
                    //return Redirect(ViewData["returnURL"].ToString());
                    return RedirectToAction(nameof(Index));
                }
			}
			catch (DbUpdateException dex)
			{
				//ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Suppliers.Phone") ||
					dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Suppliers.Email"))
				{
					if (dex.GetBaseException().Message.Contains("Phone"))
					{
						ModelState.AddModelError("Phone", "Unable to save changes. Supplier with that phone number already exists.");
					}
					else
					{
						ModelState.AddModelError("Email", "Unable to save changes. Supplier with that email already exists.");
					}
				}
				else
				{
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				}
			}

			return View(supplier);
		}

		// GET: Suppliers/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
            ViewDataReturnURL();
            if (id == null || _context.Suppliers == null)
			{
				return NotFound();
			}

			var supplier = await _context.Suppliers.FindAsync(id);
			if (supplier == null)
			{
				return NotFound();
			}
			return View(supplier);
		}

		// POST: Suppliers/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Phone,Email,Address,City,Province,Postal")] Supplier supplier)
		{
            ViewDataReturnURL();
            /*
            if (id != supplier.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.ID))
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
            return View(supplier);
            */
            var supplierToUpdate = await _context.Suppliers.SingleOrDefaultAsync(s => s.ID == id);

			if (supplierToUpdate == null)
			{
				return NotFound();
			}

			if (await TryUpdateModelAsync<Supplier>(supplierToUpdate, "", p => p.Name, p => p.Phone, p => p.Email, p => p.Address, p => p.City,
				p => p.Province, p => p.Postal))
			{
				try
				{
					await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
				catch (DbUpdateConcurrencyException)
				{
					if (!SupplierExists(supplierToUpdate.ID))
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
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
				}
			}
			return View(supplierToUpdate);
		}

		// GET: Suppliers/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Suppliers == null)
			{
				return NotFound();
			}

			var supplier = await _context.Suppliers
				.FirstOrDefaultAsync(m => m.ID == id);
			if (supplier == null)
			{
				return NotFound();
			}

			return View(supplier);
		}

		// POST: Suppliers/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Suppliers == null)
			{
				return Problem("Entity set 'EmmaSmallEngineContext.Suppliers'  is null.");
			}
			var supplier = await _context.Suppliers.FindAsync(id);
			if (supplier != null)
			{
				_context.Suppliers.Remove(supplier);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private string ControllerName()
		{
			return this.ControllerContext.RouteData.Values["controller"].ToString();
		}

		private void ViewDataReturnURL()
		{
			ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
		}

		private bool SupplierExists(int id)
		{
			return _context.Suppliers.Any(e => e.ID == id);
		}
	}
}
