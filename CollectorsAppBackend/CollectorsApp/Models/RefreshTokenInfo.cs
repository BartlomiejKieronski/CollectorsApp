using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class RefreshTokenInfo : BaseModel
    {
        [AllowNull]
        public string RefreshToken { get; set; }
        
        [AllowNull, DataType(DataType.DateTime)]
        public DateTime DateOfIssue { get; set; }
        [AllowNull]
        public string IssuerDeviceInfo { get; set; }
        [NotNull]
        public bool IsValid { get; set; } = false;

    }


}
