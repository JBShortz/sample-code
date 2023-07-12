using Microsoft.Xrm.Tooling.Connector;
using System;
using NLog;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace ICEProducerService
{
    public class CrmOperationService
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();                     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="crmSvc"></param>
        /// <returns></returns>
        public Guid createRecord(Entity entity, CrmServiceClient crmSvc)
        {
            var entityId = Guid.Empty;

            try
            {
                entityId = crmSvc.Create(entity);
                return entityId;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.Message);
                throw ex;
            }

        }
        /// <summary>
        /// To delete a record.
        /// </summary>
        /// <param name="EntityName"></param>
        /// <param name="recordId"></param>
        /// <param name="crmSvc"></param>
        public void DeleteEntityRecord(string EntityName, Guid recordId, CrmServiceClient crmSvc)
        {
            try
            {
                if (crmSvc != null && crmSvc.IsReady)
                {
                    crmSvc.DeleteEntity(EntityName, recordId);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="crmSvc"></param>
        /// <returns></returns>
        public  bool updateEntity(Entity entity, CrmServiceClient crmSvc)
        {
            try
            {
                crmSvc.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crmEntity"></param>
        /// <param name="crmField"></param>
        /// <param name="crmSvc"></param>
        /// <returns></returns>
        public DateTimeBehavior getDateBehaviour(string LogicalEntity, string LogicalAttribute, CrmServiceClient crmSvc)
        {
            //var bhr = "";
            try
            {
                RetrieveAttributeRequest retrieveAttributeRequest = new
            RetrieveAttributeRequest
                {
                    EntityLogicalName = LogicalEntity,
                    LogicalName = LogicalAttribute,
                    RetrieveAsIfPublished = true
                };
                // Execute the request.
                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)crmSvc.Execute(retrieveAttributeRequest);
                DateTimeAttributeMetadata retrievedAttributeMetadata =
                (DateTimeAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
                return retrievedAttributeMetadata.DateTimeBehavior;
                //return bhr;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return "";
            }
          
        }

    }
}
