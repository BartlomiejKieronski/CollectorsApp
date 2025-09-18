using CollectorsApp.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectorsApp.Models
{
	public class Users : ILastUpdated
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(1000)]
		public string Name { get; set; }
		[MaxLength(1000)]
		public string HashedName { get; set; }
		[MaxLength(1000)]
		public string NameIVKey { get; set; }
		[Required]
		[EmailAddress]
		[MaxLength(1000)]
		public string Email { get; set; }
		[MaxLength(1000)]
		public string HashedEmail { get; set; }
		[MaxLength(1000)]
		public string EmailIVKey { get; set; }
		[MaxLength(1000)]
		public string Password { get; set; }
		[MaxLength(256)]
		public string Salt { get; set; }
		[MaxLength(32)]
		[Required]
		public string Role { get; set; } 
		[Required]
		public bool Active { get; set; } = true;
		public DateTime? AccountCreationDate { get; set; }
		[Required]
		public bool IsSusspended { get; set; } = false;
		[Required]
		public bool IsBanned { get; set; } = false;
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime TimeStamp { get; set; } 
		public DateTime? LastLogin { get; set; }
		public DateTime? LastLogout { get; set; }
		public DateTime? LastUpdated { get; set; } 
		public DateTime? Deleted { get; set; }
	}
}
