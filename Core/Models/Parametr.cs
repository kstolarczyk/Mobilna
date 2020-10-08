using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Parametr
    {
        public int ParametrId { get; set; }
        public int ObiektId { get; set; }
        public int TypParametrowId { get; set; }
        public string Wartosc { get; set; }
        public TypParametrow TypParametrow { get; set; }
        public Obiekt Obiekt { get; set; }
        [NotMapped] public string NazwaParametru => $"{TypParametrow?.Nazwa}:";
        [NotMapped] public string WartoscParametru => $"{Wartosc}" + (string.IsNullOrEmpty(TypParametrow?.JednostkaMiary) ? "" : $" [{TypParametrow.JednostkaMiary}]");

        public void Update(Parametr other)
        {
            Wartosc = other.Wartosc;
        }
    }
}