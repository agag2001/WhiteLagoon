using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entites
{
    public class Villa
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public required string Name {  get; set; }      
        public string? Description {  get; set; }

        [Display(Name = "Price Per Night")]
        [Range(10,100000)]
        public double Price { get; set; }   
        public int Sqft {  get; set; }
        [Range(1,10)]
        public int Occupancy {  get; set; }
        [Display(Name="Image Url")]

        [NotMapped]
        public IFormFile? Image { get; set; }    
        public string? ImageUrl { get; set; }  
        public DateTime? CreatedDate {  get; set; } 
        public DateTime? UpdatedDate { get;set; }
         
        [ValidateNever]
        public IEnumerable<Amenity> VillaAmenity { get; set; }

        [NotMapped]
        public bool IsAvilable { get; set; }     = true;    

    }
}
