using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class UserPreferences : BaseModel
    {
        [NotNull, DefaultValue("Clasic")]
        public string? Layout { get; set; } = "Clasic";
        [NotNull, DefaultValue("Dark")]
        public string? Theme { get; set; } = "Dark";
        [AllowNull, DefaultValue(10)]
        public int? ItemsPerPage { get; set; } = 10;
        [AllowNull, DefaultValue(true)]
        public bool? Pagination { get; set; } = true;
    }
}
