using CollectorsApp.Models.Interfaces;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models
{
    public class Collections : IOwner
    {
        [Key, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public int ParentId { get; set; }
        [NotNull]
        public string ParentName { get; set; }
        [NotNull]
        public int Depth { get; set; }
        [NotNull]
        public int OwnerId { get; set; } 

    }
}
