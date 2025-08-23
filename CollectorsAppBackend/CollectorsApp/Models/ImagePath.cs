using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CollectorsApp.Models.Interfaces;

namespace CollectorsApp.Models
{
    public class ImagePath : IOwner
    {
        [Key, NotNull]
        public int Id { get; set; }
        [NotNull]
        public string Path { get; set; }
        [NotNull]
        public int ItemId { get; set; }
        [NotNull]
        public int OwnerId {  get; set; }
        [AllowNull, DefaultValue(false)]
        public bool IsRemoved { get; set; }
    }
}
