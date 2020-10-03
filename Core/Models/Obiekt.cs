using System;
using System.Collections.Generic;
using Core.Utility.Model;

namespace Core.Models
{
    public class Obiekt : ValidateBase, IEquatable<Obiekt>
    {
        public Obiekt()
        {
            OstatniaAktualizacja ??= DateTime.Now;
            ZdjecieLokal ??= string.Empty;
        }
        public int ObiektId { get; set; }
        public int? RemoteId { get; set; }
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public int GrupaObiektowId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Status { get; set; } // 1 - Added, 2 - Modified, 3 - Deleted, 0 - none
        public int? UserId { get; set; }
        public User User { get; set; }
        public string Zdjecie { get; set; }
        public GrupaObiektow GrupaObiektow { get; set; }
        public DateTime? OstatniaAktualizacja { get; set; }
        public bool Usuniety { get; set; }
        public List<Parametr> Parametry { get; set; } = new List<Parametr>();
        public string ZdjecieLokal { get; set; }

        public bool Equals(Obiekt other)
        {
            return other != null && other.RemoteId == RemoteId;
        }
    }
}