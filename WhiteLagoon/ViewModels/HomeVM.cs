using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.ViewModels
{
	public class HomeVM
	{
		public IEnumerable<Villa>? VillaList { get; set; }	
		public DateOnly CheckInDate { get; set; }
		public DateOnly CheckOutDate {  get; set; }	
		public int Nights {  get; set; }	
	}
}
