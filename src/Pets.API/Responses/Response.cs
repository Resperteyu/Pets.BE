using System.Collections.Generic;

namespace Pets.API.Responses
{
    public class Response
    {
        public string? Status { get; set; }
        public List<string> Message { get; set; }
    }
}
