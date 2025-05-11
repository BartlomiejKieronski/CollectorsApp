namespace CollectorsApp.Models.AuthResults
{
    public class LoginResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public LoggedUserInfo? User { get; private set; }

        private LoginResult() { }

        public static LoginResult Fail(string error) => new LoginResult
        {
            Success = false,
            ErrorMessage = error
        };

        public static LoginResult SuccessResult(LoggedUserInfo user) => new LoginResult
        {
            Success = true,
            User = user
        };
    }
}
