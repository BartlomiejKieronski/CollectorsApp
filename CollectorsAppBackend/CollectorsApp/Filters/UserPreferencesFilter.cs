using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class UserPreferencesFilter : BaseFilter
    {
        public string Layout { get; set; }
        public string Theme { get; set; }
        public int ItemsPerPage { get; set; }
        public bool Pagination { get; set; }
        public int OwnerId { get; set; }
        }
}
