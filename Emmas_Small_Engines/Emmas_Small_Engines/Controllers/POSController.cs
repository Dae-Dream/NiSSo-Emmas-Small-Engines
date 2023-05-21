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
using Microsoft.AspNetCore.Http;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Sales")]
    public class POSController : Controller
    {
        private readonly EmmaSmallEngineContext _context;
        
        static double subtotal = 0;
        static double tax = 0;
        static double total = 0;
        static double paid = 0;
        static double change = 0;

        
		static string errMsg = "";
        //static string custCheck = "Select Customer";
        static string custCheck = "Pay";

		static string payment = "Cash";
        

        static List<CartItem> _CartItems = new List<CartItem>();
        
        static List<Payment> _PaymentTypes = new List<Payment>();
        
        static int InvoiceID;

        public POSController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: POS
        public async Task<IActionResult> Index(int? page, string SearchString, int? invoiceID)
        {
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            /*
			if (invoiceID != null)
            {
				Invoice invoice1 = await _context.Invoices.FirstOrDefaultAsync(c => c.ID == invoiceID);
				InvoiceID = invoice1.ID;
			}
            if(invoiceID == -1)
            {
				custCheck = "Pay";
				return RedirectToAction("CreateInvoice", "Invoices");
			}
            */

            ViewBag.Subtotal = subtotal.ToString("C"); 
            ViewBag.Tax = tax.ToString("C"); 
            ViewBag.Total = total.ToString("C");
            ViewBag.Paid = paid;
            ViewBag.Change = change;

            ViewBag.Payment = payment;

            ViewBag.model = _CartItems;

            ViewBag.errMsg = errMsg;

            ViewBag.custCheck = custCheck;

            var orders = _context.Inventories.AsNoTracking();

            if (!String.IsNullOrEmpty(SearchString))
            {
                orders = orders.Where(p => p.Name.ToUpper().Contains(SearchString.ToUpper()));
            }
            Invoice invoice = new Invoice();

			//Add Employee Automatically by assigning it to the currently logged in user
			//For now, im trying to use a drop down list on the page
			ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FullName");
            
			//Do the same with customers
			ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName");

			var pagedData = await PaginatedList<Inventory>.CreateAsync(orders.AsNoTracking(), page ?? 1, 5);

            return View(pagedData);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(string upc)
        {
            errMsg = "";
			ViewDataReturnURL();

			Inventory product = await _context.Inventories.FirstOrDefaultAsync(p => p.UPC == upc);
            CartItem CartItem = _CartItems.Find(c => c.InventoryUPC == product.UPC);

            if (product == null)
            {
                errMsg = "Product not found";
				return RedirectToAction("Index", "POS");
			}

            if (product.Stock == 0)
            {
				errMsg = "Product out of stock";
				return RedirectToAction("Index", "POS");
			}

            if (CartItem == null)
            {
                CartItem = new CartItem
                {
                    InventoryUPC = product.UPC,
                    Inventory = product,
                    Price = product.MarkupPrice,
                    OrderQuantity = 1
                };

                _CartItems.Add(CartItem);
                product.Stock--;

				subtotal += Math.Round(CartItem.Price, 2);
				tax = Math.Round(subtotal * 0.13, 2);
				total = Math.Round(subtotal + tax, 2);

				ViewBag.Subtotal = subtotal.ToString("C");
				ViewBag.Tax = tax.ToString("C");
				ViewBag.Total = total.ToString("C");
			}
            else
            {
				errMsg = "Item already added";
				return RedirectToAction("Index", "POS");
			}

            await _context.SaveChangesAsync();
			return Redirect(ViewData["returnURL"].ToString());
		}

        public async Task<IActionResult> UpdateQuantity(string upc, int quantity)
        {
            var CartItem = _CartItems.Find(c => c.InventoryUPC == upc);
            Inventory product = await _context.Inventories.FirstOrDefaultAsync(c => c.UPC == CartItem.InventoryUPC);

            subtotal = 0;

            if (CartItem != null)
            {

                if (quantity > CartItem.OrderQuantity)
                {
                    if (product.Stock == 0)
                    {
						errMsg = "Product out of stock.";
						return RedirectToAction("Index", "POS");
					}
                    else
                    {
                        product.Stock -= quantity - CartItem.OrderQuantity;
                    }
                }
                else
                {
                    product.Stock += Math.Abs(quantity - CartItem.OrderQuantity);
                }

                await _context.SaveChangesAsync();
                CartItem.OrderQuantity = quantity;

                foreach (var c in _CartItems)
                {
					subtotal += Math.Round(c.Price * c.OrderQuantity, 2);
				}

				tax = Math.Round(subtotal * 0.13, 2);
				total = subtotal + tax;
			}

			ViewBag.Subtotal = subtotal.ToString("C");
			ViewBag.Tax = tax.ToString("C");
			ViewBag.Total = total.ToString("C");

			return RedirectToAction("Index");
		}

        [HttpPost]
        public async Task<ActionResult> RemoveFromCart(string upc)
        {
			ViewDataReturnURL();

            CartItem productToRemove = _CartItems.Find(c => c.InventoryUPC == upc);

            Inventory product = await _context.Inventories.FirstOrDefaultAsync(p => p.UPC == upc);

			if (product == null)
			{
				errMsg = "Product not found.";
				return RedirectToAction("Index", "POS");
			}

            product.Stock += productToRemove.OrderQuantity;

            await _context.SaveChangesAsync();

            _CartItems.Remove(productToRemove);

			subtotal = 0;

			foreach (var c in _CartItems)
			{
				subtotal += Math.Round(c.Price * c.OrderQuantity, 2);
			}

			tax = Math.Round(subtotal * 0.13, 2);

			total = Math.Round(subtotal + tax, 2);

			ViewBag.Subtotal = Math.Round(subtotal);
			ViewBag.Tax = Math.Round(tax, 2); ;
			ViewBag.Total = Math.Round(total);

            ViewBag.Payment = payment;

			return Redirect(ViewData["returnURL"].ToString());
		}

		[HttpPost]
        public async Task<ActionResult> Checkout(string paymentAmt)
        {
            errMsg = "";
            // Grab invoice record, if not found, prompt to make a new one.
            //Invoice invoice = await _context.Invoices.FirstOrDefaultAsync(a => a.ID == InvoiceID);
            /*
            if (custCheck == "Select Customer")
            {
                custCheck = "Pay";
                return RedirectToAction("CreateInvoice", "Invoices");
            }
            if (invoice == null )
            {
                custCheck = "Pay";
                return RedirectToAction("CreateInvoice", "Invoices");
            }
            */

            // Create a payment object from selected payment type.
            var paymentObj = (from c in _context.Payments
                              where c.Type == payment
                              select c).FirstOrDefault();

            int paymentID = paymentObj.ID;

            // If a payment amount was entered and can be interpreted as a double.
            if (paymentAmt != null && double.TryParse(paymentAmt, out double payAmt))
            {
                // If the payment amount entered is less than total (Split payments)
				if (double.Parse(paymentAmt) < total)
				{
					if (_PaymentTypes.Any(x => x.ID == paymentObj.ID))
					{
						errMsg = "Payment already used.";
						return RedirectToAction("Index", "POS");
					}
					else
					{
						_PaymentTypes.Add(paymentObj);
					}

					double amtPaid = double.Parse(paymentAmt);

					paid += amtPaid;
					total -= amtPaid;

                    total = Math.Round(total, 2);

					subtotal = 0;
					tax = 0;

					ViewBag.Paid = paid.ToString("C");
					ViewBag.Total = total.ToString("C");
					ViewBag.Subtotal = subtotal.ToString("C");
					ViewBag.Tax = tax.ToString("C");

                    if (paid >= total + paid)
                    {
                        custCheck = "Checkout";
                    }

                    return RedirectToAction("Index", "POS");
				}
				else if (double.Parse(paymentAmt) > total)
				{
					if (_PaymentTypes.Any(x => x.ID == paymentObj.ID))
					{
						errMsg = "Payment already used.";
						return RedirectToAction("Index", "POS");
					}
					else
					{
                        _PaymentTypes.Add(paymentObj);
					}

					double amtPaid = double.Parse(paymentAmt);
                    
                    paid += amtPaid;
					change = amtPaid - total;

					ViewBag.Paid = paid.ToString("C");
					ViewBag.Change = change.ToString("C");

                    if (paid >= total)
                    {
                        custCheck = "Checkout";
                    }

                    return RedirectToAction("Index", "POS");
				}
                else
                {
                    if (_PaymentTypes.Any(x => x.ID == paymentObj.ID))
                    {
						errMsg = "Payment already used";
						return RedirectToAction("Index", "POS");
					}
                    else
                    {
                        _PaymentTypes.Add(paymentObj);
                    }

					int custID = int.Parse(Request.Form["SelectedCustID"]);
					Customer cust = await _context.Customers.FirstOrDefaultAsync(c => c.ID == custID);

                    int empID = int.Parse(Request.Form["SelectedEmpID"]);
                    Employee emp = await _context.Employees.FirstOrDefaultAsync(e => e.ID == empID);

                    Invoice invoice = new Invoice
                    {
                        CustomerID = cust.ID,
                        EmployeeID = emp.ID,
						Date = DateTime.Today,
					    Description = "Order on: " + DateTime.Now.ToString()
				    };

                    _context.Invoices.Add(invoice);

					foreach (var cartItem in _CartItems)
                    {
                        InvoiceLine invoiceLine = new InvoiceLine
                        {
                            InvoiceID = invoice.ID,
                            InventoryID = cartItem.InventoryUPC,
                            Inventory = await _context.Inventories.FindAsync(cartItem.InventoryUPC),
                            Invoice = await _context.Invoices.FindAsync(invoice.ID),
                            SalePrice = cartItem.Price,
                            Quantity = cartItem.OrderQuantity
                        };

                        invoice.InvoiceLines.Add(invoiceLine);
                    }

                    foreach (var paymentType in _PaymentTypes)
                    {
                        InvoicePayment invoicePayment = new InvoicePayment
                        {
                            InvoiceID = InvoiceID,
                            PaymentID = paymentType.ID
                        };

                        invoice.InvoicePayments.Add(invoicePayment);
                    }

                    _CartItems.Clear();
                    _PaymentTypes.Clear();

                    subtotal = 0;
                    tax = 0;
                    total = 0;
                    paid = 0;
                    change = 0;

                    await _context.SaveChangesAsync();
                    
                    custCheck = "Pay";
					//InvoiceID = -1;
					
                    return RedirectToAction("Receipt", "Invoices", invoice);
                }
			}
            else
            {
                if (change != 0)
                {
					int custID = int.Parse(Request.Form["SelectedCustID"]);
					Customer cust = await _context.Customers.FirstOrDefaultAsync(c => c.ID == custID);

					int empID = int.Parse(Request.Form["SelectedEmpID"]);
					Employee emp = await _context.Employees.FirstOrDefaultAsync(e => e.ID == empID);

					Invoice invoice = new Invoice
					{
						CustomerID = cust.ID,
						EmployeeID = emp.ID,
						Date = DateTime.Today,
						Description = "Order on: " + DateTime.Now.ToString()
					};

					_context.Invoices.Add(invoice);

					foreach (var cartItem in _CartItems)
                    {
                        InvoiceLine invoiceLine = new InvoiceLine
                        {
                            InvoiceID = invoice.ID,
                            InventoryID = cartItem.InventoryUPC,
                            Inventory = await _context.Inventories.FindAsync(cartItem.InventoryUPC),
                            Invoice = await _context.Invoices.FindAsync(invoice.ID),
                            SalePrice = cartItem.Price,
                            Quantity = cartItem.OrderQuantity
                        };

                        invoice.InvoiceLines.Add(invoiceLine);
                    }

                    foreach (var paymentType in _PaymentTypes)
                    {
                        InvoicePayment invoicePayment = new InvoicePayment
                        {
                            InvoiceID = invoice.ID,
                            PaymentID = paymentType.ID
                        };

                        invoice.InvoicePayments.Add(invoicePayment);
                    }

                    _CartItems.Clear();
                    _PaymentTypes.Clear();

                    subtotal = 0;
                    tax = 0;
                    total = 0;
                    paid = 0;
                    change = 0;

                    InvoiceID = 0;

                    await _context.SaveChangesAsync();
                    //InvoiceID = -1;
					custCheck = "Pay";

					return RedirectToAction("Receipt", "Invoices", invoice);
                }

                if (paymentAmt == null)
                {
                    errMsg = "Please enter a payment amount.";
                    return RedirectToAction("Index", "POS");
                }

                return RedirectToAction("Index", "POS");
            }
        }
        public async Task<ActionResult> selCust()
        {
            custCheck = "Pay";
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateInvoice", "Invoices");
        }
        public async Task<ActionResult> Cash()
		{
			payment = "Cash";

			ViewBag.Payment = payment;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "POS");
		}
		public async Task<ActionResult> Debit()
		{
			payment = "Debit";

			ViewBag.Payment = payment;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "POS");
		}
		public async Task<ActionResult> Credit()
		{
			payment = "Credit";

			ViewBag.Payment = payment;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "POS");
		}
		public async Task<ActionResult> Cheque()
		{
			payment = "Cheque";

			ViewBag.Payment = payment;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "POS");
		}

		public async Task<IActionResult> Clear()
        {
            foreach (var cartItem in _CartItems)
            {
                string upc = cartItem.InventoryUPC;
                Inventory product = await _context.Inventories.FirstOrDefaultAsync(p => p.UPC == upc);

                product.Stock += cartItem.OrderQuantity;
                await _context.SaveChangesAsync();
            }

            _CartItems.Clear();
            subtotal = 0;
            tax = 0;
            total = 0;
            return RedirectToAction("Index");
        }

        // GET: POS/Details/5
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
        // GET: POS/Create
        public IActionResult Create()
        {
            //Add Employee Automatically by assigning it to the currently logged in user
            //For now, im trying to use a drop down list on the page
            ViewData["Employee"] = new SelectList(_context.Employees, "ID", "FullName");
            //Do the same with customers
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName");
            return View();
        }

        // POST: POS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UPC,Name,Size,Quantity,AdjustPrice,MarkupPrice,Current,Stock")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: POS/Edit/5
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

        // POST: POS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UPC,Name,Size,Quantity,AdjustPrice,MarkupPrice,Current,Stock")] Inventory inventory)
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

        // GET: POS/Delete/5
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

        // POST: POS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'EmmaSmallEngineContext.Inventories'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
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

        private bool InventoryExists(string id)
        {
          return _context.Inventories.Any(e => e.UPC == id);
        }

    }
}
