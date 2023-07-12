
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using NLog;
using System.Configuration;
using HandShakeCore;

namespace HandShakeService
{
    public class HandShakeLogSvc
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ShortMessage"></param>
        /// <param name="FullMessage"></param>
        /// <param name="OrganizationId"></param>
        public void Log2API(string reqId, string FullMessage, string OrganizationId, string Url)
        {
            try
            {
                var AppId = ConfigurationManager.AppSettings["ApplicationId"];
                var ApplicationId = -1;
                if(AppId != null && AppId != "")
                {
                    try
                    {
                        ApplicationId = Convert.ToInt32(AppId);
                    }
                    catch(Exception)
                    {
                        ApplicationId = 40; //HandShakeAPI
                    }
                }
                else
                {
                    ApplicationId = 40; //HandShakeAPI
                }
                var HandShakeLogAPIUrl = ConfigurationManager.AppSettings["HandShakeLogAPIUrl"];
                if(HandShakeLogAPIUrl != null && HandShakeLogAPIUrl != "" && UtilitySvc.IsValidUri(HandShakeLogAPIUrl))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        ApplicationLog objApplicationLog = new ApplicationLog();
                        objApplicationLog.ApplicationId = ApplicationId; 
                        objApplicationLog.OrganizationId = OrganizationId;
                        objApplicationLog.ShortMessage = reqId;
                        objApplicationLog.FullMessage = FullMessage;
                        objApplicationLog.IpAddress = Dns.GetHostName();
                        objApplicationLog.PageUrl = Url;
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ApplicationLog));
                        MemoryStream memoryStream = new MemoryStream();

                        serializer.WriteObject(memoryStream, objApplicationLog);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                        content.Headers.Add("APIKEY", OrganizationId);
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        try
                        {
                            HttpResponseMessage httpResponse = client.PostAsync(HandShakeLogAPIUrl, content).Result;
                            //var response = httpResponse.Content.ReadAsStringAsync().Result;                       
                        }
                        catch (Exception ex)
                        {
                            _log.Fatal(ex.Message);
                            throw ex;
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                _log.Fatal("Exception: {0} InnerException: {1}", ex.Message, ex.InnerException.Message);
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLegalCRM"></param>
        /// <param name="APIKEY"></param>
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLegalCRM"></param>
        /// <param name="APIKEY"></param>
        public string AuditLog(LMBLog objLMBLog)
        {
            var result = "";
            try
            {                
                if (objLMBLog == null)
                {
                    result = "No Info. Empty Payload.";
                    _log.Error(result);
                    return result;
                }
                else
                {
                    var AuditLogAPIUrl = ConfigurationManager.AppSettings["AuditLogAPIUrl"];
                    _log.Info("Calling AuditLogAPIUrl: {0}", AuditLogAPIUrl);

                    if (AuditLogAPIUrl != null && AuditLogAPIUrl != "" && UtilitySvc.IsValidUri(AuditLogAPIUrl))
                    {
                        using (HttpClient client = new HttpClient())
                        {

                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LMBLog));
                            MemoryStream memoryStream = new MemoryStream();

                            serializer.WriteObject(memoryStream, objLMBLog);

                            var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");                            
                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            try
                            {
                                HttpResponseMessage httpResponse = client.PostAsync(AuditLogAPIUrl, content).Result;
                                var response = httpResponse.Content.ReadAsStringAsync().Result;
                                _log.Info("Response: {0}", response);
                                result = response;
                            }
                            catch (Exception ex)
                            {
                                _log.Fatal("Exception: {0} InnerException: {1}", ex.Message, ex.InnerException.Message);
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        result = "AuditLogAPIUrl not found.";
                        _log.Error(result);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Fatal("Exception: {0} InnerException: {1}", ex.Message, ex.InnerException.Message);
                throw ex;
            }
        }
    }
}
