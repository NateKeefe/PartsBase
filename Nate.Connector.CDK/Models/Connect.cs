using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDK.Models
{
    class ConnectInput
    {
        public string grant_type { get; set; } = "password";
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; } = "api";
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ConnectOutput
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}
