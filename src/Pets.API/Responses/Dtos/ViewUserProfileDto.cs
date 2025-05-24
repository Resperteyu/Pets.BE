using System.Collections.Generic;
using System;

namespace Pets.API.Responses.Dtos
{
    public class ViewUserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public List<PetProfileDto> Pets { get; set; }
    }
}
