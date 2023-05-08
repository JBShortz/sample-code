=== Post to Captorra ===
Contributors: joebermudez
Donate link: 
Tags: Captorra, Forms, Contact Form 7, Gravity Forms, Ninja Forms, WPForms, Integration, Leads, API
Requires at least: 4.7
Tested up to: 5.9
Stable tag: 5.8.2
Requires PHP: 5.6
License: GPLv2 or later
License URI: https://www.gnu.org/licenses/gpl-2.0.html

Integrate form posts with the Captorra lead posting API. Post form submissions to your Captorra organization.

== Description ==

Seamlessly integrate your WordPress website with Captorra. Easily configurable. Compatible with all of the most 
popular form plugins. 

== Installation ==

1. Unzip the plugin file
2. Upload the folder `post-to-captorra` and it's contents to the `/wp-content/plugins/` directory
3. Intall a compatible forms plugin; Contact Form 7, Gravity Forms, Ninja Forms or WPForms.
4. Activate the form plugin through the 'Plugins' menu in WordPress
5. Activate the Post to Captorra through the 'Plugins' menu in WordPress
6. Use the `Integrations` plugin menu under the `Post to Captorra` menu to create a new `Integration`
7. Submit a test post with the integrated form.
8. Check that you post was successful by viewing the 'API Logs' under the `Post to Captorra` menu.


== Frequently Asked Questions ==

= Where do I get a Captorra Id, Referrer GUID or Type GUID? =

Contact support@captorra.com and include the name of the Captorra client you want to integrate with.

== Screenshots ==

1. Dasboard
2. Integrations page
3. Add Integration page 
4. Edit Integation page 
5. API logs page

== Changelog ==

= 1.1.5 = 
Updated advanced-custom-fields library to fix conflicts with existing advanced-custom-fields plugins

= 1.1.4 =
Fixed issue with CF7 select dropdown causing JSON syntax error.

= 1.1.3 =
Fixed issue with integration data (Type, Keyword, Vendor Id) not posting correctly.

= 1.1.2 =
Fixed issue with blank additional details throwing error

= 1.1 =
* New Feature: Additional Details multi-select - appends non-standard data to Details value.
* Fixed warning from wp_localize_script expecting an array.
* Fixed API response logging - response from CAPI now properly logged.
* Updated JS for dynamically loading form fields for mappings on Integration page
* Fixed API logs sorting

= 1.0 =
* Initial plugin launch.