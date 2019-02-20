using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET - INDEX
        public async Task<IActionResult> Index()
        {
            //this will allow the sub category to get the sub categories from the db 
            //and also load the foreign key of the category
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();

            return View(subCategories);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),

                //order the sub cat in the db by name, select all of them by name, then get only the names
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                //read all sub categories from db
                //include the category data with include
                //but only where the name is correct for the current selected model
                //and its in the same cat id
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name
                    && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Bad, we dont want duplicate sub categories
                    StatusMessage = "Error : Sub Category already exist under " + doesSubCategoryExists.First().Category.Name + " please choose another name";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            //the model passeed in is not in a valid state so we have to make 
            //a new one to reset to and tell the user the issue
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            //get all the sub categories from the db, where the category 
            //id matches the one requested then select them all and put 
            //them in a list
            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();

            //return that list as a js object to consume
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,

                //order the sub cat in the db by name, select all of them by name, then get only the names
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                //read all sub categories from db
                //include the category data with include
                //but only where the name is correct for the current selected model
                //and its in the same cat id
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name
                    && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Bad, we dont want duplicate sub categories
                    StatusMessage = "Error : Sub Category already exist under " + doesSubCategoryExists.First().Category.Name + " please choose another name";
                }
                else
                {
                    var subCatFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name; //this updates only one prop in the db instead of all of them

                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            //the model passeed in is not in a valid state so we have to make 
            //a new one to reset to and tell the user the issue
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(modelVM);
        }

        //GET - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        //GET - Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteConirmed(int id)
        {
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(s=>s.Id == id);

            if (subCategory == null)
                return View();

            _db.SubCategory.Remove(subCategory);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}