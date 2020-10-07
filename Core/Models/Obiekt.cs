using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped] public string Opis => $"[{Symbol}] {Nazwa}";

        [NotMapped] public string Lokalizacja => FormattedCoords();

        public void Update(Obiekt other)
        {
            Nazwa = other.Nazwa;
            Symbol = other.Symbol;
            ZdjecieLokal = other.ZdjecieLokal;
            Zdjecie = other.Zdjecie;
            Latitude = other.Latitude;
            Longitude = other.Longitude;
            OstatniaAktualizacja = other.OstatniaAktualizacja;
            foreach (var parametr in other.Parametry)
            {
                var toUpdate = Parametry.Find(p => p.TypParametrowId == parametr.TypParametrowId);
                toUpdate.Update(parametr);
            }
        }
        public bool Equals(Obiekt other)
        {
            return other != null && other.RemoteId == RemoteId;
        }

        public string FormattedCoords()
        {
            var latitudeSeconds = Math.Abs(Latitude) * 3600;
            var longitudeSeconds = Math.Abs(Longitude) * 3600;
            var latDegrees = (int) (latitudeSeconds / 3600);
            var longDegrees = (int) (longitudeSeconds / 3600);
            latitudeSeconds -= latDegrees * 3600;
            longitudeSeconds -= longDegrees * 3600;
            var latMinutes = (int) (latitudeSeconds / 60);
            var longMinutes = (int) (longitudeSeconds / 60);
            latitudeSeconds -= latMinutes * 60;
            longitudeSeconds -= longMinutes * 60;
            var latSymbol = Latitude > 0 ? "N" : "S";
            var longSymbol = Longitude > 0 ? "E" : "W";
            return
                $"{latDegrees}°{latMinutes}'{latitudeSeconds:F1}''{latSymbol}; {longDegrees}°{longMinutes}'{longitudeSeconds:F1}''{longSymbol}";
        }
    }
}