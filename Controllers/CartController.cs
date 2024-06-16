
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.DB_context;
using SmallBusiness.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;
using System.Text.Json;
using SmallBusiness.Extensions;
using Org.BouncyCastle.Asn1.X509;

public class CartController : Controller
{
    private readonly StorDbContext _dbcontext;
    private const string CartSessionKey = "CartItems";


    public CartController(StorDbContext context)
    {
        _dbcontext = context;
    }




    public IActionResult CartIndex()
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
        var cartItems = GetCartItems();
        return View(cartItems);
    }
    public IActionResult IncreaseCartItemQuantity(int itemId)
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

        var cartItems = GetCartItems();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ItemIdCart == itemId);

        if (cartItem != null)
        {
            var item = GetItemById(itemId);
            if (item != null)
            {
                if (item.StockAmount > cartItem.QuantityCart) // Check if adding one more item will exceed the stock
                {
                    cartItem.QuantityCart += 1;
                    SaveCartItems(cartItems);
                }
                else
                {
                    TempData["OutOfStockMessage"] = "This item is currently out of stock.";
                }
            }
        }

        return RedirectToAction("CartIndex");
    }



    public IActionResult AddToCart(int itemId, int quantity = 1)
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

        var item = GetItemById(itemId);
        if (item != null)
        {
            if (item.StockAmount >= quantity) // Check if the item is in stock
            {
                var cartItems = GetCartItems();
                var cartItem = cartItems.FirstOrDefault(ci => ci.ItemIdCart == itemId);

                if (cartItem == null)
                {
                    cartItems.Add(new CartItem
                    {
                        ItemIdCart = item.ItemID,
                        ItemNameCart = item.ItemName,
                        QuantityCart = quantity,
                        ItemPriceCart = item.ItemPrice

                    });
                }
                else
                {
                    cartItem.QuantityCart += quantity;
                }

                SaveCartItems(cartItems);
                // No need to save cart items in the database here.
            }
            else
            {
                TempData["OutOfStockMessage"] = "This item is currently out of stock.";
                return RedirectToAction("ItemDetails", "Categories", new { itemId = itemId });
            }
        }
        return RedirectToAction("CartIndex"); // Redirect to item details
    }



    public IActionResult RemoveFromCart(int itemId)
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

        var cartItems = GetCartItems();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ItemIdCart == itemId);

        if (cartItem != null)
        {
            cartItems.Remove(cartItem);
            SaveCartItems(cartItems);
        }

        return RedirectToAction("CartIndex");
    }


    public IActionResult Checkout()
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

        var cartItems = GetCartItems();
        Order order = null;

        if (cartItems.Any())
        {
            order = new Order
            {
                OrderData = DateTime.Now,
                OrderPrice = cartItems.Sum(ci => ci.QuantityCart * ci.ItemPriceCart),
                OrderQuantity = cartItems.Sum(ci => ci.QuantityCart),
                UserID = user.UserID, // Set the UserID to the UserID of the logged-in user
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ItemID = ci.ItemIdCart,
                    Quantity = ci.QuantityCart,
                    Price = ci.ItemPriceCart
                }).ToList()
            };

            foreach (var cartItem in cartItems)
            {
                var item = _dbcontext.Items.FirstOrDefault(i => i.ItemID == cartItem.ItemIdCart);
                if (item != null)
                {
                    item.StockAmount -= cartItem.QuantityCart;
                }
            }

            _dbcontext.Orders.Add(order);
            _dbcontext.SaveChanges();

            // Clear the cart after checkout
            HttpContext.Session.Remove(CartSessionKey);
        }

        if (order != null)
        {
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }

        return RedirectToAction("CartIndex");
    }




    public IActionResult OrderConfirmation(int orderId)
    {
        var order = _dbcontext.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).FirstOrDefault(o => o.OrderID == orderId);
        return View(order);
    }

    [HttpGet]
    public IActionResult Payment(int orderId)
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

        var order = _dbcontext.Orders.FirstOrDefault(o => o.OrderID == orderId);
        if (order == null)
        {
            return NotFound();
        }

        var payment = new Payment
        {
            OrderID = orderId,
            PaymentPrice = order.OrderPrice,
            PaymentDate = DateTime.Now
        };

        return View(payment);
    }
    [HttpPost]
    public IActionResult Payment(Payment payment)
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

        // Manually validate the payment
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(payment.PaymentType))
        {
            validationErrors.Add("Payment Type is required.");
        }

        if (payment.PaymentType == "Visa" && string.IsNullOrWhiteSpace(payment.VisaNo))
        {
            validationErrors.Add("Visa number is required for Visa payment.");
        }

        if (validationErrors.Count > 0)
        {
            ViewBag.ValidationErrors = validationErrors;
            return View(payment);
        }

        if (!string.IsNullOrWhiteSpace(payment.PaymentType) && payment.PaymentType == "Cash")
        {
            payment.VisaNo = null; // Clear the Visa number if the payment type is cash
        }

        payment.PaymentDate = DateTime.Now; // Ensure the payment date is set correctly
        _dbcontext.Payments.Add(payment);
        _dbcontext.SaveChanges();

        return RedirectToAction("PaymentConfirmation", new { paymentId = payment.PaymentID });
    }


    public IActionResult PaymentConfirmation(int paymentId)
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

        var payment = _dbcontext.Payments.Include(p => p.Orders).FirstOrDefault(p => p.PaymentID == paymentId);
        if (payment == null)
        {
            return NotFound();
        }
        return View(payment);
    }




    private List<CartItem> GetCartItems()
    {
        var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
        return cartItems ?? new List<CartItem>();
    }

    private void SaveCartItems(List<CartItem> cartItems)
    {
        HttpContext.Session.SetObjectAsJson(CartSessionKey, cartItems);
    }

    private Item GetItemById(int itemId)
    {
        // Fetch item from the database using EF Core
        return _dbcontext.Items.FirstOrDefault(i => i.ItemID == itemId);
    }
}