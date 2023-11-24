using System.ComponentModel.DataAnnotations;
using System;

namespace Pets.API.Requests.MateRequest
{
    public class PetMateRequestTransitionRequest
    {
        public Guid MateRequestId { get; set; }

        public byte MateRequestStateId { get; set; }

        public string Comment { get; set; }
    }
}
