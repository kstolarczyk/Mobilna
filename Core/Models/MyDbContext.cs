using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Core.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : this(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
        {
        }
        public MyDbContext(string path)
        {
            DatabasePath = Path.Combine(path, "EwidencjaObiektow.db");
        }

        public string DatabasePath { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DatabasePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TypParametrow>().Property(t => t.AkceptowalneWartosci)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                );
        }

        public DbSet<Obiekt> Obiekty { get; set; }
        public DbSet<Parametr> Parametry { get; set; }
        public DbSet<TypParametrow> TypyParametrow { get; set; }
        public DbSet<User> Users { get; set; }
    }
}