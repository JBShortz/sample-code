<!---AdminSite emailUtil--->
<!---SAMPLE CODE, SOME CODE REMOVED--->
<cfcomponent displayname="Email Component" name="emailUtil" output="false">
	
	<!--- call init() automatically when the CFC is instantiated --->
  	<cfset init()>

	<cffunction name="init" access="public" output="false" returntype="emailUtil">
		
		<!--- Putting the variable in the variables scope makes it available to the init() method as well as all other methods in the CFC--->
		<cfset variables.logErrors = true>
        
		<cfset variables.emailServer = ''>
		<cfset variables.emailServerPort = ''>
        <cfset variables.emailFailTo = ''>
        
        <cfset variables.credsStruct = structNew()>
        
		<cfset variables.credsStructRecord = structNew()>        
        <cfset variables.credsStructRecord["username"] = ''>
        <cfset variables.credsStructRecord["password"] = ''>
        <cfset StructInsert(variables.credsStruct, 'name', variables.credsStructRecord)>

          <!---Valid File Types for attachments--->
        <cfset variables.validFileTypeStruct = structNew()>
        <cfset variables.validFileTypeStruct['csv'] = "text/csv">
        <cfset variables.validFileTypeStruct['txt'] = "text/plain">
        <cfset variables.validFileTypeStruct['doc'] = "application/msword">
        <cfset variables.validFileTypeStruct['docx'] = "application/msword">
        <cfset variables.validFileTypeStruct['pdf'] = "application/pdf">
        <cfset variables.validFileTypeStruct['xls'] = "application/excel">
        <cfset variables.validFileTypeStruct['xlsx'] = "application/excel">
        <cfset variables.validFileTypeStruct['jpg'] = "image/jpeg">
        <cfset variables.validFileTypeStruct['jpeg'] = "image/jpeg">
        <cfset variables.validFileTypeStruct['png'] = "image/png">
        <cfset variables.validFileTypeStruct['gif'] = "image/gif">
       
		<cfreturn this />
	</cffunction>
    
    <!---send Email--->
    <cffunction name="sendEmail" displayname="Send Email" description="Builds and sends email" access="public" output="false" returntype="void">
        
        <cfargument name="EmailTo" type="string" required="yes" >
        <cfargument name="EmailFrom" type="string" required="yes">
        <cfargument name="EmailFromName" type="string" required="no" default="">
        <cfargument name="EmailReplyTo" type="string" required="no" default="">
        <cfargument name="EmailReplyToName" type="string" required="no" default="">
        <cfargument name="EmailSubject" type="string" required="yes">
        <cfargument name="EmailMessage" type="string" required="yes">
        <cfargument name="EmailWhiteListed" type="boolean" required="no" default="true">
        <cfargument name="EmailParamsArray" type="array" required="no"  default="#ArrayNew(1)#">    
        
        <cfset local.username = ''>
        <cfset local.password = ''>
        <cfset local.replyTo = ''>
        <cfset local.from = ''>
        <cfset local.validEmailParamsStruct = {}>
       
       <!---Validate EmailTo; if EmailTo is invalid, log the address--->
        <cfif validateEmail(EmailAddress = arguments.EmailTo) EQ false>
			<cfset LogEmailError(errorText = "Email Address Invalid: #arguments.EmailTo#")>
			<cfif arguments.EmailWhiteListed NEQ true>
            	<cfreturn />
            </cfif>
        </cfif>
        
        <!---Validate EmailFrom; if EmailFrom is invalid, send email to with details--->
        <cfif validateEmail(EmailAddress = arguments.EmailFrom) EQ false>
        	<cfset sendErrorEmail(errorMessage = "Invalid 'from=' Email Address", variableToDump = arguments)>
            <cfif arguments.EmailWhiteListed NEQ true>
            	<cfreturn />
            </cfif>
        </cfif>
        
        <!---Validate EmailReplyTo; if EmailReplyTo is invalid, log error, set EmailReplyTo to blank and continue--->
        <cfif len(arguments.EmailReplyTo) NEQ 0>
			<cfif validateEmail(EmailAddress = arguments.EmailReplyTo) EQ false>
                <cfset LogEmailError(errorText = "Email ReplyTo Address Invalid: #arguments.EmailReplyTo#")>
                <cfset arguments.EmailReplyTo = ''>
            </cfif>
        </cfif>

        <!---Validate EmailParamsStruct --->
        <cfif NOT arrayIsEmpty(arguments.EmailParamsArray)>
            
            <cfset local.validEmailParamsStruct = validateEmailParams(EmailParamsArray = arguments.EmailParamsArray)>
            
            <!---if returned valid struct is empty, abort email and send error with EmailParamsArray--->
            <cfif structIsEmpty(local.validEmailParamsStruct)>
                <cfset sendErrorEmail(errorMessage = "EmailParams invalid", variableToDump = arguments.EmailParamsArray)>
                <cfreturn />
            </cfif>

        </cfif>   
        
        <!---get the credentials--->
        <cfif StructKeyExists(variables.credsStruct,arguments.EmailFrom)>
			<cfset local.username = variables.credsStruct[arguments.EmailFrom].username>
            <cfset local.password = variables.credsStruct[arguments.EmailFrom].password>
        <cfelse>
        	<cfset sendErrorEmail(errorMessage = "Username does not exist in credentials structure", variableToDump = arguments)>
           	<cfreturn />
        </cfif>
        
        <!---set from --->
        <cfif len(arguments.EmailFromName) NEQ 0>
        	<cfset local.from = '"#arguments.EmailFromName#" <#arguments.EmailFrom#>'>
        <cfelse>
        	<cfset local.from = arguments.EmailFrom>
        </cfif>
                
        <!---set replyTo, if EmailReplyTo arg is blank, set to EmailFrom--->
        <cfif len(arguments.EmailReplyTo) NEQ 0>
        	<cfif len(arguments.EmailReplyToName) NEQ 0>
        		<cfset local.replyTo = '"#arguments.EmailReplyToName#" <#arguments.EmailReplyTo#>'>
            <cfelse>
            	<cfset local.replyTo = arguments.EmailReplyTo>
            </cfif>
        <cfelse>
        	<cfset local.replyTo = arguments.EmailFrom>
        </cfif>
        
        <!---set failto, if failto variable is blank set to EmailFrom--->
        <cfif len(variables.emailFailTo)>
        	<cfset local.failto = variables.emailFailTo>
        <cfelse>
        	<cfset local.failto = arguments.EmailFrom>
        </cfif>
        
        <cftry>
			 <!---Send the email--->
             <cfmail failto="#local.failto#" from="#local.from#" replyto="#local.replyTo#" to="#arguments.EmailTo#" subject="#arguments.EmailSubject#" server="#variables.emailServer#" port="#variables.emailServerPort#" username="#local.username#" password="#local.password#" spoolenable="yes" type="text/html">
                <cfif NOT structIsEmpty(local.validEmailParamsStruct)>
                    <cfloop collection="#local.validEmailParamsStruct#" item="local.file">
                        <cfmailparam file="#local.file#" type="#local.validEmailParamsStruct[local.file]#">
                    </cfloop> 
                </cfif> 
              
                <cfmailpart type="text/plain">
                #ReReplace(arguments.EmailMessage,"(<[^>]{1,}>|\t)","","all")#
                </cfmailpart>
                
                <cfmailpart type="text/html">
                <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
                <html>
                <head>
                <meta content="en-us" http-equiv="Content-Language">
                <meta content="text/html; charset=utf-8" http-equiv="Content-Type">
                <title>#arguments.EmailSubject#</title>
                </head>
                <body>
                #trim(arguments.EmailMessage)#
                </body>
				</html>
                </cfmailpart>  
                                
             </cfmail>
             
             <cfcatch type="any">
             	<cfset sendErrorEmail(errorMessage = "cfmail exception caught", variableToDump = cfcatch)>
             </cfcatch>
         </cftry>
         
        <cfreturn />
    </cffunction>
    <!---end send Email--->
    
    <!---Validates an email address--->
    <cffunction name="validateEmail" displayname="Validate Email Address" description="Validates To and From Emails before they are sent" access="private" output="false" returntype="boolean">
    
    		<cfargument name="EmailAddress" type="string" required="yes">
            
            <cfset local.validated = false>
            <cfset local.qry_checkEmailValidity = ''>
    	            
            <!---check form valid email address--->
            <cfif isEmail(str = arguments.EmailAddress) EQ false>
            	<cfreturn local.validated>
            </cfif>
            
			<!---make sure that it is not a bounced email address--->
            <!---REMOVED CFQUERY DB CHECK--->

            <cfif local.qry_checkEmailValidity.recordcount EQ 0>
            
            	<cfset local.validated = true>
           	
			<!---existing record--->
            <cfelse>
            
               <!---REMOVED CFQUERY INSERT--->
                
            </cfif>
            
		<cfreturn local.validated>
    </cffunction>
    <!---end validateEmail--->

    <!---Validates an email params struct--->
    <cffunction name="validateEmailParams" displayname="Validate Email Params" description="Validates the email params struct" access="private" output="false" returntype="struct">
    
            <cfargument name="EmailParamsArray" type="array" required="yes">
            
            <cfset local.validEmailParamsStruct = {}>
            <cfset local.file = ''>
            <cfset local.fileExt = ''>

        
        <cfloop array="#arguments.EmailParamsArray#" index="local.file">

            <!---check if file exists--->
            <cfif NOT fileExists(local.file)>
                <cfreturn {} />
            </cfif>

            <!---get the file extension--->
            <cfset local.fileExt = listLast(local.file, '.')>

            <cfif structKeyExists(variables.validFileTypeStruct, local.fileExt)>
                    <cfset local.validEmailParamsStruct['#local.file#'] = "#variables.validFileTypeStruct[local.fileExt]#">
                <cfelse>
                    <cfreturn {} />
            </cfif>
        
        </cfloop>

        <cfreturn local.validEmailParamsStruct>
    </cffunction>
    <!---end validateEmail--->
    
    <!---email validation script--->
    <cfscript>
	/**
	* Tests passed value to see if it is a valid e-mail address (supports subdomain nesting and new top-level domains).
	* Update by David Kearns to support '
	* SBrown@xacting.com pointing out regex still wasn't accepting ' correctly.
	* Should support + gmail style addresses now.
	* More TLDs
	* Version 4 by P Farrel, supports limits on u/h
	* Added mobi
	* v6 more tlds
	* 
	* @param str      The string to check. (Required)
	* @return Returns a boolean. 
	* @author Jeff Guillaume (SBrown@xacting.comjeff@kazoomis.com) 
	* @version 7, May 8, 2009 
	*/
	function isEmail(str) {
	return REFindNoCase("^['_a-z0-9-\+]+(\.['_a-z0-9-\+]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.(([a-z]{2,3})|(aero|asia|biz|cat|coop|info|museum|name|jobs|post|pro|tel|travel|mobi))$",
	arguments.str) AND len(listGetAt(arguments.str, 1, "@")) LTE 64 AND
	len(listGetAt(arguments.str, 2, "@") LTE 255) EQ 2;
	}
	</cfscript>
    
    <!---sends Error Email--->
 	<cffunction name="sendErrorEmail" displayname="Send Error Email" description="If an email would fail to send, send us an email with details." access="private" output="false" returntype="void">
    
        <cfargument name="errorMessage" type="string" required="yes">
        <cfargument name="variableToDump" type="any" required="no" default="">
        
        <cfmail to="" replyto="" from=""
         subject="Email Utilities Error" server="#variables.emailServer#" port="#variables.emailServerPort#" username="" password="" spoolenable="yes" type="text/html">
            <p>
                An error has occured while trying to send an email.<br/>
                Error Message: #arguments.errorMessage#<br/>
                CredsStruct Key List: <cfdump var="#StructKeyList(variables.credsStruct)#"> <br />
                CGI: <cfdump var="#cgi#"><br/>
                Variable to Dump : <cfdump var="#variableToDump#">
            </p>         
        </cfmail>
        
    </cffunction>
    
    <!---Log Email Util Errors--->
    <cffunction name="LogEmailError" displayname="Log Email Error" description="Log certain Errors related to sending emails" access="private" output="false" returntype="void">
    	
        <cfargument name="errorText" type="string" required="yes">
        	
            <!---log error if logErrors flag is set to true, else do nothing--->
        	<cfif variables.logErrors NEQ false>
            	 <cflog file="emailUtilErrors" type="error" text="#arguments.errorText#">    
            </cfif>
        
    	<cfreturn />
    </cffunction>
    
</cfcomponent>