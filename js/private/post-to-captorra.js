jQuery(document).ready(function($) {
  //stop our admin menus from collapsing
  if (
    $('body[class*=" capi_"]').length ||
    $('body[class*=" post-type-capi_"]').length
  ) {
    $capi_menu_li = $("#toplevel_page_capi_dashboard_admin_page");
    $capi_menu_li
      .removeClass("wp-not-current-submenu")
      .addClass("wp-has-current-submenu")
      .addClass("wp-menu-open");

    $("a:first", $capi_menu_li)
      .removeClass("wp-not-current-submenu")
      .addClass("wp-has-submenu")
      .addClass("wp-has-current-submenu")
      .addClass("wp-menu-open");
  }
});
