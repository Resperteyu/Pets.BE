using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movrr.API;

[Table("Country")]
public partial class Country
{
    [Key]
    [StringLength(2)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Name { get; set; }

    [Required]
    [StringLength(4)]
    [Unicode(false)]
    public string DialCode { get; set; }

    [InverseProperty("CountryCodeNavigation")]
    public virtual ICollection<Profile> Profiles { get; } = new List<Profile>();
}
