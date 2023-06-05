using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics;

namespace SendGrid
{
  
    public class To
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class Personalization
    {
        public List<To> to { get; set; }
    }

    public class From
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class CustomArgs
    {
        public string captorraId { get; set; }
        public Guid emailId { get; set; }
    }

    public class SendGridEmail
    {
        public List<Personalization> personalizations { get; set; }
        public From from { get; set; }
        public string subject { get; set; }
        public List<Content> content { get; set; }

        public CustomArgs custom_args { get; set; }
    }
    
    public class SendGrid : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var debug = true;

            var emailFrom = "";
            var emailFromName = "";
            var emailTo = "";
            var emailToName = "";
            var emailSubject = "";
            var emailContent = "";
            var statuscode = -1;
            var emailId = Guid.Empty;

            try
            {

                IPluginExecutionContext context =
            (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));



                if (debug)
                {
                    tracingService.Trace("Plugin Fired at: {0}", DateTime.Now);
                    tracingService.Trace("context.Depth: {0}", context.Depth);
                    //tracingService.Trace("context.UserId: {0}", context.UserId);
                    //tracingService.Trace("context.InputParameters: {0}", context.InputParameters["EmailId"]);
                    //tracingService.Trace("context.InputParameters: {0}", context.InputParameters["TemplateId"]);
                  
                }

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    emailId = entity.Id;
                    if (entity.LogicalName != "email")
                        return;
                    
                }
                else
                {
                    //tracingService.Trace("No Target");
                    return;
                }

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                OrganizationServiceContext orgSvcContext = new OrganizationServiceContext(service);

               

                var emails = from email in orgSvcContext.CreateQuery("email")
                             where (Guid)email["activityid"] == emailId
                             select new
                            {
                                recipient = email.Attributes.ContainsKey("torecipients") ? email.GetAttributeValue<string>("torecipients") : "",
                                sender = email.Attributes.ContainsKey("sender") ? email.GetAttributeValue<string>("sender") : "",
                                subject = email.Attributes.ContainsKey("subject") ? email.GetAttributeValue<string>("subject") : "",
                                description = email.Attributes.ContainsKey("description") ? email.GetAttributeValue<string>("description") : "",
                                statuscode = email.Attributes.ContainsKey("statuscode") ? email.GetAttributeValue<OptionSetValue>("statuscode").Value : -1,
                                sendermailboxid = email.Attributes.ContainsKey("sendermailboxid") ? email.GetAttributeValue<EntityReference>("sendermailboxid").Name : "",
                                regardingobjectidname = email.Attributes.ContainsKey("regardingobjectid") ? email.GetAttributeValue<EntityReference>("regardingobjectid").Name : "",

                             };

               
                foreach (var _email in emails)
                {

                    emailFrom = _email.sender;
                    emailFromName = _email.sendermailboxid;
                    emailTo = _email.recipient.TrimEnd(';');
                    emailToName = _email.regardingobjectidname;
                    emailSubject = _email.subject;
                    emailContent = _email.description;                    
                    statuscode = _email.statuscode;

                }

                if (statuscode != 6)
                {
                    //tracingService.Trace("Not Pending Sent");
                    return;
                }

                
                //tracingService.Trace("Email Status: {0}, ID: {1}, To: {2}, From: {3}, FromName: {6}, Subject: {4}, Content: {5}", statuscode, emailId, emailTo, emailFrom, emailSubject, emailContent, emailFromName);

                string response = "";
                var httpstatuscode = "";
                using (HttpClient client = new HttpClient())
                {
                    var SENDGRID = new SendGridEmail();
                    var FROM = new From { email = emailFrom, name = emailFromName };
                    SENDGRID.from = FROM;
                    SENDGRID.subject = emailSubject;

                    string[] emailList = emailTo.Split(';');

                    List<To> toList = new List<To>();

                    foreach (var email in emailList)
                    {
                        toList.Add(new To { email = email, name = email, });

                    }


                    var PERSONALIZATION = new Personalization();
                    PERSONALIZATION.to = toList;
                    List<Personalization> persList = new List<Personalization>() { PERSONALIZATION };

                    SENDGRID.personalizations = persList;

                    var CONTENT = new Content { type = "text/html", value = emailContent };
                    List<Content> contList = new List<Content>() { CONTENT };
                    SENDGRID.content = contList;

                    var CUSTOM_ARGS = new CustomArgs { captorraId = "683972", emailId = emailId };
                    SENDGRID.custom_args = CUSTOM_ARGS;

                    //tracingService.Trace("emailContent: {0}", emailContent);
                    //tracingService.Trace("SENDGRID.content: {0}", SENDGRID.content);

                    string API_KEY = "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";
                    //string API_KEY = "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";

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SendGridEmail));
                    MemoryStream memoryStream = new MemoryStream();
                    serializer.WriteObject(memoryStream, SENDGRID);
                    var jsonObject = Encoding.UTF8.GetString(memoryStream.ToArray());

                    //tracingService.Trace("jsonObject: {0}", jsonObject);

                    var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
                    HttpResponseMessage httpResponse = client.PostAsync("https://api.sendgrid.com/v3/mail/send", content).Result;
                    response = httpResponse.Content.ReadAsStringAsync().Result;

                    tracingService.Trace("httpResponse: {0}", httpResponse);
                    //tracingService.Trace("StatusCode: {0}", httpResponse.StatusCode);
                    httpstatuscode = httpResponse.StatusCode.ToString();
                    //tracingService.Trace("response: {0}", response);
                }


                if (httpstatuscode == "Accepted")
                {

                    //Accepted - successfully posted to SendGrid endpoint - 200OK
                    //SendGridHandler will update record further based on webhook response.

                    SetStateRequest closeRequest = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference("email", emailId),
                        State = new OptionSetValue(1), //Completed
                        Status = new OptionSetValue(3) //Sent
                    };

                    SetStateResponse stateSet = (SetStateResponse)service.Execute(closeRequest);

                }
                else
                {
                    //Failed to post to endpoint - typically issue with posting structure or auth creds                    

                    SetStateRequest closeRequest = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference("email", emailId),
                        State = new OptionSetValue(0), //Open
                        Status = new OptionSetValue(8) //Failed
                    };

                    SetStateResponse stateSet = (SetStateResponse)service.Execute(closeRequest);


                }

            }
            catch (Exception ex)
            {

                if (debug)
                {

                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();

                    tracingService.Trace("Logging Exception: {0}, Line: {1}", ex.Message, line);
                    tracingService.Trace("Logging Inner Exception: {0}", ex.InnerException);
                }
                throw new InvalidPluginExecutionException(ex.Message);

            }


        }

    }
}