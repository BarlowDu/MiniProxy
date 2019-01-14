using HttpClientProxy.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientProxy.Attr
{
    public class ResquestMappingAttribute:Attribute
    {
        public string Accept { get; set; } = "application/json";
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";    
        public HttpMethod Method { get; set; }

        public string Path { get; set; }

        public int TimeOut { get; set; } = -1;
    


    }
}
