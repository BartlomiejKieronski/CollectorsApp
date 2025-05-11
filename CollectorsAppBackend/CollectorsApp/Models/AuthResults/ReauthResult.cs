namespace CollectorsApp.Models.AuthResults
{
    public class ReauthResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        
        private ReauthResult() { }

        public static ReauthResult Fail(string error) => new ReauthResult
        {
            Success = false,
            ErrorMessage = error
        };

        public static ReauthResult SuccessResult() => new ReauthResult
        {
            Success = true,
        };
    }
}
