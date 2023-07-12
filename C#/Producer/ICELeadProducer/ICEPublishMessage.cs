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
using Newtonsoft.Json;

namespace ICELeadProducer
{
    public class ICEPublishMessage
    {
        public string PublishMessage(ICELeadNew _ICELeadNew, LMBConfig _LMBConfig, string reqId, bool _blnLog)
        {
            var response = "";
            try
            {
                HandShakeLogSvc objHandShakeLogSvc = new HandShakeLogSvc();

                using (HttpClient httpClient = new HttpClient())
                {
                    var payload = JsonConvert.SerializeObject(new { message = _ICELeadNew });

                    //Audit Log - Support Portal  - Payoad                 
                    if (_blnLog)
                        objHandShakeLogSvc.Log2API(reqId, "Payload: " + payload, _LMBConfig.OrganizationId, _LMBConfig.Topic);

                    var endPointUrl = _LMBConfig.PublishMessageURL + _LMBConfig.Topic + "/" + _LMBConfig.SchemaId;

                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //httpClient.DefaultRequestHeaders.Add("Authorization", "");                    
                    HttpResponseMessage httpResponse = new HttpResponseMessage();
                    httpResponse = httpClient.PostAsync(endPointUrl, content).Result;
                    response = httpResponse.Content.ReadAsStringAsync().Result;
                    //response = "{\"success\": true,\"data\": \"{\"messageGUID\":\"stg-lgl-web1.internetbrands.com5f35431272724\",\"source\":\"anonymous\"}\", \"message\": \"published successfully\", \"cacheable\": false}";
                    
                    //Audit Log - Support Portal  - Response
                    if (_blnLog && response != "")
                        objHandShakeLogSvc.Log2API(reqId, "Response: " + response, _LMBConfig.OrganizationId, _LMBConfig.PublishMessageURL);

                }
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
