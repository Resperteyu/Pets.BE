using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<PetBreed> PetBreeds { get; set; }

    public virtual DbSet<PetProfile> PetProfiles { get; set; }

    public virtual DbSet<PetType> PetTypes { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Sex> Sexes { get; set; }

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

            entity.HasOne(d => d.Type).WithMany(p => p.PetBreeds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PetBreed_TypeId");
        });

        modelBuilder.Entity<PetProfile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Breed).WithMany(p => p.PetProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PetProfile_BreedId");

            entity.HasOne(d => d.Owner).WithMany(p => p.PetProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PetProfile_OwnerId");

            entity.HasOne(d => d.Sex).WithMany(p => p.PetProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PetProfile_SexId");
        });

        modelBuilder.Entity<PetType>(entity =>
        {
            entity.Property(e => e.Name).IsFixedLength();
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Profiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Profile_CountryCode");

            entity.HasOne(d => d.Location).WithMany(p => p.Profiles).HasConstraintName("FK_Profile_LocationId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
