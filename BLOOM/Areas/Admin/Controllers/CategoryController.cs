using BLOOM.Business.Services.IServices;
using BLOOM.DataAccess.Data;
using BLOOM.Models;
using BLOOM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BLOOM.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]

    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult>Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View("Index",categories);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        //Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(Category category)
        {

            if (!String.IsNullOrEmpty(category.Name) &&
                !await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("Name", "Category name already exists.");
                return View(category);
            }

            if (ModelState.IsValid)
            {
               
                await _categoryService.CreateCategoryAsync(category);

                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }return View();

        }


        //Update
        public async Task<IActionResult> Update(int? id)
        {
            if(id ==null || id == 0)
            {
                return NotFound();
            }

            var category=await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task <IActionResult> UpdatePost(Category category)
        {

            if (!String.IsNullOrEmpty(category.Name) &&
               !await _categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError("Name", "Category name already exists.");
                return View(category);
            }

            if (ModelState.IsValid)
            {

               await _categoryService.UpdateCategoryAsync(category);

                TempData["success"] = "category updated successfully.";
                
                return RedirectToAction("Index");
            }
            return View();

        }


        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {

            await _categoryService.DeleteCategoryAsync(id);
            TempData["success"] = "category deleted successfully.";
           
            return RedirectToAction("Index");
            }
         

        
    }
}
