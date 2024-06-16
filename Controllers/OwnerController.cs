using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Attributes;
using SmallBusiness.DB_context;
using SmallBusiness.Models;
using System.Drawing.Drawing2D;

namespace SmallBusiness.Controllers
{
    [AuthorizeUserType("Owner")]
    public class OwnerController : Controller
    {
        private readonly StorDbContext _dbcontext;

        public OwnerController(StorDbContext context)
        {
            _dbcontext = context;
        }
        public IActionResult OwnerIndex()
        {
            return View();
        }

        public IActionResult Profile()
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

        public IActionResult MyBrand()
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

            var userBrands = _dbcontext.Brands.Where(b => b.UserID == user.UserID).ToList();
            return View(userBrands);
        }
        public IActionResult EditBrand(int brandId)
        {
            var brand = _dbcontext.Brands.FirstOrDefault(b => b.BrandID == brandId);
            if (brand == null)
            {
                return NotFound(); // or handle not found case accordingly
            }

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> EditBrand(Brand updatedBrand, IFormFile clientFile)
        {
            var brand = _dbcontext.Brands.FirstOrDefault(b => b.BrandID == updatedBrand.BrandID);
            if (brand == null)
            {
                return NotFound(); // or handle not found case accordingly
            }

            // Update brand properties with updatedBrand properties
            brand.BrandName = updatedBrand.BrandName;

            if (clientFile != null && clientFile.Length > 0)
            {


                if (!clientFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ClientFile", "Please upload an image file.");
                    return View(updatedBrand); // Return to the view with validation error
                }
                using (var memoryStream = new MemoryStream())
                {
                    await clientFile.CopyToAsync(memoryStream);
                    brand.BrandImage = memoryStream.ToArray();
                }
            }

            _dbcontext.SaveChanges(); // Save changes to the database

            return RedirectToAction("MyBrand");
        }



        [HttpPost]
        public IActionResult DeleteBrand(int brandId)
        {
            var brand = _dbcontext.Brands.FirstOrDefault(b => b.BrandID == brandId);
            if (brand == null)
            {
                return NotFound(); // or handle not found case accordingly
            }

            _dbcontext.Brands.Remove(brand);
            _dbcontext.SaveChanges(); // Save changes to the database

            return RedirectToAction("MyBrand");
        }

        [HttpGet]
        public IActionResult AddBrand()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // User is not logged in, redirect to login
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                // User is logged in but email not found, handle as needed (e.g., display error message)
                ViewData["ErrorMessage"] = "User email not found. Please try again.";
                return View("Error");
            }


            ViewData["CatageryList"] = _dbcontext.Catagerys.ToList();
            ViewData["UserID"] = user.UserID;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBrand([Bind("BrandID,BrandName,clientFile,CatageryId,UserID")] Brand brand)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "User email not found. Please try again.";
                return View("Error");
            }
            else
            {
                if (brand.clientFile != null && brand.clientFile.ContentType.StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        brand.clientFile.CopyTo(stream);
                        brand.BrandImage = stream.ToArray();
                    }
                }
                else
                {
                    ViewBag.error = "Please upload a valid image file.";
                    ViewData["CatageryList"] = _dbcontext.Catagerys.ToList();
                    return View(brand);
                }

                brand.UserID = user.UserID;
                _dbcontext.Add(brand);
                await _dbcontext.SaveChangesAsync(); // await added
                return RedirectToAction(nameof(MyBrand), "Owner");
            }

            ViewData["CatageryList"] = _dbcontext.Catagerys.ToList();
            return View("AddBrand", brand);
        }


        public IActionResult BrandItem(int brandId)
        {
            HttpContext.Session.SetInt32("BrandID", brandId);
            var items = _dbcontext.Items.Where(i => i.BrandID == brandId).ToList();
            ViewBag.BrandId = brandId; // Pass the brandId to the view
            return View(items);
        }



        [HttpGet]
        public IActionResult AddItem(int brandId)
        {
            HttpContext.Session.SetInt32("BrandID", brandId);
            ViewBag.BrandId = brandId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("ItemID,ItemName,clientFile,ItemPrice,ItemDescription,BrandID,StockAmount")] Item item)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var brandId = HttpContext.Session.GetInt32("BrandID");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("UserLogin", "User");
            }

            var user = _dbcontext.Users.FirstOrDefault(u => u.UserEmail == userEmail);
            if (brandId == null)
            {
                // Handle the case where the brand ID is not found in the session
                ViewBag.ErrorMessage = "Brand ID is missing.";
                return View("Error");
            }
            if (user == null)
            {
                // Handle the case where the user is not found (optional)
                return RedirectToAction("UserLogin", "User");
            }

            else
            {

                if (item.clientFile != null && item.clientFile.ContentType.StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        item.clientFile.CopyTo(stream);
                        item.ItemImage = stream.ToArray();
                    }
                }
                else
                {
                    ViewBag.error = "Please upload a valid image file.";
                    ViewData["CatageryList"] = _dbcontext.Catagerys.ToList();
                    return View(item);
                }

                item.BrandID = brandId.Value;
                _dbcontext.Add(item);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction("BrandItem", new { brandId = item.BrandID });
            }
            return View();
        }
        public IActionResult MyItem(int itemId)
        {
            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }




        [HttpGet]
        public IActionResult EditItem(int itemId)
        {
            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(Item updatedItem, IFormFile clientFile)
        {
            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == updatedItem.ItemID);
            if (item == null)
            {
                return NotFound();
            }

            item.ItemName = updatedItem.ItemName;
            item.ItemPrice = updatedItem.ItemPrice;
            item.ItemDescription = updatedItem.ItemDescription;
            item.StockAmount = updatedItem.StockAmount;  // New property


            if (clientFile != null && clientFile.Length > 0)
            {
                if (!clientFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ClientFile", "Please upload an image file.");
                    return View(updatedItem);
                }
                using (var memoryStream = new MemoryStream())
                {
                    await clientFile.CopyToAsync(memoryStream);
                    item.ItemImage = memoryStream.ToArray();
                }
            }

            _dbcontext.SaveChanges();
            return RedirectToAction("MyItem", new { itemId = item.ItemID });
        }

        [HttpPost]
        public IActionResult DeleteItem(int itemId)
        {
            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
            if (item == null)
            {
                return NotFound();
            }

            _dbcontext.Items.Remove(item);
            _dbcontext.SaveChanges();
            return RedirectToAction("BrandItem", new { brandId = item.BrandID });
        }

        public IActionResult ViewReviews(int itemId)
        {
            var reviews = _dbcontext.Reviews
                                    .Include(r => r.User)
                                    .Where(r => r.ItemId == itemId)
                                    .ToList();

            var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
            if (item == null)
            {
                return NotFound(); // Handle item not found case
            }

            ViewBag.ItemId = itemId;
            ViewBag.BrandId = item.BrandID;

            return View(reviews);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

    }
}
