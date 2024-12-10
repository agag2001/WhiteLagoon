using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.ViewModels
{
    public class VillaNumberVM
    {
        public int Villa_Number { get; set; }
        public string? SpecialDetails { get; set; }
        public int Villa_id { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ?VillaList {  get; set; } 

    }
}
