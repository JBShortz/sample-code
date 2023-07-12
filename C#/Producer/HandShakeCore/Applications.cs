using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeCore
{
    public class Applications
    {
        public int Id { get; set; }        
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public bool Active { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<DateTime> LastRunOn { get; set; }
        public Nullable<DateTime> NextRunOn { get; set; }
    }
}
