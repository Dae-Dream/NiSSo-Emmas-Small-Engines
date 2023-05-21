using Emmas_Small_Engines.Data;
using Emmas_Small_Engines.Models;
using Emmas_Small_Engines.Views.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Serialization;
namespace Emmas_Small_Engines.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly EmmaSmallEngineContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        static DateTime currentLoginTime = DateTime.MinValue;

		public HomeController(ILogger<HomeController> logger, EmmaSmallEngineContext context, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            
            if (User.Identity.IsAuthenticated && currentLoginTime == DateTime.MinValue && !User.Identity.Name.Contains("admin") && !User.Identity.Name.Contains("sales")
                && !User.Identity.Name.Contains("ordering") && !User.Identity.Name.Contains("owner") && !User.Identity.Name.Contains("technician") && !User.Identity.Name.Contains("user"))
            {
				Employee emp = await _context.Employees.FirstOrDefaultAsync(e => e.UserName == User.Identity.Name);
                currentLoginTime = DateTime.Now;
                
                if (emp != null)
                {
                    EmpLogin empLogin = new EmpLogin
                    {
                        EmployeeID = emp.ID,
                        SignIn = currentLoginTime
                    };

                    _context.EmpLogins.Add(empLogin);
                    await _context.SaveChangesAsync();

                    if (empLogin != null)
                    {
                        ViewBag.SignInTime = empLogin.SignIn;
                    }
                }
            }
            else
            {
                ViewBag.SignInTime = currentLoginTime;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userID = _signInManager.UserManager.GetUserId(User);

            var empLogin = await _context.EmpLogins
                .Where(l => l.Employee.UserName == User.Identity.Name)
                .OrderByDescending(l => l.SignIn)
                .FirstOrDefaultAsync();

            var empLogins = await _context.EmpLogins.ToListAsync();

            if (empLogin != null)
            {
                empLogin.SignOut = DateTime.Now;
                _context.EmpLogins.Update(empLogin);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            currentLoginTime = DateTime.MinValue;

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
    }
}