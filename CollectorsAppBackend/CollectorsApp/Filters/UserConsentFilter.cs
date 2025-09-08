using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class UserConsentFilter : BaseFilter
    {
        public string ConsentType { get; set; }
        public bool IsGranted { get; set; }
        public int OwnerId { get; set; }
    }
}
