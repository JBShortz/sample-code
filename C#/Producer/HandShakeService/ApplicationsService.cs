using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandShakeCore;
using HandShakeData;

using NLog;


namespace HandShakeService
{
    public class ApplicationsService
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// To insert the logs in data base.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="shortMessage"></param>
        /// <param name="fullMessage"></param>
        /// <param name="CustomerId"></param>
        public void UpdateApplication(int applicationId, int timeIntervalInMinutes)
        {            
            try
            {               
                
                using (HandShakeDbContext dbContext = new HandShakeDbContext())
                {
                    Applications objApplication = dbContext.Applications.FirstOrDefault(x => x.Id == applicationId);
                    if (objApplication != null)
                    {
                        objApplication.LastRunOn = DateTime.Now;
                        objApplication.ModifiedOn = DateTime.Now;
                        var TimeSpan = new TimeSpan(0, timeIntervalInMinutes, 0);
                        var NextRunDate = DateTime.Now.Add(TimeSpan);
                        objApplication.NextRunOn = NextRunDate;
                    }

                    dbContext.Entry(objApplication).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
              
            }
            catch (DbEntityValidationException e)
            {
                string ExceptionMessage = "There is an exception occured in updating applications." + e.Message;
                _log.Fatal(ExceptionMessage);
                throw new DbEntityValidationException(ExceptionMessage, e.InnerException);
            }
        }
    }
}






