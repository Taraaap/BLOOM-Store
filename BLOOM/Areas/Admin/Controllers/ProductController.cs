using BLOOM.Business.Services.IServices;
using BLOOM.DataAccess.Data;
using BLOOM.Models;
using BLOOM.Models.ViewModels;
using BLOOM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BLOOM.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]

    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult>Index()
        {
            return View();
        }


        [Authorize(Roles =SD.RoleAdmin)]
      
        public async Task<IActionResult> Upsert(int? id)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ProductVM productVM = new()
            {
                CategoryList = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                productVM.Product=await _productService.GetProductByIdAsync(id.Value);
                return View(productVM);
            }
            
        }

        //Create
        [HttpPost]

        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPost(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products");
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    using (var filestream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        //file.CopyTo(filestream);
                        await file.CopyToAsync(filestream);
                    }

                    productVM.Product.ImageUrl = "/" + Path.Combine(productPath, fileName).Replace("\\", "/");
                }
              

                if (productVM.Product.Id == null || productVM.Product.Id == 0)
                {
                    //Create
                    await _productService.CreateProductAsync(productVM.Product);
                }
                else
                {
                    await _productService.UpdateProductAsync(productVM.Product);
                }
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                productVM = new()
                {
                    CategoryList = categories.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
                };
             return View(productVM);
            }

        }




        #region API CALLS
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync( true);
            return Json(new { data = products });
        }


        [HttpDelete]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }
            var productToBeDeleted = await _productService.GetProductByIdAsync(id.Value);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            //Delete product image if exist
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\','/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _productService.DeleteProductAsync(id.Value);

            return Json(new { success = true, message = "Delete Successfull" });

        }
        #endregion


    }
}
