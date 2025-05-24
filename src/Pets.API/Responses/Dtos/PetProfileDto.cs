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

                int years = 0; 
                int months = 0;
                int days = 0;
                if (difference.Days > 30)
                {
                    months = (difference.Days % 365) / 30;
                    if (difference.Days > 365)
                        years = difference.Days / 365;
                }
                else
                    days = difference.Days;

                Age = new PetAge
                {
                    Years = years,
                    Months = months,
                    Days = days
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

        public bool ForSale { get; set; }
        public int Price { get; set; }
        public bool ForAdoption { get; set; }
        public bool Missing { get; set; }
        public bool Private { get; set; }

        public class PetAge
        {
            public int Years { get; set; }
            public int Months { get; set; }
            public int Days { get; set; }
        }
    }
}
