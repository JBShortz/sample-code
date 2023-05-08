(function ($) {
  $path = COOKIEPATH.path;
  //console.log("COOKIEPATH: " + $path);
  $domain = window.location.hostname;

  var date = new Date();
  date.setTime(date.getTime() + 7 * 24 * 60 * 60 * 1000);
  $expires = date.toGMTString();

  if (typeof document.referrer !== "undefined") {
    $referrer = document.referrer;
  } else {
    $referrer = "";
  }

  if (document.cookie.indexOf("CAPIREFERER=") == -1) {
    document.cookie =
      "CAPIREFERER=" +
      btoa($referrer) +
      "; expires= " +
      $expires +
      "; path=" +
      $path +
      "; domain=" +
      $domain +
      " ";
  }

  if (document.cookie.indexOf("CAPIROUTE=") >= 0) {
    var decrypted_route = atob(getCookie("CAPIROUTE"));
    var base_array = decrypted_route.split("\n\n\t");
  } else {
    var base_array = [];
  }

  var datetime = new Date();
  $tracking = datetime + " - " + $referrer;

  base_array.push($tracking);

  document.cookie =
    "CAPIROUTE=" +
    btoa(base_array.join("\n\n\t")) +
    "; expires= " +
    $expires +
    "; path=" +
    $path +
    "; domain=" +
    $domain +
    " ";
})(jQuery);

function getCookie(name) {
  var v = document.cookie.match("(^|;) ?" + name + "=([^;]*)(;|$)");
  return v ? v[2] : null;
}
