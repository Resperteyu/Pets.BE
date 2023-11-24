using System;

namespace Pets.API.Requests.MateRequest
{
    public class PetMateRequestResponseRequest
    {
        public Guid MateRequestId { get; set; }

        public byte MateRequestStateId { get; set; }

        public string Response { get; set; }
    }
}
