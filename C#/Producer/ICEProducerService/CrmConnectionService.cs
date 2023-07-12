using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace ICEProducerService
{
    public class CrmConnectionService
    {
      
      
        public static string getConnString()
        {
            var strConnString = "";
            try
            {

                strConnString = "RequireNewInstance=True"
                   + "; Url=" + "URL"
                   + "; Domain=" + "DOMAIN"
                   + "; UserName=" + "USERNAME"
                   + "; Password=" + "PASSWORD"
                   + "; AuthType=IFD";
                return strConnString;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static CrmServiceClient getServiceClient()
        {
            try
            {
                CrmServiceClient crmSvc = new CrmServiceClient(getConnString());
                return crmSvc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}