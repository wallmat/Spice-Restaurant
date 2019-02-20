using System;
using System.Collections.Generic;
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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET all categories from the db
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        //doesnt need to be async cause we are not passing any data, just passing an empty view
        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        //POST - EDIT
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //GET - Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteConirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);
            if (category == null)
                return NotFound();

            _db.Category.Remove(category);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}