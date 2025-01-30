using Learntendo_backend.Models;  

namespace Learntendo_backend.configurations
{
  
        public class JwtSettings
        {
            public string SecretKey { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
        }
    

}
