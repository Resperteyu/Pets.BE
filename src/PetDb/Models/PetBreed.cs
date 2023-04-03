using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("PetBreed")]
public partial class PetBreed
{
    [Key]
    public int Id { get; set; }

    public byte TypeId { get; set; }

    [Required]
    [StringLength(40)]
    [Unicode(false)]
    public string Title { get; set; }

    [InverseProperty("Breed")]
    public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();

    [ForeignKey("TypeId")]
    [InverseProperty("PetBreeds")]
    public virtual PetType Type { get; set; }
}
