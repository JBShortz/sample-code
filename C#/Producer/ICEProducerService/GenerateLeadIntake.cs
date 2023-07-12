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
    public class GenerateLeadIntake
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();        
        private static DateTime _defaultDate = DateTime.Parse("1/1/1990").Date;
        private static ApplicationLogService _dbLog = new ApplicationLogService();
        public List<ICELeadIntake> getRecords(string ReqId)
        {
           
            string[] intakeStatus = new string[] { "ActiveLead", "Expired", "BadNumber", "NoLongerInterested", "AlreadyHasAttorney", "Invalid", "Duplicate", "InternalReview", "SignUpPackage", "Retained", "Disqualified", "ClientRejectedUs", "NoLongerPursuing_Case", "NoResponse", "SelectedAnotherAttorney" };
            List<ICELeadIntake> lstICELeadIntake = new List<ICELeadIntake>();
            try
            {

                var crmSvcClient = CrmConnectionService.getServiceClient();

                if (crmSvcClient != null && crmSvcClient.LastCrmError != "")
                {
                    _log.Error("Last CRM Error {0} ", crmSvcClient.LastCrmError);
                    _dbLog.InsertLog(LogLevels.Fatal, ReqId, "Last CRM Error  " + crmSvcClient.LastCrmError);
                    return lstICELeadIntake;
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
                                    ice_casetypeid = contact.Attributes.ContainsKey("ice_casetypeid") ? ((EntityReference)contact.Attributes["ice_casetypeid"]).Name : null,
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
                                    
                                    cap_zipofincident = contact.Attributes.ContainsKey("cap_zipofincident") ? (string)contact.Attributes["cap_zipofincident"] : null,
                                    ice_transcriptstatus = contact.Attributes.ContainsKey("ice_transcriptstatus") ? contact.FormattedValues["ice_transcriptstatus"].ToString() : null,
                                    cap_judgesdecision = contact.Attributes.ContainsKey("cap_judgesdecision") ? (string)contact.Attributes["cap_judgesdecision"] : null,
                                    cap_dateofapplication = contact.Attributes.ContainsKey("cap_dateofapplication") ? (DateTime)contact.Attributes["cap_dateofapplication"] : _defaultDate,
                                    cap_dateinitiallydenied = contact.Attributes.ContainsKey("cap_dateinitiallydenied") ? (DateTime)contact.Attributes["cap_dateinitiallydenied"] : _defaultDate,
                                    cap_dateappealed = contact.Attributes.ContainsKey("cap_dateappealed") ? (DateTime)contact.Attributes["cap_dateappealed"] : _defaultDate,
                                    cap_dateappealeddenied = contact.Attributes.ContainsKey("cap_dateappealeddenied") ? (DateTime)contact.Attributes["cap_dateappealeddenied"] : _defaultDate,
                                    cap_datesecondappeal = contact.Attributes.ContainsKey("cap_datesecondappeal") ? (DateTime)contact.Attributes["cap_datesecondappeal"] : _defaultDate,
                                    cap_dateofhearing = contact.Attributes.ContainsKey("cap_dateofhearing") ? (DateTime)contact.Attributes["cap_dateofhearing"] : _defaultDate,
                                    cap_diagnosespreventingwork = contact.Attributes.ContainsKey("cap_diagnosespreventingwork") ? (string)contact.Attributes["cap_diagnosespreventingwork"] : null,
                                    cap_statementofdisability = contact.Attributes.ContainsKey("cap_statementofdisability") ? (string)contact.Attributes["cap_statementofdisability"] : null,
                                    cap_degree = contact.Attributes.ContainsKey("cap_degree") ? (string)contact.Attributes["cap_degree"] : null,
                                    cap_whydidyoustopworking = contact.Attributes.ContainsKey("cap_whydidyoustopworking") ? contact.FormattedValues["cap_whydidyoustopworking"].ToString() : null,
                                    cap_worksincedisabled = contact.Attributes.ContainsKey("cap_worksincedisabled") ? (string)contact.Attributes["cap_worksincedisabled"] : null,
                                    cap_dateoflastdoctorvisit = contact.Attributes.ContainsKey("cap_dateoflastdoctorvisit") ? (DateTime)contact.Attributes["cap_dateoflastdoctorvisit"] : _defaultDate,
                                    cap_expectoutofwork1year = contact.Attributes.ContainsKey("cap_expectoutofwork1year") ? contact.FormattedValues["cap_expectoutofwork1year"].ToString() : null,
                                    cap_ssattorneylast90days = contact.Attributes.ContainsKey("cap_ssattorneylast90days") ? contact.FormattedValues["cap_ssattorneylast90days"].ToString() : null,
                                    cap_otherfirmstillrepresenting = contact.Attributes.ContainsKey("cap_otherfirmstillrepresenting") ? contact.FormattedValues["cap_otherfirmstillrepresenting"].ToString() : null,
                                    cap_militaryveteran = contact.Attributes.ContainsKey("cap_militaryveteran") ? contact.FormattedValues["cap_militaryveteran"].ToString() : null,
                                    cap_osveteranrating = contact.Attributes.ContainsKey("cap_osveteranrating") ? contact.FormattedValues["cap_osveteranrating"].ToString() : null,
                                    cap_uscitizen = contact.Attributes.ContainsKey("cap_uscitizen") ? contact.FormattedValues["cap_uscitizen"].ToString() : null,
                                    cap_havevalidgreencard = contact.Attributes.ContainsKey("cap_havevalidgreencard") ? contact.FormattedValues["cap_havevalidgreencard"].ToString() : null,
                                    cap_osprivatedisability = contact.Attributes.ContainsKey("cap_osprivatedisability") ? contact.FormattedValues["cap_osprivatedisability"].ToString() : null,
                                    cap_currentlyreceivingssbenefits = contact.Attributes.ContainsKey("cap_currentlyreceivingssbenefits") ? contact.FormattedValues["cap_currentlyreceivingssbenefits"].ToString() : null,
                                    cap_osalidenial = contact.Attributes.ContainsKey("cap_osalidenial") ? contact.FormattedValues["cap_osalidenial"].ToString() : null,
                                    cap_currentclaimpending = contact.Attributes.ContainsKey("cap_currentclaimpending") ? contact.FormattedValues["cap_currentclaimpending"].ToString() : null,
                                    cap_statusofclaim = contact.Attributes.ContainsKey("cap_statusofclaim") ? contact.FormattedValues["cap_statusofclaim"].ToString() : null,
                                    cap_didyouhaveahearing = contact.Attributes.ContainsKey("cap_didyouhaveahearing") ? contact.FormattedValues["cap_didyouhaveahearing"].ToString() : null,
                                    cap_conditionslist = contact.Attributes.ContainsKey("cap_conditionslist") ? contact.FormattedValues["cap_conditionslist"].ToString() : null,
                                    cap_deceasedspouselast7years = contact.Attributes.ContainsKey("cap_deceasedspouselast7years") ? contact.FormattedValues["cap_deceasedspouselast7years"].ToString() : null,
                                    cap_marriagelasted10years = contact.Attributes.ContainsKey("cap_marriagelasted10years") ? contact.FormattedValues["cap_marriagelasted10years"].ToString() : null,
                                    cap_deceasedworked5ofthelast10years = contact.Attributes.ContainsKey("cap_deceasedworked5ofthelast10years") ? contact.FormattedValues["cap_deceasedworked5ofthelast10years"].ToString() : null,
                                    cap_taxwithholding = contact.Attributes.ContainsKey("cap_taxwithholding") ? contact.FormattedValues["cap_taxwithholding"].ToString() : null,
                                    cap_fulltimejoblast5years = contact.Attributes.ContainsKey("cap_fulltimejoblast5years") ? contact.FormattedValues["cap_fulltimejoblast5years"].ToString() : null,
                                    cap_worked5ofthelast10years = contact.Attributes.ContainsKey("cap_worked5ofthelast10years") ? contact.FormattedValues["cap_worked5ofthelast10years"].ToString() : null,
                                    cap_stillemployed = contact.Attributes.ContainsKey("cap_stillemployed") ? contact.FormattedValues["cap_stillemployed"].ToString() : null,
                                    cap_hourlywage = contact.Attributes.ContainsKey("cap_hourlywage") ? (int)contact.Attributes["cap_hourlywage"] : 0,
                                    cap_monthlywage = contact.Attributes.ContainsKey("cap_monthlywage") ? (int)contact.Attributes["cap_monthlywage"] : 0,
                                    cap_hoursperweek = contact.Attributes.ContainsKey("cap_hoursperweek") ? (int)contact.Attributes["cap_hoursperweek"] : 0,
                                    cap_hourspermonth = contact.Attributes.ContainsKey("cap_hourspermonth") ? (int)contact.Attributes["cap_hourspermonth"] : 0,
                                    cap_lastworked25hoursperweek = contact.Attributes.ContainsKey("cap_lastworked25hoursperweek") ? (string)contact.Attributes["cap_lastworked25hoursperweek"] : null,
                                    cap_ostreatment = contact.Attributes.ContainsKey("cap_ostreatment") ? contact.FormattedValues["cap_ostreatment"].ToString() : null,
                                    cap_osnarcoticpainmed = contact.Attributes.ContainsKey("cap_osnarcoticpainmed") ? contact.FormattedValues["cap_osnarcoticpainmed"].ToString() : null,
                                    cap_osoxygen = contact.Attributes.ContainsKey("cap_osoxygen") ? contact.FormattedValues["cap_osoxygen"].ToString() : null,
                                    cap_oscanewalkerwheelchair = contact.Attributes.ContainsKey("cap_osoxygen") ? contact.FormattedValues["cap_osoxygen"].ToString() : null,
                                    cap_oshandicaplacard = contact.Attributes.ContainsKey("cap_oshandicaplacard") ? contact.FormattedValues["cap_oshandicaplacard"].ToString() : null,
                                    cap_osjail6months = contact.Attributes.ContainsKey("cap_osjail6months") ? contact.FormattedValues["cap_osjail6months"].ToString() : null,
                                    cap_currentlyattendingschool = contact.Attributes.ContainsKey("cap_currentlyattendingschool") ? contact.FormattedValues["cap_currentlyattendingschool"].ToString() : null,
                                    cap_credits = contact.Attributes.ContainsKey("cap_credits") ? contact.FormattedValues["cap_credits"].ToString() : null,
                                    cap_oseducation = contact.Attributes.ContainsKey("cap_oseducation") ? contact.FormattedValues["cap_oseducation"].ToString() : null,
                                    cap_illicitdrugsinthepastyear = contact.Attributes.ContainsKey("cap_illicitdrugsinthepastyear") ? contact.FormattedValues["cap_illicitdrugsinthepastyear"].ToString() : null,
                                    cap_sobermorethan90days = contact.Attributes.ContainsKey("cap_sobermorethan90days") ? contact.FormattedValues["cap_sobermorethan90days"].ToString() : null,
                                    cap_addressvalidated = contact.Attributes.ContainsKey("cap_addressvalidated") ? (bool)contact.Attributes["cap_addressvalidated"] : false,
                                    cap_conditionsscoring = contact.Attributes.ContainsKey("cap_conditionsscoring") ? (string)contact.Attributes["cap_conditionsscoring"] : null,
                                    cap_score = contact.Attributes.ContainsKey("cap_score") ? (string)contact.Attributes["cap_score"] : null,
                                    ice_leaddetails2 = contact.Attributes.ContainsKey("ice_leaddetails2") ? (string)contact.Attributes["ice_leaddetails2"] : null,
                                    ice_contactsource = contact.Attributes.ContainsKey("ice_contactsource") ? contact.FormattedValues["ice_contactsource"].ToString() : null,
                                    salutation = contact.Attributes.ContainsKey("salutation") ? (string)contact.Attributes["salutation"] : null,
                                    suffix = contact.Attributes.ContainsKey("suffix") ? (string)contact.Attributes["suffix"] : null,
                                    cap_leadid = contact.Attributes.ContainsKey("cap_leadid") ? ((EntityReference)contact.Attributes["cap_leadid"]).Name : null,
                                    ice_signuptype = contact.Attributes.ContainsKey("ice_signuptype") ? contact.FormattedValues["ice_signuptype"].ToString() : null,
                                    cap_intakestatusreason = contact.Attributes.ContainsKey("cap_intakestatusreason") ? (string)contact.Attributes["cap_intakestatusreason"] : null,
                                    cap_laststatuschange = contact.Attributes.ContainsKey("cap_laststatuschange") ? (string)contact.Attributes["cap_laststatuschange"] : null,
                                    address1_line2 = contact.Attributes.ContainsKey("address1_line2") ? (string)contact.Attributes["address1_line2"] : null,
                                    familystatuscode = contact.Attributes.ContainsKey("familystatuscode") ? contact.FormattedValues["familystatuscode"].ToString() : null,
                                    spousesname = contact.Attributes.ContainsKey("spousesname") ? (string)contact.Attributes["spousesname"] : null,
                                    cap_agecalculatedfield = contact.Attributes.ContainsKey("cap_agecalculatedfield") ? (string)contact.Attributes["cap_agecalculatedfield"] : null,
                                    cap_emergencycontactname = contact.Attributes.ContainsKey("cap_emergencycontactname") ? (string)contact.Attributes["cap_emergencycontactname"] : null,
                                    cap_alternateaddress = contact.Attributes.ContainsKey("cap_alternateaddress") ? (string)contact.Attributes["cap_alternateaddress"] : null,
                                    cap_emergencycontactphone = contact.Attributes.ContainsKey("cap_emergencycontactphone") ? (string)contact.Attributes["cap_emergencycontactphone"] : null,
                                    cap_sameaddressasapplicant = contact.Attributes.ContainsKey("cap_sameaddressasapplicant") ? contact.FormattedValues["cap_sameaddressasapplicant"].ToString() : null,
                                    cap_relationshiptoapplicant = contact.Attributes.ContainsKey("cap_relationshiptoapplicant") ? (string)contact.Attributes["cap_relationshiptoapplicant"] : null,
                                    cap_ecspeaksenglish = contact.Attributes.ContainsKey("cap_ecspeaksenglish") ? contact.FormattedValues["cap_ecspeaksenglish"].ToString() : null,
                                    cap_mmn = contact.Attributes.ContainsKey("cap_mmn") ? (string)contact.Attributes["cap_mmn"] : null,
                                    cap_fathersname = contact.Attributes.ContainsKey("cap_fathersname") ? (string)contact.Attributes["cap_fathersname"] : null,
                                    cap_placeofbirth = contact.Attributes.ContainsKey("cap_placeofbirth") ? (string)contact.Attributes["cap_placeofbirth"] : null,
                                    cap_othernames = contact.Attributes.ContainsKey("cap_othernames") ? (string)contact.Attributes["cap_othernames"] : null,
                                    ice_lastfollowupdate = contact.Attributes.ContainsKey("ice_lastfollowupdate") ? (DateTime)contact.Attributes["ice_lastfollowupdate"] : _defaultDate,
                                    ice_nextfollowupdate = contact.Attributes.ContainsKey("ice_nextfollowupdate") ? (DateTime)contact.Attributes["ice_nextfollowupdate"] : _defaultDate,
                                    cap_leadfollowuptimeout = contact.Attributes.ContainsKey("cap_leadfollowuptimeout") ? (string)contact.Attributes["cap_leadfollowuptimeout"] : null,
                                    cap_nextcommunicationdate = contact.Attributes.ContainsKey("cap_nextcommunicationdate") ? (DateTime)contact.Attributes["cap_nextcommunicationdate"] : _defaultDate,
                                    cap_lastcommunicationdate = contact.Attributes.ContainsKey("cap_lastcommunicationdate") ? (DateTime)contact.Attributes["cap_lastcommunicationdate"] : _defaultDate,
                                    cap_dripattempt = contact.Attributes.ContainsKey("cap_dripattempt") ? (int)contact.Attributes["cap_dripattempt"] : 0,
                                    cap_leaddripstatus = contact.Attributes.ContainsKey("cap_leaddripstatus") ? contact.FormattedValues["cap_leaddripstatus"].ToString() : null,
                                    cap_leaddriptimeout = contact.Attributes.ContainsKey("cap_leaddriptimeout") ? (string)contact.Attributes["cap_leaddriptimeout"] : null,
                                    cap_communitcationcounter = contact.Attributes.ContainsKey("cap_communitcationcounter") ? (int)contact.Attributes["cap_communitcationcounter"] : 0,
                                    cap_calltrackingid = contact.Attributes.ContainsKey("cap_calltrackingid") ? ((EntityReference)contact.Attributes["cap_calltrackingid"]).Name : null,
                                    cap_phonesourceid = contact.Attributes.ContainsKey("cap_phonesourceid") ? ((EntityReference)contact.Attributes["cap_phonesourceid"]).Name : null,
                                    cap_medications = contact.Attributes.ContainsKey("cap_medications") ? (string)contact.Attributes["cap_medications"] : null,
                                    jobtitle = contact.Attributes.ContainsKey("jobtitle") ? (string)contact.Attributes["jobtitle"] : null,
                                    cap_duplicateuserid = contact.Attributes.ContainsKey("cap_duplicateuserid") ? (string)contact.Attributes["cap_duplicateuserid"] : null,
                                    cap_duplicatedate = contact.Attributes.ContainsKey("cap_duplicatedate") ? (DateTime)contact.Attributes["cap_duplicatedate"] : _defaultDate,
                                    cap_contactinguserid = contact.Attributes.ContainsKey("cap_contactinguserid") ? (string)contact.Attributes["cap_contactinguserid"] : null,
                                    cap_contactingdate = contact.Attributes.ContainsKey("cap_contactingdate") ? (DateTime)contact.Attributes["cap_contactingdate"] : _defaultDate,
                                    cap_contacteduserid = contact.Attributes.ContainsKey("cap_contacteduserid") ? (string)contact.Attributes["cap_contacteduserid"] : null,
                                    cap_contacteddate = contact.Attributes.ContainsKey("cap_contacteddate") ? (DateTime)contact.Attributes["cap_contacteddate"] : _defaultDate,
                                    cap_cantcontactuserid = contact.Attributes.ContainsKey("cap_cantcontactuserid") ? (string)contact.Attributes["cap_cantcontactuserid"] : null,
                                    cap_cantcontactdate = contact.Attributes.ContainsKey("cap_cantcontactdate") ? (DateTime)contact.Attributes["cap_cantcontactdate"] : _defaultDate,
                                    cap_clientrefusedhelpuserid = contact.Attributes.ContainsKey("cap_clientrefusedhelpuserid") ? (string)contact.Attributes["cap_clientrefusedhelpuserid"] : null,
                                    cap_clientrefusedhelpdate = contact.Attributes.ContainsKey("cap_clientrefusedhelpdate") ? (DateTime)contact.Attributes["cap_clientrefusedhelpdate"] : _defaultDate,
                                    cap_intakecompleteduserid = contact.Attributes.ContainsKey("cap_intakecompleteduserid") ? ((EntityReference)contact.Attributes["cap_intakecompleteduserid"]).Name : null,
                                    cap_intakecompleteddate = contact.Attributes.ContainsKey("cap_intakecompleteddate") ? (DateTime)contact.Attributes["cap_intakecompleteddate"] : _defaultDate,
                                    cap_casewanteduserid = contact.Attributes.ContainsKey("cap_casewanteduserid") ? (string)contact.Attributes["cap_casewanteduserid"] : null,
                                    cap_casewanteddate = contact.Attributes.ContainsKey("cap_casewanteddate") ? (DateTime)contact.Attributes["cap_casewanteddate"] : _defaultDate,
                                    cap_caserejecteduserid = contact.Attributes.ContainsKey("cap_caserejecteduserid") ? (string)contact.Attributes["cap_caserejecteduserid"] : null,
                                    cap_caserejecteddate = contact.Attributes.ContainsKey("cap_caserejecteddate") ? (DateTime)contact.Attributes["cap_caserejecteddate"] : _defaultDate,
                                    cap_clientterminateduserid = contact.Attributes.ContainsKey("cap_clientterminateduserid") ? (string)contact.Attributes["cap_clientterminateduserid"] : null,
                                    cap_clientterminateddate = contact.Attributes.ContainsKey("cap_clientterminateddate") ? (DateTime)contact.Attributes["cap_clientterminateddate"] : _defaultDate,
                                    cap_disqualifieduserid = contact.Attributes.ContainsKey("cap_disqualifieduserid") ? ((EntityReference)contact.Attributes["cap_disqualifieduserid"]).Name : null,
                                    cap_disqualifieddate = contact.Attributes.ContainsKey("cap_disqualifieddate") ? (DateTime)contact.Attributes["cap_disqualifieddate"] : _defaultDate,
                                    cap_retaineduserid = contact.Attributes.ContainsKey("cap_retaineduserid") ? (string)contact.Attributes["cap_retaineduserid"] : null,
                                    cap_retaineddate = contact.Attributes.ContainsKey("cap_retaineddate") ? (DateTime)contact.Attributes["cap_retaineddate"] : _defaultDate,
                                    cap_intakereviewerid = contact.Attributes.ContainsKey("cap_intakereviewerid") ? (string)contact.Attributes["cap_intakereviewerid"] : null,
                                    cap_intakepackageid = contact.Attributes.ContainsKey("cap_intakepackageid") ? (string)contact.Attributes["cap_intakepackageid"] : null,
                                    cap_intakecloserid = contact.Attributes.ContainsKey("cap_intakecloserid") ? (string)contact.Attributes["cap_intakecloserid"] : null,
                                    cap_synccmuserid = contact.Attributes.ContainsKey("cap_synccmuserid") ? (string)contact.Attributes["cap_synccmuserid"] : null,
                                    ice_syncdatecm = contact.Attributes.ContainsKey("ice_syncdatecm") ? (DateTime)contact.Attributes["ice_syncdatecm"] : _defaultDate,
                                    cap_fcqsentuserid = contact.Attributes.ContainsKey("cap_fcqsentuserid") ? (string)contact.Attributes["cap_fcqsentuserid"] : null,
                                    cap_fcqsentdate = contact.Attributes.ContainsKey("cap_fcqsentdate") ? (DateTime)contact.Attributes["cap_fcqsentdate"] : _defaultDate,
                                    cap_fcqreceiveduserid = contact.Attributes.ContainsKey("cap_fcqreceiveduserid") ? (string)contact.Attributes["cap_fcqreceiveduserid"] : null,
                                    cap_fcqreceiveddate = contact.Attributes.ContainsKey("cap_fcqreceiveddate") ? (DateTime)contact.Attributes["cap_fcqreceiveddate"] : _defaultDate,
                                    cap_auditrejectionconfirmeduserid = contact.Attributes.ContainsKey("cap_auditrejectionconfirmeduserid") ? (string)contact.Attributes["cap_auditrejectionconfirmeduserid"] : null,
                                    cap_auditrejectionconfirmeddate = contact.Attributes.ContainsKey("cap_auditrejectionconfirmeddate") ? (DateTime)contact.Attributes["cap_auditrejectionconfirmeddate"] : _defaultDate,
                                    cap_overrideintakeneededuserid = contact.Attributes.ContainsKey("cap_overrideintakeneededuserid") ? (string)contact.Attributes["cap_overrideintakeneededuserid"] : null,
                                    cap_overrideintakeneededdate = contact.Attributes.ContainsKey("cap_overrideintakeneededdate") ? (DateTime)contact.Attributes["cap_overrideintakeneededdate"] : _defaultDate,
                                    cap_ssaappcompleteduserid = contact.Attributes.ContainsKey("cap_ssaappcompleteduserid") ? (string)contact.Attributes["cap_ssaappcompleteduserid"] : null,
                                    cap_ssaappcompleteddate = contact.Attributes.ContainsKey("cap_ssaappcompleteddate") ? (DateTime)contact.Attributes["cap_ssaappcompleteddate"] : _defaultDate,
                                    cap_1696fileduserid = contact.Attributes.ContainsKey("cap_1696fileduserid") ? (string)contact.Attributes["cap_1696fileduserid"] : null,
                                    cap_1696fileddate = contact.Attributes.ContainsKey("cap_1696fileddate") ? (DateTime)contact.Attributes["cap_1696fileddate"] : _defaultDate,
                                    cap_reviewedforpasuserid = contact.Attributes.ContainsKey("cap_reviewedforpasuserid") ? (string)contact.Attributes["cap_reviewedforpasuserid"] : null,
                                    cap_reviewedforpasdate = contact.Attributes.ContainsKey("cap_reviewedforpasdate") ? (DateTime)contact.Attributes["cap_reviewedforpasdate"] : _defaultDate,
                                    cap_reviewedforpasappuserid = contact.Attributes.ContainsKey("cap_reviewedforpasappuserid") ? (string)contact.Attributes["cap_reviewedforpasappuserid"] : null,
                                    cap_reviewedforpasappdate = contact.Attributes.ContainsKey("cap_reviewedforpasappdate") ? (DateTime)contact.Attributes["cap_reviewedforpasappdate"] : _defaultDate,
                                    cap_droppedcaseuserid = contact.Attributes.ContainsKey("cap_droppedcaseuserid") ? (string)contact.Attributes["cap_droppedcaseuserid"] : null,
                                    cap_droppedcasedate = contact.Attributes.ContainsKey("cap_droppedcasedate") ? (DateTime)contact.Attributes["cap_droppedcasedate"] : _defaultDate,

                                };

                    if (Leads.ToList().Count > 0)
                    {
                        _log.Info("{0} Total Records Found.", Leads.ToList().Count);
                    
                        foreach (var lead in Leads)
                        {
                            //Create the new ICELeadIntake object    
                            var objICELeadIntake = new ICELeadIntake();
                            objICELeadIntake.contactid = lead.contactid;
                            objICELeadIntake.parentcustomerid = lead.parentcustomerid;
                            objICELeadIntake.ice_language = lead.ice_language;
                            objICELeadIntake.firstname = lead.firstname;
                            objICELeadIntake.lastname = lead.lastname;
                            objICELeadIntake.emailaddress1 = lead.emailaddress1;
                            objICELeadIntake.mobilephone = lead.mobilephone;
                            objICELeadIntake.address1_telephone1 = lead.address2_telephone1;
                            objICELeadIntake.address1_line1 = lead.address1_line1;
                            objICELeadIntake.address1_city = lead.address1_city;
                            objICELeadIntake.address1_stateorprovince = lead.address1_stateorprovince;
                            objICELeadIntake.address1_postalcode = lead.address1_postalcode;
                            objICELeadIntake.address1_county = lead.address1_county;
                            objICELeadIntake.address1_country = lead.address1_country;
                            if(lead.birthdate != _defaultDate)
                                objICELeadIntake.birthdate = lead.birthdate.ToShortDateString();
                            else
                                objICELeadIntake.birthdate = "";
                            objICELeadIntake.gendercode = lead.gendercode;
                            objICELeadIntake.ice_besttimereach = lead.ice_besttimereach;
                            objICELeadIntake.ice_injureddifferent = lead.ice_injureddifferent.Replace(" ", String.Empty);
                            objICELeadIntake.ice_legalrepresentative = lead.ice_legalrepresentative.Replace(" ", String.Empty);
                            objICELeadIntake.ice_clientrelationship = lead.ice_clientrelationship;
                            objICELeadIntake.ice_clientfirstname = lead.ice_clientfirstname;
                            objICELeadIntake.ice_clientlastname = lead.ice_clientlastname;
                            objICELeadIntake.address2_telephone1 = lead.address2_telephone1;
                            objICELeadIntake.emailaddress2 = lead.emailaddress2;
                            if (lead.ice_clientdateofbirth != _defaultDate)
                                objICELeadIntake.ice_clientdateofbirth = lead.ice_clientdateofbirth.ToShortDateString();
                            else
                                objICELeadIntake.ice_clientdateofbirth = null;
                            objICELeadIntake.ice_clientminor = lead.ice_clientminor;
                            objICELeadIntake.ice_clientdeceased = lead.ice_clientdeceased;
                            objICELeadIntake.ice_leaddetails = lead.ice_leaddetails;
                            objICELeadIntake.description = lead.description;
                            objICELeadIntake.ice_casetypeid = lead.ice_casetypeid;
                            objICELeadIntake.ice_rundialog = lead.ice_rundialog;                        
                            if (lead.ice_intakedate != _defaultDate)
                                objICELeadIntake.ice_intakedate = lead.ice_intakedate.ToShortDateString();
                            else
                                objICELeadIntake.ice_intakedate = null;
                            objICELeadIntake.ownerid = lead.ownerid;
                            objICELeadIntake.lead_attempts = lead.ice_attempt;
                            objICELeadIntake.update_requested = lead.update_requested;
                            objICELeadIntake.retained_by_phone = lead.retained_by_phone;
                            objICELeadIntake.retained_by = lead.retained_by;
                            objICELeadIntake.ice_dnis = lead.ice_dnis;
                            objICELeadIntake.ice_marketingsourcedetails = lead.ice_marketingsourcedetails;
                            objICELeadIntake.ice_internalnotes = lead.ice_internalnotes;
                            objICELeadIntake.ice_vendorid = lead.ice_vendorid;
                            objICELeadIntake.ice_disqualificationreason = lead.ice_disqualificationreason;
                            objICELeadIntake.ice_confidencelevel = lead.ice_confidencelevel;
                            objICELeadIntake.ice_leadgrade = lead.ice_leadgrade;
                            objICELeadIntake.ice_leadscore = decimal.ToInt32(lead.ice_leadscore);

                            if(lead.ice_closestatus != "No Sign")
                                if (lead.ice_intakestatus == "Lead")
                                    if (lead.ice_leadstatus == "New")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[0]; //ActiveLead
                                    else if (lead.ice_leadstatus == "Expired")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[1]; //Expired
                                    else if (lead.ice_leadstatus == "Wrong Number")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[2]; //BadNumber
                                    else if (lead.ice_leadstatus == "Not Interested")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[3]; //NoLongerInterested
                                    else if (lead.ice_leadstatus == "Already Has Attorney")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[4]; //AlreadyHasAttorney
                                    else if (lead.ice_leadstatus == "Invalid")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[5]; //Invalid
                                    else objICELeadIntake.ice_intakestatus = "";
                                else if (lead.ice_intakestatus == "Internal Review")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[7]; //InternalReview
                                else if (lead.ice_intakestatus == "Sign Up Package")
                                    if (lead.ice_closestatus == "Retained")
                                        objICELeadIntake.ice_intakestatus = intakeStatus[9]; //Retained
                                    else
                                        objICELeadIntake.ice_intakestatus = intakeStatus[8]; //SignUpPackage
                                else if (lead.ice_intakestatus == "Disqualified")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[10]; //Disqualified
                                else
                                    objICELeadIntake.ice_intakestatus = "";
                            else if (lead.ice_closestatus == "No Sign")
                                if (lead.ice_nosignstatus == "Client Rejected Us")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[11]; //ClientRejectedUs
                                else if(lead.ice_nosignstatus == "No Longer Pursuing Case")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[12]; //NoLongerPursuing_Case
                                else if (lead.ice_nosignstatus == "No Response")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[13]; //NoResponse
                                else if (lead.ice_nosignstatus == "Selected Another Attorney")
                                    objICELeadIntake.ice_intakestatus = intakeStatus[14]; //NoResponse
                                else
                                    objICELeadIntake.ice_intakestatus = "";
                            else
                                objICELeadIntake.ice_intakestatus = "";
                                                 
                            if (lead.ice_internalreviewdate != _defaultDate)
                                objICELeadIntake.ice_internalreviewdate = lead.ice_internalreviewdate.ToShortDateString();
                            else
                                objICELeadIntake.ice_internalreviewdate = null;
                            if (lead.ice_internalreviewdate != _defaultDate)
                                objICELeadIntake.ice_intakeclosedate = lead.ice_intakeclosedate.ToShortDateString();
                            else
                                objICELeadIntake.ice_intakeclosedate = null;

                            objICELeadIntake.ice_ani = lead.ice_ani;
                            objICELeadIntake.ice_call_received_time = lead.ice_call_received_time.ToString("hh:mm tt");
                            objICELeadIntake.cap_zipofincident = lead.cap_zipofincident;
                            objICELeadIntake.ice_transcriptstatus = lead.ice_transcriptstatus;
                            objICELeadIntake.cap_judgesdecision = lead.cap_judgesdecision;
                            objICELeadIntake.cap_dateofapplication = lead.cap_dateofapplication.ToString();
                            objICELeadIntake.cap_dateinitiallydenied = lead.cap_dateinitiallydenied.ToString();
                            objICELeadIntake.cap_dateappealed = lead.cap_dateappealed.ToString();
                            objICELeadIntake.cap_dateappealeddenied = lead.cap_dateappealeddenied.ToString();
                            objICELeadIntake.cap_datesecondappeal = lead.cap_datesecondappeal.ToString();
                            objICELeadIntake.cap_dateofhearing = lead.cap_dateofhearing.ToString();
                            objICELeadIntake.cap_diagnosespreventingwork = lead.cap_diagnosespreventingwork;
                            objICELeadIntake.cap_statementofdisability = lead.cap_statementofdisability;
                            objICELeadIntake.cap_degree = lead.cap_degree;
                            objICELeadIntake.cap_whydidyoustopworking = lead.cap_whydidyoustopworking;
                            objICELeadIntake.cap_worksincedisabled = lead.cap_worksincedisabled;
                            objICELeadIntake.cap_dateoflastdoctorvisit = lead.cap_dateoflastdoctorvisit.ToString();
                            objICELeadIntake.cap_expectoutofwork1year = lead.cap_expectoutofwork1year;
                            objICELeadIntake.cap_ssattorneylast90days = lead.cap_ssattorneylast90days;
                            objICELeadIntake.cap_otherfirmstillrepresenting = lead.cap_otherfirmstillrepresenting;
                            objICELeadIntake.cap_militaryveteran = lead.cap_militaryveteran;
                            objICELeadIntake.cap_osveteranrating = lead.cap_osveteranrating;
                            objICELeadIntake.cap_uscitizen = lead.cap_uscitizen;
                            objICELeadIntake.cap_havevalidgreencard = lead.cap_havevalidgreencard;
                            objICELeadIntake.cap_osprivatedisability = lead.cap_osprivatedisability;
                            objICELeadIntake.cap_currentlyreceivingssbenefits = lead.cap_currentlyreceivingssbenefits;
                            objICELeadIntake.cap_osalidenial = lead.cap_osalidenial;
                            objICELeadIntake.cap_currentclaimpending = lead.cap_currentclaimpending;
                            objICELeadIntake.cap_statusofclaim = lead.cap_statusofclaim;
                            objICELeadIntake.cap_didyouhaveahearing = lead.cap_didyouhaveahearing;
                            objICELeadIntake.cap_conditionslist = lead.cap_conditionslist;
                            objICELeadIntake.cap_deceasedspouselast7years = lead.cap_deceasedspouselast7years;
                            objICELeadIntake.cap_marriagelasted10years = lead.cap_marriagelasted10years;
                            objICELeadIntake.cap_deceasedworked5ofthelast10years = lead.cap_deceasedworked5ofthelast10years;
                            objICELeadIntake.cap_taxwithholding = lead.cap_taxwithholding;
                            objICELeadIntake.cap_fulltimejoblast5years = lead.cap_fulltimejoblast5years;
                            objICELeadIntake.cap_worked5ofthelast10years = lead.cap_worked5ofthelast10years;
                            objICELeadIntake.cap_stillemployed = lead.cap_stillemployed;
                            objICELeadIntake.cap_hourlywage = lead.cap_hourlywage;
                            objICELeadIntake.cap_monthlywage = lead.cap_monthlywage;
                            objICELeadIntake.cap_hoursperweek = lead.cap_hoursperweek;
                            objICELeadIntake.cap_hourspermonth = lead.cap_hourspermonth;
                            objICELeadIntake.cap_lastworked25hoursperweek = lead.cap_lastworked25hoursperweek;
                            objICELeadIntake.cap_ostreatment = lead.cap_ostreatment;
                            objICELeadIntake.cap_osnarcoticpainmed = lead.cap_osnarcoticpainmed;
                            objICELeadIntake.cap_osoxygen = lead.cap_osoxygen;
                            objICELeadIntake.cap_oscanewalkerwheelchair = lead.cap_oscanewalkerwheelchair;
                            objICELeadIntake.cap_oshandicaplacard = lead.cap_oshandicaplacard;
                            objICELeadIntake.cap_osjail6months = lead.cap_osjail6months;
                            objICELeadIntake.cap_currentlyattendingschool = lead.cap_currentlyattendingschool;
                            objICELeadIntake.cap_credits = lead.cap_credits;
                            objICELeadIntake.cap_oseducation = lead.cap_oseducation;
                            objICELeadIntake.cap_illicitdrugsinthepastyear = lead.cap_illicitdrugsinthepastyear;
                            objICELeadIntake.cap_sobermorethan90days = lead.cap_sobermorethan90days;
                            objICELeadIntake.cap_addressvalidated = lead.cap_addressvalidated;
                            objICELeadIntake.cap_conditionsscoring = lead.cap_conditionsscoring;
                            objICELeadIntake.cap_score = lead.cap_score;
                            objICELeadIntake.ice_leaddetails2 = lead.ice_leaddetails2;
                            objICELeadIntake.ice_contactsource = lead.ice_contactsource;
                            objICELeadIntake.salutation = lead.salutation;
                            objICELeadIntake.suffix = lead.suffix;
                            objICELeadIntake.cap_leadid = lead.cap_leadid;
                            objICELeadIntake.ice_signuptype = lead.ice_signuptype;
                            objICELeadIntake.cap_intakestatusreason = lead.cap_intakestatusreason;
                            objICELeadIntake.address1_line2 = lead.address1_line2;
                            objICELeadIntake.familystatuscode = lead.familystatuscode;
                            objICELeadIntake.spousesname = lead.spousesname;
                            objICELeadIntake.cap_agecalculatedfield = lead.cap_agecalculatedfield;
                            objICELeadIntake.cap_emergencycontactname = lead.cap_emergencycontactname;
                            objICELeadIntake.cap_alternateaddress = lead.cap_alternateaddress;
                            objICELeadIntake.cap_emergencycontactphone = lead.cap_emergencycontactphone;
                            objICELeadIntake.cap_sameaddressasapplicant = lead.cap_sameaddressasapplicant;
                            objICELeadIntake.cap_relationshiptoapplicant = lead.cap_relationshiptoapplicant;
                            objICELeadIntake.cap_ecspeaksenglish = lead.cap_ecspeaksenglish;
                            objICELeadIntake.cap_mmn = lead.cap_mmn;
                            objICELeadIntake.cap_fathersname = lead.cap_fathersname;
                            objICELeadIntake.cap_placeofbirth = lead.cap_placeofbirth;
                            objICELeadIntake.cap_othernames = lead.cap_othernames;
                            objICELeadIntake.ice_lastfollowupdate = lead.ice_lastfollowupdate.ToString();
                            objICELeadIntake.ice_nextfollowupdate = lead.ice_nextfollowupdate.ToString();
                            objICELeadIntake.cap_leadfollowuptimeout = lead.cap_leadfollowuptimeout;
                            objICELeadIntake.cap_nextcommunicationdate = lead.cap_nextcommunicationdate.ToString();
                            objICELeadIntake.cap_lastcommunicationdate = lead.cap_lastcommunicationdate.ToString();
                            objICELeadIntake.cap_dripattempt = lead.cap_dripattempt;
                            objICELeadIntake.cap_leaddripstatus = lead.cap_leaddripstatus;
                            objICELeadIntake.cap_leaddriptimeout = lead.cap_leaddriptimeout;
                            objICELeadIntake.cap_communitcationcounter = lead.cap_communitcationcounter;
                            objICELeadIntake.cap_calltrackingid = lead.cap_calltrackingid;
                            objICELeadIntake.cap_phonesourceid = lead.cap_phonesourceid;
                        objICELeadIntake.cap_medications = lead.cap_medications;
                        objICELeadIntake.jobtitle = lead.jobtitle;
                        objICELeadIntake.cap_duplicateuserid = lead.cap_duplicateuserid;
                        objICELeadIntake.cap_duplicatedate = lead.cap_duplicatedate.ToString();
                        objICELeadIntake.cap_contactinguserid = lead.cap_contactinguserid;
                        objICELeadIntake.cap_contactingdate = lead.cap_contactingdate.ToString();
                        objICELeadIntake.cap_contacteduserid = lead.cap_contacteduserid;
                        objICELeadIntake.cap_contacteddate = lead.cap_contacteddate.ToString();
                        objICELeadIntake.cap_cantcontactuserid = lead.cap_cantcontactuserid;
                        objICELeadIntake.cap_cantcontactdate = lead.cap_cantcontactdate.ToString();
                        objICELeadIntake.cap_clientrefusedhelpuserid = lead.cap_clientrefusedhelpuserid;
                        objICELeadIntake.cap_clientrefusedhelpdate = lead.cap_clientrefusedhelpdate.ToString();
                        objICELeadIntake.cap_intakecompleteduserid = lead.cap_intakecompleteduserid;
                        objICELeadIntake.cap_intakecompleteddate = lead.cap_intakecompleteddate.ToString();
                        objICELeadIntake.cap_casewanteduserid = lead.cap_casewanteduserid;
                        objICELeadIntake.cap_casewanteddate = lead.cap_casewanteddate.ToString();
                        objICELeadIntake.cap_caserejecteduserid = lead.cap_caserejecteduserid;
                        objICELeadIntake.cap_caserejecteddate = lead.cap_caserejecteddate.ToString();
                        objICELeadIntake.cap_clientterminateduserid = lead.cap_clientterminateduserid;
                        objICELeadIntake.cap_clientterminateddate = lead.cap_clientterminateddate.ToString();
                        objICELeadIntake.cap_disqualifieduserid = lead.cap_disqualifieduserid;
                        objICELeadIntake.cap_disqualifieddate = lead.cap_disqualifieddate.ToString();
                        objICELeadIntake.cap_retaineduserid = lead.cap_retaineduserid;
                        objICELeadIntake.cap_retaineddate = lead.cap_retaineddate.ToString();
                        objICELeadIntake.cap_intakereviewerid = lead.cap_intakereviewerid;
                        objICELeadIntake.cap_intakepackageid = lead.cap_intakepackageid;
                        objICELeadIntake.cap_intakecloserid = lead.cap_intakecloserid;
                        objICELeadIntake.cap_synccmuserid = lead.cap_synccmuserid;
                        objICELeadIntake.ice_syncdatecm = lead.ice_syncdatecm.ToString();
                        objICELeadIntake.cap_fcqsentuserid = lead.cap_fcqsentuserid;
                        objICELeadIntake.cap_fcqsentdate = lead.cap_fcqsentdate.ToString();
                        objICELeadIntake.cap_fcqreceiveduserid = lead.cap_fcqreceiveduserid;
                        objICELeadIntake.cap_fcqreceiveddate = lead.cap_fcqreceiveddate.ToString();
                        objICELeadIntake.cap_auditrejectionconfirmeduserid = lead.cap_auditrejectionconfirmeduserid;
                        objICELeadIntake.cap_auditrejectionconfirmeddate = lead.cap_auditrejectionconfirmeddate.ToString();
                        objICELeadIntake.cap_overrideintakeneededuserid = lead.cap_overrideintakeneededuserid;
                        objICELeadIntake.cap_overrideintakeneededdate = lead.cap_overrideintakeneededdate.ToString();
                        objICELeadIntake.cap_ssaappcompleteduserid = lead.cap_ssaappcompleteduserid;
                        objICELeadIntake.cap_ssaappcompleteddate = lead.cap_ssaappcompleteddate.ToString();
                        objICELeadIntake.cap_1696fileduserid = lead.cap_1696fileduserid;
                        objICELeadIntake.cap_1696fileddate = lead.cap_1696fileddate.ToString();
                        objICELeadIntake.cap_reviewedforpasuserid = lead.cap_reviewedforpasuserid;
                        objICELeadIntake.cap_reviewedforpasdate = lead.cap_reviewedforpasdate.ToString();
                        objICELeadIntake.cap_reviewedforpasappuserid = lead.cap_reviewedforpasappuserid;
                        objICELeadIntake.cap_reviewedforpasappdate = lead.cap_reviewedforpasappdate.ToString();
                        objICELeadIntake.cap_droppedcaseuserid = lead.cap_droppedcaseuserid;
                        objICELeadIntake.cap_droppedcasedate = lead.cap_droppedcasedate.ToString();

                       
                        lstICELeadIntake.Add(objICELeadIntake);

                        }
                    }
                    else
                    {
                        //no contact
                    }
                }
               
                return lstICELeadIntake;

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
            //try
            //{             

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
                    JsonPosting["ice_name"] = contactId.ToString() + "_Create_LeadIntake";
                    JsonPosting["ice_jsonstatus"] = false;
                    JsonPosting["ice_response"] = response;
                    JsonPosting["ice_payload"] = payload;
                    CrmOperationService objCrmOperationsService = new CrmOperationService();
                    objCrmOperationsService.createRecord(JsonPosting, crmSvcClient);
                    _log.Info("JSON Posting created successfully for {0} in ICE", FirstName + " " + LastName);
                                       

                }
            //}
            //catch (Exception ex)
            //{
            //    _log.Fatal("Exception {0}", ex.Message + Environment.NewLine + ex.InnerException.Message);                
            //    throw ex;
            //}
        }
    }
}
