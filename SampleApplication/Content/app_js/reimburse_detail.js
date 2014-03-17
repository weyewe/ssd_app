$(document).ready(function () {
    var vReimburseDetailEdit = 0;
    var ReimburseId = 0;
    $('#lookup_div_reimburse_detail').dialog('close');

    var ReimburseId = getQueryStringByName('Id');

    // Edit Mode
    if (ReimburseId != "") {
        FirstLoad();
    }
    // End First Load

    function FirstLoad() {
        $.ajax({
            dataType: "json",
            url: base_url + "Reimburse/GetReimburseInfo?Id=" + ReimburseId,
            success: function (result) {
                if (!result.isValid)
                    $.messager.alert('Information', result.message, 'info', function () {
                        window.location = base_url + "Reimburse";
                    });
                else {
                    $("#txtDescription").val(result.objJob.Description);
                    $("#txtTotal").numberbox('setValue', result.objJob.Total);
                    $("#txtActualPaid").numberbox('setValue', result.objJob.ActualPaid);

                    if (result.objJob.ListDetail != null) {
                        for (var i = 0; i < result.objJob.ListDetail.length; i++) {
                            AddReimburseDetail(0, result.objJob.ListDetail[i].Id, result.objJob.ListDetail[i].Description,
                                result.objJob.ListDetail[i].Amount, result.objJob.ListDetail[i].IsRejected, dateEnt(result.objJob.ListDetail[i].ExpenseDate));
                        }
                    }

                    TotalCalculation();
                }
            }
        });
    }

    // Add
    $('#btn_add_reimburse_detail').click(function () {
        vReimburseDetailEdit = 0;
        ClearDetail();
        $('#lookup_btn_add_reimburse_detail').data('reimbursedetailid', 0);
        $('#lookup_div_reimburse_detail').dialog('open');
    });

    // Edit
    $('#btn_edit_reimburse_detail').click(function () {

        var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id) {

            vReimburseDetailEdit = 1;

            var ret = jQuery("#table_reimburse_detail").jqGrid('getRowData', id);

            $('#lookup_btn_add_reimburse_detail').data('reimbursedetailid', id);
            $('#lookup_btn_add_reimburse_detail').data('isrejected', ret.isrejected);

            $('#txtDescReimburseDetail').val(ret.description);
            $('#txtAmountReimburseDetail').numberbox('setValue', ret.amount);
            $('#txtExpenseDateReimburseDetail').datebox('setValue', ret.expensedate);

            $('#lookup_div_reimburse_detail').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        }
    });

    // Delete
    $('#btn_delete_reimburse_detail').click(function () {

        var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id) {
            $('#lookup_btn_add_reimburse_detail').data('reimbursedetailid', 0);
            var isValid = false;
            var message = "";
            DeleteReimburseDetail(id, function (data) {
                isValid = data.isValid;
                message = data.message
            });

            if (!isValid) {
                $.messager.alert('Warning', message, 'warning');
            }
            else {

                jQuery("#table_reimburse_detail").jqGrid('delRowData', id);
            }
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        }

        TotalCalculation();
    });

    // Save
    $('#lookup_btn_add_reimburse_detail').click(function () {

        // Assigned Reimburse Detail Id
        var reimburseDetailId = 0
        if (vReimburseDetailEdit == 1) {
            reimburseDetailId = $('#lookup_btn_add_reimburse_detail').data('reimbursedetailid');
        }

        // ONLY Update Reimburse
        if ((ReimburseId != undefined && ReimburseId != "")) {
            var submitURL = (ReimburseId != undefined && ReimburseId != "") ? (vReimburseDetailEdit == 0) ? base_url + "Reimburse/InsertReimburseDetail" : base_url + "Reimburse/UpdateReimburseDetail" : "";
            var isValid = true;
            var message = "OK";

            var isrejected = $('#lookup_btn_add_reimburse_detail').data('isrejected');
            // On Add
            if (vReimburseDetailEdit == 0)
                isrejected = 0;

            // Add or Edit
            SaveReimburseDetail(submitURL, reimburseDetailId, $('#txtDescReimburseDetail').val(), $('#txtAmountReimburseDetail').numberbox('getValue'),
                            isrejected, $('#txtExpenseDateReimburseDetail').datebox("getValue"), function (data) {
                                isValid = data.isValid;
                                message = data.message;
                                if (data.objResult != null)
                                    reimburseDetailId = data.objResult.ReimburseDetailId;
                            });

            if (!isValid) {
                $.messager.alert('Warning', message, 'warning');
            }
            else {
                AddReimburseDetail(vReimburseDetailEdit, reimburseDetailId, $('#txtDescReimburseDetail').val(), $('#txtAmountReimburseDetail').numberbox('getValue'),
                            isrejected, $('#txtExpenseDateReimburseDetail').datebox("getValue"));
            }
        }
        else {
            var isrejected = $('#lookup_btn_add_reimburse_detail').data('isrejected');
            // On Add
            if (vReimburseDetailEdit == 0)
                isrejected = 0;

            AddReimburseDetail(vReimburseDetailEdit, reimburseDetailId, $('#txtDescReimburseDetail').val(), $('#txtAmountReimburseDetail').numberbox('getValue'),
                        isrejected, $('#txtExpenseDateReimburseDetail').datebox("getValue"));
        }
        ClearDetail();
        TotalCalculation();

        $('#lookup_div_reimburse_detail').dialog('close');
    });

    // Close
    $('#lookup_btn_cancel_reimburse_detail').click(function () {
        $('#lookup_div_reimburse_detail').dialog('close');
    });

    function GetNextId(objJqGrid) {
        var nextid = 0;
        if (objJqGrid.getDataIDs().length > 0)
            nextid = parseInt(objJqGrid.getDataIDs()[objJqGrid.getDataIDs().length - 1]) + 1;

        return nextid;
    }

    function TotalCalculation() {
        var total = 0;
        var actualPaid = 0;
        var data = $("#table_reimburse_detail").jqGrid('getGridParam', 'data');
        for (var i = 0; i < data.length; i++) {

            total += parseFloat(data[i].amount);
            if (data[i].isrejected == "NO")
                actualPaid += parseFloat(data[i].amount);
        }
        total = Math.round(total * 100) / 100;
        $("#txtTotal").numberbox("setValue", total);
        actualPaid = Math.round(actualPaid * 100) / 100;
        $("#txtActualPaid").numberbox("setValue", actualPaid);
    }

    function ClearDetail() {
        $('#txtDescReimburseDetail').val('');
        $('#txtAmountReimburseDetail').numberbox('setValue', 0);

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        $('#txtExpenseDateReimburseDetail').datebox('setValue', mm + '/' + dd + '/' + yyyy);
    }

    function AddReimburseDetail(isEdit, v_reimbursedetailid, v_description, v_amount, v_isrejected, v_expensedate) {
        var newData = {};
        newData.description = v_description;
        newData.amount = v_amount;
        newData.isrejected = v_isrejected;
        newData.expensedate = v_expensedate;

        if (isEdit == 0) {
            v_reimbursedetailid = v_reimbursedetailid == 0 ? GetNextId($("#table_reimburse_detail")) : v_reimbursedetailid;
            jQuery("#table_reimburse_detail").jqGrid('addRowData', v_reimbursedetailid, newData);
        }
        else {
            //  Edit
            var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
            if (id) {
                jQuery("#table_reimburse_detail").jqGrid('setRowData', id, newData);
            }
        }

        // Reload
        $("#table_reimburse_detail").trigger("reloadGrid");
    }

    jQuery("#table_reimburse_detail").jqGrid({
        datatype: "local",
        height: 120,
        rowNum: 150,
        colNames: ['Description', 'Amount', 'Rejected', 'Expense Date'],
        colModel: [
            { name: 'description', index: 'description', width: 285 },
            { name: 'amount', index: 'amount', width: 120, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
            { name: 'isrejected', index: 'isrejected', align: 'center', width: 60 },
            { name: 'expensedate', index: 'expensedate', align: 'right', width: 100 }
        ],
        gridComplete:
        function () {
            var ids = $(this).jqGrid('getDataIDs');
            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];
                rowIsRejected = $(this).getRowData(cl).isrejected;
                if (rowIsRejected == 'true' || rowIsRejected == true || rowIsRejected == "YES") {
                    rowIsRejected = "YES";
                } else {
                    rowIsRejected = "NO";
                }
                $(this).jqGrid('setRowData', ids[i], { isrejected: rowIsRejected });
            }
        }
    });//END GRID
    $("#table_reimburse_detail").jqGrid('navGrid', '#toolbar_trans_epl', { del: false, add: false, edit: false, search: false })
           .jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false });

    // ------------------------------------------------------------------------ SAVE
    $('#reimburse_form_btn_save').click(function () {
        // Reimburse Detail
        var ReimburseDetail = PopulateReimburseDetail();
        // END Reimburse Detail

        var submitURL = base_url + "Reimburse/Insert";
        if (ReimburseId != undefined && ReimburseId != 0)
            submitURL = base_url + "Reimburse/Update";

        $.ajax({
            contentType: "application/json",
            type: 'POST',
            url: submitURL,
            data: JSON.stringify({
                Id: ReimburseId,
                Description: $('#txtDescription').val(),
                Total: $('#txtTotal').numberbox('getValue'),
                ActualPaid: $('#txtActualPaid').numberbox('getValue'),
                ListDetail: ReimburseDetail
            }),
            success: function (result) {
                if (result.isValid) {
                    $.messager.alert('Information', result.message, 'info', function () {
                        window.location = base_url + "Reimburse/detail?Id=" + result.objResult.Id;
                    });
                }
                else {
                    $.messager.alert('Warning', result.message, 'warning');
                }
            }
        });
    });

    // Reimburse Detail
    function PopulateReimburseDetail() {
        var ReimburseDetail = [];
        var data = $("#table_reimburse_detail").jqGrid('getGridParam', 'data');
        for (var i = 0; i < data.length; i++) {
            var obj = {};
            obj['Description'] = data[i].description;
            obj['Amount'] = data[i].amount;
            obj['IsRejected'] = data[i].isrejected;
            obj['ExpenseDate'] = data[i].expensedate;
            ReimburseDetail.push(obj);
        }
        return ReimburseDetail
    }
    // END Reimburse Detail


    // Save Reimburse Detail
    function SaveReimburseDetail(submitURL, v_reimbursedetailId, v_description, v_amount, v_isrejected, v_expensedate, callback) {
        if (submitURL != undefined && submitURL != "") {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: submitURL,
                data: JSON.stringify({
                    Id: v_reimbursedetailId, ReimburseId: ReimburseId,
                    Description: v_description, Amount: v_amount,
                    IsRejected: v_isrejected, ExpenseDate: v_expensedate
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    callback(result);
                }
            });
        }
    }
    // END Save Reimburse Detail

    // Delete Reimburse Detail
    function DeleteReimburseDetail(v_reimbursedetailId, callback) {
        $.ajax({
            contentType: "application/json",
            type: 'POST',
            url: base_url + 'Reimburse/DeleteReimburseDetail',
            data: JSON.stringify({
                Id: v_reimbursedetailId
            }),
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
                return false;
            },
            success: function (result) {
                callback(result);
            }
        });
    }
    // END Delete Reimburse Detail

    // Reject ONLY Update Reimburse
    $("#reimbursedetail_btn_reject").click(function () {
        var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id && (ReimburseId != undefined && ReimburseId != "")) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/RejectReimburseDetail',
                data: JSON.stringify({
                    Id: id
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    if (result.isValid) {
                        $.messager.alert('Information', result.message, 'info', function () {

                            // Update List
                            var newData = {};
                            newData.isrejected = 1;
                            var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
                            if (id) {
                                jQuery("#table_reimburse_detail").jqGrid('setRowData', id, newData);
                            }
                            // Reload
                            $("#table_reimburse_detail").trigger("reloadGrid");

                            TotalCalculation();
                        });
                    }
                    else {
                        $.messager.alert('Warning', result.message, 'warning');
                    }
                }
            });
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });
    // END Reject ONLY Update Reimburse

    // Un-Reject ONLY Update Reimburse
    $("#reimbursedetail_btn_unreject").click(function () {
        var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id && (ReimburseId != undefined && ReimburseId != "")) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/UnRejectReimburseDetail',
                data: JSON.stringify({
                    Id: id
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    if (result.isValid) {
                        $.messager.alert('Information', result.message, 'info', function () {

                            // Update List
                            var newData = {};
                            newData.isrejected = 0;
                            var id = jQuery("#table_reimburse_detail").jqGrid('getGridParam', 'selrow');
                            if (id) {
                                jQuery("#table_reimburse_detail").jqGrid('setRowData', id, newData);
                            }

                            // Reload
                            $("#table_reimburse_detail").trigger("reloadGrid");

                            TotalCalculation();
                        });
                    }
                    else {
                        $.messager.alert('Warning', result.message, 'warning');
                    }
                }
            });
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });
    // END Un-Reject ONLY Update Reimburse

});