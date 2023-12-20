using Pets.Db.Models;
using System;
using System.Collections.Generic;

namespace Pets.API.Responses
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
