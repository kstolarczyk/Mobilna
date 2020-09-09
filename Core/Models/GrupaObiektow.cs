using System.Collections.Generic;

namespace Core.Models
{
    public class GrupaObiektow
    {
        public int GrupaObiektowId { get; set; }
        public string Nazwa { get; set; }
        public string Symbol { get; set; }
        public List<Obiekt> Obiekty { get; set; } = new List<Obiekt>();
        public List<TypParametrow> TypyParametrow { get; set; } = new List<TypParametrow>();
    }
}