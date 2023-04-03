using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("Sex")]
public partial class Sex
{
    [Key]
    public byte Id { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Title { get; set; }

    [InverseProperty("Sex")]
    public virtual ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
}
