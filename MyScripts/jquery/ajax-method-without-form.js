
$(function () {

    var parentAjaxFormSubmit = function () {
        $("#searchModal").modal('show');

        var input = $("#searchTerm").val();
        if (input == null || input == ''){
         }
        else{
            var $form = $("#searchForm");
            $form.children('#term').val(input);
            $form.submit();
        }
        //var options = {
        //    url: $form.attr("action"),
        //    method: $form.attr("method"),
        //    data: $form.serialize()
        //};

        //$.ajax(options).done(function (data) {                 //calls ajax function and when it is done the data from server is returned in data
        //    var $target = $($form.attr("data-pxp-target"));
        //    $target.replaceWith(data);
        //});

    };
    $("#btnSearchCustomer").click(parentAjaxFormSubmit);   //If it finds any html with data-pxp-ajax attribute=true it calss ajaxFormSubmit function on submission

    var searchPickupSubmit = function () {
        $("#searchModal").modal('show');

        var input = $("#pickupAddress").val();
        if (input == null || input == '') {
        }
        else {
            var $form = $("#searchForm");
            $form.children('#term').val(input);
            $form.submit();
        }

    };
    $("#btnSearchCustomerPickup").click(searchPickupSubmit);


    var searchPhoneSubmit = function () {
        $("#searchModal").modal('show');

        var input = $("#contactNumber").val();
        if (input == null || input == '') {
        }
        else {
            var $form = $("#searchForm");
            $form.children('#term').val(input);
            $form.submit();
        }

    };
    $("#btnSearchCustomerPhone").click(searchPhoneSubmit);

});