﻿using CollectorsApp.Models;

namespace CollectorsApp.Services.Token
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        Task<string> GenerateJwtToken(LoggedUserInfo userInfo, int expires);
        bool ValidateRefreshTokenDate(DateTime issued, int validDays);
    }
}
