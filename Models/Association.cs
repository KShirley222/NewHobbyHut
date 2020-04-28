using System.ComponentModel.DataAnnotations;

namespace HobbyHut.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int HobbyId { get; set; }
        public Hobby HobbyA { get; set; }
        public User UserA { get; set; }
    }
}