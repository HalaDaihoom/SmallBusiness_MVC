
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallBusiness.data;
using SmallBusiness.DB_context;
using SmallBusiness.Models;



namespace SmallBusiness.Controllers
{
    public class UserController : Controller
    {
        private readonly StorDbContext _dbContext;

        public UserController(StorDbContext context)
        {
            _dbContext = context;
        }
        private bool isAuthenticate;



        [HttpGet]
        public IActionResult UserSignUp()
        {
            if (HttpContext.Session.GetString("Email") is not null)
            {
                return Redirect("/User/UserSignUp");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserSignUp(User user)
        {
            // if (ModelState.IsValid)
            //{


            var check = _dbContext.Users.FirstOrDefault(s => s.UserEmail == user.UserEmail);
            if (check == null)
            {

                string input = user.UserPassword;
                string input2 = user.ConfirmPassword;
                if (!string.IsNullOrEmpty(input))
                {
                    user.UserPassword = Hash.Hashpassword(input);
                    user.ConfirmPassword = Hash.Hashpassword(input2);

                }
                if (user.clientFile != null && user.clientFile.ContentType.StartsWith("image/"))
                {
                    MemoryStream stream = new MemoryStream();
                    user.clientFile.CopyTo(stream);
                    user.UserImage = stream.ToArray();
                }
                else
                {
                    ViewBag.error = "Please upload a valid image file.";
                    return View(user);
                }
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return RedirectToAction("UserLogin");
            }
            else
            {
                ViewBag.error = "Email already exists";
                return View(user);
            }


        }



        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(User user)
        {

            var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserEmail == user.UserEmail && u.UserPassword == Hash.Hashpassword(user.UserPassword));

            if (existingUser != null)
            {
                HttpContext.Session.SetString("UserEmail", existingUser.UserEmail);
                HttpContext.Session.SetString("type", existingUser.type);

                if (existingUser.type == "Customer")
                {
                    
                    return RedirectToAction("CustomerCategory", "Customer");
                }
                else if (existingUser.type == "Owner")
                {
                   

                    return RedirectToAction("Profile", "Owner");
                }else if(existingUser.type == "Admin")
                {
                    return RedirectToAction("AdminIndex", "Admin");
                }



            }
            else
            {

                ViewBag.error = "Invalid Email or Password";
                return View();
            }

            return View(user);
        }

    }
}

