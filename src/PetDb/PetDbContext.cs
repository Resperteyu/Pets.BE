using Microsoft.EntityFrameworkCore;
using PetDb.Models;

namespace Movrr.API;

public partial class PetDbContext : DbContext
{
    public PetDbContext()
    {
    }

    public PetDbContext(DbContextOptions<PetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<PetBreed> PetBreeds { get; set; }

    public DbSet<PetProfile> PetProfiles { get; set; }

    public DbSet<PetType> PetTypes { get; set; }

    public DbSet<Profile> Profiles { get; set; }

    public DbSet<Sex> Sexes { get; set; }


    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=PetDb;Integrated Security=True;TrustServerCertificate=Yes");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.CountryCode).IsFixedLength();
        });

        modelBuilder.Entity<PetBreed>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne<PetType>().WithMany().HasForeignKey(p => p.TypeId);
        });

        modelBuilder.Entity<PetProfile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne<PetBreed>().WithMany().HasForeignKey(p => p.BreedId);

            entity.HasOne<Profile>().WithMany().HasForeignKey(p => p.OwnerId);

            entity.HasOne<Sex>().WithMany().HasForeignKey(p =>p.SexId);
        });

        modelBuilder.Entity<PetType>(entity =>
        {
            entity.Property(e => e.Name).IsFixedLength();
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne<Country>().WithMany().HasForeignKey(p => p.CountryCode);

            entity.HasOne<Location>().WithMany().HasForeignKey(p => p.LocationId);

            entity.OwnsOne(e => e.RefreshTokens);
        });
    }
}
