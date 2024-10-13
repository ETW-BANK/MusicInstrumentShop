using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MusicShop.Models
{
   public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]

        public decimal Price { get; set; }


        public int CategoryId { get; set; }



        [ForeignKey("CategoryId")]

        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}
