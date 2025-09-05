using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class BaseModel : IOwner
    {
        [NotNull, AutoIncrement,Key]
        public int Id { get; set; }
        [NotNull]
        public int OwnerId { get; set; }
        [AllowNull]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [AllowNull]
        public DateTime? LastUpdated { get; set; }
        [AllowNull]
        public DateTime? Deleted { get; set; }
    }
}
