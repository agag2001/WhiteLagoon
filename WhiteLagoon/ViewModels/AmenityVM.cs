using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.ViewModels
{
    public class AmenityVM
    {
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; } 
        public int VillaId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }
    }
}
