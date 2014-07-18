$(document).ready(function () {

    $('#submitted_panel').dialog('close');
    $('#confirmed_panel').dialog('close');
    $('#cleared_panel').dialog('close');
    $('#reimburse_pnl').dialog('close');
    $('#reimburse_detail_pnl').dialog('close');

    function ClearErrorMessage() {
        $('span[class=errormessage]').text('').remove();
    }

    /*================================================ Reimburse List ================================================*/
    $("#list_reimburse").jqGrid({
        url: base_url + 'Reimburse/GetListReimburse',
        datatype: "json",
        colNames: ['Name', 'Description', 'Total', 'Submit', 'Submitted Date', 'Confirm', 'Confirmed Date', 'Clear', 'Cleared Date',
				'Actual Paid', 'Created Date'],
        colModel: [
                { name: 'name', index: 'name', width: 100, align: "center" },
				  { name: 'description', index: 'description', width: 150 },
				  { name: 'total', index: 'total', width: 80, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
				  { name: 'issubmitted', index: 'issubmitted', width: 60, align: "center" },
				  { name: 'submitteddate', index: 'submitteddate', hidden: true, width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'isconfirmed', index: 'isconfirmed', width: 60, align: "center" },
				  { name: 'confirmeddate', index: 'confirmeddate', hidden: true, width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'iscleared', index: 'iscleared', width: 60, align: "center" },
				  { name: 'clearancedate', index: 'clearancedate', hidden: true, width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'actualpaid', index: 'actualpaid', width: 80, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
				  { name: 'entrydate', index: 'createdon', hidden: true, width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "M d, Y" } }
        ],
        page: 'last', // last page
        pager: jQuery('#pager_list_reimburse'),
        altRows: true,
        rowNum: 50,
        rowList: [50, 100, 150],
        imgpath: 'themes/start/images',
        sortname: 'id',
        viewrecords: true,
        shrinkToFit: false,
        sortorder: "asc",
        onSelectRow: function (id) {
            var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
            if (id) {
                var lookUpURL = base_url + 'Reimburse/GetListReimburseDetail';
                var lookupGrid = $('#list_reimburse_detail');

                lookupGrid.setGridParam({
                    postData: {
                        'reimburseid': function () { return id; }
                    },
                    url: lookUpURL
                }).trigger("reloadGrid");
            } else {
                $.messager.alert('Information', 'Please Select Master Reimburse First...!!', 'info');
            };
        },
        //width: $(window).width(),
        //height: $(window).height() - 200,
        gridComplete:
		  function () {
		      var ids = $(this).jqGrid('getDataIDs');
		      for (var i = 0; i < ids.length; i++) {
		          var cl = ids[i];
		          rowIsSubmitted = $(this).getRowData(cl).issubmitted;
		          if (rowIsSubmitted == 'true') {
		              rowIsSubmitted = "YES";
		          } else {
		              rowIsSubmitted = "NO";
		          }
		          $(this).jqGrid('setRowData', ids[i], { issubmitted: rowIsSubmitted });

		          rowIsConfirmed = $(this).getRowData(cl).isconfirmed;
		          if (rowIsConfirmed == 'true') {
		              rowIsConfirmed = "YES";
		          } else {
		              rowIsConfirmed = "NO";
		          }
		          $(this).jqGrid('setRowData', ids[i], { isconfirmed: rowIsConfirmed });

		          rowIsCleared = $(this).getRowData(cl).iscleared;
		          if (rowIsCleared == 'true') {
		              rowIsCleared = "YES";
		          } else {
		              rowIsCleared = "NO";
		          }
		          $(this).jqGrid('setRowData', ids[i], { iscleared: rowIsCleared });
		      }
		  }
    });//END GRID
    $("#list_reimburse").jqGrid('navGrid', '#toolbar_trans_epl', { del: false, add: false, edit: false, search: false })
           .jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false });

    /*================================================ End Reimburse List ================================================*/

    var reimburseEdit = 0;
    var reimburseId = 0;
    $("#reimburse_btn_add_new").click(function () {
        //window.location = base_url + "reimburse/detail";

        reimburseId = 0;
        reimburseEdit = 0;
        ClearData();
        $("#reimburse_btn_save").data('kode', 0);
        $('#reimburse_pnl').dialog('open');
    });

    $("#reimburse_btn_edit").click(function () {
        ClearData();
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse").jqGrid('getRowData', id);

            reimburseEdit = 1;
            $("#reimburse_btn_save").data('kode', id);
            $("#txtDescription").val(ret.description);

            $('#reimburse_pnl').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });

    $("#reimburse_btn_save").click(function () {

        ClearErrorMessage();

        var submitURL = '';
        var id = $("#reimburse_btn_save").data('kode');

        // Update
        if (id != undefined && id != '' && !isNaN(id) && id > 0) {
            submitURL = base_url + 'Reimburse/Update';
        }
            // Insert
        else {
            submitURL = base_url + 'Reimburse/Insert';
        }

        $.ajax({
            contentType: "application/json",
            type: 'POST',
            url: submitURL,
            data: JSON.stringify({
                Id: id, Description: $("#txtDescription").val()
            }),
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
                return false;
            },
            success: function (result) {
                if (result.model.Errors != null) {
                    for (var key in result.model.Errors) {
                        if (key != null && key != undefined && key != 'Generic') {
                            $('input[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                            $('textarea[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                        }
                        else {
                            $.messager.alert('Warning', result.model.Errors[key], 'warning');
                        }
                    }
                }
                else {
                    ReloadReimburse();
                }
            }
        });
    });

    $("#reimburse_btn_close").click(function () {
        $('#reimburse_pnl').dialog('close');
    });

    $("#reimburse_btn_del").click(function () {
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/Delete',
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
                            ReloadReimburse();
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

    function ClearData() {
        $('#txtDescription').val('').text('').removeClass('errormessage');
        $('#reimburse_btn_save').data('kode', '');

        ClearErrorMessage();
    }

    function ReloadReimburse() {
        $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
    }

    $("#reimburse_btn_reload").click(function () {
        ReloadReimburse();
    });

    // ------------------------------------------------------------ Submitted Reimburse ------------------------------------------------------------ 
    $("#reimburse_btn_submit").click(function () {
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse").jqGrid('getRowData', id);
            if (ret.issubmitted == 'YES') {
                $('#txtSubmitDate').datebox('setValue', ret.submitteddate);
            }
            else {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!
                var yyyy = today.getFullYear();
                $('#txtSubmitDate').datebox('setValue', mm + '/' + dd + '/' + yyyy);
            }

            $('#submitted_panel').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });

    $("#submitted_btn_close").click(function () {
        $('#submitted_panel').dialog('close');
    });

    $("#submitted_btn_save").click(function () {

        ClearErrorMessage();

        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/Submit',
                data: JSON.stringify({
                    Id: id, SubmittedDate: $('#txtSubmitDate').datebox('getValue')
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    if (result.model.Errors != null) {
                        for (var key in result.model.Errors) {
                            if (key != null && key != undefined && key != 'Generic') {
                                $('input[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                                $('textarea[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                            }
                            else {
                                $.messager.alert('Warning', result.model.Errors[key], 'warning');
                            }
                        }
                    }
                    else {
                        ReloadReimburse();
                        $('#submitted_panel').dialog('close');
                    }
                }
            });
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });
    // ------------------------------------------------------------ END Submitted Reimburse ------------------------------------------------------------


    // ------------------------------------------------------------ Confirmed Reimburse ------------------------------------------------------------
    $("#reimburse_btn_confirm").click(function () {
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse").jqGrid('getRowData', id);
            if (ret.isconfirmed == 'YES') {
                $('#txtConfirmDate').datebox('setValue', ret.confirmeddate);
            }
            else {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!
                var yyyy = today.getFullYear();
                $('#txtConfirmDate').datebox('setValue', mm + '/' + dd + '/' + yyyy);
            }

            $('#confirmed_panel').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });

    $("#confirmed_btn_close").click(function () {
        $('#confirmed_panel').dialog('close');
    });

    $("#confirmed_btn_save").click(function () {

        ClearErrorMessage();

        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/Confirm',
                data: JSON.stringify({
                    Id: id, ConfirmedDate: $('#txtConfirmDate').datebox('getValue')
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    if (result.model.Errors != null) {
                        for (var key in result.model.Errors) {
                            if (key != null && key != undefined && key != 'Generic') {
                                $('input[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                                $('textarea[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                            }
                            else {
                                $.messager.alert('Warning', result.model.Errors[key], 'warning');
                            }
                        }
                    }
                    else {
                        ReloadReimburse();
                        $('#confirmed_panel').dialog('close');
                    }
                }
            });
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });
    // ------------------------------------------------------------ END Confirmed Reimburse ------------------------------------------------------------


    // ------------------------------------------------------------ Cleared Reimburse ------------------------------------------------------------
    $("#reimburse_btn_clear").click(function () {
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse").jqGrid('getRowData', id);
            if (ret.iscleared == 'YES') {
                $('#txtClearDate').datebox('setValue', ret.clearancedate);
            }
            else {
                var today = new Date();
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!
                var yyyy = today.getFullYear();
                $('#txtClearDate').datebox('setValue', mm + '/' + dd + '/' + yyyy);
            }

            $('#cleared_panel').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });

    $("#cleared_btn_close").click(function () {
        $('#cleared_panel').dialog('close');
    });

    $("#cleared_btn_save").click(function () {

        ClearErrorMessage();

        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/Clear',
                data: JSON.stringify({
                    Id: id, ClearanceDate: $('#txtClearDate').datebox('getValue')
                }),
                async: false,
                cache: false,
                timeout: 30000,
                error: function () {
                    return false;
                },
                success: function (result) {
                    if (result.model.Errors != null) {
                        for (var key in result.model.Errors) {
                            if (key != null && key != undefined && key != 'Generic') {
                                $('input[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                                $('textarea[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                            }
                            else {
                                $.messager.alert('Warning', result.model.Errors[key], 'warning');
                            }
                        }
                    }
                    else {
                        ReloadReimburse();
                        $('#cleared_panel').dialog('close');
                    }
                }
            });
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });
    // ------------------------------------------------------------ Cleared Reimburse ------------------------------------------------------------


    /*================================================ Reimburse Detail List ================================================*/
    $("#list_reimburse_detail").jqGrid({
        url: base_url + 'index.html',
        datatype: "json",
        colNames: ['Description', 'Amount', 'Rejected', 'Expense Date'],
        colModel: [
				  { name: 'description', index: 'description', width: 200 },
				  { name: 'amount', index: 'total', width: 100, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
				  { name: 'rejected', index: 'rejected', width: 80, align: "center" },
				  { name: 'expensedate', index: 'expensedate', width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } }
        ],
        page: 'last', // last page
        pager: jQuery('#pager_list_reimburse_detail'),
        altRows: true,
        rowNum: 50,
        rowList: [50, 100, 150],
        imgpath: 'themes/start/images',
        sortname: 'id',
        viewrecords: true,
        shrinkToFit: false,
        sortorder: "asc",
        gridComplete:
		  function () {
		      var ids = $(this).jqGrid('getDataIDs');
		      for (var i = 0; i < ids.length; i++) {
		          var cl = ids[i];
		          rowIsRejected = $(this).getRowData(cl).rejected;
		          if (rowIsRejected == 'true') {
		              rowIsRejected = "YES";
		          } else {
		              rowIsRejected = "NO";
		          }
		          $(this).jqGrid('setRowData', ids[i], { rejected: rowIsRejected });
		      }
		  }
    });//END GRID
    $("#list_reimburse_detail").jqGrid('navGrid', '#toolbar_trans_epl', { del: false, add: false, edit: false, search: false })
           .jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false });

    /*================================================ End Reimburse Detail List ================================================*/


    /*================================================ Reimburse Detail ================================================*/

    var reimburseDetailEdit = 0;
    $("#reimburse_detail_btn_add_new").click(function () {
        //window.location = base_url + "reimburse/detail";

        ClearDataDetail();
        reimburseDetailEdit = 0;
        $("#reimburse_detail_btn_save").data('kode', 0);
        $('#reimburse_detail_pnl').dialog('open');
    });

    $("#reimburse_detail_btn_edit").click(function () {
        ClearDataDetail();
        var id = jQuery("#list_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse_detail").jqGrid('getRowData', id);

            reimburseDetailEdit = 1;
            $("#reimburse_detail_btn_save").data('kode', id);
            $("#txtDescReimburseDetail").val(ret.description);
            $("#txtAmountReimburseDetail").numberbox('setValue', ret.amount);
            $("#txtExpenseDateReimburseDetail").datebox('setValue', ret.expensedate);

            $('#reimburse_detail_pnl').dialog('open');
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });

    $("#reimburse_detail_btn_save").click(function () {

        ClearErrorMessage();

        var reimburseid = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (!reimburseid) {
            $.messager.alert('Information', 'Please Select Master Reimburse First...!!', 'info');
            return;
        };

        var submitURL = '';
        var id = $("#reimburse_detail_btn_save").data('kode');

        // Update
        if (id != undefined && id != '' && !isNaN(id) && id > 0) {
            submitURL = base_url + 'Reimburse/UpdateReimburseDetail';
        }
            // Insert
        else {
            submitURL = base_url + 'Reimburse/InsertReimburseDetail';
        }

        $.ajax({
            contentType: "application/json",
            type: 'POST',
            url: submitURL,
            data: JSON.stringify({
                Id: id, ReimburseId: reimburseid,
                Description: $("#txtDescReimburseDetail").val(),
                ExpenseDate: $("#txtExpenseDateReimburseDetail").datebox('getValue'),
                Amount: $("#txtAmountReimburseDetail").numberbox('getValue')
            }),
            async: false,
            cache: false,
            timeout: 30000,
            error: function () {
                return false;
            },
            success: function (result) {
                if (result.model.Errors != null) {
                    for (var key in result.model.Errors) {
                        if (key != null && key != undefined && key != 'Generic') {
                            $('input[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                            $('textarea[name=' + key + ']').addClass('errormessage').after('<span class="errormessage">**' + result.model.Errors[key] + '</span>');
                        }
                        else {
                            $.messager.alert('Warning', result.model.Errors[key], 'warning');
                        }
                    }
                }
                else {
                    ReloadReimburse();
                }
            }
        });
    });

    $("#reimburse_detail_btn_close").click(function () {
        $('#reimburse_detail_pnl').dialog('close');
    });

    $("#reimburse_detail_btn_del").click(function () {
        var id = jQuery("#list_reimburse_detail").jqGrid('getGridParam', 'selrow');
        if (id) {
            $.ajax({
                contentType: "application/json",
                type: 'POST',
                url: base_url + 'Reimburse/DeleteReimburseDetail',
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
                            ReloadReimburseDetail();
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

    function ClearDataDetail() {
        $('#txtDescReimburseDetail').val('').text('').removeClass('errormessage');
        $('#txtAmountReimburseDetail').numberbox('setValue', 0).removeClass('errormessage');
        $('#txtExpenseDateReimburseDetail').datebox('setValue', '').removeClass('errormessage');
        $('#reimburse_detail_btn_save').data('kode', '');

        ClearErrorMessage();
    }

    function ReloadReimburseDetail() {
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var lookUpURL = base_url + 'Reimburse/GetListReimburseDetail';
            var lookupGrid = $('#list_reimburse_detail');

            lookupGrid.setGridParam({
                postData: {
                    'reimburseid': function () { return id; }
                },
                url: lookUpURL
            }).trigger("reloadGrid");
        } else {
            $.messager.alert('Information', 'Please Select Master Reimburse First...!!', 'info');
        };
    }

    $("#reimburse_detail_btn_reload").click(function () {
        ReloadReimburseDetail();
    });

    /*================================================ End Reimburse Detail ================================================*/

});