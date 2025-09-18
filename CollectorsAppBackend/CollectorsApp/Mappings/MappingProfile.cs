using AutoMapper;
using CollectorsApp.Models;
using CollectorsApp.Models.Analytics;
using CollectorsApp.Models.APILogs;
using CollectorsApp.Models.DTO.AdminComments;
using CollectorsApp.Models.DTO.APILogs;
using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Models.DTO.CollectableItems;
using CollectorsApp.Models.DTO.Collections;
using CollectorsApp.Models.DTO.ImagePaths;
using CollectorsApp.Models.DTO.UserConsent;
using CollectorsApp.Models.DTO.UserPreferences;
using CollectorsApp.Models.DTO.Users;

namespace CollectorsApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add your mappings here
            // CreateMap<Source, Destination>();

            CreateMap<AdminCommentCreateRequest, AdminComment>();
            CreateMap<AdminComment, AdminCommentResponse>();
            
            CreateMap<APILog,APILogResponse>();

            CreateMap<LoginRequest, Users>();
            CreateMap<PasswordResetConfirmRequest, Users>();
            CreateMap<RegisterRequest,Users>();
            CreateMap<PasswordResetRequest, Users>();

            CreateMap<CollectableItemCreateRequest,CollectableItems>();
            CreateMap<CollectableItemUpdateRequest,CollectableItems>();
            CreateMap<CollectableItems, CollectableItemResponse>();

            CreateMap<Collections, CollectionResponse>();
            CreateMap<CollectionCreateRequest, Collections>();
            CreateMap<CollectionUpdateRequest, Collections>();

            CreateMap<ImagePath, ImagePathResponse>();
            CreateMap<ImagePathCreateRequest, ImagePath>();
            CreateMap<ImagePathUpdateRequest, ImagePath>();

            CreateMap<UserConsentCreateOrUpdateRequest, UserConsent>();
            CreateMap<UserConsent, UserConsentResponse>();

            CreateMap<UserPreferences, UserPreferencesResponse>();
            CreateMap<UserPreferencesUpdateRequest, UserPreferences>();

            CreateMap<UserUpdateRequest, Users>();
            CreateMap<Users, UserResponse>();
        }
    }
}
