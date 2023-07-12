using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeCore
{
    public class ApplicationLog
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int LogLevelId { get; set; }
        public int CaptorraId { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }        
        public string IpAddress { get; set; }
        public string PageUrl { get; set; }      

        public DateTime CreatedOn { get; set; }
        public LogLevels LogLevel
        {
            get
            {
                return (LogLevels)this.LogLevelId;
            }
            set
            {
                this.LogLevelId = (int)value;
            }
        }

        [NotMapped]
        public string OrganizationId { get; set; }

    }
}
