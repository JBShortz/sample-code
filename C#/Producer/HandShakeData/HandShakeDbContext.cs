using HandShakeCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeData
{
    public class HandShakeDbContext : DbContext
    {
        /// <summary>
        /// Not to intialize the DB
        /// </summary>
        static HandShakeDbContext()
        {
            Database.SetInitializer<HandShakeDbContext>(null);
        }
        /// <summary>
        /// Default Connection String
        /// </summary>
        public HandShakeDbContext() : base("Name=HandShakeConnectionString")
        {
        }
        /// <summary>
        /// DbSets
        /// </summary>
        
        public DbSet<ApplicationLog> ApplicationLog { get; set; }        
        public DbSet<Applications> Applications { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();            
            modelBuilder.Configurations.Add(new ApplicationLogMap());            
            modelBuilder.Configurations.Add(new ApplicationsMap());
        }
    }
}
