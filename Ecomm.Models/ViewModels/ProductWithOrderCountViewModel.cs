using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecomm.Models;

namespace Ecomm.Models.ViewModels
{
    public class ProductWithOrderCountViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Title { get; set; }
        public int OrderCount { get; set; }
        public int Id { get; set; }
        public int Count { get; set; }
        [Required]
        public string Author { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ListPrice is required.")]
        [Range(1, 1000, ErrorMessage = "ListPrice must be between 1 and 1000.")]
        public double ListPrice { get; set; }//600

        [Required(ErrorMessage = "Price is required.")]
        public double Price { get; set; }//590

        [Required(ErrorMessage = "Price50 is required.")]
        [Range(1, 1000, ErrorMessage = "Price50 must be between 1 and 1000.")]
        public double Price50 { get; set; }//500

        [Required(ErrorMessage = "Price100 is required.")]
        [Range(1, 1000, ErrorMessage = "Price100 must be between 1 and 1000.")]
        public double Price100 { get; set; }//450

        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
       // public Category Category { get; set; }

        [Display(Name = "CoverType")]
        public int CoverTypeId { get; set; }
        public CoverType CoverType { get; set; }
        public string Rating { get; set; }

    }
}
