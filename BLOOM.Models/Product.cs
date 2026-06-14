using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BLOOM.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string SKU { get; set; } = string.Empty;
        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [Display (Name ="List Price")]
        [Range(1, 10000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price (1–2 bottles)")]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for (3–9)")]
        [Range(1, 10000)]
        public double Price50 { get; set; }


        [Required]
        [Display(Name = "Price for 10+")]
        [Range(1, 10000)]
        public double Price100 { get; set; }


        //Foreign Key  Category
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }



        [ValidateNever]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }


    }
}
