using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Obiekt
    {
        public Obiekt()
        {
            OstatniaAktualizacja = DateTime.Now;
        }
        public int ObiektId { get; set; }
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public int GrupaObiektowId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Status { get; set; } // 1 - Added, 2 - Modified, 3 - Deleted, 4 - Synchronized
        public int UserId { get; set; }
        public User User { get; set; }
        public GrupaObiektow GrupaObiektow { get; set; }
        public DateTime OstatniaAktualizacja { get; set; }
        public List<Parametr> Parametry { get; set; } = new List<Parametr>();

    }
}