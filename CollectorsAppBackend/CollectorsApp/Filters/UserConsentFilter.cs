using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Filters
{
    public class UserConsentFiler : BaseFilters
    {
        public string ConsentType { get; set; }
        public bool IsGranted { get; set; }
    }
}
