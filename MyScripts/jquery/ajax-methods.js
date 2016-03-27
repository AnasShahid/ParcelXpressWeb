
$(function () {
    var ajaxFormSubmit = function () {
        var $form = $(this);

        var options = {
            url: $form.attr("action"),
            method: $form.attr("method"),
            data: $form.serialize()
        };

        $.ajax(options).done(function (data) {                 //calls ajax function and when it is done the data from server is returned in data
            var $target = $($form.attr("data-pxp-target"));
            $target.replaceWith(data);
        });

        return false;
    };
    $("form[data-pxp-ajax='true']").submit(ajaxFormSubmit);   //If it finds any html with data-pxp-ajax attribute=true it calss ajaxFormSubmit function on submission


    // Code for intercepting click event on paged list

    var getPage = function () {
        var $a = $(this);
        var options = {
            url: $a.attr("href"),
            data:$("form").serialize(),
            type:"get"
        };
        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-pxp-target");
            $(target).replaceWith(data);
            e.preventDefault();
        });
        return false;
    };

    $(".main-content").on("click", ".pagedList a", getPage);


});