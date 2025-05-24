﻿namespace Pets.Db.Models
{

    public class Sex
    {
        public byte Id { get; set; }

        public string Title { get; set; }

        public ICollection<PetProfile> PetProfiles { get; } = new List<PetProfile>();
    }
}
