$(document).ready(function () {
    $("#product-table").hide();
    var visibleLocations = [];

    $('.checkbox').change(function () {
        var selectedValues = [];
        visibleLocations = [];
        $('.checkbox:checked').each(function () {
            selectedValues.push($(this).val());
        });

        if (selectedValues.length != 0) {
            console.log(selectedValues, selectedValues.length)
        }

        $("#product-table tbody tr.filterable").each(function () {
            if ($.inArray($(this).attr("data-id"), selectedValues) !== -1) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });

        var noItems = false;
        $('.checkbox:checked').each(function () {
            var checkboxId = $(this).val();
            var itemsCount = $("#product-table tr.filterable[data-id='" + checkboxId + "']").length;
            if (itemsCount === 0) {
                noItems = true;
                var rowExists = $("#product-table tbody tr[data-checkbox='" + checkboxId + "']").length > 0;
                if (!rowExists) {
                    $("#product-table tbody").append(`<tr data-checkbox="${checkboxId}"><td colspan='4'>There are no items for ${$(this).parent().text().trim()}.</td></tr>`);
                }
            } else {
                $("#product-table tbody tr[data-checkbox='" + checkboxId + "']").remove();
            }
        });

        $("#product-table tbody tr.filterable").each(function () {
            if ($.inArray($(this).attr("data-id"), selectedValues) !== -1) {
                $(this).show();
                // If the row is visible, add its location to the array
                visibleLocations.push($(this).find("input[name='locations']").val());
            } else {
                $(this).hide();
            }
        });

        // Hide the table header if no items are selected
        if (selectedValues.length == 0) {
            $("#product-table thead").hide();
        } else {
            $("#product-table thead").show();
        }

        let uniqueArr = [...new Set(visibleLocations)];

        // Hide the "no items" row if the checkbox is unchecked
        $('.checkbox:not(:checked)').each(function () {
            var checkboxId = $(this).val();
            $("#product-table tbody tr[data-checkbox='" + checkboxId + "']").remove();
        });
        $('.hidden-locations-input').val(uniqueArr.join(','));
        //$('.hidden-dataInfo-input').val(JSON.stringify(dictionary));
        //console.log(dictionary);
        $("#product-table").show();
    });
});