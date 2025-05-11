using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CollectorsApp.Models.Interfaces;

namespace CollectorsApp.Models
{
    public class CollectableItems : IOwner
    {
        [Key, NotNull]
        public int Id { get; set; }
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
        [NotNull]
        public int OwnerId { get; set; } 
        [NotNull]
        public DateTime InsertDate { get; set; }
        [AllowNull]
        public DateTime? DateOfAquire { get; set; }
        [NotNull]
        public int CollectionId { get; set; } 
        [AllowNull]
        public string? State { get; set; }
        [AllowNull]
        public string? Description {  get; set; } 
    }
}
