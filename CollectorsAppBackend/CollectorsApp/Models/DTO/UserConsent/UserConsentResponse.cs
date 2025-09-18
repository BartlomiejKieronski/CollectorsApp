namespace CollectorsApp.Models.DTO.UserConsent
{
    public sealed class UserConsentResponse
    {
        public int Id { get; init; }
        public string ConsentType { get; init; } = default!;
        public bool IsGranted { get; init; }
        public DateTime? TimeStamp { get; init; }
    }
}
