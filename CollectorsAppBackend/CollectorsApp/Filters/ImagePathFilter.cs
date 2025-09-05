using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class ImagePathFilter :BaseFilters
    {
        public string Path { get; set; }
        public int ItemId { get; set; }
        public bool IsRemoved { get; set; }
    }
}
