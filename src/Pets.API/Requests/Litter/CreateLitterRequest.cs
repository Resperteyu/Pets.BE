using System;

namespace Pets.API.Requests.Litter
{
    public class CreateLitterRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int BreedId { get; set; }

        public Guid MotherPetId { get; set; }

        public Guid FatherPetId { get; set; }

        public bool Available { get; set; }

        public int Size { get; set; }

    }
}
