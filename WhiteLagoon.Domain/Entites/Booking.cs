using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entites
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name{ get; set; }
        [EmailAddress]
        public string Email { get; set; } 
        public string? Phone {  get; set; } 

        public DateTime BookingDate { get; set; }
         public DateOnly CheckInDate { get; set; }
        public  DateOnly CheckOutDate { get; set; }

        public double TotalCost { get; set; }
        public int Nights { get; set; } 
        public string? Status { get; set; } // track user status(approved ,, checkIn , checkout)

        //Payment
        public DateTime PaymentDate { get; set; }   
        public bool IsPaymentSuccessful { get; set; }  
        
        public string? StripSessionId {  get; set; } 
        public string? StripPaymentIntentId {  get; set; }  
         public DateTime ActualCheckInDate { get; set; }
        public DateTime ActualCheckOutDate { get;   set; }  

        public int VillaNumber { get; set; }
        [NotMapped]
        public List<VillaNumber> VillaNumbers { get; set; }    

        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [Required]
        public string UserId { get; set; }


        [ForeignKey("VillaId")] 
        public Villa Villa { get; set; }
        [Required]
        public int VillaId {  get; set; }   
        

    }
}
