using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using System.Data;
using Microsoft.CodeAnalysis;
using NuGet.Versioning;

namespace Emmas_Small_Engines.Controllers
{
    [Authorize(Roles = "Owner, Admin, Ordering, Technician")]
    public class AnalyticsController : Controller
    {
        private readonly EmmaSmallEngineContext _context;

        public AnalyticsController(EmmaSmallEngineContext context)
        {
            _context = context;
        }

        // GET: AnalyticsController
        public  IActionResult Index(string actionButton)
        {
            /* await AddReportAnalytics(actionButton);
             var report =  _context.SalesReports
                 .Include(s => s.SalesReportEmps)
                 .Include(s => s.SalesReportItems)
                 .Where(s => s.ID == -1)
                 .ToArray();



             ViewBag.Report = report;
             ViewBag.MonthPrior = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day);*/
            var topThree = _context.SalesReportEmps
                .OrderBy(se => se.EmpSales)
                .Distinct()
                .ToArray();


           
            ViewBag.TopThree = topThree;

            return View();
        }

        // GET: AnalyticsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AnalyticsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnalyticsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AnalyticsController/Edit/5
        /*public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AnalyticsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AnalyticsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AnalyticsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
        //----------------------FUNCTIONS--------------------------
        // AddReport By Angel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReportAnalytics( string actionButton)                        
        {
            ViewDataReturnURL();
            var salesReport = new SalesReport()
            {
                ID= -1,
                srepName = "Monthly Report",
                srepCriteria = "All Employees",
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.Today.Day),
                EndDate = DateTime.Today
            };
           

            // Get all the invoices filtered by date and Employee
            var invoices = await _context.Invoices
                .Include(i => i.Employee)
                .Include(i => i.InvoicePayments).ThenInclude(i => i.Payment)
                .Include(i => i.InvoiceLines).ThenInclude(i => i.Inventory)
                .Where(i => i.Date >= salesReport.StartDate && i.Date <= salesReport.EndDate)
                .Where(i => salesReport.srepCriteria != "" ? (i.Employee.FirstName + " " + i.Employee.LastName) == salesReport.srepCriteria : true)
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

                    /*if (actionButton == "Save")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("SalesReport", new { salesReport.ID }); // "Prints" the Receipt*/
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
            return View("Index");//RedirectToAction("Index");
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }
        private void PopulateDropDownLists(SalesReport salesReport = null)
        {
            ViewData["srepCriteria"] = CriteriaList(salesReport?.srepCriteria);
        }
        private SelectList CriteriaList(string selectedId)
        {
            return new SelectList(_context.Employees
                .OrderBy(e => e.FirstName).ThenBy(e => e.LastName), "FullName", "FullName", selectedId);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
    }
}
