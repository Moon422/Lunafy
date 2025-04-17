using Lunafy.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace Lunafy.Data;

public class LunafyDbContext : DbContext
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Auth> Auths { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<GenreAlbumMapping> GenreAlbumMappings { get; set; }
    public DbSet<GenreSongMapping> GenreSongMappings { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<ArtistSongMapping> ArtistSongMappings { get; set; }
    public DbSet<User> Users { get; set; }

    public LunafyDbContext(DbContextOptions options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(e => e.Year)
                .HasMaxLength(4)
                .IsFixedLength();
        });

        modelBuilder.Entity<Auth>(entity =>
        {
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(60)
                .IsFixedLength()
                .IsRequired();

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Auth>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasMany<Album>()
                .WithMany()
                .UsingEntity<GenreAlbumMapping>(r => r.HasOne<Album>().WithMany().HasForeignKey(e => e.AlbumId).OnDelete(DeleteBehavior.Cascade).IsRequired(),
                    l => l.HasOne<Genre>().WithMany().HasForeignKey(e => e.GenreId).OnDelete(DeleteBehavior.Cascade).IsRequired());

            entity.HasMany<Song>()
                .WithMany()
                .UsingEntity<GenreSongMapping>(r => r.HasOne<Song>().WithMany().HasForeignKey(e => e.SongId).OnDelete(DeleteBehavior.Cascade).IsRequired(),
                    l => l.HasOne<Genre>().WithMany().HasForeignKey(e => e.GenreId).OnDelete(DeleteBehavior.Cascade).IsRequired());
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.Property(e => e.Year)
                .HasMaxLength(4)
                .IsFixedLength();

            entity.HasOne<Album>()
                .WithMany()
                .HasForeignKey(e => e.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<Artist>()
                .WithMany()
                .UsingEntity<ArtistSongMapping>(r => r.HasOne<Artist>().WithMany().HasForeignKey(e => e.ArtistId).OnDelete(DeleteBehavior.Cascade).IsRequired(),
                    l => l.HasOne<Song>().WithMany().HasForeignKey(e => e.SongId).OnDelete(DeleteBehavior.Cascade).IsRequired());
        });
    }
}