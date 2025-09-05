using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class UserFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public bool? Active { get; set; }
        public DateTime AccountCreationDate { get; set; }
        public bool IsSusspended { get; set; }
        public bool IsBanned { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; }
        public DateTime LastLogout { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Deleted { get; set; }
    }
}
