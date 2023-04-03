using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("PetType")]
public partial class PetType
{
    [Key]
    public byte Id { get; set; }

    [StringLength(10)]
    public string Name { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<PetBreed> PetBreeds { get; } = new List<PetBreed>();
}
