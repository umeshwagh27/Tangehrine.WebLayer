$(document).click(function (event) {
    //if you click on anything except the modal itself or the "open modal" link, close the modal
    if (!$(event.target).closest(".cstm-offcanvas").length) {
        $("html").removeClass("show-offcanvas");
    }
});


$(document).ready(function () {
    $(".sidebar-toggle").click(function (e) {
        e.stopPropagation();
        $("html").toggleClass("show-sidebar");
    });

    $(".offcanvas-btn").click(function (e) {
        e.stopPropagation();
        $("html").toggleClass("show-offcanvas");
    });
    

    function scrolling() {
        var sticky = $("header"),
            scroll = $(window).scrollTop();

        if (scroll >= 15) sticky.addClass("fixed");
        else sticky.removeClass("fixed");
    }
    scrolling();
    $(window).scroll(scrolling);

    // hide #back-top first
    $("#myBtn").hide();

    // fade in #back-top
    $(function () {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 100) {
                $("#myBtn").fadeIn();
            } else {
                $("#myBtn").fadeOut();
            }
        });

        // scroll body to 0px on click
        $("#myBtn").click(function () {
            $("body,html").animate(
                {
                    scrollTop: 0,
                },
                1000
            );
            return false;
        });
    });

    // $('.date-selector').datepicker();

    // $('.time-selector').timepicker({
    // 	format: 'hh:mm TT'
    // });

    
});
