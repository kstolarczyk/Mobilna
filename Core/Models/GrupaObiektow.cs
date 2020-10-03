using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class GrupaObiektow
    {
        public GrupaObiektow()
        {
            OstatniaAktualizacja ??= DateTime.Now;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int GrupaObiektowId { get; set; }
        public string Nazwa { get; set; }
        public string Symbol { get; set; }
        public DateTime? OstatniaAktualizacja { get; set; }
        public bool Usunieta { get; set; }
        public List<Obiekt> Obiekty { get; set; } = new List<Obiekt>();
        public List<TypParametrow> TypyParametrow { get; set; } = new List<TypParametrow>();

        public GrupaObiektow Update(GrupaObiektow other)
        {
            Nazwa = other.Nazwa;
            Symbol = other.Symbol;
            OstatniaAktualizacja = other.OstatniaAktualizacja;
            Usunieta = other.Usunieta;
            TypyParametrow = other.TypyParametrow;
            return this;
        }
    }
}