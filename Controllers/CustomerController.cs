using Microsoft.AspNetCore.Mvc;
using SmallBusiness.DB_context;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using SmallBusiness.Models;
using SmallBusiness.Attributes;

namespace SmallBusiness.Controllers
{
    [AuthorizeUserType("Customer")]
    public class Customer : Controller
    {


        private readonly StorDbContext _dbcontext;

        public Customer(StorDbContext context)
        {
            _dbcontext = context;
        }
        public IActionResult CustomerIndex()
        {

            var userEmail = HttpContext.Session.GetString("UserEmail");


            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }

            return View();
        }

        public IActionResult CustomerAbout()
        {

            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }

            return View();
        }
        public IActionResult CustomerCategory()
        {

            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }

            var items = _dbcontext.Catagerys.ToList(); // Replace "Items" with the name of your DbSet property for items
            return View(items);
        }
        public IActionResult CustomerService()
        {

            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }

            return View();
        }

        public IActionResult ProfileCustomer()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }


            return View(user);

        }

        public IActionResult MyOrder()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                return RedirectToAction("UserLogin", "User");
            }
            var userOrders = _dbcontext.Orders
                   .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Item)
                   .Where(o => o.UserID == user.UserID)
                   .ToList();

            return View(userOrders);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
