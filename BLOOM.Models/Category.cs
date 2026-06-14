using System.ComponentModel.DataAnnotations;

namespace BLOOM.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name="Category Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Display Order")]
        [Range(0,100 ,ErrorMessage = "Display order must be between 0 and 100.")]
        public int? DisplayOrder { get; set; }
        public string? ImageUrl { get; set; }
    }
}