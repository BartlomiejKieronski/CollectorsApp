using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ServiceStack.DataAnnotations;

namespace CollectorsApp.Models
{
    public class Users
    {
        [Key, NotNull, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [AllowNull]
        public string? HashedName { get; set; }
        [AllowNull]
        public string? NameIVKey { get; set; }

        [NotNull, EmailAddress]
        public string Email { get; set; }
        [AllowNull]
        public string? HashedEmail { get; set; }
        [AllowNull]
        public string? EmailIVKey { get; set; }

        [AllowNull, RegularExpression("^.{6,32}$")]
        public string Password { get; set; }
        [AllowNull]
        public string? Salt { get; set; }
        [AllowNull, DefaultValue("user")]
        public string? Role { get; set; }
        [AllowNull, DefaultValue(true)]
        public bool? Active { get; set; }
        [AllowNull]
        public DateTime AccountCreationDate { get; set; }
    }
}
