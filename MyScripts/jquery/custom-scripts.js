﻿
$(function () {
    //Function to control loading image
    jQuery.fn.center = function () {
        this.css("position", "fixed");
        this.css("top", ($(window).height() / 2) - (this.outerHeight() / 2));
        this.css("left", ($(window).width() / 2) - (this.outerWidth() / 2));
        return this;
    }



    //Customer save/update confirmation without account
    var submitWithConfirmation = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
        var showConfirm = $button.attr("data-showConfirm");

        if (showConfirm == "True") {
            BootstrapDialog.show({
                message: 'Account is not generated for this customer, Are you sure to save the changes?',
                title: 'Confirmation',
                buttons: [{
                    label: 'No',
                    cssClass: 'btn-danger',
                    action: function (dialogItself) {
                        dialogItself.close();
                        return false;
                    }
                }, {
                    label: 'Yes',
                    cssClass: 'btn-success',
                    action: function (dialogItself) {
                        dialogItself.close();
                        $form.submit();
                        return false;
                    }
                }
                ]
            });
        }
        else {
            $form.submit();
        }
        return false;
    };

    $("#btnSaveCustomer").click(submitWithConfirmation);
    //-------------
    //code for loading messages with AJAX
    var messageNav = function () {
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var options = {
            url: targetUrl,
            type: 'get'
        };

        $.ajax(options).done(function (data) {
            $("#messageContainer").replaceWith(data);
        });

        return false;

    };

    $("#sideNavigation").on('click', 'li a', messageNav);
    //--------------------------------

    var activeJobsRefresh = function () {
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var options = {
            url: targetUrl,
            type:'get'
        };
        $.ajax(options).done(function (data) {
            $("#activeJobsList").replaceWith(data);
        });
        return false;
    };

    $("#activeJobsRefresh").on('click', '.refreshButton', activeJobsRefresh);

    var activeDriverRefresh = function () {
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var options = {
            url: targetUrl,
            type: 'get'
        };
        $.ajax(options).done(function (data) {
            $("#activeDriverGrid").replaceWith(data);
        });
        return false;
    };

    $("#activeDriverRefresh").on('click', '.refreshDriverButton', activeDriverRefresh);

    var selectInvoiceForPayment = function () {
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var options = {
            url: targetUrl,
            type: 'get'
        };
        $.ajax(options).done(function (data) {
            $("#selectedInvoice").replaceWith(data);
        });
        return false;
    }
    $(".btnSelectInvoice").click(selectInvoiceForPayment);

    var loadDetailAjax = function () {
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var targetElement = $a.attr("data-pxp-target");
        var options = {
            url: targetUrl,
            type: 'get'
        };
        $.ajax(options).done(function (data) {
            $(targetElement).replaceWith(data);
        });
        return false;
    }
    $(".btnSelectDetails").click(loadDetailAjax);


    var cancelJobConfirm = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
        var showConfirm = $button.attr("data-showConfirm");

            BootstrapDialog.show({
                message: 'Are you sure you want to delete this job?',
                title: 'Confirmation',
                buttons: [{
                    label: 'No',
                    cssClass: 'btn-default',
                    action: function (dialogItself) {
                        dialogItself.close();
                        return false;
                    }
                }, {
                    label: 'Yes',
                    cssClass: 'btn-danger',
                    action: function (dialogItself) {
                        dialogItself.close();
                        $form.submit();
                        return false;
                    }
                }
                ]
            });
        
        
        return false;
    };

    $("#btnCancelJob").click(cancelJobConfirm);
    //Confirmation on delete click.

    var settleDriverAccount = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
        var accountType = $button.attr("data-accountType");
        var msg = "";
        if (accountType == 'driver') {
            msg = "Please confirm that you want to post driver account. (This will settle his commission)"
        }
        else if (accountType = 'invoice') {
            msg = "Do you want to mark this invoice as paid?"
        }
        else {
            msg = "Are you sure you want to post customer receipt?"
        }
        BootstrapDialog.show({

            message: msg,
            title: 'Confirmation',
            buttons: [{
                label: 'Cancel',
                cssClass: 'btn-default',
                action: function (dialogItself) {
                    dialogItself.close();
                    return false;
                }
            }, {
                label: 'Confirm',
                cssClass: 'btn-success',
                action: function (dialogItself) {
                    dialogItself.close();
                    $form.submit();
                    return false;
                }
            }
            ]
        });


        return false;
    };
    $("#settleAccount").click(settleDriverAccount);

    var deleteConfirmation = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
     
        BootstrapDialog.show({
            message: 'Are you sure you want to delete ?',
            title: 'Confirmation',
            buttons: [{
                label: 'No',
                cssClass: 'btn-default',
                action: function (dialogItself) {
                    dialogItself.close();
                    return false;
                }
            }, {
                label: 'Yes',
                cssClass: 'btn-danger',
                action: function (dialogItself) {
                    dialogItself.close();
                    $form.submit();
                    return false;
                }
            }
            ]
        });


        return false;
    };
    $('#btnDelete').click(deleteConfirmation);


    var generateEmailConfirmation = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
        BootstrapDialog.show({
            message: 'Are you sure you want to generate invoice and send to customer?',
            title: 'Confirmation',
            buttons: [{
                label: 'No',
                cssClass: 'btn-default',
                action: function (dialogItself) {
                    dialogItself.close();
                    return false;
                }
            }, {
                label: 'Yes',
                cssClass: 'btn-success',
                action: function (dialogItself) {
                    dialogItself.close();
                    $("#loading").show();
                    $form.submit();
                    return false;
                }
            }
            ]
        });


        return false;
    };
    $('#btnGenerateStatement,[name="btnGenerateStatement"],[name="btnGenerateContractStatement"]').click(generateEmailConfirmation);

    var showReportParams = function () {
        $("#selectedCustomerDiv").removeClass("hidden");
        var $a = $(this);
        var targetUrl = $a.attr("href");
        var targetElement = $a.attr("data-pxp-target");
        var options = {
            url: targetUrl,
            type: 'get'
        };
        $.ajax(options).done(function (data) {
            $(targetElement).replaceWith(data);
        });
        return false;
    }
    $('[name="btnShowReportParams"]').click(showReportParams);
    
    var submitWithLoader = function () {
        var $button = $(this);
        var $form = $button.parents("form").first();
        $("#loading").show();
        $form.submit();
        
    }
    $('#btnSubmitWithShowLoading').click(submitWithLoader);

    var showEmailDiv = function () {
        $("#enterEmailDiv").removeClass("hidden");
    }
    $('#showEmailDiv').click(showEmailDiv);

    var showSelectDriverDiv = function () {
        $("#selectDriverDiv").toggle();
    }
    $('#showDriverDiv').click(showSelectDriverDiv);


    var count = 0;
    var showAnotherDropField = function () {
        if (count == 0 || count == 'undefined')
        { count = 1; }
        else
        { count = count + 1; }
        if (count < 5) {
            var elementName = "#address" + count;
            var element = $(elementName);
            element.removeClass('VisiblityHidden');
            var parent = element.parent('div');
            parent.removeClass('VisiblityHidden');
        }
        return false;

    };
    $("#addField").click(showAnotherDropField);

    var contactCount = 0;
    var showAnotherContactNp = function () {
        if (contactCount == 0 || contactCount == 'undefined')
        { contactCount = 1; }
        else
        { contactCount = contactCount + 1; }
        if (contactCount < 3) {
            var elementName = "#contact" + contactCount;
            var element = $(elementName);
            element.removeClass('VisiblityHidden');
            var parent = element.parent('div');
            parent.removeClass('VisiblityHidden');
        }
        return false;

    };
    $("#addContactNo").click(showAnotherContactNp);

    $("#hourlyDriverCheck").click(function (event) {
        var commissionField = $("#txtCommissionRate");
        if ($(event.target).is(":checked") == true) {
            commissionField.val('0.00');
            commissionField.attr("disabled", "disabled");
        }
        else {
            commissionField.val('');
            commissionField.removeAttr("disabled");
        }
    });
});
