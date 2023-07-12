using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HandShakeService;
using NLog;
using HandShakeCore;
using System.Configuration;
using System.IO;
using System.Reflection;
using NLog.Config;

namespace ICELeadProducer
{
    class Program
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        static CancellationTokenSource cts = new CancellationTokenSource();
        static async Task Main(string[] args)
        {
            try
            {
                var reqId = Guid.NewGuid().ToString();


                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (baseDir != "" && File.Exists(baseDir + "\\NLog.config"))
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(baseDir + "\\NLog.config", true);
                }
                var blnCapLog = ConfigurationManager.AppSettings["blnCapLog"];
                var isLog = false;
                if(blnCapLog != null && blnCapLog != "" && blnCapLog == "YES")
                {
                    isLog = true;
                }
                else if (blnCapLog != null && blnCapLog != "" && blnCapLog == "NO")
                {
                    isLog = false;
                }
                LMBConfig objLMBConfig = new LMBConfig();
                //Getting the Kafka Configuration
                //stg-lmb-kafka1.internetbrands.com:9092                
                objLMBConfig.BootstrapServer = ConfigurationManager.AppSettings["BootstrapServer"];
                //legalcrm-leads-message PublishMessageURL
                objLMBConfig.Topic = ConfigurationManager.AppSettings["Topic"];
                var Id = ConfigurationManager.AppSettings["SchemaId"];
                if (Id != "" && UtilitySvc.isValidInt(Id))
                {
                    objLMBConfig.SchemaId = Convert.ToInt32(Id);
                }
                objLMBConfig.SchemaRegistryUrl = ConfigurationManager.AppSettings["SchemaRegistryUrl"];
                objLMBConfig.PublishMessageURL = ConfigurationManager.AppSettings["PublishMessageURL"];
                //d3ea27d4-76b2-e911-a2ca-00505692291d - Rupinder
                objLMBConfig.OrganizationId = ConfigurationManager.AppSettings["OrganizationId"];
                ICEPublishMessage objICEPublishMessage = new ICEPublishMessage();
                GenerateRecord objGenerateRecord = new GenerateRecord();
                List<ICELeadNew> lstICELeadNew = new List<ICELeadNew>();
                lstICELeadNew = objGenerateRecord.getRecords();
                if(lstICELeadNew.Count > 0)
                {
                    foreach(var objICELeadNew in lstICELeadNew)
                    {
                        objICEPublishMessage.PublishMessage(objICELeadNew, objLMBConfig, reqId, isLog);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.Message);
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }


    }
}
