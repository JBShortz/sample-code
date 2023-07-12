using CAPFCQSCore;
using CAPFCQSService;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;

namespace CAPFCQS.Main
{
    class Program
    {
        static void Main(string[] args)
        {

            CustomerInfo _CustomerInfo1 = new CustomerInfo();
            _CustomerInfo1.CrmUrl = CRM_URL;
            _CustomerInfo1.CustomerId = CUSTOMER_ID;
            _CustomerInfo1.Domain = DOMAIN;
            _CustomerInfo1.OrganizationName = ORGANIZATION_NAME;
            _CustomerInfo1.UserName = USERNAME;
            _CustomerInfo1.Password = PASSWORD;

            var crmSvcClient1 = CrmConnectionService.getServiceClient(_CustomerInfo1);


            string fetchquery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='task'>
                                        <attribute name='subject' />
                                        <attribute name='statecode' />
                                        <attribute name='prioritycode' />
                                        <attribute name='scheduledend' />
                                        <attribute name='createdby' />
                                        <attribute name='regardingobjectid' />
                                        <attribute name='activityid' />
                                        <order attribute='subject' descending='false' />
                                        <filter type='and'> 
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='subject' operator='eq' value='FCQ Request' />  
                                        </filter>
                                      </entity>
                                    </fetch>";

            //<condition attribute='activityid' operator='eq' uiname='FCQ Request' uitype='task' value='{2d9a1595-f168-ec11-b824-00155d842e53}' />
            //<condition attribute='activityid' operator='eq' uiname='FCQ Request' uitype='task' value='{8be79ad7-fd68-ec11-b824-00155d842e53}' />

            EntityCollection taskEC = crmSvcClient1.RetrieveMultiple(new FetchExpression(fetchquery));

            if (taskEC.Entities.Count > 0)
            {
                foreach (var ent in taskEC.Entities)
                {
                    //Grab Documents
                    int cntDuplicate = 1;
                    string filename = "";
                    var Documents = new Dictionary<string, string>();

                    OrganizationServiceContext svcContext = new OrganizationServiceContext(crmSvcClient1);
                    var notes = from annotation in svcContext.CreateQuery("annotation")
                                where ((EntityReference)annotation["objectid"]).Id == ent.Id
                                select new
                                {
                                    id = annotation.Id,
                                    FileName = (annotation.Attributes.Contains("filename")) ? annotation.GetAttributeValue<string>("filename") : "",
                                    documentbody = (annotation.Attributes.Contains("documentbody")) ? annotation.GetAttributeValue<string>("documentbody") : ""
                                };
                    var count = notes.ToList().Count;
                    foreach (var _notes in notes)
                    {
                        if (_notes.FileName != "")
                        {
                            filename = _notes.FileName;
                            if (Documents.ContainsKey(filename))
                            {
                                filename = UtilityService.AppendSeparator(_notes.FileName, cntDuplicate.ToString(), "_");
                                cntDuplicate++;
                            }
                            Documents.Add(filename, _notes.documentbody);
                        }
                    }

                    var path = ConfigurationManager.AppSettings["filePath"];
                   

                    foreach (KeyValuePair<string, string> doc in Documents)
                    {
                        
                        System.IO.File.WriteAllBytes(path + doc.Key, Convert.FromBase64String(doc.Value));
                        
                    }

                  
                    //Mark Complete
                    //SetStateRequest req = new SetStateRequest();
                    //req.EntityMoniker = new EntityReference("task", ent.Id);
                    //req.State = new OptionSetValue(1); 
                    //req.Status = new OptionSetValue(5); //status Completed
                    //crmSvcClient1.Execute(req);
                }


            }



        }
    }
}
