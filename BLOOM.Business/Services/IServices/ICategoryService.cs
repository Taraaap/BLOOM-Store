using BLOOM.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.Business.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryID = null);
    }
}
