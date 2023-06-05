<?php
/*
Plugin Name: Post to Captorra
Plugin URI: https://www.captorra.com/captorra-api-posting/
Description: Integrate form posts with Captorra's lead posting API - CAPI. Compatible with WPForms, Contact Form 7, Gravity Forms and Ninja Forms. Post new leads automically to Captorra. Analytics tracking. Logs API responses.   
Version: 1.1.5
Author: Joe Bermudez
Author URI: https://www.captorra.com
License: GPL2
License URI: https://www.gnu.org/licenses/gpl-2.0.html
Text Domain: post-to-captorra
*/

/*  TABLE OF CONTENTS */
/*
    0. CONSTANTS
    1. HOOKS  
    2. FILTERS
        2.1 capi_integration_column_headers
        2.2 capi_integration_column_data
        2.3 capi_register_custom_admin_titles
        2.4 capi_custom_admin_titles
        2.5 capi_admin_menus
    3. EXTERNAL SCRIPTS
        3.1 ACF
        3.2 WP_List_Table Extension
        3.3 capi_public_scripts
        3.4 capi_admin_scripts    
    4. ACTIONS
        4.1 capi_add_submission_hooks
        4.2 capi_wpcf7_before_send_mail
        4.3 capi_gf_after_submission
        4.4 capi_ninja_forms_after_submission
        4.5 capi_wpforms_process_complete
        4.6 capi_create_process_log_table
        4.7 capi_create_api_log_table
        4.8 capi_activate_plugin
        4.9 capi_add_process_log
        4.10 capi_add_api_log
        4.11 capi_check_wp_version 
    5. HELPERS
        5.1 capi_acf_load_field
        5.2 capi_acf_input_admin_footer
        5.3 capi_load_mappings
        5.4 capi_get_cf7_mappings
        5.5 capi_get_gf_mappings
        5.6 capi_get_njn_mappings
        5.7 capi_get_wpf_mappings
        5.8 capi_get_mapping
        5.9 capi_get_plugin_forms
        5.10 capi_get_cf7_forms
        5.11 capi_get_gf_forms
        5.12 capi_get_nf_forms
        5.13 capi_get_wp_forms
        5.14 capi_get_integration_id
        5.15 capi_get_integration_data
        5.16 capi_get_acf_key
        5.17 capi_create_payload
        5.18 capi_post_to_url
        5.19 capi_return_json
        5.20 capi_get_process_logs
        5.21 capi_get_api_logs
        5.22 capi_get_admin_notice
    6. ADMIN PAGES
        6.1 capi_dashboard_admin_page    
        6.2 capi_logs_admin_page
    7. CUSTOM POST TYPES
    8. COOKIES
        8.1 capi_return_lead_source_type
        8.2 capi_return_referring_source
*/

/* 0. CONSTANTS */

//CAPI endpoint url
defined( 'CAPI_URL') or define( 'CAPI_URL', 'https://captorraapi.captorra.com/api/captorraapi/create');

//Contact Form 7 Constants
defined( 'CAPI_CF7_PLUGIN' ) or define( 'CAPI_CF7_PLUGIN', 'wpcf7' );
defined( 'CAPI_CF7_REPORTINGNAME' ) or define( 'CAPI_CF7_REPORTINGNAME', 'CF7' );
defined( 'CAPI_CF7_POST_SUBMISSION' ) or define( 'CAPI_CF7_POST_SUBMISSION', 'wpcf7_before_send_mail' );
defined( 'CAPI_CF7_FORM_ID_PREFIX' ) or define( 'CAPI_CF7_FORM_ID_PREFIX', 'cf7_' );

//Gravity Forms Constants
defined( 'CAPI_GF_PLUGIN' ) or define( 'CAPI_GF_PLUGIN', 'gf_edit_forms' );
defined( 'CAPI_GF_REPORTINGNAME' ) or define( 'CAPI_GF_REPORTINGNAME', 'GravityForms' );
defined( 'CAPI_GF_POST_SUBMISSION' ) or define( 'CAPI_GF_POST_SUBMISSION', 'gform_after_submission' );
defined( 'CAPI_GF_FORM_ID_PREFIX' ) or define( 'CAPI_GF_FORM_ID_PREFIX', 'gf_' );

//WPForms Constants
defined( 'CAPI_WPF_PLUGIN' ) or define( 'CAPI_WPF_PLUGIN', 'wpforms' );
defined( 'CAPI_WPF_REPORTINGNAME' ) or define( 'CAPI_WPF_REPORTINGNAME', 'WPForms' );
defined( 'CAPI_WPF_POST_SUBMISSION' ) or define( 'CAPI_WPF_POST_SUBMISSION', 'wpforms_process_complete' );
defined( 'CAPI_WPF_FORM_ID_PREFIX' ) or define( 'CAPI_WPF_FORM_ID_PREFIX', 'wpf_' );

//Ninja Forms Constants
defined( 'CAPI_NF_PLUGIN' ) or define( 'CAPI_NF_PLUGIN', 'ninja-forms' );
defined( 'CAPI_NF_REPORTINGNAME' ) or define( 'CAPI_NF_REPORTINGNAME', 'NinjaForms' );
defined( 'CAPI_NF_POST_SUBMISSION' ) or define( 'CAPI_NF_POST_SUBMISSION', 'ninja_forms_after_submission' );
defined( 'CAPI_NF_FORM_ID_PREFIX' ) or define( 'CAPI_NF_FORM_ID_PREFIX', 'njn_' );


/* 1. HOOKS */

//adds the form submission hooks/filters based on if a plugin exists
add_action('init','capi_add_submission_hooks');

//public scripts
add_action('wp_enqueue_scripts', 'capi_public_scripts');

//registers custom admin columns headers
add_filter('manage_edit-capi_integration_columns', 'capi_integration_column_headers');

//register custom admin column data
add_filter('manage_capi_integration_posts_custom_column', 'capi_integration_column_data', 1, 2);
add_action(
    'admin_head-edit.php',
    'capi_register_custom_admin_titles'
);

//register custom menus
add_action('admin_menu', 'capi_admin_menus');

//register external files to admin pages
add_action('admin_enqueue_scripts','capi_admin_scripts');

//ACF 
add_filter('acf/load_field/name=capi_form','capi_acf_load_field');
add_action('acf/input/admin_footer', 'capi_acf_input_admin_footer');

//register ajax action
add_action('wp_ajax_capi_load_mappings', 'capi_load_mappings'); //admin user
add_action('wp_ajax_capi_load_selected', 'capi_load_selected'); //admin user

//Advanced Custom Fields Settings
define( 'CAPI_MY_ACF_PATH', plugin_dir_path(__FILE__) . 'lib/advanced-custom-fields/' );
define( 'CAPI_MY_ACF_URL', plugin_dir_url(__FILE__) . 'lib/advanced-custom-fields/' );
add_filter('acf/settings/url', 'capi_acf_settings_url');
function capi_acf_settings_url( $url ) {
    return CAPI_MY_ACF_URL;
}
add_filter('acf/settings/show_admin', 'capi_acf_settings_show_admin');
function capi_acf_settings_show_admin( $show_admin ) {
    return true;
}

//register activate/deactivate/uninstall functions
register_activation_hook(__FILE__, 'capi_activate_plugin');
add_action('admin_notices', 'capi_check_wp_version');
register_uninstall_hook(__FILE__, 'capi_uninstall_plugin');


/* 2. FILTERS */

//2.1
function capi_integration_column_headers($columns) {
    
    $columns = array(
        'cb' => '<input type="checkbox" />',
        'title' =>__('Form ID'),
        'id' =>__('Captorra Id'),
        'date' => __('Created On'),
    );

    //returning new columns
    return $columns;
}

//2.2
function capi_integration_column_data($column, $post_id) {

    //setup our return text
    $output = '';

    switch($column) {

        case 'title': 
           $form = get_field('capi_form', $post_id);
           $output .= $form;
           break;
        case 'id': 
            $id = get_field('capi_captorra_id', $post_id);
            $output .= $id;
            break;
        case 'date':
            $date = get_post_meta($post_id, 'start_date', true);
            $output .= $date;
            break;
    }

    echo $output;

}

//2.3
//registers special custom admin title columns
function capi_register_custom_admin_titles() {
    add_filter(
        'the_title',
        'capi_custom_admin_titles',
        99,
        2
    );
}

//2.4
// handles custom admin title "title" column data for post types without titles
function capi_custom_admin_titles($title, $post_id) {
    global $post;

    $output = $title;

    if(isset($post->post_type)):
            switch($post->post_type) {
                case 'capi_integration':
                    $form = get_field('capi_form', $post_id);
                    $output = $form;
                break;
            }
    endif;

    return $output;
}

//2.5
function capi_admin_menus() {

    //main menu
    $top_menu_item = 'capi_dashboard_admin_page';

    add_menu_page('','Post to Captorra', 'manage_options', 'capi_dashboard_admin_page','capi_dashboard_admin_page','dashicons-menu');

    //submenu items
    add_submenu_page($top_menu_item, '', 'Dashboard', 'manage_options', $top_menu_item, $top_menu_item);
    add_submenu_page($top_menu_item, '', 'Integrations', 'manage_options', 'edit.php?post_type=capi_integration');  
    add_submenu_page($top_menu_item, '', 'API Logs', 'manage_options', 'capi_logs_admin_page', 'capi_logs_admin_page');
   
}

/* 3. EXTERNAL SCRIPTS */

//3.1 include ACF
include_once(plugin_dir_path(__FILE__) .'lib/advanced-custom-fields/acf.php');

//3.2 include WP_List_Table class extension 
include_once(plugin_dir_path(__FILE__). 'class/capi_log_list_table.php');

//3.3
function capi_public_scripts(){

    wp_register_script('capi-cookies-js-public', plugins_url('/js/public/capi-cookies.js', __FILE__),
    array('jquery'),'',true);

    wp_enqueue_script('capi-cookies-js-public');

    $cookies = array( 'path' => COOKIEPATH );
    wp_localize_script( 'capi-cookies-js-public', 'COOKIEPATH', $cookies );
}

//3.4
function capi_admin_scripts() {

    wp_register_script('post-to-captorra-js-private', plugins_url('/js/private/post-to-captorra.js',__FILE__),array('jquery'),'',true);

    wp_enqueue_script('post-to-captorra-js-private');

}

/* 4. ACTIONS */

//4.1
function capi_add_submission_hooks() {


    if( !function_exists('is_plugin_active') ) {
			
        include_once( ABSPATH . 'wp-admin/includes/plugin.php' );
        
    }
    
    //CF7 Hook
    if(is_plugin_active('contact-form-7/wp-contact-form-7.php') || class_exists('WPCF7_ContactForm') ):

        add_action( CAPI_CF7_POST_SUBMISSION, 'capi_wpcf7_before_send_mail', 10, 1 ); 

    endif;

    //GravityForms Hook
    if(is_plugin_active('gravityforms/gravityforms.php') || class_exists('RGFormsModel') ):

        add_action(CAPI_GF_POST_SUBMISSION, "capi_gf_after_submission", 10, 2);

    endif;

    //NinjaForms Hook
    if(is_plugin_active('ninja-forms/ninja-forms.php') || class_exists('Ninja_Forms') ):

        add_action(CAPI_NF_POST_SUBMISSION, "capi_ninja_forms_after_submission", 10, 2);

    endif;

    //WPForms
    if(is_plugin_active('wpforms-lite/wpforms-lite.php') || class_exists('WPForms') ):	

        add_action(CAPI_WPF_POST_SUBMISSION, "capi_wpforms_process_complete", 10, 4);

    endif;

}

//4.2
function capi_wpcf7_before_send_mail($contact_form) {

    $result = false;      

    try {

        //form id
        $form_id = $contact_form->id();

        //integration id
        $integration_id = capi_get_integration_id($form_id,'cf7_');

        //integration exists
        if($integration_id):

            //create unique process_id
            $uid = uniqid('capi', true);

            //cf7 process started
            //capi_add_process_log($integration_id, $uid, "CF7 - Integration found");

            //setup integration data array
            $integration_data = array();

            //integration data
            $integration_data = capi_get_integration_data($integration_id,'cf7_');
            //capi_add_process_log($integration_id, $uid, "CF7 - Integration data found for ID: " . $integration_id . ".");

            //get the form submission posted data
            $submission = WPCF7_Submission::get_instance();
            $posted_data = $submission->get_posted_data();
            $conversion_page = $submission->get_meta("url");

            //flatten array
            foreach ($posted_data as &$value):
                if(is_array($value)):
                    $value = current($value);
                endif;
            endforeach;

            $payload = array();
            $payload = capi_create_payload($posted_data, $integration_data, $conversion_page);
            
            if(is_array($payload)):

                //capi_add_process_log($integration_id, $uid, "CF7 - Payload created.");

                $json_payload = json_encode($payload);
                $result = capi_post_to_url(CAPI_URL, $json_payload);

                capi_add_api_log($integration_id, $json_payload, $result, $uid);
                //wp_mail( "jbermudez@captorra.com", "response", print_r($result,true));
                
                return $result;    
                

            endif;            
        
        endif;   
  
    } catch (Exception $e) {

        //error log
        capi_add_process_log($integration_id, $uid, "CF7 - EXCEPTION - " . $e.message);
    
    }
  
    //capi_add_process_log($integration_id, $uid, "CF7 - Integration Complete - " . $result);
    return $result;

}

//4.3
function capi_gf_after_submission($entry, $form) {
    
    $result = false;
    
    try {

        //form id
        $form_id = $form["id"];

        //integration id
        $integration_id = capi_get_integration_id($form_id,'gf_');

        //integration exists
        if($integration_id):

            //create unique process_id
            $uid = uniqid('capi', true);

            //setup integration data array
            $integration_data = array();

            //integration data
            $integration_data = capi_get_integration_data($integration_id,'gf_');

            //get the form submission posted data
            $posted_data = $entry; 
            $conversion_page  = $entry["source_url"];  
                  
            $payload = array();
            $payload = capi_create_payload($posted_data, $integration_data, $conversion_page);
            
            if(is_array($payload)):

                $json_payload = json_encode($payload);
                $result = capi_post_to_url(CAPI_URL, $json_payload);

                capi_add_api_log($integration_id, $json_payload, $result, $uid);
            
                return $result;                  

            endif;           
        
        endif;        
  
    } catch (Exception $e) {
        //error log 
        capi_add_process_log($integration_id, $uid, "GF - EXCEPTION - " . $e.message);      
    }
  
   
    return $result;

}

//4.4
function capi_ninja_forms_after_submission($form_data) {

    $result = false;

    try {

        //form id
        $form_id = $form_data["form_id"];

        //integration id
        $integration_id = capi_get_integration_id($form_id,'njn_');

        //integration exists
        if($integration_id):

            //create unique process_id
            $uid = uniqid('capi', true);

            //setup integration data array
            $integration_data = array();

            //integration data
            $integration_data = capi_get_integration_data($integration_id,'njn_');
           
            $fields = $form_data["fields"];

            $posted_data = array();

            foreach($fields as $field):
                $posted_data [$field["key"]] = $field["value"];                      
            endforeach;

            $conversion_page = wp_get_referer();
                  
            $payload = array();
            $payload = capi_create_payload($posted_data, $integration_data, $conversion_page);
           
            if(is_array($payload)):

                $json_payload = json_encode($payload);
                $result = capi_post_to_url(CAPI_URL, $json_payload);

                capi_add_api_log($integration_id, $json_payload, $result, $uid);
           
                return $result;

            endif;            
        
        endif;           
  
    } catch (Exception $e) {

        //error log
        capi_add_process_log($integration_id, $uid, "NJN - EXCEPTION - " . $e.message);      

    }  
  
    return $result;

}

//4.5
function capi_wpforms_process_complete( $fields, $entry, $form_data, $entry_id) {

    $result = false;
    
    try {

        //form id
        $form_id = $form_data["id"];

        //integration id
        $integration_id = capi_get_integration_id($form_id,'wpf_');

        //integration exists
        if($integration_id):

             //create unique process_id
             $uid = uniqid('capi', true);

            //setup integration data array
            $integration_data = array();

            //integration data
            $integration_data = capi_get_integration_data($integration_id,'wpf_');

            $fields = $entry["fields"];

            $posted_data = array();

            foreach($fields as $key => $value):
                if (is_array($value)):
                    foreach ($value as $subkey => $subvalue):
                        $posted_data ["wpf_" . $key . "_" . $subkey] = $subvalue; 
                    endforeach;  
                else:
                    $posted_data ["wpf_" . $key] = $value;                      
                endif;
            endforeach;

            $conversion_page = wp_get_referer();
                  
           
            $payload = array();
            $payload = capi_create_payload($posted_data, $integration_data, $conversion_page);
           
            if(is_array($payload)):

                $json_payload = json_encode($payload);
                $result = capi_post_to_url(CAPI_URL, $json_payload);

                capi_add_api_log($integration_id, $json_payload, $result, $uid);
           
                return $result;    
               

            endif;            
        
        endif;           
  
    } catch (Exception $e) {

        //error log
        capi_add_process_log($integration_id, $uid, "WPF - EXCEPTION - " . $e.message);      

    }      

    return $result;
}

//4.6
function capi_create_process_log_table() {

    //wordpress class to interact with database
    global $wpdb;

    $return_value = false;

    try {

            $table_name = $wpdb->prefix . "capi_process_log";
            $charset_collate = $wpdb->get_charset_collate();

            $sql = "CREATE TABLE $table_name (
                id mediumint(11) NOT NULL AUTO_INCREMENT,                
                integration_id mediumint(11) NOT NULL,
                process_id varchar(128) NOT NULL,               
                process_message MEDIUMTEXT NOT NULL,                
                UNIQUE KEY id (id)
                ) $charset_collate;";
            
            //include wordpress functions for dbDelta
            require_once(ABSPATH . 'wp-admin/includes/upgrade.php');

            //crea a new table in none exists or update and existing one
            dbDelta($sql);

            $return_value = true;

    } catch(Exception $e) {

    }

    return $return_value;

}

//4.7
function capi_create_api_log_table() {

    //wordpress class to interact with database
    global $wpdb;

    $return_value = false;

    try {

            $table_name = $wpdb->prefix . "capi_api_log";
            $charset_collate = $wpdb->get_charset_collate();

            $sql = "CREATE TABLE $table_name (
                id mediumint(11) NOT NULL AUTO_INCREMENT,                
                integration_id mediumint(11) NOT NULL,  
                process_id varchar(128) NOT NULL,                       
                payload MEDIUMTEXT NOT NULL,
                response MEDIUMTEXT NOT NULL,                 
                UNIQUE KEY id (id)
                ) $charset_collate;";
            
            //include wordpress functions for dbDelta
            require_once(ABSPATH . 'wp-admin/includes/upgrade.php');

            //crea a new table in none exists or update and existing one
            dbDelta($sql);

            $return_value = true;

    } catch(Exception $e) {

    }

    return $return_value;

}

//4.8
function capi_activate_plugin() {

    capi_create_process_log_table();
    capi_create_api_log_table();
}

//4.9
function capi_add_process_log($integration_id, $process_id, $process_message){

    global $wpdb;

    $return_data = false;

    try {

        $table_name = $wpdb->prefix . 'capi_process_log';

        $wpdb->insert(
            $table_name,
            array(
                'integration_id' => $integration_id,
                'process_id' => $process_id,
                'process_message' => $process_message,                
            ),
            array(
                '%d',
                '%s',
                '%s',
            )
        );

        $return_value = true;


    } catch(Exception $e) {

        

    }

    return $return_value;
}

//4.10
function capi_add_api_log($integration_id, $payload, $response, $process_id){
    
        global $wpdb;

        $return_data = false;

        try {

            $table_name = $wpdb->prefix . 'capi_api_log';
           
            $wpdb->insert(
                $table_name,
                array(                
                    'integration_id' => $integration_id,
                    'process_id' => $process_id,
                    'payload' => $payload,
                    'response' => $response,                
                ),
                array(                
                    '%d',
                    '%s',
                    '%s',
                    '%s',
                )
            );

           
            $return_value = true;


    } catch(Exception $e) {

        

    }

    return $return_value;
}

//4.11
function capi_check_wp_version() {
	
	global $pagenow;
	
	
	if ( $pagenow == 'plugins.php' && is_plugin_active('post-to-captorra/post-to-captorra.php') ):
	
		// get the wp version
		$wp_version = get_bloginfo('version');
		
		// tested vesions
		// these are the versions we've tested our plugin in
		$tested_versions = array(
            '5.9',
            '5.8.2',
            '5.7',
            '5.6',
            '5.5.3',
            '5.5.2',
            '5.5.1',
            '5.5',
            '5.4.4',
            '5.4.3',
            '5.4.2',
            '5.4.1',		
            '5.3.2',

           	
		);
		
		// IF the current wp version is  in our tested versions...
		if( !in_array( $wp_version, $tested_versions ) ):
		
			// get notice html
			$notice = capi_get_admin_notice('Post to Captorra has not been tested in your version of WordPress. It still may work though...','error');
			
			// echo the notice html
			echo( $notice );		
		
		endif;
	
	endif;
	
}

//4.12
function capi_uninstall_plugin() {

    capi_remove_plugin_tables();

    capi_remove_post_data();

    
}

//4.13
function capi_remove_plugin_tables() {

    global $wpdb;

    $tables_removed = false;

    try {

        $api_table = $wpdb->prefix . "capi_api_log";
        $process_table = $wpdb->prefix . "capi_process_log";

        $tables_removed = $wpdb->query("DROP TABLES IF EXISTS $api_table , $process_table");


    } catch(Exception $e){

    }

    return $tables_removed;

}

//4.14
function capi_remove_post_data() {

    global $wpdb;

    $data_removed = false;

    try {

        $table_name = $wpdb->prefix . "posts";

        $custom_post_types = array(
            'capi_integration'
        );

        $data_removed = $wpdb->query(
            $wpdb->prepare(
                "DELETE FROM $table_name WHERE post_type = %s OR post_type = %s",
                $custom_post_types[0],
                $custom_post_types[1]
            )
        );

        $table_name_1 = $wpdb->prefix . "_postmeta";
        $table_name_2 = $wpdb->prefix . "_posts";

        $wpdb->query(
            $wpdb->prepare(
                "DELETE pm FROM $table_name_1 pm
                LEFT JOIN $table_name_2 wp ON wp.ID = pm.post_id
                WHERE wp.ID IS NULL"
            )
        );

    } catch(Exception $e){

    }

    return $data_removed;

}




/* 5. HELPERS */

//5.1
function capi_acf_load_field( $field ) {
    
        $forms = capi_get_plugin_forms();
        
        $field['choices'] = array(
            '0' => 'No Form'
        );    

        foreach ($forms as $f):

            $field['choices'][$f['id']] = $f['title'];
        
        endforeach;	
	
    return $field;
    
}

//5.2
function capi_acf_input_admin_footer() {
    ?>    
    <script type="text/javascript">
    (function($) {
 
        if (typeof(acf) == 'undefined') { return; }   

        let dropdown = document.querySelectorAll('select[id^=acf-field_5e31e505e5e91-]');
        var $form_id = acf.getField('field_5e31b06b0b451').val(); 
        
           //form already set
            if($form_id != 0) {
               //console.log($form_id);
               //console.log(<?php echo get_the_ID(); ?>);
                $params = {id: $form_id, postID: <?php echo get_the_ID(); ?> }               
                $.ajax({
                    url: ajaxurl + '?action=capi_load_mappings',
                    method: "post",
                    dataType: "json",
                    data: $params,
                    success: function(response) {
                        $.each(dropdown, function (key, value){
                            $.each(response.data, function (key, entry) {
                                var option = document.createElement('option');
                                option.value = entry.name
                                option.text = entry.type + " " + entry.name;                          
                                value.add(option,null);
                            });    
                        });     
                    },
                    complete: function(response){
                        //load selected
                        $.ajax({
                            url: ajaxurl + '?action=capi_load_selected',
                            method: "post",
                            dataType: "json",
                            data: $params,
                            success: function(response) {
                                //console.log(response);
                                $.each(response.data, function (key, entry) {  
                                    
                                    var element = document.getElementById('acf-field_5e31e505e5e91-' + entry.meta_key);
                                    if(entry.meta_key == "field_5eda674f874c6") {
                                        var values = entry.meta_value;
                                        //console.log(values);
                                        for (var i = 0; i < element.options.length; i++) {
                                            element.options[i].selected = values.indexOf(element.options[i].value) >= 0;
                                        }
                                    } else {                             
                                        element.value = entry.meta_value;
                                    }
                                });                                 
                            }
                        });         
                    },
                    error: function(response) {
						//console.log(response);
                    }
                    
                });
            }

            //called when Form field value changes
            $('#acf-field_5e31b06b0b451').change(function(){

                $form_id = acf.getField('field_5e31b06b0b451').val();                
                $params = {id: $form_id, postID: <?php echo get_the_ID(); ?>}

                $.ajax({
                    url: ajaxurl + '?action=capi_load_mappings',
                    method: "post",
                    dataType: "json",
                    data: $params,
                    success: function(response) {
                        //console.log(response);
                        $.each(dropdown, function (key, value){                            
                            value.options.length = 0;
                            var option = document.createElement('option');
                            option.value = 0;   
                            option.text = 'No Mapping';      
                            value.add(option,0)
                            $.each(response.data, function (key, entry) {
                                var option = document.createElement('option');
                                option.value = entry.name
                                option.text = entry.type + " " + entry.name;                        
                                value.add(option,null)                                                    
                            });    
                        });     
                    },
                    error: function(response) {
                        //console.log(response);                        
                    }                    
                });        

            });       
        
    })(jQuery);	
    </script>    
    <?php    
}

//5.3
function capi_load_mappings() {    

    $result = array(
        'status' => 0,      
        'data' => '', 
        'error' => '',
        'errors' => array(),       
    );   

    try {

        $id = sanitize_key($_POST['id']);
        $pos = strrpos($id, '_') + 1;
        $prefix = strstr($id, '_', true); 
        
        $form_id = substr($id, $pos);
        $post_id = intval($_POST['postID']);

        //if ids not set, return;
        if (!isset($form_id) || !isset($post_id)):
            return;
        endif;

        //ids should be numeric
        if(!is_numeric($form_id) || !is_numeric($post_id)):
            return;
        endif;
        
        //check the prefix
        if($prefix == 'cf7'):
            $mappings = capi_get_cf7_mappings($form_id, $post_id);
        elseif($prefix == 'gf'):
        	$mappings = capi_get_gf_mappings($form_id, $post_id);
		elseif($prefix == 'njn'):
            $mappings = capi_get_njn_mappings($form_id, $post_id);
        elseif($prefix == 'wpf'):
            $mappings = capi_get_wpf_mappings($form_id, $post_id);
        endif;

    } catch (Exception $e) {        
        
    }

    $result['data'] = $mappings; 

    capi_return_json($result);   
}

//5.4
function capi_get_cf7_mappings($form_id, $post_id) {

    $objForm = WPCF7_ContactForm::get_instance($form_id);
    $manager = WPCF7_FormTagsManager::get_instance($form_id);
       
        $tags = $manager->scan( $objForm->prop('form') );
        $filter_result = $manager->filter( $tags, 'name' );

        foreach ($filter_result as $key => $value):
            if($value->name !== ''):
                $mapping = array(
                    'type' => $value->type,
                    'name' => $value->name               
                );

                $rows = capi_get_mapping($post_id, $value->name);

                foreach($rows as $row):
                    if($row->meta_value == $value->name):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;   

                $mappings []= $mapping;
            endif;
        endforeach;

    return $mappings;
}

//5.5
function capi_get_gf_mappings($form_id, $post_id) {

	$form = GFAPI::get_form($form_id);
    $fields = $form["fields"];   

    foreach($fields as $field):
		if(!empty($field["inputs"])):
			foreach($field["inputs"] as $input):				
                $mapping = array(
                    'type' => $field["label"] . "-" . $input["label"],
                    'name' => $input["id"]
                );	

                $rows = capi_get_mapping($post_id, $input["id"]);
                foreach($rows as $row):
                    if($row->meta_value == $input["id"]):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;
                $mappings [] = $mapping;			
			endforeach;
		else:
			$mapping = array(
				'type' => $field["label"],
				'name' => $field["id"]
			);
	
			$rows = capi_get_mapping($post_id, $field["id"]);
			foreach($rows as $row):
				if($row->meta_value == $field["id"]):
					$mapping['select'] = capi_get_acf_key($row->meta_key);
				endif;
			endforeach;
	
			$mappings [] = $mapping;
		endif;
    endforeach;
    
    return $mappings;
}

//5.6
function capi_get_njn_mappings($form_id, $post_id) {
	
    $fields = Ninja_Forms()->form($form_id)->get_fields();

        foreach($fields as $field):
            
            if($field->get_settings("type") !== 'submit'):
                $mapping = array(
                    'type' => $field->get_settings("type"),
                    'name' =>  $field->get_setting("key")
                );	
            
                $rows = capi_get_mapping($post_id,  $field->get_setting("key"));

                foreach($rows as $row):
                    if($row->meta_value ==  $field->get_setting("key")):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;

                $mappings []= $mapping;

            endif;
                
        endforeach;
	
    return $mappings;
}

//5.7
function capi_get_wpf_mappings($form_id, $post_id) {
	
    $form = wpforms()->form->get($form_id);

    $form_data = ! empty( $form->post_content ) ? wpforms_decode( $form->post_content ) : '';

    $fields = $form_data["fields"];

        foreach($fields as $field):            
            
            if($field["format"] == "first-last"):

                $name = "wpf_" . $field["id"] . "_first";

                $mapping = array(
                    'type' => $field["label"] . "-first"  ,
                    'name' =>  $name
                );

                $rows = capi_get_mapping($post_id,  $name);

                foreach($rows as $row):
                    if($row->meta_value == $name):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;

                $mappings []= $mapping;

                $name = "wpf_" . $field["id"] . "_last";
                
                $mapping = array(
                    'type' => $field["label"] . "-last"  ,
                    'name' =>  $name
                );             

                $rows = capi_get_mapping($post_id,  $name);

                foreach($rows as $row):
                    if($row->meta_value == $name):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;

            else:
                
                $name =  "wpf_" . $field["id"];

                $mapping = array(
                    'type' => $field["label"] ,
                    'name' =>  $name
                );

                $rows = capi_get_mapping($post_id,  $name);

                foreach($rows as $row):
                    if($row->meta_value == $name):
                        $mapping['select'] = capi_get_acf_key($row->meta_key);
                    endif;
                endforeach;
            
            endif;

           $mappings []= $mapping;
                
        endforeach;     
	
    return $mappings;
}

function capi_load_selected() {

    $result = array(
        'status' => 0,      
        'data' => '', 
        'error' => '',
        'errors' => array(),       
    );   

    try {

        $id = sanitize_key($_POST['id']);
        $pos = strrpos($id, '_') + 1;
        $prefix = strstr($id, '_', true); 
        
        $form_id = substr($id, $pos);
        $post_id = intval($_POST['postID']);

        //if ids not set, return;
        if (!isset($form_id) || !isset($post_id)):
            return;
        endif;

        //ids should be numeric
        if(!is_numeric($form_id) || !is_numeric($post_id)):
            return;
        endif;

        $rows = capi_get_all_mappings($post_id);

        //replace meta_key value with acf field id
        foreach($rows as $row):
            $row->meta_key = capi_get_acf_key($row->meta_key);
        endforeach;
        
        foreach($rows as $row):
            if($row->meta_key == "field_5eda674f874c6"):
                $row->meta_value = unserialize($row->meta_value);
            endif;
        endforeach;

    } catch (Exception $e) {        
        
    }

    $result['data'] = $rows; 

    capi_return_json($result);   

}

//5.8
function capi_get_mapping($post_id, $value) {

    global $wpdb;
	
	$table_name =  $wpdb->prefix . "postmeta";

    $query = "SELECT * FROM $table_name WHERE meta_key LIKE 'capi_mappings_%' AND meta_value = '$value' AND post_id = $post_id";
   
    $rows = $wpdb->get_results($query);

    return $rows;

}

function capi_get_all_mappings($post_id) {

    global $wpdb;
	
	$table_name =  $wpdb->prefix . "postmeta";

    $query = "SELECT * FROM $table_name WHERE meta_key LIKE 'capi_mappings_%' AND post_id = $post_id";
   
    $rows = $wpdb->get_results($query);

    return $rows;

}

//5.9
function capi_get_plugin_forms() {

	try {
	
        if(is_plugin_active('contact-form-7/wp-contact-form-7.php') || class_exists('WPCF7_ContactForm') ):
        $cf7_forms = capi_get_cf7_forms();
        foreach($cf7_forms as $f) {
                $form = array(
                'id' => CAPI_CF7_FORM_ID_PREFIX . $f->ID,
                    'title' => sprintf('(%s) %s', CAPI_CF7_REPORTINGNAME, $f->post_title)
                );
                $forms []= $form;
            }
        endif;
        
        if(is_plugin_active('gravityforms/gravityforms.php') || class_exists('RGFormsModel') ):		
            $gf_forms = capi_get_gf_forms();
            foreach($gf_forms as $f) {
                $form = array(
                    'id' => CAPI_GF_FORM_ID_PREFIX . $f["id"],
                    'title' => sprintf('(%s) %s', CAPI_GF_REPORTINGNAME, $f["title"])
                );
                $forms [] = $form;
            }
        endif;
            
        if(is_plugin_active('ninja-forms/ninja-forms.php') || class_exists('Ninja_Forms') ):			
            $nf_forms = capi_get_nf_forms();		
            foreach($nf_forms as $f) {
                $form = array(
                    'id' => CAPI_NF_FORM_ID_PREFIX . $f["id"],
                    'title' => sprintf('(%s) %s', CAPI_NF_REPORTINGNAME, $f["data"]["title"])
                );
                $forms [] = $form;
            }
        endif;

        if(is_plugin_active('wpforms-lite/wpforms-lite.php') || class_exists('WPForms') ):			
            $wp_forms = capi_get_wp_forms();		
            foreach($wp_forms as $f) {
                $form = array(
                    'id' => CAPI_WPF_FORM_ID_PREFIX . $f->ID,
                    'title' => sprintf('(%s) %s', CAPI_WPF_REPORTINGNAME, $f->post_title)
                );
                $forms [] = $form;
            }
        endif;
		
	} catch(Exception $e) {
		echo print_r($e,true);
	}		
	
    return $forms;

}

//5.10
function capi_get_cf7_forms() {
    return get_posts(array(
        'numberposts' => -1,
        'orderby' => 'ID',
        'order' => 'ASC',
        'post_type' => 'wpcf7_contact_form'));
}

//5.11
function capi_get_gf_forms() {
	return GFAPI::get_forms();	
}

//5.12
function capi_get_nf_forms() {
	return ninja_forms_get_all_forms();
}

//5.13
function capi_get_wp_forms(){
    return wpforms()->form->get();
}

//5.14
function capi_get_integration_id($form_id, $form_prefix) {

    $integration_id = 0;

    try {

        $integration_query = new WP_Query(
            array(
                'post_type' => 'capi_integration',
                'posts_per_page' => 1, 
                'meta_key' => 'capi_form',
                'meta_query' => array(
                    array(
                        'key' => 'capi_form',
                        'value' => $form_prefix . $form_id,
                        'compare' => '=',
                    ),
                ),
            )
        );
      
        if($integration_query->have_posts()):

            $integration_query->the_post();
            $integration_id = get_the_ID();

        endif;


    } catch (Exception $e) {

    }
  
    wp_reset_query();

    return (int)$integration_id;


}

//5.15
function capi_get_integration_data($integration_id, $form_prefix) {

    $integration_data = array();

    $integration = get_post($integration_id);

    if(isset($integration->post_type) && $integration->post_type == 'capi_integration'):

        $integration_data = array(
            'form' => get_field(capi_get_acf_key('capi_form'), $integration_id),
            'captorraid' => get_field(capi_get_acf_key('capi_captorra_id'), $integration_id),
            'referrer' => get_field(capi_get_acf_key('capi_referrer'), $integration_id),            
            'mappings' => get_field(capi_get_acf_key('capi_mappings'), $integration_id)
        );  

    endif;

    return $integration_data;

}

//5.16
function capi_get_acf_key($field_name) {

    $field_key = $field_name;

    switch($field_name) {
        case 'capi_form': 
            $field_key = 'field_5e31b06b0b451';
        break;
        case 'capi_captorra_id':
            $field_key = 'field_5e31e39ce5e8f';
        break;      
        case 'capi_referrer':
            $field_key = 'field_5e31e3c6e5e90';
        break;
        case 'capi_mappings':
            $field_key = 'field_5e31e505e5e91';
        break;   
        case 'capi_mappings_capi_first':
            $field_key = 'field_5e31fd6201fb5';
        break;  
        case 'capi_mappings_capi_last':
            $field_key = 'field_5e31fd6c01fb6';
        break;  
        case 'capi_mappings_capi_primary':
            $field_key = 'field_5e32fbd7393f6';
        break;    
        case 'capi_mappings_capi_secondary':
            $field_key = 'field_5e32fbf6393f7';
        break;     
        case 'capi_mappings_capi_email':
            $field_key = 'field_5e32fc19393f8';
        break;
        case 'capi_mappings_capi_address':
            $field_key = 'field_5e32fc29393f9';
        break;
        case 'capi_mappings_capi_city':
            $field_key = 'field_5e32fc35393fa';
        break;
        case 'capi_mappings_capi_state':
            $field_key = 'field_5e32fc44393fb';
        break;
        case 'capi_mappings_capi_zip':
            $field_key = 'field_5e32fc57393fc';
        break;
        case 'capi_mappings_capi_county':
            $field_key = 'field_5e32fc65393fd';
        break;
        case 'capi_mappings_capi_details':
            $field_key = 'field_5e32fc96393ff';
        break;
        case 'capi_mappings_capi_additional_details':
            $field_key = 'field_5eda674f874c6';
        break;
        case 'capi_mappings_capi_type':
            $field_key = 'field_5e32fcaa39400';
        break;
        case 'capi_mappings_capi_keyword':
            $field_key = 'field_5e32fcb739401';
        break;
        case 'capi_mappings_capi_vendor_id':
            $field_key = 'field_5e32fcc639402';
        break;

    }

    return $field_key;
}

//5.17
function capi_create_payload($posted_data, $integration_data, $conversion_page) {

    //$cookies = capi_check_cookies();
    $cookies = true;

    $referrer = esc_url_raw(base64_decode($_COOKIE["CAPIREFERER"]));

    if($referrer == ""):
        $referrer = "Direct Access";
    endif;

    $routeArray = esc_url_raw(base64_decode($_COOKIE["CAPIROUTE"]));   
    
    if($referrer == "Direct Access"):
        $host = "";
    else:
        $host = parse_url($referrer)['host'];
    endif;
    
    $root = parse_url(get_site_url())["host"];

    $source_type = capi_return_lead_source_type($host, $root, $cookies);

    //search engine keyword - later version

    $branded = "No";

    if($source_type = "Direct Access"):
        $branded = "Yes";
    endif;
   
    $source = capi_return_referring_source($host, $root, $cookies);

    $payload = array();

    $details = isset($posted_data[$integration_data['mappings']['capi_details']]) ? esc_attr($posted_data[$integration_data['mappings']['capi_details']]) : null;
    $additional_details = capi_process_additional_details($posted_data, $integration_data['mappings']['capi_additional_details']);
    $full_details = $details . $additional_details;

    $payload = array(
        "CaptorraId" => $integration_data['captorraid'],
        "Referrer" => $integration_data['referrer'],
        "First" => isset($posted_data[$integration_data['mappings']['capi_first']]) ? $posted_data[$integration_data['mappings']['capi_first']] : null,
        "Last" => isset($posted_data[$integration_data['mappings']['capi_last']]) ? $posted_data[$integration_data['mappings']['capi_last']] : null,
        "Primary" => isset($posted_data[$integration_data['mappings']['capi_primary']]) ? $posted_data[$integration_data['mappings']['capi_primary']] : null,
        "Secondary" => isset($posted_data[$integration_data['mappings']['capi_secondary']]) ? $posted_data[$integration_data['mappings']['capi_secondary']] : null,
        "Email" => isset($posted_data[$integration_data['mappings']['capi_email']]) ? $posted_data[$integration_data['mappings']['capi_email']] : null,
        "Address" => isset($posted_data[$integration_data['mappings']['capi_address']]) ? $posted_data[$integration_data['mappings']['capi_address']] : null,
        "City" => isset($posted_data[$integration_data['mappings']['capi_city']]) ? $posted_data[$integration_data['mappings']['capi_city']] : null,
        "State" => isset($posted_data[$integration_data['mappings']['capi_state']]) ? $posted_data[$integration_data['mappings']['capi_state']] : null,
        "Zip" => isset($posted_data[$integration_data['mappings']['capi_zip']]) ? $posted_data[$integration_data['mappings']['capi_zip']] : null,
        "County" => isset($posted_data[$integration_data['mappings']['capi_county']]) ? $posted_data[$integration_data['mappings']['capi_county']] : null,
        "Details" => $full_details,
        "Type" => isset($integration_data['mappings']['capi_type']) ? $integration_data['mappings']['capi_type'] : null,
        "Keyword" => isset($integration_data['mappings']['capi_keyword']) ? $integration_data['mappings']['capi_keyword'] : null,
        "ID" => isset($integration_data['mappings']['capi_vendor_id']) ? $integration_data['mappings']['capi_vendor_id'] : null,
        "Source" => isset($source_type) ? $source_type : null,
        "Branded" => isset($branded) ? $branded : null,
        "Conversion" => isset($conversion_page) ? $conversion_page : null,
        "ReferrerURL" => isset($referrer) ? $referrer : null,
        "Tracking" => isset($routeArray) ? $routeArray : null,
    );

    return $payload;

}

function capi_process_additional_details($posted_data, $additional_details) {

    $processed_details = '';

    if (is_array($additional_details) || is_object($additional_details)):
        foreach($additional_details as $value):
            $processed_details .=  "\r\n";
            $processed_details .= isset($posted_data[$value]) ? $posted_data[$value] : null;
        endforeach;
    endif;
    return $processed_details;
}

//5.18
function capi_post_to_url($url, $payload) {

    try {


        $response = wp_remote_post( $url, array(
            'method' => 'POST',
            'timeout' => 45,
            'redirection' => 5,
            'httpversion' => '1.0',
            'blocking' => true,
            'headers' => array('Content-Type: application/json'),
            'sslverify' => false,
            'body' => $payload,
            
            )
        );
        
        if ( is_wp_error( $response ) ) {
           $error_message = $response->get_error_message();
           capi_add_process_log(0, "capi_post_to_url EXCEPTION - " . $error_message);
        } 
    
    } catch (Exception $e) {

       //capi_add_process_log(0, "capi_post_to_url EXCEPTION - " . $e.message);       

    }

    return $response["body"];
}

//5.19
function capi_return_json($php_array) {

    //encode result as json string
    $json_result = json_encode($php_array);    

    //return result
    die($json_result);

    //stop all other processing
    exit;

}

//5.20
function capi_get_process_logs() {

    global $wpdb;

    $table_name =  $wpdb->prefix . "capi_process_log";

    $query = "SELECT id, integration_id, process_id, process_message FROM $table_name";
   
    $rows = $wpdb->get_results($query);

    return $rows;

}

//5.21
function capi_get_api_logs() {

    global $wpdb;

    $table_name =  $wpdb->prefix . "capi_api_log";

    $query = "SELECT id, integration_id, process_id, payload, response FROM $table_name";
   
    $rows = $wpdb->get_results($query,ARRAY_A);

    return $rows;

}

//5.22
function capi_get_admin_notice( $message, $class ) {
	
	// setup our return variable
	$output = '';
	
	try {
		
		// create output html
		$output = '
		 <div class="'. $class .'">
		    <p>'. $message .'</p>
		</div>
		';
	    
	} catch( Exception $e ) {
		
		// php error
		
	}
	
	// return output
	return $output;
	
}

/* 6. ADMIN PAGES */

//6.1
function capi_dashboard_admin_page() {

    $output = '<div class="wrap"><div id="icon-options-general" class="icon32"></div>';

    $output .= '<div id="poststuff"><div id="post-body" class="metabox-holder columns-2">';

    $output .= '<div class="post-body-content">
                <div class="meta-box-sortables ui-sortable">
                <div class="postbox">
                <h2 class="hndle"><span>Post to Captorra</span></h2>
                <div class="inside">
                <p>Captorra provides their clients with an API that allows vendors to post a lead into their Captorra organization.
                 The base integration allows for a number of common lead fields to be passed directly into Captorra to create a new lead for follow-up.</p>
                <h3>Setup</h3>
                <ul>
                <li>Contact Captorra to aquire the approiate CaptorraId, Referrer GUID and Type GUID (if needed).</li>
                <li>Make sure a compatible form plugin is installed. Post to Captorra is compatibale with Contact Form 7, Gravity Forms,
                Ninja Forms and WPForms.</li>
                <li>If the Post to Captorra plugin is installed prior to a valid form plugin, you may need to deactive/reactivate the Post
                to Captorra plugin for forms to populate on the Add Integration page. </li>
                </ul>

               
                <p><a href="edit.php?post_type=capi_integration" class="button button-primary">Add Integration</a> 
                <a href="admin.php?page=capi_logs_admin_page" class="button button-primary">View API Logs</a></p>

                </p>
                </div>
                </div>';

    $output .= '</div></div></div>';
    
    echo $output;

}

//6.2
function capi_logs_admin_page() {

    $Log_List_Table = new CAPI_Log_List_Table();

    echo '<div class="wrap"><h2>Logs</h2>';
  
        $Log_List_Table->prepare_items();
        $Log_List_Table->display();

    echo '</div>';
    
    

}

/* 7. CUSTOM POST TYPES */

//7.1
//integrations
include_once(plugin_dir_path(__FILE__). 'cpt/capi_integration.php');

/* 8. COOKIES */

//8.1
function capi_return_lead_source_type($host, $root, $cookies) {

    $source_type = "Unclassifed";

    if(!$cookies && $host == ""):
        $source_type = "Unknown";
        return $source_type;
    endif;

    if($cookies && $host == ""):
        $source_type = "Direct Access";
        return $source_type;
    endif;

    if(strpos($root,$host) !== false):
        $source_type = "Direct Access";
        return $source_type;
    endif;

    $search_engines = array("google.com", "bing.com", "yahoo.com", "duckduckgo.com", "ask.com", "aol.com");

    foreach ($search_engines as $search_engine):
        if(strpos($search_engine,$host) !== false):
            $source_type = "Search";
            return $source_type;
        endif;
    endforeach;

    $source_type = "Referring Site";
    return $source_type;    

}

//8.2
function capi_return_referring_source($host, $root, $cookies) {

    $source = $host;

    if(!$cookies && $host == ""):
        $source = "Unknown";
        return $source;
    endif;

    if($cookies && $host = ""):
        $source = "Direct Access";
        return $source;
    endif;

    if($host !== ""):
        if(strpos($root,$host) !== false):
            $source = "Direct Access";
            return $source;
        endif;
    endif;

    return $source;

}