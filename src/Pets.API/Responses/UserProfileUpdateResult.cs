using Microsoft.AspNetCore.Identity;
using Pets.API.Responses.Dtos;
using System.Collections.Generic;

namespace Pets.API.Responses
{
    public class UserProfileUpdateResult
    {
        public bool Success { get; set; }
        public UserProfileDto UpdatedProfile { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
