using HandShakeCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandShakeData
{
    public class ApplicationsMap : EntityTypeConfiguration<Applications>
    {
        public ApplicationsMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);


            this.Property(t => t.ApplicationName);
            this.Property(t => t.ApplicationDescription);            
            this.Property(t => t.Active);
            this.Property(t => t.CreatedOn);
            this.Property(t => t.ModifiedOn);
            this.Property(t => t.LastRunOn);
            this.Property(t => t.NextRunOn);


            #region Table & Column Mapping
            this.ToTable("Applications");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ApplicationName).HasColumnName("ApplicationName");
            this.Property(t => t.ApplicationDescription).HasColumnName("ApplicationDescription");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            this.Property(t => t.LastRunOn).HasColumnName("LastRunOn");
            this.Property(t => t.NextRunOn).HasColumnName("NextRunOn");
            #endregion
        }
    }
}
