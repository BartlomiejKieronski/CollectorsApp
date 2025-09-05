using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class Collections : BaseModel
    {
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public int ParentId { get; set; }
        [NotNull]
        public string ParentName { get; set; }
        [NotNull]
        public int Depth { get; set; }
        [AllowNull, DefaultValue(false)]
        public bool IsRemoved { get; set; }
    }
}
