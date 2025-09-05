using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CollectorsApp.Models.Interfaces;

namespace CollectorsApp.Models
{
    public class ImagePath : BaseModel
    {
        [NotNull]
        public string Path { get; set; }
        [NotNull]
        public int ItemId { get; set; }
        [AllowNull, DefaultValue(false)]
        public bool IsRemoved { get; set; }
    }
}
