<cfcomponent output="false" name="blogCategories">

  <cffunction name="init" access="public" output="false" returntype="blogCategories">

    <cfargument name="title" type="string" default="Blog Categories" />
    <cfargument name="pageID" type="numeric" default="-1" />

    <!---set default values structure--->
    <cfset variables.defaults = structNew()>
    <cfset variables.defaults["title"] = arguments.title>
    <cfset variables.defaults["pageID"] = arguments.pageID>
    <cfset variables.defaults["blogactive"] = application.blogactive>

    <cfreturn this />
  </cffunction>

  <cffunction name="execute" displayname="Execute" access="public" output="false" returntype="string" hint="I am the required function which takes in all the shortcode attributes and ultimately returns the processed content.">

    <cfargument name="attrs" type="struct" required="true" />
    <cfargument name="content" type="string" required="true" />
    <cfargument name="tagname" type="string" required="true" />
    <cfargument name="constrainAttributes" type="any" required="true" />

    <cfset local.returnContent = ''>
    <cfset local.qry_getBlogRootByPageID = ''>
    <cfset local.blogRootID = ''>
    <cfset local.blogRootFullURL = ''>
    <cfset local.qry_getBlogCategories = ''>


    <!---this step is required. We must combine the user defined attributes with the default attributes--->
    <cfset arguments.attrs = constrainAttributes(variables.defaults, arguments.attrs) />

    <cfset local.qry_getPageTemplateByPageID = getPageTemplateByPageID(pageID = arguments.attrs.pageID)>

    <cfif local.qry_getPageTemplateByPageID.recordcount AND arguments.attrs.blogactive EQ 1>

      <cfset local.blogRootID = (local.qry_getPageTemplateByPageID.pageTemplate EQ 3 ? local.qry_getPageTemplateByPageID.pageParentID : local.qry_getPageTemplateByPageID.pageID) />

      <cfset local.qry_getBlogRootByPageID = getBlogRootByPageID(pageID = local.blogRootID)>

      <cfset local.blogRootFullURL = '#application.fullUrl#/#local.qry_getBlogRootByPageID.pageParentURL##local.qry_getBlogRootByPageID.pageURL#'>

      <cfset local.qry_getBlogCategories = getBlogCategories(pageID = local.blogRootID)>

      <cfif local.qry_getBlogCategories.recordcount EQ 0>
        <cfreturn local.returnContent>
      </cfif>

      <cfoutput>
        <cfsavecontent variable="local.returnContent">
          #transmogrify(title = returnValidTitle(arguments.attrs.title), qry_getBlogCategories = local.qry_getBlogCategories, blogRootFullURL = local.blogRootFullURL)#
        </cfsavecontent>
      </cfoutput>

    </cfif>

    <cfreturn local.returnContent />

  </cffunction>

	<cffunction name="transmogrify" displayname="Transform, esp. in a surprising or magical manner." access="private" output="false" returntype="string" hint="I return site specific formatting for the data that has been validated">

		<cfargument name="title" type="string" required="true" />
		<cfargument name="qry_getBlogCategories" type="query" required="true">
    <cfargument name="blogRootFullURL" type="string" required="true" />

		<cfset local.transmogrifiedData = ''>

			<cfoutput>
				<cfsavecontent variable="local.transmogrifiedData">
					<div class="widgetWrap aside">
					<div class="title"><span>#arguments.title#</span></div>
					<div class="body">
						<ul class="childrenPageList blog-categories">
							<cfloop query="#arguments.qry_getBlogCategories#">
								<li><a href="#arguments.blogRootFullURL#/category/#blogPostCategoryURL#">#blogPostCategoryName#</a><span class="badge">#numberformat(theCount)#</span></li>
							</cfloop>
						</ul>
					</div>
					<div class="foot"></div>
				</div>
				</cfsavecontent>
			</cfoutput>

		<cfreturn local.transmogrifiedData />
	</cffunction>

  <cffunction name="getPageTemplateByPageID" description="Get the page template" access="private" output="false" returntype="query">

    <!---arguments--->
    <cfargument name="pageID" type="numeric" required="yes" hint="I am the ID of the blog root">

    <cfset local.qry_getPageTemplateByPageID = ''>

    <cfquery name="local.qry_getPageTemplateByPageID">
    SELECT TOP 1 pageID, pageTemplate, pageParentID
    FROM page
    WHERE pageID = <cfqueryparam cfsqltype="cf_sql_integer" value="#arguments.pageID#">
    AND pageStatus <> <cfqueryparam cfsqltype="cf_sql_integer" value="2">
    </cfquery>

    <cfreturn local.qry_getPageTemplateByPageID>
  </cffunction>

  <cffunction name="getBlogRootByPageID" description="Get Blog Root ID" access="private" output="false" returntype="query" hint="I get all the blog root ID based on the pageID">

    <!---arguments--->
    <cfargument name="pageID" type="numeric" required="yes" hint="I am the ID of the blog root">

    <cfset local.qry_getBlogRootByPageID = ''>

    <cfquery name="local.qry_getBlogRootByPageID">
    SELECT TOP 1 pageID, pageTemplate, pageParentID, pageURL, pageParentURL
    FROM page
    WHERE pageID = <cfqueryparam cfsqltype="cf_sql_integer" value="#arguments.pageID#">
    AND pageStatus <> <cfqueryparam cfsqltype="cf_sql_integer" value="2">
    </cfquery>

    <cfreturn local.qry_getBlogRootByPageID>
  </cffunction>

  <cffunction name="getBlogCategories" description="Get Blog Categories" access="private" output="false" returntype="query" hint="I get all the blog categories">

    <cfargument name="pageID" type="numeric" required="yes" hint="I am the ID of the blog root">

    <cfset var qry_getBlogCategories = ''>

    <cfquery name="qry_getBlogCategories">
    SELECT C.blogPostCategoryName, C.blogPostCategoryURL, COUNT(L.blogPostPageID) AS theCount
    FROM blogPostCategory AS C INNER JOIN pageBlogPost AS L
    ON C.blogPostCategoryID = L.blogPostCategory INNER JOIN page AS P
        ON L.blogPostPageID = P.pageID
        WHERE P.pageStatus <> <cfqueryparam cfsqltype="cf_sql_integer" value="2">
        AND c.blogPostCategoryBlogID = <cfqueryparam cfsqltype="cf_sql_integer" value="#arguments.pageID#">
    GROUP BY C.blogPostCategoryName, c.blogPostCategoryURL
    ORDER BY thecount DESC
    </cfquery>

    <cfreturn qry_getBlogCategories>
  </cffunction>

  <cffunction name="returnValidTitle" description="Return Valid Title" access="private" output="false" returntype="string" hint="I return a valid version of the title. If it is blank, I return the default.">

    <!---arguments--->
    <cfargument name="title" type="string" required="yes">

    <cfset arguments.title = htmleditformat(trim(arguments.title))>

    <cfif len(arguments.title) EQ 0>
      <cfset arguments.title = variables.defaults.title>
    </cfif>

    <cfreturn arguments.title>
  </cffunction>

  <cffunction name="reference" description="I am the required reference guide for this shortcode" access="remote" output="false" returntype="struct">

    <cfset var referenceStruct = structNew()>
    <cfset var description = ''>
    <cfset var argumentList = ''>
    <cfset var example = ''>

    <cfsavecontent variable="description">
    <p>This shortcode was designed specifically for a module on blog pages. It allows for sorting blog posts by category.</p>
    </cfsavecontent>
    <cfsavecontent variable="argumentList">
    <ul>
      <li><strong>Title</strong>:
        <ul>
                  <li>Blog Categories (default)</li>
                    <li>A custom title</li>
        </ul>
      </li>
    </ul>
        </cfsavecontent>
    <cfsavecontent variable="example">
    <cfoutput>[blogCategories pageID="3" /]</cfoutput>
    </cfsavecontent>

    <cfset referenceStruct["description"] = description>
    <cfset referenceStruct["argumentList"] = argumentList>
    <cfset referenceStruct["example"] = example>

    <cfreturn referenceStruct>
  </cffunction>

</cfcomponent>