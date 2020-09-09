namespace Core.Models
{
    public class TypParametrow
    {
        public int TypParametrowId { get; set; }
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public string TypDanych { get; set; }
        public string JednostkaMiary { get; set; }
        public string[] AkceptowalneWartosci { get; set; }
    }
}