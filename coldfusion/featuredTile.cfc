<cfcomponent output="false" name="featuredTile">

	<cffunction name="init" access="public" output="false" returntype="featuredTile">

		<cfargument name="captionHeading" type="string" default="" />
		<cfargument name="captionText" type="string" default="" />
		<cfargument name="link" type="string" default="" />
		<cfargument name="backgroundImage" type="string" default="" />
		<cfargument name="size" type="string" default="medium" />

		<!---set default values structure--->
		<cfset variables.defaults = structNew()>
		<cfset variables.defaults["captionHeading"] = arguments.captionHeading>
		<cfset variables.defaults["captionText"] = arguments.captionText>
		<cfset variables.defaults["link"] = arguments.link>
		<cfset variables.defaults["backgroundImage"] = arguments.backgroundImage>
		<cfset variables.defaults["size"] = arguments.size>

		<cfreturn this />
	</cffunction>

	<cffunction name="execute" displayname="Execute" access="public" output="false" returntype="string" hint="I am the required function which takes in all the shortcode attributes and ultimately returns the processed content.">

		<cfargument name="attrs" type="struct" required="true" />
		<cfargument name="content" type="string" required="true" />
		<cfargument name="tagname" type="string" required="true" />
		<cfargument name="constrainAttributes" type="any" required="true" />

		<cfset local.returnContent = ''>

		<!---this step is required. We must combine the user defined attributes with the default attributes--->
		<cfset arguments.attrs = constrainAttributes(variables.defaults, arguments.attrs) />


		<cfset arguments.attrs.captionHeading =  htmleditformat(trim(arguments.attrs.captionHeading))>
		<cfset arguments.attrs.captionText =  htmleditformat(trim(arguments.attrs.captionText))>

		<cfif NOT len(arguments.attrs.captionHeading) OR NOT len(arguments.attrs.captionText)>
			<cfreturn returnContent />
		</cfif>

		<cfif NOT validateURL(arguments.attrs.link)>
			<cfreturn returnContent />
		</cfif>

		<cfoutput>
            <cfsavecontent variable="returnContent">
				#transmogrify(size = returnValidSize(arguments.attrs.size), backgroundImage = arguments.attrs.backgroundImage, link = arguments.attrs.link, captionHeading = arguments.attrs.captionHeading, captionText = arguments.attrs.captionText)#
            </cfsavecontent>
    	</cfoutput>

		<cfreturn returnContent />
	</cffunction>

	<cffunction name="transmogrify" displayname="Transform, esp. in a surprising or magical manner." access="private" output="false" returntype="string" hint="I return site specific formatting for the data that has been validated">

		<cfargument name="captionHeading" type="string" required="true">
		<cfargument name="captionText" type="string" required="true">
		<cfargument name="size" type="string" required="true">
		<cfargument name="backgroundImage" type="string" required="true">
		<cfargument name="link" type="string" required="true">

		<cfset local.transmogrifiedData = ''>

		<cfoutput>
			<cfsavecontent variable="local.transmogrifiedData">
			<div class="newstile-wrap clearfix">
				<a href="#arguments.link#" class="#arguments.size# clearfix">
					<figure>
						<cfif validateImageURL(arguments.backgroundImage)>
						<img src="#arguments.backgroundImage#">
						</cfif>
						<figcaption>
							<h5>#arguments.captionHeading#</h5>
							<span>#arguments.captionText#</span>
						</figcaption>
					</figure>
				</a>
			</div>
			</cfsavecontent>
		</cfoutput>

		<cfreturn local.transmogrifiedData />
	</cffunction>


	<cffunction name="returnValidSize" description="Return Valid Size" access="private" output="false" returntype="string" hint="I make sure the size is not blank and is a valid choice">

		<!---arguments--->
		<cfargument name="size" type="string" required="yes">

		<cfset arguments.size = htmleditformat(trim(arguments.size))>

		<cfset var validStruct = structNew()>

		<!---create a structure of valid button types and their corressponding classes--->
		<cfset validStruct["small"] = 'small'>
		<cfset validStruct["medium"] = 'medium'>
		<cfset validStruct["large"] = 'large'>


		<!---if the button type passed in is found in the valid list--->
		<cfif structKeyExists(validStruct, arguments.size)>
			<!---replace the button type mask with the corressponding class--->
			<cfset arguments.size = validStruct[arguments.size]>
		<!---if the type is not valid, use blank--->
		<cfelse>
			<cfset arguments.size = variables.defaults.size>
		</cfif>

		<cfreturn arguments.size>
	</cffunction>

	<cffunction name="validateImageURL" description="Validate Image URL" access="private" output="false" returntype="boolean" hint="I return false if the image path is not a valid URL">

		<!---arguments--->
		<cfargument name="url" type="string" required="yes">

		<cfset var urlStruct = structNew()>

		<cfset arguments.url = trim(arguments.url)>

		<!---if its blank--->
		<cfif len(arguments.url) EQ 0>
			<cfreturn false>
		</cfif>

		<cfset urlStruct = advancedURLRegex(URL = arguments.url)>

		<!---URL not valid--->
		<cfif len(urlStruct.protocol) EQ 0 or len(urlStruct.domain) EQ 0 or len(urlStruct.domainEnding) EQ 0 or len(urlStruct.file) EQ 0 or len(urlStruct.fileType) EQ 0>
			<cfreturn false>
		</cfif>

		<!---Image type is not valid --->
		<cfif urlStruct.fileType NEQ '.jpg' AND urlStruct.fileType NEQ '.jpeg' AND urlStruct.fileType NEQ '.gif' AND urlStruct.fileType NEQ '.png' AND urlStruct.fileType NEQ '.local'>
			<cfreturn false>
		</cfif>

		<cfreturn true>
	</cffunction>

	<cffunction name="validateURL" description="Validate URL" access="private" output="false" returntype="boolean" hint="I return false if the path is not a valid URL">

		<!---arguments--->
		<cfargument name="url" type="string" required="yes">

		<cfset local.urlStruct = {}>

		<cfset arguments.url = trim(arguments.url)>

		<!---if its blank--->
		<cfif NOT len(arguments.url)>
			<cfreturn false>
		</cfif>

		<cfset local.urlStruct = advancedURLRegex(URL = arguments.url)>

		<!---URL not valid--->
		<cfif NOT len(urlStruct.protocol) OR NOT len(urlStruct.domain) OR NOT len(urlStruct.domainEnding)>
			<cfreturn false>
		</cfif>

		<cfreturn true>
	</cffunction>


	<!---Break down URL into pieces--->
	<cffunction name="advancedURLRegex" displayname="Adanced URL Regex" description="I break down a URL string into all of its components and return a structure." output="no" access="public" returntype="struct">

		<!--- Accepts a URL --->
		<cfargument name="URL" type="string" required="yes">

		<cfset var urlStructure = structNew()>
		<cfset var advancedURLRegex = ''>
		<cfset var initialURLStruct = ''>
		<cfset var i = ''>

		<!---initial, empty structure--->
		<cfset urlStructure["success"] = 0>
		<cfset urlStructure["URL"] = ''>
		<cfset urlStructure["protocol"] = ''>
		<cfset urlStructure["user"] = ''>
		<cfset urlStructure["password"] = ''>
		<cfset urlStructure["subdomain"] = ''>
		<cfset urlStructure["domain"] = ''>
		<cfset urlStructure["domainEnding"] = ''>
		<cfset urlStructure["path"] = ''>
		<cfset urlStructure["file"] = ''>
		<cfset urlStructure["fileType"] = ''>
		<cfset urlStructure["parameters"] = ''>

		<cfsavecontent variable="advancedURLRegex">([a-z0-9_\-]{1,5}:\/\/)?(([a-z0-9_\-]{1,}):([a-z0-9_\-]{1,})\@)?((www\.)|([a-z0-9_\-]{1,}\.)+)?([a-z0-9_\-]{3,})(\.[a-z]{2,4})(\/([a-z0-9_\-]{1,}\/)+)?([a-z0-9_\-/]{1,})?(\.[a-z]{2,})?(\?)?(((\&)?[a-z0-9_\-]{1,}(\=[a-z0-9_\-]{1,})?)+)?</cfsavecontent>

		<cftry>

		<!---a structure that holds two arrays, for the position and length of the capture groups--->
		<cfset initialURLStruct = REFindNoCase(advancedURLRegex,arguments.URL,1,'true')>

		<!---assuming we found anything--->
		<cfif not ArrayIsEmpty(initialURLStruct["pos"])>

			<!---lets loops to build our structure--->
			<cfloop index="i" from="1" to="#ArrayLen(initialURLStruct.pos)#">
				<cfif i EQ 1 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["URL"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 2 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["protocol"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 4 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["user"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 5 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["password"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 6 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["subdomain"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 9 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["domain"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 10 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["domainEnding"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 11 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["path"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 13 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["file"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 14 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["fileType"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelseif i EQ 16 and initialURLStruct.len[i] NEQ 0>
					<cfset urlStructure["parameters"] = Mid(arguments.URL, initialURLStruct.pos[i], initialURLStruct.len[i])>
				<cfelse>
				</cfif>
			</cfloop>

		</cfif>

		<cfcatch type="any">
		</cfcatch>
		</cftry>

		<cfset urlStructure["success"] = 1>

		<cfreturn urlStructure>

	</cffunction>

	<!---this is a REQUIRED function for custom content handling--->
	<cffunction name="customHandler" description="Custom Handler" access="public" output="false" returntype="struct" hint="I define the custom functions that this shortcode uses.">

		<cfset var customHandlers = structNew()>

		<cfset customHandlers["customRequiredAssets"] = 'customRequiredAssets'>

		<cfreturn customHandlers />
	</cffunction>

	<!---return required css and js--->
	<cffunction name="customRequiredAssets" description="Return Required Assets" access="public" output="false" returntype="struct" hint="I return a struct with arrays containing the required CSS and Javascript files for this shortcode to function properly.">

		<cfset var requiredAssetsStruct = structNew()>

		<cfset requiredAssetsStruct["css"] = []>
		<cfset requiredAssetsStruct["js"] = []>

		<cfset requiredAssetsStruct["css"][1] = '<link rel="stylesheet" href="/assets/css/shortcodes/featuredtile/featuredtile.css">'>

		<cfreturn requiredAssetsStruct>
	</cffunction>

	<cffunction name="reference" description="I am the required reference guide for this shortcode" access="remote" output="false" returntype="struct">

		<cfset var referenceStruct = structNew()>
		<cfset var description = ''>
		<cfset var argumentList = ''>
		<cfset var example = ''>

		<cfsavecontent variable="description">
		<p>The featureBoxes shortcode adds a captioned image with a hover effect that links to a featured story, blog post or news article.</p>
		</cfsavecontent>
		<cfsavecontent variable="argumentList">
		<ul>
			<li><strong>Caption Heading</strong>:
				<ul>
                	<li>blank (default)</li>
                    <li>A custom heading</li>
				</ul>
			</li>
			<li><strong>Caption Text</strong>:
				<ul>
                	<li>blank (default)</li>
                    <li>Custom text</li>
				</ul>
			</li>
			<li><strong>backgroundImage</strong>:
				<ul>
                	<li>blank (default)</li>
                    <li>An valid image URL</li>
				</ul>
			</li>
			<li><strong>link</strong>:
				<ul>
                	<li>blank (default)</li>
                    <li>A valid url</li>
				</ul>
			</li>
			<li><strong>size</strong>:
				<ul>
                	<li>medium (default)</li>
                    <li>small, medium or large</li>
				</ul>
			</li>
		</ul>
        </cfsavecontent>
		<cfsavecontent variable="example">
		NA
		</cfsavecontent>

		<cfset referenceStruct["description"] = description>
		<cfset referenceStruct["argumentList"] = argumentList>
		<cfset referenceStruct["example"] = example>

		<cfreturn referenceStruct>
	</cffunction>

</cfcomponent>