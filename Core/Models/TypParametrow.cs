using System;

namespace Core.Models
{
    public class TypParametrow
    {
        public TypParametrow()
        {
            OstatniaAktualizacja ??= DateTime.Now;
        }
        public int TypParametrowId { get; set; }
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public string TypDanych { get; set; }
        public string JednostkaMiary { get; set; }
        public bool Usuniety { get; set; }
        public DateTime? OstatniaAktualizacja { get; set; }
        public string[] AkceptowalneWartosci { get; set; }

        public TypParametrow Update(TypParametrow other)
        {
            Symbol = other.Symbol;
            Nazwa = other.Nazwa;
            TypDanych = other.TypDanych;
            JednostkaMiary = other.JednostkaMiary;
            Usuniety = other.Usuniety;
            OstatniaAktualizacja = other.OstatniaAktualizacja;
            AkceptowalneWartosci = other.AkceptowalneWartosci;
            return this;
        }
    }
}