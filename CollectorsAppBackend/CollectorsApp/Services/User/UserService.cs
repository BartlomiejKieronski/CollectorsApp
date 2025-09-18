using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Services.User
{
    public class UserService : IUserService
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        private readonly IUserRepository _userRepository;
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserPreferencesRepository _userPreferencesRepository;
        public UserService(IDataHash dataHash, IAesEncryption aesEncryption, IUserRepository userRepository, IUserConsentRepository userConsentRepository, IUserPreferencesRepository userPreferences) 
        {
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
            _userRepository = userRepository;
            _userConsentRepository = userConsentRepository;
            _userPreferencesRepository = userPreferences;
        }

        public async Task<string> RegisterUserAsync(Users user)
        {
            try
            {
                var credentials = await _dataHash.GetCredentialsAsync(user.Password);
                var hashName = await _dataHash.GenerateHmacAsync(user.Name);
                var hashEmail = await _dataHash.GenerateHmacAsync(user.Email);
                var encryptedName = await _aesEncryption.AesEncrypt(user.Name);
                var encryptedEmail = await _aesEncryption.AesEncrypt(user.Email);

                user.Email = encryptedEmail.Item1;
                user.EmailIVKey = encryptedEmail.Item2;
                user.HashedEmail = hashEmail;
                user.Name = encryptedName.Item1;
                user.NameIVKey = encryptedName.Item2;
                user.HashedName = hashName;
                user.Salt = credentials.Item1;
                user.Password = credentials.Item2;

                user.Active = true;
                user.Role = "user";
                user.AccountCreationDate = DateTime.Now;
                user.IsBanned = false;
                user.IsSusspended = false;

                var succes = await _userRepository.PostUser(user);
            
                if (succes == "user exists")
                {
                    return "user exists";
                }
             
                var IPConsent = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted=false,ConsentType = "IPAdress"};
                var Id = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted = false, ConsentType = "PersonalIdentifyingData" };
                var DeviceId = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted = false, ConsentType = "DeviceID" };
                var Analytics = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted = false, ConsentType = "Analytics" };
                var ErrorReporting = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted = false, ConsentType = "ErrorReporting" };
                var Cookies = new UserConsent() { OwnerId = Convert.ToInt32(succes), IsGranted = false, ConsentType = "Cookies" };
            
                await _userConsentRepository.PostAsync(IPConsent);
                await _userConsentRepository.PostAsync(Id);
                await _userConsentRepository.PostAsync(Analytics);
                await _userConsentRepository.PostAsync(ErrorReporting);
                await _userConsentRepository.PostAsync(Cookies);
                await _userConsentRepository.PostAsync(DeviceId);

                var preferences = new UserPreferences() { OwnerId = Convert.ToInt32(succes), Theme = "dark", ItemsPerPage = 20, Layout = "Clasic", Pagination = true };

                await _userPreferencesRepository.PostAsync(preferences);
            }
            catch
            {
                return "error creating user";
            }
            return "created succesfully";
        }
    }
}
