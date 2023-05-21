using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Emmas_Small_Engines.Data;
using Emmas_Small_Engines.Models;
using Emmas_Small_Engines.ViewModels;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage;
using Emmas_Small_Engines.Utilities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authorization;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Ordering, Technician")]
    public class OrderRequestsController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public OrderRequestsController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: OrderRequests
        public async Task<IActionResult> Index(int? page, string searchString, string actionButton, string sortDirection = "asc", string sortField = "ID")
        { 
            
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);
           
            string[] sortOptions = new[] { "ID", "Description", "SentDate", "ReceiveDate", "ExternalOrderNumber", "Customer" };


            var orderRequests = _context.OrderRequests
                .Include(o => o.Customer)
                .Include(o => o.OrderRequest_Inventories).ThenInclude(oi => oi.Inventory)
                .AsNoTracking();

            

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim(' ');


                orderRequests = orderRequests.Where(or => or.ExternalOrderNumber.Contains(searchString.ToUpper()));


                /*orderRequests = orderRequests.Where(s => s.OrderRequest_Inventories.Any(i => i.InventoryID == inventories.Any(i => I))); */


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
     
            if (sortField == "Description")
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.Description);
                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.Description);
                }
            }
            else if (sortField == "SentDate")
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.SentDate);
                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.SentDate);
                }
            }
            else if (sortField == "ReceiveDate")
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.ReceiveDate);
                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.ReceiveDate);
                }
            }
            else if (sortField == "ExternalOrderNumber")
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.ExternalOrderNumber);
                        //Customer.FirstName)
                        //.ThenBy(i => i.Customer.LastName);
                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.ExternalOrderNumber);
                }
            }
            else if (sortField == "Customer")
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.Customer.LastName);

                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.Customer.LastName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    orderRequests = orderRequests
                        .OrderBy(i => i.ID);
                }
                else
                {
                    orderRequests = orderRequests
                        .OrderByDescending(i => i.ID);
                }
            }


            int pageSize = 5;
            var pagedData = await PaginatedList<OrderRequest>.CreateAsync(orderRequests.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: OrderRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests
                .Include(o => o.Customer)
                .Include(o => o.OrderRequest_Inventories).ThenInclude(oi => oi.Inventory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderRequest == null)
            {
                return NotFound();
            }

            return View(orderRequest);
        }

        // GET: OrderRequests/Create
        public IActionResult Create()
        {
            ViewDataReturnURL();
            OrderRequest orderRequest = new OrderRequest();
            orderRequest.ExternalOrderNumber= "EM-"+GenerateNextEON();
            orderRequest.CustomerID = 2;
            ViewData["orderAmount"] = new MultiSelectList(new List<ListOptionVM>(), "ID", "DisplayText");
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName", orderRequest.CustomerID);
			PopulateAssignedInventoryData(orderRequest);
            return View();
        }

        // POST: OrderRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNumber,CustomerID")] OrderRequest orderRequest, string[] selectedOptions, string[] orderAmt)
        {
            ViewData["orderAmt"] = new MultiSelectList(new List<ListOptionVM>(), "ID", "DisplayText");
            try
            {
                PopulateAssignedInventoryData(orderRequest);
                UpdateOrderRequestInventories(selectedOptions, orderRequest, orderAmt);
                orderRequest.CustomerID = 2;
                orderRequest.ExternalOrderNumber = "EM-" + GenerateNextEON();
                if (ModelState.IsValid)
                {
                    _context.Add(orderRequest);
					await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName", orderRequest.CustomerID);
            //ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Name", "Select a supplier");
            GenerateNextEON();
            return View(orderRequest);
        }

        // GET: OrderRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["orderAmt"] = new MultiSelectList(new List<ListOptionVM>(), "ID", "DisplayText");
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests
            .Include(o => o.OrderRequest_Inventories).ThenInclude(o => o.Inventory)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.ID == id);

            if (orderRequest == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName", orderRequest.CustomerID);
            PopulateAssignedInventoryData(orderRequest);
            GenerateNextEON();
            //UpdateOrderRequestInventories()
            return View(orderRequest);
        }

        // POST: OrderRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNumber,CustomerID")] OrderRequest orderRequest, string[] selectedOptions, string[] orderAmt)            
        {
            var orderRequestToUpdate = await _context.OrderRequests
                .Include(o => o.OrderRequest_Inventories).ThenInclude(o => o.Inventory)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (id != orderRequestToUpdate.ID)
            {
                return NotFound();
            }
            
            


            if (ModelState.IsValid)
            { 
                try
                {
                    _context.Update(orderRequestToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderRequestExists(orderRequestToUpdate.ID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName", orderRequest.CustomerID);
            GenerateNextEON();
            ViewData["orderAmt"] = new MultiSelectList(new List<ListOptionVM>(), "ID", "DisplayText");
            PopulateAssignedInventoryData(orderRequestToUpdate);
            UpdateOrderRequestInventories(selectedOptions, orderRequestToUpdate, orderAmt);

            return View(orderRequestToUpdate);
        }

        // GET: OrderRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests
                .Include(o => o.Customer)
                .Include(o => o.OrderRequest_Inventories).ThenInclude(oi => oi.Inventory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderRequest == null)
            {
                return NotFound();
            }

            return View(orderRequest);
        }

        // POST: OrderRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderRequests == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.OrderRequests'  is null.");
            }
            var orderRequest = await _context.OrderRequests.FindAsync(id);
            if (orderRequest != null)
            {
                _context.OrderRequests.Remove(orderRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //-------------SEND ORDER AND ORDER RECEIVED BUTTON METHODS---------------
        
        public async Task<IActionResult> Send(int id)
        {
            ViewDataReturnURL();
            foreach (var item in _context.OrderRequests)
            {
                if (item.ID == id && item.SentDate == null)
                {
                    item.SentDate = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());
        }
  
        //Receive Task does update the Receive date of the order requests, however for some reason it isnt updating inventory stock
        public async  Task<IActionResult> Receive(int id)
        {
            ViewDataReturnURL();
            foreach (var item in _context.OrderRequests)
            {
                if (item.ID == id && item.ReceiveDate == null)
                {
                    item.ReceiveDate = DateTime.Now;
                    foreach (var oiItem in item.OrderRequest_Inventories) {
                        var itemToUpdate = await _context.Inventories.FirstOrDefaultAsync(i=> i.UPC == oiItem.Inventory.UPC); //Find the matching inventory item
                        if(itemToUpdate != null)
                        {
                            itemToUpdate.Stock = itemToUpdate.Stock + oiItem.OrderAmount;
                           
                        }
                        else
                        {
                            return NotFound();
                        }
                       
                    }                                      
                }               
            }            
            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString()); 
        }
        //Cancel order function
        //removes the sent date of an order which mimicks cancelling an order, allows you to remove an order from the list after being sent
        public async Task<IActionResult> Cancel_Order(int id)
        {
            ViewDataReturnURL();
            foreach (var item in _context.OrderRequests)
            {
                if (item.ID == id && item.SentDate != null)
                {
                    item.SentDate = null;
                    
                }
            }
            await _context.SaveChangesAsync();
            return Redirect(ViewData["returnURL"].ToString());
        }

        //Generates the next external order number from the last added order request.
        private int GenerateNextEON() 
        {
			int NextExtOrdNum = Convert.ToInt32(_context.OrderRequests.AsNoTracking().OrderBy(eon => eon.ExternalOrderNumber).Last().ExternalOrderNumber.Remove(0, 3)) + 1;
            int currentOrdNum = Convert.ToInt32(_context.OrderRequests.AsNoTracking().OrderBy(eon => eon.ExternalOrderNumber).Last().ExternalOrderNumber.Remove(0, 3));
            ViewBag.ExtOrdNum = "EM-" + currentOrdNum;
            ViewBag.NextExtOrdNum = "EM-" + NextExtOrdNum;
            ViewBag.Inventory = _context.Inventories.OrderBy(o=>o.Name).AsNoTracking().ToArray();

            return NextExtOrdNum;
        }
     
        //------------------------------------------------------------------------------------------------
        private void PopulateAssignedInventoryData(OrderRequest order)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Inventories;
            var currentOptionsHS = new HashSet<string>(order.OrderRequest_Inventories.Select(o => o.InventoryID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var i in allOptions)
            {
                if (currentOptionsHS.Contains(i.UPC))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = i.UPC,
                       
                        DisplayText = i.Name
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = i.UPC,
                        DisplayText = i.Name
                    });
                }
            }
            ViewBag.selected = selected;
            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(i => i.DisplayText), "ID", "DisplayText");
            //ViewData["availOpts"] = new MultiSelectList(available.OrderBy(i => i.DisplayText), "ID", "DisplayText");
        }
        private void UpdateOrderRequestInventories(string[] selectedOptions, OrderRequest orderToUpdate, string[] orderAmt)
        {
            if (selectedOptions == null)
            {
                orderToUpdate.OrderRequest_Inventories = new List<OrderRequest_Inventory>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<string>(orderToUpdate.OrderRequest_Inventories.Select(b => b.InventoryID));
            var orderAmtHS = new HashSet<string>(orderAmt);

            
            int index = 1;
            foreach (var i in _context.Inventories)
            {
                if (selectedOptionsHS.Contains(i.UPC))//it is selected
                {
                    if (!currentOptionsHS.Contains(i.UPC))//but not currently in the OrderRequest's collection - Add it!
                    {
                        int orderQty = Convert.ToInt32(orderAmtHS.ElementAt(index));
                        orderToUpdate.OrderRequest_Inventories.Add(new OrderRequest_Inventory
                        {
                            InventoryID = i.UPC,
                            OrderAmount = orderQty,
                            OrderRequestID = orderToUpdate.ID
                        });
                        index++;
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(i.UPC))//but is currently in the OrderRequest's collection - Remove it!
                    {
                        OrderRequest_Inventory invToRemove = orderToUpdate.OrderRequest_Inventories.FirstOrDefault(d => d.InventoryID == i.UPC);
                        _context.Remove(invToRemove);
                    }
                }
            }
        }
        
        private bool OrderRequestExists(int id)
        {
          return _context.OrderRequests.Any(e => e.ID == id);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }
    }
}
