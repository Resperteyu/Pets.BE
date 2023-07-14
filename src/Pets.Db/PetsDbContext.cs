using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetDb.Models;
using Pets.Db.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pets.Db;

public partial class PetsDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public PetsDbContext()
    {

    }

    public PetsDbContext(DbContextOptions<PetsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<PetBreed> PetBreeds { get; set; }

    public DbSet<PetProfile> PetProfiles { get; set; }

    public DbSet<PetType> PetTypes { get; set; }

    public DbSet<Sex> Sexes { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "Users");
            entity.Property(e => e.CountryCode).HasColumnType("char(2)");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.HasOne(e => e.Country).WithMany().HasForeignKey(p => p.CountryCode).IsRequired(false);
            entity.HasOne(e => e.Location).WithMany().HasForeignKey(p => p.LocationId).IsRequired(false);
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable(name: "Roles");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable(name: "UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryCode);

            entity.Property(e => e.CountryCode).HasColumnType("char(2)");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.DialCode).HasMaxLength(4);

            entity.ToTable("Country");
        });

        modelBuilder.Entity<PetBreed>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(40);

            entity.HasOne<PetType>().WithMany().HasForeignKey(p => p.TypeId);

            entity.ToTable("PetBreed");
        });

        modelBuilder.Entity<PetProfile>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.DateOfBirth).HasColumnType("date");

            entity.HasOne(e => e.Breed).WithMany(e => e.PetProfiles).HasForeignKey(p => p.BreedId).IsRequired();
            entity.HasOne(e => e.Owner).WithMany(e => e.PetProfiles).HasForeignKey(p => p.OwnerId).IsRequired();
            entity.HasOne(e => e.Sex).WithMany(e => e.PetProfiles).HasForeignKey(p => p.SexId).IsRequired();

            entity.ToTable("PetProfile");
        });

        modelBuilder.Entity<PetType>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(10);

            entity.ToTable("PetType");
        });

        modelBuilder.Entity<Sex>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(10);

            entity.ToTable("Sex");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)");

            entity.ToTable("Location");
        });

        base.OnModelCreating(modelBuilder);
        //modelBuilder.Seed();
    }
}
