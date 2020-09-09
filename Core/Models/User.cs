using System.Collections.Generic;

namespace Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string EncodedPassword { get; set; }
        public List<GrupaObiektow> GrupyObiektow { get; set; } = new List<GrupaObiektow>();
    }
}