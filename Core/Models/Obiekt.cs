using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Core.Utility.Model;

namespace Core.Models
{
    public class Obiekt : ValidateBase, IEquatable<Obiekt>
    {
        private string _nazwa;
        private string _symbol;
        private decimal _latitude;
        private decimal _longitude;
        private GrupaObiektow _grupaObiektow;

        public Obiekt()
        {
            OstatniaAktualizacja ??= DateTime.Now;
            ZdjecieLokal ??= string.Empty;
        }

        public int ObiektId { get; set; }
        public int? RemoteId { get; set; }

        [Required]
        public string Symbol
        {
            get => _symbol;
            set => SetProperty(ref _symbol, value);
        }

        [Required]
        public string Nazwa
        {
            get => _nazwa;
            set => SetProperty(ref _nazwa, value);
        }

        public int GrupaObiektowId { get; set; }

        public decimal Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public decimal Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public int Status { get; set; } // 1 - Added, 2 - Modified, 3 - Deleted, 0 - none
        public int? UserId { get; set; }
        public User User { get; set; }
        public string Zdjecie { get; set; }

        [Required]
        public GrupaObiektow GrupaObiektow
        {
            get => _grupaObiektow;
            set => SetProperty(ref _grupaObiektow, value);
        }

        public DateTime? OstatniaAktualizacja { get; set; }
        public bool Usuniety { get; set; }
        public List<Parametr> Parametry { get; set; } = new List<Parametr>();
        public string ZdjecieLokal { get; set; }

        [NotMapped] public string Opis => $"[{Symbol}] {Nazwa}";

        [NotMapped] public string LokalizacjaOneLine => FormattedCoords("; ");
        [NotMapped] public string LokalizacjaMultiLine => FormattedCoords(Environment.NewLine);

        [NotMapped] public string NazwaError => GetSingleError(nameof(Nazwa));
        [NotMapped] public string SymbolError => GetSingleError(nameof(Symbol));
        [NotMapped] public string GrupaObiektowError => GetSingleError(nameof(GrupaObiektow));
        public void Update(Obiekt other)
        {
            Nazwa = other.Nazwa;
            Symbol = other.Symbol;
            Zdjecie = other.Zdjecie;
            Latitude = other.Latitude;
            Longitude = other.Longitude;
            OstatniaAktualizacja = other.OstatniaAktualizacja;
            foreach (var parametr in other.Parametry)
            {
                var toUpdate = Parametry.Find(p => p.TypParametrowId == parametr.TypParametrowId);
                toUpdate?.Update(parametr);
            }
        }

        public new void ValidateEntity()
        {
            base.ValidateEntity();
            Parametry.AsParallel().ForAll(p => p.ValidateEntity());
        }

        public bool Equals(Obiekt other)
        {
            return other != null && other.RemoteId == RemoteId;
        }

        public string FormattedCoords(string delimiter)
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
                $"{latDegrees}°{latMinutes}'{latitudeSeconds:F1}''{latSymbol}{delimiter}{longDegrees}°{longMinutes}'{longitudeSeconds:F1}''{longSymbol}";
        }
    }
}