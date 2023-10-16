using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Request
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public string SenderUsername { get; set; }
        public string Uri { get; set; }

        public Dictionary<string, Object> Header = new Dictionary<string, Object>()
        {
            {"Accept", null },
            {"Accept_Language", null  },
            { "Timeout" , null },
            {"Authentication" , String.Empty }
        };

        public Dictionary<string, object> Content = new Dictionary<string, object>();

       

        public void SetAuthentication(String authentication)
        {
            Header["Authentication"] = authentication;
        }
        public void AddContent(String Key, Object Value)
        {
            Content.Add(Key, Value);

        }
    }
}
