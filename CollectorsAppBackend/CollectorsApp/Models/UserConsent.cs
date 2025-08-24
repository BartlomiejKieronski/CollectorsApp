using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class UserConsent
    {
        [NotNull,AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int UserId { get; set; }
        [NotNull]
        public string ConsentType { get; set; }
        [NotNull]
        public bool IsGranted { get; set; }
        [NotNull]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
