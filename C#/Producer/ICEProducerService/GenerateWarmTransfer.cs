using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandShakeCore;
using HandShakeService;
using ICEProducerService;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using NLog;

namespace ICELeadProducer
{
    public class GenerateWarmTransfer
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private static DateTime _defaultDate = DateTime.Parse("1/1/1990").Date;
        private static ApplicationLogService _dbLog = new ApplicationLogService();
        public List<ICEWarmTransfer> getRecords(string ReqId)
        {

            string[] intakeStatus = new string[] { "ActiveLead", "Expired", "BadNumber", "NoLongerInterested", "AlreadyHasAttorney", "Invalid", "Duplicate", "InternalReview", "SignUpPackage", "Retained", "Disqualified", "ClientRejectedUs", "NoLongerPursuing_Case", "NoResponse", "SelectedAnotherAttorney" };
            List<ICEWarmTransfer> lstICEWarmTransfer = new List<ICEWarmTransfer>();
            try
            {

                var crmSvcClient = CrmConnectionService.getServiceClient();

                if (crmSvcClient != null && crmSvcClient.LastCrmError != "")
                {
                    _log.Error("Last CRM Error {0} ", crmSvcClient.LastCrmError);
                    _dbLog.InsertLog(LogLevels.Fatal, ReqId, "Last CRM Error  " + crmSvcClient.LastCrmError);
                    return lstICEWarmTransfer;
                }
                else
                {
                    _log.Info("CRM Connection successfull.");
                }

                using (OrganizationServiceContext orgSvcContext = new OrganizationServiceContext(crmSvcClient))
                {
                    _log.Info("Getting leads to be published from CRM");
                    var Leads = from contact in orgSvcContext.CreateQuery("contact")
                                where ((bool)contact["ice_lmbupdate"]) == true
                                && ((OptionSetValue)contact["statecode"]).Value == 0
                               
                                select new
                                {
                                    contactid = contact.Attributes.ContainsKey("contactid") ? (Guid)contact.Attributes["contactid"] : Guid.Empty,
                                    parentcustomerid = contact.Attributes.ContainsKey("parentcustomerid") ? ((EntityReference)contact.Attributes["parentcustomerid"]).Name : "",
                                    ice_language = contact.Attributes.ContainsKey("ice_language") ? contact.FormattedValues["ice_language"].ToString() : "",
                                    firstname = contact.Attributes.ContainsKey("firstname") ? (string)contact.Attributes["firstname"] : "",
                                    lastname = contact.Attributes.ContainsKey("lastname") ? (string)contact.Attributes["lastname"] : "",
                                    emailaddress1 = contact.Attributes.ContainsKey("emailaddress1") ? (string)contact.Attributes["emailaddress1"] : "",
                                    mobilephone = contact.Attributes.ContainsKey("mobilephone") ? (string)contact.Attributes["mobilephone"] : "",
                                    address1_telephone1 = contact.Attributes.ContainsKey("address1_telephone1") ? (string)contact.Attributes["address1_telephone1"] : null,
                                    address1_line1 = contact.Attributes.ContainsKey("address1_line1") ? (string)contact.Attributes["address1_line1"] : "",
                                    address1_city = contact.Attributes.ContainsKey("address1_city") ? (string)contact.Attributes["address1_city"] : "",
                                    address1_stateorprovince = contact.Attributes.ContainsKey("address1_stateorprovince") ? (string)contact.Attributes["address1_stateorprovince"] : "",
                                    address1_postalcode = contact.Attributes.ContainsKey("address1_postalcode") ? (string)contact.Attributes["address1_postalcode"] : "",
                                    address1_county = contact.Attributes.ContainsKey("address1_county") ? (string)contact.Attributes["address1_county"] : null,
                                    address1_country = contact.Attributes.ContainsKey("address1_country") ? (string)contact.Attributes["address1_country"] : null,
                                    birthdate = contact.Attributes.ContainsKey("birthdate") ? (DateTime)contact.Attributes["birthdate"] : _defaultDate,
                                    gendercode = contact.Attributes.ContainsKey("gendercode") ? contact.FormattedValues["gendercode"].ToString() : "",
                                    ice_besttimereach = contact.Attributes.ContainsKey("ice_besttimereach") ? (string)contact.Attributes["ice_besttimereach"] : "",
                                    ice_injureddifferent = contact.Attributes.ContainsKey("ice_injureddifferent") ? contact.FormattedValues["ice_injureddifferent"].ToString() : "",
                                    ice_legalrepresentative = contact.Attributes.ContainsKey("ice_legalrepresentative") ? contact.FormattedValues["ice_legalrepresentative"].ToString() : "LegalRepresentative",
                                    ice_clientrelationship = contact.Attributes.ContainsKey("ice_clientrelationship") ? (string)contact.Attributes["ice_clientrelationship"] : null,
                                    ice_clientfirstname = contact.Attributes.ContainsKey("ice_clientfirstname") ? (string)contact.Attributes["ice_clientfirstname"] : null,
                                    ice_clientlastname = contact.Attributes.ContainsKey("ice_clientlastname") ? (string)contact.Attributes["ice_clientlastname"] : null,
                                    address2_telephone1 = contact.Attributes.ContainsKey("address2_telephone1") ? (string)contact.Attributes["address2_telephone1"] : null,
                                    emailaddress2 = contact.Attributes.ContainsKey("emailaddress2") ? (string)contact.Attributes["emailaddress2"] : null,
                                    ice_clientdateofbirth = contact.Attributes.ContainsKey("ice_clientdateofbirth") ? (DateTime)contact.Attributes["ice_clientdateofbirth"] : _defaultDate,
                                    ice_clientminor = contact.Attributes.ContainsKey("ice_clientminor") ? (bool)contact.Attributes["ice_clientminor"] : false,
                                    ice_clientdeceased = contact.Attributes.ContainsKey("ice_clientdeceased") ? (bool)contact.Attributes["ice_clientdeceased"] : false,
                                    ice_leaddetails = contact.Attributes.ContainsKey("ice_leaddetails2") ? (string)contact.Attributes["ice_leaddetails2"] : null,
                                    description = contact.Attributes.ContainsKey("description") ? (string)contact.Attributes["description"] : null,
                                    ice_casetypeid = contact.Attributes.ContainsKey("ice_casetypeid") ? ((EntityReference)contact.Attributes["ice_casetypeid"]).Id :Guid.Empty,
                                    ice_casetypeidname = contact.Attributes.ContainsKey("ice_casetypeid") ? ((EntityReference)contact.Attributes["ice_casetypeid"]).Name : null,
                                    ice_rundialog = contact.Attributes.ContainsKey("ice_rundialog") ? (bool)contact.Attributes["ice_rundialog"] : false,
                                    ice_intakedate = contact.Attributes.ContainsKey("ice_intakedate") ? (DateTime)contact.Attributes["ice_intakedate"] : _defaultDate,
                                    ownerid = contact.Attributes.ContainsKey("ownerid") ? ((EntityReference)contact.Attributes["ownerid"]).Name : null,
                                    ice_attempt = contact.Attributes.ContainsKey("ice_attempt") ? (int)contact.Attributes["ice_attempt"] : 0,
                                    update_requested = contact.Attributes.ContainsKey("update_requested") ? (bool)contact.Attributes["update_requested"] : false,
                                    retained_by_phone = contact.Attributes.ContainsKey("retained_by_phone") ? (bool)contact.Attributes["retained_by_phone"] : false,
                                    retained_by = contact.Attributes.ContainsKey("retained_by") ? (string)contact.Attributes["retained_by"] : null,
                                    ice_dnis = contact.Attributes.ContainsKey("ice_dnis") ? (string)contact.Attributes["ice_dnis"] : null,
                                    ice_ani = contact.Attributes.ContainsKey("telephone3") ? (string)contact.Attributes["telephone3"] : null,
                                    ice_call_received_time = contact.Attributes.ContainsKey("ice_intakedate") ? (DateTime)contact.Attributes["ice_intakedate"] : _defaultDate,
                                    ice_marketingsourcedetails = contact.Attributes.ContainsKey("ice_marketingsourcedetails") ? (string)contact.Attributes["ice_marketingsourcedetails"] : null,
                                    ice_internalnotes = contact.Attributes.ContainsKey("ice_internalnotes") ? (string)contact.Attributes["ice_internalnotes"] : null,
                                    ice_vendorid = contact.Attributes.ContainsKey("ice_vendorid") ? (string)contact.Attributes["ice_vendorid"] : null,
                                    ice_disqualificationreason = contact.Attributes.ContainsKey("ice_disqualificationreason") ? (string)contact.Attributes["ice_disqualificationreason"] : null,
                                    ice_confidencelevel = contact.Attributes.ContainsKey("ice_confidencelevel") ? (string)contact.Attributes["ice_confidencelevel"] : null,
                                    ice_leadgrade = contact.Attributes.ContainsKey("ice_leadgrade") ? contact.FormattedValues["ice_leadgrade"].ToString() : "A",
                                    ice_leadscore = contact.Attributes.ContainsKey("ice_leadscore") ? (decimal)contact.Attributes["ice_leadscore"] : 0,
                                    ice_intakestatus = contact.Attributes.ContainsKey("ice_intakestatus") ? contact.FormattedValues["ice_intakestatus"].ToString() : null,
                                    ice_leadstatus = contact.Attributes.ContainsKey("ice_leadstatus") ? contact.FormattedValues["ice_leadstatus"].ToString() : null,
                                    ice_closestatus = contact.Attributes.ContainsKey("ice_closestatus") ? contact.FormattedValues["ice_closestatus"].ToString() : null,
                                    ice_nosignstatus = contact.Attributes.ContainsKey("ice_nosignstatus") ? contact.FormattedValues["ice_nosignstatus"].ToString() : null,
                                    ice_signuppackagedate = contact.Attributes.ContainsKey("ice_signuppackagedate") ? (DateTime)contact.Attributes["ice_signuppackagedate"] : _defaultDate,
                                    ice_internalreviewdate = contact.Attributes.ContainsKey("ice_internalreviewdate") ? (DateTime)contact.Attributes["ice_internalreviewdate"] : _defaultDate,
                                    ice_intakeclosedate = contact.Attributes.ContainsKey("ice_intakeclosedate") ? (DateTime)contact.Attributes["ice_intakeclosedate"] : _defaultDate,
                                    cap_nolocampaignid = contact.Attributes.ContainsKey("cap_nolocampaignid") ? (string)contact.Attributes["cap_nolocampaignid"] : null,
                                    address3_primarycontactname = contact.Attributes.ContainsKey("address3_primarycontactname") ? (string)contact.Attributes["address3_primarycontactname"] : null,
                                    cap_zipofincident = contact.Attributes.ContainsKey("cap_zipofincident") ? (string)contact.Attributes["cap_zipofincident"] : null,
                                    ice_transcriptstatus = contact.Attributes.ContainsKey("ice_transcriptstatus") ? contact.FormattedValues["ice_transcriptstatus"].ToString() : null,
                                    cap_dialogqid01 = contact.Attributes.ContainsKey("cap_dialogqid01") ? (string)contact.Attributes["cap_dialogqid01"] : null,
                                    cap_dialogqid02 = contact.Attributes.ContainsKey("cap_dialogqid02") ? (string)contact.Attributes["cap_dialogqid02"] : null,
                                    cap_dialogqid03 = contact.Attributes.ContainsKey("cap_dialogqid03") ? (string)contact.Attributes["cap_dialogqid03"] : null,
                                    cap_dialogqid04 = contact.Attributes.ContainsKey("cap_dialogqid04") ? (string)contact.Attributes["cap_dialogqid04"] : null,
                                    cap_dialogqid05 = contact.Attributes.ContainsKey("cap_dialogqid05") ? (string)contact.Attributes["cap_dialogqid05"] : null,
                                    cap_dialogqid06 = contact.Attributes.ContainsKey("cap_dialogqid06") ? (string)contact.Attributes["cap_dialogqid06"] : null,
                                    cap_dialogaid01 = contact.Attributes.ContainsKey("cap_dialogaid01") ? (string)contact.Attributes["cap_dialogaid01"] : null,
                                    cap_dialogaid02 = contact.Attributes.ContainsKey("cap_dialogaid02") ? (string)contact.Attributes["cap_dialogaid02"] : null,
                                    cap_dialogaid03 = contact.Attributes.ContainsKey("cap_dialogaid03") ? (string)contact.Attributes["cap_dialogaid03"] : null,
                                    cap_dialogaid04 = contact.Attributes.ContainsKey("cap_dialogaid04") ? (string)contact.Attributes["cap_dialogaid04"] : null,
                                    cap_dialogaid05 = contact.Attributes.ContainsKey("cap_dialogaid05") ? (string)contact.Attributes["cap_dialogaid05"] : null,
                                    cap_dialogaid06 = contact.Attributes.ContainsKey("cap_dialogaid06") ? (string)contact.Attributes["cap_dialogaid06"] : null,
                                };

                    if (Leads.ToList().Count > 0)
                    {
                        _log.Info("{0} Total Records Found.", Leads.ToList().Count);
                        foreach (var lead in Leads)
                        {
                            //Create the new ICEWarmTransfer object    
                            var objICEWarmTransfer = new ICEWarmTransfer();
                            objICEWarmTransfer.contactid = lead.contactid;
                            objICEWarmTransfer.parentcustomerid = lead.parentcustomerid;
                            objICEWarmTransfer.ice_language = lead.ice_language;
                            objICEWarmTransfer.firstname = lead.firstname;
                            objICEWarmTransfer.lastname = lead.lastname;
                            objICEWarmTransfer.emailaddress1 = lead.emailaddress1;
                            objICEWarmTransfer.mobilephone = lead.mobilephone;
                            objICEWarmTransfer.address1_telephone1 = lead.address2_telephone1;
                            objICEWarmTransfer.address1_line1 = lead.address1_line1;
                            objICEWarmTransfer.address1_city = lead.address1_city;
                            objICEWarmTransfer.address1_stateorprovince = lead.address1_stateorprovince;
                            objICEWarmTransfer.address1_postalcode = lead.address1_postalcode;
                            objICEWarmTransfer.address1_county = lead.address1_county;
                            objICEWarmTransfer.address1_country = lead.address1_country;
                            if (lead.birthdate != _defaultDate)
                                objICEWarmTransfer.birthdate = lead.birthdate.ToShortDateString();
                            else
                                objICEWarmTransfer.birthdate = "";
                            objICEWarmTransfer.gendercode = lead.gendercode;
                            objICEWarmTransfer.ice_besttimereach = lead.ice_besttimereach;
                            objICEWarmTransfer.ice_injureddifferent = lead.ice_injureddifferent.Replace(" ", String.Empty);
                            objICEWarmTransfer.ice_legalrepresentative = lead.ice_legalrepresentative.Replace(" ", String.Empty);
                            objICEWarmTransfer.ice_clientrelationship = lead.ice_clientrelationship;
                            objICEWarmTransfer.ice_clientfirstname = lead.ice_clientfirstname;
                            objICEWarmTransfer.ice_clientlastname = lead.ice_clientlastname;
                            objICEWarmTransfer.address2_telephone1 = lead.address2_telephone1;
                            objICEWarmTransfer.emailaddress2 = lead.emailaddress2;
                            if (lead.ice_clientdateofbirth != _defaultDate)
                                objICEWarmTransfer.ice_clientdateofbirth = lead.ice_clientdateofbirth.ToShortDateString();
                            else
                                objICEWarmTransfer.ice_clientdateofbirth = null;
                            objICEWarmTransfer.ice_clientminor = lead.ice_clientminor;
                            objICEWarmTransfer.ice_clientdeceased = lead.ice_clientdeceased;
                            objICEWarmTransfer.ice_leaddetails = lead.ice_leaddetails;
                            objICEWarmTransfer.description = lead.description;
                            objICEWarmTransfer.ice_casetypeid = lead.ice_casetypeidname;
                            objICEWarmTransfer.ice_rundialog = lead.ice_rundialog;
                            if (lead.ice_intakedate != _defaultDate)
                                objICEWarmTransfer.ice_intakedate = lead.ice_intakedate.ToShortDateString();
                            else
                                objICEWarmTransfer.ice_intakedate = null;
                            objICEWarmTransfer.ownerid = lead.ownerid;
                            objICEWarmTransfer.lead_attempts = lead.ice_attempt;
                            objICEWarmTransfer.update_requested = lead.update_requested;
                            objICEWarmTransfer.retained_by_phone = lead.retained_by_phone;
                            objICEWarmTransfer.retained_by = lead.retained_by;
                            objICEWarmTransfer.ice_dnis = lead.ice_dnis;
                            objICEWarmTransfer.ice_ani = lead.ice_ani;
                            objICEWarmTransfer.ice_call_received_time = lead.ice_call_received_time.ToString("hh:mm tt");
                            objICEWarmTransfer.ice_marketingsourcedetails = lead.ice_marketingsourcedetails;
                            objICEWarmTransfer.ice_internalnotes = lead.ice_internalnotes;
                            objICEWarmTransfer.ice_vendorid = lead.ice_vendorid;
                            objICEWarmTransfer.ice_disqualificationreason = lead.ice_disqualificationreason;
                            objICEWarmTransfer.ice_confidencelevel = lead.ice_confidencelevel;
                            objICEWarmTransfer.ice_leadgrade = lead.ice_leadgrade;
                            objICEWarmTransfer.ice_leadscore = decimal.ToInt32(lead.ice_leadscore);

                            if (lead.ice_closestatus != "No Sign")
                                if (lead.ice_intakestatus == "Lead")
                                    if (lead.ice_leadstatus == "New")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[0]; //ActiveLead
                                    else if (lead.ice_leadstatus == "Expired")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[1]; //Expired
                                    else if (lead.ice_leadstatus == "Wrong Number")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[2]; //BadNumber
                                    else if (lead.ice_leadstatus == "Not Interested")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[3]; //NoLongerInterested
                                    else if (lead.ice_leadstatus == "Already Has Attorney")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[4]; //AlreadyHasAttorney
                                    else if (lead.ice_leadstatus == "Invalid")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[5]; //Invalid
                                    else objICEWarmTransfer.ice_intakestatus = "";
                                else if (lead.ice_intakestatus == "Internal Review")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[7]; //InternalReview
                                else if (lead.ice_intakestatus == "Sign Up Package")
                                    if (lead.ice_closestatus == "Retained")
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[9]; //Retained
                                    else
                                        objICEWarmTransfer.ice_intakestatus = intakeStatus[8]; //SignUpPackage
                                else if (lead.ice_intakestatus == "Disqualified")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[10]; //Disqualified
                                else
                                    objICEWarmTransfer.ice_intakestatus = "";
                            else if (lead.ice_closestatus == "No Sign")
                                if (lead.ice_nosignstatus == "Client Rejected Us")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[11]; //ClientRejectedUs
                                else if (lead.ice_nosignstatus == "No Longer Pursuing Case")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[12]; //NoLongerPursuing_Case
                                else if (lead.ice_nosignstatus == "No Response")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[13]; //NoResponse
                                else if (lead.ice_nosignstatus == "Selected Another Attorney")
                                    objICEWarmTransfer.ice_intakestatus = intakeStatus[14]; //NoResponse
                                else
                                    objICEWarmTransfer.ice_intakestatus = "";
                            else
                                objICEWarmTransfer.ice_intakestatus = "";

                            if (lead.ice_internalreviewdate != _defaultDate)
                                objICEWarmTransfer.ice_internalreviewdate = lead.ice_internalreviewdate.ToShortDateString();
                            else
                                objICEWarmTransfer.ice_internalreviewdate = null;
                            if (lead.ice_internalreviewdate != _defaultDate)
                                objICEWarmTransfer.ice_intakeclosedate = lead.ice_intakeclosedate.ToShortDateString();
                            else
                                objICEWarmTransfer.ice_intakeclosedate = null;

                            objICEWarmTransfer.campaign_id = lead.cap_nolocampaignid == null ? 0 : int.Parse(lead.cap_nolocampaignid);

                            var CaseTypes = from casetype in orgSvcContext.CreateQuery("ice_casetype")
                                                 where (Guid)casetype["ice_casetypeid"] == lead.ice_casetypeid
                                                 select new
                                                 { 
                                                     cap_nolopracticeareaid = casetype.Attributes.ContainsKey("cap_nolopracticeareaid") ? (string)casetype.Attributes["cap_nolopracticeareaid"] : null
                                                 };

                            foreach (var type in CaseTypes)
                            {
                                objICEWarmTransfer.practice_area_id = type.cap_nolopracticeareaid == null ? 0 : int.Parse(type.cap_nolopracticeareaid);
                            }

                            objICEWarmTransfer.account_id = lead.address3_primarycontactname == null ? 0 : int.Parse(lead.address3_primarycontactname);
                            objICEWarmTransfer.cap_zipofincident = lead.cap_zipofincident;
                            objICEWarmTransfer.ice_transcriptstatus = lead.ice_transcriptstatus == null ? "" : lead.ice_transcriptstatus.Replace(" ", String.Empty);

                        List<QuestionAnswer> questionAnswer = new List<QuestionAnswer>();
                            if (lead.cap_dialogqid01 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid01), answers = int.Parse(lead.cap_dialogaid01) });
                            if (lead.cap_dialogqid02 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid02), answers = int.Parse(lead.cap_dialogaid02) });
                            if (lead.cap_dialogqid03 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid03), answers = int.Parse(lead.cap_dialogaid03) });
                            if (lead.cap_dialogqid04 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid04), answers = int.Parse(lead.cap_dialogaid04) });
                            if (lead.cap_dialogqid05 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid05), answers = int.Parse(lead.cap_dialogaid05) });
                            if (lead.cap_dialogqid06 != null) questionAnswer.Add(new QuestionAnswer { question_id = int.Parse(lead.cap_dialogqid06), answers = int.Parse(lead.cap_dialogaid06) });


                        objICEWarmTransfer.question_set = questionAnswer;

                        lstICEWarmTransfer.Add(objICEWarmTransfer);
                        }
                    }
                    else
                    {
                        //no contact
                    }
                }

                return lstICEWarmTransfer;

            }
            catch (Exception ex)
            {
                _dbLog.InsertLog(LogLevels.Fatal, ReqId, "Exception: " + ex.Message + Environment.NewLine + "InnerException: " + ex.InnerException.Message);
                _log.Fatal(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IntakeId"></param>
        /// <param name="payload"></param>
        /// <param name="response"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        public void CreateJSonPostingUpdateIntake(Guid contactid, string payload, string response, string FirstName, string LastName)
        {
            try
            {

                if (contactid != Guid.Empty)
                {
                    var crmSvcClient = CrmConnectionService.getServiceClient();
                    var contactId = Guid.Empty;
                    //Create the JSON Posting Entity.
                    Entity JsonPosting = new Entity("ice_jsonposting");
                    EntityReference refContact = null;
                    contactId = contactid;
                    if (contactId != Guid.Empty)
                    {
                        refContact = new EntityReference("contact", contactId);
                        JsonPosting["ice_intakeid"] = refContact;
                    }
                    JsonPosting["ice_name"] = contactId.ToString() + "_Create_WarmTransfer";
                    JsonPosting["ice_jsonstatus"] = false;
                    JsonPosting["ice_response"] = response;
                    JsonPosting["ice_payload"] = payload;
                    CrmOperationService objCrmOperationsService = new CrmOperationService();
                    objCrmOperationsService.createRecord(JsonPosting, crmSvcClient);
                    _log.Info("JSON Posting created successfully for {0} in ICE", FirstName + " " + LastName);

                    //// Update the Intake ice_lmbupdate = false  
                    //Entity intake = null;
                    //intake = new Entity("contact");
                    //intake.Id = contactId;
                    //intake["ice_lmbupdate"] = false;
                    //objCrmOperationsService.updateEntity(intake, crmSvcClient);
                    //_log.Info("{0} updated with ice_lmbupdate = No successfully in ICE.", FirstName + " " + LastName);


                }
            }
            catch (Exception ex)
            {
                _log.Fatal("Exception {0}", ex.Message + Environment.NewLine + ex.InnerException.Message);
                throw ex;
            }
        }

       
    }
}

