using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class CollectableItems : BaseModel
    {
        [NotNull]
        public string ItemName { get; set; }
        [AllowNull]
        public DateTime? ItemYear { get; set; }
        [AllowNull]
        public string? ItemNumismat { get; set; }
        [AllowNull]
        public string? ItemValue { get; set; }
        [AllowNull]
        public string? PhotoFilePath { get; set; }
        [AllowNull]
        public DateTime? DateOfAquire { get; set; }
        [NotNull]
        public int CollectionId { get; set; } 
        [AllowNull]
        public string? State { get; set; }
        [AllowNull]
        public string? Description {  get; set; }
        [AllowNull, DefaultValue(false)]
        public bool IsRemoved { get; set; }
    }
}
