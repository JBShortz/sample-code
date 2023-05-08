<?php

function capi_register_capi_integration() {

	/**
	 * Post Type: Integrations.
	 */

	$labels = [
		"name" => __( "Integrations", "twentytwenty" ),
		"singular_name" => __( "Integration", "twentytwenty" ),
		"edit_item" => __( "Edit Integration", "twentytwenty" ),
		"add_new_item" => __( "Add New Integration", "twentytwenty" ),
	];

	$args = [
		"label" => __( "Integrations", "twentytwenty" ),
		"labels" => $labels,
		"description" => "",
		"public" => false,
		"publicly_queryable" => false,
		"show_ui" => true,
		"show_in_rest" => true,
		"rest_base" => "",
		"rest_controller_class" => "WP_REST_Posts_Controller",
		"has_archive" => false,
		"show_in_menu" => false,
		"show_in_nav_menus" => true,
		"delete_with_user" => false,
		"exclude_from_search" => true,
		"capability_type" => "post",
		"map_meta_cap" => true,
		"hierarchical" => false,
		"rewrite" => [ "slug" => "capi_integration", "with_front" => false ],
		"query_var" => true,
		"supports" => false,
	];

	register_post_type( "capi_integration", $args );
}

add_action( 'init', 'capi_register_capi_integration' );


if( function_exists('acf_add_local_field_group') ):

acf_add_local_field_group(array(
	'key' => 'group_5e31b063d74ba',
	'title' => 'Integration Details',
	'fields' => array(
		array(
			'key' => 'field_5e31b06b0b451',
			'label' => 'Form',
			'name' => 'capi_form',
			'type' => 'select',
			'instructions' => 'Dropdown is populated with available forms across all compatible plugins.',
			'required' => 1,
			'conditional_logic' => 0,
			'wrapper' => array(
				'width' => '',
				'class' => '',
				'id' => '',
			),
			'choices' => array(
				'0' => 'No Form'			
			),
			'default_value' => array(
			),
			'allow_null' => 0,
			'multiple' => 0,
			'ui' => 0,
			'return_format' => 'value',
			'ajax' => 0,
			'placeholder' => '',
		),
		array(
			'key' => 'field_5e31e39ce5e8f',
			'label' => 'CaptorraId',
			'name' => 'capi_captorra_id',
			'type' => 'text',
			'instructions' => 'Unique six-digit ID identifying a Captorra organization.',
			'required' => 1,
			'conditional_logic' => 0,
			'wrapper' => array(
				'width' => '',
				'class' => '',
				'id' => '',
			),
			'default_value' => '',
			'placeholder' => '',
			'prepend' => '',
			'append' => '',
			'maxlength' => '',
		),
		array(
			'key' => 'field_5e31e3c6e5e90',
			'label' => 'Referrer GUID',
			'name' => 'capi_referrer',
			'type' => 'text',
			'instructions' => 'Unique GUID identifying a posting source.',
			'required' => 1,
			'conditional_logic' => 0,
			'wrapper' => array(
				'width' => '',
				'class' => '',
				'id' => '',
			),
			'default_value' => '',
			'placeholder' => '',
			'prepend' => '',
			'append' => '',
			'maxlength' => '',
		),
		array(
			'key' => 'field_5e31e505e5e91',
			'label' => 'Mappings',
			'name' => 'capi_mappings',
			'type' => 'group',
			'instructions' => 'Below are the standard fields accepted by the Captorra lead posting API. Dropdowns will be populated with all available fields from the Form selected above.
			',
			'required' => 0,
			'conditional_logic' => array(
				array(
					array(
						'field' => 'field_5e31b06b0b451',
                        'operator' => '!=',
                        'value' => 'custom',
					),
				),
			),
			'wrapper' => array(
				'width' => '',
				'class' => '',
				'id' => '',
			),
			'layout' => 'row',
			'sub_fields' => array(
				array(
					'key' => 'field_5e31fd6201fb5',
					'label' => 'First',
					'name' => 'capi_first',
					'type' => 'select',
					'instructions' => 'First Name',
					'required' => 1,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
                    ),
                    'choices' => array(
						'0' => 'No Mapping'
						
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
					
				),
				array(
					'key' => 'field_5e31fd6c01fb6',
					'label' => 'Last',
					'name' => 'capi_last',
					'type' => 'select',
					'instructions' => 'Last Name',
					'required' => 1,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fbd7393f6',
					'label' => 'Primary',
					'name' => 'capi_primary',
					'type' => 'select',
					'instructions' => 'Primary Number',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fbf6393f7',
					'label' => 'Secondary',
					'name' => 'capi_secondary',
					'type' => 'select',
					'instructions' => 'Secondary Number',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc19393f8',
					'label' => 'Email',
					'name' => 'capi_email',
					'type' => 'select',
					'instructions' => 'Email Address',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc29393f9',
					'label' => 'Address',
					'name' => 'capi_address',
					'type' => 'select',
					'instructions' => 'Street Address',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc35393fa',
					'label' => 'City',
					'name' => 'capi_city',
					'type' => 'select',
					'instructions' => '',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc44393fb',
					'label' => 'State',
					'name' => 'capi_state',
					'type' => 'select',
					'instructions' => '',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc57393fc',
					'label' => 'Zip',
					'name' => 'capi_zip',
					'type' => 'select',
					'instructions' => '',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fc65393fd',
					'label' => 'County',
					'name' => 'capi_county',
					'type' => 'select',
					'instructions' => '',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
                    'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				// array(
				// 	'key' => 'field_5e32fc84393fe',
				// 	'label' => 'Best Time',
				// 	'name' => 'capi_besttime',
				// 	'type' => 'select',
				// 	'instructions' => '',
				// 	'required' => 0,
				// 	'conditional_logic' => 0,
				// 	'wrapper' => array(
				// 		'width' => '',
				// 		'class' => '',
				// 		'id' => '',
				// 	),
                //     'choices' => array(
				// 	),
				// 	'allow_null' => 0,
				// 	'multiple' => 0,
				// 	'ui' => 0,
				// 	'return_format' => 'value',
				// 	'ajax' => 0,
				// 	'placeholder' => '',
				// ),
				array(
					'key' => 'field_5e32fc96393ff',
					'label' => 'Details',
					'name' => 'capi_details',
					'type' => 'select',
					'instructions' => 'Details regarding the lead',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
                    'choices' => array(
						'0' => 'No Mapping'
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5eda674f874c6',
					'label' => 'Additional Details',
					'name' => 'capi_additional_details',
					'type' => 'select',
					'instructions' => 'Select additional fields to append to the Details value (Ctrl + click for multi select)',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'choices' => array(
					),
					'default_value' => array(
					),
					'allow_null' => 0,
					'multiple' => 1,
					'ui' => 0,
					'ajax' => 0,
					'return_format' => 'value',
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fcaa39400',
					'label' => 'Type',
					'name' => 'capi_type',
					'type' => 'text',
					'instructions' => 'Case Type GUID',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
					'default_value' => '',
                    'placeholder' => '',
                    'prepend' => '',
                    'append' => '',
                    'maxlength' => '',
				),
				array(
					'key' => 'field_5e32fcb739401',
					'label' => 'Keyword',
					'name' => 'capi_keyword',
					'type' => 'text',
					'instructions' => 'Transation Number / Addtional Marking Details',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
                    'choices' => array(
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				array(
					'key' => 'field_5e32fcc639402',
					'label' => 'ID',
					'name' => 'capi_vendor_id',
					'type' => 'text',
					'instructions' => 'Vendor unique ID',
					'required' => 0,
					'conditional_logic' => 0,
					'wrapper' => array(
						'width' => '',
						'class' => '',
						'id' => '',
					),
                    'choices' => array(
					),
					'allow_null' => 0,
					'multiple' => 0,
					'ui' => 0,
					'return_format' => 'value',
					'ajax' => 0,
					'placeholder' => '',
				),
				// array(
				// 	'key' => 'field_5e32fd2d39404',
				// 	'label' => 'Custom',
				// 	'name' => 'capi_custom',
				// 	'type' => 'textarea',
				// 	'instructions' => 'Advanced Custom Fields',
				// 	'required' => 0,
				// 	'conditional_logic' => 0,
				// 	'wrapper' => array(
				// 		'width' => '',
				// 		'class' => '',
				// 		'id' => '',
				// 	),
				// 	'default_value' => '',
				// 	'placeholder' => '',
				// 	'maxlength' => '',
				// 	'rows' => '',
				// 	'new_lines' => '',
				// ),

			),
		),
	),
	'location' => array(
		array(
			array(
				'param' => 'post_type',
				'operator' => '==',
				'value' => 'capi_integration',
			),
		),
	),
	'menu_order' => 0,
	'position' => 'normal',
	'style' => 'default',
	'label_placement' => 'top',
	'instruction_placement' => 'label',
	'hide_on_screen' => array(
		0 => 'permalink',
		1 => 'the_content',
		2 => 'excerpt',
		3 => 'discussion',
		4 => 'comments',
		5 => 'revisions',
		6 => 'slug',
		7 => 'author',
		8 => 'format',
		9 => 'page_attributes',
		10 => 'featured_image',
		11 => 'categories',
		12 => 'tags',
		13 => 'send-trackbacks',
	),
	'active' => true,
	'description' => '',
));

endif;