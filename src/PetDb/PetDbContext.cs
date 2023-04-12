using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PetDb.Models;

namespace PetDb;

public partial class PetDbContext : DbContext
{
    public PetDbContext()
    {
    }

    public PetDbContext(DbContextOptions<PetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<PetBreed> PetBreeds { get; set; }

    public DbSet<PetProfile> PetProfiles { get; set; }

    public DbSet<PetType> PetTypes { get; set; }

    public DbSet<Sex> Sexes { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=PetDb-test;Integrated Security=True;TrustServerCertificate=Yes");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property<DateTime>("Created").HasColumnType("datetime2");
            entity.Property<string>("Email").HasColumnType("nvarchar(max)");
            entity.Property<string>("PasswordHash").HasColumnType("nvarchar(max)");
            entity.Property<DateTime?>("PasswordReset").HasColumnType("datetime2");
            entity.Property<string>("ResetToken").HasColumnType("nvarchar(max)");
            entity.Property<DateTime?>("ResetTokenExpires").HasColumnType("datetime2");
            entity.Property<DateTime?>("Updated").HasColumnType("datetime2");
            entity.Property<string>("VerificationToken").HasColumnType("nvarchar(max)");
            entity.Property<DateTime?>("Verified").HasColumnType("datetime2");

            entity.OwnsMany(e => e.RefreshTokens);

            entity.ToTable("Account");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            //TDB

            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            entity.Property<Guid>("AccountId").HasColumnType("uniqueidentifier");
            entity.Property<DateTime>("Created").HasColumnType("datetime2");
            entity.Property<string>("CreatedByIp").HasColumnType("nvarchar(max)");
            entity.Property<DateTime>("Expires").HasColumnType("datetime2");
            entity.Property<string>("ReplacedByToken").HasColumnType("nvarchar(max)");
            entity.Property<DateTime?>("Revoked").HasColumnType("datetime2");
            entity.Property<string>("RevokedByIp").HasColumnType("nvarchar(max)");
            entity.Property<string>("Token").HasColumnType("nvarchar(max)");

            entity.HasIndex("AccountId");

            entity.ToTable("RefreshToken");

            //TDB entity.WithOwner("Account").HasForeignKey("AccountId");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CountryCode).HasColumnType("char(2)");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);

            //entity.HasOne<Account>().WithOne().HasForeignKey(p => p.Id);
            entity.HasOne<Country>().WithMany().HasForeignKey(p => p.CountryCode);
            entity.HasOne<Location>().WithMany().HasForeignKey(p => p.LocationId);

            entity.ToTable("Profile");
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
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(40);

            entity.HasOne<PetType>().WithMany().HasForeignKey(p => p.TypeId);

            entity.ToTable("PetBreed");
        });

        modelBuilder.Entity<PetProfile>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(250);

            entity.HasOne<PetBreed>().WithMany().HasForeignKey(p => p.BreedId);
            entity.HasOne<Profile>().WithMany().HasForeignKey(p => p.OwnerId);
            entity.HasOne<Sex>().WithMany().HasForeignKey(p =>p.SexId);

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

        
    }
}
