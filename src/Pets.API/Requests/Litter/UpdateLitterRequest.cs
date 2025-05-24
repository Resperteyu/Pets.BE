using System;

namespace Pets.API.Requests.Litter
{
    public class UpdateLitterRequest
    {
        public Guid Id { get; set; }

        public bool Available { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }       
    }
}
