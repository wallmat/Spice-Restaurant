using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            //create a new index view model for the landing page
            IndexViewModel IndexVM = new IndexViewModel()
            {
                //get the menu items with the categories and sub categories to display
                //then get the list of categories
                //and aget the list of coupons but only active ones
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(s => s.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsidentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var count = _db.shoppingCart.Where(s => s.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }

            return View(IndexVM);
        }

        //GET - DETAILS 
        //we want only this method to reqire auth not all of them
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            //get the menu item from the db based on the id, and include the category and sub category
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cart = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(cart);
        }

        //POST - DETAILS
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;

            //if not a valid state just return the item from the db
            if (!ModelState.IsValid)
            {
                //get the menu item from the db based on the id, and include the category and sub category
                var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cartObj.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cart = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cart);
            }
            
            //get the users identity, then get the claim/user
            var claimsidentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            cartObj.ApplicationUserId = claim.Value;

            //get the cart from that db that matches the user id and menu id to see if they already added this item
            ShoppingCart cartFromDb = await _db.shoppingCart.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId 
                                                                    && c.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();

            //if they havent added it before
            if (cartFromDb == null)
            {
                await _db.shoppingCart.AddAsync(cartObj);
            }
            else
            {
                //if it does just add the new ones they want to add
                cartFromDb.Count = cartFromDb.Count + cartObj.Count;
            }

            await _db.SaveChangesAsync();

            //get the current tolal items in the cart and update the visual shopping cart using sessions
            var count = _db.shoppingCart.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

            return RedirectToAction(nameof(Index));
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
    }
}
