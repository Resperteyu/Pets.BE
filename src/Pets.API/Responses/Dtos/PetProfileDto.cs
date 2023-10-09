using System;

namespace Pets.API.Responses.Dtos
{
    public class PetProfileDto
    {
        public Guid Id { get; set; }

        private DateTime _dateOfBirth;
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;

                // Calculating and setting pet age as we cannot rely on client date & time
                TimeSpan difference = DateTime.Now - value;

                Age = new PetAge
                {
                    Years = difference.Days / 365,
                    Months = (difference.Days % 365) / 30
                };
            }
        }

        public bool AvailableForBreeding { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public PetOwnerInfosDto Owner { get; set; }

        public SexDto Sex { get; set; }

        public PetBreedDto Breed { get; set; }

        public PetAge Age { get; private set; }

        public class PetAge
        {
            public int Years { get; set; }
            public int Months { get; set; }
        }
    }
}
