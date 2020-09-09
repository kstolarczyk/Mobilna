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
    }
}