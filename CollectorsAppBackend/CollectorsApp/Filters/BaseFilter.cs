namespace CollectorsApp.Filters
{
    public class BaseFilters
    {
        public int? Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? After { get; set; }
        public DateTime? Before { get; set; }
        public int? NumberOfRecords { get; set; }
        public int? Page { get; set; }
        public string? OrderBy { get; set; }
        public bool? SortDescending { get; set; }
    }
}
