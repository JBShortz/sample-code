<cfset navItem = 2>
<cfset appNav = 1>

<!---form defaults--->
<cfparam name="form.startdate" type="any" default="">
<cfparam name="form.enddate" type="any" default="">
<cfparam name="form.InquiryCaseTypeID" type="any" default="-1">
<cfparam name="form.InquiryStatus" type="any" default="-1">
<cfparam name="form.InquiryReferringSource" type="any" default="-1">
<cfparam name="form.InquiryWebsite" type="any" default="-1">
<cfparam name="form.InquiryReferrerKW" type="any" default="-1">
<cfparam name="form.InquiryBranded" type="any" default="-1">

<!---REMOVED SEVERAL QUERIES FOR SAMPLE--->

<!DOCTYPE html>
<!--[if lt IE 7]>      <html class="no-js lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>         <html class="no-js lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>         <html class="no-js lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!--> <html class="no-js"> <!--<![endif]-->
<head>
  <meta name="robots" content="noindex, nofollow">
  <title></title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <cfinclude template="includes/global_prehead.cfm">
</head>

<body>

     <cfinclude template="includes/topnav.cfm">
     <cfinclude template="includes/lead_reports_nav.cfm">


  <div class="container">

    <div class="page-header">
      <h1>Leads By Source Type</h1>
    </div>

    <cfoutput>
	    <form name="searchLeadsBySourceType" id="search-criteria" action="lead_reports_sourcetype.cfm" method="post">
	        <div class="container">
	            <div class="panel panel-default">
	                <div class="panel-heading">
	                    <h3 class="panel-title">Filter</h3>
	                </div>
	                <div class="panel-body">
	                    <div class="row">
	                        <div class="col-lg-3 col-md-3">
	                            <div class="input-group">
	                                <span class="input-group-addon"><i class="icon-calendar"></i></span>
	                                <input type="text" name="startdate" id="startdate" class="form-control" value="#form.startdate#" placeholder="Start Date" />
	                            </div>
	                        </div>
	                        <div class="col-lg-3 col-md-3">
	                            <div class="input-group">
	                                <span class="input-group-addon"><i class="icon-calendar"></i></span>
	                                <input type="text" name="enddate" id="enddate" class="form-control" value="#form.enddate#" placeholder="End Date" />
	                            </div>
	                        </div>
	                        <div class="form-group col-lg-3 col-md-3">
	                            <select name="InquiryReferringSource" id="InquiryReferringSource" class="select2" placeholder="Referring Source">
	                                <option></option>
	                                <cfloop query="getReferringSources">
	                                    <option <cfif form.InquiryReferringSource EQ InquiryReferringSourceID>selected="selected"</cfif> value="#InquiryReferringSourceID#">#InquiryReferringSourceName#</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                        <div class="form-group col-lg-3 col-md-3">
	                            <select name="InquiryStatus" id="InquiryStatus" class="select2" placeholder="Status">
	                                <option></option>
	                                <cfloop query="getStatuses">
	                                    <option <cfif form.InquiryStatus EQ captorraStatusID>selected="selected"</cfif> value="#captorraStatusID#">#captorraStatusName#</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                    </div>
	                    <div class="row">
	                        <div class="col-lg-3 col-md-3 form-group">
	                            <select name="InquiryCaseTypeID" id="InquiryCaseTypeID" class="select2" placeholder="Case Type">
	                                <option></option>
	                                <cfloop query="getCaseTypes">
	                                    <option <cfif form.InquiryCaseTypeID EQ captorraCaseTypeID>selected="selected"</cfif> value="#captorraCaseTypeID#">#captorraCaseTypeName#</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                        <div class="col-lg-3 col-md-3 form-group">
	                            <select name="InquiryWebsite" id="InquiryWebsite" class="select2" placeholder="Website">
	                                <option></option>
	                                <cfloop query="getWebsites">
	                                    <option <cfif form.InquiryWebsite EQ inquiryWebsiteID>selected="selected"</cfif> value="#inquiryWebsiteID#">#inquiryWebsiteURL#</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                        <div class="form-group col-lg-3 col-md-3">
	                            <select name="InquiryReferrerKW" id="InquiryReferrerKW" class="select2" placeholder="Keyword">
	                                <option></option>
	                                <cfloop query="getReferrerKW">
	                                    <option <cfif form.InquiryReferrerKW EQ InquiryReferrerKWID>selected="selected"</cfif> value="#InquiryReferrerKWID#">#trim(left(InquiryReferrerKW,45))# (#theCount#)</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                        <div class="form-group col-lg-3 col-md-3">
	                            <select name="InquiryBranded" id="InquiryBranded" class="select2" placeholder="Branded">
	                                <option></option>
	                                <cfloop query="getBranded">
	                                    <option <cfif form.InquiryBranded EQ InquiryBrandedID>selected="selected"</cfif> value="#InquiryBrandedID#">#InquiryBrandedStatus#</option>
	                                </cfloop>
	                            </select>
	                        </div>
	                    </div>
	                </div>
	                <div class="panel-footer">
	                    <input id="submit" type="submit" name="submit" value="Search" class="btn btn-primary btn-sm">
	                    <input type="hidden" name="gosearch" value="1" />
	                </div>
	            </div>
	        </div>
	    </form>
	</cfoutput>

<cfif structKeyExists(FORM,'gosearch') AND FORM.gosearch EQ 1>

<!---REMOVED CFQUERY FOR SAMPLE--->

    <div class ="container">
      <div class="panel panel-default">
        <div class="panel-heading">
          <h3 class="panel-title">Results
         <!---  <form style="float:right;" name="generateReport" action="lead_reports_sourcetype.cfm" method="post">
              <input type="hidden" name="reportName" value="Leads By Source Type" />
              <input type="hidden" name="generateReport" value="1" />
              <input type="submit"  class="btn btn-info btn-xs"  value="Export to Excel" />
          </form> --->
          </h3>
        </div>
        <div class="panel-body">
          <table class="table table-bordered table-striped table-hover dataTable">
            <thead>
                  <tr>
                      <th>Source Type</th>
                      <th>Leads</th>

                  </tr>
              </thead>
              <!---footer for totals--->
            <tfoot>
              <tr>
                  <th colspan="1" style="text-align:right">Total:</th>
                  <th></th>
              </tr>
          </tfoot>
              <tbody>
                <cfoutput query="getleadsbysourcetype">
                  <tr>
                    <td>#InquirySourceTypeName#</td>
                    <td>#theCount#</td>
                  </tr>
                </cfoutput>
              </tbody>
          </table>
        </div>
      </div>
    </div>

 </cfif>

<div id="push"></div>

<cfinclude template="includes/global_js.cfm">
<script type="text/javascript">
$(document).ready(function(){
  $('.dataTable').DataTable({
    "order": [1,'desc'],
    "footerCallback": function ( row, data, start, end, display ) {
      var api = this.api();
      
      // Remove the formatting to get integer data for summation
      var intVal = function ( i ) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '')*1 :
            typeof i === 'number' ?
            i : 0;
      };
      

      for(var i = 1; i < 2; i++) {
        // Total over all pages
        var total = api
            .column( i )
            .data()
            .reduce( function (a, b) {
                return intVal(a) + intVal(b);
        });
          
        //Total over this page
        // var pageTotal = api
        //     .column( i, { page: 'current'} )
        //     .data()
        //     .reduce( function (a, b) {
        //         return intVal(a) + intVal(b);
        //     } );

        //Update footer
        $(api.column( i ).footer()).html(total);
        }
     }
  
  });
});
</script>
</body>
</html>
