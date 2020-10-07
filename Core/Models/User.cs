using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string EncodedPassword { get; set; }
        public List<GrupaObiektow> GrupyObiektow { get; set; } = new List<GrupaObiektow>();
    }
}