using System;

namespace Pets.API.Requests.MateRequest
{
    public class PetMateRequestReplyRequest
    {
        public Guid MateRequestId { get; set; }

        public byte MateRequestStateId { get; set; }

        public string Response { get; set; }
    }
}
