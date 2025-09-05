using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class UserConsent : BaseModel
    {
        [NotNull]
        public string ConsentType { get; set; }
        [NotNull]
        public bool IsGranted { get; set; }
    }
}
