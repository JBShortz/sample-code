using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeCore
{
    public class ICELeadIntake
	{
		public string parentcustomerid { get; set; }
		public Guid contactid { get; set; }
		public string ice_language { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string emailaddress1 { get; set; }
		public string mobilephone { get; set; }
		public string address1_telephone1 { get; set; }
		public string address1_line1 { get; set; }
		public string address1_city { get; set; }
		public string address1_stateorprovince { get; set; }
		public string address1_postalcode { get; set; }
		public string address1_county { get; set; }
		public string address1_country { get; set; }
		public string birthdate { get; set; }
		public string gendercode { get; set; }
		public string ice_besttimereach { get; set; }
		public string ice_injureddifferent { get; set; }
		public string ice_legalrepresentative { get; set; }
		public string ice_clientrelationship { get; set; }
		public string ice_clientfirstname { get; set; }
		public string ice_clientlastname { get; set; }
		public string address2_telephone1 { get; set; }
		public string emailaddress2 { get; set; }
		public string ice_clientdateofbirth { get; set; }
		public bool ice_clientminor { get; set; }
		public bool ice_clientdeceased { get; set; }
		public string ice_clientdateofdeath { get; set; }
		public string ice_leaddetails { get; set; }
		public string description { get; set; }
		public string ice_casetypeid { get; set; }
		public bool ice_rundialog { get; set; }
		public string ice_intakedate { get; set; }
		public string ownerid { get; set; }
		public int lead_attempts { get; set; }
		public bool update_requested { get; set; }
		public bool retained_by_phone { get; set; }
		public string retained_by { get; set; }
		public string ice_dnis { get; set; }
		public string ice_ani { get; set; }
		public string ice_call_received_time { get; set; }
		public string ice_marketingsourcedetails { get; set; }
		public string ice_internalnotes { get; set; }
		public string ice_vendorid { get; set; }
		public string ice_disqualificationreason { get; set; }
		public string ice_confidencelevel { get; set; }
		public string ice_leadgrade { get; set; }
		public int ice_leadscore { get; set; }
		public string ice_intakestatus { get; set; }
        public string ice_signpackagedate { get; set; }
		public string ice_internalreviewdate { get; set; }
		public string ice_intakeclosedate { get; set; }
        public int campaign_id { get; set; } //cap_nolocampaignid
        public int practice_area_id { get; set; } //cap_nolopracticeareaid - related casetype record
        public int account_id { get; set; } //address3_primarycontactname
        public string cap_zipofincident { get; set; }
        public string ice_transcriptstatus { get; set; }
        public List<QuestionAnswer> question_set { get; set; }
		public string cap_judgesdecision { get; set; }
		public string cap_dateofapplication { get; set; }
		public string cap_dateinitiallydenied { get; set; }
		public string cap_dateappealed { get; set; }
		public string cap_dateappealeddenied { get; set; }
		public string cap_datesecondappeal { get; set; }
		public string cap_dateofhearing { get; set; }
		public string cap_diagnosespreventingwork { get; set; }
		public string cap_statementofdisability { get; set; }
		public string cap_degree { get; set; }
		public string cap_whydidyoustopworking { get; set; }
		public string cap_worksincedisabled { get; set; }
		public string cap_dateoflastdoctorvisit { get; set; }
		public string cap_expectoutofwork1year { get; set; }
		public string cap_ssattorneylast90days { get; set; }
		public string cap_otherfirmstillrepresenting { get; set; }
		public string cap_militaryveteran { get; set; }
		public string cap_osveteranrating { get; set; }
		public string cap_uscitizen { get; set; }
		public string cap_havevalidgreencard { get; set; }
		public string cap_osprivatedisability { get; set; }
		public string cap_currentlyreceivingssbenefits { get; set; }
		public string cap_osalidenial { get; set; }
		public string cap_currentclaimpending { get; set; }
		public string cap_statusofclaim { get; set; }
		public string cap_didyouhaveahearing { get; set; }
		public string cap_conditionslist { get; set; }
		public string cap_deceasedspouselast7years { get; set; }
		public string cap_marriagelasted10years { get; set; }
		public string cap_deceasedworked5ofthelast10years { get; set; }
		public string cap_taxwithholding { get; set; }
		public string cap_fulltimejoblast5years { get; set; }
		public string cap_worked5ofthelast10years { get; set; }
		public string cap_stillemployed { get; set; }
		public int cap_hourlywage { get; set; }
		public int cap_monthlywage { get; set; }
		public int cap_hoursperweek { get; set; }
		public int cap_hourspermonth { get; set; }
		public string cap_lastworked25hoursperweek { get; set; }
		public string cap_ostreatment { get; set; }
		public string cap_osnarcoticpainmed { get; set; }
		public string cap_osoxygen { get; set; }
		public string cap_oscanewalkerwheelchair { get; set; }
		public string cap_oshandicaplacard { get; set; }
		public string cap_osjail6months { get; set; }
		public string cap_currentlyattendingschool { get; set; }
		public string cap_credits { get; set; }
		public string cap_oseducation { get; set; }
		public string cap_illicitdrugsinthepastyear { get; set; }
		public string cap_sobermorethan90days { get; set; }
		public bool cap_addressvalidated { get; set; }
		public string cap_conditionsscoring { get; set; }
		public string cap_score { get; set; }
		public string ice_leaddetails2 { get; set; }
		public string ice_contactsource { get; set; }
		public string salutation { get; set; }
		public string suffix { get; set; }
		public string cap_leadid { get; set; }
		public string ice_signuptype { get; set; }
		public string cap_intakestatusreason { get; set; }
		public string address1_line2 { get; set; }
		public string familystatuscode { get; set; }
		public string spousesname { get; set; }
		public string cap_agecalculatedfield { get; set; }
		public string cap_emergencycontactname { get; set; }
		public string cap_alternateaddress { get; set; }
		public string cap_emergencycontactphone { get; set; }
		public string cap_sameaddressasapplicant { get; set; }
		public string cap_relationshiptoapplicant { get; set; }
		public string cap_ecspeaksenglish { get; set; }
		public string cap_mmn { get; set; }
		public string cap_fathersname { get; set; }
		public string cap_placeofbirth { get; set; }
		public string cap_othernames { get; set; }
		public string ice_lastfollowupdate { get; set; }
		public string ice_nextfollowupdate { get; set; }
		public string cap_leadfollowuptimeout { get; set; }
		public string cap_nextcommunicationdate { get; set; }
		public string cap_lastcommunicationdate { get; set; }
		public int cap_dripattempt { get; set; }
		public string cap_leaddripstatus { get; set; }
		public string cap_leaddriptimeout { get; set; }
		public int cap_communitcationcounter { get; set; }
		public string cap_calltrackingid { get; set; }
		public string cap_phonesourceid { get; set; }
		public string cap_medications { get; set; }
		public string jobtitle { get; set; }
		public string cap_duplicateuserid { get; set; }
		public string cap_duplicatedate { get; set; }
		public string cap_contactinguserid { get; set; }
		public string cap_contactingdate { get; set; }
		public string cap_contacteduserid { get; set; }
		public string cap_contacteddate { get; set; }
		public string cap_cantcontactuserid { get; set; }
		public string cap_cantcontactdate { get; set; }
		public string cap_clientrefusedhelpuserid { get; set; }
		public string cap_clientrefusedhelpdate { get; set; }
		public string cap_intakecompleteduserid { get; set; }
		public string cap_intakecompleteddate { get; set; }
		public string cap_casewanteduserid { get; set; }
		public string cap_casewanteddate { get; set; }
		public string cap_caserejecteduserid { get; set; }
		public string cap_caserejecteddate { get; set; }
		public string cap_clientterminateduserid { get; set; }
		public string cap_clientterminateddate { get; set; }
		public string cap_disqualifieduserid { get; set; }
		public string cap_disqualifieddate { get; set; }
		public string cap_retaineduserid { get; set; }
		public string cap_retaineddate { get; set; }
		public string cap_intakereviewerid { get; set; }
		public string cap_intakepackageid { get; set; }
		public string ice_signupackagedate { get; set; }
		public string cap_intakecloserid { get; set; }
		public string cap_synccmuserid { get; set; }
		public string ice_syncdatecm { get; set; }
		public string cap_fcqsentuserid { get; set; }
		public string cap_fcqsentdate { get; set; }
		public string cap_fcqreceiveduserid { get; set; }
		public string cap_fcqreceiveddate { get; set; }
		public string cap_auditrejectionconfirmeduserid { get; set; }
		public string cap_auditrejectionconfirmeddate { get; set; }
		public string cap_overrideintakeneededuserid { get; set; }
		public string cap_overrideintakeneededdate { get; set; }
		public string cap_ssaappcompleteduserid { get; set; }
		public string cap_ssaappcompleteddate { get; set; }
		public string cap_1696fileduserid { get; set; }
		public string cap_1696fileddate { get; set; }
		public string cap_reviewedforpasuserid { get; set; }
		public string cap_reviewedforpasdate { get; set; }
		public string cap_reviewedforpasappuserid { get; set; }
		public string cap_reviewedforpasappdate { get; set; }
		public string cap_droppedcaseuserid { get; set; }
		public string cap_droppedcasedate { get; set; }


	}

 





}

