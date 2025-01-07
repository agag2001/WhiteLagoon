
namespace WhiteLagoon.Application.Common.DTO
{    public class RedialChartDto
    {
        public double TotalCount {  get; set; }    
        public double CountInCurrentMonth { get; set; }
        public bool IsRatioIncrease {  get; set; }  
        public int[] Series {  get; set; }  
    }


}
