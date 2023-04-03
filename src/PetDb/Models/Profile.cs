using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("Profile")]
public partial class Profile
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(2)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string LastName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string PhoneNumber { get; set; }

    public bool EmailVerified { get; set; }

    public bool PhoneVerified { get; set; }

    public int? LocationId { get; set; }

    [ForeignKey("CountryCode")]
    [InverseProperty("Profiles")]
    public virtual Country CountryCodeNavigation { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("Profiles")]
    public virtual Location Location { get; set; }

    [InverseProperty("Owner")]
    public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
}
