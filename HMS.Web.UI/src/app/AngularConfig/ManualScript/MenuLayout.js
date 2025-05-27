$(".sidebar_mini #sidebar_main .menu_section > ul > li > ul :not(li) li").hover(function () {
    $(".sidebar_submenu").toggleClass("menuWidth");
});
$(".sidebar_mini #sidebar_main .menu_section > ul > li > ul > li > ul li[title]").hover(function () {
    $(".sidebar_submenu").toggleClass("menuWidth-v2");
});
