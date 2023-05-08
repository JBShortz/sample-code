<?php

if( ! class_exists( 'WP_List_Table' ) ):
    require_once( ABSPATH . 'wp-admin/includes/class-wp-list-table.php' );
endif;




class CAPI_Log_List_Table extends WP_List_Table {    

    // var $example_data = array(
    //     array('ID' => 1,'booktitle' => 'Quarter Share', 'author' => 'Nathan Lowell',
    //           'isbn' => '978-0982514542'),
    //     array('ID' => 2, 'booktitle' => '7th Son: Descent','author' => 'J. C. Hutchins',
    //           'isbn' => '0312384378'),
    //     array('ID' => 3, 'booktitle' => 'Shadowmagic', 'author' => 'John Lenahan',
    //           'isbn' => '978-1905548927'),
    //     array('ID' => 4, 'booktitle' => 'The Crown Conspiracy', 'author' => 'Michael J. Sullivan',
    //           'isbn' => '978-0979621130'),
    //     array('ID' => 5, 'booktitle'     => 'Max Quick: The Pocket and the Pendant', 'author'    => 'Mark Jeffrey',
    //           'isbn' => '978-0061988929'),
    //     array('ID' => 6, 'booktitle' => 'Jack Wakes Up: A Novel', 'author' => 'Seth Harwood',
    //           'isbn' => '978-0307454355')
    //   );

    public function __construct() {
        //global $status, $page;       

        parent::__construct( array(
            'singular' => __( 'Log', 'sp' ), //singular name of the listed records
            'plural'   => __( 'Logs', 'sp' ), //plural name of the listed records
            'ajax'     => false 

         ) );

        //add_action( 'admin_head', array( $this, 'admin_header' ), 10, 2 );
        $this->admin_header();
    }

              

    function get_log_data() {

        global $wpdb;
    
        $table_name =  $wpdb->prefix . "capi_api_log";
    
        $query = "SELECT id, integration_id, process_id, payload, response FROM $table_name";
       
        $rows = $wpdb->get_results($query,ARRAY_A);
    
        return $rows;
    
    }

    public function admin_header() {
        $page = ( isset($_GET['page'] ) ) ? esc_attr( $_GET['page'] ) : false;       
        if( 'capi_logs_admin_page' != $page )
        return;
        echo '<style type="text/css">';
        echo '.wp-list-table .column-id { width: 5%; }';
        echo '.wp-list-table .column-integration_id { width: 9%; }';
        echo '.wp-list-table .column-payload { width: 40%; }';
        echo '.wp-list-table .column-response { width: 35%; }';        
        echo '</style>';
    }

    function no_items() {
    _e( 'No logs found.' );
    }

    function get_columns(){
        $columns = array(
          'id' => 'Id',
          'integration_id' => 'Integration Id',
          'payload'    => 'Payload',
          'response'      => 'Response'
        );
        return $columns;
    }

    function prepare_items() {
        $columns = $this->get_columns();
        $hidden = array();
        $sortable = $this->get_sortable_columns();
        $this->_column_headers = array($columns, $hidden, $sortable);       

        $posts = $this->get_log_data();        

        usort( $posts, array( &$this, 'usort_reorder' ) );

        $per_page = 5;
        $current_page = $this->get_pagenum();
        $total_items = count($this->get_log_data());
        
        $this->set_pagination_args(array(
            'total_items' => $total_items,
            'per_page' => $per_page,
            'total_pages' => ceil( $total_items / $per_page )
        ));

        $last_post = $current_page * $per_page;
        $first_post = $last_post - $per_page + 1;
        $last_post > $total_items AND $last_post = $total_items;

         // Setup the range of keys/indizes that contain 
        // the posts on the currently displayed page(d).
        // Flip keys with values as the range outputs the range in the values.
        $range = array_flip( range( $first_post - 1, $last_post - 1, 1 ) );

        // Filter out the posts we're not displaying on the current page.
        $posts_array = array_intersect_key( $posts, $range );

        

        $this->items = $posts_array;

      
    }

    function column_default( $item, $column_name ) {
        switch( $column_name ) { 
          case 'id':
          case 'integration_id':
          case 'payload':
          case 'response':
            return $item[ $column_name ];
          default:
            return;
            //return print_r( $this->get_log_data(), true ) ; //Show the whole array for troubleshooting purposes
        }
    }

    function get_sortable_columns() {
        $sortable_columns = array(
          'id'  => array('id',false), 
          'integration_id'  => array('integration_id',false),              
        );
        return $sortable_columns;
    }

    function usort_reorder( $a, $b ) {
        $orderby = ( ! empty( $_GET['orderby'] ) ) ? $_GET['orderby'] : 'id';
        $order = ( ! empty($_GET['order'] ) ) ? $_GET['order'] : 'desc';
        $result = strnatcmp( $a[$orderby], $b[$orderby] );
        return ( $order === 'asc' ) ? $result : -$result;
      }


}
