using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.ViewModels
{
    public class RegisterVM
    {
        [EmailAddress]
        [Required]
        public string Email {  get; set; }
        [Required]
        [DataType (DataType.Password)]  
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword {  get; set; }        
        public string Name {  get; set; }       
        public string? PhoneNumber { get; set; }   
        public string?  RedirectURL {  get; set; }        
        public string ?Role { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }   

    }
}
