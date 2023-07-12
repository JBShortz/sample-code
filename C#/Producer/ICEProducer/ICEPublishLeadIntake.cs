using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HandShakeCore;
using HandShakeService;
using ICELeadProducer;
using Newtonsoft.Json;
using NLog;

namespace ICEProducer
{
    public class ICEPublishLeadIntake
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private static ApplicationLogService _dbLog = new ApplicationLogService();
        
        public async Task<string> PublishMessage(ICELeadIntake _ICELeadIntake, LMBConfig _LMBConfig, string reqId, int topic)
        {
            var response = "";
            var endPointUrl = "";

            var blnEmailNotification = ConfigurationManager.AppSettings["blnEmailNotification"];
            var isEmailNotification = false;
            if (blnEmailNotification != null && blnEmailNotification != "" && blnEmailNotification == "YES")
            {
                isEmailNotification = true;
            }
            else if (blnEmailNotification != null && blnEmailNotification != "" && blnEmailNotification == "NO")
            {
                isEmailNotification = false;
            }

            try
            {
                await Task.Run(() =>
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        _log.Info("Publishing LeadIntake {0} {1}", _ICELeadIntake.firstname, _ICELeadIntake.lastname);
                        var payload = JsonConvert.SerializeObject(new { message = _ICELeadIntake });
                       
                        _log.Info("Payload LeadIntake {0}: ", payload);
                        _dbLog.InsertLog(LogLevels.Information, reqId, "Payload LeadIntake: " + payload);                         
                        
                        endPointUrl = _LMBConfig.PublishMessageURL + _LMBConfig.Topic + "/" + _LMBConfig.SchemaId;
                        _log.Info("Endpoint {0}", endPointUrl);
                        HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        content.Headers.Add("source", "ICE-CRM");
                      

                        HttpResponseMessage httpResponse = new HttpResponseMessage();
                        try
                        {
                            httpResponse = httpClient.PostAsync(endPointUrl, content).Result;
                            response = httpResponse.Content.ReadAsStringAsync().Result;
                           
                        }
                        catch (Exception ex)
                        {
                            _dbLog.InsertLog(LogLevels.Fatal, reqId, "Exception: " + ex.Message + Environment.NewLine + "InnerException: " + ex.InnerException.Message);
                            if (isEmailNotification)
                            {
                                var EmailBody = "ICE Producer Failed";
                                EmailBody = EmailBody + Environment.NewLine;
                                EmailBody = EmailBody + "Payload LeadIntake: " + payload;
                                EmailBody = EmailBody + Environment.NewLine;
                                EmailBody = EmailBody + "Exception: " + ex.Message;
                                EmailBody = EmailBody + Environment.NewLine;
                                EmailBody = EmailBody + "InnerException: " + ex.InnerException.Message;
                                EmailService _EmailService = new EmailService();
                                _EmailService.SendGMail(EmailBody);
                            }
                        }

                        if (response != null && response != "")
                            _dbLog.InsertLog(LogLevels.Information, reqId, "Response: " + response);
                        if (response != null && response != "" && response.Contains("success"))
                        {
                            _log.Info("Published LeadIntake Successfully Response: {0}", response);
                            GenerateLeadIntake objGenerateLeadIntake = new GenerateLeadIntake();
                            objGenerateLeadIntake.CreateJSonPostingUpdateIntake(_ICELeadIntake.contactid, payload, response, _ICELeadIntake.firstname, _ICELeadIntake.lastname);
                        }
                        else if (response != null && response != "" && !response.Contains("success"))//Did not get a success response from kafka. Flip the flags & write the response
                        {
                            _log.Error("Did not get publish Response: {0}", response);
                            _dbLog.InsertLog(LogLevels.Error, reqId, "Did not get publish Response: " + response);
                            GenerateLeadIntake objGenerateLeadIntake = new GenerateLeadIntake();
                            objGenerateLeadIntake.CreateJSonPostingUpdateIntake(_ICELeadIntake.contactid, payload, response, _ICELeadIntake.firstname, _ICELeadIntake.lastname);
                        }
                        else if (response == null || response == "") //Did not get any response from kafka.
                        {
                            //No response from Kafka        
                            _log.Error("No response from Kafka for payload: {0}", payload);
                            _dbLog.InsertLog(LogLevels.Error, reqId, "No response from Kafka for payload: " + payload);
                        }
                    }
                });
                return response;
            }
            catch(Exception ex)
            {
                _dbLog.InsertLog(LogLevels.Fatal, reqId, "Exception: " + ex.Message + Environment.NewLine + "InnerException: " + ex.InnerException.Message);
                if (isEmailNotification)
                {                   
                    var EmailBody = "ICE Producer Failed";
                    EmailBody = EmailBody + Environment.NewLine;
                    EmailBody = EmailBody + "Exception: " + ex.Message;
                    EmailBody = EmailBody + Environment.NewLine;
                    EmailBody = EmailBody + "InnerException: " + ex.InnerException.Message;
                    EmailService _EmailService = new EmailService();
                    _EmailService.SendGMail(EmailBody);
                }
                _log.Fatal(ex.Message + Environment.NewLine + ex.InnerException.Message);
                throw ex;
            }
        }
    }
}
