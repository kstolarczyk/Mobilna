using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            optionsBuilder.UseSqlite($"Filename={DatabasePath};");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TypParametrow>().Property(t => t.AkceptowalneWartosci)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v)
                );
            modelBuilder.Entity<GrupaObiektowTypParametrow>()
                .HasKey(gt => new {gt.GrupaObiektowId, gt.TypParametrowId});
        }

        public async Task ClearExceptUserAsync()
        {
            var user = await Users.FirstOrDefaultAsync().ConfigureAwait(false);
            var images = await Obiekty.AsNoTracking().Select(o => o.ZdjecieLokal).Where(s => !string.IsNullOrEmpty(s))
                .ToListAsync().ConfigureAwait(false);
            await Database.EnsureDeletedAsync().ConfigureAwait(false);
            await Database.MigrateAsync().ConfigureAwait(false);
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            images.AsParallel().ForAll(s => File.Delete(Path.Combine(folder, s)));
            await Users.AddAsync(user).ConfigureAwait(false);
            await SaveChangesAsync().ConfigureAwait(false);

        }

        public DbSet<Obiekt> Obiekty { get; set; }
        public DbSet<Parametr> Parametry { get; set; }
        public DbSet<TypParametrow> TypyParametrow { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GrupaObiektow> GrupyObiektow { get; set; }
        public DbSet<GrupaObiektowTypParametrow> GrupaObiektowTypParametrow { get; set; }
    }

    public static class DbSetExtensions
    {
        public static void Clear<T>(this DbSet<T> table) where T : class
        {
            if (!table.Any()) return;
            table.RemoveRange(table);
        }
    }
}

