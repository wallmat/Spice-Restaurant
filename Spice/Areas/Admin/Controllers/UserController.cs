using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            //we want to return a list of all the users except the user logged in cause they are super admin

            //this gets the user identity
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            //the user logged in.
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //return everyone except the logged in user
            return View(await _db.ApplicationUser.Where(u=>u.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
                return NotFound();

            //same as await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            var user = await _db.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();

            //to lock out a user in asp just set the lockout end prop from null to a date so now + 1000 years 
            user.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
                return NotFound();

            //same as await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            var user = await _db.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();

            //to lock out a user in asp just set the lockout end prop from null to a date so now + 1000 years 
            user.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}