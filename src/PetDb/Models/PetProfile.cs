using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("PetProfile")]
public partial class PetProfile
{
    [Key]
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public byte SexId { get; set; }

    public int BreedId { get; set; }

    [Column(TypeName = "date")]
    public DateTime DateOfBirth { get; set; }

    public bool AvailableForBreeding { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Name { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string Description { get; set; }

    [ForeignKey("BreedId")]
    [InverseProperty("PetProfiles")]
    public virtual PetBreed Breed { get; set; }

    [ForeignKey("OwnerId")]
    [InverseProperty("PetProfiles")]
    public virtual Profile Owner { get; set; }

    [ForeignKey("SexId")]
    [InverseProperty("PetProfiles")]
    public virtual Sex Sex { get; set; }
}
