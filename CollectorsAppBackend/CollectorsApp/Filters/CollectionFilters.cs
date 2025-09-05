using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class CollectionFilters : BaseFilters
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public int Depth { get; set; }
        public bool IsRemoved { get; set; }
    }
}
