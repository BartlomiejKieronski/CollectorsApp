using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class UserPreferences : IOwner
    {
        [NotNull, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int OwnerId {  get; set; }
        [NotNull, DefaultValue("Classical")]
        public string Layout { get; set; }
        public string Theme { get; set; }
        [AllowNull, DefaultValue(10)]
        public int ItemsPerPage { get; set; }
        [AllowNull, DefaultValue(true)]
        public bool Pagination { get; set; }
    }
}
