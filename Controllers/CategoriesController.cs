using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.DB_context;
using SmallBusiness.Models;

namespace SmallBusiness.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly StorDbContext _dbcontext;

        public CategoriesController(StorDbContext context)
        {
            _dbcontext = context;
        }



        [HttpGet]
        public IActionResult Brand(int CatageryId)
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

            HttpContext.Session.SetInt32("CatageryId", CatageryId);
            var catId = HttpContext.Session.GetInt32("CatageryId");
            var brands = _dbcontext.Brands.Where(i => i.CatageryId == CatageryId).ToList();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Item(int BrandID)
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

            HttpContext.Session.SetInt32("BrandID", BrandID);
            var brandId = HttpContext.Session.GetInt32("BrandID");
            var Items = _dbcontext.Items.Where(i => i.BrandID == BrandID).ToList();
            return View(Items);
        }

        public IActionResult ItemDetails(int itemId)
        {
            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> ItemReviews(int itemId)
        {
            var item = await _dbcontext.Items.FindAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            var reviews = await _dbcontext.Reviews.Include(r => r.User)
                                                  .Where(r => r.ItemId == itemId)
                                                  .ToListAsync();

            var model = new Tuple<Item, IEnumerable<Review>>(item, reviews);
            return View(model);
        }

        [HttpGet]
        public IActionResult AddReview(int itemId)
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

            var review = new Review
            {
                ItemId = itemId,
                UserId = user.UserID
            };

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Review review, string TriedItem)
        {
            if (TriedItem == "no")
            {
                ModelState.AddModelError("", "You cannot add a review without trying the item.");
                return View(review);
            }


            _dbcontext.Reviews.Add(review);
            await _dbcontext.SaveChangesAsync();
            return RedirectToAction("ItemReviews", new { itemId = review.ItemId });


            return View(review);
        }



    }
}
