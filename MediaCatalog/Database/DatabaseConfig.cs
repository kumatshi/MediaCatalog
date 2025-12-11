using Microsoft.EntityFrameworkCore;
using MediaCatalog.Models;
using System.Collections.ObjectModel;

namespace MediaCatalog.Database
{
    public class DatabaseConfig
    {
        public static string GetConnectionString()
        {
            return "Host=localhost;Port=5432;Database=mediacatalog;Username=postgres;Password=password;Timeout=300;CommandTimeout=300";
        }
    }

    public class MediaCatalogContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Music> Musics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                optionsBuilder.UseNpgsql(DatabaseConfig.GetConnectionString());
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).HasMaxLength(100);
                entity.Property(e => e.Year);
                entity.Property(e => e.Rating);
                entity.Property(e => e.DateAdded)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");
                entity.Property(e => e.StatusType).HasMaxLength(50);
                entity.Property(e => e.Author).HasMaxLength(100);
                entity.Property(e => e.PageCount);
                entity.Property(e => e.ISBN).HasMaxLength(20);
            });
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).HasMaxLength(100);
                entity.Property(e => e.Year);
                entity.Property(e => e.Rating);
                entity.Property(e => e.DateAdded)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");
                entity.Property(e => e.StatusType).HasMaxLength(50);
                entity.Property(e => e.Director).HasMaxLength(100);
                entity.Property(e => e.Duration);
                entity.Property(e => e.Studio).HasMaxLength(100);
            });
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).HasMaxLength(100);
                entity.Property(e => e.Year);
                entity.Property(e => e.Rating);
                entity.Property(e => e.DateAdded)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");
                entity.Property(e => e.StatusType).HasMaxLength(50);
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Developer).HasMaxLength(100);
                entity.Property(e => e.PlayTime);
            });
            modelBuilder.Entity<Music>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Genre).HasMaxLength(100);
                entity.Property(e => e.Year);
                entity.Property(e => e.Rating);
                entity.Property(e => e.DateAdded)
                    .HasColumnType("timestamp without time zone")
                    .HasDefaultValueSql("NOW()");
                entity.Property(e => e.StatusType).HasMaxLength(50);
                entity.Property(e => e.Artist).HasMaxLength(100);
                entity.Property(e => e.Album).HasMaxLength(100);
                entity.Property(e => e.Duration);
                entity.Property(e => e.Format).HasMaxLength(10);
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.FileSize);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}