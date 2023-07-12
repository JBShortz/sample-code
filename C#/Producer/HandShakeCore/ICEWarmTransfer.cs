using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HandShakeCore
{
	public class ICEWarmTransfer
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




	}


}

