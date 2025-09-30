using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Services.User
{
    /// <summary>
    /// Provides functionality for managing user accounts, including user registration,  initialization of user consents
    /// and preferences, and secure handling of sensitive data.
    /// </summary>
    /// <remarks>This service is responsible for creating new user entries in the database, securely
    /// encrypting  and hashing sensitive user data, and setting default values for user properties. It also initializes
    /// user consents and preferences with default values. The service relies on injected dependencies for  data
    /// hashing, encryption, and repository operations.</remarks>
    public class UserService : IUserService
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        private readonly IUserRepository _userRepository;
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserPreferencesRepository _userPreferencesRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with the specified dependencies.
        /// </summary>
        /// <remarks>This constructor is typically used to inject the required dependencies for the <see
        /// cref="UserService"/>. Ensure that all parameters are non-null to avoid runtime exceptions.</remarks>
        /// <param name="dataHash">The service responsible for hashing user data.</param>
        /// <param name="aesEncryption">The service used for encrypting and decrypting user data.</param>
        /// <param name="userRepository">The repository for managing user information.</param>
        /// <param name="userConsentRepository">The repository for managing user consent data.</param>
        /// <param name="userPreferences">The repository for managing user preferences.</param>
        public UserService(IDataHash dataHash, IAesEncryption aesEncryption, IUserRepository userRepository, IUserConsentRepository userConsentRepository, IUserPreferencesRepository userPreferences) 
        {
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
            _userRepository = userRepository;
            _userConsentRepository = userConsentRepository;
            _userPreferencesRepository = userPreferences;
        }

        /// <summary>
        /// Registers a new user asynchronously, encrypting sensitive data and storing user information in the database.
        /// </summary>
        /// <remarks>This method performs the following operations: <list type="number">
        /// <item><description>Encrypts and hashes sensitive user data, such as the name and email.</description></item>
        /// <item><description>Generates and stores default user preferences and consent settings.</description></item>
        /// <item><description>Ensures the user is marked as active and assigns the default role of
        /// "user".</description></item> </list> The method ensures that sensitive data is securely handled using
        /// encryption and hashing mechanisms.</remarks>
        /// <param name="user">The <see cref="Users"/> object containing the user's information to be registered. The <see
        /// cref="Users.Password"/>, <see cref="Users.Name"/>, and <see cref="Users.Email"/> properties must be
        /// provided.</param>
        /// <returns>A <see cref="string"/> indicating the result of the registration process. Possible values include: <list
        /// type="bullet"> <item><description><c>"created successfully"</c> if the user was registered
        /// successfully.</description></item> <item><description><c>"user exists"</c> if a user with the same
        /// credentials already exists.</description></item> <item><description><c>"error creating user"</c> if an error
        /// occurred during the registration process.</description></item> </list></returns>
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
