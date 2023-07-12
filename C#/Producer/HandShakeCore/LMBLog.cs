using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeCore
{
    /// <summary>
    /// https://confluence.internetbrands.com/pages/viewpage.action?pageId=75553587
    /// </summary>
    public class LMBLog
    {
        //** -> Mandatory

        //*Unique Guid Example: stg-lgl-web1.internetbrands.com5eab5327c1995
        public string message_guid { get; set; } 
        //message_payload
        public string message_payload { get; set; }
        //**Audit message Examples:
        //Required field value is missing",
        //"Processed Successfully", 
        //"Updated record in database with id 1234", 
        //"Could not find record in database" etc.,
        public string audit_log { get; set; }
        //consumer error message
        //if consumer raises exception,
        //you can pass the exception details / error message in this field
        public string error_message { get; set; }
        //Pass a boolean to indicate if the request is valid. For example, 
        //if email address is 1234 then the message is not valid and should be logged as false
        public bool is_valid_request { get; set; }
        //To indicate if the mapping is found or not.
        public bool is_mapping_found { get; set; }
        //This is in context with is_mapping_found field. 
        //If the mapping is found then what was the id of the mapping which was found
        public string bu_id { get; set; }
        //Name of the topic from where this message was processed. 
        public string topic_name { get; set; }
        //**Name of the kafka consumer. This is required. 
        public string consumer_name { get; set; }
        //**Immediate status available while processing the message from kafka consumer,
        //One status from this list [ Success, Failed].
        public string status { get; set; }

    }
}
