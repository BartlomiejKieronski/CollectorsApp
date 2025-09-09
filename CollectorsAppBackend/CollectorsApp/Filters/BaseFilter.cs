﻿namespace CollectorsApp.Filters
{
    public class BaseFilter
    {
        public int? Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? After { get; set; }
        public DateTime? Before { get; set; }
        public int? NumberOfRecords { get; set; }
        public int? Page { get; set; }
        public string? OrderBy { get; set; }
        public bool? SortDescending { get; set; }
        public string? SearchText { get; set; }
        public string? Keywords { get; set; }
    }
}
