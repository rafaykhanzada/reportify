namespace Core.Data.DTOs
{
    public class FilterDto
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? SearchBy { get; set; }
    }
    public class SearchParams
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
    public class DateFilterVM
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
