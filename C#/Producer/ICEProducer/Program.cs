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
using ICELeadProducer;
using Microsoft.Xrm.Sdk;
using ICEProducerService;

namespace ICEProducer
{
    class Program
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        static CancellationTokenSource cts = new CancellationTokenSource();
        private static int ApplicationId = 54; //ICEProducer
        private static int TimeInterval = 5; //In Minutes
        private static ApplicationLogService _dbLog = new ApplicationLogService();
        static async Task Main(string[] args)
        {
            var reqId = Guid.NewGuid().ToString();
            //try
            //{
                _log.Info("Process Start at: {0}", DateTime.Now);
                _log.Info("Updating Applications for LastRunOn & NextRunOn.");
                ApplicationsService objApplicationsService = new ApplicationsService();
                objApplicationsService.UpdateApplication(ApplicationId, TimeInterval);
                _log.Info("Applications updated for LastRunOn & NextRunOn.");

                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (baseDir != "" && File.Exists(baseDir + "\\NLog.config"))
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(baseDir + "\\NLog.config", true);
                }
             
                LMBConfig objLMBConfig = new LMBConfig();
                //Getting the Kafka Configuration                           
                objLMBConfig.BootstrapServer = ConfigurationManager.AppSettings["BootstrapServer"];               
                objLMBConfig.Topic = ConfigurationManager.AppSettings["Topic"];
                objLMBConfig.Topic_2 = ConfigurationManager.AppSettings["Topic_2"];
                var Id = ConfigurationManager.AppSettings["SchemaId"];
                var Id_2 = ConfigurationManager.AppSettings["SchemaId_2"];

                if (Id != "" && UtilitySvc.isValidInt(Id))
                {
                    objLMBConfig.SchemaId = Convert.ToInt32(Id);
                }
                if (Id_2 != "" && UtilitySvc.isValidInt(Id_2))
                {
                    objLMBConfig.SchemaId_2 = Convert.ToInt32(Id_2);
                }

                objLMBConfig.SchemaRegistryUrl = ConfigurationManager.AppSettings["SchemaRegistryUrl"];
                objLMBConfig.PublishMessageURL = ConfigurationManager.AppSettings["PublishMessageURL"];
                objLMBConfig.PublishMessageStagingURL = ConfigurationManager.AppSettings["PublishMessageStagingURL"];
                objLMBConfig.OrganizationId = ConfigurationManager.AppSettings["OrganizationId"];

                GenerateLeadIntake objGenerateLeadIntake = new GenerateLeadIntake();
                List<ICELeadIntake> lstICELeadIntake = new List<ICELeadIntake>();
                lstICELeadIntake = objGenerateLeadIntake.getRecords(reqId);

                ICEPublishLeadIntake objICEPublishLeadIntake = new ICEPublishLeadIntake();
                if (lstICELeadIntake != null && lstICELeadIntake.Count > 0)
                {
                    var topic = 1;
                    foreach (var objICELeadIntake in lstICELeadIntake)
                    {
                        //Console.WriteLine(objICELeadIntake.contactid);
                        //Console.ReadLine();
                        var response = await objICEPublishLeadIntake.PublishMessage(objICELeadIntake, objLMBConfig, reqId, topic);
                    }
                }

                GenerateWarmTransfer objGenerateWarmTransfer = new GenerateWarmTransfer();
                List<ICEWarmTransfer> lstICEWarmTransfer = new List<ICEWarmTransfer>();
                lstICEWarmTransfer = objGenerateWarmTransfer.getRecords(reqId);

                ICEPublishWarmTransfer objICEPublishWarmTransfer = new ICEPublishWarmTransfer();
                if (lstICEWarmTransfer != null && lstICEWarmTransfer.Count > 0)
                {
                    var topic = 2;
                    foreach (var objICEWarmTransfer in lstICEWarmTransfer)
                    {
                        //Console.WriteLine(objICEWarmTransfer.contactid);
                        //Console.ReadLine();
                        var response = await objICEPublishWarmTransfer.PublishMessage(objICEWarmTransfer, objLMBConfig, reqId, topic);

                        //// Update the Intake ice_lmbupdate = false                
                        var crmSvcClient = CrmConnectionService.getServiceClient();
                        var contactId = Guid.Empty;
                        Entity intake = null;
                        intake = new Entity("contact");
                        intake.Id = objICEWarmTransfer.contactid;
                        intake["ice_lmbupdate"] = false;
                        CrmOperationService objCrmOperationsService = new CrmOperationService();
                        objCrmOperationsService.updateEntity(intake, crmSvcClient);
                        _log.Info("{0} updated with ice_lmbupdate = No successfully in ICE.", objICEWarmTransfer.firstname + " " + objICEWarmTransfer.lastname);
                    }
                }


                _log.Info("Process End at: {0}", DateTime.Now);



            //}
            //catch (Exception ex)
            //{
            //    _dbLog.InsertLog(LogLevels.Fatal, reqId, "Exception: " + ex.Message + Environment.NewLine + "InnerException: " + ex.InnerException.Message);
            //    _log.Fatal(ex.Message);
            //    Console.WriteLine(ex.ToString());
            //    throw ex;
            //}
        }


    }
}
