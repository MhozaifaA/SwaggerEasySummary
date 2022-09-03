

(function () {
    var link = document.querySelector("link[rel*='icon']") || document.createElement('link');;
    document.head.removeChild(link);
    link = document.querySelector("link[rel*='icon']") || document.createElement('link');
    document.head.removeChild(link);
    link = document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'shortcut icon';
    link.href = '/favicon.png';
    document.getElementsByTagName('head')[0].appendChild(link);
})();

//(function calllooppostman() {

//    setTimeout(() => {
//        let notyet = bringpostman();
//        if (notyet)
//            calllooppostman();
//    }, 400)
//})();

$(document).ready(() => {


    $(window).on("load",function () {

        $('head').append('<link type="image/x-icon" rel="icon" href="/favicon-16x16.png" sizes="16x16">')
            .append('<link type="image/x-icon" rel="icon" href="/favicon-96x96.png" sizes="96x96">');
       
    });

   

});





//function bringpostman() {

//    if ($('hgroup')) {
       
//           $('hgroup').append('<a style="display: block;" id="postmanLink" target="_blank" href="https://documenter.getpostman.com/view/20879575/UyxdMpiH" rel="noopener noreferrer" class="link"><span class="url"><strong style="color:#ff6c37">Postman:</strong> https://documenter.getpostman.com/view/</span></a>');

//        if ($("#postmanLink").length == 0) {
//            return true;
//        }
//        else
//            return false;
//    }
//    return true;
//}