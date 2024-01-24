using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetDb.Models;
using Pets.Db.Models;

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

    public DbSet<MateRequestState> MateRequestStates { get; set; }

    public DbSet<MateRequest> MateRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "Users");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);

            entity.HasOne(u => u.Address)
                  .WithOne(a => a.ApplicationUser)
                  .HasForeignKey<Address>(a => a.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Cascade);

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
            entity.HasKey(e => e.Code);

            entity.Property(e => e.Code).HasColumnType("char(2)");
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

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Line1).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Line2).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Postcode).HasMaxLength(20).IsRequired();

            entity.HasOne(a => a.Location)
                  .WithOne(l => l.Address)
                  .HasForeignKey<Location>(l => l.AddressId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("Address");
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

        modelBuilder.Entity<MateRequestState>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(20);

            entity.ToTable("MateRequestState");
        });

        modelBuilder.Entity<MateRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(3000);
            entity.Property(e => e.AmountAgreement).HasMaxLength(250);
            entity.Property(e => e.LitterSplitAgreement).HasMaxLength(250);
            entity.Property(e => e.BreedingPlaceAgreement).HasMaxLength(250);
            entity.Property(e => e.Response).HasMaxLength(3000);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreationDate).HasColumnType("date");
            entity.HasIndex(e => e.PetOwnerId);
            entity.HasIndex(e => e.PetMateOwnerId);

            entity.HasOne(p => p.PetProfile).WithMany().HasForeignKey(p => p.PetProfileId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            entity.HasOne(p => p.PetMateProfile).WithMany().HasForeignKey(p => p.PetMateProfileId).OnDelete(DeleteBehavior.NoAction).IsRequired();
            entity.HasOne(p => p.MateRequestState).WithMany(p => p.MateRequests).HasForeignKey(p => p.MateRequestStateId).IsRequired();

            entity.ToTable("MateRequest");
        });

        base.OnModelCreating(modelBuilder);
    }
}