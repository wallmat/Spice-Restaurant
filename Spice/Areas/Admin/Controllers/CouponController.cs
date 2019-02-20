using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (!ModelState.IsValid)
                return View(coupons);

            var files = HttpContext.Request.Form.Files;
            if(files.Count > 0)
            {
                //a file was loaded lets push it to the db
                byte[] p1 = null;
                using (var fStream = files[0].OpenReadStream())
                {
                    //convert the file to a byte arr 
                    using (var mStream = new MemoryStream())
                    {
                        fStream.CopyTo(mStream);
                        p1 = mStream.ToArray();
                    }
                }
                coupons.Picture = p1;
            }

            _db.Coupon.Add(coupons);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupon.FirstOrDefaultAsync(c=>c.Id == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupon.FirstOrDefaultAsync(c => c.Id == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        //POST - EDIT
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            //make sure the id isnt zero, but why?
            if (coupon.Id == 0)
                return NotFound();

            //get the coupon from the db that we are updating
            var couponFromDb = await _db.Coupon.Where(c => c.Id == coupon.Id).FirstOrDefaultAsync();

            //if the state isnt valid just return the same coupon
            //it will show error messages
            if (!ModelState.IsValid)
                return View(coupon);

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                //a file was loaded lets push it to the db
                byte[] p1 = null;
                using (var fStream = files[0].OpenReadStream())
                {
                    //convert the file to a byte arr 
                    using (var mStream = new MemoryStream())
                    {
                        fStream.CopyTo(mStream);
                        p1 = mStream.ToArray();
                    }
                }
                couponFromDb.Picture = p1;
            }

            //set the coup from the db to the updated info passed in
            couponFromDb.MinimumAmount = coupon.MinimumAmount;
            couponFromDb.Name = coupon.Name;
            couponFromDb.Discount = coupon.Discount;
            couponFromDb.CouponType = coupon.CouponType;
            couponFromDb.IsActive = coupon.IsActive;

            //save the changes and redirect back to the index
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupon.FirstOrDefaultAsync(c => c.Id == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            //make sure the id isnt zero, but why?
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupon.FirstOrDefaultAsync(c => c.Id == id);

            if (coupon == null)
                return NotFound();

            _db.Remove(coupon);

            //save the changes and redirect back to the index
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}