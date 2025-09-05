using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class CollectableItemsFilter : BaseFilters
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public DateTime? ItemYear { get; set; }
        public string? ItemNumismat { get; set; }
        public string? ItemValue { get; set; }
        public string? PhotoFilePath { get; set; }
        public int OwnerId { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DateOfAquire { get; set; }
        public int CollectionId { get; set; }
        public string? State { get; set; }
        public string? Description { get; set; }
        public bool IsRemoved { get; set; }
    }
}
