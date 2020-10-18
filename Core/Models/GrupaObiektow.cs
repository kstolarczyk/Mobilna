using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvvmCross.ViewModels;

namespace Core.Models
{
    public class GrupaObiektow : MvxNotifyPropertyChanged
    {
        public GrupaObiektow()
        {
            OstatniaAktualizacja ??= DateTime.Now;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int GrupaObiektowId { get; set; }

        private string _nazwa;
        private string _symbol;
        public string Nazwa { get => _nazwa; set => SetProperty(ref _nazwa, value); }
        public string Symbol { get => _symbol; set => SetProperty(ref _symbol, value); }
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
        public override string ToString()
        {
            return $"{Nazwa} [{Symbol}]";
        }
    }
}