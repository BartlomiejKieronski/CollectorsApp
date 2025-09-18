using CollectorsApp.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class PasswordReset : IOwner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(1000)]
        public string Email { get; set; }
        [Required]
        [MaxLength(256)]
        public string Token { get; set; }
        [Required]
        public int OwnerId { get; set; }
    }
}
