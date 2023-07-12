using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandShakeData;
using HandShakeCore;

using NLog;


namespace HandShakeService

{
    public class ApplicationLogService
    {

        private static Logger _log = LogManager.GetCurrentClassLogger();
        private int _appId = 54; //ICEProducer
        private string _pageURL = "ICE Producer";
        private int _CaptorraId = 0;
        /// <summary>
        /// To insert the logs in data base.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="shortMessage"></param>
        /// <param name="fullMessage"></param>
        /// <param name="CustomerId"></param>
        public void InsertLog(LogLevels logLevel, string reqId, string fullMessage = "")
        {
            HandShakeDbContext dbContext = new HandShakeDbContext();
            try
            {
                
                var log = new ApplicationLog
                {
                    LogLevel = logLevel,
                    ShortMessage = reqId,                    
                    ApplicationId = _appId,
                    CaptorraId = _CaptorraId,
                    FullMessage = fullMessage,
                    IpAddress = "IPADDRESS",
                    PageUrl = _pageURL,
                    CreatedOn = DateTime.Now.ToLocalTime()
                };
                dbContext.ApplicationLog.Add(log);
                dbContext.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {
                string ExceptionMessage = "There is an exception occured in writting DB logs." + e.Message;
                _log.Error(ExceptionMessage);
                throw new DbEntityValidationException(ExceptionMessage, e.InnerException);
            }
        }

        


       
    }
}
