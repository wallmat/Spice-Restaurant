using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Category
    {
        [Key] //Id is automatically set as primary key
        public int Id { get; set; }

        [Required]
        [Display(Name="Category")]
        public string Name { get; set; }
    }
}
