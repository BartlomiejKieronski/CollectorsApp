using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class UserPreferencesFiler : BaseFilters
    {
        public string Layout { get; set; }
        public string Theme { get; set; }
        [AllowNull, DefaultValue(10)]
        public int ItemsPerPage { get; set; }
        [AllowNull, DefaultValue(true)]
        public bool Pagination { get; set; }
    }
}
