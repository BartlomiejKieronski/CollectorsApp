using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CollectorsApp.Models
{
    public class PasswordResetModel : IOwner
    {
        [Key, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Email { get; set; }
        [NotNull]
        public string Token { get; set; }
        [NotNull]
        public int OwnerId { get; set; } 

    }
}
