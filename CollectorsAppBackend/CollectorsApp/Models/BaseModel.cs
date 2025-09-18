using CollectorsApp.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class BaseModel : IOwner, ILastUpdated
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
