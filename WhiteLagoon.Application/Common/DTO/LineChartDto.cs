namespace WhiteLagoon.Application.Common.DTO
{

    public class LineChartDto
    {
        public List<LineItem> Series { get; set; }
        public string[] Categories { get; set; }

    }
    public class LineItem
    {
        public string Name { get; set; }
        public int[] Data { get; set; }
    }
}

