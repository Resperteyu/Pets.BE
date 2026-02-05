using System;
using System.Collections.Generic;

namespace Pets.API.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
} 