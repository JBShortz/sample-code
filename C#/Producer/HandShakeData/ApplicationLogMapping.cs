using HandShakeCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandShakeData
{
    public class ApplicationLogMap : EntityTypeConfiguration<ApplicationLog>
    {
        public ApplicationLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);


            this.Ignore(t => t.LogLevel);

            this.Property(t => t.ShortMessage);
            this.Property(t => t.FullMessage);
            this.Property(t => t.IpAddress);
            this.Property(t => t.PageUrl);
            this.Property(t => t.ApplicationId);
            this.Property(t => t.CreatedOn);
            this.Property(t => t.LogLevelId);
            this.Property(t => t.CaptorraId);
            this.Property(t => t.ApplicationId);

            #region Table & Column Mapping
            this.ToTable("ApplicationLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ShortMessage).HasColumnName("ShortMessage");
            this.Property(t => t.FullMessage).HasColumnName("FullMessage");
            this.Property(t => t.IpAddress).HasColumnName("IpAddress");
            this.Property(t => t.PageUrl).HasColumnName("PageUrl");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.LogLevelId).HasColumnName("LogLevelId");
            this.Property(t => t.CaptorraId).HasColumnName("CaptorraId");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            #endregion
        }
    }
}
