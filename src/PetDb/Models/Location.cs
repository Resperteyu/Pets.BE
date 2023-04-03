using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("Location")]
public partial class Location
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Latitude { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal Longitude { get; set; }

    [InverseProperty("Location")]
    public virtual ICollection<Profile> Profiles { get; } = new List<Profile>();
}
