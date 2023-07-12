using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeCore
{
    public class LMBConfig
    {        
        public string BootstrapServer { get; set; }      
        public string Topic { get; set; }
        public string Topic_2 { get; set; }
        public string GroupId { get; set; }       
        public string SchemaRegistryUrl { get; set; }        
        public string PublishMessageURL { get; set; }
        public string PublishMessageStagingURL { get; set; }
        public string OrganizationId { get; set; }
        public int SchemaId { get; set; }
        public int SchemaId_2 { get; set; }
    }
}
