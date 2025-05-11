using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class RefreshTokenInfo : IOwner
    {
        [Key, NotNull, AutoIncrement]
        public int Id { get; set; }
        [AllowNull]
        public string RefreshToken { get; set; }
        [AllowNull]
        public int OwnerId { get; set; }
        [AllowNull, DataType(DataType.DateTime)]
        public DateTime DateOfIssue { get; set; }
        [AllowNull]
        public string IssuerDeviceInfo { get; set; }
        [NotNull]
        public bool IsValid { get; set; } = false;

    }


}
