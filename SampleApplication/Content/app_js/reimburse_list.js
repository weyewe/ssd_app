$(document).ready(function () {

    $('#submitted_panel').dialog('close');
    $('#confirmed_panel').dialog('close');
    $('#cleared_panel').dialog('close');

    /*================================================ Reimburse List ================================================*/
    $("#list_reimburse").jqGrid({
        url: base_url + 'Reimburse/GetListReimburse',
        datatype: "json",
        colNames: ['Name', 'Description', 'Total', 'Submitted', 'Submitted Date', 'Confirmed', 'Confirmed Date', 'Cleared', 'Cleared Date',
				'Actual Paid', 'Created Date'],
        colModel: [
                { name: 'name', index: 'name', width: 130, align: "center" },
				  { name: 'description', index: 'description', width: 200 },
				  { name: 'total', index: 'total', width: 100, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
				  { name: 'issubmitted', index: 'issubmitted', width: 80, align: "center" },
				  { name: 'submitteddate', index: 'submitteddate', width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'isconfirmed', index: 'isconfirmed', width: 80, align: "center" },
				  { name: 'confirmeddate', index: 'confirmeddate', width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'iscleared', index: 'iscleared', width: 80, align: "center" },
				  { name: 'clearancedate', index: 'clearancedate', width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "m/d/Y" } },
				  { name: 'actualpaid', index: 'actualpaid', width: 130, align: "right", formatter: 'currency', formatoptions: { decimalSeparator: ".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: '0.00' } },
				  { name: 'entrydate', index: 'createdon', width: 100, align: "center", formatter: 'date', formatoptions: { srcformat: "Y-m-d", newformat: "M d, Y" } }
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
        width: $("#reimburse_toolbar").width(),
        height: $(window).height() - 200,
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

    $("#reimburse_btn_reload").click(function () {
        $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
    });

    $("#reimburse_btn_add_new").click(function () {
        window.location = base_url + "reimburse/detail";
    });

    $("#reimburse_btn_edit, #reimburse_btn_del").click(function () {

        var buttonID = $(this).attr('id');
        var id = jQuery("#list_reimburse").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#list_reimburse").jqGrid('getRowData', id);

            // On Edit Mode
            if (buttonID == "reimburse_btn_edit") {
                window.location = base_url + "Reimburse/Detail?Id=" + id;
            }
            // On Delete Mode
            if (buttonID == "reimburse_btn_del") {
                DeleteReimburseDetail(id);
            }
        } else {
            $.messager.alert('Information', 'Please Select Data...!!', 'info');
        };
    });


    // Delete Reimburse Detail
    function DeleteReimburseDetail(v_reimbursedetailId) {
        $.ajax({
            contentType: "application/json",
            type: 'POST',
            url: base_url + 'Reimburse/Delete',
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
                if (result.isValid) {
                    $.messager.alert('Information', result.message, 'info', function () {
                        // Reload
                        $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
                    });
                }
                else {
                    $.messager.alert('Warning', result.message, 'warning');
                }
            }
        });
    }
    // END Delete Reimburse Detail

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
                    if (result.isValid) {
                        $.messager.alert('Information', result.message, 'info', function () {

                            $('#submitted_panel').dialog('close');

                            // Reload
                            $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
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
                    if (result.isValid) {
                        $.messager.alert('Information', result.message, 'info', function () {

                            $('#confirmed_panel').dialog('close');

                            // Reload
                            $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
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
                    if (result.isValid) {
                        $.messager.alert('Information', result.message, 'info', function () {

                            $('#cleared_panel').dialog('close');

                            // Reload
                            $("#list_reimburse").setGridParam({ url: base_url + 'Reimburse/GetListReimburse', postData: { filters: null }, page: 'last' }).trigger("reloadGrid");
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
    // ------------------------------------------------------------ Cleared Reimburse ------------------------------------------------------------

});