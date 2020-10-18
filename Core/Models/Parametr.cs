using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Utility.Model;

namespace Core.Models
{
    public class Parametr : ValidateBase
    {
        private string _wartosc;
        public int ParametrId { get; set; }
        public int ObiektId { get; set; }
        public int TypParametrowId { get; set; }
        [Required]
        public string Wartosc { get => _wartosc; set => SetProperty(ref _wartosc, value); }
        public TypParametrow TypParametrow { get; set; }
        public Obiekt Obiekt { get; set; }

        [NotMapped] public string WartoscError => GetSingleError(nameof(Wartosc));
        [NotMapped] public string NazwaParametru => $"{TypParametrow?.Nazwa}:";
        [NotMapped] public string WartoscParametru => $"{Wartosc}" + (string.IsNullOrEmpty(TypParametrow?.JednostkaMiary) ? "" : $" [{TypParametrow.JednostkaMiary}]");

        public void Update(Parametr other)
        {
            Wartosc = other.Wartosc;
        }

        protected override IEnumerable<string> ValidateCustom(string propertyName)
        {
            return base.ValidateCustom(propertyName);
        }
    }
}