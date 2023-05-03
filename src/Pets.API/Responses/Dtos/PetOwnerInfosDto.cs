using System;

namespace Pets.API.Responses.Dtos
{
    public class PetOwnerInfosDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
