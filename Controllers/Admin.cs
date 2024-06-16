using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Attributes;
using SmallBusiness.data;
using SmallBusiness.DB_context;
using SmallBusiness.Models;
using System.Drawing.Drawing2D;

namespace Small_Business_project.Controllers
{
    [AuthorizeUserType("Admin")]
    public class AdminController : Controller
    {
        private readonly StorDbContext _context;

        public AdminController(StorDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminIndex()
        {
            return View();
        }
        public async Task<IActionResult> IndexCatagery()
        {
            return _context.Catagerys != null ?
                        View(await _context.Catagerys.ToListAsync()) :
                        Problem("Entity set 'StorDbContext.Catagerys'  is null.");
        }

        public IActionResult CreateCatagery()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCatagery([Bind("CatageryId,CatageryName,clientFile")] Catagery catagery)
        {
            
                if (catagery.clientFile != null && catagery.clientFile.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        await catagery.clientFile.CopyToAsync(stream);
                        catagery.CatageryImage = stream.ToArray();
                    }
                }
                else
                {
                    ViewBag.error = "Please upload a valid image file.";
                    return View(catagery);
                }

                _context.Add(catagery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexCatagery));
            
            return View(catagery);
        }



        public async Task<IActionResult> EditCatagery(int? id)
        {
            if (id == null || _context.Catagerys == null)
            {
                return NotFound();
            }

            var catagery = await _context.Catagerys.FindAsync(id);
            if (catagery == null)
            {
                return NotFound();
            }
            return View(catagery);
        }

        // POST: Catageries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCatagery(int id, [Bind("CatageryId,CatageryName,clientFile")] Catagery catagery)
        {
            if (id != catagery.CatageryId)
            {
                return NotFound();
            }

            try
            {
                if (catagery.clientFile != null && catagery.clientFile.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        await catagery.clientFile.CopyToAsync(stream);
                        catagery.CatageryImage = stream.ToArray();
                    }
                }

                _context.Update(catagery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexCatagery));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatageryExists(catagery.CatageryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(catagery);
        }
        // GET: Catageries/Delete/5
        public async Task<IActionResult> DeleteCatagery(int? id)
        {
            if (id == null || _context.Catagerys == null)
            {
                return NotFound();
            }

            var catagery = await _context.Catagerys
                .FirstOrDefaultAsync(m => m.CatageryId == id);
            if (catagery == null)
            {
                return NotFound();
            }

            return View(catagery);
        }

        // POST: Catageries/Delete/5
        [HttpPost, ActionName("DeleteCatagery")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCatagery(int id)
        {
            if (_context.Catagerys == null)
            {
                return Problem("Entity set 'StorDbContext.Catagerys'  is null.");
            }
            var catagery = await _context.Catagerys.FindAsync(id);
            if (catagery != null)
            {
                _context.Catagerys.Remove(catagery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexCatagery));
        }

        private bool CatageryExists(int id)
        {
            return _context.Catagerys.Any(e => e.CatageryId == id);
        }

        public async Task<IActionResult> DetailsCatagery(int? id)
        {
            if (id == null || _context.Catagerys == null)
            {
                return NotFound();
            }

            var catagery = await _context.Catagerys
                .FirstOrDefaultAsync(m => m.CatageryId == id);
            if (catagery == null)
            {
                return NotFound();
            }

            return View(catagery);
        }

        // GET: Users
        public async Task<IActionResult> IndexUsers()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Entity set 'StorDbContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> DetailsUsers(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult CreateUsers()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsers([Bind("UserID,FirstName,LastName,UserEmail,UserPassword,ConfirmPassword,UserPhone,UserGender,clientFile,type")] User user)
        {
            string input = user.UserPassword;
            string input2 = user.ConfirmPassword;
            if (!string.IsNullOrEmpty(input))
            {
                user.UserPassword = Hash.Hashpassword(input);
                user.ConfirmPassword = Hash.Hashpassword(input2);
            }

            if (user.clientFile != null && user.clientFile.ContentType.ToLower().StartsWith("image/"))
            {
                using (var stream = new MemoryStream())
                {
                    await user.clientFile.CopyToAsync(stream);
                    user.UserImage = stream.ToArray();
                }
            }
            else
            {
                ViewBag.error = "Please upload a valid image file.";
                return View(user);
            }

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexUsers));
        
    return View(user);
    }

        
        public async Task<IActionResult> EditUsers(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsers(int id, [Bind("UserID,FirstName,LastName,UserEmail,UserPassword,ConfirmPassword,UserPhone,UserGender,UserImage,clientFile,type")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            try
            {
                if (user.clientFile != null && user.clientFile.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        await user.clientFile.CopyToAsync(stream);
                        user.UserImage = stream.ToArray();
                    }
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUsers));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UserExists(user.UserID))
                {
                    throw;
                }
                else
                {
                    return NotFound();
                }
            }

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> DeleteUsers(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("DeleteUsers")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUsers(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'StorDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexUsers));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }




        //Brand//
        public async Task<IActionResult> IndexBrand()
        {
            var storDbContext = _context.Brands.Include(b => b.Catagery).Include(b => b.User);
            return View(await storDbContext.ToListAsync());
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> DetailsBrand(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .Include(b => b.Catagery)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BrandID == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult CreateBrand()
        {
            ViewData["CatageryId"] = new SelectList(_context.Catagerys, "CatageryId", "CatageryName");
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "UserEmail");
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand([Bind("BrandID,BrandName,clientFile,CatageryId,UserID")] Brand brand)
        {
            // if (ModelState.IsValid)
            //{
            if (brand.clientFile != null && brand.clientFile.ContentType.ToLower().StartsWith("image/"))
            {
                using (var stream = new MemoryStream())
                {
                    await brand.clientFile.CopyToAsync(stream);
                    brand.BrandImage = stream.ToArray();
                }
            }
            else
            {
                ViewBag.error = "Please upload a valid image file.";
                ViewData["CatageryId"] = new SelectList(_context.Catagerys, "CatageryId", "CatageryName", brand.CatageryId);
                ViewData["UserID"] = new SelectList(_context.Users, "UserID", "UserEmail", brand.UserID);
                return View(brand);
            }

            _context.Add(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexBrand));
            // }
            ViewData["CatageryId"] = new SelectList(_context.Catagerys, "CatageryId", "CatageryName", brand.CatageryId);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "UserEmail", brand.UserID);
            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> EditBrand(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            ViewData["CatageryId"] = new SelectList(_context.Catagerys, "CatageryId", "CatageryName", brand.CatageryId);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "UserEmail", brand.UserID);
            return View(brand);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(int id, [Bind("BrandID,BrandName,clientFile,CatageryId,UserID")] Brand brand)
        {
            if (id != brand.BrandID)
            {
                return NotFound();
            }

            try
            {
                if (brand.clientFile != null && brand.clientFile.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var stream = new MemoryStream())
                    {
                        await brand.clientFile.CopyToAsync(stream);
                        brand.BrandImage = stream.ToArray();
                    }
                }

                _context.Update(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexBrand));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.BrandID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CatageryId"] = new SelectList(_context.Catagerys, "CatageryId", "CatageryName", brand.CatageryId);
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "UserEmail", brand.UserID);
            return View(brand);
        }
        // GET: Brands/Delete/5
        public async Task<IActionResult> DeleteBrand(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .Include(b => b.Catagery)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BrandID == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("DeleteBrand")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedBrand(int id)
        {
            if (_context.Brands == null)
            {
                return Problem("Entity set 'StorDbContext.Brands'  is null.");
            }
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexBrand));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.BrandID == id);
        }
    }
}
