
$(function () {

    var parentAjaxFormSubmit = function () {
        $("#searchModal").modal('show');

        //var pickupAddress = $('#PickupAddress').val();
        //var dropAddress = $('#DropAddress').val();
        //var dropAddress1 = $('#DropAddress1').val();
        //var dropAddress2 = $('#DropAddress2').val();
        //var dropAddress3 = $('#DropAddress3').val();
        //var dropAddress4 = $('#DropAddress4').val();
        var paymentMode = $('#PaymentMode').val();


        //$.session.set('PickupAddress', pickupAddress);
        //$.session.set('DropAddress', dropAddress);
        //$.session.set('DropAddress1', dropAddress1);
        //$.session.set('DropAddress2', dropAddress2);
        //$.session.set('DropAddress3', dropAddress3);
        //$.session.set('DropAddress4', dropAddress4);
        $.session.set('PaymentMode', paymentMode);



        var input = $("#searchTerm").val();
        if (input == null || input == ''){
         }
        else{
            var $form = $("#searchForm");
            $form.children('#term').val(input);
            $form.children('#category').val('');
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
 
    $(".paymentModeRadio").click(function (event) {
        //For dashboard
         $('.jobFormInput').find('.mdl-textfield__input').removeAttr("disabled");
         $('.jobFormInput').find('.is-disabled').removeClass('is-disabled');

        //For new job
         $('.jobFormInput').find('.form-control').removeAttr("disabled");

        var amountField = $("#txtAmount");
        var isContract = event.target.value == "4";     //If payment mode is contract then disable the amount field.
        if (isContract == true) {
            amountField.val('0.00');
            amountField.attr("disabled", "disabled");
        }
        else {
            amountField.val('');
            amountField.removeAttr("disabled");
        }
        var doSearch = $.session.get('DoSearch');
        if (doSearch == "false" || event.target.value == "1" || event.target.value == "2")  //If already searched then dont search. If payment mode is Cash or credit card then dont search
            return;
        $("#searchModal").modal('show');
        var paymentMode = event.target.value;
       // $.session.set('PaymentMode', paymentMode);
        var $form = $("#searchForm");
        $form.children('#category').val(paymentMode);
        $form.submit();
    });
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